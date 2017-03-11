using Executor.AI;
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

        public bool Alerted { get; set; }

        public IEnumerable<ActionClause> ActionClauses { get { return this.activeBook.ActionClauses; } }

        public Component_AI(Guidebook activeBook)
        {
            this.activeBook = activeBook;
            this.Alerted = false;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        public IEnumerable<RogueSharp.Cell> AlertCells(ArenaState arena)
        {
            var radius = this.Parent.TryGetAttribute(EntityAttributeType.DETECTION_RADIUS).Value;
            var myPosition = this.Parent.TryGetPosition();

            List<RogueSharp.Cell> cells = new List<RogueSharp.Cell>();

            for (int x = -radius; x <= radius; ++x)
            {
                for (int y = -radius; y <= radius; ++y)
                {
                    var d = (int)Math.Floor(Math.Sqrt(x * x + y * y));
                    if (d <= radius)
                    {
                        int mx = myPosition.X + x;
                        int my = myPosition.Y + y;
                        if (mx >= 0 && my >= 0 && mx < arena.ArenaMap.Width && my < arena.ArenaMap.Height)
                        {
                            var cell = arena.ArenaMap.GetCell(myPosition.X + x, myPosition.Y + y);
                            if (cell.IsWalkable)
                                cells.Add(cell);
                        }
                    }
                }
            }
            return cells;
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
        }

        private void HandleEntityAttribute(GameQuery_EntityAttribute q)
        {
            if (q.AttributeType == EntityAttributeType.DETECTION_RADIUS)
                q.RegisterBaseValue(Config.ZERO);
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
