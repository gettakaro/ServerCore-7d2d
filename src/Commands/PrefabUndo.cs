using System;
using System.Collections.Generic;
using System.Threading;

namespace ServerCore.CustomCommands
{
    public class PrefabUndo : ConsoleCmdAbstract
    {

        private static Dictionary<string, List<PrefabUndoObj>> undoObjs;

        public override string getDescription()
        {
            return "Undo last prefab command";
        }

        public override string getHelp()
        {
            return "Usage:\n" +
                "  1. bundo\n" +
                "1. Undo prefabs command. Works with brender, fblock, brepblock and bdup\n" +
                "By default the size of undo history ise set to 1. You can change the undo history size using \"setbundosize\"\n";
        }

        public override string[] getCommands()
        {
            return new[] { "pc-bundo", "bundo" };
        }

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                ClientInfo ci = _senderInfo.RemoteClientInfo;
                string entityID = "";
                if (ci == null)
                {
                    entityID = "server_";

                }
                else
                {
                    entityID = ci.entityId + "";
                }

                PrefabUndoObj undoObj = PrefabUndo.getUndoPrefab(entityID);

                if (undoObj == null)
                {
                    SdtdConsole.Instance.Output("ERR: Unable to undo the last prefab command");
                    return;
                }

                Dictionary<long, Chunk> dic = new Dictionary<long, Chunk>();
                for (int j = 0; j < undoObj.Prefab.size.x; j++)
                {
                    for (int k = 0; k < undoObj.Prefab.size.z; k++)
                    {
                        for (int m = 0; m < undoObj.Prefab.size.y; m++)
                        {
                            if (GameManager.Instance.World.IsChunkAreaLoaded(undoObj.Position.x + j, undoObj.Position.y + m, undoObj.Position.z + k))
                            {
                                Chunk c = GameManager.Instance.World.GetChunkFromWorldPos(undoObj.Position.x + j, undoObj.Position.y + m, undoObj.Position.z + k) as Chunk;
                                if (!dic.ContainsKey(c.Key))
                                {
                                    dic.Add(c.Key, c);
                                    var sleepers = c.GetSleeperVolumes();
                                    var count = sleepers.Count;
                                    if (count > 0)
                                    {
                                        sleepers.Clear();
                                    }
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

                PrefabStorage.RunIncludingStorage(() => undoObj.Prefab.CopyIntoLocal(GameManager.Instance.World.ChunkCache, new Vector3i(undoObj.Position.x, undoObj.Position.y, undoObj.Position.z), true, true, FastTags<TagGroup.Global>.none));

                Thread.Sleep(50);
                ForceChunkReload.exec(dic);

                StabilityCalculator stabCalc = new StabilityCalculator();
                stabCalc.Init(GameManager.Instance.World);

                for (int j = 0; j < undoObj.Prefab.size.x; j++)
                {
                    for (int k = 0; k < undoObj.Prefab.size.z; k++)
                    {
                        for (int m = 0; m < undoObj.Prefab.size.y; m++)
                        {
                            BlockValue block = GameManager.Instance.World.GetBlock(undoObj.Position.x + j, undoObj.Position.y + m, undoObj.Position.z + k);
                            if (block.type != BlockValue.Air.type)
                            {
                                Vector3i _position = new Vector3i(undoObj.Position.x + j, undoObj.Position.y + m, undoObj.Position.z + k);
                                stabCalc.BlockPlacedAt(_position, false);

                            }
                        }
                    }
                }

                stabCalc.Cleanup();
                stabCalc = null;

                //remove spawned sleepers
                int x1 = undoObj.Position.x;
                int z1 = undoObj.Position.z;
                int x2 = undoObj.Position.x + undoObj.Prefab.size.x;
                int z2 = undoObj.Position.z + undoObj.Prefab.size.z;

                RemoveSpawnedSleepers(x1, z1, x2, z2);

                //remove from dynamicPrefabDecorator if id > -1
                if (undoObj.PrefabInstanceId != -1)
                {
                    DynamicPrefabDecorator dpd = GameManager.Instance.GetDynamicPrefabDecorator();
                    List<PrefabInstance> allPrefabs = new List<PrefabInstance>();
                    dpd.GetWorldPrefabs(allPrefabs);

                    for (int i = 0; i < allPrefabs.Count; i++)
                    {
                        PrefabInstance fab = allPrefabs[i];
                        if (fab != null)
                        {
                            if (undoObj.PrefabInstanceId == fab.id)
                            {
                                dpd.RemoveWorldPrefab(fab);
                                dpd.poiPrefabs.Remove(fab);

                                PathAbstractions.AbstractedLocation location = PathAbstractions.WorldsSearchPaths.GetLocation(GameManager.Instance.World.ChunkCache.Name, null, null);
                                dpd.Save(location.FullPath);
                                SdtdConsole.Instance.Output($"Found undoBrender with \"addtorwg\". Removed prefab with PrefabInstanceId={fab.id} from Randomgen World.");
                                break;
                            }
                        }
                    }
                }

                SdtdConsole.Instance.Output("Prefab Undone at " + undoObj.Position.x + " " + undoObj.Position.y + " " + undoObj.Position.z);

            }
            catch (Exception e)
            {
                SdtdConsole.Instance.Output("Error in PrefabUndo.Run: " + e);
            }
        }

        public static PrefabUndoObj getUndoPrefab(string entityID)
        {
            if (undoObjs == null)
            {
                return null;
            }
            List<PrefabUndoObj> list;
            if (undoObjs.TryGetValue(entityID, out list))
            {
                if (list == null || list.Count == 0)
                {
                    return null;
                }
                PrefabUndoObj obj = list[0];
                list.RemoveAt(0);
                return obj;
            }
            return null;
        }

        public static void setUndo(string entityID, Prefab pref, Vector3i location, int id)
        {
            if (undoObjs == null)
            {
                undoObjs = new Dictionary<string, List<PrefabUndoObj>>();
            }
            List<PrefabUndoObj> list;
            if (!undoObjs.TryGetValue(entityID, out list))
            {
                list = new List<PrefabUndoObj>();
                undoObjs.Add(entityID, list);
            }
            if (list.Count >= ServerCoreSettings.Instance.Bundo_HistorySize)
            {
                list.RemoveAt(list.Count - 1);
            }
            PrefabUndoObj obj = new PrefabUndoObj(pref, location, id);
            list.Insert(0, obj);

            undoObjs.Remove(entityID);
            undoObjs.Add(entityID, list);
        }

        private static void RemoveSpawnedSleepers(int x1, int z1, int x2, int z2)
        {
            List<EntityAlive> hostiles = new List<EntityAlive>();

            try
            {
                hostiles.Clear();
                for (int index = 0; index < GameManager.Instance.World.Entities.list.Count; ++index)
                {
                    EntityAlive entityAlive = GameManager.Instance.World.Entities.list[index] as EntityAlive;
                    if (entityAlive != null && entityAlive.IsAlive() && EntityClass.list[entityAlive.entityClass].bIsEnemyEntity)
                    {
                        Vector3i hostilePos = new Vector3i(entityAlive.GetPosition());

                        //Fix the order of xyz1 xyz2
                        if (x2 < x1)
                        {
                            int val = x1;
                            x1 = x2;
                            x2 = val;
                        }

                        if (z2 < z1)
                        {
                            int val = z1;
                            z1 = z2;
                            z2 = val;
                        }

                        if (hostilePos.x > x1 && hostilePos.x < x2 && hostilePos.z > z1 && hostilePos.z < z2)
                        {
                            hostiles.Add(entityAlive);
                        }
                    }
                }

                if (hostiles.Count > 0)
                {
                    foreach (EntityAlive ea in hostiles)
                    {
                        GameManager.Instance.World.RemoveEntity(ea.entityId, EnumRemoveEntityReason.Killed);
                    }
                }

            }
            catch (Exception e) { Log.Error(e.ToString()); }
        }
    }

    public class PrefabUndoObj
    {
        private Prefab prefab;
        private Vector3i position;
        private int prefabInstanceId;

        public PrefabUndoObj(Prefab prefab, Vector3i position, int id)
        {
            this.Prefab = prefab;
            this.Position = position;
            this.PrefabInstanceId = id;
        }

        public Prefab Prefab
        {
            get
            {
                return prefab;
            }

            set
            {
                prefab = value;
            }
        }

        public Vector3i Position
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }

        public int PrefabInstanceId
        {
            get
            {
                return prefabInstanceId;
            }

            set
            {
                prefabInstanceId = value;
            }
        }
    }
}
