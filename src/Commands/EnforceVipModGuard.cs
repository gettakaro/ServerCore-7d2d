using System;
using System.Collections.Generic;

namespace ServerCore.CustomCommands
{
    public class EnforceVipModGuard : ConsoleCmdAbstract
    {
        public override string getDescription()
        {
            return "Enforce VIP mod ownership action for non-VIPs (ONLY USE WITH CSMM FOR CHECKING PLAYER ROLE LEVEL NON_VIP!).";
        }
        public override string getHelp()
        {
            return "Usage:\n" +
                   " enforcevipmodguard <EntityId/Name/steamId>\n" +
                   " enforcevipmodguard list\n" +
                   " enforcevipmodguard remove <entityId>\n" +
                   " ONLY USE VIA CSMM AFTER CHECKING PLAYER ROLE LEVEL NON_VIP!";
        }
        public override string[] getCommands()
        {
            return new[] { "pc-enforcevipmodguard", "enforcevipmodguard", "evmg" };
        }
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count != 1 && _params.Count !=2)
                {
                    SdtdConsole.Instance.Output(string.Format("ERR: Wrong number of arguments, expected 1 or 2, found {0}", _params.Count));
                    return;
                }

                if (_params[0].Trim().EqualsCaseInsensitive("list"))
                {
                    SdtdConsole.Instance.Output($"Players owning VIP mod(s) in memory:");
                    SdtdConsole.Instance.Output($"entityId        VIPMod");
                    
                    foreach(KeyValuePair<int,string> kvp in RegionReset.lstVipModUsers)
                    {
                        SdtdConsole.Instance.Output($"{kvp.Key}     {kvp.Value}");
                    }
                    return;
                }

                if (_params.Count == 2)
                {
                    if(_params[0].EqualsCaseInsensitive("remove"))
                    {
                        int eId = int.Parse(_params[1]);
                        if (RegionReset.lstVipModUsers.ContainsKey(eId))
                        {
                            RegionReset.lstVipModUsers.Remove(eId);
                            ClientInfo ciID = ConsoleHelper.ParseParamIdOrName(_params[1]);
                            if (ciID != null)
                            {
                                if(RegionReset.lstVIPModGuardCommandFired.Contains(ciID.PlatformId.ToString()))
                                {
                                    RegionReset.lstVIPModGuardCommandFired.Remove(ciID.PlatformId.ToString());
                                }
                            }
                            SdtdConsole.Instance.Output($"entityId {eId} has been removed from the VIPModGuard list.");
                        }
                        else
                        {
                            SdtdConsole.Instance.Output($"entityId {eId} could not be found in VIPModGuard list.");
                        }
                    }
                    else
                    {
                        SdtdConsole.Instance.Output("ERR: incorrect parameters");
                    }
                    return;
                }

                ClientInfo ci = ConsoleHelper.ParseParamIdOrName(_params[0]);
                if (ci == null)
                {
                    string errMsg = "ERR: player cannot be found";
                    SdtdConsole.Instance.Output(errMsg);
                    return;
                }

                if (RegionReset.lstVipModUsers.ContainsKey(ci.entityId))
                {
                    if(!RegionReset.lstVIPModGuardCommandFired.Contains(ci.PlatformId.ToString()))
                    {
                        string command = ServerCoreSettings.Instance.VIPModGuard_DetectedCommand;
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
                                    cmd = cmd.Replace("${steamId}", ci.PlatformId.ToString());
                                    cmd = cmd.Replace("${platformId}", ci.PlatformId.ToString());
                                    cmd = cmd.Replace("${entityId}", ci.entityId.ToString());
                                    cmd = cmd.Replace("${playerName}", ci.playerName);
                                    cmd = cmd.Replace("${vipMod}", RegionReset.lstVipModUsers[ci.entityId]);

                                    SdtdConsole.Instance.ExecuteAsync(cmd, iConsole);
                                }
                            }
                            else
                            {
                                //just 1 command
                                command = command.Replace("${steamId}", ci.PlatformId.ToString());
                                command = command.Replace("${platformId}", ci.PlatformId.ToString());
                                command = command.Replace("${entityId}", ci.entityId.ToString());
                                command = command.Replace("${playerName}", ci.playerName);
                                command = command.Replace("${vipMod}", RegionReset.lstVipModUsers[ci.entityId]);

                                CmdClaimCommandResult iConsole = new CmdClaimCommandResult();
                                SdtdConsole.Instance.ExecuteAsync(command, iConsole);
                            }

                            RegionReset.lstVIPModGuardCommandFired.Add(ci.PlatformId.ToString());

                            Log.Out($"[PrismaCore] VIP mod(s) detected on {ci.playerName} entityId: {ci.entityId}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[PrismaCore] Error in EnforceVipModGuard.Run: {0}.", e));
            }
        }
    }
}
