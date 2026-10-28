using ServerCore.CustomCommands;
using Epic.OnlineServices.Presence;
using HarmonyLib;
using LiteNetLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ServerCore
{
    internal static class Patcher
    {
        public static void DoPatching()
        {
            try
            {
                Harmony harmonyPatcher = new Harmony("io.takaro.servercore");

                var gameMethod = AccessTools.Method(typeof(TEFeatureLandClaim), "OnAdded");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod OnAdded not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("LCB");
                    
                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod CBB or CBA not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(GameManager), "ChangeBlocks");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod ChangeBlocks not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("CBB");
                    var pof = typeof(StateManager).GetMethod("CBA");

                    if (prf == null || pof == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod CBB or CBA not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), new HarmonyMethod(pof), null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(NetPackagePlayerStats), "ProcessPackage");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod ProcessPackage not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("LLB");
                    var pof = typeof(StateManager).GetMethod("LLA");

                    if (prf == null || pof == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod LLB or LLA not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), new HarmonyMethod(pof), null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(World), "AddFallingBlocks");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod AddFallingBlocks not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("FBSP");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod FBSP not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(World), "AddFallingBlock");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod AddFallingBlock not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("FBP");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod FBP not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(SdtdConsole), "executeCommand");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod executeCommand not found. Abort patching.");
                }
                else
                {
                    var pof = typeof(StateManager).GetMethod("LCP");

                    if (pof == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod LCP not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, null, new HarmonyMethod(pof), null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer), "ConnectionRequestCheck");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod ConnectionRequestCheck not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("CRC");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod CRC not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(EntityAlive), "ClientKill");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod ClientKill not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("CKP");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod CKP not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(EntityAlive), "OnEntityDeath");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod OnEntityDeath not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("OED");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod OED not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(SleeperVolume), "UpdatePlayerTouched");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod UpdatePlayerTouched not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("SRP");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod SRP not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(SleeperVolume), "UpdateSpawn");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod UpdateSpawn not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("SHP");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod SHP not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(VehicleManager), "Save");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod SaveVehicle not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("AV");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod AV not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(VehicleManager), "Load");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod LoadVehicle not found. Abort patching.");
                }
                else
                {
                    var pof = typeof(StateManager).GetMethod("LV");

                    if (pof == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod LV not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, null, new HarmonyMethod(pof), null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(VehicleManager), "AddTrackedVehicle");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod AddTrackedVehicle not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("ATV");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod ATV not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(DroneManager), "Save");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod SaveDrone not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("AD");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod AD not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(DroneManager), "Load");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod LoadDrone not found. Abort patching.");
                }
                else
                {
                    var pof = typeof(StateManager).GetMethod("LD");

                    if (pof == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod LD not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, null, new HarmonyMethod(pof), null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(DroneManager), "AddTrackedDrone");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod AddTrackedDrone not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("ATD");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod ATD not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(AIDirectorBloodMoonParty), "Tick");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod Tick not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("AIBT");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod AIBT not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(AIDirectorBloodMoonComponent), "StartBloodMoon");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod StartBloodMoon not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("DAOBS");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod DAOBS not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(ConsoleCmdTeleportPlayer), "Execute");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod Execute not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("TPB");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod TPB not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(TileEntity), "setModified");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod setModified not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("TESMB");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod TESMB not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }

                gameMethod = AccessTools.Method(typeof(BlockCompositeTileEntity), "IsTileEntitySavedInPrefab");
                if (gameMethod == null)
                {
                    Log.Out("[PrismaCore] GameMethod IsTileEntitySavedInPrefab not found. Abort patching.");
                }
                else
                {
                    var prf = typeof(StateManager).GetMethod("TESIPB");

                    if (prf == null)
                    {
                        Log.Out("[PrismaCore] PatchMethod TESIPB not found. Abort patching.");
                    }
                    else
                    {
                        harmonyPatcher.Patch(gameMethod, new HarmonyMethod(prf), null, null);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[PrismaCore] Error in applying Harmony patches: {0}", e.ToString()));
            }
        }
    }

    public static class StateManager
    {
        public class StateChangeBlocks
        {
            public List<BlockChangeInfo> bci;
            public PlatformUserIdentifierAbs playerId;
        }

        public class StatePlayerLevel
        {
            public int EntityId;
            public int Level;
        }

        public static bool CBB(GameManager __instance, PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange, ref StateChangeBlocks __state)
        {
            try
            {
                bool skip;
                
                if (persistentPlayerId == null)
                {
                    return true;
                }

                if (_blocksToChange == null)
                {
                    return true;
                }

                __state = new StateChangeBlocks()
                {
                    playerId = persistentPlayerId,
                    bci = new List<BlockChangeInfo>()
                };

                foreach (BlockChangeInfo bc in _blocksToChange)
                {
                    //&& bc.bChangeBlockValue
                    skip = false;

                    if (bc != null)
                    {
                        if (bc.blockValue.type == BlockValue.Air.type)
                        {
                            continue;
                        }

                        BlockValue bv = __instance.World.GetBlock(bc.blockValueRef);

                        if (bv.Block is BlockPoweredDoor || bv.Block is BlockPlacementDoor || bv.Block is BlockCampfire || bv.Block is BlockForge || bv.Block is BlockTrapDoor || bv.Block is BlockWorkstation || bv.Block is BlockSign || bv.Block is BlockQuestActivate || bv.Block.shape is BlockShapeTerrain)
                        {
                            List<DbClaim> lstClaims = Database.Instance.GetAllDbClaims();

                            foreach (DbClaim activeClaim in lstClaims)
                            {
                                if (activeClaim.Type.Trim().ContainsCaseInsensitive("problock:"))
                                {
                                    if (bc.blockValueRef.BlockPosition.x >= activeClaim.W_bound && bc.blockValueRef.BlockPosition.x <= activeClaim.E_bound && bc.blockValueRef.BlockPosition.z >= activeClaim.S_bound && bc.blockValueRef.BlockPosition.z <= activeClaim.N_bound)
                                    {
                                        skip = true;
                                        break;
                                    }
                                }
                                else if (activeClaim.Type.Trim().EqualsCaseInsensitive("landclaim"))
                                {
                                    if (bc.blockValueRef.BlockPosition.x >= activeClaim.W_bound && bc.blockValueRef.BlockPosition.x <= activeClaim.E_bound && bc.blockValueRef.BlockPosition.z >= activeClaim.S_bound && bc.blockValueRef.BlockPosition.z <= activeClaim.N_bound)
                                    {
                                        skip = true;
                                        break;                                        
                                    }
                                }
                            }
                        }

                        if (skip)
                        {
                            continue;
                        }

                        if (!bc.bChangeDamage)
                        {
                            __state.bci.Add(bc);
                        }
                    }
                }
            }
            catch { }

            return true;
        }

        public static bool CRC(NetworkServerLiteNetLib.LiteNetLibAuthWrapperServer __instance, ConnectionRequest _request)
        {
            try
            {
                if (__instance == null)
                    return true;

                if (Reset.resetActive)
                {
                    _request.Reject(NetworkServerLiteNetLib.rejectInvalidPassword);
                    return false;
                }
            }
            catch { }

            return true;
        }

        public static void CBA(GameManager __instance, PlatformUserIdentifierAbs persistentPlayerId, List<BlockChangeInfo> _blocksToChange, StateChangeBlocks __state)
        {
            try
            {
                if (__state == null)
                    return;

                RegionReset.CheckAntiBlock(__state.playerId, __state.bci);
            }
            catch { }
        }

        public static bool LLB(NetPackagePlayerStats __instance, ref StatePlayerLevel __state, World _world)
        {
            try
            {
                if (_world == null)
                    return true;

                if (__instance == null)
                    return true;

                EntityAlive entityAlive = _world.GetEntity(__instance.entityId) as EntityAlive;
                if (entityAlive)
                {
                    EntityPlayer entityPlayer = entityAlive as EntityPlayer;

                    if (entityPlayer != null)
                    {
                        __state = new StatePlayerLevel()
                        {
                            EntityId = entityPlayer.entityId,
                            Level = entityPlayer.Progression.Level,
                        };
                    }
                }
            }
            catch { }

            return true;
        }

        public static void LLA(NetPackagePlayerStats __instance, StatePlayerLevel __state, World _world)
        {
            try
            {
                if (_world == null || __state == null)
                    return;

                if (_world.GetEntity(__state.EntityId) is EntityPlayer player)
                {
                    if (player.Progression.Level > __state.Level)
                    {
                        ClientInfo clientInfo = ConnectionManager.Instance.Clients.ForEntityId(player.entityId);
                        if (clientInfo == null) return;

                        Log.Out($"[PrismaCore]playerLeveled: {clientInfo.playerName} ({clientInfo.PlatformId}) made level {player.Progression.Level} (was {__state.Level})");

                        //check if level jump is > 1. Anticheat.
                        if ((player.Progression.Level - __state.Level) >= ServerCoreSettings.Instance.LevelJumpDetection_MinimumLevelJumpTrigger)
                        {
                            Log.Out($"[PrismaCore] WARNING: {clientInfo.playerName} ({clientInfo.PlatformId}) jumped up more than one level ({__state.Level} -> {player.Progression.Level}).");

                            int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(clientInfo);

                            if (AdminLvL > ServerCoreSettings.Instance.LevelJumpDetection_ExcludeAdminLvl)
                            {
                                string command = ServerCoreSettings.Instance.LevelJumpDetection_DetectedCommand;
                                if (!string.IsNullOrEmpty(command) && !command.EqualsCaseInsensitive("none"))
                                {
                                    if (command.Contains(";"))
                                    {
                                        //multiple commands
                                        string[] arrCommands = command.Split(';');
                                        CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                        foreach (string s in arrCommands)
                                        {
                                            string cmd = s;
                                            cmd = cmd.Replace("${steamId}", clientInfo.PlatformId.ToString());
                                            cmd = cmd.Replace("${platformId}", clientInfo.PlatformId.ToString());
                                            cmd = cmd.Replace("${entityId}", clientInfo.entityId.ToString());
                                            cmd = cmd.Replace("${playerName}", clientInfo.playerName);

                                            SdtdConsole.Instance.ExecuteAsync(cmd, iConsole);
                                        }
                                    }
                                    else
                                    {
                                        //just 1 command
                                        command = command.Replace("${steamId}", clientInfo.PlatformId.ToString());
                                        command = command.Replace("${platformId}", clientInfo.PlatformId.ToString());
                                        command = command.Replace("${entityId}", clientInfo.entityId.ToString());
                                        command = command.Replace("${playerName}", clientInfo.playerName);

                                        CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                        SdtdConsole.Instance.ExecuteAsync(command, iConsole);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch { }

        }

        public static bool FBSP(World __instance, IList<Vector3i> _list)
        {
            try
            {
                if (__instance == null || _list == null)
                    return true;

                if (ServerCoreSettings.Instance.PreventFallingBlocks > 0)
                {
                    DamageHandler.HandleFallingBlocks(__instance, _list);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch { }

            return true;
        }

        public static bool FBP(World __instance, Vector3i _blockPos)
        {
            try
            {
                if (__instance == null)
                    return true;

                if (ServerCoreSettings.Instance.PreventFallingBlocks > 0)
                {
                    DamageHandler.HandleFallingBlock(__instance, _blockPos);
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch { }

            return true;
        }

        public static void LCP(List<string> __result, string _command, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (WriteLog.commandsForOutput.ContainsKey(_command))
                {
                    if (__result.Count > 0)
                    {
                        if (!WriteLog.commandsForOutput[_command])
                        {
                            string output = string.Empty;

                            foreach (string str in __result)
                            {
                                output += $"{str}\n";
                            }
                            Log.Out($"[PrismaCore] Output from command {_command}:\n{output}");
                            WriteLog.commandsForOutput.Remove(_command);
                        }
                        else
                        {
                            Log.Out($"[PrismaCore] Output from command {_command} (with splitlog parameter):");
                            foreach (string str in __result)
                            {
                                Log.Out($"[W2L]{str}");
                            }
                            WriteLog.commandsForOutput.Remove(_command);
                        }
                    }
                }
            }
            catch { }
        }

        public static bool CKP(EntityAlive __instance, DamageResponse _dmResponse)
        {

            if (__instance == null || _dmResponse.Source == null)
            {
                return true;
            }

            if (_dmResponse.Strength >= ServerCoreSettings.Instance.DamageDetection_MinAmountDamage)
            {
                if (__instance.IsAlive())
                {
                    var offenderEntity0 = GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive;
                    if (offenderEntity0 == null) return true;

                    if (offenderEntity0 is EntityPlayer)
                    {
                        var offenderClientInfo0 = ConnectionManager.Instance.Clients.ForEntityId(offenderEntity0.entityId);

                        if (offenderClientInfo0 == null) return true;

                        int AdminLvL = GameManager.Instance.adminTools.Users.GetUserPermissionLevel(offenderClientInfo0);

                        if (AdminLvL > ServerCoreSettings.Instance.DamageDetection_ExcludeAdminLvl)
                        {
                            DamageHandler.LogDamageDetection(offenderClientInfo0.playerName, offenderClientInfo0.PlatformId.ToString(), _dmResponse.Strength);

                            string command = ServerCoreSettings.Instance.DamageDetection_DetectedCommand;
                            if (!string.IsNullOrEmpty(command) && !command.EqualsCaseInsensitive("none"))
                            {
                                if (command.Contains(";"))
                                {
                                    //multiple commands
                                    string[] arrCommands = command.Split(';');
                                    CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                    foreach (string s in arrCommands)
                                    {
                                        string cmd = s;
                                        cmd = cmd.Replace("${steamId}", offenderClientInfo0.PlatformId.ToString());
                                        cmd = cmd.Replace("${platformId}", offenderClientInfo0.PlatformId.ToString());
                                        cmd = cmd.Replace("${entityId}", offenderClientInfo0.entityId.ToString());
                                        cmd = cmd.Replace("${playerName}", offenderClientInfo0.playerName);

                                        SdtdConsole.Instance.ExecuteAsync(cmd, iConsole);
                                    }
                                }
                                else
                                {
                                    //just 1 command
                                    command = command.Replace("${steamId}", offenderClientInfo0.PlatformId.ToString());
                                    command = command.Replace("${platformId}", offenderClientInfo0.PlatformId.ToString());
                                    command = command.Replace("${entityId}", offenderClientInfo0.entityId.ToString());
                                    command = command.Replace("${playerName}", offenderClientInfo0.playerName);

                                    CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                    SdtdConsole.Instance.ExecuteAsync(command, iConsole);
                                }
                            }
                        }

                        return true;
                    }
                }
            }

            if (__instance is EntityPlayer)
            //player died -> take action
            {
                if (__instance.IsAlive())
                {
                    var offenderEntity = GameManager.Instance.World.GetEntity(_dmResponse.Source.getEntityId()) as EntityAlive;
                    if (offenderEntity == null) return true;

                    if (offenderEntity is EntityPlayer)
                    {
                        var victimClientInfo = ConnectionManager.Instance.Clients.ForEntityId(__instance.entityId);
                        var offenderClientInfo = ConnectionManager.Instance.Clients.ForEntityId(offenderEntity.entityId);

                        if (victimClientInfo == null || offenderClientInfo == null)
                            return true;

                        if (victimClientInfo.PlatformId.ToString() == offenderClientInfo.PlatformId.ToString())
                        {
                            return true;
                        }

                        var damprops = new DamageProperties
                        {
                            VictimPosition = __instance.GetBlockPosition(),
                            OffenderPosition = offenderEntity.GetBlockPosition(),
                            VictimEntityId = __instance.entityId,
                            VictimName = __instance.EntityName,
                            VictimSteamId = victimClientInfo.PlatformId.ToString(),
                            OffenderEntityId = offenderEntity.entityId,
                            OffenderName = offenderEntity.EntityName,
                            OffenderSteamId = offenderClientInfo.PlatformId.ToString()
                        };

                        DamageHandler.HandleDamagePlayer(damprops, true);
                    }
                    else
                    {
                        var victimClientInfo = ConnectionManager.Instance.Clients.ForEntityId(__instance.entityId);

                        if (victimClientInfo == null)
                            return true;

                        var damprops = new DamageProperties
                        {
                            VictimPosition = __instance.GetBlockPosition(),
                            OffenderPosition = offenderEntity.GetBlockPosition(),
                            VictimEntityId = __instance.entityId,
                            VictimName = __instance.EntityName,
                            VictimSteamId = victimClientInfo.PlatformId.ToString(),
                            OffenderEntityId = offenderEntity.entityId,
                            OffenderName = offenderEntity.EntityName,
                            OffenderSteamId = null
                        };

                        DamageHandler.HandleDamageOther(damprops, true);
                    }
                }
            }
            return true;
        }

        public static bool OED(EntityAlive __instance)
        {
            if (__instance == null)
            {
                return true;
            }

            if (__instance.GetType() == typeof(EntityPlayer))
            {
                ClientInfo ci = ConnectionManager.Instance.Clients.ForEntityId(__instance.entityId);
                if (ci != null)
                {
                    Log.Out($"[PrismaCore]playerDied: {ci.playerName} ({ci.PlatformId}) died @ {(int)Math.Floor(__instance.position.x)} {(int)Math.Floor(__instance.position.y)} {(int)Math.Floor(__instance.position.z)}");

                    Vector3i pos = new Vector3i();

                    pos.x = (int)Math.Floor(__instance.position.x);
                    pos.y = (int)Math.Floor(__instance.position.y);
                    pos.z = (int)Math.Floor(__instance.position.z);

                    if (API.dicDied.ContainsKey(ci.PlatformId.ToString()))
                    {
                        API.dicDied.Remove(ci.PlatformId.ToString());
                        API.dicDied.Add(ci.PlatformId.ToString(), pos);
                    }
                    else
                        API.dicDied.Add(ci.PlatformId.ToString(), pos);
                }
            }

            return true;
        }

        public static bool SRP(SleeperVolume __instance, ref ulong ___respawnTime, World _world)
        {
            try
            {
                if (__instance == null || _world == null)
                {
                    return true;
                }

                bool respawnDisabled = ServerCoreSettings.Instance.DisableSleeperRespawn_Enabled;

                if (respawnDisabled)
                {
                    ___respawnTime = ulong.MaxValue;
                }
            }
            catch { }

            return true;
        }

        public static bool SHP(SleeperVolume __instance, World _world)
        {
            try
            {
                if (__instance == null || _world == null)
                {
                    return true;
                }

                if (ConnectionManager.Instance.ClientCount() == 0)
                {
                    return true;
                }

                if (!GameStats.GetBool(EnumGameStats.IsSpawnEnemies))
                {
                    return true;
                }

                int count = GameStats.GetInt(EnumGameStats.EnemyCount);
                if (count >= GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies))
                {
                    return true;
                }

                if (ServerCoreSettings.Instance.DisableSleepers_Enabled)
                {
                    return false;
                }

                if (ServerCoreSettings.Instance.DisableSleepers_BloodmoonOnly_Enabled)
                {
                    if (_world.aiDirector.BloodMoonComponent.BloodMoonActive)
                    {
                        return false;
                    }
                }

                if (API.hostilefreeClaims != null && API.hostilefreeClaims.Count != 0)
                {
                    foreach (hostilefreeClaim hfClaim in API.hostilefreeClaims)
                    {
                        if (hfClaim == null) continue;

                        Vector3 hostilePos = __instance.Center;
                        if (hostilePos.x > hfClaim.W_bound && hostilePos.x < hfClaim.E_bound && hostilePos.z > hfClaim.S_bound && hostilePos.z < hfClaim.N_bound)
                        {
                            return false;
                        }
                    }
                }

                foreach (string steamId in ClaimProtector.adminBubble)
                {
                    foreach (KeyValuePair<int, EntityPlayer> player in _world.Players.dict)
                    {
                        ClientInfo ci = ConnectionManager.Instance.Clients.ForEntityId(player.Key);
                        if (steamId.Equals(ci.PlatformId.ToString()))
                        {
                            Vector3 pos2 = player.Value.position;
                            Vector3 hostilePos = __instance.Center;

                            if (Math.Abs(pos2.x - hostilePos.x) < 25 && Math.Abs(pos2.y - hostilePos.y) < 25 && Math.Abs(pos2.z - hostilePos.z) < 25)
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch { }

            return true;
        }


        public static bool AV(VehicleManager __instance, ref List<EntityVehicle> ___vehiclesActive, ref List<EntityCreationData> ___vehiclesUnloaded)
        {
            try
            {
                if (__instance == null)
                {
                    return true;
                }

                RegionReset.vehicles = ___vehiclesActive;
                RegionReset.vehicleStubs = ___vehiclesUnloaded;
            }
            catch { }

            return true;
        }
        public static void LV(VehicleManager __instance, ref List<EntityCreationData> ___vehiclesUnloaded)
        {
            try
            {
                if (__instance == null)
                {
                    return;
                }

                RegionReset.vehicleStubs = ___vehiclesUnloaded;
            }
            catch { }
        }

        public static bool ATV(VehicleManager __instance, EntityVehicle _vehicle)
        {
            try
            {
                if (__instance == null || _vehicle == null)
                {
                    return true;
                }

                int id = _vehicle.entityId;
                PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(_vehicle.GetOwner());
                string owner = "unknown";

                if (playerDataFromEntityID != null)
                {
                    owner = playerDataFromEntityID.PlayerName.DisplayName;
                }

                Database.Instance.SetVehicleOwner(id, owner);

            }
            catch { }

            return true;
        }

        public static bool LCB(TEFeatureLandClaim __instance, Vector3i _blockPos, BlockValue _blockValue)
        {
            try
            {
                if (__instance == null)
                {
                    return true;
                }

                if(RegionReset.HandleLCB(_blockPos, _blockValue, __instance.Parent.Owner))
                { 
                    return false; 
                }
                else
                {
                    return true;
                }
            }
            catch { }

            return true;
        }

        public static bool AD(DroneManager __instance, ref List<EntityDrone> ___dronesActive, ref List<EntityCreationData> ___dronesUnloaded)
        {
            try
            {
                if (__instance == null)
                {
                    return true;
                }

                RegionReset.drones = ___dronesActive;
                RegionReset.droneStubs = ___dronesUnloaded;
            }
            catch { }

            return true;
        }
        public static void LD(DroneManager __instance, ref List<EntityCreationData> ___dronesUnloaded)
        {
            try
            {
                if (__instance == null)
                {
                    return;
                }

                RegionReset.droneStubs = ___dronesUnloaded;
            }
            catch { }
        }

        public static bool ATD(DroneManager __instance, EntityDrone _drone)
        {
            try
            {
                if (__instance == null || _drone == null)
                {
                    return true;
                }

                int id = _drone.entityId;
                PersistentPlayerData playerDataFromEntityID = GameManager.Instance.GetPersistentPlayerList().GetPlayerData(_drone.GetOwner());
                string owner = "unknown";

                if (playerDataFromEntityID != null)
                {
                    owner = playerDataFromEntityID.PlayerName.DisplayName;
                }

                Database.Instance.SetDroneOwner(id, owner);

            }
            catch { }

            return true;
        }

        public static bool DAOBS(AIDirectorBloodMoonComponent __instance)
        {
            if (ServerCoreSettings.Instance.BloodmoonSpawner_DespawnAllOnStart)
            {
                try
                {
                    if (__instance == null)
                    {
                        return true;
                    }

                    List<Entity> hostiles = new List<Entity>();

                    List<Entity> list = new List<Entity>(GameManager.Instance.World.Entities.list);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Entity entity = list[i];
                        if (entity != null && !(entity is EntityPlayer) && entity is EntityAlive && !(entity is EntityVehicle) && !(entity is EntityTurret) && EntityClass.list[entity.entityClass].bIsEnemyEntity)
                        {
                            hostiles.Add(entity);
                        }
                    }

                    if (hostiles.Count > 0)
                    {
                        foreach (EntityAlive ea in hostiles.Cast<EntityAlive>())
                        {
                            if (ea.GetType().ToString() == "EntityTrader")
                            {
                                continue;
                            }
                            try
                            {
                                GameManager.Instance.World.RemoveEntity(ea.entityId, EnumRemoveEntityReason.Killed);
                            }
                            catch { continue; }
                        }

                        Log.Out($"[PrismaCore]BloodmoonSpawner: Despawned {hostiles.Count} hostiles before Bloodmoon start.");
                    }
                }
                catch { }
            }

            return true;
        }

        public static bool AIBT(AIDirectorBloodMoonParty __instance, ref bool __result, World _world, double _dt, bool _canSpawn, ref AIDirectorGameStagePartySpawner ___partySpawner, ref List<ManagedZombie> ___zombies, ref Vector3 ___spawnDirectionV, ref int ___groupIndex, ref int ___spawnBaseDir, ref Vector3 ___spawnBasePos, ref int ___nextPlayer)
        {
            try
            {
                if (ServerCoreSettings.Instance.BloodmoonSpawner_OverrideVanillaSpawner)
                {
                    if (__instance == null)
                    {
                        return true;
                    }

                    var miInitParty = AccessTools.Method(typeof(AIDirectorBloodMoonParty), "InitParty");
                    var miSeekTarget = AccessTools.Method(typeof(AIDirectorBloodMoonParty), "SeekTarget");
                    var miCalcBestDir = AccessTools.Method(typeof(AIDirectorBloodMoonParty), "CalcBestDir");
                    var miIsPlayerATarget = AccessTools.Method(typeof(AIDirectorBloodMoonParty), "IsPlayerATarget");
                    var miSpawnZombie = AccessTools.Method(typeof(AIDirectorBloodMoonParty), "SpawnZombie");

                    if (___partySpawner.partyLevel < 0)
                    {

                        miInitParty.Invoke(__instance, null);
                    }
                    for (int i = ___zombies.Count - 1; i >= 0; i--)
                    {
                        ManagedZombie managedZombie = ___zombies[i];
                        managedZombie.updateDelay -= (float)_dt;
                        if (managedZombie.updateDelay <= 0f)
                        {
                            managedZombie.updateDelay = 1.8f;

                            var parameters = new object[] { managedZombie };

                            if (!(bool)miSeekTarget.Invoke(__instance, parameters))
                            {
                                ___zombies.RemoveAt(i);
                            }
                        }
                    }
                    ___partySpawner.Tick(_dt);
                    bool result = false;
                    if (_canSpawn)
                    {
                        if (!___partySpawner.canSpawn || ___partySpawner.partyMembers.Count == 0)
                        {
                            __result = true;
                            return false;
                        }

                        if(AIDirector.CanSpawn(1.9f))
                        {
                            if (GameStats.GetInt(EnumGameStats.EnemyCount) < GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies) + ServerCoreSettings.Instance.BloodmoonSpawner_Overridden_AddMaxAliveServerDuringBloodmoon)
                            {
                                int num = ___partySpawner.groupIndex;
                                if (num != ___groupIndex)
                                {
                                    ___groupIndex = num;
                                    ___spawnBaseDir += 120;
                                    var parameters = new object[] { ___spawnBasePos };
                                    miCalcBestDir.Invoke(__instance, parameters);
                                }
                                result = true;
                                int count = ___partySpawner.partyMembers.Count;

                                if (ServerCoreSettings.Instance.BloodmoonSpawner_Overridden_AdjustBMEnemyCountPerPlayerToNrOnlinePlayers)
                                {
                                    double playerCount = _world.Players.dict.Count;
                                    int MaxSpawnedZombies = GamePrefs.GetInt(EnumGamePrefs.MaxSpawnedZombies) + ServerCoreSettings.Instance.BloodmoonSpawner_Overridden_AddMaxAliveServerDuringBloodmoon;
                                    int spawns = (int)Math.Round(MaxSpawnedZombies / playerCount);

                                    if (spawns > GameStats.GetInt(EnumGameStats.BloodMoonEnemyCount))
                                    {
                                        spawns = GameStats.GetInt(EnumGameStats.BloodMoonEnemyCount);
                                    }

                                    if (___partySpawner.maxAlive <= 0 || ___zombies.Count < spawns * count)
                                    {
                                        for (int j = count; j > 0; j--)
                                        {
                                            if (___nextPlayer >= count)
                                            {
                                                ___nextPlayer = 0;
                                            }
                                            EntityPlayer entityPlayer = ___partySpawner.partyMembers[___nextPlayer];
                                            bool flag = false;

                                            var parameters = new object[] { entityPlayer };

                                            if ((bool)miIsPlayerATarget.Invoke(__instance, parameters))
                                            {
                                                var parameters2 = new object[] { _world, entityPlayer, entityPlayer.position, ___spawnDirectionV };
                                                flag = (bool)miSpawnZombie.Invoke(__instance, parameters2);
                                                ___nextPlayer++;
                                            }
                                            else
                                            {
                                                ___nextPlayer++;
                                            }

                                            if (___zombies.Count >= spawns * count)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (___partySpawner.maxAlive <= 0 || ___zombies.Count < ServerCoreSettings.Instance.BloodmoonSpawner_Overridden_BMEnemyCountPerPlayer * count)
                                    {
                                        for (int j = count; j > 0; j--)
                                        {
                                            if (___nextPlayer >= count)
                                            {
                                                ___nextPlayer = 0;
                                            }
                                            EntityPlayer entityPlayer = ___partySpawner.partyMembers[___nextPlayer];
                                            bool flag = false;

                                            var parameters = new object[] { entityPlayer };

                                            if ((bool)miIsPlayerATarget.Invoke(__instance, parameters))
                                            {
                                                var parameters2 = new object[] { _world, entityPlayer, entityPlayer.position, ___spawnDirectionV };
                                                flag = (bool)miSpawnZombie.Invoke(__instance, parameters2);
                                                ___nextPlayer++;
                                            }
                                            else
                                            {
                                                ___nextPlayer++;
                                            }

                                            if (___zombies.Count >= ServerCoreSettings.Instance.BloodmoonSpawner_Overridden_BMEnemyCountPerPlayer * count)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    __result = result;
                    return false;
                }
            }
            catch { }

            return true;
        }

        // Prefab.copyFromWorld clones tile entities without a chunk; vanilla setModified dereferences it and throws
        public static bool TESMB(TileEntity __instance)
        {
            return __instance.GetChunk() != null;
        }

        public static bool TESIPB(BlockCompositeTileEntity __instance, ref bool __result)
        {
            if (!PrefabStorage.IncludeStorage || !PrefabStorage.IsStorage(__instance))
            {
                return true;
            }
            __result = true;
            return false;
        }

        public static bool TPB(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 2 && _params.Count != 4 && _params.Count != 5)
                {
                    SdtdConsole.Instance.Output("Wrong number of arguments, expected 2, 4 or 5, found " + _params.Count + ".");
                    return false;
                }
                ClientInfo clientInfo = ConsoleHelper.ParseParamIdOrName(_params[0], true, false);
                Vector3 newPos = default(Vector3);
                Vector3? viewDirection = null;
                if (clientInfo == null && (GameManager.IsDedicatedServer || !ConsoleHelper.ParamIsLocalPlayer(_params[0], true, false)))
                {
                    SdtdConsole.Instance.Output("Playername or entity/steamid id not found.");
                    return false;
                }
                if (_params.Count >= 4)
                {
                    int num;
                    if (!int.TryParse(_params[1], out num))
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given x coordinate is not a valid integer");
                        return false;
                    }
                    int num2;
                    if (!int.TryParse(_params[2], out num2))
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given y coordinate is not a valid integer");
                        return false;
                    }
                    int num3;
                    if (!int.TryParse(_params[3], out num3))
                    {
                        SingletonMonoBehaviour<SdtdConsole>.Instance.Output("The given z coordinate is not a valid integer");
                        return false;
                    }
                    newPos.x = (float)num;
                    newPos.y = (float)num2;
                    newPos.z = (float)num3;
                    if (_params.Count == 5)
                    {
                        if (_params[4].EqualsCaseInsensitive("n") || _params[4].EqualsCaseInsensitive("north"))
                        {
                            viewDirection = new Vector3?(new Vector3(0f, 0f, 0f));
                        }
                        else if (_params[4].EqualsCaseInsensitive("ne") || _params[4].EqualsCaseInsensitive("northeast"))
                        {
                            viewDirection = new Vector3?(new Vector3(0f, 45f, 0f));
                        }
                        else if (_params[4].EqualsCaseInsensitive("e") || _params[4].EqualsCaseInsensitive("east"))
                        {
                            viewDirection = new Vector3?(new Vector3(0f, 90f, 0f));
                        }
                        else if (_params[4].EqualsCaseInsensitive("se") || _params[4].EqualsCaseInsensitive("southeast"))
                        {
                            viewDirection = new Vector3?(new Vector3(0f, 135f, 0f));
                        }
                        else if (_params[4].EqualsCaseInsensitive("s") || _params[4].EqualsCaseInsensitive("south"))
                        {
                            viewDirection = new Vector3?(new Vector3(0f, 180f, 0f));
                        }
                        else if (_params[4].EqualsCaseInsensitive("sw") || _params[4].EqualsCaseInsensitive("southwest"))
                        {
                            viewDirection = new Vector3?(new Vector3(0f, 225f, 0f));
                        }
                        else if (_params[4].EqualsCaseInsensitive("w") || _params[4].EqualsCaseInsensitive("west"))
                        {
                            viewDirection = new Vector3?(new Vector3(0f, 270f, 0f));
                        }
                        else if (_params[4].EqualsCaseInsensitive("nw") || _params[4].EqualsCaseInsensitive("northwest"))
                        {
                            viewDirection = new Vector3?(new Vector3(0f, 315f, 0f));
                        }
                    }
                }
                else if (_params.Count == 2)
                {
                    ClientInfo clientInfo2 = ConsoleHelper.ParseParamIdOrName(_params[1], true, false);
                    EntityPlayer entityPlayer;
                    if (clientInfo2 == null)
                    {
                        if (GameManager.IsDedicatedServer || !ConsoleHelper.ParamIsLocalPlayer(_params[1], true, false))
                        {
                            SingletonMonoBehaviour<SdtdConsole>.Instance.Output("Target playername or entity/steamid id not found.");
                            return false;
                        }
                        entityPlayer = GameManager.Instance.World.GetPrimaryPlayer();
                    }
                    else
                    {
                        entityPlayer = GameManager.Instance.World.Players.dict[clientInfo2.entityId];
                    }
                    newPos = entityPlayer.GetPosition();
                    newPos.y += 1f;
                    newPos.z += 1f;
                }
                NetPackageTeleportPlayer netPackageTeleportPlayer = NetPackageManager.GetPackage<NetPackageTeleportPlayer>().Setup(newPos, viewDirection, false);
                if (clientInfo == null)
                {
                    netPackageTeleportPlayer.ProcessPackage(GameManager.Instance.World, GameManager.Instance);
                    return false;
                }

                clientInfo.SendPackage(netPackageTeleportPlayer);

                //fix duping exploit
                DamageHandler.StartThreadParameterizedCL(clientInfo);

                return false;
            }
            catch (Exception e)
            {
                Log.Out($"Error in TPB: {e}");
            }

            return true;
        }

        public class ManagedZombie
        {
            public ManagedZombie(EntityEnemy _zombie, EntityPlayer _player)
            {
                this.zombie = _zombie;
                this.player = _player;
            }

            public EntityPlayer player;

            public EntityEnemy zombie;

            public float updateDelay;
        }
    }
}
