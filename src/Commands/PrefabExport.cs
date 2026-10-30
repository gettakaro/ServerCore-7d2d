using System;
using System.Collections.Generic;

namespace ServerCore.CustomCommands
{
    public class PrefabExport : ConsoleCmdAbstract
    {

        private static Dictionary<int, Vector3i> savedLocation = new Dictionary<int, Vector3i>();

        public override string getDescription()
        {
            return "Exports as Prefab some space";
        }

        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. bexport <x1> <x2> <y1> <y2> <z1> <z2> <prefab_file_name> [overwrite] [withstorage]\n" +
                "  2. bexport \n" +
                "  3. bexport <prefab_file_name> [overwrite] [withstorage]\n\n" +
                "1. Export the defined area to a prefabFile in folder .../UserData/LocalPrefabs/  \n" +
                "2. Store the player position to be used togheter on method 3.  \n" +
                "3. Use stored position on method 2. with current position to export the area to prefab File in folder .../UserData/LocalPrefabs/ \n" +
                "   NOTE: Sleepervolumes are lost during this process. See brender for more information.\n" +
                "   NOTE: Storage blocks (crates, safes, ...) are not exported, unless you add \"withstorage\" to export them with their contents.";
        }

        public override string[] getCommands()
        {
            return new[] { "pc-bexport", "bexport" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                bool withStorage = PrefabStorage.TakeWithStorageParam(_params);
                if (_params.Count != 8 && _params.Count != 7 && _params.Count != 0 && _params.Count != 1 && _params.Count != 2)
                {
                    SdtdConsole.Instance.Output("ERR: Wrong number of arguments, expected 0 or 1, 2, 7 or 8, found " + _params.Count + ".");
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

                    string fileName = "";

                    if (_params.Count == 0)
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
                        if (savedLocation.ContainsKey(ci.entityId))
                        {
                            savedLocation.Remove(ci.entityId);
                        }
                        savedLocation.Add(ci.entityId, new Vector3i(ep.GetBlockPosition().x, ep.GetBlockPosition().y, ep.GetBlockPosition().z));
                        SdtdConsole.Instance.Output("Stored position: " + ep.GetBlockPosition().x + " " + ep.GetBlockPosition().y + " " + ep.GetBlockPosition().z);
                        return;
                    }
                    else if (_params.Count == 1 || _params.Count == 2)
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
                        if (!savedLocation.ContainsKey(ci.entityId))
                        {
                            SdtdConsole.Instance.Output("ERR: There isnt any stored location. Use method 2. to store a position.");
                            SdtdConsole.Instance.Output(GetHelp());
                            return;
                        }
                        Vector3i storedPos;
                        savedLocation.TryGetValue(ci.entityId, out storedPos);
                        savedLocation.Remove(ci.entityId);

                        x1 = storedPos.x;
                        y1 = storedPos.y;
                        z1 = storedPos.z;

                        x2 = ep.GetBlockPosition().x;
                        y2 = ep.GetBlockPosition().y;
                        z2 = ep.GetBlockPosition().z;

                        fileName = _params[0];
                    }
                    else if (_params.Count == 7 || _params.Count == 8)
                    {
                        int.TryParse(_params[0], out x1);
                        int.TryParse(_params[2], out y1);
                        int.TryParse(_params[4], out z1);

                        int.TryParse(_params[1], out x2);
                        int.TryParse(_params[3], out y2);
                        int.TryParse(_params[5], out z2);

                        fileName = _params[6];
                    }
                    if (x1 == int.MinValue || y1 == int.MinValue || z1 == int.MinValue || x2 == int.MinValue || y2 == int.MinValue || z2 == int.MinValue)
                    {
                        SdtdConsole.Instance.Output("ERR: At least one of the given coordinates is not a valid integer");
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

                    if (!_params.ContainsCaseInsensitive("overwrite"))
                    {
                        if (Prefab.PrefabExists(fileName))
                        {
                            SdtdConsole.Instance.Output("A prefab with the name \"" + fileName + "\" already exists.");
                            return;
                        }
                    }

                    Prefab pref = new Prefab();


                    pref.location = Prefab.LocationForNewPrefab(fileName);

                    bool saved;
                    PrefabStorage.IncludeStorage = withStorage;
                    try
                    {
                        pref.copyFromWorld(GameManager.Instance.World, new Vector3i(x1, y1, z1), new Vector3i(x2, y2, z2));
                        pref.bCopyAirBlocks = true;
                        int skipped = withStorage ? 0 : PrefabStorage.RemoveStorageBlocks(pref);
                        if (skipped > 0)
                        {
                            SdtdConsole.Instance.Output("Skipped " + skipped + " storage block(s). Add \"withstorage\" to export them with their contents.");
                        }
                        saved = pref.Save(pref.location, true);
                    }
                    finally
                    {
                        PrefabStorage.IncludeStorage = false;
                    }

                    if (saved)
                    {
                        SdtdConsole.Instance.Output("Prefab " + fileName + " exported. Area mapped from " + x1 + " " + y1 + " " + z1 + " to " + x2 + " " + y2 + " " + z2);
                        return;
                    }

                    SdtdConsole.Instance.Output("Prefab could not be saved");
                }
            }
            catch (Exception e)
            {
                Log.Out("Error in PrefabExport.Run: " + e);
                SdtdConsole.Instance.Output("ERR: Prefab export failed: " + e.Message + " (see server log)");
            }
        }
    }
}
