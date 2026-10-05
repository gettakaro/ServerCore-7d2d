# ServerCore for 7 Days to Die

ServerCore is a server-side mod for 7 Days to Die dedicated servers. It adds about 100 admin console commands, advanced land claims, region resets, chat control, teleports, vehicle recall and more.

**Documentation:** [gettakaro.github.io/ServerCore-7d2d](https://gettakaro.github.io/ServerCore-7d2d/) (source in [`docs/`](docs/))

ServerCore is the continuation of **PrismaCore**, the mod Prisma501 built and maintained for over ten years. It started as CPM, later became the CSMM Patrons Mod, and then PrismaCore. Prisma retired from 7D2D modding in September 2026 and handed the mod over to the community. It's now open source under the MIT licence.

ServerCore 3.1.0 is PrismaCore 2.5 as open source, updated for game version 3.3.0 b18. It adds VIP ModGuard and otherwise works like PrismaCore 2.5.

## Who maintains this

The [Takaro](https://takaro.io) team maintains ServerCore, but it's a community project first. Everything happens in the open: the code, the issues, the roadmap and the release process. Anyone can open an issue, propose a feature or send a pull request, and we want regular contributors to become maintainers.

ServerCore doesn't depend on Takaro. It works the same with Takaro, CSMM, any other server manager or plain console commands.

PrismaCore was closed source for its whole life. We think a mod this many servers depend on should be open, so that anyone can see how it works, fix it and keep it alive.

## Compatibility with PrismaCore

ServerCore 3.1.0 is a drop-in replacement for PrismaCore 2.5. Console commands (including the `pc-` forms), command output, log lines and data files like `PrismaCoreSettings.xml` work as before, so your settings, claims and server manager integrations carry over. See the [compatibility page](https://gettakaro.github.io/ServerCore-7d2d/project/compatibility/) for details.

## Supported game versions

| ServerCore | PrismaCore | Game version |
|---|---|---|
| 3.1.0 | – | 3.3.0 b18 |
| 3.1.0-exp.N (pre-release) | – | 3.3.0 EXP (experimental branch) |
| – | 2.5 (last PrismaCore release) | 3.2.0 b10 |

Pre-releases are built from the `experimental` branch for the game's experimental branch. See [RELEASING.md](RELEASING.md) for how releases are made.

## Installation

1. Stop your server.
2. If you run PrismaCore, remove `Mods/PrismaCore`. Don't run both mods at the same time: they register the same commands.
3. Download the latest `ServerCore-<version>.zip` from [Releases](https://github.com/gettakaro/ServerCore-7d2d/releases).
4. Extract it so you have `Mods/ServerCore/` in your server's install directory.
5. Start the server.

Moving from PrismaCore? See the [migration guide](https://gettakaro.github.io/ServerCore-7d2d/start-here/migrating-from-prismacore/).

### Development builds

To try unreleased changes, download the latest build of a branch. Each is replaced on every push, so don't run them in production:

- `main` (stable game version): [ServerCore-rolling-stable.zip](https://github.com/gettakaro/ServerCore-7d2d/releases/download/rolling-stable/ServerCore-rolling-stable.zip)
- `experimental` (experimental game version): [ServerCore-rolling-experimental.zip](https://github.com/gettakaro/ServerCore-7d2d/releases/download/rolling-experimental/ServerCore-rolling-experimental.zip)

Each pull request also gets a build, linked in a comment on the PR.

## Features

A short overview. Run `pc-help` in the server console for the full command list.

| Area | Highlights |
|---|---|
| Advanced claims | Jail, PvP arenas, hostile-free zones and reverse claims (`ccc`), `arrest` / `release`, claim friends, land claim tools |
| World resets | Reset chunks, regions, unclaimed regions and RWG prefabs, remove sleeper volumes |
| Builder tools | Copy, export, render, fill and replace blocks and prefabs, with undo |
| Blood moon | Spawner control (`bms*`), blood-moon-aware shutdown and timers, `isbloodmoon` |
| Spawning | Targeted hordes (`th`), scouts, multiple entities |
| Chat | Hide chat commands (`hccp`), mute, chat names and colours, group colours, messages with a custom sender (`say2`, `pm2`) |
| Teleports | Home (`teleh`), move to player, coordinates or waypoint (`mv`, `mvw`), offline moves, waypoints (`wpc`) |
| Vehicles | Recall your minibike, jeep, drone, gyrocopter, bicycle, motorcycle, blimp or helicopter, take ownership, check contents |
| Items | Give into the backpack (`giveplus`), remove items (`rii`), wipe inventories (`wi`), banned items |
| Players | Reset level, skill points or player data, set death count, permadeath, reserved slots, buffs and skills listing |
| Web | Map rendering and the ClaimCreator web dashboard |

## Contributing

Contributions are welcome, from bug reports to new features. Read [CONTRIBUTING.md](CONTRIBUTING.md) to get started. Issues labelled [`good first issue`](https://github.com/gettakaro/ServerCore-7d2d/labels/good%20first%20issue) are a good place to begin.

## Community and support

- **Questions and ideas**: [GitHub Discussions](https://github.com/gettakaro/ServerCore-7d2d/discussions)
- **Bugs and feature requests**: [GitHub Issues](https://github.com/gettakaro/ServerCore-7d2d/issues)
- **Chat**: the [Takaro Discord](https://aka.takaro.io/discord), where the PrismaCore community already lives
- **Security issues**: see [SECURITY.md](SECURITY.md)

## Licence

[MIT](LICENSE). Copyright Prisma501 and the ServerCore contributors.
