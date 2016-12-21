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
        private Entity player;
        private Entity enemy;
        private List<Entity> mapEntities;
        private IMap arenaMap;

        // Turn state
        private Entity nextEntity;

        // TODO: lol at exposing literally everything
        public int CurrentTick { get { return this.currentTick; } }
        public Entity Mech1 { get { return this.player; } }
        public Entity Mech2 { get { return this.enemy; } }
        public Entity NextEntity { get { return this.nextEntity; } }
        public IMap ArenaMap { get { return this.arenaMap; } }

        // TODO: Create a "Mech/Map Blueprint" so you don't pass a literal Entity/IMap instance in!
        public Arena(Entity player, Entity enemy, IMap arenaMap)
        {
            this.currentTick = 0;
            this.player = player;
            this.enemy = enemy;
            this.mapEntities = new List<Entity>();
            this.mapEntities.Add(player);
            this.mapEntities.Add(enemy);
            this.arenaMap = arenaMap;

            ForwardToNextAction();
        }

        private void ForwardToNextAction(bool pass = false)
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

            var orderedEntities = allTimeTrackers.Where(e => !e.TryGetDestroyed().Destroyed)
                .OrderBy(e => e.HandleQuery(new GameQuery_TicksToLive(this.currentTick)).TicksToLive);

            if (pass && this.nextEntity == orderedEntities.FirstOrDefault())
                this.nextEntity = orderedEntities.ElementAtOrDefault(1);
            else
                this.nextEntity = orderedEntities.FirstOrDefault();

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
                    if (this.arenaMap.IsWalkableAndOpen(nx, ny, this.mapEntities))
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

        // TODO: Testing! Don't directly call!
        public void TryPlayerAttack()
        {
            if (this.nextEntity.HasComponentOfType<Component_Weapon>())
            {
                Console.WriteLine("########## ATTACK INFO ##########");
                var guns = this.player.HandleQuery(new GameQuery_SubEntities(SubEntitiesSelector.WEAPON)).SubEntities;
                foreach (var gun in guns)
                {
                    this.player.HandleEvent(new GameEvent_Attack(this.currentTick, player, enemy, gun, this.arenaMap));
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
            if (this.nextEntity == player)
            {
                var position = this.player.HandleQuery(new GameQuery_Position());
                if (this.arenaMap.IsWalkableAndOpen(position.X + dx, position.Y + dy, mapEntities))
                {
                    this.player.HandleEvent(new GameEvent_MoveSingle(this.currentTick, (XDirection)dx, (YDirection)dy));
                }
                this.ForwardToNextAction();
            }
            else
            {
                Console.WriteLine("CANNOT MOVE MOVE NOT NEXT!");
            }
        }

        public void PlayerPassAction()
        {
            this.ForwardToNextAction(pass: true);
        }
    }
}
