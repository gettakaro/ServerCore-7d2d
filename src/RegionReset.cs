using Epic.OnlineServices.Presence;
using Epic.OnlineServices.RTCAudio;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using UnityEngine;
using System.Linq;
using static AIDirectorPlayerInventory;

namespace ServerCore
{
    public class RegionReset
    {
        public static List<string> lstRegions = new List<string>();
        public static List<string> lstPrefabExceptions = new List<string>();
        public static List<string> lstQuestPoiExceptions = new List<string>();
        public static List<string> lstAllPoiExceptions = new List<string>();
        public static List<string> lstRegionsClaimed = new List<string>();
        private static List<DbClaim> lstClaims = new List<DbClaim>();
        public static Dictionary<string,int> lstBannedItems = new Dictionary<string,int>();
        public static List<string> lstVIPModGuardItems = new List<string>();
        public static List<string> lstVIPModGuardCommandFired = new List<string>();

        public static string GamePathSaves = GameIO.GetSaveGameDir();
        public static string RegionPath = $"{GamePathSaves}/ResetRegions";
        public static string RegionFile = $"{RegionPath}/regions.txt";
        public static string PrefabExceptionFile = $"{RegionPath}/ResetPrefabs_Exceptions.txt";
        public static string QuestPoiExceptionFile = $"{RegionPath}/QuestPoi_Exceptions.txt";
        public static string AllPoiExceptionFile = $"{RegionPath}/AllPoi_Exceptions.txt";
        public static string BannedItemsFile = $"{API.GamePath}/PrismaCoreBannedItems.txt";
        public static string VIPModGuardItemsFile = $"{API.GamePath}/VIPModGuardItems.txt";

        private static readonly string modPath = (Application.platform != RuntimePlatform.OSXPlayer) ? (Application.dataPath + "/../Mods") : (Application.dataPath + "/../../Mods");
        public static bool resetVehicles = false;
        public static bool resetDrones = false;
        public volatile static List<EntityVehicle> vehicles = new List<EntityVehicle>();
        public volatile static List<EntityCreationData> vehicleStubs = new List<EntityCreationData>();
        public volatile static List<EntityDrone> drones = new List<EntityDrone>();
        public volatile static List<EntityCreationData> droneStubs = new List<EntityCreationData>();
        protected static readonly byte[] FileHeaderMagicBytes = Encoding.ASCII.GetBytes("7rg");

        private static ItemStack[] items;
        private static ItemStack itemStack;
        private static ItemValue itemValue;
        private static ItemClass itemClass;
        public static Dictionary<int, string> lstVipModUsers = new Dictionary<int, string>();

        // Since 3.3 ItemValue no longer exposes its installed mods as an array
        private static ItemValue[] Modifications(ItemValue item)
        {
            return Enumerable.Range(0, item.ModificationCount).Select(item.GetModification).ToArray();
        }

        public static void LoadBannedItems()
        {
            if (!File.Exists(BannedItemsFile))
            {
                using (File.Create(BannedItemsFile)) { }
                Log.Out("[PrismaCore] Created new empty PrismaCoreBannedItems.txt in " + API.GamePath);
            }
            else
            {
                var bannedItemsContent = File.ReadAllLines(BannedItemsFile);
                lstBannedItems = new Dictionary<string, int>();
                foreach (string line in bannedItemsContent)
                {
                    if(line != null && line != string.Empty)
                    {
                        if(!lstBannedItems.ContainsKey(line))
                        {
                            lstBannedItems.Add(line.Split(':')[0], Convert.ToInt32(line.Split(':')[1]));
                        }
                    }
                }

                Log.Out($"[PrismaCore] Loaded PrismaCoreBannedItems.txt from {API.GamePath}");
            }
        }

        public static void HandleBannedItems(ModEvents.SSavePlayerDataData _data)
        {
            bool cmdExecuted = false;
            items = PlayerDataBlobs.ReadInventory(_data.PlayerDataFile).ItemGrid.CloneItems();
            for (int i = 0; i < items.Length; i++)
            {
                itemStack = items[i];
                if (itemStack != null && !itemStack.IsEmpty())
                {
                    itemClass = itemStack.itemValue.ItemClass;
                    string itemName = itemClass.Name ?? itemClass.GetItemName();
                    if (lstBannedItems.ContainsKey(itemName))
                    {
                        int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_data.ClientInfo);
                        if (AdminLvL > lstBannedItems[itemName])
                        {
                            //busted, take action
                            Log.Out($"[PrismaCore]Banned item ({itemName}) detected on belt of {_data.ClientInfo.playerName} ({_data.ClientInfo.PlatformId}) !!!!!");
                            Log.Out($"[PrismaCore]Permissionlevel: {AdminLvL} Item permissionlevel: {lstBannedItems[itemName]}");

                            string command = ServerCoreSettings.Instance.BannedItems_DetectedCommand;
                            if (!string.IsNullOrEmpty(command))
                            {
                                if (command.Contains(";"))
                                {
                                    //multiple commands
                                    string[] arrCommands = command.Split(';');
                                    CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                    foreach (string s in arrCommands)
                                    {
                                        string cmd = s;
                                        cmd = cmd.Replace("${steamId}", _data.ClientInfo.PlatformId.ToString());
                                        cmd = cmd.Replace("${platformId}", _data.ClientInfo.PlatformId.ToString());
                                        cmd = cmd.Replace("${entityId}", _data.ClientInfo.entityId.ToString());
                                        cmd = cmd.Replace("${playerName}", _data.ClientInfo.playerName);

                                        SdtdConsole.Instance.ExecuteAsync(cmd, iConsole);
                                    }
                                }
                                else
                                {
                                    //just 1 command
                                    command = command.Replace("${steamId}", _data.ClientInfo.PlatformId.ToString());
                                    command = command.Replace("${platformId}", _data.ClientInfo.PlatformId.ToString());
                                    command = command.Replace("${entityId}", _data.ClientInfo.entityId.ToString());
                                    command = command.Replace("${playerName}", _data.ClientInfo.playerName);

                                    CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                    SdtdConsole.Instance.ExecuteAsync(command, iConsole);
                                }

                                cmdExecuted = true;
                            }
                        }
                    }
                }
            }

            items = PlayerDataBlobs.ReadBag(_data.PlayerDataFile).ItemGrid.CloneItems();
            for (int i = 0; i < items.Length; i++)
            {
                itemStack = items[i];
                if (itemStack != null && !itemStack.IsEmpty())
                {
                    itemClass = itemStack.itemValue.ItemClass;
                    string itemName = itemClass.Name ?? itemClass.GetItemName();
                    if (lstBannedItems.ContainsKey(itemName))
                    {
                        int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_data.ClientInfo);
                        if (AdminLvL > lstBannedItems[itemName])
                        {
                            //busted, take action
                            Log.Out($"[PrismaCore]Banned item ({itemName}) detected in backpack of {_data.ClientInfo.playerName} ({_data.ClientInfo.PlatformId}) !!!!!");
                            Log.Out($"[PrismaCore]Permissionlevel: {AdminLvL} Item permissionlevel: {lstBannedItems[itemName]}");

                            string command = ServerCoreSettings.Instance.BannedItems_DetectedCommand;
                            if (!string.IsNullOrEmpty(command))
                            {
                                if(!cmdExecuted)
                                {
                                    if (command.Contains(";"))
                                    {
                                        //multiple commands
                                        string[] arrCommands = command.Split(';');
                                        CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                        foreach (string s in arrCommands)
                                        {
                                            string cmd = s;
                                            cmd = cmd.Replace("${steamId}", _data.ClientInfo.PlatformId.ToString());
                                            cmd = cmd.Replace("${platformId}", _data.ClientInfo.PlatformId.ToString());
                                            cmd = cmd.Replace("${entityId}", _data.ClientInfo.entityId.ToString());
                                            cmd = cmd.Replace("${playerName}", _data.ClientInfo.playerName);

                                            SdtdConsole.Instance.ExecuteAsync(cmd, iConsole);
                                        }
                                    }
                                    else
                                    {
                                        //just 1 command
                                        command = command.Replace("${steamId}", _data.ClientInfo.PlatformId.ToString());
                                        command = command.Replace("${platformId}", _data.ClientInfo.PlatformId.ToString());
                                        command = command.Replace("${entityId}", _data.ClientInfo.entityId.ToString());
                                        command = command.Replace("${playerName}", _data.ClientInfo.playerName);

                                        CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                        SdtdConsole.Instance.ExecuteAsync(command, iConsole);
                                    }

                                    cmdExecuted = true;
                                }
                            }
                        }
                    }
                }
            }

