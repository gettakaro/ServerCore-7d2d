---
title: Compatibility
description: How ServerCore relates to PrismaCore 2.5, and which game versions are supported.
---

## Coming from PrismaCore

ServerCore 3.1.0 is a drop-in replacement for PrismaCore 2.5. The mod has a new name, but the parts your server and tools rely on work as before:

- **Console commands**: the same names and aliases, including the `pc-` prefixed forms
- **Command output**: the same text, so server managers and community modules that parse it keep working
- **Log lines**: including the `[PrismaCore]` prefixed lines that CSMM, Takaro and community modules read
- **Data and config files**: `PrismaCoreSettings.xml`, `PrismaCoreStrings.xml` and the `PrismaCore*.db` databases keep their names and formats, so your claims, waypoints and settings carry over

When a release changes any of these, it's a new major version, and the [changelog](/ServerCore-7d2d/project/changelog/) explains what to update.

## Supported game versions

| ServerCore | PrismaCore | Game version |
|---|---|---|
| 3.1.0 | – | 3.3.0 b18 |
| 3.1.0-exp.N (pre-release) | – | 3.3.0 EXP (experimental branch) |
| – | 2.5 (last PrismaCore release) | 3.2.0 b10 |

Pre-releases are builds for the game's experimental branch. Use them on test servers.
