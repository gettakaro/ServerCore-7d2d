using System;
using System.Collections.Generic;
using UnityEngine;

namespace ServerCore.CustomCommands
{
    public class RemoveItemConsole : ConsoleCmdAbstract
    {
        public static class Phrases
        {
            public const string Removed = "Removed {0}x {1}{2} from {3}.";
            public const string NothingFound = "No '{0}{1}' found on {2}.";
            public const string PlayerNotFound = "Player '{0}' not found.";
            public const string PlayerNotSpawned = "Player '{0}' is not spawned or is dead. Unable to remove item.";
            public const string ItemNotFound = "Unable to find item '{0}'.";
            public const string InvalidArgs = "[PrismaCore] Wrong number of arguments, expected 2 to 4, found '{0}'";
            public const string SaveNote = "[PrismaCore] Player will be kicked so inventory changes apply on reconnect.";
        }

        public override string getDescription() =>
            "Built by Tree. Full credits and many thanks for this beauty!\n" +
            "Removes an item from a player's toolbelt, backpack, and equipment; also persists changes to saved player data.\n" +
            "Target by PlayerName, EntityId, or SteamID.\n" +
            "Optional quality targeting; supports 'all' to remove everything.";
        public override string getHelp()
        {
            return "Usage:\n" +
                   "  rii <PlayerName|SteamID|EntityId> <Item> [Count|all] [Quality]\n" +
                   "  rii all <Item> [Count|all] [Quality]\n\n" +
                   "Examples:\n" +
                   "  rii Bob pistol                                 -> removes 1 pistol (any quality)\n" +
                   "  rii Bob pistol 2                               -> removes 2 pistols (any quality)\n" +
                   "  rii Bob pistol 2 2                             -> removes 2 pistols of quality 2\n" +
                   "  rii Bob pistol all                             -> removes ALL pistols (any quality)\n" +
                   "  rii Bob pistol all 5                           -> removes ALL pistols of quality 5\n" +
                   "  rii all pistol all                             -> removes ALL pistols from ALL players\n" +
                   "  rii Steam_76561198016034203 gunMGT0PipeMachineGun 2 -> removes 2 Pipe Machine Guns from that SteamID\n\n" +
                   "Notes:\n" +
                   "  - If Quality is provided, only that quality is removed.\n" +
                   "  - If Quality is omitted, all qualities match.\n" +
                   "  - 'all' can be used instead of Count to remove every matching item.\n";
        }

        public override string[] getCommands() => new[] { "pc-removeinvitem", "rii", "removeinvitem" };

        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            try
            {
                if (_params.Count < 2 || _params.Count > 4)
                {
                    SdtdConsole.Instance.Output(Phrases.InvalidArgs, _params.Count);
                    return;
                }

                string target = _params[0];
                string itemName = _params[1];

                if (string.IsNullOrEmpty(itemName))
                {
                    SdtdConsole.Instance.Output(Phrases.ItemNotFound, itemName);
                    return;
                }

                ItemValue baseIv = ItemClass.GetItem(itemName);
                if (baseIv == null || baseIv.type == ItemValue.None.type)
                {
                    SdtdConsole.Instance.Output(Phrases.ItemNotFound, itemName);
                    return;
                }

                // Defaults
                int toRemove = 1;
                ushort? quality = null;
                bool removeAll = false;

                // Parse: [Count|all] [Quality] (and allow 'all' in either position)
                if (_params.Count >= 3)
                {
                    string tok = _params[2];
                    if (tok.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        removeAll = true;
                    }
                    else if (int.TryParse(tok, out int count))
                    {
                        toRemove = Mathf.Clamp(count, 1, 1_000_000);
                    }
                }

                if (_params.Count >= 4)
                {
                    string tok = _params[3];
                    if (tok.Equals("all", StringComparison.OrdinalIgnoreCase))
                    {
                        // allow "treerm Bob pistol 2 all" meaning remove all (ignore the 2)
                        removeAll = true;
                    }
                    else if (int.TryParse(tok, out int q))
                    {
                        q = Mathf.Clamp(q, 0, ushort.MaxValue);
                        quality = (ushort)q;
                    }
                }

                if (removeAll)
                    toRemove = int.MaxValue;

                // If quality is set (>0), enforce match by preparing a target ItemValue with that quality
                ItemValue targetIv = baseIv;
                if (quality.HasValue && quality.Value > 0)
                {
                    targetIv = baseIv.Clone();
                    targetIv.Quality = quality.Value;
                }

                if (target.Equals("all", StringComparison.OrdinalIgnoreCase))
                {
                    foreach (var cInfo in ConnectionManager.Instance.Clients.list)
                    {
                        if (cInfo == null || !cInfo.loginDone) continue;
                        RemoveFromOne(cInfo, targetIv, itemName, toRemove, quality);
                    }
                }
                else
                {
                    var cInfo = GetClientInfoFromNameOrId(target);
                    if (cInfo == null)
                    {
                        SdtdConsole.Instance.Output(Phrases.PlayerNotFound, target);
                        return;
                    }
                    RemoveFromOne(cInfo, targetIv, itemName, toRemove, quality);
                }
            }
            catch (Exception e)
            {
                SdtdConsole.Instance.Output($"[TreeRemoveItem] Error: {e}");
            }
        }

