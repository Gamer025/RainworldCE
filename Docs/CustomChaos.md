## CustomChaos scripting

RainWorldCE 2.0.0 added a custom scripting backend allowign for custom event workflows. 
The system can be activated by placing a file CC.txt next to modinfo.json when the mod is installed.  
This will disable the random event selection mechanism and will cause CE to be fully script driven.  
This MD described the possible commands that can be used and how to use them.  

Available commands:
EventName [EventTime] - Triggers the specified event, you will need to use the internal name listed at the bottom of this file. You can optionally specify how long the event will be active  
WAIT Time - Will wait for the specified amount of time, if you don't add these to your script everything will be executed instantly! Required if you build a loop using GOTO  
GOTO Line - Jump back or forward to the specified line number. DON'T FORGET TO ADD WAIT COMMANDS IF YOU ARE BUILDING A LOOP  
RANDOM - Trigger a random event, respecting the active Chaos Edition settings  

#### FAQ:
In the Remix options one of the lines shows up as ERROR:  
You have entered an invalid command in that line (WAIT without time, Non existing event name etc.) This line will do nothing and you should fix it  

Internal event names to be used in scripts:  
CreatureMigration - Coming through  
CreatureRandomizer - DNA Mutations  
Darkness - Darkness / Shaded Region  
FoodLevel - Digestive reordering  
Friend - The Friend  
Gift - A small/normal/great gift  
Hiccup - Hiccup  
Hunger - Unending Hunger  
InvertControls - Directional confusion  
KarmaLevel - Karma shuffle  
LiftOff - Lift off  
LizardPack - The pack  
LowGravity - Low gravity  
MovementTime - Super Slugcat  
NoodleInvasion - Noodle invasion  
PaletteRandomizer - Too many mushrooms  
RainbowCat - Rainbow Cat  
RainyDay - Rainy day  
RandomLorePearl - A piece of history
Rifle - Free gun    
RoomConnectionShuffle - Geographic issues  
Rot - The Rot
ScavConvention - Annual scavenger convention  
ScavengerGang - Kill squad  
SecondChance - Second Chance  
SecretEvent - Secret event  
SpawnRedLizard - Danger lurks around  
SpearMaster - Spear Master  
Speed - Speeeeed  
SpitOutItem - Stomach ache  
Teleport - /tp  
Thanksgiving - Thanksgiving  
WaterRising - Water rising  
WeatherForecast - Weather Forecast  