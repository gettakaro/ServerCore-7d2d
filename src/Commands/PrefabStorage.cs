using System;
using System.Collections.Generic;

namespace ServerCore.CustomCommands
{
    // Storage blocks (crates, safes, LCB storage, ...) are stripped from bexport, brender and bdup
    // so bases can't be copied with their loot, unless the admin passes "withstorage".
    internal static class PrefabStorage
    {
        public const string WithStorageParam = "withstorage";

        // While set, the game treats storage as saved in prefabs, so copyFromWorld, Load and
        // CopyIntoLocal carry crate contents. Only set during a "withstorage" command or an undo copy/restore.
        public static bool IncludeStorage;

        // Undo must restore exactly what was there, including container contents
        public static void RunIncludingStorage(Action _action)
        {
            bool previous = IncludeStorage;
            IncludeStorage = true;
            try
            {
                _action();
            }
            finally
            {
                IncludeStorage = previous;
            }
        }

        public static bool TakeWithStorageParam(List<string> _params)
        {
            int index = _params.FindIndex(p => p.EqualsCaseInsensitive(WithStorageParam));
            if (index < 0)
            {
                return false;
            }
            _params.RemoveAt(index);
            return true;
        }

        public static bool IsStorage(Block _block)
        {
            return _block is BlockCompositeTileEntity composite && composite.CompositeData.HasFeature<TEFeatureStorage>();
        }

        public static int RemoveStorageBlocks(Prefab _prefab)
        {
            int removed = 0;
            for (int x = 0; x < _prefab.size.x; x++)
            {
                for (int y = 0; y < _prefab.size.y; y++)
                {
                    for (int z = 0; z < _prefab.size.z; z++)
                    {
                        BlockValue bv = _prefab.GetBlock(x, y, z);
                        // Child cells of multi-block storage carry the parent's block type, so they go too
                        if (!IsStorage(bv.Block))
                        {
                            continue;
                        }
                        _prefab.SetBlock(x, y, z, BlockValue.Air);
                        _prefab.tileEntities.Remove(new Vector3i(x, y, z));
                        if (!bv.ischild)
                        {
                            removed++;
                        }
                    }
                }
            }
            return removed;
        }
    }
}
