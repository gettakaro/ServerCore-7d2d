using System.IO;

namespace ServerCore
{
    public class RegionWatcher
    {
        public static FileSystemWatcher fsw = new FileSystemWatcher(RegionReset.RegionPath, "regions.txt");
        public static FileSystemWatcher fileWatcher = new FileSystemWatcher(API.GamePath, PermaDeathClass.file);
        public static FileSystemWatcher fileWatcherDonorSlots = new FileSystemWatcher(API.GamePath, ReservedSlots.file);

        public static FileSystemWatcher fileWatcherStrings;
        public static FileSystemWatcher fileWatcherSettings;
        public static FileSystemWatcher fileWatcherBannedItems = new FileSystemWatcher(API.GamePath, "PrismaCoreBannedItems.txt");
        public static FileSystemWatcher filewatcherVIP = new FileSystemWatcher(API.GamePath, "VIPModGuardItems.txt");

        public static void LoadWatchers()
        {
            InitFileWatchers();
        }

        private static void InitFileWatchers()
        {
            fsw.Changed += new FileSystemEventHandler(OnFileChanged);
            fsw.Created += new FileSystemEventHandler(OnFileChanged);
            fsw.Deleted += new FileSystemEventHandler(OnFileChanged);
            fsw.EnableRaisingEvents = true;

            fileWatcher.Changed += new FileSystemEventHandler(OnFileChanged2);
            fileWatcher.Created += new FileSystemEventHandler(OnFileChanged2);
            fileWatcher.Deleted += new FileSystemEventHandler(OnFileChanged2);
            fileWatcher.EnableRaisingEvents = true;

            fileWatcherStrings = new FileSystemWatcher(API.GamePath, ServerCoreStrings.SaveFileName);
            fileWatcherStrings.Changed += new FileSystemEventHandler(OnFileChanged3);
            fileWatcherStrings.Created += new FileSystemEventHandler(OnFileChanged3);
            fileWatcherStrings.Deleted += new FileSystemEventHandler(OnFileChanged3);
            fileWatcherStrings.EnableRaisingEvents = true;

            fileWatcherDonorSlots.Changed += new FileSystemEventHandler(OnFileChanged4);
            fileWatcherDonorSlots.Created += new FileSystemEventHandler(OnFileChanged4);
            fileWatcherDonorSlots.Deleted += new FileSystemEventHandler(OnFileChanged4);
            fileWatcherDonorSlots.EnableRaisingEvents = true;

            fileWatcherSettings = new FileSystemWatcher(API.GamePath, ServerCoreSettings.SaveFileName);
            fileWatcherSettings.Changed += new FileSystemEventHandler(OnFileChanged5);
            fileWatcherSettings.Created += new FileSystemEventHandler(OnFileChanged5);
            fileWatcherSettings.Deleted += new FileSystemEventHandler(OnFileChanged5);
            fileWatcherSettings.EnableRaisingEvents = true;

            fileWatcherBannedItems.Changed += new FileSystemEventHandler(OnFileChanged6);
            fileWatcherBannedItems.Created += new FileSystemEventHandler(OnFileChanged6);
            fileWatcherBannedItems.Deleted += new FileSystemEventHandler(OnFileChanged6);
            fileWatcherBannedItems.EnableRaisingEvents = true;

            filewatcherVIP.Changed += new FileSystemEventHandler(OnFileChanged7);
            filewatcherVIP.Created += new FileSystemEventHandler(OnFileChanged7);
            filewatcherVIP.Deleted += new FileSystemEventHandler(OnFileChanged7);
            filewatcherVIP.EnableRaisingEvents = true;
        }

        private static void OnFileChanged(object source, FileSystemEventArgs e)
        {
            RegionReset.LoadRegions();
        }

        private static void OnFileChanged2(object source, FileSystemEventArgs e)
        {
            PermaDeathClass.Loadxml();
        }

        private static void OnFileChanged3(object source, FileSystemEventArgs e)
        {
            ServerCoreStrings.Load();
        }

        private static void OnFileChanged4(object source, FileSystemEventArgs e)
        {
            ReservedSlots.LoadXml();
            Log.Out("[PrismaCore] PrismaCoreDonorSlots.xml has been reloaded to memory.");
        }

        private static void OnFileChanged5(object source, FileSystemEventArgs e)
        {
            ServerCoreSettings.Load();
        }

        private static void OnFileChanged6(object source, FileSystemEventArgs e)
        {
            RegionReset.LoadBannedItems();
        }

        private static void OnFileChanged7(object source, FileSystemEventArgs e)
        {
            RegionReset.LoadVIPGuardItems();
        }
    }
}
