# Changelog

All notable changes to ServerCore are documented here. The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/), and the project uses [semantic versioning](https://semver.org).

## [Unreleased]

## [3.1.0]

The first ServerCore release: PrismaCore 2.5 as open source, for game version 3.3.0 b18. Game version 3.2 is no longer supported; stay on PrismaCore 2.5 for it.

### Changed

- Renamed PrismaCore to ServerCore and released it as open source under the MIT licence. Console commands, aliases, command output, log lines and data files are unchanged.
- `cvc` (check vehicle content), `rii` (remove an item from a player), `wi` (wipe a player's inventory) and the RegionReset banned-item check now use the player save and container format of game version 3.3.
- Kills made while riding a vehicle are logged with `meleeHandPlayer` as the weapon, because game version 3.3 no longer swaps the inventory when a player gets on a vehicle.
- New installs now send server chat messages as "Lara", a nod to Prisma501, the author of PrismaCore. Existing servers keep their saved name; change it with `scn <name>`.

### Added

- VIP ModGuard: flags players who have a listed item mod, such as a donor mod, installed in their armor, toolbelt or backpack items, and runs a configured command for them with `evmg`. Off by default. Written by Prisma501 for PRG, who shared it. See [VIP ModGuard](https://gettakaro.github.io/ServerCore-7d2d/features/vip-modguard/).
- The ClaimCreator web UI (v2.2.1) ships in the release zip, with Steam login certificates that match the chain Steam serves today. Thanks to Prisma501 for the drone icon fix.
- `Config/buffs.xml` with the tooltip buffs ships in the mod folder, as it did with PrismaCore.
- Documentation site at [gettakaro.github.io/ServerCore-7d2d](https://gettakaro.github.io/ServerCore-7d2d/).

### Fixed

- Other mods, such as the Takaro connector, now receive player deaths, kills and leaves, and the server log again shows the game's `GMSG: Player '…' died` and `left the game` lines. PrismaCore claimed every game message and stopped both. Turning a message off with a `GMSG_*_Enabled` setting still hides it.

## [2.5] - PrismaCore

The last release of PrismaCore by Prisma501, for game version 3.2.0 b10. This is the baseline ServerCore continues from.

[Unreleased]: https://github.com/gettakaro/ServerCore-7d2d/commits/main
[2.5]: https://gettakaro.github.io/ServerCore-7d2d/project/changelog/#prismacore-version-history
