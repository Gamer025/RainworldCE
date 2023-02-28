using BepInEx.Logging;
using RainWorldCE.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Replaces all creatures in the current room with DLLs, if non found spawns one instead
    /// </summary>
    internal class Rot : CEEvent
    {
        public Rot()
        {
            _name = "The Rot";
            _description = "Eventually everything will be overtaken by it";
            _activeTime = (int)(60 * RainWorldCE.eventDurationMult);
            _repeatEverySec = 5;
        }

        int chance = 100;
        Dictionary<AbstractCreature, AbstractCreature> restore = new Dictionary<AbstractCreature, AbstractCreature>();

        public override void RecurringTrigger()
        {
            if (rnd.Next(100) < chance)
            {

                List<AbstractCreature> oldCreatures = EventHelpers.CurrentRoom.creatures.Where(x => 
                    x.creatureTemplate.type != CreatureTemplate.Type.Slugcat &&
                    x.creatureTemplate.type != CreatureTemplate.Type.Overseer &&
                    x.creatureTemplate.type != CreatureTemplate.Type.DaddyLongLegs &&
                    x.creatureTemplate.type != CreatureTemplate.Type.Fly)
                    .ToList();
                if (TryGetConfigAsBool("excludePups"))
                    oldCreatures = oldCreatures.Where(x => x.creatureTemplate.type != MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC).ToList();
                AbstractCreature oldCreature = oldCreatures.FirstOrDefault();

                if (oldCreature is null)
                {
                    //No DLL has yet been spawned, create a new one
                    if (chance == 100)
                    {
                        WriteLog(LogLevel.Debug, "Found no creatures to replace with DLL, spawning one");
                        AbstractCreature DLL = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.DaddyLongLegs), null, new WorldCoordinate(game.world.offScreenDen.index, -1, -1, 0), game.GetNewID());
                        DLL.ChangeRooms(EventHelpers.MainPlayer.pos);
                        if (TryGetConfigAsBool("restoreCreatures"))
                        {
                            restore.Add(DLL, null);
                        }
                    }
                    chance = 0;
                    return;
                }

                WriteLog(LogLevel.Debug, $"Found {oldCreature} , pos: {oldCreature.pos}");
                AbstractCreature newCreature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.DaddyLongLegs), null, oldCreature.pos, game.GetNewID());
                WriteLog(LogLevel.Debug, $"Replace creature with {newCreature}");
                EventHelpers.CurrentRoom.AddEntity(newCreature);
                newCreature.Realize();
                newCreature.realizedCreature.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);

                if (TryGetConfigAsBool("restoreCreatures"))
                {
                    oldCreature.realizedCreature.room = null;
                    oldCreature.Abstractize(oldCreature.pos);
                    oldCreature.Room.RemoveEntity(oldCreature.ID);
                    if (oldCreature is not null && !oldCreature.slatedForDeletion)
                    {
                        restore.Add(newCreature, oldCreature);
                    }
                }
                else
                    oldCreature.realizedCreature.Destroy();
                chance -= 30;

            }
            else
            {
                WriteLog(LogLevel.Debug, $"Chance didn't trigger, disabling spawning of new DLLs");
                chance = 0;
            }
        }

        public override void ShutdownTrigger()
        {
            if (TryGetConfigAsBool("restoreCreatures"))
            {
                WriteLog(LogLevel.Debug, "Restoring creatures...");
                foreach (KeyValuePair<AbstractCreature, AbstractCreature> kvp in restore)
                {
                    //If value (orig creature) is null just delete the replacement
                    if (kvp.Value is null)
                    {
                        if (kvp.Key.realizedCreature is not null)
                        {
                            kvp.Key.realizedCreature.Destroy();
                        }
                        else
                        {
                            kvp.Key.Room.RemoveEntity(kvp.Key);
                            kvp.Key.Destroy();
                        }
                        return;
                    }

                    //Otherwise restoer the orig creature and delete replacemnet
                    WriteLog(LogLevel.Debug, $"Restoring {kvp.Value} with {kvp.Key} as randomized creature");
                    kvp.Value.pos = kvp.Key.pos;
                    kvp.Key.Room.AddEntity(kvp.Value);
                    if (kvp.Key.realizedCreature is not null)
                    {
                        if (kvp.Key.realizedCreature.room is not null)
                        {
                            kvp.Value.Realize();
                            kvp.Value.realizedCreature.PlaceInRoom(kvp.Key.realizedCreature.room);
                        }
                        else
                        {
                            WriteLog(LogLevel.Debug, $"Realized creature room null, try abstract: {kvp.Key.Room.realizedRoom}");
                            kvp.Value.Realize();
                            kvp.Value.realizedCreature.PlaceInRoom(kvp.Key.Room.realizedRoom);
                        }
                        kvp.Key.realizedCreature.Destroy();
                    }
                    else
                        kvp.Key.Room.RemoveEntity(kvp.Key);
                    kvp.Key.Destroy();
                }
            }
        }
        public override List<EventConfigEntry> ConfigEntries
        {
            get
            {
                List<EventConfigEntry> options = new List<EventConfigEntry>
                {
                    new BooleanConfigEntry("Restore creatures?", "Restore the original creatures at the end of the event?", "restoreCreatures", false, this),
                    new BooleanConfigEntry("Exclude slugpups?", "Prevent slugpups from being rotten?", "excludePups", true, this)
                };
                return options;
            }
        }
    }
}
