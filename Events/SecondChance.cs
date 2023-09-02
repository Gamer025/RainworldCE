using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml.Schema;
using static AbstractPhysicalObject;

namespace RainWorldCE.Events
{
    /// <summary>
    /// Prevents the player from dying in most cases, by respawning them in the last room
    /// </summary>
    internal class SecondChance : CEEvent
    {
        public SecondChance()
        {
            _name = "Second Chance";
            _description = "A never ending cycle of pain\r\n Only triggers on actual dead/might take a few seconds";
            _activeTime = 9999;
        }

        AbstractRoom lastRoom = null;
        int lastRoomNode = 0;
        bool playerSaved = false;

        public override void StartupTrigger()
        {
            On.Player.Die += PlayerDieHook;
            On.AbstractCreature.Die += AbstractCreatureDieHook;
            On.AbstractCreature.Abstractize += AbstractCreatureAbstractizeHook;
            On.UpdatableAndDeletable.Destroy += DestroyHook;
            On.DaddyLongLegs.Collide += DLLCollideHook;
            On.AbstractCreature.IsEnteringDen += CreatureEnterDenHook;
            lastRoom = EventHelpers.CurrentRoom;
        }

        public override void ShutdownTrigger()
        {
            On.Player.Die -= PlayerDieHook;
            On.AbstractCreature.Die -= AbstractCreatureDieHook;
            On.AbstractCreature.Abstractize -= AbstractCreatureAbstractizeHook;
            On.UpdatableAndDeletable.Destroy -= DestroyHook;
            On.DaddyLongLegs.Collide -= DLLCollideHook;
            On.AbstractCreature.IsEnteringDen -= CreatureEnterDenHook;
            if (playerSaved)
            {
                //Spark
                foreach (AbstractCreature player in game.Players)
                {
                    for (int amount = 0; amount < 20; amount++)
                        EventHelpers.CurrentRoom.realizedRoom.AddObject(new Spark(player.realizedCreature.mainBodyChunk.pos, RWCustom.Custom.RNV() * UnityEngine.Random.value * 40f, new UnityEngine.Color(1f, 1f, 1f), null, 30, 120));
                }
                lastRoom.realizedRoom.PlaySound(SoundID.SS_AI_Give_The_Mark_Boom, 0f, 1f, 1f);
            }

        }


        public override void PlayerChangingRoomTrigger(ref ShortcutHandler self, ref Creature creature, ref Room room, ref ShortcutData shortCut)
        {
            lastRoom = room.abstractRoom;
            lastRoomNode = shortCut.destNode;
        }

        public void PlayerDieHook(On.Player.orig_Die orig, Player self)
        {
            WriteLog(BepInEx.Logging.LogLevel.Debug, $"Prevented Player Die of {self}");
            SafePlayer();
            return;
        }

        public void AbstractCreatureDieHook(On.AbstractCreature.orig_Die orig, AbstractCreature self)
        {
            if (self.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
            {
                WriteLog(BepInEx.Logging.LogLevel.Debug, $"Prevented AbstractCreature Die of {self}");
                SafePlayer();
                return;
            }
            orig(self);
        }

        public void AbstractCreatureAbstractizeHook(On.AbstractCreature.orig_Abstractize orig, AbstractCreature self, WorldCoordinate coord)
        {
            if (self.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
            {
                WriteLog(BepInEx.Logging.LogLevel.Debug, $"Prevented Abstractize of {self}");
                SafePlayer();
                return;
            }
            orig(self, coord);
        }

        //Some deaths also destroy the players characters, this prevents this
        public void DestroyHook(On.UpdatableAndDeletable.orig_Destroy orig, UpdatableAndDeletable self)
        {
            if (self.GetType() == typeof(Player))
            {
                WriteLog(BepInEx.Logging.LogLevel.Debug, $"Prevented destroy of object");
                SafePlayer();
                return;
            }
            orig(self);
        }

        public void DLLCollideHook(On.DaddyLongLegs.orig_Collide orig, DaddyLongLegs self, PhysicalObject otherObject, int myChunk, int otherChunk)
        {
            if (otherObject is Player)
            {
                SafePlayer();
                return;
            }
            orig(self, otherObject, myChunk, otherChunk);
        }

        public void CreatureEnterDenHook(On.AbstractCreature.orig_IsEnteringDen orig, AbstractCreature self, WorldCoordinate den)
        {
            WriteLog(BepInEx.Logging.LogLevel.Debug, $"self: {self}, creaturetemp: {self.creatureTemplate}, type: {self.creatureTemplate.type}, stuckobj: {self.stuckObjects.Count}]");
            if (self.creatureTemplate.type == CreatureTemplate.Type.Slugcat)
            {
                SafePlayer();
                return;
            }
            for (int num = self.stuckObjects.Count - 1; num >= 0; num--)
            {
                if (num < self.stuckObjects.Count && self.stuckObjects[num] is CreatureGripStick && self.stuckObjects[num].A == self && self.stuckObjects[num].B is AbstractCreature)
                {
                    if ((self.stuckObjects[num].B as AbstractCreature).creatureTemplate.type == CreatureTemplate.Type.Slugcat)
                    {
                        self.DropCarriedObject((self.stuckObjects[num] as CreatureGripStick).grasp);
                        SafePlayer();
                    }
                }
            }
            orig(self, den);
        }

        private void SafePlayer()
        {
            WriteLog(BepInEx.Logging.LogLevel.Debug, $"{new System.Diagnostics.StackTrace()}");
            if (playerSaved) return;

            if (lastRoom.realizedRoom == null)
            {
                lastRoom.RealizeRoom(game.world, game);
            }
            foreach (AbstractCreature player in game.Players)
            {
                WriteLog(BepInEx.Logging.LogLevel.Debug, $"Trying to save {player}, {player.realizedCreature}. Last room was: {lastRoom.name}. Node: {lastRoomNode}");

                for (int i = player.stuckObjects.Count - 1; i >= 0; i--)
                {
                    AbstractPhysicalObject.AbstractObjectStick stick = player.stuckObjects[i];
                    if (stick.A is AbstractCreature && (stick.A as AbstractCreature).creatureTemplate.type != CreatureTemplate.Type.Slugcat)
                        (stick.A as AbstractCreature).realizedCreature.LoseAllGrasps();
                    if (stick.B is AbstractCreature && (stick.B as AbstractCreature).creatureTemplate.type != CreatureTemplate.Type.Slugcat)
                        (stick.B as AbstractCreature).realizedCreature.LoseAllGrasps();
                }

                game.shortcuts.CreatureTeleportOutOfRoom(player.realizedCreature, player.pos, new WorldCoordinate(lastRoom.index, -1, -1, lastRoomNode));
            }
            _activeTime = 1;
            playerSaved = true;
        }
    }
}
