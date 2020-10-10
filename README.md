# RabbitLabirint

This repository contains the MVP of a 2D mobile game called RabbitLabirint. The game is a logic game.

## Objectives of the game

The player controls a rabbit, which must: 
- get to the finish line 
- not collide with enemies 
- on the way to the finish line, collect all the carrots
- use the shield to safely pass through the enemy

## Control
After loading the level, when you click anywhere on the map, except for UI objects, possible paths will be drawn along which the player can go in 1 move. 
After clicking again, the player will move to the nearest point that belongs to the set of points of the possible path and is closest to the click.

## Game settings

The game allows you to: 
- adjust the volume (general, music, effects) 
- delete the process of passing

## Levels

The game is divided into levels. 
Each subsequent level is accessed after passing the current one. 
For completing a level, stars from 1 to 3 are awarded. 
Levels can be replayed.

## Other
All graphics were drawn by [Melai](https://melai.ru)

Sounds are taken from free sources. 

The author of the music is listed on the main screen. 
Links to the author of the repository and the author of the music can be found in the About window (you can open it on the screen with the settings, the About button)

## About implementation

Finite state machines are used to control the game and the player. 
Finite state machines of the game has 3 states: loadout, game and game over.
The player's finite state machines have 7 states.

### Game Manager
The Game manager is a state machine, that will switch between state according to current gamestate. 

It has 3 states:
1. Loadout state - When starting the game. The main menu and settings opens.
2. Game state - Upon transition to this state, the level is loaded. It is in this state that the gameplay itself occurs.
3. Gameover state - The transition to this state is carried out when the player loses.

### Level Manager
The game takes place within 1 scene. Levels are managed by the Level Manager. Each level has its own prefab.

### PlayerController

PlayerController is a finite state machine.
It have 7 states:
1. Empty - The initial state when we do not need to display the player on the stage and control him. The player's sprite is not rendered.
2. Idle - The player's sprite is rendered. Waiting for a tap on the screen to draw possible paths for the player.
3. Waiting - The player's sprite is rendered. Draw possible paths for the player and wait for a tap to calculate the goal to which the player will move.
4. Moving - The player's sprite is rendered. Move the player to the selected target.
5. Attacked - The player's sprite is rendered. Player goes into this state when the player was attacked by the enemy.
6. Finishing - The player's sprite is rendered. Go to this state after completing the target movement. Checking the finish line and the number of steps remaining.
7. Finished - The player's sprite is rendered. The state of the player upon successful completion of the level.

![player's states](https://melai.ru/wp-content/uploads/2020/10/PlayerStates.jpg)

### PlayerData

Progress of passing and setting is saved locally in bin file. For this, the PlayerData class is used. 
Local storage is only used because it's an MVP and not a full-fledged game.
For publication it is worth using other types of data storage.

At the moment, the following data is saved: 
- version (used to control data reading) 
- volume (general, music, effects) 
- levels passed and their results 
- current level

When introducing additional fields for storage, it is important to respect the queue, as all fields are written in strict order. 
They are read from memory in the same order.

### Rule tile (Road rule tile)

To add roads to the level, sprites were drawn and then rule tiles were added with them.
![Road Rule Tiles](https://melai.ru/wp-content/uploads/2020/10/RuleTiles.jpg)

## Game screenshots
![StartScreen](https://melai.ru/wp-content/uploads/2020/10/StartScreen.jpg)
![Settings](https://melai.ru/wp-content/uploads/2020/10/SettingsScreen.jpg)
![Levels](https://melai.ru/wp-content/uploads/2020/10/Levels.jpg)
![Level](https://melai.ru/wp-content/uploads/2020/10/Level4.jpg)
![ChoiceWay](https://melai.ru/wp-content/uploads/2020/10/ChoiceWays.jpg)
![ResultLevel](https://melai.ru/wp-content/uploads/2020/10/ResultLevel.jpg)








