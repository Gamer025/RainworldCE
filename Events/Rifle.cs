using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreSlugcats;
using UnityEngine;

namespace RainWorldCE.Events
{
    internal class Rifle : CEEvent
    {
        public Rifle()
        {
            _name = "Free gun";
            _description = "Let the slaughter beging";
        }

        public override void StartupTrigger()
        {
            List<AbstractPhysicalObject.AbstractObjectType> physicalObjects = new List<AbstractPhysicalObject.AbstractObjectType> {
            AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant,
            AbstractPhysicalObject.AbstractObjectType.DataPearl,
            AbstractPhysicalObject.AbstractObjectType.FlareBomb,
            AbstractPhysicalObject.AbstractObjectType.PuffBall,
            AbstractPhysicalObject.AbstractObjectType.ScavengerBomb,
            AbstractPhysicalObject.AbstractObjectType.SporePlant,
            AbstractPhysicalObject.AbstractObjectType.DangleFruit,
            AbstractPhysicalObject.AbstractObjectType.NeedleEgg,
            AbstractPhysicalObject.AbstractObjectType.Rock};
            if (ModManager.MSC)
            {
                physicalObjects.Add(MoreSlugcatsEnums.AbstractObjectType.FireEgg);
                physicalObjects.Add(MoreSlugcatsEnums.AbstractObjectType.SingularityBomb);
            }

            foreach (AbstractCreature player in EventHelpers.AllPlayers)
            {
                AbstractPhysicalObject aAmmo = null;
                AbstractPhysicalObject.AbstractObjectType ammo = physicalObjects[rnd.Next(physicalObjects.Count)];
                for (int i = 0; i < 2; i++)
                {
                    if (ammo == AbstractPhysicalObject.AbstractObjectType.PuffBall ||
                        ammo == AbstractPhysicalObject.AbstractObjectType.DangleFruit ||
                        ammo == AbstractPhysicalObject.AbstractObjectType.FlareBomb ||
                        ammo == AbstractPhysicalObject.AbstractObjectType.NeedleEgg ||
                        ammo == AbstractPhysicalObject.AbstractObjectType.FirecrackerPlant)
                    {
                        aAmmo = new AbstractConsumable(game.world, ammo, null, player.pos, game.GetNewID(), -1, -1, null);
                    }
                    else if (ammo == AbstractPhysicalObject.AbstractObjectType.SporePlant)
                    {
                        aAmmo = new SporePlant.AbstractSporePlant(game.world, null, player.pos, game.GetNewID(), -1, -1, null, used: false, pacified: true);
                    }
                    else if (ammo == AbstractPhysicalObject.AbstractObjectType.DataPearl)
                    {
                        aAmmo = new DataPearl.AbstractDataPearl(game.world, AbstractPhysicalObject.AbstractObjectType.DataPearl, null, player.pos, game.GetNewID(), -1, -1, null, DataPearl.AbstractDataPearl.DataPearlType.Misc);
                    }
                    else if (ammo == MoreSlugcatsEnums.AbstractObjectType.FireEgg)
                    {
                        float hue = Mathf.Lerp(0.35f, 0.6f, RWCustom.Custom.ClampedRandomVariation(0.5f, 0.5f, 2f));
                        aAmmo = new FireEgg.AbstractBugEgg(game.world, null, player.pos, game.GetNewID(), hue);
                    }
                    else
                    {
                        aAmmo = new AbstractPhysicalObject(game.world, ammo, null, player.pos, game.GetNewID());
                    }
                    aAmmo.Realize();
                    aAmmo.realizedObject.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);
                }

                JokeRifle.AbstractRifle abstractRifle = new JokeRifle.AbstractRifle(game.world, null, player.pos, game.GetNewID(), JokeRifle.AmmoTypeFromObject(aAmmo.realizedObject));
                abstractRifle.Realize();
                abstractRifle.realizedObject.PlaceInRoom(EventHelpers.CurrentRoom.realizedRoom);
            }
        }
    }
}
