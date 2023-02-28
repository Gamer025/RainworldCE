using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RainWorldCE.Attributes;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a slugpup
    /// </summary>
    [MSCEvent]
    internal class Slugpup : CEEvent
    {
        public Slugpup()
        {
            _name = "The Mother";
            _description = "Protect him with your life";
        }

        public override void StartupTrigger()
        {
            AbstractCreature creature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(MoreSlugcats.MoreSlugcatsEnums.CreatureTemplateType.SlugNPC), null, EventHelpers.MainPlayer.pos, game.GetNewID());
            EventHelpers.MakeCreatureLikeAndFollowCreature(creature, EventHelpers.MainPlayer);
            creature.Realize();
            creature.realizedCreature.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);
            WriteLog(LogLevel.Debug, $"Spawned slugpup: {creature}");
        }

    }
}
