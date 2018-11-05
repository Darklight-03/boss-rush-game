# 315-game-team10

web address: teamproject1.ddns.net:3000

## Requirements

To play the game, all that you need is a browser capable of running a unity game. This should work on all major browsers.  
To host the game on your own webserver, you need:  
- nodejs
- install all packages with npm install
- should work on any operating system

## Compilation

To build the unity game:
1. open `unity-project` in unity
2. press `ctrl+shift+b`
3. click `webgl`


![image](https://puu.sh/BWBr9/4d4b2dac66.png)


4. click build
5. choose where to save it
6. click `select folder`
7. wait long time


To build web server:
1. navigate to the `website` directory
2. run `npm install`
3. place your unity build in the `unity-build` folder
4. run `node app.js`
5. now you can navigate to `localhost:3000` to play the game.

## Structure

unity project is in `unity-project/`
website is in `website/`
code is in `unity-project/Assets/Scripts`
resources are in `unity-project/Assets/Scenes` in various subfolders.


## Authors
Bo Corman `bdc3142@tamu.edu`  
Jiahao Zhao `zhao626008414@tamu.edu`  
Walter Stager `wts923@tamu.edu`  
Wilson Kahlich `w_kahlich@tamu.edu`  
