# Installation
Install the latest release, extract it then move the content to the game root directory. 
For linux add in steam launch options:
WINEDLLOVERRIDES="winhttp=n,b" %command%
Launch the game with the plugin once, then change the config in BepInEx/config/leaderboardpatch.cfg, here edit the username and flags.

# TODO
* Add the right medal for each leaderboard's entry

# Building
Follow [BepInEx: 1. Setting up the development environment](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html), then run "dotnet build". To use the plugin you'll need to generate the leaderboards and the flags, for that follow the Download Leaderboards and Flags section.

# Download Leaderboards and Flags
Download all the source code, then run the python file. Inside the folder Data/Leaderboards(Must be created manually!) you'll find the leaderboards for each level, and inside Data/Flags you'll find the flags.

