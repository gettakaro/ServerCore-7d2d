---
title: Settings file
description: Reference for every setting available in PrismaCoreSettings.xml, ServerCore's configuration file.
---

## Intro

ServerCore has a lot of flexibility when it comes to how it behaves and what features are active. Settings can be done by consolecommands and are set in /Saves/PrismaCoreSettings.xml eventually. PrismaCoreSettings.xml can be editted manually too and saving it will make the changed settings effective immediately even on a running server.

## The various settings explained

`<AdvClaims_Reversed_TpHeight>-1</AdvClaims_Reversed_TpHeight>`

By default the revesed claim teleport sends the player to ground level (y=-1). Set this to a height if you want the player to tp to a specific height.

`<AdvClaims_PVP_KillingMode>3</AdvClaims_PVP_KillingMode>`

Define the PVP killing mode in an Adv. Claim PVP

`<BlockUTF8Names_Enabled>true</BlockUTF8Names_Enabled>`

Prevent or allow players to connect that have UTF-8 charset characters in their name.

`<BloodmoonSpawner_DespawnAllOnStart>false</BloodmoonSpawner_DespawnAllOnStart>`

Despawn all zombies alive on map right before bloodmoon starts. This will make sure you get as much bloodmoonspawns as possible.

`<BloodmoonSpawner_OverrideVanillaSpawner>false</BloodmoonSpawner_OverrideVanillaSpawner>`

Set true to override vanilla bloodmoon spawner and let ServerCore handle bloodmoon.

`<BloodmoonSpawner_Overridden_AdjustBMEnemyCountPerPlayerToNrOnlinePlayers>true</BloodmoonSpawner_Overridden_AdjustBMEnemyCountPerPlayerToNrOnlinePlayers>`

Set true to let ServerCore handle the bloodmoonspawning based on number of online players. This is the full auto mode and is recommended. Max alive zombies per player will be calculated and when players are in a bloodmoon party, ServerCore will bring the joy to all members of that party.

`<BloodmoonSpawner_Overridden_BMEnemyCountPerPlayer>2</BloodmoonSpawner_Overridden_BMEnemyCountPerPlayer>`

Only used if the previous setting is false. This will set the maximum number of zombies alive per player during bloodmoon regardless of number of players online.

`<BloodmoonSpawner_Overridden_AddMaxAliveServerDuringBloodmoon>0</BloodmoonSpawner_Overridden_AddMaxAliveServerDuringBloodmoon>`

This value will be added to the maximum allowed zombies alive that is configured in serverconfig.xml. Use this to spice up the bloodmoon spawning even more by allowing more zombies alive during bloodmoons.

`<SpecialCharacters>!@#$%^&amp;*(),.?":;|&lt;&gt;'</SpecialCharacters>`

Define explicit special characters that are not allowed in playernames. Beware of xml breaking characters. They need to be escaped.

`<SpecialCharactersNameBlock_Enabled>false</SpecialCharactersNameBlock_Enabled>`

Enable/Disable preventing players with special characters in their name from connecting.

`<Bundo_HistorySize>5</Bundo_HistorySize>`

The number of bundo's that are stored and you can do after prefab commands like fblock, brender, bdup, brepblock etc.

`<ChatCommandPermissions_ft>0</ChatCommandPermissions_ft>`

`<ChatCommandPermissions_ftw>0</ChatCommandPermissions_ftw>`

`<ChatCommandPermissions_mv>0</ChatCommandPermissions_mv>`

`<ChatCommandPermissions_mvw>0</ChatCommandPermissions_mvw>`

`<ChatCommandPermissions_bubble>0</ChatCommandPermissions_bubble>`

`<ChatCommandPermissions_listwp>0</ChatCommandPermissions_listwp>`

`<ChatCommandPermissions_setwp>0</ChatCommandPermissions_setwp>`

`<ChatCommandPermissions_delwp>0</ChatCommandPermissions_delwp>`

`<ChatCommandPermissions_tb>0</ChatCommandPermissions_tb>`

`<ChatCommandPermissions_rt>0</ChatCommandPermissions_rt>`