        // --- helpers ----------------------------------------------------------

        private static ClientInfo GetClientInfoFromNameOrId(string identifier)
        {
            foreach (var cInfo in ConnectionManager.Instance.Clients.list)
            {
                if (cInfo.playerName.Equals(identifier, StringComparison.OrdinalIgnoreCase) ||
                    cInfo.entityId.ToString() == identifier ||
                    cInfo.PlatformId.CombinedString == identifier)
                    return cInfo;
            }
            return null;
        }

        private static void RemoveFromOne(ClientInfo cInfo, ItemValue targetIv, string itemName, int toRemove, ushort? quality)
        {
            var world = GameManager.Instance.World;
            if (world == null) return;

            world.Players.dict.TryGetValue(cInfo.entityId, out var player);
            if (player == null || !player.IsSpawned() || player.IsDead())
            {
                SdtdConsole.Instance.Output(Phrases.PlayerNotSpawned, cInfo.PlatformId.CombinedString);
                return;
            }

            string displayName = player.PlayerDisplayName ?? cInfo.playerName;

            // ---- LIVE REMOVAL ----
            int removedLive = 0;

            removedLive += RemoveFromInventory(player.inventory, player, targetIv, toRemove - removedLive);

            if (removedLive < toRemove)
                removedLive += RemoveFromBag(player.bag, toRemove - removedLive, targetIv);

            if (removedLive < toRemove && player.equipment != null)
                removedLive += RemoveFromEquipment(player, targetIv, toRemove - removedLive, quality);

            // ---- ALWAYS MIRROR TO SAVE ----
            int removedSave = RemoveFromSavedData(cInfo, targetIv, toRemove, quality);

            int removedTotal = Math.Max(removedLive, removedSave);

            string qualSuffix = (quality.HasValue && quality.Value > 0) ? $" (quality={quality.Value})" : "";

            if (removedLive > 0 || removedSave > 0)
            {
                SdtdConsole.Instance.Output(Phrases.Removed, removedTotal, itemName, qualSuffix, displayName);
                SdtdConsole.Instance.Output(Phrases.SaveNote);

                GameUtils.KickPlayerForClientInfo(
                    cInfo,
                    new GameUtils.KickPlayerData(GameUtils.EKickReason.ManualKick, _customReason: "Inventory change applied; please reconnect."));
            }
            else
            {
                SdtdConsole.Instance.Output(Phrases.NothingFound, itemName, qualSuffix, displayName);
            }
        }

        // Toolbelt (Inventory) – DecItem + held-item sync
        private static int RemoveFromInventory(global::Inventory inv, EntityPlayer player, ItemValue targetIv, int remaining)
        {
            if (inv == null || remaining <= 0) return 0;

            int removed = inv.DecItem(targetIv, remaining);
            if (removed > 0)
            {
                inv.CallOnToolbeltChangedInternal();
                player.bPlayerStatsChanged = true;

                var pkg = NetPackageManager.GetPackage<NetPackageHoldingItem>().Setup(player);
                ConnectionManager.Instance.SendPackage(pkg, false, -1, player.entityId);
            }
            return removed;
        }

        // Backpack (Bag)
        private static int RemoveFromBag(global::Bag bag, int remaining, ItemValue targetIv)
        {
            if (bag == null || remaining <= 0) return 0;
            return bag.DecItem(targetIv, remaining);
        }

