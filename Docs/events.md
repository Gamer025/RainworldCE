# RainworldCE Event Creation Guide
So you want to add your own event to RainworldCE?  
This guide will teach you the basics of what is needed for creating your own events.  
RainworldCE has been designed to allow for as easy as possible event creation and all event related code is located in the Events folder.

### Basics
All events inherit from the CEEvent class, located in the CEEEvents.cs file in the Events folder.  
All events should be located in the Events folder and RainworldCE.Events namespace.  
An event consists of certain fields/properties that describe it and its behavior. By overriding trigger methods an event can react to predefined situations.  
You will want to set the necessary fields in your events constructor and then override whichever methods/triggers to which you want to react to.  
Don't perform any game related actions in your constructor, there are multiple cases where instances of your event will be created while the game is not active. Use `StartupTrigger` instead.  
Otherwise use the storyModeActive EventHelper to check if the game is active if you need to get/generate data on event construction. However make sure to not modify the games state as construction doesn't necessarily mean your even it active!  
The EventHelpers class contains multiple useful properties and methods that you can use in your events.  
Use the `game` field to access RainworldGame, which contains stuff like the world etc.

### Event fields
This table lists all fields and how they influence your event.
| Name | Explanation | Required |
|------|-------------|----------|
|Name|The name of your event.</br> Displayed as big text during event selection and as small text in the bottom left while your event is active.|Yes|
|Description|The description of your event.<br> Displayed as small text below the name when your event gets selected|Yes|
|activeTime|Determines for how many seconds your event will be active for.<br> You will want to set this field if you are creating an event that reacts to triggers like the player changing the room or if you are using repeatEverySec<br> **Tip:** Use code like `(int)(60 * RainWorldCE.eventDurationMult)` if you want your duration to be affected by the event time multiplier setting.|No, disabled by default|
|RepeatEverySec|How often the `RecurringTrigger` method will be invoked in seconds.</br> You should set this field if you intend to use the `RecurringTrigger` method.|No|
|AllowMultiple|Determines if multiple instances of an event can be active at the same time.</br> Use this for events with an `ActiveTime` set if you want to allow the event to be active multiple times.|No, false by default|
|ConfigEntries|Override this properties get method to implement custom config options for you event.<br> See the "Custom event configs" section for more help|No|

### Event methods
This table list all overridable trigger methods that you event can use.
| Name | Explanation |
|------|-------------|
|StartupTrigger|Instantly executed when your event got selected.</br> Use this for one off events or to setup your active event.|
|ShutdownTrigger|Executed when your event expired or the cycle has ended.<br> Use this method for cleaning up the mess your event left behind (in case you want to do that).|
|RecurringTrigger|Method that gets triggered every X seconds.<br> The time between triggers is determined by the `RepeatEverySec` field.|
|PlayerChangingRoomTrigger|This method gets triggered when the player is about to change room.</br> To be exact it triggers before ShortcutHandlers SuckInCreature method.<br> Use the supplied `ShortcutData` to determine to which room the player is trying to move to.|
|PlayerChangedRoomTrigger|This method gets triggered when the player has changed room and the new room has already been realized.</br> To be exact it triggers after RoomCameras ChangeRoom method.<br> `Room` will be the realized room the player has moved to.|

### Custom event configs
RainworldCE comes with a system that allows for easily adding ConfigMenu entries for your event.  
This allows players do configure your event in case they have CM installed.  
Currently bools (checkboxes) and int ranges (sliders) are supported.

To add custom configs to your event you will want to override the get method of the ConfigEntries property of your event.  
You will want to return a collection of EventConfigEntry with each one describing an option for your event.  
Use BooleanConfigEntry for checkboxes and IntegerConfigEntry for sliders.  
Each config entry must have an unique key in your event.  
Use the `GetConfig` method included in your CEEvent to retrieve a specified key. You will need to cast the result to the wanted type.  
If a key is not prestent (for example because CM isn't loaded) null will be returned.  
You can also use `TryGetConfigAsBool` and `TryGetConfigAsInt` to get an already converted value for a key.  
The bool method will return false if the key is not present, the int method will return 0.

### Tips and tricks
In case you ever get lost and aren't sure how to create an event it's probably best to check out an already existing event that's closest to your event.  
This will not only tell how an event is generally structured and created but also give you some insights on how to perform certain actions with Rainworlds code.  
You might also want to check out EventHelpers.cs for useful helper methods that allow you to do certain things like getting the player's creature, current room etc.