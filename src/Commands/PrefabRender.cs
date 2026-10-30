using System;
using System.Collections.Generic;
using System.Threading;

namespace ServerCore.CustomCommands
{
    public class PrefabRender : ConsoleCmdAbstract
    {

        public override string getDescription()
        {
            return "Renders a Prefab on given location";
        }

        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. brender <prefab_file_name> <x> <y> <z> <rot> [nosleepers] [addtorwg] [withstorage]\n" +
                "  2. brender <prefab_file_name> <rot> [nosleepers] [addtorwg] [withstorage]\n" +
                "  3. brender <prefab_file_name> <rot> <depth> [nosleepers] [addtorwg] [withstorage]\n" +
                "<rot> prefab rotation -> needs to be equal to 0,1,2 or 3\n" +
                "1. Render prefab on <x> <y> <z> location\n" +
                "2. Render prefab on your position\n" +
                "3. Render prefab on your position with y deslocated <depth blocks>\n" +
                "   NOTE: Sleeper control is ONLY possible on prefabs that are present in prefabs.xml (world folder) that is used to create the map (RWG).\n" +
                "   NOTE: Use parameter \"addtorwg\" to permanently add this prefab to the current RWG world. Can be reset like any other RWG prefab and will still be in world after a wipe. Will cause re-download of world for clients! A prefab that contains storage blocks needs \"withstorage\" too.\n" +
                "   NOTE: Storage blocks (crates, safes, ...) in the prefab are not rendered, unless you add \"withstorage\" to render them with their contents.";

        }

