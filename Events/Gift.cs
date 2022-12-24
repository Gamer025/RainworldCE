using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static SlugcatStats;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a random gift on top of each player
    /// </summary>
    internal class Gift : CEEvent
    {
        public Gift()
        {
            size = GetRandomGiftSize;
            _name = "A small/normal/great gift";
            if (EventHelpers.StoryModeActive)
            {
                _name = $"A {size} gift";
            }
            _description = size switch
            {
                GiftSize.small => "Take this, it's dangerous to go alone",
                GiftSize.normal => "You will probably need this",
                GiftSize.big => "A great gift for an amazing little creature",
                _ => "",
            };
        }

        enum GiftSize
        {
            small,
            normal,
            big
        }

        private readonly GiftSize size;

        public override void StartupTrigger()
        {
            foreach (AbstractCreature player in EventHelpers.AllPlayers)
            {
                AbstractPhysicalObject reward;
                switch (size)
                {
                    case GiftSize.small:
                        switch (rnd.Next(10))
                        {
                            case < 4:
                                reward = new AbstractSpear(game.world, null, player.pos, game.GetNewID(), false);
                                break;
                            case <= 7:
                                reward = new AbstractConsumable(game.world, AbstractPhysicalObject.AbstractObjectType.DangleFruit, null, player.pos, game.GetNewID(), -1, -1, null);
                                break;
                            case > 7:
                                reward = new DataPearl.AbstractDataPearl(game.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, player.pos, game.GetNewID(), -1, -1, null, DataPearl.AbstractDataPearl.DataPearlType.Misc);
                                break;
                        }
                        break;
                    case GiftSize.normal:
                        int chance = rnd.Next(10);
                        WriteLog(LogLevel.Info, $"Chance {chance}");
                        switch (chance)
                        {
                            case < 4:
                                reward = new AbstractSpear(game.world, null, player.pos, game.GetNewID(), true);
                                break;
                            case <= 7:
                                reward = new AbstractPhysicalObject(game.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, player.pos, game.GetNewID());
                                break;
                            case > 7:
                                EntityID id = game.GetNewID();
                                reward = new VultureMask.AbstractVultureMask(game.world, null, player.pos, id, id.RandomSeed, false);
                                break;
                        }
                        break;
                    case GiftSize.big:
                        switch (rnd.Next(10))
                        {
                            case < 7:
                                reward = new AbstractConsumable(game.world, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null, player.pos, game.GetNewID(), -1, -1, null);
                                break;
                            case <= 8:
                                reward = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.TubeWorm), null, player.pos, game.GetNewID());
                                break;
                            case > 8:
                                reward = new AbstractPhysicalObject(game.world, AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer, null, player.pos, game.GetNewID());
                                break;
                        }
                        break;
                    default:
                        reward = new AbstractSpear(game.world, null, player.pos, game.GetNewID(), false);
                        break;
                }

                WriteLog(LogLevel.Debug, $"Gift reward is {reward}");
                reward.Realize();
                reward.realizedObject.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);
            }
            
        }

        private static GiftSize GetRandomGiftSize
        {
            get
            {
                switch (rnd.Next(100))
                {
                    case <= 30:
                        return GiftSize.small;
                    case < 90:
                        return GiftSize.normal;
                    case >= 90:
                        return GiftSize.big;
                }
            }
        }
    }
}
