![ELA logo](EliteLogAgent/Resources/elite-dangerous-icon.ico)

:warning::warning::warning:PROJECT SUSPENDED:warning::warning::warning:

The current tragic and unforgivable events are sending waves of impact throughout the world. Unfortunately, for this project, the ripple means that there is no possiblity to pay for hosting of ClickOnce update files - which are at the heart of fast & seamless updates ELA was providing. This may change in future - however, at the moment I have no choice but to recommend to move to alternative software

Thank you for everyone who used and helped to improve this software

# Elite Log Agent

A Windows utility, sitting in tray. Feeds your commander data to INARA and/or EDSM sites. Also helps other players by sharing trade/commodity/outfitting data via EDDN

Key features:
* Fills your player profile on INARA and/or EDSM 
* Sends station data to community sites like EDDB (commodity, shipyard, outfitting)
* Supports multiple accounts
* Does not require authenticating via Frontier API site

## Quickstart

* Download [ClickOnce installer][clickonce] or [portable version][release-latest]
* Input commander name(s) and api key(s) for Inara and/or EDSM
* Save settings (press 'Apply' and/or 'OK')
* Launch the game.

## Plugin development

At this time, I see the best way of plugin support as incorporating them into this codebase. This will most likely change once more stable version is reached.
To contribute to development, please fork the repository and raise PRs

## Contributions

You're welcome to contribute by:

1. Using the application!
2. Raising [issues](https://github.com/DarkWanderer/Elite-Log-Agent/issues) on GitHub
3. Proposing pull request with changes and/or new functionality, including plugins

## SDLC

Builds are done in AppVeyor. `master` branch is the primary integration branch ('potentially releasable').
Releasing to ClickOnce installer and github  is done via merging to `prod` branch

| Item          | Status  |
| ------------- | ------------: |
| master   | [![appveyor build status][buildstatus-master]][project] |
| prod     | [![appveyor build status][buildstatus-prod]][project]   |
| coverage | ![coverage badge][codecov-badge]
| release  | ![release badge][release-badge]

## Links

* [Elite: Dangerous in official store](https://www.frontierstore.net/games/elite-dangerous-cat.html)
* [INARA](https://inara.cz)
* [EDSM](https://edsm.net)

[buildstatus-master]: https://ci.appveyor.com/api/projects/status/6n52i9wkthtwtb34/branch/master
[buildstatus-prod]: https://ci.appveyor.com/api/projects/status/6n52i9wkthtwtb34/branch/prod
[project]: https://ci.appveyor.com/project/DarkWanderer/Elite-Log-Agent
[clickonce]: https://elitelogagent.blob.core.windows.net/clickonce/EliteLogAgent.application
[release-latest]: https://github.com/DarkWanderer/Elite-Log-Agent/releases/latest
[releases]: https://github.com/DarkWanderer/Elite-Log-Agent/releases
[codecov-badge]: https://codecov.io/gh/DarkWanderer/Elite-Log-Agent/branch/master/graph/badge.svg
[release-badge]: https://img.shields.io/github/release/DarkWanderer/Elite-Log-Agent.svg
