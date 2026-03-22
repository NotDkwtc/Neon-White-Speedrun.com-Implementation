# Installation
Install the latest release, extract it then move the content to the game root directory. 
For linux add in steam launch options:
WINEDLLOVERRIDES="winhttp=n,b" %command%

# TODO
* Add the user's country flag as his steam image
* Add player record in leaderboard
* Add the right medal for each leaderboard's entry

# Building
Follow [BepInEx: 1. Setting up the development environment](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/1_setup.html), then run "dotnet build". To use the plugin you'll need to generate the leaderboards, for that follow the Download Leaderboards section.

# Known Issues
* "My Score" button doesn't work.

# Download Leaderboards
Download all the source code, then run the python file. Inside the folder Data/Leaderboards(Must be created manually!) you'll find the leaderboards for each level