        public override string[] getCommands()
        {
            return new[] { "pc-brender", "brender" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                bool withStorage = PrefabStorage.TakeWithStorageParam(_params);
                PrefabStorage.IncludeStorage = withStorage;
                if (_params.Count < 2 || _params.Count > 7)
                {
                    SdtdConsole.Instance.Output("ERR: Wrong number of arguments, expected 2 to 7, found " + _params.Count + ".");
                    SdtdConsole.Instance.Output(GetHelp());
                    return;
                }
                else
                {

                    int x = int.MinValue;
                    int y = int.MinValue;
                    int z = int.MinValue;
                    int rot = int.MinValue;
                    int depth = 0;
                    bool nosleepers = false;
                    bool addtorwg = false;

                    if (_params.ContainsCaseInsensitive("addtorwg") && _params.ContainsCaseInsensitive("nosleepers"))
                    {
                        nosleepers = true;
                        addtorwg = true;
                        _params.RemoveAt(_params.Count - 1);
                        _params.RemoveAt(_params.Count - 1);
                    }

                    if (_params.ContainsCaseInsensitive("addtorwg"))
                    {
                        addtorwg = true;
                        _params.RemoveAt(_params.Count - 1);
                    }

                    if (_params.ContainsCaseInsensitive("nosleepers"))
                    {
                        nosleepers = true;
                        _params.RemoveAt(_params.Count - 1);
                    }

                    if (_params.Count == 5)
                    {
                        int.TryParse(_params[1], out x);
                        int.TryParse(_params[2], out y);
                        int.TryParse(_params[3], out z);
                        int.TryParse(_params[4], out rot);

                        if (x == int.MinValue || y == int.MinValue || z == int.MinValue)
                        {
                            SdtdConsole.Instance.Output("ERR: At least one of the given coordinates is not a valid integer");
                            return;
                        }
                    }

                    else if (_params.Count == 2 || _params.Count == 3)
                    {
                        if (_senderInfo.RemoteClientInfo == null)
                        {
                            SdtdConsole.Instance.Output("ERR: This command can be only sent by player in game.");
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
                            SdtdConsole.Instance.Output("ERR: Unable to get your position");
                            return;

                        }
                        x = ep.GetBlockPosition().x;
                        y = ep.GetBlockPosition().y;
                        z = ep.GetBlockPosition().z;

                        if (_params.Count == 2)
                        {
                            int.TryParse(_params[1], out rot);
                        }

                        if (_params.Count == 3)
                        {
                            int.TryParse(_params[1], out rot);
                            int.TryParse(_params[2], out depth);
                            y = y + depth;
                        }
                    }
                    else
                    {
                        //invalid parameters 
                        SdtdConsole.Instance.Output("ERR: Wrong number of arguments or arguments in wrong order.");
                        return;
                    }

                    if (rot < 0 || rot > 3)
                    {
                        SdtdConsole.Instance.Output("ERR: Invalid rotation parameter. It need to be 0,1,2 or 3");
                        return;
                    }

                    Prefab pref = new Prefab();

                    if (!Prefab.PrefabExists(_params[0]))
                    {
                        PathAbstractions.AbstractedLocation fabLocation = Prefab.LocationForNewPrefab(_params[0]);
                        if (!pref.Load(fabLocation))
                        {
                            SdtdConsole.Instance.Output("ERR: Unable to load prefab " + _params[0]);
                            return;
                        }
                    }
                    else
                    {
                        if (!pref.Load(_params[0]))
                        {
                            SdtdConsole.Instance.Output("ERR: Unable to load prefab " + _params[0]);
                            return;
                        }
                    }

                    pref.bCopyAirBlocks = true;
                    int skipped = withStorage ? 0 : PrefabStorage.RemoveStorageBlocks(pref);
                    if (skipped > 0)
                    {
                        SdtdConsole.Instance.Output("Skipped " + skipped + " storage block(s). Add \"withstorage\" to render them with their contents.");
                    }
                    if (skipped > 0 && addtorwg)
                    {
                        SdtdConsole.Instance.Output("ERR: Prefab " + _params[0] + " contains " + skipped + " storage block(s). addtorwg renders the prefab file again when the region resets, storage included, so storage can't be skipped. Add \"withstorage\" to render it with storage, or export it again without \"withstorage\".");
                        return;
                    }
                    y = y + pref.yOffset;
                    pref.RotateY(false, rot);

                    Prefab undo = new Prefab(new Vector3i(pref.size.x, pref.size.y, pref.size.z));
                    undo.bCopyAirBlocks = true;
                    PrefabStorage.RunIncludingStorage(() => undo.copyFromWorld(GameManager.Instance.World, new Vector3i(x, y, z), new Vector3i(x + pref.size.x, y + pref.size.y, z + pref.size.z)));

                    pref.CopyIntoLocal(GameManager.Instance.World.ChunkCache, new Vector3i(x, y, z), true, true, FastTags<TagGroup.Global>.none);

                    Thread.Sleep(50);

                    HashSetLong dic = new HashSetLong();
                    for (int j = 0; j < pref.size.x; j++)
                    {
                        for (int k = 0; k < pref.size.z; k++)
                        {
                            for (int m = 0; m < pref.size.y; m++)
                            {
                                if (GameManager.Instance.World.IsChunkAreaLoaded(x + j, y + m, z + k))
                                {
                                    Chunk c = GameManager.Instance.World.GetChunkFromWorldPos(x + j, y + m, z + k) as Chunk;
                                    if (!dic.Contains(c.Key))
                                    {
                                        dic.Add(c.Key);
                                        if (nosleepers)
                                        {
                                            c.GetSleeperVolumes().Clear();
                                        }
                                    }
                                }
                                else
                                {
                                    SdtdConsole.Instance.Output("ERR: The prefab is too far away. Chunk not loaded on that area");
                                    return;
                                }
                            }
                        }
                    }

                    World world = GameManager.Instance.World;

                    world.m_ChunkManager.ResendChunksToClients(dic);

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

                    if (addtorwg)
                    {
                        DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
                        PrefabInstance pi = new PrefabInstance(dpd.GetNextId(), pref.location, new Vector3i(x, y, z), (byte)pref.GetLocalRotation(), pref, 0)
                        {
                            bPrefabCopiedIntoWorld = true
                        };
                        
                        dpd.AddWorldPrefab(pi, true);
                        dpd.poiPrefabs.Add(pi);
                        
                        PathAbstractions.AbstractedLocation location = PathAbstractions.WorldsSearchPaths.GetLocation(GameManager.Instance.World.ChunkCache.Name, null, null);
                        dpd.Save(location.FullPath);

                        if (_senderInfo.RemoteClientInfo != null)
                        {
                            PrefabUndo.setUndo(_senderInfo.RemoteClientInfo.entityId + "", undo, new Vector3i(x, y, z), pi.id);
                        }
                        else
                        {
                            PrefabUndo.setUndo("server_", undo, new Vector3i(x, y, z), pi.id);
                        }
                    }
                    else
                    {
                        if (_senderInfo.RemoteClientInfo != null)
                        {
                            PrefabUndo.setUndo(_senderInfo.RemoteClientInfo.entityId + "", undo, new Vector3i(x, y, z), -1);
                        }
                        else
                        {
                            PrefabUndo.setUndo("server_", undo, new Vector3i(x, y, z), -1);
                        }
                    }

                    SdtdConsole.Instance.Output("Prefab " + _params[0] + " loaded at " + x + " " + y + " " + z);
                }
            }
            catch (Exception e)
            {
                SdtdConsole.Instance.Output("Error in PrefabRender.Run: " + e);
            }
            finally
            {
                PrefabStorage.IncludeStorage = false;
            }
        }
    }

}
