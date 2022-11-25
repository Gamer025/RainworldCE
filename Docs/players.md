# RainworldCE Players Guide
This readme contains several kinds of information about RainworldCEs config options and potential issues.
## ConfigMachine options
### Main
**Duration between events**  
Determines how often events will generate in seconds.  
Default: 60 seconds  

**Don't repeat events for (%)**  
Did you know that pure randomness can be pretty repetitive? This prevents that.  
Whenever a events gets choosen it will be blocked for X other events.  
This slider determines that value, if you have 10 events enabled and set the percentage to 50% then an event would be blocked for 5 guranteed other events.  
Default: 50%
  
**Max events per cycle**  
Determines how many events should trigger per ingame cycle.  
Pretty useful if you for example only want to trigger one event at the start of the game.  
Default: 200 events  

**Event duration multiplier (base is 10)**  
Ok this one requires a bit of math.  
By default most active events will last for 2 minutes. This options lets you increase/decrease that time for some events.  
If you set the slider to 10 all events will have their default length.  
If you set it to 1 all events would be 10 times shorter
If you were to set it to 50 all events would be 5 times longer.  
Default: 10  

**Show active events**  
Determines if active events (events with a timer) will be displayed in the bottom left.  
Default: Yes

### Events
This tab allows you to enable and disable certain events.  
Want the game to spawn a red lizard in a random room every 10 seconds? You can do that  
Want the game to be constantly in "Time only moves when you move" mode? You can do that  
Feel free to mix and match events however you want.

### Event Config
Some events implement custom options thats allows you to customize them even further  
Check the hover text to find out more about the specific options

## Known issues / FAQ
#### Why am I getting events called "Nothing"
This means you managed to make RainworldCE run out of events too choose from, congratulations.  
On a more serious note: This most likely means you have only one or a few time based (active events) enabled.  
Most of these only support one being active at any time, so if all your enabled events are already active you get "Nothing".
#### I set the don't repeat option to X but I still got a event that should have been blocked
This most likely means you ran in the previously described scenario (all active events are already active).  
To prevent dissapointment instead of getting "Nothing" the game will then allow for a normaly blocked event to trigger.
#### I can't find the options menu
Make sure ConfigMachine is installed
#### Something broke
If you think you found a bug or any other kind of mistake in the game please report them [here](https://github.com/Gamer025/RainworldCE/issues).  
If possible try to include a description of what you did to trigger the bug.  
Log files and your ConfigMachine config file would also be greatly apprciated.
#### I have a cool idea for an event
If you are familiar with C# and maybe even Rain World modding I heavily recommend reading through the event creation guide here: [Events.md](/Docs/events.md)  
Adding a event is quite easy and only requires knowledge about where to find / do stuff in the games code 
Otherwise feel free to open a post in [the ideas discussion board](https://github.com/Gamer025/RainworldCE/discussions/categories/ideas) 