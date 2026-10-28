using System;
using System.Collections.Generic;
using System.Threading;

namespace ServerCore.CustomCommands
{
    public class PrefabDuplicator : ConsoleCmdAbstract
    {

        private static Dictionary<int, Vector3i> location1 = new Dictionary<int, Vector3i>();
        private static Dictionary<int, Vector3i> location2 = new Dictionary<int, Vector3i>();

        public override string getDescription()
        {
            return "Copy an Area to another location";
        }

        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. bdup <x1> <x2> <y1> <y2> <z1> <z2> <x> <y> <z> <rot>\n" +
                "  2. bdup p1\n" +
                "  3. bdup p2\n" +
                "  4. bdup <x> <y> <z> <rot>\n" +
                "  5. bdup <rot>\n" +
                "1. duplicate the defined area on x,y,z\n" +
                "2. Store on position 1 your current location\n" +
                "3. Store on position 2 your current location\n" +
                "4. use stored position 1 and 2 to duplicate on x,y,z\n" +
                "5. use stored position 1 and 2 to duplicate on your current location\n" +
                "<rot> prefab rotation -> need to be equal 0,1,2 or 3\n" +
                "Storage blocks (crates, safes, ...) are not duplicated, unless you add \"withstorage\" to methods 1, 4 or 5 to duplicate them with their contents.\n";
        }

