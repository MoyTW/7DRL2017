using RLNET;
using RogueSharp;

using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArena
{
    public class Arena
    {
        private int currentTick;

        // TODO: Don't literally have Player/Enemy, as two AIs can fight each other!
        private Entity mech1;
        private Entity mech2;
        private List<Entity> mapEntities;
        private IMap arenaMap;

        // Turn state
        private Entity nextEntity;

        // TODO: lol at exposing literally everything
        public int CurrentTick { get { return this.currentTick; } }
        public Entity Mech1 { get { return this.mech1; } }
        public Entity Mech2 { get { return this.mech2; } }
        public Entity NextEntity { get { return this.nextEntity; } }
        public IMap ArenaMap { get { return this.arenaMap; } }

        public bool ShouldWaitForPlayerInput {
            get
            {
                return !this.Mech1.HasComponentOfType<Component_AI>() &&
                    (this.NextEntity == this.Mech1 ||
                    Mech1.TryGetSubEntities(SubEntitiesSelector.TRACKS_TIME).Contains(this.NextEntity));
            }
        }

        public bool IsWalkableAndOpen(int x, int y)
        {
            foreach (var en in mapEntities)
            {
                var position = (GameQuery_Position)en.HandleQuery(new GameQuery_Position());
                if (position != null && position.BlocksMovement && position.X == x && position.Y == y)
                    return false;
            }
            return this.ArenaMap.IsWalkable(x, y);
        }

        // TODO: Create a "Mech/Map Blueprint" so you don't pass a literal Entity/IMap instance in!
        public Arena(Entity mech1, Entity mech2, IMap arenaMap)
        {
            if (!mech1.HasComponentOfType<Component_Player>() && !mech1.HasComponentOfType<Component_AI>())
                throw new ArgumentException("Can't initialize Arena: Mech 1 has no player or AI!");
            else if (!mech2.HasComponentOfType<Component_AI>())
                throw new ArgumentNullException("Can't initialize Arena: Mech 2 has no AI!");

            this.currentTick = 0;
            this.mech1 = mech1;
            this.mech2 = mech2;
            this.mapEntities = new List<Entity>();
            this.mapEntities.Add(mech1);
            this.mapEntities.Add(mech2);
            this.arenaMap = arenaMap;

            ForwardToNextAction();
        }

        // Pilot is killed when the head is destroyed
        // TODO: This is a really awkward way of looking up "Does it have ahead or not?"
        private bool IsPilotKilled(Entity mech)
        {
            return mech.TryGetSubEntities(SubEntitiesSelector.BODY_PART)
                .Where(e => e.GetComponentOfType<Component_BodyPartLocation>().Location == BodyPartLocation.HEAD)
                .First()
                .TryGetDestroyed();
        }

        // You're considered unable to fight if your torso goes down or you run out of weapons
        private bool IsMechUnableToFight(Entity mech)
        {
            var torsoDestroyed = mech.TryGetSubEntities(SubEntitiesSelector.BODY_PART)
                .Where(e => e.GetComponentOfType<Component_BodyPartLocation>().Location == BodyPartLocation.TORSO)
                .First()
                .TryGetDestroyed();
            var noWeapons = mech.TryGetSubEntities(SubEntitiesSelector.WEAPON).Count() == 0;
            return torsoDestroyed || noWeapons;
        }

        public bool IsMatchEnded()
        {
            return this.IsPilotKilled(this.Mech1) || this.IsMechUnableToFight(this.Mech1) ||
                this.IsPilotKilled(this.Mech2) || this.IsMechUnableToFight(this.Mech2);
        }

        private void ForwardToNextAction()
        {
            List<Entity> allTimeTrackers = new List<Entity>();
            foreach(var entity in this.mapEntities)
            {
                if (entity.HasComponentOfType<Component_TracksTime>())
                    allTimeTrackers.Add(entity);
                var subQuery = new GameQuery_SubEntities(SubEntitiesSelector.TRACKS_TIME);
                var subTimeTrackers = entity.HandleQuery(subQuery).SubEntities;
                allTimeTrackers.AddRange(subTimeTrackers);
            }

            this.nextEntity = allTimeTrackers.Where(e => !e.TryGetDestroyed())
                .OrderBy(e => e.TryGetTicksToLive(this.CurrentTick))
                .FirstOrDefault();

            int nextTicks = nextEntity.HandleQuery(new GameQuery_TicksToLive(this.currentTick)).TicksToLive;
            this.currentTick += nextTicks;

            Console.WriteLine("Ticks: " + this.currentTick + " Next Entity: " + nextEntity.ToString() + " Next Ticks: " + nextTicks);
        }

        public bool PlaceEntityNear(Entity en, int x, int y)
        {
            for (int nx = x - 1; nx <= x + 1; nx++)
            {
                for (int ny = y - 1; ny <= y + 1; ny++)
                {
                    if (this.IsWalkableAndOpen(nx, ny))
                    {
                        if (!this.mapEntities.Contains(en))
                            this.mapEntities.Add(en);
                        en.AddComponent(new Component_Position(nx, ny, true));
                        return true;
                    }
                }
            }
            return false;
        }

        public void TryFindAndExecuteNextCommand()
        {
            // If it's the player's turn we must wait on input!
            if (this.ShouldWaitForPlayerInput)
                return;

            var queryCommand = this.Mech2.HandleQuery(new GameQuery_Command(this.Mech2, this.NextEntity, this));
            if (!queryCommand.Completed)
                throw new ArgumentException("Didn't register AI move, something malfunctioned really bad in your AI!");
            else
                this.Mech2.HandleEvent(queryCommand.Command);

            this.ForwardToNextAction();
        }

        // TODO: Testing! Don't directly call!
        public void TryPlayerAttack()
        {
            if (this.ShouldWaitForPlayerInput && this.nextEntity.HasComponentOfType<Component_Weapon>())
            {
                Console.WriteLine("########## ATTACK INFO ##########");
                var guns = this.mech1.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.WEAPON)).SubEntities;
                foreach (var gun in guns)
                {
                    this.mech1.HandleEvent(new GameEvent_Attack(this.currentTick, mech1, mech2, gun, this.arenaMap));
                }
                this.ForwardToNextAction();
            }
            else
            {
                Console.WriteLine("CANNOT ATTACK GUNS NOT NEXT!");
            }
        }

        // TODO: Testing! Don't directly call!
        public void TryPlayerMove(int dx, int dy)
        {
            if (this.nextEntity == mech1)
            {
                var position = this.mech1.HandleQuery(new GameQuery_Position());
                if (this.IsWalkableAndOpen(position.X + dx, position.Y + dy))
                {
                    this.mech1.HandleEvent(new GameEvent_MoveSingle(mech1, this.currentTick, dx, dy, this));
                }
                this.ForwardToNextAction();
            }
            else
            {
                Console.WriteLine("CANNOT MOVE MOVE NOT NEXT!");
            }
        }

        public void PlayerDelayAction(DelayDuration duration)
        {
            if (this.ShouldWaitForPlayerInput)
            {
                this.nextEntity.HandleEvent(
                    new GameEvent_Delay(this.CurrentTick, this.Mech1, this.nextEntity, duration));
                this.ForwardToNextAction();
            }
        }
    }
}