`<ChatCommandPermissions_get>0</ChatCommandPermissions_get>`

`<ChatCommandPermissions_rtc>0</ChatCommandPermissions_rtc>`

`<ChatCommandPermissions_bag>1000</ChatCommandPermissions_bag>`

`<ChatCommandPermissions_ls>0</ChatCommandPermissions_ls>`

`<ChatCommandPermissions_day7>1000</ChatCommandPermissions_day7>`

`<ChatCommandPermissions_hostiles>0</ChatCommandPermissions_hostiles>`

`<ChatCommandPermissions_bed>0</ChatCommandPermissions_bed>`

`<ChatCommandPermissions_loctrack>0</ChatCommandPermissions_loctrack>`

Set permissionlevel for all available ServerCore chatcommands. For free for all use permission level 1000. To completely disable a chatcommand use permission level -1.

`<PrismaCorePrefix>/</PrismaCorePrefix>`

The prefix for ServerCore chatcommands

`<DamageDetection_MinAmountDamage>5000</DamageDetection_MinAmountDamage>`

The amount of damage done on entities which will trigger the damagedetection anticheat.

Logline example for using for custom CSMM discord notification:

[PrismaCore]damageDetection(Entity): Player Prisma501 (7656114567822412) triggered damage detection! Damage done: 5000

`<DamageDetection_DetectedCommand>ban add ${platformId} 10 years "Nice hack mate"</DamageDetection_DetectedCommand>`

The (optional) command(s) to execute when damagedetections gets triggered. Use semicolon ( ; ) to seperate commands. Placeholders for use in commands: (casesensitive): ${playerName}, ${entityId} and ${platformId}

`<DamageDetection_ExcludeAdminLvl>0</DamageDetection_ExcludeAdminLvl>`

The player/admin level which will be excluded from damagedetection check.

`<DisableSleeperRespawn_Enabled>false</DisableSleeperRespawn_Enabled>`

When enabled sleepers will not respawn anymore AFTER they have been cleared. Alters the sleepervolumes on map and is permanent after sleepers have been cleared from a POI.

`<DisableSleepers_Enabled>false</DisableSleepers_Enabled>`

When enabled there will be no sleeperspawns in the world. Not permanent. When disabled after enabling sleepers will spawn again.

`<DisableSleepers_BloodmoonOnly_Enabled>false</DisableSleepers_BloodmoonOnly_Enabled>`

Same as disablesleepers above but ONLY prevents sleeperspawns during bloodmoon. Not permanent.

`<DroneDupePrevention_Enabled>true</DroneDupePrevention_Enabled>`

Enable to prevent players from teleporting if they have a drone deployed. A drone and teleporting can be used to easily dupe items ingame.

`<GMSG_PlayerJoined_Enabled>true</GMSG_PlayerJoined_Enabled>`

`<GMSG_PlayerLeft_Enabled>true</GMSG_PlayerLeft_Enabled>`

`<GMSG_PlayerDied_Enabled>true</GMSG_PlayerDied_Enabled>`

`<GMSG_PlayerKilled_Enabled>true</GMSG_PlayerKilled_Enabled>`

Show native gamemessages in chat for joining, leaving, dying and killing. These are the vanilla global gamemessages and disabling them can have impact on CSMM hooks you might have on those actions.

`<HideChatCommandPrefixes_Prefixes>/</HideChatCommandPrefixes_Prefixes>`

Define the prefixes for chatcommands that are not to be shown in public chat. Seperate with comma for multiple prefix hiding (/,$).

`<HideChatCommandPrefixes_Enabled>true</HideChatCommandPrefixes_Enabled>`

Enable or disable prefix hiding in chat. Prefixes in HideChatCommandPrefixes_Prefixes will be used when enabled.

`<LevelJumpDetection_MinimumLevelJumpTrigger>2</LevelJumpDetection_MinimumLevelJumpTrigger>`

The amount of jumping levels on which the leveljumping detection will trigger. For vanilla a jump of 2 is impossible, but if you have mods/events that grant a LOT of XP a jump of 3 or 4 for might be possible. You can configure a higher level jump trigger with this setting then.