        public override string[] getCommands()
        {
            return new[] { "pc-bdup", "bdup" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                bool withStorage = PrefabStorage.TakeWithStorageParam(_params);
                PrefabStorage.IncludeStorage = withStorage;
                int x1 = int.MinValue;
                int y1 = int.MinValue;
                int z1 = int.MinValue;

                int x2 = int.MinValue;
                int y2 = int.MinValue;
                int z2 = int.MinValue;

                int x = int.MinValue;
                int y = int.MinValue;
                int z = int.MinValue;

                int rot = int.MinValue;

                if (_params.Count != 10 && _params.Count != 1 && _params.Count != 4)
                {
                    SdtdConsole.Instance.Output("ERR: Wrong number of arguments, expected 10, found " + _params.Count + ".");
                    SdtdConsole.Instance.Output(GetHelp());
                    return;
                }



                if (_params.Count == 1)
                {
                    string param = _params[0];
                    if (param.ToLower().Equals("p1"))
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
                        if (location1.ContainsKey(ci.entityId))
                        {
                            location1.Remove(ci.entityId);
                        }
                        location1.Add(ci.entityId, new Vector3i(ep.GetBlockPosition().x, ep.GetBlockPosition().y, ep.GetBlockPosition().z));
                        SdtdConsole.Instance.Output("Stored position 1: " + ep.GetBlockPosition().x + " " + ep.GetBlockPosition().y + " " + ep.GetBlockPosition().z);
                        return;
                    }
                    else if (param.ToLower().Equals("p2"))
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
                        if (location2.ContainsKey(ci.entityId))
                        {
                            location2.Remove(ci.entityId);
                        }
                        location2.Add(ci.entityId, new Vector3i(ep.GetBlockPosition().x, ep.GetBlockPosition().y, ep.GetBlockPosition().z));
                        SdtdConsole.Instance.Output("Stored position 2: " + ep.GetBlockPosition().x + " " + ep.GetBlockPosition().y + " " + ep.GetBlockPosition().z);
                        return;
                    }
                    else
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

                        if (!location1.ContainsKey(ci.entityId))
                        {
                            SdtdConsole.Instance.Output("ERR: There isnt any stored location 1. Use method 2. to store a position.");
                            SdtdConsole.Instance.Output(GetHelp());
                            return;
                        }

                        if (!location2.ContainsKey(ci.entityId))
                        {
                            SdtdConsole.Instance.Output("ERR: There isnt any stored location 2. Use method 3. to store a position.");
                            SdtdConsole.Instance.Output(GetHelp());
                            return;
                        }

                        int.TryParse(param, out rot);

                        Vector3i storedPos1;
                        location1.TryGetValue(ci.entityId, out storedPos1);

                        x1 = storedPos1.x;
                        y1 = storedPos1.y;
                        z1 = storedPos1.z;

                        Vector3i storedPos2;
                        location2.TryGetValue(ci.entityId, out storedPos2);

                        x2 = storedPos2.x;
                        y2 = storedPos2.y;
                        z2 = storedPos2.z;

                        x = ep.GetBlockPosition().x;
                        y = ep.GetBlockPosition().y;
                        z = ep.GetBlockPosition().z;
                    }
                }
                else if (_params.Count == 4)
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

                    if (!location1.ContainsKey(ci.entityId))
                    {
                        SdtdConsole.Instance.Output("ERR: There isnt any stored location 1. Use method 2. to store a position.");
                        SdtdConsole.Instance.Output(GetHelp());
                        return;
                    }

                    if (!location2.ContainsKey(ci.entityId))
                    {
                        SdtdConsole.Instance.Output("ERR: There isnt any stored location 2. Use method 3. to store a position.");
                        SdtdConsole.Instance.Output(GetHelp());
                        return;
                    }

                    int.TryParse(_params[1], out x);
                    int.TryParse(_params[2], out y);
                    int.TryParse(_params[3], out z);
                    int.TryParse(_params[4], out rot);

                    Vector3i storedPos1;
                    location1.TryGetValue(ci.entityId, out storedPos1);

                    x1 = storedPos1.x;
                    y1 = storedPos1.y;
                    z1 = storedPos1.z;

                    Vector3i storedPos2;
                    location2.TryGetValue(ci.entityId, out storedPos2);

                    x2 = storedPos2.x;
                    y2 = storedPos2.y;
                    z2 = storedPos2.z;

                }
                else if (_params.Count == 10)
                {
                    int.TryParse(_params[0], out x1);
                    int.TryParse(_params[2], out y1);
                    int.TryParse(_params[4], out z1);

                    int.TryParse(_params[1], out x2);
                    int.TryParse(_params[3], out y2);
                    int.TryParse(_params[5], out z2);

                    int.TryParse(_params[6], out x);
                    int.TryParse(_params[7], out y);
                    int.TryParse(_params[8], out z);
                    int.TryParse(_params[9], out rot);
                }

                if (x == int.MinValue || y == int.MinValue || z == int.MinValue)
                {
                    SdtdConsole.Instance.Output("ERR: At least one of the given coordinates is not a valid integer");
                    return;
                }

                if (rot < 0 || rot > 3)
                {
                    SdtdConsole.Instance.Output("ERR: Invalid rotation parameter. It need to be 0,1,2 or 3");
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

                if (x1 == int.MinValue || y1 == int.MinValue || z1 == int.MinValue || x2 == int.MinValue || y2 == int.MinValue || z2 == int.MinValue || x == int.MinValue || y == int.MinValue || z == int.MinValue)
                {
                    SdtdConsole.Instance.Output("ERR: At least one of the given coordinates is not a valid integer");
                    return;
                }

                Prefab pref = new Prefab();
                pref.copyFromWorld(GameManager.Instance.World, new Vector3i(x1, y1, z1), new Vector3i(x2, y2, z2));
                pref.bCopyAirBlocks = true;
                int skipped = withStorage ? 0 : PrefabStorage.RemoveStorageBlocks(pref);
                if (skipped > 0)
                {
                    SdtdConsole.Instance.Output("Skipped " + skipped + " storage block(s). Add \"withstorage\" to duplicate them with their contents.");
                }

                SdtdConsole.Instance.Output("Area duplicated from " + x1 + " " + y1 + " " + z1 + " to " + x2 + " " + y2 + " " + z2);

                pref.RotateY(false, rot);

                Dictionary<long, Chunk> dic = new Dictionary<long, Chunk>();
                for (int j = 0; j < pref.size.x; j++)
                {
                    for (int k = 0; k < pref.size.z; k++)
                    {
                        for (int m = 0; m < pref.size.y; m++)
                        {
                            if (GameManager.Instance.World.IsChunkAreaLoaded(x + j, y + m, z + k))
                            {
                                Chunk c = GameManager.Instance.World.GetChunkFromWorldPos(x + j, y + m, z + k) as Chunk;
                                if (!dic.ContainsKey(c.Key))
                                {
                                    dic.Add(c.Key, c);
                                }
                            }
                            else
                            {
                                SdtdConsole.Instance.Output("ERR: The renfer prefab is far away. Chunk not loaded on that area");
                                return;
                            }
                        }
                    }
                }

                Prefab undo = new Prefab(new Vector3i(pref.size.x, pref.size.y, pref.size.z));
                undo.bCopyAirBlocks = true;

                PrefabStorage.RunIncludingStorage(() => undo.copyFromWorld(GameManager.Instance.World, new Vector3i(x, y, z), new Vector3i(x + pref.size.x, y + pref.size.y, z + pref.size.z)));
                if (_senderInfo.RemoteClientInfo != null)
                {
                    PrefabUndo.setUndo(_senderInfo.RemoteClientInfo.entityId + "", undo, new Vector3i(x, y, z), -1);
                }
                else
                {
                    PrefabUndo.setUndo("server_", undo, new Vector3i(x, y, z), -1);
                }

                DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
                PrefabInstance prefInstance = new PrefabInstance(dpd.GetNextId(), pref.location, new Vector3i(x, y, z), (byte)pref.GetLocalRotation(), pref, 0);

                List<SleeperVolume> slVolumes = prefInstance.sleeperVolumes;
                foreach (SleeperVolume slVolume in slVolumes)
                {
                    prefInstance.sleeperVolumes.Remove(slVolume);
                }

                pref.CopyIntoLocal(GameManager.Instance.World.ChunkCache, new Vector3i(x, y, z), true, true, FastTags<TagGroup.Global>.none);

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
                            BlockValue block = GameManager.Instance.World.GetBlock(x + j, y + m, z + k);
                            if (block.type != BlockValue.Air.type)
                            {
                                Vector3i _position = new Vector3i(x + j, y + m, z + k);
                                stabCalc.BlockPlacedAt(_position, false);
                            }
                        }
                    }
                }
                stabCalc.Cleanup();
                stabCalc = null;

                SdtdConsole.Instance.Output("Duplicated Area at " + x + " " + y + " " + z);
            }
            catch (Exception e)
            {
                SdtdConsole.Instance.Output("Error in PrefabDuplicator.Run: " + e);
            }
            finally
            {
                PrefabStorage.IncludeStorage = false;
            }
        }

    }
}
