---
title: Migrating from PrismaCore
description: How to move a server from PrismaCore 2.5 to ServerCore, and what stays the same.
---

ServerCore is the continuation of PrismaCore 2.5. It's built to replace it with nothing else to change: same commands, same output, same data files.

## Before you start

- **Game version.** ServerCore 3.1.0 targets game 3.3.0 b18. PrismaCore 2.5 was built for game 3.2.0 b10, so update the game and swap the mod at the same time.
- **Other mods.** Anything that worked with PrismaCore 2.5 should work with ServerCore. The one exception is PrismaCore itself: never run both.

## Steps

1. **Stop the server.**
2. **Back up your PrismaCore data.** ServerCore keeps using the same files, but take a copy anyway. None of them are in the mod folder:
   - In the save game root (the `Saves` folder): `PrismaCoreSettings.xml`, `PrismaCoreStrings.xml`, `PrismaCoreDonorSlots.xml`, `PrismaCorePermaDeath.xml`
   - In each world's save folder: the `PrismaCore*.db` files (players, claims, waypoints, teleports on spawn, group colours, and vehicle and drone owners)
3. **Remove `Mods/PrismaCore`.**
4. **Install ServerCore**: extract `ServerCore-<version>.zip` so you have `Mods/ServerCore/`.
5. **Start the server** and check the log for ServerCore loading. Run `pc-help` in the console to confirm the commands are there.

Your settings, claims, waypoints and other data are read from the same files as before.

## Server managers and modules

- **Takaro**: nothing to change. The 7 Days to Die server type with "Use PrismaCore/CPM" turned on keeps working, because the commands and log lines are unchanged.
- **CSMM**: nothing to change, for the same reason.
- **Community modules** that call PrismaCore commands keep working unchanged.

See [Compatibility](/ServerCore-7d2d/project/compatibility/) for what carries over from PrismaCore. For common questions about this migration, see the [FAQ](/ServerCore-7d2d/start-here/faq/).
