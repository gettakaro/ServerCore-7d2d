using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ServerCore
{

    public class API : IModApi
    {
        public static Dictionary<string, Vector3i> dicDied = new Dictionary<string, Vector3i>();
        public static string GamePath = GameIO.GetSaveGameRootDir();

        public static int MaxPlayers = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("ServerMaxPlayerCount"));

        public static List<hostilefreeClaim> hostilefreeClaims = new List<hostilefreeClaim>();

        public static string modPath;

        public void InitMod(Mod mod)
        {
            ModEvents.GameStartDone.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameStartDoneData>(GameAwake));
            ModEvents.GameShutdown.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SGameShutdownData>(GameShutdown));
            ModEvents.PlayerSpawnedInWorld.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SPlayerSpawnedInWorldData>(PlayerSpawnedInWorld));
            ModEvents.PlayerDisconnected.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SPlayerDisconnectedData>(PlayerDisconnected));
            ModEvents.ChatMessage.RegisterHandler(new ModEvents.ModEventInterruptibleHandlerDelegate<ModEvents.SChatMessageData>(ChatMessage));
            ModEvents.PlayerLogin.RegisterHandler(new ModEvents.ModEventInterruptibleHandlerDelegate<ModEvents.SPlayerLoginData>(PlayerLogin));
            ModEvents.EntityKilled.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SEntityKilledData>(EntityKilled));
            ModEvents.GameMessage.RegisterHandler(new ModEvents.ModEventInterruptibleHandlerDelegate<ModEvents.SGameMessageData>(GameMessage));
            ModEvents.CalcChunkColorsDone.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SCalcChunkColorsDoneData>(CalcChunkColorsDone));
            ModEvents.SavePlayerData.RegisterHandler(new ModEvents.ModEventHandlerDelegate<ModEvents.SSavePlayerDataData>(SavePlayerData));
            modPath = mod.Path;
        }

        private void SavePlayerData(ref ModEvents.SSavePlayerDataData _data)
        {
            if(ServerCoreSettings.Instance.BannedItems_Enabled)
            {
                RegionReset.HandleBannedItems(_data);
            }

            if (ServerCoreSettings.Instance.VIPModGuard_Enabled)
            {
                RegionReset.HandleVIPGuardItems(_data);
            }
        }

        private ModEvents.EModEventResult ChatMessage(ref ModEvents.SChatMessageData _data)
        {
            return ChatFilter.Exec(_data.ClientInfo, _data.ChatType, _data.Message, _data.MainName, _data.RecipientEntityIds);
        }

        private ModEvents.EModEventResult GameMessage(ref ModEvents.SGameMessageData _data)
        {
            switch (_data.MessageType)
            {
                case EnumGameMessages.JoinedGame:
                    if (!ServerCoreSettings.Instance.GMSG_PlayerJoined_Enabled) return ModEvents.EModEventResult.StopHandlersAndVanilla;
                    break;
                case EnumGameMessages.LeftGame:
                    if (!ServerCoreSettings.Instance.GMSG_PlayerLeft_Enabled) return ModEvents.EModEventResult.StopHandlersAndVanilla;
                    break;
                case EnumGameMessages.EntityWasKilled:
                    if (!string.IsNullOrEmpty(_data.SecondaryName))
                    {
                        if (!ServerCoreSettings.Instance.GMSG_PlayerKilled_Enabled) return ModEvents.EModEventResult.StopHandlersAndVanilla;
                    }
                    else
                    {
                        if (!ServerCoreSettings.Instance.GMSG_PlayerDied_Enabled) return ModEvents.EModEventResult.StopHandlersAndVanilla;
                    }
                    break;
            }

            // Continue, not StopHandlersRunVanilla: stopping would hide the message from mods loaded
            // after this one and make the game skip its "GMSG:" log line.
            return ModEvents.EModEventResult.Continue;
        }

        private void GameAwake(ref ModEvents.SGameStartDoneData _data)
        {
            BsonMapper.Global.EmptyStringToNull = false;

            Patcher.DoPatching();

            List<DbClaim> lstClaims = Database.Instance.GetAllDbClaims();
            bool found = false;

            if (lstClaims != null && lstClaims.Count != 0)
            {
                foreach (DbClaim activeClaim in lstClaims)
                {
                    if (activeClaim == null) continue;

                    if (activeClaim.Type.ToLower().Equals("hostilefree"))
                    {
                        hostilefreeClaim claim = new hostilefreeClaim();
                        claim.W_bound = activeClaim.W_bound;
                        claim.E_bound = activeClaim.E_bound;
                        claim.N_bound = activeClaim.N_bound;
                        claim.S_bound = activeClaim.S_bound;
                        hostilefreeClaims.Add(claim);
                        found = true;
                    }
                }

                if (found)
                {
                    Log.Out("[PrismaCore] Loaded hostilefree claims into memory for sleeperhandling.");
                }
            }

            if (!Directory.Exists(RegionReset.RegionPath)) Directory.CreateDirectory(RegionReset.RegionPath);
            if (!File.Exists(RegionReset.RegionFile))
            {
                using (File.Create(RegionReset.RegionFile)) { }
                Log.Out("[PrismaCore] Created new empty regions.txt in " + RegionReset.RegionPath);
            }
            if (!File.Exists(RegionReset.PrefabExceptionFile))
            {
                using (File.Create(RegionReset.PrefabExceptionFile)) { }
                Log.Out("[PrismaCore] Created new empty ResetPrefabs_Exceptions.txt in " + RegionReset.RegionPath);
            }
            if (!File.Exists(RegionReset.QuestPoiExceptionFile))
            {
                using (File.Create(RegionReset.QuestPoiExceptionFile)) { }
                Log.Out("[PrismaCore] Created new empty QuestPoi_Exceptions.txt in " + RegionReset.RegionPath);
            }
            if (!File.Exists(RegionReset.AllPoiExceptionFile))
            {
                using (File.Create(RegionReset.AllPoiExceptionFile)) { }
                Log.Out("[PrismaCore] Created new empty AllPoi_Exceptions.txt in " + RegionReset.RegionPath);
            }
            if (!File.Exists(RegionReset.VIPModGuardItemsFile))
            {
                using (File.Create(RegionReset.VIPModGuardItemsFile)) { }
                Log.Out("[PrismaCore] Created new empty VIPModGuardItems.txt in " + RegionReset.RegionPath);
            }

            ServerCoreStrings.Load();
            ServerCoreSettings.Load();

            //save on server initialization for getting new strings into xml
            ServerCoreStrings.Instance.Save();
            ServerCoreSettings.Instance.Save();

            PermaDeathClass.Loadxml();
            ReservedSlots.LoadXml();
            RegionReset.LoadRegions();
            RegionReset.LoadVIPGuardItems();
            RegionReset.LoadBannedItems();
            RegionWatcher.LoadWatchers();
            
            new Web.Web();

            if (!Directory.Exists(LocationTracker.StatisticsPath)) Directory.CreateDirectory(LocationTracker.StatisticsPath);
            else
            {
                Log.Out("[PrismaCore] Starting cleanup of player databases...");
                Database.Instance.CleanDataBases();
                Log.Out("[PrismaCore] Player databases have been cleaned!");
            }

            LocationTracker.Start();
            ClaimProtector.Start();

            VehicleManager.Instance.Load();
            DroneManager.instance.Load();
        }

        private void GameShutdown(ref ModEvents.SGameShutdownData _data)
        {
            try
            {
                try
                {
                    if (ServerCoreSettings.Instance.Vehicles_RemoveOnRestart || RegionReset.resetVehicles)
                    {
                        Log.Out("[PrismaCore] Resetting vehicles.");

                        string saveFolder = GameIO.GetSaveGameDir();

                        if (SdFile.Exists($"{saveFolder}/vehicles.dat.bak"))
                        {
                            SdFile.Delete($"{saveFolder}/vehicles.dat.bak");
                        }

                        if (SdFile.Exists($"{saveFolder}/vehicles.dat"))
                        {
                            SdFile.Delete($"{saveFolder}/vehicles.dat");
                        }

                        Log.Out("[PrismaCore] Done resetting vehicles.");
                    }
                }
                catch { Log.Out("[PrismaCore] Error in deleting vehicle files."); }

                try
                {
                    if (ServerCoreSettings.Instance.Drones_RemoveOnRestart || RegionReset.resetDrones)
                    {
                        Log.Out("[PrismaCore] Resetting drones.");

                        string saveFolder = GameIO.GetSaveGameDir();

                        if (SdFile.Exists($"{saveFolder}/drones.dat.bak"))
                        {
                            SdFile.Delete($"{saveFolder}/drones.dat.bak");
                        }

                        if (SdFile.Exists($"{saveFolder}/drones.dat"))
                        {
                            SdFile.Delete($"{saveFolder}/drones.dat");
                        }

                        Log.Out("[PrismaCore] Done resetting drones.");
                    }
                }
                catch { Log.Out("[PrismaCore] Error in deleting drone files."); }

                //unload PrismaCore threads
                try
                {
                    LocationTracker.Unload();
                    Thread.Sleep(1);
                    ClaimProtector.Unload();
                    Thread.Sleep(1);
                }
                catch { }

                //copy logfile
                try
                {
                    if (ServerCoreSettings.Instance.CreateTimeStampedCopyLogFile)
                    {
                        string logFile = Application.consoleLogPath;
                        string logTime = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss");
                        string destFile = $"{logFile}__{logTime}.log";
                        SdFile.Copy(logFile, destFile, true);
                    }
                }
                catch (Exception e)
                { Log.Out($"[PrismaCore] No copy of latest logfile could be created. Error: {e}"); }

            }
            catch (Exception e)
            {
                Log.Out("Error in GameShutdown: " + e);
            }
        }

        private void PlayerDisconnected(ref ModEvents.SPlayerDisconnectedData _data)
        {
            if (_data.ClientInfo == null)
                return;

            try
            {
                EntityPlayer ep = GameManager.Instance.World.Players.dict[_data.ClientInfo.entityId];
                LastKnownLocation lkl = new LastKnownLocation();
                lkl.X = (int)Math.Floor(ep.position.x);
                lkl.Y = (int)Math.Floor(ep.position.y);
                lkl.Z = (int)Math.Floor(ep.position.z);
                long sessionPlayTime = (long)(Time.timeSinceLevelLoad - ep.CreationTimeSinceLevelLoad);
                Database.Instance.SetOffline(_data.ClientInfo.PlatformId.ToString(), DateTime.Now, sessionPlayTime, lkl);
            }
            catch
            {
                Database.Instance.SetOffline(_data.ClientInfo.PlatformId.ToString(), DateTime.Now);
            }

            if (ClaimProtector.adminBubble.Contains(_data.ClientInfo.PlatformId.ToString()))
                ClaimProtector.adminBubble.Remove(_data.ClientInfo.PlatformId.ToString());
        }

        private void EntityKilled(ref ModEvents.SEntityKilledData _data)
        {
            if (_data.KillingEntity != null && _data.KilledEntitiy != null)
            {
                if (_data.KillingEntity.entityType == EntityType.Player)
                {
                    ClientInfo ci = ConsoleHelper.ParseParamIdOrName(_data.KillingEntity.entityId.ToString());
                    if (ci == null) return;

                    EntityAlive ea = _data.KilledEntitiy as EntityAlive;

                    int? entID = _data.KillingEntity.entityId;

                    string weap = DamageHandler.weaponUsed(entID);

                    if (_data.KilledEntitiy.entityType == EntityType.Zombie)
                    {
                        Log.Out($"[PrismaCore]entityKilled: {ci.playerName} ({ci.PlatformId}) killed zombie {ea.LocalizedEntityName} with {weap}");
                    }
                    else
                    {
                        Log.Out($"[PrismaCore]entityKilled: {ci.playerName} ({ci.PlatformId}) killed animal {ea.LocalizedEntityName} with {weap}");
                    }
                }
            }
        }

        private void PlayerSpawnedInWorld(ref ModEvents.SPlayerSpawnedInWorldData _data)
        {
            if (_data.ClientInfo == null)
                return;

            if (_data.RespawnType == RespawnType.EnterMultiplayer || _data.RespawnType == RespawnType.JoinMultiplayer)
            {
                try
                {
                    EntityPlayer ep = GameManager.Instance.World.Players.dict[_data.ClientInfo.entityId];
                    LastKnownLocation lkl = new LastKnownLocation();
                    lkl.X = (int)Math.Floor(ep.position.x);
                    lkl.Y = (int)Math.Floor(ep.position.y);
                    lkl.Z = (int)Math.Floor(ep.position.z);
                    Database.Instance.SetOnline(_data.ClientInfo.PlatformId.ToString(), _data.ClientInfo.entityId, _data.ClientInfo.playerName, _data.ClientInfo.ip, DateTime.Now, _data.ClientInfo.CrossplatformId.ToString(), lkl);
                }
                catch
                {
                    Database.Instance.SetOnline(_data.ClientInfo.PlatformId.ToString(), _data.ClientInfo.entityId, _data.ClientInfo.playerName, _data.ClientInfo.ip, DateTime.Now, _data.ClientInfo.CrossplatformId.ToString());
                }

                try
                {
                    string coords = Database.Instance.GetTeleSpawn(_data.ClientInfo.PlatformId.ToString());
                    if (!string.IsNullOrEmpty(coords))
                    {
                        UnityEngine.Vector3 dest = new UnityEngine.Vector3();

                        float.TryParse(coords.Split(',')[0], out float x);
                        float.TryParse(coords.Split(',')[1], out float y);
                        float.TryParse(coords.Split(',')[2], out float z);

                        dest.x = x;
                        dest.y = y;
                        dest.z = z;

                        NetPackageTeleportPlayer pkg = NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(dest);
                        _data.ClientInfo.SendPackage(pkg);
                        Database.Instance.RemoveTeleSpawn(_data.ClientInfo.PlatformId.ToString());
                        _data.ClientInfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.Offline_Teleport), null, EMessageSender.None,GeneratedTextManager.BbCodeSupportMode.Supported));

                    }
                }
                catch (Exception e)
                {
                    Log.Out("Error in offline spawner: " + e.ToString());
                }

                if (ReservedSlots.Enabled)
                {
                    if (ReservedSlots.dicWelcomeStatus.ContainsKey(_data.ClientInfo.PlatformId.ToString()))
                    {
                        ReservedSlots.WelcomePlayer(_data.ClientInfo);
                    }
                }
            }

            if (_data.RespawnType == RespawnType.Died)
            {
                //check for permadeath players and reset
                if (PermaDeathClass.DictPermaDeath.ContainsKey(_data.ClientInfo.PlatformId.ToString()))
                {
                    //start new thread for checking if player spawned and delete ttp shizzle
                    StartThreadParameterized(_data.ClientInfo);

                    return;
                }
            }
        }

        private ModEvents.EModEventResult PlayerLogin(ref ModEvents.SPlayerLoginData _data)
        {
            if (ReservedSlots.Enabled)
            {
                ReservedSlots.CheckReservedSlot(_data.ClientInfo);
            }

            if (ServerCoreSettings.Instance.BlockUTF8Names_Enabled)
            {
                if (Encoding.UTF8.GetByteCount(_data.ClientInfo.playerName) != _data.ClientInfo.playerName.Length)
                {
                    //contains NOT only ascii chars -> kick player
                    Log.Out(string.Format("[PrismaCore] Kicking Player {0}: Non ASCII characters detected in playername.", _data.ClientInfo.playerName));
                    GameUtils.KickPlayerForClientInfo(_data.ClientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick, 0, default(DateTime), ServerCoreStrings.Instance.BlockUTF8Names_KickMessage));
                    return ModEvents.EModEventResult.StopHandlersRunVanilla;
                }
            }

            if (ServerCoreSettings.Instance.SpecialCharactersNameBlock_Enabled)
            {
                if (hasSpecialChar(_data.ClientInfo.playerName))
                {
                    //contains forbidden chars -> kick player
                    Log.Out(string.Format("[PrismaCore] Kicking Player {0}: Forbidden characters detected in playername.", _data.ClientInfo.playerName));
                    string msg = ServerCoreStrings.Instance.SpecialCharactersNameBlock_KickMessage.Replace("{forbiddenChars}", ServerCoreSettings.Instance.SpecialCharacters);
                    GameUtils.KickPlayerForClientInfo(_data.ClientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick, 0, default(DateTime), msg));
                }
            }

            return ModEvents.EModEventResult.StopHandlersRunVanilla;
        }

        private void CalcChunkColorsDone(ref ModEvents.SCalcChunkColorsDoneData _data)
        {
            MapRendering.MapRendering.RenderSingleChunk(_data.Chunk);
        }

        private Thread StartThreadParameterized(ClientInfo pdClientInfo)
        {
            var t = new Thread(() => permaDeathHandler(pdClientInfo))
            {
                IsBackground = true
            };
            t.Start();
            return t;
        }

        private static bool hasSpecialChar(string input)
        {
            string specialChars = ServerCoreSettings.Instance.SpecialCharacters;
            foreach (var item in specialChars)
            {
                if (input.Contains(item)) return true;
            }

            return false;
        }

        private static void permaDeathHandler(ClientInfo pdClientInfo)
        {
            string steamID = pdClientInfo.PlatformId.ToString();
            string msg = ServerCoreStrings.Instance.Permadeath_Kickmessage;

            EntityPlayer pl = GameManager.Instance.World.Players.dict[pdClientInfo.entityId];

            while (pl == null)
            {
                pl = GameManager.Instance.World.Players.dict[pdClientInfo.entityId];
                Thread.Sleep(1000);
            }

            while (!pl.IsSpawned())
            {
                Thread.Sleep(1000);
            }

            PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerDataFromEntityID(pdClientInfo.entityId);

            GameUtils.KickPlayerForClientInfo(pdClientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick, 0, default(DateTime), msg));
            Thread.Sleep(7000);
            string fileMAP = $"{GameIO.GetSaveGameDir()}/Player/{playerDataFromEntityID.PlayerData.PrimaryId}.map";
            string fileTTP = $"{GameIO.GetSaveGameDir()}/Player/{playerDataFromEntityID.PlayerData.PrimaryId}.ttp";
            string fileBAK = $"{GameIO.GetSaveGameDir()}/Player/{playerDataFromEntityID.PlayerData.PrimaryId}.ttp.bak";
            if (SdFile.Exists(fileMAP))
            {
                SdFile.Delete(fileMAP);
            }
            if (SdFile.Exists(fileTTP))
            {
                SdFile.Delete(fileTTP);
            }
            if (SdFile.Exists(fileBAK))
            {
                SdFile.Delete(fileBAK);
            }
        }

        public static PlatformUserIdentifierAbs GetUserIdAbs(string input)
        {
            PlatformUserIdentifierAbs userId = null;

            if (PlatformUserIdentifierAbs.TryFromCombinedString(input, out userId))
            {
            }
            else
            {
                ClientInfo ci = ConsoleHelper.ParseParamIdOrName(input);
                if (ci != null)
                {
                    userId = ci.PlatformId;
                }
                else
                {
                    return userId;
                }
            }

            return userId;
        }
    }

    public class hostilefreeClaim
    {
        public int W_bound { get; set; }
        public int E_bound { get; set; }
        public int N_bound { get; set; }
        public int S_bound { get; set; }

    }
}

