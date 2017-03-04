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
        private IRandom seededRand;

        // Turn state
        // TODO: Don't have Command + Executor, it's awkard as heck!
        private Entity nextCommandEntity;
        private Entity nextExecutorEntity;

        // TODO: lol at exposing literally everything
        public int ArenaSeed { get; }
        public int CurrentTick { get { return this.currentTick; } }
        public Entity Player { get; }
        public Entity Mech2 { get { return this.mech2; } }
        public Entity NextExecutorEntity { get { return this.nextExecutorEntity; } }
        public string MapID { get; }
        public string MatchID { get; }
        public IMap ArenaMap { get; }
        public PathFinder ArenaPathFinder { get; }
        public IRandom SeededRand { get { return this.seededRand; } }

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
        public ArenaState(Entity player, Entity mech2, string mapID, IMap arenaMap, PathFinder arenaPathFinder,
            int arenaSeed, string matchID)
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
            this.MatchID = matchID;
            this.MapID = mapID;
            this.ArenaMap = arenaMap;
            this.ArenaPathFinder = arenaPathFinder;
            this.ArenaSeed = arenaSeed;
            this.seededRand = new DotNetRandom(arenaSeed);

            ForwardToNextAction();
        }

        #region State Changes

        private void ForwardToNextAction()
        {
            // Mech1 always moves fully before mech2 if possible! First player advantage.
            var mech1Query = this.Player.HandleQuery(new GameQuery_NextTimeTracker(this.CurrentTick));
            var mech2Query = mech2.HandleQuery(new GameQuery_NextTimeTracker(this.CurrentTick));
            if (mech1Query.NextEntityTicksToLive <= mech2Query.NextEntityTicksToLive)
            {
                this.nextCommandEntity = this.Player;
                this.nextExecutorEntity = mech1Query.NextEntity;
                this.currentTick += mech1Query.NextEntityTicksToLive;
            }
            else
            {
                this.nextCommandEntity = this.Mech2;
                this.nextExecutorEntity = mech2Query.NextEntity;
                this.currentTick += mech2Query.NextEntityTicksToLive;
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
                new GameQuery_Command(this.nextCommandEntity, this.NextExecutorEntity, this));
            if (!queryCommand.Completed)
            {
                Log.ErrorLine("Failed to register AI command for " + this.nextExecutorEntity);
                queryCommand.RegisterCommand(new GameEvent_Delay(this.CurrentTick, this.nextCommandEntity,
                    this.nextExecutorEntity, DelayDuration.NEXT_ACTION));
                this.nextCommandEntity.HandleEvent(queryCommand.Command);
            }
            else
                this.nextCommandEntity.HandleEvent(queryCommand.Command);

            this.ForwardToNextAction();
        }

        #region Player Commands

        // TODO: Testing! Don't directly call!
        public void TryPlayerAttack()
        {
            if (this.ShouldWaitForPlayerInput)
            {
                Log.DebugLine("########## ATTACK INFO ##########");

                // TODO: Equipped items are *not* "Whatever is held in right right arm"
                var equippedWeapon = this.Player.GetComponentOfType<Component_Skeleton>()
                    .InspectBodyPart(BodyPartLocation.RIGHT_ARM)
                    .TryGetSubEntities(SubEntitiesSelector.WEAPON)
                    .FirstOrDefault();
                if (equippedWeapon != null)
                {
                    var attack = new GameEvent_Attack(this.CurrentTick, this.Player, this.Mech2, equippedWeapon,
                        this.ArenaMap, BodyPartLocation.TORSO);
                    this.Player.HandleEvent(attack);
                }

                this.ForwardToNextAction();
            }
        }

        // TODO: Testing! Don't directly call!
        public void TryPlayerMove(int dx, int dy)
        {
            if (this.NextExecutorEntity == this.Player)
            {
                var position = this.Player.HandleQuery(new GameQuery_Position());
                if (this.IsWalkableAndOpen(position.X + dx, position.Y + dy))
                {
                    this.Player.HandleEvent(new GameEvent_MoveSingle(this.Player, this.currentTick, dx, dy, this));
                }
                this.ForwardToNextAction();
            }
            else
            {
                Log.DebugLine("CANNOT MOVE MOVE NOT NEXT!");
            }
        }

        public void PlayerDelayAction(DelayDuration duration)
        {
            if (this.ShouldWaitForPlayerInput)
                this.ForwardToNextAction();
        }

        #endregion
    }
}
