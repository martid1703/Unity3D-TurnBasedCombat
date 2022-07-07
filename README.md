# 2D turn based battle system
## This is a learning project implementing 2d turn based battle made in Unity
Unit images/animations for miner unit are properties of Unfrozen Studio, unit images/animations for paladin unit are properties of the Spine example pack (Esoteric software).
### Game executables are located in Build.zip

<img src="README-screenshot.png" width="100%" height="100%">


## Controls:
* Hit `Escape` key to show pause menu. From there you can either unpause, quit or restart the game.
* All other controls are gui

## What is implemented:
* turn based combat system

    There are two players named "Player1" and "Player2". They can either attack or skip a turn.
Players can be controlled by Human or by AI, which is set with the respecitve "IsHuman" button.
> At the moment AI is not implemented and is choosing attack or skip randomly.
* Unit visuals

    Units have HP and Initiative bar. Later is used at the start of the game to form initial battlequeue.
    Also on mouse over the unit some unit data is displayed above: unit type, hp, damage, inititive
    Unit selection is done with mouse. When unit is selected as target the yellow circle beneath is becomes red.
    When unit is attacker his selection circle is blue.
* Battle queue visual representation

    At the top is displayed battle queue visual representation for the first 8 units in the queue. Other units in the queue are displayed when some units take turn and move to the end of the queue or die and visual slots become available.
* Increment/Decrement number of units in runtime

    For each player increment(+)/decrement(+) buttons are available, which allow to control number of units at runtime.
Units are added at the end of the battlequeue. Units count cannot go below 1, attacking unit can not be deleted this way.
* BattleSpeed change

    BattleSpeed slider allows to change animation and movement speed at runtime from 0 to 5. Default is 2.

## Known issues:
* Sounds for unit animations do not work well with spine animation events. May be because animations events are incorrect. This is why there's no sound for miner unit movement, and attack/damage sounds are called manually from controller.
*  There's no proper run animation for miner unit in spine asset, so it uses some other animation instead.