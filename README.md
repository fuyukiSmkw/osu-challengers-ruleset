# osu-challengers-ruleset

An osu!lazer custom ruleset that integrates quick access to osu!Challengers playlists and stats directly into the game.

## Install

Go to Releases, download the dll file, and place it in the `rulesets` subfolder of your osu!lazer directory.

* On Windows, this is usually located at `%APPDATA%\osu\rulesets`
* On Linux, it's usually at `~/.local/share/osu/rulesets`

## Build

Requires .NET `9.0` or higher.

```bash
dotnet build -c Release
```

Remove `-c Release` during development.

## Acknowledgements

* [ppy/osu](https://github.com/ppy/osu): The official osu!lazer project
* [MATRIX-feather/LLin](https://github.com/MATRIX-feather/LLin): A custom ruleset that provides beatmap downloads from a mirror site and includes a music player
* [EVAST9919/lazer-sandbox](https://github.com/EVAST9919/lazer-sandbox): A custom ruleset
