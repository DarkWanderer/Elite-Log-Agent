# Elite-Log-Agent

A windows utility written in C#, presenting Json events from Elite: Dangerous log as data source. 
Allows updating external sites (and possibly do other actions) in near-real-time as the log gets updated

Currently in alpha stage

## Current Features

* EDSM support (most log events)
* Inara support (balance, travel, materials, engineer progress)

## Planned features

* Support for [Canonn API v2](https://github.com/derrickmehaffy/CAPI-V2)
* Google sheets for Powerplay/BGS

## How to install/use

* Download latest version (links below)
* Input commander name, Inara and EDSM API keys
* Save settings, launch the game.

## Get latest version

* ClickOnce application installer [here](https://elitelogagent.blob.core.windows.net/clickonce/EliteLogAgent.application)
* Latest (pre-)release merged .exe [here](https://github.com/DarkWanderer/Elite-Log-Agent/releases)

## Contributions

You're welcome to contribute by

## Build status

[![appveyor build status][image]][project]

## Links

* [Elite: Dangerous in official store](https://www.frontierstore.net/games/elite-dangerous-cat.html)
* [INARA](https://inara.cz)

[image]: https://ci.appveyor.com/api/projects/status/6n52i9wkthtwtb34/branch/master
[project]: https://ci.appveyor.com/project/DarkWanderer/Elite-Log-Agent