        // Equipment (slot scan, quality enforced if provided)
        private static int RemoveFromEquipment(EntityPlayer player, ItemValue targetIv, int remaining, ushort? quality)
        {
            if (remaining <= 0 || player.equipment == null) return 0;

            int removed = 0;
            var eq = player.equipment;
            int slots = eq.GetSlotCount();

            for (int i = 0; i < slots && removed < remaining; i++)
            {
                var iv = eq.GetSlotItem(i);
                if (iv == null || iv.type == ItemValue.None.type) continue;

                if (iv.type == targetIv.type && (!quality.HasValue || iv.Quality == quality.Value))
                {
                    eq.ItemGrid.SetItem(i, ItemStack.Empty);
                    removed += 1;
                }
            }
            if (removed > 0) player.bPlayerStatsChanged = true;
            return removed;
        }

        // --------- SAVE-FILE PERSIST ---------

        private static int RemoveFromSavedData(ClientInfo cInfo, ItemValue targetIv, int maxToRemove, ushort? quality)
        {
            var pdf = cInfo.latestPlayerData;
            if (pdf == null || maxToRemove <= 0) return 0;

            int removed = 0;

            if (removed < maxToRemove)
            {
                global::Inventory belt = PlayerDataBlobs.ReadInventory(pdf);
                int fromBelt = FilterStacks(belt.ItemGrid, targetIv, maxToRemove - removed, quality);
                if (fromBelt > 0) PlayerDataBlobs.WriteInventory(pdf, belt);
                removed += fromBelt;
            }

            if (removed < maxToRemove)
            {
                global::Bag bag = PlayerDataBlobs.ReadBag(pdf);
                int fromBag = FilterStacks(bag.ItemGrid, targetIv, maxToRemove - removed, quality);
                if (fromBag > 0) PlayerDataBlobs.WriteBag(pdf, bag);
                removed += fromBag;
            }

            if (removed < maxToRemove)
            {
                Equipment equipment = PlayerDataBlobs.ReadEquipment(pdf);
                int fromEquipment = FilterEquip(equipment, targetIv, maxToRemove - removed, quality);
                if (fromEquipment > 0) PlayerDataBlobs.WriteEquipment(pdf, equipment);
                removed += fromEquipment;
            }

            if (pdf.dragAndDropItem != null && removed < maxToRemove)
            {
                var st = pdf.dragAndDropItem;
                if (st.itemValue != null &&
                    st.itemValue.type == targetIv.type &&
                    (!quality.HasValue || st.itemValue.Quality == quality.Value))
                {
                    int take = Math.Min(st.count, maxToRemove - removed);
                    st.count -= take;
                    if (st.count <= 0) st.Clear();
                    pdf.dragAndDropItem = st;
                    removed += take;
                }
            }

            if (removed > 0)
            {
                pdf.bModifiedSinceLastSave = true;
                try { GameManager.Instance.SavePlayerData(cInfo, pdf); } catch { /* best effort */ }
            }

            return removed;
        }

        private static int FilterStacks(ItemStackGrid slots, ItemValue targetIv, int remaining, ushort? quality)
        {
            int removed = 0;
            for (int i = 0; i < slots.Length && removed < remaining; i++)
            {
                var st = slots[i];
                if (st == null || st.IsEmpty()) continue;
                if (st.itemValue == null || st.itemValue.type != targetIv.type) continue;
                if (quality.HasValue && st.itemValue.Quality != quality.Value) continue;

                int take = Math.Min(st.count, remaining - removed);
                st.count -= take;
                if (st.count <= 0) st.Clear();
                removed += take;
            }
            return removed;
        }

        private static int FilterEquip(Equipment eq, ItemValue targetIv, int remaining, ushort? quality)
        {
            int removed = 0;
            int n = eq.GetSlotCount();
            for (int i = 0; i < n && removed < remaining; i++)
            {
                var iv = eq.GetSlotItem(i);
                if (iv == null || iv.type != targetIv.type) continue;
                if (quality.HasValue && iv.Quality != quality.Value) continue;

                eq.ItemGrid.SetItem(i, ItemStack.Empty);
                removed++;
            }
            return removed;
        }
    }
}