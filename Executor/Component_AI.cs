using Executor.AI;
using RogueSharp;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Executor
{
    [Serializable()]
    class Component_AI : Component
    {
        private Guidebook activeBook;

        public bool Scanned { get; set; }
        public bool Alerted { get; set; }
        public bool OnReturnLeg { get; private set; }
        public GameQuery_Position PatrolStart { get; private set; }
        public GameQuery_Position PatrolEnd { get; private set; }

        public IEnumerable<ActionClause> ActionClauses { get { return this.activeBook.ActionClauses; } }

        public Component_AI(Guidebook activeBook)
        {
            this.activeBook = activeBook;
            this.Scanned = false;
            this.Alerted = false;
            this.OnReturnLeg = false;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private Cell PositionToCell(GameQuery_Position pos, ArenaState arena)
        {
            return arena.ArenaMap.GetCell(pos.X, pos.Y);
        }

        public class CellInfo
        {
            public List<Cell> AlertCells = new List<Cell>();
            public List<Cell> ScanCells = new List<Cell>();
        }

        public CellInfo AlertCells(ArenaState arena)
        {
            var info = new CellInfo();
            var scanRadius = this.Parent.TryGetAttribute(EntityAttributeType.SCAN_REQUIRED_RADIUS).Value;
            var detectRadius = this.Parent.TryGetAttribute(EntityAttributeType.DETECTION_RADIUS).Value;
            var myPosition = this.Parent.TryGetPosition();

            for (int x = -scanRadius; x <= scanRadius; ++x)
            {
                for (int y = -scanRadius; y <= scanRadius; ++y)
                {
                    var d = (int)Math.Floor(Math.Sqrt(x * x + y * y));
                    int mx = myPosition.X + x;
                    int my = myPosition.Y + y;
                    Cell cell = null;
                    if (mx >= 0 && my >= 0 && mx < arena.ArenaMap.Width && my < arena.ArenaMap.Height)
                    {
                        cell = arena.ArenaMap.GetCell(myPosition.X + x, myPosition.Y + y);
                    }

                    if (cell != null && d <= detectRadius && !this.Alerted)
                    {
                        if (cell.IsWalkable)
                            info.AlertCells.Add(cell);
                    }
                    else if (!this.Scanned && cell != null && d <= scanRadius)
                    {
                        if (cell.IsWalkable)
                            info.ScanCells.Add(cell);
                    }
                }
            }
            return info;
        }

        private CommandStub MoveEventForPath(GameQuery_Command commandQuery, Path path)
        {
            var commandPos = commandQuery.CommandEntity.TryGetPosition();
            var nextCell = path.CurrentStep;

            if (path.CurrentStep != path.End)
            {
                path.StepForward();
            }

            return new CommandStub_MoveSingle(commandQuery.CommandEntity.EntityID, nextCell.X - commandPos.X,
                nextCell.Y - commandPos.Y);
        }

        public void DeterminePatrolPath(ArenaState state, IRandom rand)
        {
            this.PatrolStart = this.Parent.TryGetPosition();
            var cells = state.WalkableCells();
            Cell cell = rand.RandomElement(cells);
            while (Config.MinPatrolDistance < ArenaState.DistanceBetweenPositions(this.PatrolStart.X,
                this.PatrolStart.Y, cell.X, cell.Y))
            {
                cell = rand.RandomElement(cells);
            }
            var endPos = new GameQuery_Position();
            endPos.RegisterPosition(cell.X, cell.Y, false);
            this.PatrolEnd = endPos;
        }

        private void HandleQueryCommand(GameQuery_Command q)
        {
            if (this.Alerted)
            {
                this.activeBook.TryRegisterCommand(q);
            }
            else if (ArenaState.DistanceBetweenEntities(this.Parent, q.ArenaState.Player) <=
                    this.Parent.TryGetAttribute(EntityAttributeType.DETECTION_RADIUS).Value)
            {
                q.ArenaState.AlertAllAIs();
            }
            else
            {
                var myPos = this.Parent.TryGetPosition();
                if (myPos.X == this.PatrolStart.X && myPos.Y == this.PatrolStart.Y)
                    this.OnReturnLeg = false;
                else if (myPos.X == this.PatrolEnd.X && myPos.Y == this.PatrolEnd.Y)
                    this.OnReturnLeg = true;

                Cell myCell = q.ArenaState.ArenaMap.GetCell(myPos.X, myPos.Y);
                Path patrolPath;
                if (!this.OnReturnLeg)
                {
                    patrolPath = q.ArenaState.ShortestPath(myCell, this.PositionToCell(this.PatrolEnd, q.ArenaState));
                }
                else
                {
                    patrolPath = q.ArenaState.ShortestPath(myCell, 
                        this.PositionToCell(this.PatrolStart, q.ArenaState));
                }
                if (patrolPath != null)
                    q.RegisterCommand(this.MoveEventForPath(q, patrolPath));
                else
                    q.RegisterCommand(new CommandStub_Delay(this.Parent.EntityID, Config.ONE));
            }
        }

        private void HandleEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (q.AttributeType == EntityAttributeType.DETECTION_RADIUS)
                q.RegisterBaseValue(Config.ZERO);
            else if (q.AttributeType == EntityAttributeType.SCAN_REQUIRED_RADIUS)
                q.RegisterBaseValue(this.Parent.TryGetAttribute(EntityAttributeType.DETECTION_RADIUS).Value + 1);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Command)
                this.HandleQueryCommand((GameQuery_Command)q);
            else if (q is GameQuery_EntityAttribute)
                this.HandleEntityAttribute((GameQuery_EntityAttribute)q);

            return q;

        }
    }
}
