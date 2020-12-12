# AmmoTweaks
Configurable patcher intended as a WACCF-friendly alternative to ABT.
Will rescale ammo damage between a configurable minimum and maximum while removing ammo weight.
Can optionally rename ammo for better sorting and adjust recoverable ammo found on bodies.

## Settings
Configuration file (Data/config.json)
- rescaling: Enables new damage scaling. Set to false to disable.
- minDamage: Allowed minimum damage.
- maxDamage: Allowed maximum damage.
- lootMult: Modifies amount of ammo found on bodies. Values below 1 will reduce found ammo while values above 1 will increase it. Set to 1 to disable.
- renaming: Used to rename ammunition using the following scheme: "Iron Arrow" -> "Arrow - Iron". Set to true to enable.
- separator: Separator used if renaming is enabled. For example " - " and  ": "