`<LevelJumpDetection_DetectedCommand>arrest ${platformId}</LevelJumpDetection_DetectedCommand>`

The (optional) command(s) to execute when leveljumping detection gets triggered. Use semicolon ( ; ) to seperate commands. Placeholders for use in commands: (casesensitive): ${playerName}, ${entityId} and ${platformId}

`<LevelJumpDetection_ExcludeAdminLvl>0</LevelJumpDetection_ExcludeAdminLvl>`

The player/admin level which will be excluded from leveljumping check.

`<LocationTracker_Enabled>true</LocationTracker_Enabled>`

Enable or disable local location tracking.

`<LocationTracker_ChatCommand>/loctrack</LocationTracker_ChatCommand>`

Define the chatcommand for locationtracker queries in chat. Prefix MUST be included.

`<LocationTracker_ChatCommandEnabled>true</LocationTracker_ChatCommandEnabled>`

Enable or disable the availability of the locationtracker chatcommand.

`<LocationTracker_RecordingIntervalSeconds>15</LocationTracker_RecordingIntervalSeconds>`

Location recording interval in seconds.

`<LocationTracker_MaximumDataAgeHours>72</LocationTracker_MaximumDataAgeHours>`

Number of IRL hours that data will be kept in tracker databases. Use 0 for infinite storage (no cleaning will take place).

`<LocationTracker_NearDistance>200</LocationTracker_NearDistance>`

Distance used when doing /loctrack near. Distance is in blocks.

`<LocationTracker_ResponseColor>4DA6FF</LocationTracker_ResponseColor>`

The HEX color that will be used in locationtracker chatcommands responses.

`<MaxAdminLevelGodMode>0</MaxAdminLevelGodMode>`

Define the player/admin level which is allowed to use Godmode whithout triggering Godmode anticheat.

Logline example for using for custom CSMM discord notification:

[PrismaCore]Unauthorized GodMode detected on Prisma501 (23654789532547) !!!!!

[PrismaCore]Permissionlevel: 1000 MaxAdminLevelGodMode: 0

`<GodModeDetectedCommand>ban add ${platformId} 10 years "Nice hack mate"</GodModeDetectedCommand>`

The (optional) command(s) to execute when Godmode Anticheat gets triggered. Use semicolon ( ; ) to seperate commands. Placeholders for use in commands: (casesensitive): ${playerName}, ${entityId} and ${platformId}

`<MaxAdminLevelSpectatorMode>0</MaxAdminLevelSpectatorMode>`

Define the player/admin level which is allowed to use Spectatormode whithout triggering Spectatormode anticheat.

Logline example for using for custom CSMM discord notification:

[PrismaCore]Unauthorized SpectatorMode detected on Prisma501 (23654789532547) !!!!!

[PrismaCore]Permissionlevel: 1000 MaxAdminLevelSpectatorMode: 0

`<SpectatorModeDetectedCommand>ban add ${platformId} 10 years "Nice hack mate"</SpectatorModeDetectedCommand>`

The (optional) command(s) to execute when SpectatorMode Anticheat gets triggered. Use semicolon ( ; ) to seperate commands. Placeholders for use in commands: (casesensitive): ${playerName}, ${entityId} and ${platformId}

`<MaxChatLength>0</MaxChatLength>`

Define the maximum allowed number of characters in chat. The chatmessage will get blocked if length exceeds this setting.

`<NighttimeAnnouncer_Enabled>true</NighttimeAnnouncer_Enabled>`

Enable/Disable the nighttime announcer.

`<NighttimeAnnouncer_Warnhours>2</NighttimeAnnouncer_Warnhours>`

Number of ingame hours before 22:00 the nighttimeannouncer should announce.

`<NotifyAdmin_Level>0</NotifyAdmin_Level>`

The level of players/admins that will get notified (if online) when a player with an expired reserved slot joins the server.

`<PlayerFlying_TriggerHeight>0</PlayerFlying_TriggerHeight>`

The height (in blocks) that will trigger flying detection anticheat. 0 is disabled.

