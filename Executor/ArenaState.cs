using RogueSharp;
using RogueSharp.Random;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Executor
{
    public class ArenaState
    {
        private int currentTick;

        private List<Entity> mapEntities;

        // Turn state
        private Entity nextCommandEntity;

        public Entity NextCommandEntity { get { return this.nextCommandEntity; } }

        // TODO: lol at exposing literally everything
        public int CurrentTick { get { return this.currentTick; } }
        public Entity Player { get; }
        public string MapID { get; }
        public IMap ArenaMap { get; }
        public PathFinder ArenaPathFinder { get; }

        public bool ShouldWaitForPlayerInput {
            get
            {
                return !this.Player.HasComponentOfType<Component_AI>() && this.nextCommandEntity == this.Player;
            }
        }

        public bool IsWalkableAndOpen(int x, int y)
        {
            if (x < 0 || y < 0 || x >= this.ArenaMap.Width || y >= this.ArenaMap.Height)
                return false;

            foreach (var en in mapEntities)
            {
                var position = (GameQuery_Position)en.HandleQuery(new GameQuery_Position());
                if (position != null && position.BlocksMovement && position.X == x && position.Y == y)
                    return false;
            }
            return this.ArenaMap.IsWalkable(x, y);
        }

        public IList<Entity> InspectMapEntities()
        {
            return this.mapEntities.AsReadOnly();
        }

        public Entity EntityAtPosition(int x, int y)
        {
            foreach (var en in mapEntities)
            {
                var position = (GameQuery_Position)en.HandleQuery(new GameQuery_Position());
                if (position != null && position.X == x && position.Y == y)
                    return en;
            }
            return null;
        }

        public bool IsMatchEnded()
        {
            bool survivingAIs = this.mapEntities.Where(e => e.HasComponentOfType<Component_AI>())
                .Any(e => !e.GetComponentOfType<Component_Skeleton>().IsKilled);
            return this.Player.GetComponentOfType<Component_Skeleton>().IsKilled || !survivingAIs;
        }

        // TODO: Create a "Mech/Map Blueprint" so you don't pass a literal Entity/IMap instance in!
        public ArenaState(IEnumerable<Entity> mapEntities, string mapID, IMap arenaMap, PathFinder arenaPathFinder)
        {
            this.currentTick = 0;

            this.mapEntities = new List<Entity>();
            foreach (Entity e in mapEntities)
            {
                if (e.HasComponentOfType<Component_Player>())
                    this.Player = e;
                this.mapEntities.Add(e);
            }

            if (this.Player == null)
                throw new ArgumentException("Can't initialize Arena: Could not find player!");

            this.MapID = mapID;
            this.ArenaMap = arenaMap;
            this.ArenaPathFinder = arenaPathFinder;
            ForwardToNextAction();
        }

        public ArenaState DeepCopy()
        {
            List<Entity> copyList = new List<Entity>();
            foreach (var e in this.mapEntities)
            {
                copyList.Add(e.DeepCopy());
            }
            return new ArenaState(copyList, this.MapID, this.ArenaMap.Clone(), this.ArenaPathFinder);
        }

        #region State Changes

        private void ForwardToNextAction()
        {
            int leastTTL = 9999;
            Entity next = null;

            foreach (var entity in this.mapEntities)
            {
                var nextTTL = entity.HandleQuery(new GameQuery_TicksToLive(this.CurrentTick)).TicksToLive;
                if (nextTTL < leastTTL)
                {
                    leastTTL = nextTTL;
                    next = entity;
                }
            }

            this.nextCommandEntity = next;
            this.currentTick += leastTTL;
        }

        public Tuple<int, int> EmptyCellNear(int x, int y)
        {
            int round = 0;
            while (round < 10)
            {
                var firstEmptyNear = this.DiamondFirstEmpty(x, y, round);
                if (firstEmptyNear != null)
                    return firstEmptyNear;
                else
                    round++;
            }
            return null;
        }

        private Tuple<int, int> DiamondFirstEmpty(int x, int y, int round)
        {
            int runningX = x;
            int runningY = y - round;
            int thisRound = round;

            while (thisRound > 0)
            {
                runningX--;
                runningY++;
                if (this.IsWalkableAndOpen(runningX, runningY))
                    return new Tuple<int, int>(runningX, runningY);
                thisRound--;
            }
            thisRound = round;
            while (thisRound > 0)
            {
                runningX++;
                runningY++;
                if (this.IsWalkableAndOpen(runningX, runningY))
                    return new Tuple<int, int>(runningX, runningY);
                thisRound--;
            }
            thisRound = round;
            while (thisRound > 0)
            {
                runningX++;
                runningY--;
                if (this.IsWalkableAndOpen(runningX, runningY))
                    return new Tuple<int, int>(runningX, runningY);
                thisRound--;
            }
            thisRound = round;
            while (thisRound > 1)
            {
                runningX--;
                runningY--;
                if (this.IsWalkableAndOpen(runningX, runningY))
                    return new Tuple<int, int>(runningX, runningY);
                thisRound--;
            }
            return null;
        }

        public bool PlaceEntityNear(Entity en, int x, int y)
        {
            var emptyCell = this.EmptyCellNear(x, y);
            if (emptyCell == null)
            {
                return false;
            }
            else
            {
                if (!this.mapEntities.Contains(en))
                    this.mapEntities.Add(en);
                en.AddComponent(new Component_Position(emptyCell.Item1, emptyCell.Item2, true));
                return true;
            }
        }

        #endregion

        public void TryFindAndExecuteNextCommand()
        {
            // If it's the player's turn we must wait on input!
            if (this.ShouldWaitForPlayerInput)
                return;

            var queryCommand = this.nextCommandEntity.HandleQuery(
                new GameQuery_Command(this.nextCommandEntity, this));
            if (!queryCommand.Completed)
            {
                Log.ErrorLine("Failed to register AI command for " + this.nextCommandEntity);
                queryCommand.RegisterCommand(new GameEvent_Delay(this.CurrentTick, this.nextCommandEntity,
                    this.nextCommandEntity, DelayDuration.NEXT_ACTION));
                this.nextCommandEntity.HandleEvent(queryCommand.Command);
            }
            else
                this.nextCommandEntity.HandleEvent(queryCommand.Command);

            this.ForwardToNextAction();
        }

        public Entity ResolveEID(string eid)
        {
            return this.mapEntities.Where(e => e.EntityID == eid).First();
        }

        // TODO: Whoops, I designed the stubs badly. I should swap the resolution function to the stub classes.
        public void ResolveStub(CommandStub stub)
        {
            var gameEvent = stub.ReifyStub(this);
            if (gameEvent != null)
                gameEvent.CommandEntity.HandleEvent(gameEvent);
            else
                Console.WriteLine("Couldn't resolve stub: " + stub.ToString());
            this.ForwardToNextAction();
        }
    }
}
