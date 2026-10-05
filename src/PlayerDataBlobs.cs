using System.IO;

namespace ServerCore
{
    // Since 3.3 PlayerDataFile keeps the toolbelt, backpack and equipment as serialized blobs.
    // An empty blob (a player that never saved) decodes to an empty container.
    public static class PlayerDataBlobs
    {
        public static global::Inventory ReadInventory(PlayerDataFile pdf)
        {
            // Inventory.ReadInto needs a live entity, so decode the toolbelt the way it is written.
            ItemStackGrid grid = ItemStackGrid.Create(new Vector2i(0, 0), XUiC_ItemStack.StackLocationTypes.ToolBelt, _hasLocks: false, _hasPreferences: false, null);
            int selectedSlot = 0;
            StreamUtils.FromBlob(pdf.inventoryData, br =>
            {
                br.ReadByte();
                grid.ReadInto(br, StreamModeRead.Persistency);
                selectedSlot = br.ReadByte();
            });
            return new global::Inventory(grid, selectedSlot);
        }

        public static void WriteInventory(PlayerDataFile pdf, global::Inventory inventory)
        {
            pdf.inventoryData = StreamUtils.ToBlob(bw => inventory.Write(bw, StreamModeWrite.Persistency));
        }

        public static global::Bag ReadBag(PlayerDataFile pdf)
        {
            global::Bag bag = new global::Bag(new Vector2i(0, 0), XUiC_ItemStack.StackLocationTypes.Backpack, null);
            StreamUtils.FromBlob(pdf.bagData, br => bag = global::Bag.Read(br, XUiC_ItemStack.StackLocationTypes.Backpack, StreamModeRead.Persistency, null));
            return bag;
        }

        public static void WriteBag(PlayerDataFile pdf, global::Bag bag)
        {
            pdf.bagData = StreamUtils.ToBlob(bw => bag.Write(bw, StreamModeWrite.Persistency));
        }

        public static Equipment ReadEquipment(PlayerDataFile pdf)
        {
            Equipment equipment = new Equipment();
            StreamUtils.FromBlob(pdf.equipmentData, br => equipment = Equipment.Read(br, StreamModeRead.Persistency));
            return equipment;
        }

        public static void WriteEquipment(PlayerDataFile pdf, Equipment equipment)
        {
            pdf.equipmentData = StreamUtils.ToBlob(bw => equipment.Write(bw, StreamModeWrite.Persistency));
        }
    }
}
