# AmmoTweaks
Configurable patcher intended as a WACCF-friendly alternative to ABT.
Will rescale ammo damage removing ammo weight as well as adjusting projectile speed and gravity.
Can optionally rename ammo for better sorting and adjust recoverable ammo found on bodies.

## Settings
Configuration file (Data/config.json)
- tweakNonPlayable: Apply tweaks to enemy-only (non-playable) ammunation (such as Dwarven Sphere's bolts)
  - TODO: use instead - list of NonPlayable to tweak damage only
- damageRescaling:  Enables projectile damage tweaks. Set to false to disable.
- damageMult: Modifies damage.  Values below 1 will reduce damage while values above 1 will increase it.  
- minDamage: Allowed minimum damage. Set negative number to disable.
- maxDamage: Allowed maximum damage. Set negative number to disable.
- lootMult: Modifies amount of ammo found on bodies. Values below 1 will reduce found ammo while values above 1 will increase it. Set to 1 to disable.
- renaming: Used to rename ammunition using the following scheme: "Iron Arrow" -> "Arrow - Iron". Set to true to enable.
- separator: Separator used if renaming is enabled. For example " - " and  ": "
- speedchanges: Enables projectile speed and gravity tweaks. Set to false to disable.
- speedarrow: new arrow speed.
- speedbolt: new bolt speed.
- gravity: new gravity value for both arrows and bolts.
