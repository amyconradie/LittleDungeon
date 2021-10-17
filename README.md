# Little Dungeon
Simple Unity Roguelike game with a procedurally generated maze. 
- Procedurally Generates and Braids a Maze of user-defined size. 
- AI Enemies controlled by a Finite State Machine.
- Uses A* Pathfinding Project for AI Movement: https://arongranberg.com/astar/
- Sound effects obtained from: https://www.zapsplat.com

## Game Mechanics

#### Game Goal: 
- Get to the Stairs

#### Controls: 
- Use WASD or Arrow keys to move 
- Press E to Interact with items
- Esc to open Pause Menu

#### All Character Stats:
- All characters have X number of Hearts
- All characters have a HungerBar 
- The HungerBar automatically goes down over time
- When character's die they will spawn a steak

#### Enemy Behaviours:
- Roaming
- Chasing the Player
- Looking for Food
- Looking for Health Potion

#### Consumables:
- Steaks reduce character's hunger by 100%
- Eat Mushrooms to decrease hunger by 2
- Drink health potions to restore all hearts
- If HungerBar is fully restored, a heart will be restored too

#### Audio Clips
- Metal Clink Sound whenever buttons are clicked
- Bite Sound whenever enemy collides with another enemy or the player
- Bouncy Noise whenever consumables are consumed
- Victory Sound when stairs have been reached
- Roblox Death Sound on character deaths

## Enemy AI
### Finite State machine
#### States
- Roaming
- ChasingPlayer
- Hurt
- Hungry

### FSM
#### Roaming:
- If (the enemy is stuck in place): A* Target = new random position
- If (the enemy's Health is less than full): state = hurt
- If (the enemy's HungerBar is less than or equal to 75%): state = hungry (is this fuzzy logic?)
- If (the player is within range): state = ChasePlayer
- Else: A* Target = new random position
#### ChasingPlayer:
- If (the player is out of range): state = roaming
- If (the player is dead): state = roaming
- Else: keep following the player
#### Hurt:
- If (there are no more potions): state = roaming
- Else: A* Target = Nearest Potion
#### Hungry:
- If (there are no more mushrooms or steaks): state = roaming
- Else: A* Target = Nearest Mushroom/Steak

#### Some issues with the AI
- AI are not aware of each other, so they keep getting stuck on each other and they only pseudo-attack when they chase.
- AI tends to get stuck on other AI, solved for the Roaming state (if stuck in the same place for too long, select a new random position in the maze), but not resolved for the other states. 
- Something weird happens with the Hurt state, AI needs to be in the Hurt state (have less than full health) to be able to target healing potions, but the AI can still sometimes pick up potions when it's at full health.
- On the player's death the camera is supposed to follow the first living AI inside the "Enemies" GameObject, and then there if there are no living enemies, the camera will zoom out to show the entire maze. But sometimes the camera won't focus on the last living AI, it will still zoom out when they're all dead though.

## Maze Generation
Get user-defined maze dimensions, dimensions have to be an odd number and any dimensions less than 11 will break the maze. So to avoid breaking the maze, any dimensions less than 11 will just return an empty room with walls on the outskirts.
#### 1. Recursive Maze Generation Function. (MazeGenerator class)
Create a 2D int maze.
- empty = 0
- wall = 1
- enemy = 2
- mushroom = 3
- potions = 4
- stairs = 5

1.5. Make a 3x3 tile clearing in the centre of the maze. The player will always spawn in the centre, and this makes it so that no enemies can immediately kill the player.

#### 2. Remove Deadends by Braiding the Maze. (MazeGenerator class)
Count the dead ends in the int maze and Braid the maze, (remove the dead ends). Removing dead ends will help prevent the player from being cornered by any single enemy (multiple enemies can still corner the player).

#### 3. Set Up Enemy and Item Spawn Points. (MazeGenerator class)
Look for open spaces in the int maze, and randomly select an open spot to spawn each element.
- Spawn Enemies
- Spawn Mushrooms
- Spawn Potions
- Spawn Goal (Stairs)

#### 4. Instantiate GameObjects according to their int maze value. (MazeVisual class)
Randomly instantiate floor and wall tile prefabs to make the maze look more interesting. Store all the enemies and consumables in lists for access in the collision class.

## Other Stuff

### Setting up the project in Unity
- In the Game Tab: make sure the aspect ratio is set to 16:9, and MaximiseOnPlay is on.
- To see where each enemy's A* Pathfinding Seeker is targeting, enable Gizmos and make sure 'seeker' is visible under the Gizmos checklist