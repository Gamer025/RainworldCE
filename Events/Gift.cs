using BepInEx.Logging;
using UnityEngine;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Spawns a random gift on top of each player
    /// </summary>
    internal class Gift : CEEvent
    {
        public Gift()
        {
            _name = "A small/normal/great gift";
            _description = "You will probably need this";
        }

        enum GiftSize
        {
            small,
            normal,
            big
        }

        private GiftSize size;

        public override void StartupTrigger()
        {
            size = GetRandomGiftSize;
            _name = $"A {size} gift";
            _description = size switch
            {
                GiftSize.small => "Take this, it's dangerous to go alone",
                GiftSize.normal => "You will probably need this",
                GiftSize.big => "A great gift for an amazing little creature",
                _ => "",
            };
            foreach (AbstractCreature player in EventHelpers.AllPlayers)
            {
                AbstractPhysicalObject reward = null;
                if (ModManager.MSC)
                {
                    switch (size)
                    {
                        case GiftSize.small:
                            switch (rnd.Next(20))
                            {
                                case < 5:
                                    reward = new AbstractSpear(game.world, null, player.pos, game.GetNewID(), true);
                                    break;
                                case < 14:
                                    reward = new MoreSlugcats.LillyPuck.AbstractLillyPuck(game.world, null, player.pos, game.GetNewID(), 3, -1, -1, null);
                                    break;
                                case <= 17:
                                    reward = new AbstractSpear(game.world, null, player.pos, game.GetNewID(), hue: 1f, explosive: false);
                                    break;
                                case > 17:
                                    reward = new DataPearl.AbstractDataPearl(game.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, player.pos, game.GetNewID(), -1, -1, null, DataPearl.AbstractDataPearl.DataPearlType.Misc);
                                    break;
                            }
                            break;
                        case GiftSize.normal:
                            switch (rnd.Next(20))
                            {
                                case < 7:
                                    reward = new AbstractConsumable(game.world, MoreSlugcats.MoreSlugcatsEnums.AbstractObjectType.GooieDuck, null, player.pos, game.GetNewID(), -1, -1, null);
                                    break;
                                case < 12:
                                    reward = new AbstractPhysicalObject(game.world, AbstractPhysicalObject.AbstractObjectType.ScavengerBomb, null, player.pos, game.GetNewID());
                                    break;
                                case <= 15:
                                    reward = new AbstractSpear(game.world, null, player.pos, game.GetNewID(), electric: true, explosive: false);
                                    break;
                                case > 15:
                                    EntityID id = game.GetNewID();
                                    reward = new VultureMask.AbstractVultureMask(game.world, null, player.pos, id, id.RandomSeed, true);
                                    break;
                            }
                            break;
                        case GiftSize.big:
                            switch (rnd.Next(20))
                            {
                                case < 10:
                                    reward = new AbstractConsumable(game.world, AbstractPhysicalObject.AbstractObjectType.KarmaFlower, null, player.pos, game.GetNewID(), -1, -1, null);
                                    break;
                                case < 14:
                                    reward = new AbstractCreature(game.world, StaticWorld.GetCreatureTemplate(CreatureTemplate.Type.TubeWorm), null, player.pos, game.GetNewID());
                                    break;
                                case <= 17:
                                    reward = new AbstractPhysicalObject(game.world, MoreSlugcats.MoreSlugcatsEnums.AbstractObjectType.SingularityBomb, null, player.pos, game.GetNewID());
                                    reward.realizedObject = new MoreSlugcats.SingularityBomb(reward, game.world)
                                    {
                                        zeroMode = true,
                                        explodeColor = new Color(1f, 0.2f, 0.2f),
                                        connections = new CoralBrain.Mycelium[0],
                                        holoShape = null

                                    };
                                    EventHelpers.CurrentRoom.realizedRoom.PlaySound(MoreSlugcats.MoreSlugcatsEnums.MSCSoundID.Inv_Hit, 0f, 1f, 0.8f + UnityEngine.Random.value * 1f);
                                    break;
                                case > 17:
                                    reward = new AbstractPhysicalObject(game.world, AbstractPhysicalObject.AbstractObjectType.SSOracleSwarmer, null, player.pos, game.GetNewID());
                                    break;
                            }
                            break;
                        default:
                            reward = new AbstractSpear(game.world, null, player.pos, game.GetNewID(), false);
                            break;
                    }
                }
                else
                {
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
                            switch (rnd.Next(10))
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
