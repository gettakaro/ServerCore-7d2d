using JetBrains.Annotations;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace ServerCore
{
    [Serializable]
    [XmlRoot("PrismaCoreSettings")]
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class ServerCoreSettings
    {
        private static ServerCoreSettings _instance;
        public static ServerCoreSettings Instance => _instance ?? (_instance = new ServerCoreSettings());

        public const string SaveFileName = "PrismaCoreSettings.xml";
        
        #region Persistent values to be saved

        //Adv. Reversed Claim
        public int AdvClaims_Reversed_TpHeight = -1;

        //Adv. PVP claim
        public int AdvClaims_PVP_KillingMode = 3;

        public bool BannedItems_Enabled = false;
        public string BannedItems_DetectedCommand = string.Empty;

        //BlockUTF8Names
        public bool BlockUTF8Names_Enabled = false;

        //BloodmoonSpawner
        public bool BloodmoonSpawner_DespawnAllOnStart = false;
        public bool BloodmoonSpawner_OverrideVanillaSpawner = false;
        public bool BloodmoonSpawner_Overridden_AdjustBMEnemyCountPerPlayerToNrOnlinePlayers = true;
        public int BloodmoonSpawner_Overridden_BMEnemyCountPerPlayer = 2;
        public int BloodmoonSpawner_Overridden_AddMaxAliveServerDuringBloodmoon = 0;

        //block freeformat characters
        public string SpecialCharacters = "!@#$%^&*(),.?\":;|<>'";
        public bool SpecialCharactersNameBlock_Enabled = false;

        //Bundo_HistorySize
        public int Bundo_HistorySize = 5;

        //ChatCommandPermissions
        public int ChatCommandPermissions_ft = 0;
        public int ChatCommandPermissions_ftw = 0;
        public int ChatCommandPermissions_mv = 0;
        public int ChatCommandPermissions_mvw = 0;
        public int ChatCommandPermissions_bubble = 0;
        public int ChatCommandPermissions_listwp = 0;
        public int ChatCommandPermissions_setwp = 0;
        public int ChatCommandPermissions_delwp = 0;
        public int ChatCommandPermissions_tb = 0;
        public int ChatCommandPermissions_rt = 0;
        public int ChatCommandPermissions_get = 0;
        public int ChatCommandPermissions_bag = 1000;
        public int ChatCommandPermissions_ls = 1000;
        public int ChatCommandPermissions_day7 = 1000;
        public int ChatCommandPermissions_hostiles = 1000;
        public int ChatCommandPermissions_bed = 0;
        public int ChatCommandPermissions_loctrack = 0;

        //Command received color
        public string CommandReceivedColor = "D00000";

        //PrismaCorePrefix
        public string PrismaCorePrefix = "/";

        //DamageDetection
        public int DamageDetection_MinAmountDamage = 5000;
        public string DamageDetection_DetectedCommand = "none";
        public int DamageDetection_ExcludeAdminLvl = 0;

        //SleeperVolumeControl
        public bool DisableSleeperRespawn_Enabled = false;
        public bool DisableSleepers_Enabled = false;
        public bool DisableSleepers_BloodmoonOnly_Enabled = false;

        //GMSG joined, left, killed, died
        public bool GMSG_PlayerJoined_Enabled = true;
        public bool GMSG_PlayerLeft_Enabled = true;
        public bool GMSG_PlayerDied_Enabled = true;
        public bool GMSG_PlayerKilled_Enabled = true;

        //HideChatCommandPrefixes
        public string HideChatCommandPrefixes_Prefixes = "/,$";
        public bool HideChatCommandPrefixes_Enabled = false;

        //LevelJumpDetection
        public int LevelJumpDetection_MinimumLevelJumpTrigger = 2;
        public string LevelJumpDetection_DetectedCommand = "none";
        public int LevelJumpDetection_ExcludeAdminLvl = 0;

        //LocationTracker
        public bool LocationTracker_Enabled = true;
        public string LocationTracker_ChatCommand = "/loctrack";
        public bool LocationTracker_ChatCommandEnabled = true;
        public int LocationTracker_RecordingIntervalSeconds = 15;
        public int LocationTracker_MaximumDataAgeHours = 72;
        public int LocationTracker_NearDistance = 200;
        public string LocationTracker_ResponseColor = "4DA6FF";

        //GodMode detection
        public int MaxAdminLevelGodMode = 0;
        public string GodModeDetectedCommand = string.Empty;

        //Spectator detection
        public int MaxAdminLevelSpectatorMode = 0;
        public string SpectatorModeDetectedCommand = string.Empty;

        //MaxChatLength
        public int MaxChatLength = 0;

        //NighttimeAnnouncer
        public bool NighttimeAnnouncer_Enabled = true;
        public int NighttimeAnnouncer_Warnhours = 2;

        //notify admin level
        public int NotifyAdmin_Level = 0;

        //PlayerFlying
        public int PlayerFlying_TriggerHeight = 0;
        public int PlayerFlying_MaxAdminLevelFlying = 0;

        //PreventFallingBlocks
        public int PreventFallingBlocks = 0;

        //Quest poi protection
        public bool QuestPoiProtection_Enabled = false;

        //All poi protection
        public bool AllPoiProtection_Enabled = false;

        //ResetPrefabs
        public bool ResetPrefabs_ExcludeClaimedPrefabs = true;

        //ShutdownBA
        public int ShutdownBA_DelayRestartBloodDayAfter = 15;
        public int ShutdownBA_DelayRestartAfterBloodmoonUntil = 10;
        public int ShutdownBA_MinimumUptimeRequired = 0;

        //TimerBA
        public int TimerBA_DelayBloodDayAfter = 15;
        public int TimerBA_DelayAfterBloodmoonUntil = 10;

        //Vehicle removal on boot
        public bool Vehicles_RemoveOnRestart = false;

        //VIPModGuard
        public bool VIPModGuard_Enabled = false;
        public int VIPModGuard_ExcludeAdminLvl = 0;
        public string VIPModGuard_DetectedCommand = "none";

        public bool Drones_RemoveOnRestart = false;

        //timestamped logfile
        public bool CreateTimeStampedCopyLogFile = false;

        //WebUI settings
        public int WebUI_Port = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("WebDashboardPort")) + 1;

        #endregion

        public void Save()
        {
            lock (SaveFileName)
            {
                try
                {
                    var saveFilePath = $"{API.GamePath}/{SaveFileName}";
                    using (var writer = new XmlTextWriter(new StreamWriter(saveFilePath)))
                    {
                        writer.Formatting = Formatting.Indented;
                        var serializer = new XmlSerializer(this.GetType());
                        serializer.Serialize(writer, this);
                        writer.Flush();
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("[PrismaCore] Could not save PrismaCore settings: " + ex);
                }
            }
        }

         public static void Load()
        {
            var saveFilePath = $"{API.GamePath}/{SaveFileName}";
            lock (SaveFileName)
            {
                if (!File.Exists(saveFilePath))
                {
                    Instance.Save();
                    return;
                }

                try
                {
                    using (var reader = new StreamReader(saveFilePath))
                    {
                        var serializer = new XmlSerializer(typeof(ServerCoreSettings));
                        _instance = serializer.Deserialize(reader) as ServerCoreSettings;
                    }
                    Log.Out($"[PrismaCore] PrismaCoreSettings loaded from {SaveFileName}.");
                }
                catch (Exception ex)
                {
                    Log.Error("[PrismaCore] Could not load PrismaCoreSettings: " + ex);
                }
            }
        }
    }
}
