using System;
using System.Collections.Generic;
using System.Threading;

namespace ServerCore.CustomCommands
{
    public class PrefabReplaceBlock : ConsoleCmdAbstract
    {

        private static Dictionary<int, Vector3i> location = new Dictionary<int, Vector3i>();

        public override string getDescription()
        {
            return "Replace blocks from a defined location";
        }

        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. brepblock <block_to_be_replaced> <block_name> <x1> <x2> <y1> <y2> <z1> <z2>\n" +
                "  2. brepblock <block_to_be_replaced> <block_name> <x>@<qnt> <y>@<qnt> <z>@<qnt>\n" +
                "  3. brepblock <block_to_be_replaced> <block_name> <qnt> <qnt> <qnt>\n" +
                "  4. brepblock <block_name>\n" +
                "  5. brepblock p1\n" +
                "  6. brepblock p2 <block_to_be_replaced_name or id> <block_name>\n" +
                "1. replace blocks block_to_be_replaced with block_name from x1,y1,z1 to x2,y2,z2\n" +
                "2. replace blocks block_to_be_replaced with block_name from x,y,z each quantity. Quantity can be posivite or negative.\n" +
                "3. replace blocks block_to_be_replaced with block_name from your position each quantity. Quantity can be posivite or negative.\n" +
                "4. Search for block names. Fill with * to list all.\n" +
                "5. Store your position to be used on method 6.\n" +
                "6. replace blocks block_to_be_replaced with block_name from position stored on method 5 until your current location.\n";

        }

