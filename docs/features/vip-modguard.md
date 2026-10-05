---
title: VIP ModGuard
description: Detect players who have listed item mods, such as donor mods, installed in their gear, and run a command for them.
---

## Intro

Banned items checks catch items in a player's inventory, but not item mods that are installed into a weapon or a piece of armor. VIP ModGuard fills that gap. You list the item mods you want to guard, for example mods you give to donors, and ServerCore notes every player who has one of them installed. Your server manager, such as CSMM or Takaro, then decides whether that player is allowed to have it and tells ServerCore to act.

VIP ModGuard was written by Prisma501 for the PRG servers, who shared it with the community.

VIP ModGuard is off by default and does nothing until you configure it.

## How it works

1. You list the guarded item mods in `VIPModGuardItems.txt`.
2. Each time the server saves a player's data, ServerCore looks at the mods installed in the armor they wear and in the items on their toolbelt and in their backpack. When it finds a guarded mod, it remembers the player and the mod. It does nothing else yet.
3. Your server manager checks whether the player is allowed to have VIP mods, for example by their role. For players who are not, it runs `evmg <player>` in the server console.
4. `evmg` runs the command in `VIPModGuard_DetectedCommand` for that player, once per server session.

Players whose admin permission level is at or below `VIPModGuard_ExcludeAdminLvl` are never flagged.

## Setup

### The item list

ServerCore creates an empty `VIPModGuardItems.txt` in the save game root (the `Saves` folder), next to `PrismaCoreSettings.xml`, on first start. Put one item name per line, exactly as the item is named in the game's `items.xml`:

```
modGunLaserSight
modArmorBandolier
```

ServerCore reloads the file as soon as you save it.

### Settings

Add these to `PrismaCoreSettings.xml`. See [Settings File](/ServerCore-7d2d/features/settings-file/) for what each one does.

```xml
<VIPModGuard_Enabled>true</VIPModGuard_Enabled>
<VIPModGuard_ExcludeAdminLvl>0</VIPModGuard_ExcludeAdminLvl>
<VIPModGuard_DetectedCommand>say "${playerName} has a VIP mod: ${vipMod}"</VIPModGuard_DetectedCommand>
```

`VIPModGuard_DetectedCommand` takes one console command, or several separated by `;`. These placeholders are replaced before a command runs:

| Placeholder | Replaced with |
|---|---|
| `${steamId}` | The player's platform ID |
| `${platformId}` | The player's platform ID |
| `${entityId}` | The player's entity ID |
| `${playerName}` | The player's name |
| `${vipMod}` | The guarded mod that was found |

## The evmg command

`pc-enforcevipmodguard`, with the aliases `enforcevipmodguard` and `evmg`.

| Usage | What it does |
|---|---|
| `evmg <entityId/name/steamId>` | If the player was flagged, runs `VIPModGuard_DetectedCommand` for them. It runs once per player until the server restarts or you use `evmg remove`. |
| `evmg list` | Lists the flagged players and the mod found on each. |
| `evmg remove <entityId>` | Clears the player from the flagged list, so they are checked from scratch. |

:::caution
`evmg` does not check who the player is. Only run it for players who are not allowed to have VIP mods, after your server manager has checked their role.
:::
