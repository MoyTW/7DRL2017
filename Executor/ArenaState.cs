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
        private List<GameEvent_Command> executedCommands = new List<GameEvent_Command>();

        private List<Entity> mapEntities;

        // Turn state
        private Entity nextCommandEntity;

        public Entity NextCommandEntity { get { return this.nextCommandEntity; } }
        public IEnumerable<GameEvent_Command> ExecutedCommands { get { return this.executedCommands; } }

        public void ClearExecutedCommands() { this.executedCommands.Clear(); }

        // TODO: lol at exposing literally everything
        public int Level { get; }
        public int CurrentTick { get { return this.currentTick; } }
        public Entity Player { get; }
        public string MapID { get; }
        public IMap ArenaMap { get; }
        private PathFinder ArenaPathFinder { get; }
        public List<String> ArenaLog { get; }
        public bool PlayerWon
        {
            get
            {
                return !this.mapEntities.Where(e => e.HasComponentOfType<Component_AI>())
                    .Any(e => !e.TryGetDestroyed());
            }
        }
        public bool PlayerLost
        {
            get
            {
                return this.Player.TryGetDestroyed();
            }
        }

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

        public Path ShortestPath(Cell source, Cell destination)
        {
            try
            {
                return this.ArenaPathFinder.ShortestPath(source, destination);
            } catch (ArgumentOutOfRangeException ex)
            {
                Log.ErrorLine(ex.ToString());
                return null;
            }
        }

        public static int DistanceBetweenEntities(Entity a, Entity b)
        {
            var aPos = a.TryGetPosition();
            var bPos = b.TryGetPosition();

            return ArenaState.DistanceBetweenPositions(aPos.X, aPos.Y, bPos.X, bPos.Y);
        }

        public static int DistanceBetweenPositions(int x0, int y0, int x1, int y1)
        {
            return (int)Math.Floor(Math.Sqrt((x0 - x1) * (x0 - x1) + (y0 - y1) * (y0 - y1)));
        }

        public IList<Entity> InspectMapEntities()
        {
            return this.mapEntities.AsReadOnly();
        }

        public Entity EntityAtPosition(int x, int y)
        {
            foreach (var en in mapEntities)
            {
                if (!en.TryGetDestroyed())
                {
                    var position = (GameQuery_Position)en.HandleQuery(new GameQuery_Position());
                    if (position != null && position.X == x && position.Y == y)
                        return en;
                }
            }
            return null;
        }

        public ArenaState(IEnumerable<Entity> mapEntities, string mapID, IMap arenaMap, PathFinder arenaPathFinder, int level)
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
            this.ArenaLog = new List<String>();

            this.Level = level;

            ForwardToNextAction();
        }

        public ArenaState DeepCopy()
        {
            List<Entity> copyList = new List<Entity>();
            foreach (var e in this.mapEntities)
            {
                copyList.Add(e.DeepCopy());
            }
            return new ArenaState(copyList, this.MapID, this.ArenaMap.Clone(), this.ArenaPathFinder, this.Level);
        }

        // Only call this if you're using the arena as a copy!
        public void RemoveAllAIEntities()
        {
            for (int i = this.mapEntities.Count - 1; i >= 0; i--)
            {
                if (this.mapEntities[i].HasComponentOfType<Component_AI>())
                    this.mapEntities.RemoveAt(i);
            }
        }

        #region State Changes

        public void AlertAllAIs()
        {
            foreach (var entity in this.mapEntities.Where(e => e.HasComponentOfType<Component_AI>()))
            {
                entity.GetComponentOfType<Component_AI>().Alerted = true;
            }
            this.ArenaLog.Add("Your cloak was disrupted! All enemies are now on ALERT!");
        }

        private void ForwardToNextAction()
        {
            int leastTTL = 9999;
            Entity next = null;

            foreach (var entity in this.mapEntities.Where(e => !e.TryGetDestroyed()))
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

        public IEnumerable<Cell> WalkableCells()
        {
            return this.ArenaMap.GetAllCells().Where(c => c.IsWalkable).ToList();
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
                var remainingAP = this.nextCommandEntity.TryGetAttribute(EntityAttributeType.CURRENT_AP).Value;
                queryCommand.RegisterCommand(new CommandStub_Delay(this.nextCommandEntity.EntityID, remainingAP));
            }
            this.ResolveStub(queryCommand.Command);
        }

        public Entity ResolveEID(string eid)
        {
            return this.mapEntities.Where(e => e.EntityID == eid).First();
        }

        // TODO: Whoops, I designed the stubs badly. I should swap the resolution function to the stub classes.
        public void ResolveStub(CommandStub stub)
        {
            var gameEvent = stub.ReifyStub(this);
                
            if (gameEvent != null && gameEvent.CommandEntity == this.nextCommandEntity)
            {
                gameEvent.CommandEntity.HandleEvent(gameEvent);
                if (gameEvent.ShouldLog)
                    this.ArenaLog.Add(gameEvent.LogMessage);
                this.executedCommands.Add(gameEvent);
            }
            else if (gameEvent != null)
            {
                Log.ErrorLine("Can't resolve stub " + stub + " against entity " + gameEvent.CommandEntity +
                    " as next Entity is " + this.nextCommandEntity);
            }
            else
                throw new NullReferenceException("Stub " + stub + " reified to null; instead, return a delay!");

            // Hacky
            if (this.nextCommandEntity == this.Player)
            {
                foreach (var ai in this.mapEntities.Where(e => e.HasComponentOfType<Component_AI>()))
                {
                    if (!ai.GetComponentOfType<Component_AI>().Scanned &&
                        ArenaState.DistanceBetweenEntities(this.Player, ai) <=
                        ai.TryGetAttribute(EntityAttributeType.SCAN_REQUIRED_RADIUS).Value)
                    {
                        ai.GetComponentOfType<Component_AI>().Scanned = true;
                        this.ArenaLog.Add("Scanned " + ai.Label + "!");
                    }
                }
            }
            this.ForwardToNextAction();
        }
    }
}