Logline example for using for custom CSMM discord notification:

[PrismaCore]Player Prisma501 (23654789532547) seems to be flying !!!!!

`<PlayerFlying_MaxAdminLevelFlying>0</PlayerFlying_MaxAdminLevelFlying>`

The players/admins level that will be exluded from flying detection anticheat.

`<PreventFallingBlocks>400</PreventFallingBlocks>`

If set to 0, falling blocks prevention is disabled. Setting to any number higher than 0 will prevent ALL blocks from falling. This number will be used to log the falling blocks if the number of simoultaniously falling blocks is higher than this setting.

Logline example for using for custom CSMM discord notification:

[PrismaCore] 658 falling blocks prevented! @ 2130 7 161

`<QuestPoiProtection_Enabled>true</QuestPoiProtection_Enabled>`

Quests wont activate if there is a bedroll/lcb in the quest poi. Enable protection and players cannot place bedrolls/lcb's in poi's that can be selected as questpoi by the game. Exceptions per POI or POI type can be added in /Saves/WorldName/SeedName/ResetRegions/QuestPoi_Exceptions.txt

`<AllPoiProtection_Enabled>false</AllPoiProtection_Enabled>`

Enable protection and players cannot place bedrolls/lcb's in ALL poi's on map. Exceptions per POI or POI type can be added in /Saves/WorldName/SeedName/ResetRegions/AllPoi_Exceptions.txt

`<ResetPrefabs_ExcludeClaimedPrefabs>true</ResetPrefabs_ExcludeClaimedPrefabs>`

If true, prefabs that contain a lcb will not be reset when doing a resetrwgprefabs (rrp) run.

`<ShutdownBA_DelayRestartBloodDayAfter>15</ShutdownBA_DelayRestartBloodDayAfter>`

Shutdownba console command is bloodmoon aware. Here you define at which hour during bloodday and later the shutdown of the server will be delayed.

`<ShutdownBA_DelayRestartAfterBloodmoonUntil>10</ShutdownBA_DelayRestartAfterBloodmoonUntil>`

The hour after bloodmoon that the delayed server shutdown will kick in again after a delayed shutdownba.

`<ShutdownBA_MinimumUptimeRequired>0</ShutdownBA_MinimumUptimeRequired>`

The minimum amount of minutes the server has to be up before shutdownba will reboot the server. 0 means reboot without considdering the server uptime. Any number higher than 0 will define the number of minutes the server has to be up before the shutdownba will actually reboot the server.

`<Vehicles_RemoveOnRestart>false</Vehicles_RemoveOnRestart>`

If true, all vehicles that are NOT stored (backpack, container etc) will be removed at every server restart.

`<VIPModGuard_Enabled>false</VIPModGuard_Enabled>`

If true, ServerCore flags players who have an item mod listed in `VIPModGuardItems.txt` installed in their gear. See [VIP ModGuard](/ServerCore-7d2d/features/vip-modguard/).

`<VIPModGuard_ExcludeAdminLvl>0</VIPModGuard_ExcludeAdminLvl>`

Players with an admin permission level at or below this number are never flagged by VIP ModGuard.

`<VIPModGuard_DetectedCommand>none</VIPModGuard_DetectedCommand>`

The console command, or several separated by `;`, that `evmg` runs for a flagged player. See [VIP ModGuard](/ServerCore-7d2d/features/vip-modguard/) for the placeholders.

`<Drones_RemoveOnRestart>false</Drones_RemoveOnRestart>`

If true, all drones that are NOT stored (backpack, container etc) will be removed at every server restart.

`<CreateTimeStampedCopyLogFile>true</CreateTimeStampedCopyLogFile>`

For serverowners that have a server start mechanism in place that will overwrite the logfile over and over again. If true this will create a timestamped copy of the logfile with every server restart.

`<WebUI_Port>28006</WebUI_Port>`

TCP Port of ServerCore ClaimCreator. Will initially be controlpanel port + 3. Use this only if you have a gamehoster who doesnt want to open up a simple tcp port. Allocs patched map will break if you change port. You are on your own for making that work again.
