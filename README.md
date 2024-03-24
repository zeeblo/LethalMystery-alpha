# Gameplay
https://youtu.be/6Ui0rqmn6CA

Note: I removed the Skeld map and Among Us sound effects since those are directly under their IP.
Furthermore this will not be kept updated, once v50 of Lethal Company is released, it is possible the mod will stop working.

# Start

If you don't have BepInEx then follow this tutorial https://www.youtube.com/watch?v=eXA60ZWMI4M and ONLY watch up to 2:20. You don't need to watch the rest. (You can but the instructions given after aren't necessary)


## Now that you have BepInEx

### Click on the Releases section

<img src=".\Assets\releases.png" alt="release screenshot 1">

### Download the zip

<img src=".\Assets\releases2.png" alt="release screenshot 2">

### Go to your plugins folder

<img src=".\Assets\extractHere.png" alt="extractHere">

- Unpack the zip
- Run Lethal Company
- You're set!



# **IMPORTANT**

Once you host a game, if anyone leaves the lobby at any point in time, the mod wont work correctly. **Everyone** has to restart the game if 1 person leaves. Even if a match has not started.




# FAQ

## How do we start the game?
The game only starts when you (the host) enter this command in the terminal:

start (gamemode) (muted/unmuted)

eg. `start normal unmuted`

ONLY DO THIS ONCE YOU'VE PULLED THE LEVER

### Gamemodes

normal - A weapon is spawned under you that can 1 tap employees. (Most stable version)

monster - There can only be 1 imposter in this gamemode and they have the ability to 
shapeshift into a monster. (unstable version)

### Muted/Unmuted

Muted - Proximity chat is turned off unless a meeting is called or the game ends.
(this prevents dead people from talking as well, not intentionally).

Unmuted - Proximity chat is enabled throughout the game (with the exception of the grace period)

**Important:** Once the game ends, make sure that if you're the host, the FIRST thing that
you do is type "stop" in the terminal.


## We can't afford to go to expensive planets
If you're the host type scoins (num).

eg. `scoins 999`

## The game started but no one has their suits
The game only starts when you enter this command in the terminal:

start (gamemode) (muted/unmuted)
eg. `start normal unmuted`

If you've already done this then type "`suits`" in the terminal


## I'm the imposter but I don't have my weapon
If the gamemode is set to "monster" then you wont have a weapon and should
instead press "8" to transform. 

If the gamemode is "normal"

In the terminal type "spweapon". This will spawn
a yieldsign below you that will 1 tap anyone.

If this doesn't spawn anything, you can buy a
regular shovel which will still 1 tap anyone (only if ur imposter)


## The scraps from last game are still in the ship

Once the ship is in space, remember to type "eject" in the terminal. It will prompt
you with a message asking you if you want to continue, type "c".
This will flush the ship of previous items
