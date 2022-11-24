using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using static SlugcatStats;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a scavenger squad trying to kill the player and makes them enter the room of the player
    /// </summary>
    internal class ScavengerGang : CEEvent
    {
        public ScavengerGang()
        {
            _name = "Kill squad";
            _description = "They are out for blood";
        }

        public override void StartupTrigger()
        {
            AbstractCreature creatureLeader = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Scavenger), null, new WorldCoordinate(game.world.offScreenDen.index, -1, -1, 0), game.GetNewID());
            ScavengerAbstractAI.ScavengerSquad sqaud = new ScavengerAbstractAI.ScavengerSquad(creatureLeader);
            for (int i = 0; i < 2; i++)
            {
                AbstractCreature creature = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.Scavenger), null, new WorldCoordinate(game.world.offScreenDen.index, -1, -1, 0), game.GetNewID());
                sqaud.AddMember(creature);
                sqaud.missionType = ScavengerAbstractAI.ScavengerSquad.MissionID.HuntCreature;
                sqaud.targetCreature = helper.MainPlayer;
                creature.ChangeRooms(helper.MainPlayer.pos);
                helper.MakeCreatureAttackCreature(creature, helper.MainPlayer);
            }
            creatureLeader.ChangeRooms(helper.MainPlayer.pos);
            helper.MakeCreatureAttackCreature(creatureLeader, helper.MainPlayer);
        }
    }
}