            Equipment equipment = PlayerDataBlobs.ReadEquipment(_data.PlayerDataFile);
            int eqCount = equipment.GetSlotCount();
            for (int i = 0; i < eqCount; i++)
            {
                itemValue = equipment.GetSlotItem(i);
                if (itemValue != null && !itemValue.IsEmpty())
                {
                    itemClass = itemValue.ItemClass;
                    string itemName = itemClass.Name ?? itemClass.GetItemName();
                    if (lstBannedItems.ContainsKey(itemName))
                    {
                        int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_data.ClientInfo);
                        if (AdminLvL > lstBannedItems[itemName])
                        {
                            //busted, take action
                            Log.Out($"[PrismaCore]Banned item ({itemName}) detected in equipment of {_data.ClientInfo.playerName} ({_data.ClientInfo.PlatformId}) !!!!!");
                            Log.Out($"[PrismaCore]Permissionlevel: {AdminLvL} Item permissionlevel: {lstBannedItems[itemName]}");

                            string command = ServerCoreSettings.Instance.BannedItems_DetectedCommand;
                            if (!string.IsNullOrEmpty(command))
                            {
                                if(!cmdExecuted)
                                {
                                    if (command.Contains(";"))
                                    {
                                        //multiple commands
                                        string[] arrCommands = command.Split(';');
                                        CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                        foreach (string s in arrCommands)
                                        {
                                            string cmd = s;
                                            cmd = cmd.Replace("${steamId}", _data.ClientInfo.PlatformId.ToString());
                                            cmd = cmd.Replace("${platformId}", _data.ClientInfo.PlatformId.ToString());
                                            cmd = cmd.Replace("${entityId}", _data.ClientInfo.entityId.ToString());
                                            cmd = cmd.Replace("${playerName}", _data.ClientInfo.playerName);

                                            SdtdConsole.Instance.ExecuteAsync(cmd, iConsole);
                                        }
                                    }
                                    else
                                    {
                                        //just 1 command
                                        command = command.Replace("${steamId}", _data.ClientInfo.PlatformId.ToString());
                                        command = command.Replace("${platformId}", _data.ClientInfo.PlatformId.ToString());
                                        command = command.Replace("${entityId}", _data.ClientInfo.entityId.ToString());
                                        command = command.Replace("${playerName}", _data.ClientInfo.playerName);

                                        CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                        SdtdConsole.Instance.ExecuteAsync(command, iConsole);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void HandleVIPGuardItems(ModEvents.SSavePlayerDataData _data)
        {
            //armor
            Equipment eq = PlayerDataBlobs.ReadEquipment(_data.PlayerDataFile);
            List<ItemValue> armorItems = eq.GetArmor();

            foreach (ItemValue armorItem in armorItems)
            {
                if (armorItem.HasModSlots && armorItem.HasMods())
                {
                    ItemValue[] _parts = Modifications(armorItem);

                    if (_parts != null && _parts.Length > 0)
                    {
                        for (int i = 0; i < _parts.Length; i++)
                        {
                            if (_parts[i] != null)
                            {
                                if (_parts[i].type != ItemValue.None.type)
                                {
                                    ItemClass ib = ItemClass.list[_parts[i].type];
                                    string itemName = ib.GetItemName();
                                    if (RegionReset.lstVIPModGuardItems.Contains(itemName.Trim()))
                                    {
                                        int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_data.ClientInfo);

                                        if (AdminLvL > ServerCoreSettings.Instance.VIPModGuard_ExcludeAdminLvl)
                                        {
                                            //Log.Out($"[PrismaCore]VIP mod on {_cInfo.entityId} Mod: {itemName}");
                                            if (lstVipModUsers.ContainsKey(_data.ClientInfo.entityId))
                                            {
                                                lstVipModUsers[_data.ClientInfo.entityId] = itemName;
                                            }
                                            else
                                            {
                                                lstVipModUsers.Add(_data.ClientInfo.entityId, itemName);
                                            }
                                        }

                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //belt
            ItemStack[] itemStackBelt = PlayerDataBlobs.ReadInventory(_data.PlayerDataFile).ItemGrid.CloneItems();

            for (int i = 0; i < itemStackBelt.Length; i++)
            {
                if (itemStackBelt[i] != null)
                {
                    if (itemStackBelt[i].itemValue.type != ItemValue.None.type)
                    {
                        if (itemStackBelt[i].itemValue.HasModSlots && itemStackBelt[i].itemValue.HasMods())
                        {
                            ItemValue[] _parts = Modifications(itemStackBelt[i].itemValue);

                            if (_parts != null && _parts.Length > 0)
                            {
                                for (int j = 0; j < _parts.Length; j++)
                                {
                                    if (_parts[j] != null)
                                    {
                                        if (_parts[j].type != ItemValue.None.type)
                                        {
                                            ItemClass ib = ItemClass.list[_parts[j].type];
                                            string itemName = ib.GetItemName();
                                            if (RegionReset.lstVIPModGuardItems.Contains(itemName.Trim()))
                                            {
                                                int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_data.ClientInfo);

                                                if (AdminLvL > ServerCoreSettings.Instance.VIPModGuard_ExcludeAdminLvl)
                                                {
                                                    //Log.Out($"[PrismaCore]VIP mod on {_cInfo.entityId} Mod: {itemName}");
                                                    if (lstVipModUsers.ContainsKey(_data.ClientInfo.entityId))
                                                    {
                                                        lstVipModUsers[_data.ClientInfo.entityId] = itemName;
                                                    }
                                                    else
                                                    {
                                                        lstVipModUsers.Add(_data.ClientInfo.entityId, itemName);
                                                    }
                                                }

                                                return;
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }

            //backpack
            ItemStack[] itemStack = PlayerDataBlobs.ReadBag(_data.PlayerDataFile).ItemGrid.CloneItems();

            for (int i = 0; i < itemStack.Length; i++)
            {
                if (itemStack[i] != null)
                {
                    if (itemStack[i].itemValue.type != ItemValue.None.type)
                    {
                        if (itemStack[i].itemValue.HasModSlots && itemStack[i].itemValue.HasMods())
                        {
                            ItemValue[] _parts = Modifications(itemStack[i].itemValue);

                            if (_parts != null && _parts.Length > 0)
                            {
                                for (int j = 0; j < _parts.Length; j++)
                                {
                                    if (_parts[j] != null)
                                    {
                                        if (_parts[j].type != ItemValue.None.type)
                                        {
                                            ItemClass ib = ItemClass.list[_parts[j].type];
                                            string itemName = ib.GetItemName();

                                            if (RegionReset.lstVIPModGuardItems.Contains(itemName.Trim()))
                                            {
                                                int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(_data.ClientInfo);

                                                if (AdminLvL > ServerCoreSettings.Instance.VIPModGuard_ExcludeAdminLvl)
                                                {
                                                    //Log.Out($"[PrismaCore]VIP mod on {_cInfo.entityId} Mod: {itemName}");
                                                    if (lstVipModUsers.ContainsKey(_data.ClientInfo.entityId))
                                                    {
                                                        lstVipModUsers[_data.ClientInfo.entityId] = itemName;
                                                    }
                                                    else
                                                    {
                                                        lstVipModUsers.Add(_data.ClientInfo.entityId, itemName);
                                                    }
                                                }

                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void LoadRegions()
        {
            if (!File.Exists(RegionFile))
            {
                using (File.Create(RegionFile)) { }
                Log.Out("[PrismaCore] Created new empty regions.txt in " + RegionPath);
            }
            else
            {
                var regionContent = File.ReadAllLines(RegionFile);
                lstRegions = new List<string>(regionContent);
                SyncRegionClaims();
            }
        }

        public static void LoadVIPGuardItems()
        {
            if (!SdFile.Exists(VIPModGuardItemsFile))
            {
                using (SdFile.Create(VIPModGuardItemsFile)) { }
                Log.Out("[PrismaCore] Created new empty VIPModGuardItems.txt in " + API.GamePath);
            }
            else
            {
                var regionContent = SdFile.ReadAllLines(VIPModGuardItemsFile);
                lstVIPModGuardItems = new List<string>(regionContent);
                Log.Out("[PrismaCore] Loaded VIPModGuardItems");
            }
        }

        public static void LoadPrefabExceptons()
        {
            if (!File.Exists(PrefabExceptionFile))
            {
                using (File.Create(PrefabExceptionFile)) { }
                Log.Out("[PrismaCore] Created new file: " + PrefabExceptionFile);
            }
            else
            {
                var prefabContent = File.ReadAllLines(PrefabExceptionFile);
                lstPrefabExceptions = new List<string>(prefabContent);
            }
        }

        public static void LoadQuestPoiExceptons()
        {
            if (!File.Exists(QuestPoiExceptionFile))
            {
                using (File.Create(QuestPoiExceptionFile)) { }
                Log.Out("[PrismaCore] Created new file:  " + QuestPoiExceptionFile);
            }
            else
            {
                var prefabContent = File.ReadAllLines(QuestPoiExceptionFile);
                lstQuestPoiExceptions = new List<string>(prefabContent);
            }
        }

        public static void LoadAllPoiExceptons()
        {
            if (!File.Exists(AllPoiExceptionFile))
            {
                using (File.Create(AllPoiExceptionFile)) { }
                Log.Out("[PrismaCore] Created new file:  " + AllPoiExceptionFile);
            }
            else
            {
                var prefabContent = File.ReadAllLines(AllPoiExceptionFile);
                lstAllPoiExceptions = new List<string>(prefabContent);
            }
        }

        public static void SyncRegionClaims()
        {
            List<DbClaim> lstClaims = Database.Instance.GetAllDbClaims();

            //read Notification.txt 
            string notifyText = string.Empty;

            //add all that are missing
            bool foundClaim = false;

            foreach (string rg in lstRegions)
            {
                foundClaim = false;
                foreach (DbClaim activeClaim in lstClaims)
                {
                    if (activeClaim == null) continue;

                    if (activeClaim.Id.EqualsCaseInsensitive(rg.Trim()))
                    {
                        foundClaim = true;
                    }
                }

                if (!foundClaim)
                {
                    // no notify claim present -> make it
                    // region to boundary

                    string[] arr = rg.Trim().Split('.');
                    string sLat = arr[1];
                    string sLng = arr[2];

                    int.TryParse(sLat, out int lat);
                    int.TryParse(sLng, out int lng);

                    notifyText = $"{ServerCoreStrings.Instance.Resetregion_EnterNotification}:{ServerCoreStrings.Instance.Resetregion_ExitNotification}";

                    var dbclaim = new DbClaim
                    {
                        Id = rg.Trim(),
                        W_bound = (lat * 512),
                        E_bound = (lat * 512) + 512,
                        N_bound = (lng * 512) + 512,
                        S_bound = (lng * 512),
                        AccessLevel = 0,
                        Type = "notify:" + notifyText,
                        Whitelist = ""
                    };

                    Database.Instance.SaveDbClaim(dbclaim);
                }
            }

            lstClaims = Database.Instance.GetAllDbClaims();
            //remove all that are obsolete

            foreach (DbClaim activeClaim in lstClaims)
            {
                if (activeClaim == null) continue;

                if (activeClaim.Id.StartsWith("r.") && activeClaim.Id.EndsWith(".7rg"))
                {
                    foundClaim = false;
                    foreach (string rg in lstRegions)
                    {
                        if (activeClaim.Id.EqualsCaseInsensitive(rg.Trim()))
                        {
                            foundClaim = true;
                        }
                    }
                    if (!foundClaim)
                    {
                        // remove the claim from persistent data
                        Database.Instance.DeleteDbClaim(activeClaim.Id);
                    }
                }
            }
        }

        public static void SaveBannedItemsList()
        {
            using (TextWriter tw = new StreamWriter(BannedItemsFile))
            {
                foreach (KeyValuePair<string,int> kvp in lstBannedItems)
                {
                    tw.WriteLine($"{kvp.Key}:{kvp.Value}");
                }
            }

            return;
        }

        public static void SaveRegionsList()
        {
            RegionWatcher.fsw.EnableRaisingEvents = false;

            using (TextWriter tw = new StreamWriter(RegionFile))
            {
                foreach (string s in lstRegions)
                    tw.WriteLine(s);
            }

            SyncRegionClaims();

            RegionWatcher.fsw.EnableRaisingEvents = true;
            return;
        }

        public static bool HandleLCB(Vector3i _blockPos, BlockValue _blockValue, PlatformUserIdentifierAbs playerId)
        {
            //lcb -> check questpoi, resetregion and lcbfree adv. claim

            lstClaims = Database.Instance.GetAllDbClaims();
            ClientInfo clientinfo = ConsoleHelper.ParseParamIdOrName(playerId.ToString());
            int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(clientinfo);

            //check if any claims in lstClaims else skip resetregion and lcbfree
            if (lstClaims != null && lstClaims.Count != 0)
            {
                int LCBsize = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("LandClaimSize"));
                decimal d = (LCBsize - 1) / 2;
                int halfLCB = Convert.ToInt32(Math.Floor(d));

                foreach (DbClaim activeClaim in lstClaims)
                {
                    if (activeClaim == null) continue;

                    if (activeClaim.Id.StartsWith("r.") && activeClaim.Id.EndsWith(".7rg"))
                    {
                        if (_blockPos.x >= activeClaim.W_bound && _blockPos.x <= activeClaim.E_bound && _blockPos.z >= activeClaim.S_bound && _blockPos.z <= activeClaim.N_bound)
                        {
                            if (_blockPos.x != 0 && _blockPos.z != 0)
                            {
                                if (clientinfo != null)
                                {
                                    GiveItem(clientinfo, "keystoneBlock", 1);
                                    Log.Out($"[PrismaCore] ResetRegions: LCB given back to player {clientinfo.playerName}");
                                    if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_lcbinregion"))
                                    {
                                        EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                        showTooltip(ep, "prismacore_tooltip_lcbinregion");
                                    }
                                    else
                                    {
                                        clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.Resetregion_LCBinRegion), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                    }
                                }

                                List<BlockChangeInfo> changes = new List<BlockChangeInfo>();

                                BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                changes.Add(bciAnti);
                                GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                try
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                catch
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                finally { }

                                Log.Out("[PrismaCore] ResetRegions: LCB at (" + _blockPos.ToString() + ") removed.");

                                return true;
                            }
                        }
                        else if (_blockPos.x + halfLCB >= activeClaim.W_bound && _blockPos.x - halfLCB <= activeClaim.E_bound && _blockPos.z >= activeClaim.S_bound && _blockPos.z <= activeClaim.N_bound)
                        {
                            //border inside resetregion
                            if (_blockPos.x != 0 && _blockPos.z != 0)
                            {
                                if (clientinfo != null)
                                {
                                    GiveItem(clientinfo, "keystoneBlock", 1);
                                    Log.Out($"[PrismaCore] ResetRegions: LCB given back to player {clientinfo.playerName}");
                                    if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_lcb2close"))
                                    {
                                        EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                        showTooltip(ep, "prismacore_tooltip_lcb2close");
                                    }
                                    else
                                    {
                                        clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.Resetregion_LCB2Close), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                    }
                                }

                                List<BlockChangeInfo> changes = new List<BlockChangeInfo>();

                                BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                changes.Add(bciAnti);
                                GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                try
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                catch
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                finally { }

                                Log.Out("[PrismaCore] ResetRegions: LCB at (" + _blockPos.ToString() + ") removed.");

                                return true;
                            }
                        }
                        else if (_blockPos.x >= activeClaim.W_bound && _blockPos.x <= activeClaim.E_bound && _blockPos.z + halfLCB >= activeClaim.S_bound && _blockPos.z - halfLCB <= activeClaim.N_bound)
                        {
                            //border inside resetregion
                            if (_blockPos.x != 0 && _blockPos.z != 0)
                            {
                                if (clientinfo != null)
                                {
                                    GiveItem(clientinfo, "keystoneBlock", 1);
                                    Log.Out($"[PrismaCore] ResetRegions: LCB given back to player {clientinfo.playerName}");
                                    if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_lcb2close"))
                                    {
                                        EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                        showTooltip(ep, "prismacore_tooltip_lcb2close");
                                    }
                                    else
                                    {
                                        clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.Resetregion_LCB2Close), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                    }
                                }

                                List<BlockChangeInfo> changes = new List<BlockChangeInfo>();

                                BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                changes.Add(bciAnti);
                                GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                try
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                catch
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                finally { }

                                Log.Out("[PrismaCore] ResetRegions: LCB at (" + _blockPos.ToString() + ") removed.");

                                return true;
                            }
                        }
                        else if (_blockPos.x - halfLCB >= activeClaim.W_bound && _blockPos.x - halfLCB <= activeClaim.E_bound && _blockPos.z - halfLCB >= activeClaim.S_bound && _blockPos.z - halfLCB <= activeClaim.N_bound)
                        {
                            //NE
                            if (_blockPos.x != 0 && _blockPos.z != 0)
                            {
                                if (clientinfo != null)
                                {
                                    GiveItem(clientinfo, "keystoneBlock", 1);
                                    Log.Out($"[PrismaCore] ResetRegions: LCB given back to player {clientinfo.playerName}");
                                    if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_lcb2close"))
                                    {
                                        EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                        showTooltip(ep, "prismacore_tooltip_lcb2close");
                                    }
                                    else
                                    {
                                        clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.Resetregion_LCB2Close), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                    }
                                }

                                List<BlockChangeInfo> changes = new List<BlockChangeInfo>();

                                BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                changes.Add(bciAnti);
                                GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                try
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                catch
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                finally { }

                                Log.Out("[PrismaCore] ResetRegions: LCB at (" + _blockPos.ToString() + ") removed.");

                                return true;
                            }
                        }
                        else if (_blockPos.x + halfLCB >= activeClaim.W_bound && _blockPos.x + halfLCB <= activeClaim.E_bound && _blockPos.z - halfLCB >= activeClaim.S_bound && _blockPos.z - halfLCB <= activeClaim.N_bound)
                        {
                            //NW
                            if (_blockPos.x != 0 && _blockPos.z != 0)
                            {
                                if (clientinfo != null)
                                {
                                    GiveItem(clientinfo, "keystoneBlock", 1);
                                    Log.Out($"[PrismaCore] ResetRegions: LCB given back to player {clientinfo.playerName}");
                                    if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_lcb2close"))
                                    {
                                        EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                        showTooltip(ep, "prismacore_tooltip_lcb2close");
                                    }
                                    else
                                    {
                                        clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.Resetregion_LCB2Close), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                    }
                                }

                                List<BlockChangeInfo> changes = new List<BlockChangeInfo>();

                                BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                changes.Add(bciAnti);
                                GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                try
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                catch
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                finally { }

                                Log.Out("[PrismaCore] ResetRegions: LCB at (" + _blockPos.ToString() + ") removed.");

                                return true;
                            }
                        }
                        else if (_blockPos.x + halfLCB >= activeClaim.W_bound && _blockPos.x + halfLCB <= activeClaim.E_bound && _blockPos.z + halfLCB >= activeClaim.S_bound && _blockPos.z + halfLCB <= activeClaim.N_bound)
                        {
                            //SW
                            if (_blockPos.x != 0 && _blockPos.z != 0)
                            {
                                if (clientinfo != null)
                                {
                                    GiveItem(clientinfo, "keystoneBlock", 1);
                                    Log.Out($"[PrismaCore] ResetRegions: LCB given back to player {clientinfo.playerName}");
                                    if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_lcb2close"))
                                    {
                                        EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                        showTooltip(ep, "prismacore_tooltip_lcb2close");
                                    }
                                    else
                                    {
                                        clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.Resetregion_LCB2Close), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                    }
                                }

                                List<BlockChangeInfo> changes = new List<BlockChangeInfo>();

                                BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                changes.Add(bciAnti);
                                GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                try
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                catch
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                finally { }

                                Log.Out("[PrismaCore] ResetRegions: LCB at (" + _blockPos.ToString() + ") removed.");

                                return true;
                            }
                        }
                        else if (_blockPos.x - halfLCB >= activeClaim.W_bound && _blockPos.x - halfLCB <= activeClaim.E_bound && _blockPos.z + halfLCB >= activeClaim.S_bound && _blockPos.z + halfLCB <= activeClaim.N_bound)
                        {
                            //SE
                            if (_blockPos.x != 0 && _blockPos.z != 0)
                            {
                                if (clientinfo != null)
                                {
                                    GiveItem(clientinfo, "keystoneBlock", 1);
                                    Log.Out($"[PrismaCore] ResetRegions: LCB given back to player {clientinfo.playerName}");
                                    if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_lcb2close"))
                                    {
                                        EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                        showTooltip(ep, "prismacore_tooltip_lcb2close");
                                    }
                                    else
                                    {
                                        clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.Resetregion_LCB2Close), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                    }
                                }

                                List<BlockChangeInfo> changes = new List<BlockChangeInfo>();

                                BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                changes.Add(bciAnti);
                                GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                try
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                catch
                                {
                                    GameManager.Instance.SetBlocksRPC(changes);
                                }
                                finally { }

                                Log.Out("[PrismaCore] ResetRegions: LCB at (" + _blockPos.ToString() + ") removed.");

                                return true;
                            }
                        }
                    }
                }

                //Adv. Claim LcbFree
                foreach (DbClaim activeClaim in lstClaims)
                {
                    if (activeClaim == null) continue;

                    if (activeClaim.Type.Trim().EqualsCaseInsensitive("lcbfree"))
                    {
                        if (_blockPos.x >= activeClaim.W_bound && _blockPos.x <= activeClaim.E_bound && _blockPos.z >= activeClaim.S_bound && _blockPos.z <= activeClaim.N_bound)
                        {
                            if (AdminLvL <= activeClaim.AccessLevel || activeClaim.Whitelist.Contains(clientinfo.PlatformId.ToString()) || activeClaim.Id.Contains(clientinfo.PlatformId.ToString()))
                            {
                                continue;
                            }
                            else
                            {
                                if (_blockPos.x != 0 && _blockPos.z != 0)
                                {
                                    if (clientinfo != null)
                                    {
                                        //give back to player
                                        if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_lcbfree"))
                                        {
                                            EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                            showTooltip(ep, "prismacore_tooltip_lcbfree");
                                        }
                                        else
                                        {
                                            clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.AdvClaims_LcbFree), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                        }

                                        GiveItem(clientinfo, "keystoneBlock", 1);
                                        Log.Out($"[PrismaCore] Adv. Claim LcbFree: LCB given back to player {clientinfo.playerName}");
                                    }

                                    List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                    BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                    changes.Add(bciAnti);
                                    GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                    try
                                    {
                                        GameManager.Instance.SetBlocksRPC(changes);
                                    }
                                    catch { GameManager.Instance.SetBlocksRPC(changes); }
                                    finally { }

                                    Log.Out("[PrismaCore] Adv. Claim LcbFree: LCB at (" + _blockPos.ToString() + ") removed.");

                                    return true;

                                }
                            }
                        }
                    }
                }
            }

            if (ServerCoreSettings.Instance.AllPoiProtection_Enabled)
            {
                //AllPoiProtection

                DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
                LoadAllPoiExceptons();

                if (dynamicPrefabDecorator != null)
                {
                    List<PrefabInstance> allPrefabs = new List<PrefabInstance>();
                    dynamicPrefabDecorator.GetWorldPrefabs(allPrefabs);

                    for (int i = 0; i < allPrefabs.Count; i++)
                    {
                        PrefabInstance fab = allPrefabs[i];
                        if (fab != null)
                        {
                            if (lstAllPoiExceptions.ContainsCaseInsensitive(fab.name) || lstAllPoiExceptions.ContainsCaseInsensitive(fab.location.Name))
                            {
                                continue;
                            }

                            int int2 = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("LandClaimSize"));
                            int num2 = int2 / 2;
                            Vector3i BoxMin = fab.boundingBoxPosition;
                            Vector3i BoxMax = fab.boundingBoxPosition + fab.boundingBoxSize;

                            Vector3i p = new Vector3i();
                            p.x = _blockPos.x - num2;
                            p.z = _blockPos.z - num2;

                            if (p.x <= BoxMax.x && p.x + int2 >= BoxMin.x && p.z <= BoxMax.z && p.z + int2 >= BoxMin.z)
                            {
                                if (_blockPos.x != 0 && _blockPos.z != 0)
                                {
                                    if (clientinfo != null)
                                    {
                                        if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_allpoiprotectionlcb"))
                                        {
                                            EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                            showTooltip(ep, "prismacore_tooltip_allpoiprotectionlcb");
                                        }
                                        else
                                        {
                                            clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.AllPoiProtection_LcbMessage), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                        }

                                        GiveItem(clientinfo, "keystoneBlock", 1);
                                        Log.Out($"[PrismaCore] AllPoiProtection: LCB given back to player {clientinfo.playerName}");
                                    }

                                    List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                    BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                    changes.Add(bciAnti);

                                    GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                    try
                                    {
                                        GameManager.Instance.SetBlocksRPC(changes);
                                    }
                                    catch { GameManager.Instance.SetBlocksRPC(changes); }
                                    finally { }

                                    Log.Out("[PrismaCore] AllPoiProtection: LCB at (" + _blockPos.ToString() + ") removed.");

                                    return true;
                                }
                            }

                        }
                    }//end of prefabloop
                }
            }
            else if (ServerCoreSettings.Instance.QuestPoiProtection_Enabled)
            {
                //QuestPoiProtection

                DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
                LoadQuestPoiExceptons();

                if (dynamicPrefabDecorator != null)
                {
                    List<PrefabInstance> allPrefabs = new List<PrefabInstance>();
                    dynamicPrefabDecorator.GetWorldPrefabs(allPrefabs);

                    for (int i = 0; i < allPrefabs.Count; i++)
                    {
                        PrefabInstance fab = allPrefabs[i];
                        if (fab != null)
                        {
                            if (fab.prefab.HasQuestTag() && fab.prefab.DifficultyTier > 0)
                            {
                                if (lstQuestPoiExceptions.ContainsCaseInsensitive(fab.name) || lstQuestPoiExceptions.ContainsCaseInsensitive(fab.location.Name))
                                {
                                    continue;
                                }

                                int int2 = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("LandClaimSize"));
                                int num2 = int2 / 2;
                                Vector3i BoxMin = fab.boundingBoxPosition;
                                Vector3i BoxMax = fab.boundingBoxPosition + fab.boundingBoxSize;

                                Vector3i p = new Vector3i();
                                p.x = _blockPos.x - num2;
                                p.z = _blockPos.z - num2;

                                if (p.x <= BoxMax.x && p.x + int2 >= BoxMin.x && p.z <= BoxMax.z && p.z + int2 >= BoxMin.z)
                                {
                                    if (_blockPos.x != 0 && _blockPos.z != 0)
                                    {
                                        if (clientinfo != null)
                                        {
                                            if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_questpoiprotectionlcb"))
                                            {
                                                EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                                showTooltip(ep, "prismacore_tooltip_questpoiprotectionlcb");
                                            }
                                            else
                                            {
                                                clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.QuestPoiProtection_LcbMessage), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                            }

                                            GiveItem(clientinfo, "keystoneBlock", 1);
                                            Log.Out($"[PrismaCore] QuestpoiProtection: LCB given back to player {clientinfo.playerName}");
                                        }

                                        List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                        BlockChangeInfo bciAnti = new BlockChangeInfo(new BlockValueRef(_blockPos), new BlockValue(0), true, false);

                                        changes.Add(bciAnti);

                                        GameManager.Instance.GetPersistentPlayerList().RemoveLandProtectionBlock(_blockPos);

                                        try
                                        {
                                            GameManager.Instance.SetBlocksRPC(changes);
                                        }
                                        catch { GameManager.Instance.SetBlocksRPC(changes); }
                                        finally { }

                                        Log.Out("[PrismaCore] QuestpoiProtection: LCB at (" + _blockPos.ToString() + ") removed.");

                                        return true;
                                    }
                                }
                            }
                        }
                    }//end of prefabloop
                }
            }

            return false;
        }

        public static void CheckAntiBlock(PlatformUserIdentifierAbs playerId, List<BlockChangeInfo> bci)
        {
            if (bci.Count == 0)
            {
                return;
            }

            lstClaims = Database.Instance.GetAllDbClaims();
            ClientInfo clientinfo = ConsoleHelper.ParseParamIdOrName(playerId.ToString());
            int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(clientinfo);
            
            foreach (BlockChangeInfo bc in bci)
            {
                if (bc != null)
                {
                    //problock and landclaim check
                    bool removed = false;

                    foreach (DbClaim activeClaim in lstClaims)
                    {
                        if (activeClaim == null) continue;

                        if (activeClaim.Type.Trim().ContainsCaseInsensitive("problock:"))
                        {
                            if (bc.blockValueRef.BlockPosition.x >= activeClaim.W_bound && bc.blockValueRef.BlockPosition.x <= activeClaim.E_bound && bc.blockValueRef.BlockPosition.z >= activeClaim.S_bound && bc.blockValueRef.BlockPosition.z <= activeClaim.N_bound)
                            {
                                if (AdminLvL <= activeClaim.AccessLevel || activeClaim.Whitelist.Contains(clientinfo.PlatformId.ToString()) || activeClaim.Id.Contains(clientinfo.PlatformId.ToString()))
                                {
                                    continue;
                                }
                                else
                                {
                                    if (bc.blockValueRef.BlockPosition.x != 0 && bc.blockValueRef.BlockPosition.z != 0)
                                    {
                                        if (bc.blockValue.Block != null && !activeClaim.Type.Trim().Contains(bc.blockValue.Block.GetBlockName()))
                                        {
                                            if (clientinfo != null)
                                            {
                                                //give back to
                                                if (BuffManager.Buffs.ContainsKey($"prismacore_tooltip_{activeClaim.Id}"))
                                                {
                                                    EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                                    showTooltip(ep, $"prismacore_tooltip_{activeClaim.Id}");
                                                }
                                                else
                                                {
                                                    string proBlocks = activeClaim.Type.Trim().Split(':')[1];
                                                    string msg = ServerCoreStrings.Instance.AdvClaims_ProBlock.Replace("{proBlocks}", proBlocks);
                                                    clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, msg), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                                }

                                                GiveItem(clientinfo, bc.blockValue.Block.GetBlockName(), 1);
                                                Log.Out($"[PrismaCore] Adv. Claim ProBlock: Illegal block ({bc.blockValue.Block.GetBlockName()}) given back to player {clientinfo.playerName}");
                                            }

                                            List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                            BlockChangeInfo bciPro = new BlockChangeInfo(bc.blockValueRef, new BlockValue(0), true, false);

                                            changes.Add(bciPro);

                                            try
                                            {
                                                GameManager.Instance.SetBlocksRPC(changes);
                                            }
                                            catch { GameManager.Instance.SetBlocksRPC(changes); }
                                            finally { }

                                            Log.Out("[PrismaCore] Adv. Claim ProBlock: Block at (" + bc.blockValueRef.BlockPosition.ToString() + ") removed.");

                                            removed = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (activeClaim.Type.Trim().EqualsCaseInsensitive("landclaim"))
                        {
                            if (bc.blockValueRef.BlockPosition.x >= activeClaim.W_bound && bc.blockValueRef.BlockPosition.x <= activeClaim.E_bound && bc.blockValueRef.BlockPosition.z >= activeClaim.S_bound && bc.blockValueRef.BlockPosition.z <= activeClaim.N_bound)
                            {
                                if (AdminLvL <= activeClaim.AccessLevel || activeClaim.Whitelist.Contains(clientinfo.PlatformId.ToString()) || activeClaim.Id.Contains(clientinfo.PlatformId.ToString()))
                                {
                                    continue;
                                }
                                else
                                {
                                    if (bc.blockValueRef.BlockPosition.x != 0 && bc.blockValueRef.BlockPosition.z != 0)
                                    {
                                        if (clientinfo != null)
                                        {
                                            //give back to player
                                            if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_landclaim"))
                                            {
                                                EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                                showTooltip(ep, "prismacore_tooltip_landclaim");
                                            }
                                            else
                                            {
                                                string msg = ServerCoreStrings.Instance.AdvClaims_Landclaim;
                                                clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, msg), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                            }

                                            GiveItem(clientinfo, bc.blockValue.Block.GetBlockName(), 1);
                                            Log.Out($"[PrismaCore] Adv. Claim Landclaim: Illegal block ({bc.blockValue.Block.GetBlockName()}) given back to player {clientinfo.playerName}");
                                        }

                                        List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                        BlockChangeInfo bciLandclaim = new BlockChangeInfo(bc.blockValueRef, new BlockValue(0), true, false);

                                        changes.Add(bciLandclaim);

                                        try
                                        {
                                            GameManager.Instance.SetBlocksRPC(changes);
                                        }
                                        catch { GameManager.Instance.SetBlocksRPC(changes); }
                                        finally { }

                                        Log.Out("[PrismaCore] Adv. Claim Landclaim: Block at (" + bc.blockValueRef.BlockPosition.ToString() + ") removed.");

                                        removed = true;
                                        break;

                                    }
                                }
                            }
                        }
                    }

                    if (removed)
                    {
                        continue;
                    }
                                       
                    else if (bc.blockValue.Block is BlockSleepingBag)
                    {
                        //bedroll -> check questpoi/allpoi + check if bedroll is an antiblock first!

                        //check if bedroll is antiblock
                        if (lstClaims != null && lstClaims.Count != 0)
                        {
                            bool blockRemoved = false;

                            foreach (DbClaim activeClaim in lstClaims)
                            {
                                if (activeClaim == null) continue;

                                if (activeClaim.Type.Trim().ContainsCaseInsensitive("antiblock:"))
                                {
                                    if (bc.blockValueRef.BlockPosition.x >= activeClaim.W_bound && bc.blockValueRef.BlockPosition.x <= activeClaim.E_bound && bc.blockValueRef.BlockPosition.z >= activeClaim.S_bound && bc.blockValueRef.BlockPosition.z <= activeClaim.N_bound)
                                    {
                                        if (AdminLvL <= activeClaim.AccessLevel || activeClaim.Whitelist.Contains(clientinfo.PlatformId.ToString()) || activeClaim.Id.Contains(clientinfo.PlatformId.ToString()))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            if (bc.blockValueRef.BlockPosition.x != 0 && bc.blockValueRef.BlockPosition.z != 0)
                                            {
                                                if (bc.blockValue.Block != null && activeClaim.Type.Trim().Contains(bc.blockValue.Block.GetBlockName()))
                                                {
                                                    if (clientinfo != null)
                                                    {
                                                        if (BuffManager.Buffs.ContainsKey($"prismacore_tooltip_{activeClaim.Id}"))
                                                        {
                                                            EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                                            showTooltip(ep, $"prismacore_tooltip_{activeClaim.Id}");
                                                        }
                                                        else
                                                        {
                                                            string msg = ServerCoreStrings.Instance.AdvClaims_AntiBlock.Replace("{blockName}", bc.blockValue.Block.GetBlockName());
                                                            clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, msg), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                                        }
                                                        GiveItem(clientinfo, bc.blockValue.Block.GetBlockName(), 1);
                                                        Log.Out($"[PrismaCore] Adv. Claim AntiBlock: Illegal block ({bc.blockValue.Block.GetBlockName()}) detected from player {clientinfo.playerName}");
                                                    }

                                                    List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                                    BlockChangeInfo bciAnti = new BlockChangeInfo(bc.blockValueRef, new BlockValue(0), true, false);

                                                    changes.Add(bciAnti);

                                                    GameManager.Instance.GetPersistentPlayerList().SpawnPointRemoved(bc.blockValueRef.BlockPosition);

                                                    try
                                                    {
                                                        GameManager.Instance.SetBlocksRPC(changes);
                                                    }
                                                    catch { GameManager.Instance.SetBlocksRPC(changes); }
                                                    finally { }

                                                    Log.Out("[PrismaCore] Adv. Claim AntiBlock: Block at (" + bc.blockValueRef.BlockPosition.ToString() + ") removed.");

                                                    blockRemoved = true;

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (blockRemoved)
                            {
                                continue;
                            }
                        }

                        if (ServerCoreSettings.Instance.AllPoiProtection_Enabled)
                        {
                            //check for allpois

                            DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
                            LoadAllPoiExceptons();

                            if (dynamicPrefabDecorator != null)
                            {
                                List<PrefabInstance> allPrefabs = new List<PrefabInstance>();
                                dynamicPrefabDecorator.GetWorldPrefabs(allPrefabs);

                                for (int i = 0; i < allPrefabs.Count; i++)
                                {
                                    PrefabInstance fab = allPrefabs[i];
                                    if (fab != null)
                                    {
                                        if (lstAllPoiExceptions.ContainsCaseInsensitive(fab.name) || lstAllPoiExceptions.ContainsCaseInsensitive(fab.location.Name))
                                        {
                                            continue;
                                        }

                                        int deadsize = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("BedrollDeadZoneSize"));
                                        Vector3i BoxMin = fab.boundingBoxPosition;
                                        Vector3i BoxMax = fab.boundingBoxPosition + fab.boundingBoxSize;

                                        Vector3i expand = new Vector3i(deadsize, deadsize, deadsize);
                                        Vector3i BoxMinNew = BoxMin - expand;
                                        Vector3i BoxMaxNew = BoxMax + expand;

                                        if (bc.blockValueRef.BlockPosition.x >= BoxMinNew.x && bc.blockValueRef.BlockPosition.x < BoxMaxNew.x && bc.blockValueRef.BlockPosition.y >= BoxMinNew.y && bc.blockValueRef.BlockPosition.y < BoxMaxNew.y && bc.blockValueRef.BlockPosition.z >= BoxMinNew.z && bc.blockValueRef.BlockPosition.z < BoxMaxNew.z)
                                        {
                                            if (bc.blockValueRef.BlockPosition.x != 0 && bc.blockValueRef.BlockPosition.z != 0)
                                            {
                                                if (clientinfo != null)
                                                {
                                                    if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_allpoiprotectionbed"))
                                                    {
                                                        EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                                        showTooltip(ep, "prismacore_tooltip_allpoiprotectionbed");
                                                    }
                                                    else
                                                    {
                                                        clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.AllPoiProtection_BedMessage), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                                    }

                                                    GiveItem(clientinfo, "bedroll", 1);
                                                    Log.Out($"[PrismaCore] AllPoiProtection: Bedroll given back to player {clientinfo.playerName}");
                                                }

                                                List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                                BlockChangeInfo bciAnti = new BlockChangeInfo(bc.blockValueRef, new BlockValue(0), true, false);

                                                changes.Add(bciAnti);
                                                GameManager.Instance.GetPersistentPlayerList().SpawnPointRemoved(bc.blockValueRef.BlockPosition);

                                                try
                                                {
                                                    GameManager.Instance.SetBlocksRPC(changes);
                                                }
                                                catch { GameManager.Instance.SetBlocksRPC(changes); }
                                                finally { }

                                                Log.Out("[PrismaCore] AllPoiProtection: Bedroll at (" + bc.blockValueRef.BlockPosition.ToString() + ") removed.");

                                                break;
                                            }
                                        }

                                    }
                                }//end of prefabloop
                            }
                        }
                        else if (ServerCoreSettings.Instance.QuestPoiProtection_Enabled)
                        {
                            //check for questpois
                            DynamicPrefabDecorator dynamicPrefabDecorator = GameManager.Instance.GetDynamicPrefabDecorator();
                            LoadQuestPoiExceptons();

                            if (dynamicPrefabDecorator != null)
                            {
                                List<PrefabInstance> allPrefabs = new List<PrefabInstance>();
                                dynamicPrefabDecorator.GetWorldPrefabs(allPrefabs);

                                for (int i = 0; i < allPrefabs.Count; i++)
                                {
                                    PrefabInstance fab = allPrefabs[i];
                                    if (fab != null)
                                    {
                                        if (fab.prefab.HasQuestTag() && fab.prefab.DifficultyTier > 0)
                                        {
                                            if (lstQuestPoiExceptions.ContainsCaseInsensitive(fab.name) || lstQuestPoiExceptions.ContainsCaseInsensitive(fab.location.Name))
                                            {
                                                continue;
                                            }

                                            int deadsize = GamePrefs.GetInt(EnumUtils.Parse<EnumGamePrefs>("BedrollDeadZoneSize"));
                                            Vector3i BoxMin = fab.boundingBoxPosition;
                                            Vector3i BoxMax = fab.boundingBoxPosition + fab.boundingBoxSize;

                                            Vector3i expand = new Vector3i(deadsize, deadsize, deadsize);
                                            Vector3i BoxMinNew = BoxMin - expand;
                                            Vector3i BoxMaxNew = BoxMax + expand;

                                            if (bc.blockValueRef.BlockPosition.x >= BoxMinNew.x && bc.blockValueRef.BlockPosition.x < BoxMaxNew.x && bc.blockValueRef.BlockPosition.y >= BoxMinNew.y && bc.blockValueRef.BlockPosition.y < BoxMaxNew.y && bc.blockValueRef.BlockPosition.z >= BoxMinNew.z && bc.blockValueRef.BlockPosition.z < BoxMaxNew.z)
                                            {
                                                if (bc.blockValueRef.BlockPosition.x != 0 && bc.blockValueRef.BlockPosition.z != 0)
                                                {
                                                    if (clientinfo != null)
                                                    {
                                                        if (BuffManager.Buffs.ContainsKey("prismacore_tooltip_questpoiprotectionbed"))
                                                        {
                                                            EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                                            showTooltip(ep, "prismacore_tooltip_questpoiprotectionbed");
                                                        }
                                                        else
                                                        {
                                                            clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, ServerCoreStrings.Instance.QuestPoiProtection_BedMessage), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                                        }

                                                        GiveItem(clientinfo, "bedroll", 1);
                                                        Log.Out($"[PrismaCore] QuestpoiProtection: Bedroll given back to player {clientinfo.playerName}");
                                                    }

                                                    List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                                    BlockChangeInfo bciAnti = new BlockChangeInfo(bc.blockValueRef, new BlockValue(0), true, false);

                                                    changes.Add(bciAnti);
                                                    GameManager.Instance.GetPersistentPlayerList().SpawnPointRemoved(bc.blockValueRef.BlockPosition);

                                                    try
                                                    {
                                                        GameManager.Instance.SetBlocksRPC(changes);
                                                    }
                                                    catch { GameManager.Instance.SetBlocksRPC(changes); }
                                                    finally { }

                                                    Log.Out("[PrismaCore] QuestpoiProtection: Bedroll at (" + bc.blockValueRef.BlockPosition.ToString() + ") removed.");

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }//end of prefabloop
                            }
                        }
                    }
                    else
                    {
                        //all other blocks -> Antiblock adv. claim

                        //check if in Antiblock claim and if block not allowed
                        if (lstClaims != null && lstClaims.Count != 0)
                        {
                            foreach (DbClaim activeClaim in lstClaims)
                            {
                                if (activeClaim == null) continue;

                                if (activeClaim.Type.Trim().ContainsCaseInsensitive("antiblock:"))
                                {
                                    if (bc.blockValueRef.BlockPosition.x >= activeClaim.W_bound && bc.blockValueRef.BlockPosition.x <= activeClaim.E_bound && bc.blockValueRef.BlockPosition.z >= activeClaim.S_bound && bc.blockValueRef.BlockPosition.z <= activeClaim.N_bound)
                                    {
                                        if (AdminLvL <= activeClaim.AccessLevel || activeClaim.Whitelist.Contains(clientinfo.PlatformId.ToString()) || activeClaim.Id.Contains(clientinfo.PlatformId.ToString()))
                                        {
                                            continue;
                                        }
                                        else
                                        {
                                            if (bc.blockValueRef.BlockPosition.x != 0 && bc.blockValueRef.BlockPosition.z != 0)
                                            {
                                                if (bc.blockValue.Block != null && activeClaim.Type.Trim().Contains(bc.blockValue.Block.GetBlockName()))
                                                {
                                                    if (clientinfo != null)
                                                    {
                                                        if (BuffManager.Buffs.ContainsKey($"prismacore_tooltip_{activeClaim.Id}"))
                                                        {
                                                            EntityPlayer ep = GameManager.Instance.World.Players.dict[clientinfo.entityId];
                                                            showTooltip(ep, $"prismacore_tooltip_{activeClaim.Id}");
                                                        }
                                                        else
                                                        {
                                                            string msg = ServerCoreStrings.Instance.AdvClaims_AntiBlock.Replace("{blockName}", bc.blockValue.Block.GetBlockName());
                                                            clientinfo.SendPackage(NetPackageManager.GetPackage<NetPackageChat>().Setup(EChatType.Whisper, -1, Utils.CreateGameMessage(ServerCoreStrings.Instance.ServerChatName, msg), null, EMessageSender.None, GeneratedTextManager.BbCodeSupportMode.Supported));
                                                        }

                                                        GiveItem(clientinfo, bc.blockValue.Block.GetBlockName(), 1);
                                                        Log.Out($"[PrismaCore] Adv. Claim AntiBlock: Illegal block ({bc.blockValue.Block.GetBlockName()}) given back to player {clientinfo.playerName}");
                                                    }

                                                    List<BlockChangeInfo> changes = new List<BlockChangeInfo>();
                                                    BlockChangeInfo bciAnti = new BlockChangeInfo(bc.blockValueRef, new BlockValue(0), true, false);

                                                    changes.Add(bciAnti);

                                                    try
                                                    {
                                                        GameManager.Instance.SetBlocksRPC(changes);
                                                    }
                                                    catch { GameManager.Instance.SetBlocksRPC(changes); }
                                                    finally { }

                                                    Log.Out("[PrismaCore] Adv. Claim AntiBlock: Block at (" + bc.blockValueRef.BlockPosition.ToString() + ") removed.");

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static void GiveItem(ClientInfo ci, string item, int amount)
        {
            if (ci == null)
            {
                return;
            }

            World ww = GameManager.Instance.World;

            ItemValue iv = ItemClass.GetItem(item, true);
            if (iv.type == ItemValue.None.type)
            {
                return;
            }
            iv = new ItemValue(iv.type, true);

            if (iv == null)
            {
                return;
            }

            var entityItem = (EntityItem)EntityFactory.CreateEntity(new EntityCreationData
            {
                entityClass = EntityClass.FromString("item"),
                id = EntityFactory.nextEntityID++,
                itemStack = new ItemStack(iv, amount),
                pos = ww.Players.dict[ci.entityId].position,
                rot = new Vector3(20f, 0f, 20f),
                lifetime = 60f,
                belongsPlayerId = ci.entityId
            });
            ww.SpawnEntityInWorld(entityItem);
            ci.SendPackage(NetPackageManager.GetPackage<NetPackageEntityCollect>().Setup(entityItem.entityId, ci.entityId));
            ww.RemoveEntity(entityItem.entityId, EnumRemoveEntityReason.Killed);
        }

        private static void showTooltip(EntityPlayer ep, string tooltip)
        {
            if (BuffManager.Buffs.ContainsKey(tooltip))
            {
                ep.Buffs.AddBuff(tooltip, -1, true, false);
            }
            else
            {
                Log.Out($"[PrismaCore] ERR: showTooltip: Tooltip {tooltip} can not be found.");
            }
        }

        public static void RR()
        {
            World world = GameManager.Instance.World;
            ChunkCluster chunkCache = world.ChunkCache;
            ChunkProviderGenerateWorld chunkProviderGenerateWorld = chunkCache.ChunkProvider as ChunkProviderGenerateWorld;
            HashSetLong hashSetLong = new HashSetLong();
            HashSetLong hashSetLong2 = new HashSetLong();
            HashSetLong hashSetLong3;

            if (chunkProviderGenerateWorld == null)
            {
                SdtdConsole.Instance.Output("[PrismaCore]Region reset failed: ChunkProviderGenerateWorld could not be found for current world instance.");
                return;
            }
            chunkProviderGenerateWorld.MainThreadCacheProtectedPositions();
            ChunkProtectionLevel chunkProtectionLevel = ChunkProtectionLevel.None;

            int regionX;
            int regionZ;
            string region;
            int totalRegions = 0;
            int totalSyncedChunks = 0;

            foreach (string s in RegionReset.lstRegions)
            {
                //r.-3.0.7rg
                hashSetLong.Clear();
                hashSetLong2.Clear();

                region = s.Replace("r.", string.Empty);
                region = region.Replace(".7rg", string.Empty);
                int.TryParse(region.Split('.')[0], out regionX);
                int.TryParse(region.Split('.')[1], out regionZ);
                hashSetLong3 = chunkProviderGenerateWorld.ResetRegion(regionX, regionZ, chunkProtectionLevel);

                if ((chunkProtectionLevel & ChunkProtectionLevel.CurrentlySynced) == ChunkProtectionLevel.None)
                {
                    foreach (long num2 in hashSetLong3)
                    {
                        if (chunkCache.ContainsChunkSync(num2))
                        {
                            hashSetLong.Add(num2);
                        }
                    }
                    if (hashSetLong.Count > 0)
                    {
                        foreach (long num3 in hashSetLong)
                        {
                            if (!chunkProviderGenerateWorld.GenerateSingleChunk(chunkCache, num3, true))
                            {
                                SdtdConsole.Instance.Output(string.Format("[PrismaCore]Region reset failed regenerating chunk at world XZ position: {0}, {1}", WorldChunkCache.extractX(num3) << 4, WorldChunkCache.extractZ(num3) << 4));
                            }
                            else
                            {
                                hashSetLong2.Add(num3);
                            }
                        }
                        world.m_ChunkManager.ResendChunksToClients(hashSetLong2);

                    }
                }
                totalRegions += 1;
                totalSyncedChunks += hashSetLong3.Count;
            }

            SdtdConsole.Instance.Output($"[PrismaCore]Region reset complete. Reset {totalSyncedChunks} chunks in {totalRegions} regions.");
        }

        public static void RRunclaimed()
        {
            World world = GameManager.Instance.World;
            ChunkCluster chunkCache = world.ChunkCache;
            ChunkProviderGenerateWorld chunkProviderGenerateWorld = chunkCache.ChunkProvider as ChunkProviderGenerateWorld;
            HashSetLong hashSetLong = new HashSetLong();
            HashSetLong hashSetLong2 = new HashSetLong();
            HashSetLong hashSetLong3;

            if (chunkProviderGenerateWorld == null)
            {
                SdtdConsole.Instance.Output("[PrismaCore]Region reset failed: ChunkProviderGenerateWorld could not be found for current world instance.");
                return;
            }
            chunkProviderGenerateWorld.MainThreadCacheProtectedPositions();
            ChunkProtectionLevel chunkProtectionLevel = ChunkProtectionLevel.None;

            int regionX;
            int regionZ;
            string region;
            int totalRegions = 0;
            int totalSyncedChunks = 0;

            string regionFolder = GameIO.GetSaveGameDir() + "/Region";

            //loop through all region files present on fs and delete the ones not in lstRegionsClaimed
            SdDirectoryInfo d = new SdDirectoryInfo(regionFolder);

            foreach (var file in d.GetFiles("*.7rg"))
            {
                try
                {
                    if (!RegionReset.lstRegionsClaimed.Contains(file.Name))
                    {
                        //r.-3.0.7rg
                        hashSetLong.Clear();
                        hashSetLong2.Clear();

                        region = file.Name.Replace("r.", string.Empty);
                        region = region.Replace(".7rg", string.Empty);
                        int.TryParse(region.Split('.')[0], out regionX);
                        int.TryParse(region.Split('.')[1], out regionZ);
                        hashSetLong3 = chunkProviderGenerateWorld.ResetRegion(regionX, regionZ, chunkProtectionLevel);

                        if ((chunkProtectionLevel & ChunkProtectionLevel.CurrentlySynced) == ChunkProtectionLevel.None)
                        {
                            foreach (long num2 in hashSetLong3)
                            {
                                if (chunkCache.ContainsChunkSync(num2))
                                {
                                    hashSetLong.Add(num2);
                                }
                            }
                            if (hashSetLong.Count > 0)
                            {
                                foreach (long num3 in hashSetLong)
                                {
                                    if (!chunkProviderGenerateWorld.GenerateSingleChunk(chunkCache, num3, true))
                                    {
                                        SdtdConsole.Instance.Output(string.Format("[PrismaCore]Region reset failed regenerating chunk at world XZ position: {0}, {1}", WorldChunkCache.extractX(num3) << 4, WorldChunkCache.extractZ(num3) << 4));
                                    }
                                    else
                                    {
                                        hashSetLong2.Add(num3);
                                    }
                                }
                                world.m_ChunkManager.ResendChunksToClients(hashSetLong2);

                            }
                        }
                        totalRegions += 1;
                        totalSyncedChunks += hashSetLong3.Count;
                    }
                }
                catch { Log.Out($"[PrismaCore] Error in handling unclaimed region file {file.Name}"); }

            }

            SdtdConsole.Instance.Output($"[PrismaCore]Unclaimed region reset complete. Reset {totalSyncedChunks} chunks in {totalRegions} unclaimed regions.");
        }
    }
}