        public override string[] getCommands()
        {
            return new[] { "pc-brepblock", "brepblock" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {

            try
            {
                if (_params.Count != 1 && _params.Count != 3 && _params.Count != 5 && _params.Count != 6 && _params.Count != 8 && _params.Count != 9)
                {
                    SdtdConsole.Instance.Output("ERR: Wrong number of arguments, expected 1 or 3 or 5,6 or 8,9 found " + _params.Count + ".");
                    SdtdConsole.Instance.Output(GetHelp());
                    return;
                }
                else
                {
                    int x1 = int.MinValue;
                    int y1 = int.MinValue;
                    int z1 = int.MinValue;

                    int x2 = int.MinValue;
                    int y2 = int.MinValue;
                    int z2 = int.MinValue;

                    int rot = 0;

                    int nextStoreX = 0;
                    int nextStoreY = 0;
                    int nextStoreZ = 0;

                    string blockName = "";
                    string blockNameToBeReplace = "";

                    if (_params.Count == 1)
                    {
                        if (_params[0].ToLower().Equals("p1"))
                        {
                            ClientInfo ci = _senderInfo.RemoteClientInfo;
                            if (ci == null)
                            {
                                SdtdConsole.Instance.Output("ERR: Unable to get your position");
                                return;

                            }
                            EntityPlayer ep = GameManager.Instance.World.Players.dict[ci.entityId];
                            if (ep == null)
                            {
                                SdtdConsole.Instance.Output("ERR: Unable to get your position");
                                return;

                            }
                            if (location.ContainsKey(ci.entityId))
                            {
                                location.Remove(ci.entityId);
                            }
                            location.Add(ci.entityId, new Vector3i(ep.GetBlockPosition().x, ep.GetBlockPosition().y, ep.GetBlockPosition().z));
                            SdtdConsole.Instance.Output("Stored position: " + ep.GetBlockPosition().x + " " + ep.GetBlockPosition().y + " " + ep.GetBlockPosition().z);
                            return;
                        }
                        ShowListOfBlocks(_params[0]);
                        return;
                    }
                    else if (_params.Count == 3)
                    {
                        if (!_params[0].ToLower().Equals("p2"))
                        {
                            SdtdConsole.Instance.Output("ERR: Invalid request. param 0 must be p2");
                            SdtdConsole.Instance.Output(GetHelp());
                            return;
                        }

                        ClientInfo ci = _senderInfo.RemoteClientInfo;
                        if (ci == null)
                        {
                            SdtdConsole.Instance.Output("ERR: Unable to get your position");
                            return;

                        }
                        EntityPlayer ep = GameManager.Instance.World.Players.dict[ci.entityId];
                        if (ep == null)
                        {
                            SdtdConsole.Instance.Output("Unable to get your position");
                            return;

                        }
                        if (!location.ContainsKey(ci.entityId))
                        {
                            SdtdConsole.Instance.Output("ERR: There isnt any stored location. Use method 5. to store a position.");
                            SdtdConsole.Instance.Output(GetHelp());
                            return;
                        }
                        Vector3i storedPos;
                        location.TryGetValue(ci.entityId, out storedPos);

                        x1 = storedPos.x;
                        y1 = storedPos.y;
                        z1 = storedPos.z;

                        x2 = ep.GetBlockPosition().x;
                        y2 = ep.GetBlockPosition().y;
                        z2 = ep.GetBlockPosition().z;

                        rot = 0;

                        nextStoreX = x2;
                        nextStoreY = y1;
                        nextStoreZ = z2;
                        blockName = _params[2].ToLower();
                        blockNameToBeReplace = _params[1].ToLower();
                    }

                    if (_params.Count == 8 || _params.Count == 9)
                    {
                        int.TryParse(_params[2], out x1);
                        int.TryParse(_params[4], out y1);
                        int.TryParse(_params[6], out z1);

                        int.TryParse(_params[3], out x2);
                        int.TryParse(_params[5], out y2);
                        int.TryParse(_params[7], out z2);

                        blockName = _params[1].ToLower();
                        blockNameToBeReplace = _params[0].ToLower();
                    }
                    else if (_params.Count == 5 || _params.Count == 6)
                    {
                        string[] xParts = _params[2].Split('@');
                        string[] yParts = _params[3].Split('@');
                        string[] zParts = _params[4].Split('@');

                        blockName = _params[1].ToLower();
                        blockNameToBeReplace = _params[0].ToLower();

                        if (xParts.Length == 1 && yParts.Length == 1 && zParts.Length == 1)
                        {
                            ClientInfo ci = _senderInfo.RemoteClientInfo;
                            if (ci == null)
                            {
                                SdtdConsole.Instance.Output("ERR: Unable to get your position");
                                return;

                            }
                            EntityPlayer ep = GameManager.Instance.World.Players.dict[ci.entityId];
                            if (ep == null)
                            {
                                SdtdConsole.Instance.Output("ERR: Unable to get your position");
                                return;

                            }
                            x1 = ep.GetBlockPosition().x;
                            y1 = ep.GetBlockPosition().y;
                            z1 = ep.GetBlockPosition().z;

                            int.TryParse(_params[2], out x2);
                            int.TryParse(_params[3], out y2);
                            int.TryParse(_params[4], out z2);

                            if (x2 == 0 || y2 == 0 || z2 == 0)
                            {
                                SdtdConsole.Instance.Output("ERR: Quantity can not be 0.");
                                return;
                            }

                            if (x2 == -1) x2 = 1;
                            if (y2 == -1) y2 = 1;
                            if (z2 == -1) z2 = 1;

                            if (x2 < 0) x2 = x1 + x2 + 1;
                            else x2 = x1 + x2 - 1;
                            if (y2 < 0) y2 = y1 + y2 + 1;
                            else y2 = y1 + y2 - 1;
                            if (z2 < 0) z2 = z1 + z2 + 1;
                            else z2 = z1 + z2 - 1;
                        }
                        else if (xParts.Length == 2 && yParts.Length == 2 && zParts.Length == 2)
                        {
                            int.TryParse(xParts[0], out x1);
                            int.TryParse(yParts[0], out y1);
                            int.TryParse(zParts[0], out z1);

                            int.TryParse(xParts[1], out x2);
                            int.TryParse(yParts[1], out y2);
                            int.TryParse(zParts[1], out z2);

                            if (x2 == 0 || y2 == 0 || z2 == 0)
                            {
                                SdtdConsole.Instance.Output("ERR: Quantity can not be 0.");
                                return;
                            }

                            if (x2 == -1) x2 = 1;
                            if (y2 == -1) y2 = 1;
                            if (z2 == -1) z2 = 1;

                            if (x2 < 0) x2 = x1 + x2 + 1;
                            else x2 = x1 + x2 - 1;
                            if (y2 < 0) y2 = y1 + y2 + 1;
                            else y2 = y1 + y2 - 1;
                            if (z2 < 0) z2 = z1 + z2 + 1;
                            else z2 = z1 + z2 - 1;
                        }
                        else
                        {
                            SdtdConsole.Instance.Output("ERR: Invalid Paramaters.");
                            SdtdConsole.Instance.Output(GetHelp());
                            return;
                        }
                    }


                    if (x1 == int.MinValue || y1 == int.MinValue || z1 == int.MinValue || x2 == int.MinValue || y2 == int.MinValue || z2 == int.MinValue)
                    {
                        SdtdConsole.Instance.Output("ERR: At least one of the given coordinates is not a valid integer");
                        SdtdConsole.Instance.Output(GetHelp());
                        return;
                    }

                    //Fix the order of xyz1 xyz2
                    if (x2 < x1)
                    {
                        int val = x1;
                        x1 = x2;
                        x2 = val;
                    }

                    if (y2 < y1)
                    {
                        int val = y1;
                        y1 = y2;
                        y2 = val;
                    }

                    if (z2 < z1)
                    {
                        int val = z1;
                        z1 = z2;
                        z2 = val;
                    }


                    if (rot < 0 || rot > 3)
                    {
                        SdtdConsole.Instance.Output("ERR: Invalid rotation parameter. It need to be 0,1,2 or 3");
                        return;
                    }

                    if (x1 == int.MinValue || y1 == int.MinValue || z1 == int.MinValue || x2 == int.MinValue || y2 == int.MinValue || z2 == int.MinValue)
                    {
                        SdtdConsole.Instance.Output("ERR: At least one of the given coordinates is not a valid integer");
                        return;
                    }

                    bool blockRepFound = false;
                    int blockRepId = -999;
                    bool hasBlockRepId = int.TryParse(blockNameToBeReplace, out blockRepId);
                    BlockValue blockRepV = BlockValue.Air;
                    foreach (Block block in Block.list)
                    {
                        if (block != null && ((block.GetBlockName() != null && block.GetBlockName().ToLower().Equals(blockNameToBeReplace)) || (hasBlockRepId && block.blockID == blockRepId)))
                        {
                            blockRepFound = true;
                            blockRepV = Block.GetBlockValue(block.GetBlockName());
                            break;
                        }
                    }

                    if (!blockRepFound)
                    {
                        SdtdConsole.Instance.Output("ERR: Invalid block name or ID " + blockNameToBeReplace + "");
                        ShowListOfBlocks("*");
                        return;
                    }


                    bool blockFound = false;
                    int blockId = -999;
                    bool hasBlockId = int.TryParse(blockName, out blockId);
                    BlockValue blockV = BlockValue.Air;
                    foreach (Block block in Block.list)
                    {
                        if (block != null && ((block.GetBlockName() != null && block.GetBlockName().ToLower().Equals(blockName)) || (hasBlockId && block.blockID == blockId)))
                        {
                            blockFound = true;
                            blockV = Block.GetBlockValue(block.GetBlockName());
                            break;
                        }
                    }

                    if (!blockFound)
                    {
                        SdtdConsole.Instance.Output("ERR: Invalid block name or ID " + blockName + "");
                        ShowListOfBlocks("*");
                        return;
                    }

                    Vector3i vectori2 = new Vector3i((x2 - x1) + 1, (y2 - y1) + 1, (z2 - z1) + 1);
                    Prefab pref = new Prefab(vectori2);
                    pref.bCopyAirBlocks = true;


                    Dictionary<long, Chunk> dic = new Dictionary<long, Chunk>();
                    for (int k = y1; k <= y2; k++)
                    {
                        for (int i = x1; i <= x2; i++)
                        {
                            for (int j = z1; j <= z2; j++)
                            {
                                BlockValue currentBlock = GameManager.Instance.World.GetBlock(new Vector3i(i, k, j));
                                if (currentBlock.type == blockRepV.type)
                                {
                                    pref.SetBlock(i - x1, k - y1, j - z1, blockV);
                                }
                                else
                                {
                                    pref.SetBlock(i - x1, k - y1, j - z1, currentBlock);
                                }
                                if (GameManager.Instance.World.IsChunkAreaLoaded(i, k, j))
                                {
                                    Chunk c = GameManager.Instance.World.GetChunkFromWorldPos(i, k, j) as Chunk;
                                    if (!dic.ContainsKey(c.Key))
                                    {
                                        dic.Add(c.Key, c);
                                    }
                                }
                                else
                                {
                                    SdtdConsole.Instance.Output("ERR: The blocks are to far away. Chunk not loaded on that area");
                                    return;
                                }
                            }
                        }
                    }

                    pref.RotateY(false, rot);

                    Prefab undo = new Prefab(new Vector3i(pref.size.x, pref.size.y, pref.size.z));
                    undo.bCopyAirBlocks = true;
                    PrefabStorage.RunIncludingStorage(() => undo.copyFromWorld(GameManager.Instance.World, new Vector3i(x1, y1, z1), new Vector3i(x2, y2, z2)));
                    if (_senderInfo.RemoteClientInfo != null)
                    {
                        PrefabUndo.setUndo(_senderInfo.RemoteClientInfo.entityId + "", undo, new Vector3i(x1, y1, z1), -1);
                    }
                    else
                    {
                        PrefabUndo.setUndo("server_", undo, new Vector3i(x1, y1, z1), -1);
                    }

                    pref.CopyIntoLocal(GameManager.Instance.World.ChunkCache, new Vector3i(x1, y1, z1), true, true, FastTags<TagGroup.Global>.none);

                    Thread.Sleep(50);
                    ForceChunkReload.exec(dic);
                    StabilityCalculator stabCalc = new StabilityCalculator();
                    stabCalc.Init(GameManager.Instance.World);

                    for (int j = 0; j < pref.size.x; j++)
                    {
                        for (int k = 0; k < pref.size.z; k++)
                        {
                            for (int m = 0; m < pref.size.y; m++)
                            {
                                BlockValue block = GameManager.Instance.World.GetBlock(x1 + j, y1 + m, z1 + k);
                                if (block.type != BlockValue.Air.type)
                                {
                                    Vector3i _position = new Vector3i(x1 + j, y1 + m, z1 + k);
                                    stabCalc.BlockPlacedAt(_position, false);
                                }
                            }
                        }
                    }
                    stabCalc.Cleanup();
                    stabCalc = null;

                    if (_params.Count == 3)
                    {
                        if (_senderInfo.RemoteClientInfo != null)
                        {
                            location.Remove(_senderInfo.RemoteClientInfo.entityId);
                            location.Add(_senderInfo.RemoteClientInfo.entityId, new Vector3i(nextStoreX, nextStoreY, nextStoreZ));
                            SdtdConsole.Instance.Output("Stored position: " + nextStoreX + " " + nextStoreY + " " + nextStoreZ);
                        }
                    }

                    SdtdConsole.Instance.Output("Block replaced from " + x1 + " " + y1 + " " + z1 + " to " + x2 + " " + y2 + " " + z2);
                }
            }
            catch (Exception e)
            {
                SdtdConsole.Instance.Output("Error in PrefabFillBlock.Run: " + e);
            }
        }

        public void ShowListOfBlocks(string blockName)
        {
            SdtdConsole.Instance.Output("blockName:" + blockName);
            blockName = blockName.ToLower();
            foreach (Block block in Block.list)
            {
                if (blockName == "*")
                {
                    if (block != null && block.GetBlockName() != null)
                        SdtdConsole.Instance.Output("BlockID:" + block.blockID + "   Name:" + block.GetBlockName());
                }
                else if (block != null && block.GetBlockName() != null && block.GetBlockName().ToLower().Contains(blockName))
                    SdtdConsole.Instance.Output("BlockID:" + block.blockID + "   Name:" + block.GetBlockName());
            }
        }
    }
}
