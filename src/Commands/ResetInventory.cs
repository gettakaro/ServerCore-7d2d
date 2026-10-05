using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace ServerCore.CustomCommands
{
    public class ResetInventory : ConsoleCmdAbstract
    {
        public override string getDescription()
        {
            return "Reset a player's inventory completely or partially (bag, belt and equipment).";
        }
        public override string getHelp()
        {
            return "Usage: wi <platformID> [bag] [belt] [equipment] [all]";
        }
        public override string[] getCommands()
        {
            return new[] { "pc-wi", "wi", "wipeinventory" };
        }
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count < 2 || _params.Count > 4)
                {
                    SdtdConsole.Instance.Output(string.Format("ERR: Wrong number of arguments, expected 2 to 4, found {0}", _params.Count));
                    return;
                }

                ClientInfo clientinfo = ConsoleHelper.ParseParamIdOrName(_params[0]);

                if (clientinfo == null)
                {
                    DbPlayer dbplayer = null;

                    if (_params[0].Trim().ToLower().StartsWith("steam_"))
                    {
                        dbplayer = Database.Instance.GetDbPlayer(_params[0].Trim().Replace("steam_", "Steam_"));
                    }
                    else if (_params[0].Trim().ToLower().StartsWith("xbl_"))
                    {
                        dbplayer = Database.Instance.GetDbPlayer(_params[0].Trim().Replace("xbl_", "XBL_"));
                    }
                    else if (_params[0].Trim().ToLower().StartsWith("psn_"))
                    {
                        dbplayer = Database.Instance.GetDbPlayer(_params[0].Trim().Replace("psn_", "PSN_"));
                    }

                    if (dbplayer == null)
                    {
                        SdtdConsole.Instance.Output($"ERR: Player with steamId/XblId/PsnId {_params[0]} can not be found.");
                        return;
                    }

                    if (File.Exists($"{GameIO.GetPlayerDataDir()}/{dbplayer.EOS_Id}.ttp"))
                    {

                        if (File.Exists($"{GameIO.GetPlayerDataDir()}/{dbplayer.EOS_Id}.ttp.bak.prismacore"))
                        {
                            File.Delete($"{GameIO.GetPlayerDataDir()}/{dbplayer.EOS_Id}.ttp.bak.prismacore");
                        }

                        PlayerDataFile playerDataFile = new PlayerDataFile();
                        playerDataFile.Load(GameIO.GetPlayerDataDir(), dbplayer.EOS_Id);

                        if (_params.ContainsCaseInsensitive("bag") || _params.ContainsCaseInsensitive("all"))
                        {
                            global::Bag bag = PlayerDataBlobs.ReadBag(playerDataFile);
                            bag.Clear();
                            PlayerDataBlobs.WriteBag(playerDataFile, bag);
                        }

                        if (_params.ContainsCaseInsensitive("belt") || _params.ContainsCaseInsensitive("all"))
                        {
                            global::Inventory belt = PlayerDataBlobs.ReadInventory(playerDataFile);
                            belt.ItemGrid.ClearItems();
                            PlayerDataBlobs.WriteInventory(playerDataFile, belt);
                        }

                        if (_params.ContainsCaseInsensitive("equipment") || _params.ContainsCaseInsensitive("all"))
                        {
                            PlayerDataBlobs.WriteEquipment(playerDataFile, new Equipment());
                        }

                        if (File.Exists($"{GameIO.GetPlayerDataDir()}/{dbplayer.EOS_Id}.ttp.bak"))
                        {
                            File.Move($"{GameIO.GetPlayerDataDir()}/{dbplayer.EOS_Id}.ttp.bak", $"{GameIO.GetPlayerDataDir()}/{dbplayer.EOS_Id}.ttp.bak.prismacore");
                        }

                        playerDataFile.Save(GameIO.GetPlayerDataDir(), dbplayer.EOS_Id);
                        playerDataFile = null;
                    }
                    else
                    {
                        SdtdConsole.Instance.Output($"ERR: Playerfile for cannot be found!");
                        return;
                    }

                    SdtdConsole.Instance.Output($"You have reset the inventory for the player.");
                }
                else
                {
                    StartThreadParameterized(clientinfo, _params);
                }
            }
            catch (Exception e)
            {
                Log.Out(string.Format("[PrismaCore] Error in ResetInventory.Run: {0}.", e));
            }
        }

        private Thread StartThreadParameterized(ClientInfo clientInfo, List<string> _params)
        {
            var t = new Thread(() => wipeInventoryHandler(clientInfo, _params))
            {
                IsBackground = true
            };
            t.Start();
            return t;
        }

        private static void wipeInventoryHandler(ClientInfo clientInfo, List<string> _params)
        {
            string steamID = clientInfo.CrossplatformId.ToString();
            string msg = "Resetting your inventory.";

            GameUtils.KickPlayerForClientInfo(clientInfo, new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick, 0, default(DateTime), msg));
            Thread.Sleep(7000);

            if (File.Exists($"{GameIO.GetPlayerDataDir()}/{steamID}.ttp"))
            {

                if (File.Exists($"{GameIO.GetPlayerDataDir()}/{steamID}.ttp.bak.prismacore"))
                {
                    File.Delete($"{GameIO.GetPlayerDataDir()}/{steamID}.ttp.bak.prismacore");
                }

                PlayerDataFile playerDataFile = new PlayerDataFile();
                playerDataFile.Load(GameIO.GetPlayerDataDir(), steamID);

                if (_params.ContainsCaseInsensitive("bag") || _params.ContainsCaseInsensitive("all"))
                {
                    global::Bag bag = PlayerDataBlobs.ReadBag(playerDataFile);
                    bag.Clear();
                    PlayerDataBlobs.WriteBag(playerDataFile, bag);
                }

                if (_params.ContainsCaseInsensitive("belt") || _params.ContainsCaseInsensitive("all"))
                {
                    global::Inventory belt = PlayerDataBlobs.ReadInventory(playerDataFile);
                    belt.ItemGrid.ClearItems();
                    PlayerDataBlobs.WriteInventory(playerDataFile, belt);
                }

                if (_params.ContainsCaseInsensitive("equipment") || _params.ContainsCaseInsensitive("all"))
                {
                    PlayerDataBlobs.WriteEquipment(playerDataFile, new Equipment());
                }

                if (File.Exists($"{GameIO.GetPlayerDataDir()}/{steamID}.ttp.bak"))
                {
                    File.Move($"{GameIO.GetPlayerDataDir()}/{steamID}.ttp.bak", $"{GameIO.GetPlayerDataDir()}/{steamID}.ttp.bak.prismacore");
                }

                playerDataFile.Save(GameIO.GetPlayerDataDir(), steamID);
            }
            else
            {
                SdtdConsole.Instance.Output($"ERR: Playerfile for player cannot be found!");
                return;
            }

            SdtdConsole.Instance.Output($"You have reset the inventory for player.");
        }
    }
}
