# Elite-Log-Agent

A windows utility written in C#, presenting Json events from Elite: Dangerous log as data source. 
Allows updating external sites (and possibly do other actions) in near-real-time as the log gets updated

Currently in alpha stage

## Quickstart: how to install/use

* Download latest version (links below)
* Input commander name, Inara and EDSM API keys, click 'verify' to check info is correct
* Save settings (apply and/or OK)
* (optional) Upload last 5 logs using relevant button
* launch the game.

## Current Features

* EDSM support (most log events)
* Inara support (balance, travel, materials, engineer progress)

## Planned features

* Support for [Canonn API v2](https://github.com/derrickmehaffy/CAPI-V2)
* Google sheets for Powerplay/BGS

## Get latest version

* ClickOnce application installer [here](https://elitelogagent.blob.core.windows.net/clickonce/EliteLogAgent.application)
* Latest (pre-)release merged .exe [here](https://github.com/DarkWanderer/Elite-Log-Agent/releases)

## Plugin development

The code base has not yet reached maturity, so any (even ostensibly public) APIs might change. Hence, at this time, I see the best way of plugin support as incorporating them into this codebase. This will most likely change once a stable version is reached

## Contributions

You're welcome to contribute by

1. Raising [issues](https://github.com/DarkWanderer/Elite-Log-Agent/issues) on GitHub
2. Using the application!
3. Proposing pull request with changes and/or new functionality, including plugins

## SDLC

Builds are done in AppVeyor. `master` branch is the primary integration branch ('potentially releasable').
GitHub pre-releases are published from `master`
Releasing to ClickOnce installer is done via merging to `prod` branch

| Branch        | Build status  |
| ------------- | ------------: |
| master        | [![appveyor build status][buildstatus-master]][project] |
| prod          | [![appveyor build status][buildstatus-prod]][project]   |

## Links

* [Elite: Dangerous in official store](https://www.frontierstore.net/games/elite-dangerous-cat.html)
* [INARA](https://inara.cz)

[buildstatus-master]: https://ci.appveyor.com/api/projects/status/6n52i9wkthtwtb34/branch/master
[buildstatus-prod]: https://ci.appveyor.com/api/projects/status/6n52i9wkthtwtb34/branch/prod
[project]: https://ci.appveyor.com/project/DarkWanderer/Elite-Log-Agent
