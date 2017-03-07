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

        private Entity mech2;
        private List<Entity> mapEntities;

        // Turn state
        private Entity nextCommandEntity;

        public Entity NextCommandEntity { get { return this.nextCommandEntity; } }

        // TODO: lol at exposing literally everything
        public int CurrentTick { get { return this.currentTick; } }
        public Entity Player { get; }
        public Entity Mech2 { get { return this.mech2; } }
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
            foreach (var en in mapEntities)
            {
                var position = (GameQuery_Position)en.HandleQuery(new GameQuery_Position());
                if (position != null && position.BlocksMovement && position.X == x && position.Y == y)
                    return false;
            }
            return this.ArenaMap.IsWalkable(x, y);
        }

        public bool IsMatchEnded()
        {
            return this.Player.GetComponentOfType<Component_Skeleton>().IsKilled ||
                this.Mech2.GetComponentOfType<Component_Skeleton>().IsKilled;
        }

        // TODO: Create a "Mech/Map Blueprint" so you don't pass a literal Entity/IMap instance in!
        public ArenaState(Entity player, Entity mech2, string mapID, IMap arenaMap, PathFinder arenaPathFinder)
        {
            if (!player.HasComponentOfType<Component_Player>())
                throw new ArgumentException("Can't initialize Arena: Player has no Component_Player");
            else if (!mech2.HasComponentOfType<Component_AI>())
                throw new ArgumentNullException("Can't initialize Arena: Mech 2 has no AI!");

            this.currentTick = 0;
            this.Player = player;
            this.mech2 = mech2;
            this.mapEntities = new List<Entity>();
            this.mapEntities.Add(player);
            this.mapEntities.Add(mech2);
            this.MapID = mapID;
            this.ArenaMap = arenaMap;
            this.ArenaPathFinder = arenaPathFinder;

            ForwardToNextAction();
        }

        public ArenaState DeepCopy()
        {
            return new ArenaState(this.Player.DeepCopy(), this.Mech2.DeepCopy(), this.MapID, this.ArenaMap.Clone(),
                this.ArenaPathFinder);
        }


        #region State Changes

        private void ForwardToNextAction()
        {
            // Mech1 always moves fully before mech2 if possible! First player advantage.
            var mech1Query = this.Player.HandleQuery(new GameQuery_TicksToLive(this.CurrentTick));
            var mech2Query = mech2.HandleQuery(new GameQuery_TicksToLive(this.CurrentTick));
            if (mech1Query.TicksToLive <= mech2Query.TicksToLive)
            {
                this.nextCommandEntity = this.Player;
                this.currentTick += mech1Query.TicksToLive;
            }
            else
            {
                this.nextCommandEntity = this.Mech2;
                this.currentTick += mech2Query.TicksToLive;
            }
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

        // TODO: Whoops, I designed the stubs badly. I should swap the resolution function to the stub classes.
        public void ResolveStub(CommandStub stub)
        {
            var gameEvent = stub.ReifyStub(this);
            gameEvent.CommandEntity.HandleEvent(gameEvent);
            this.ForwardToNextAction();
        }
    }
}
