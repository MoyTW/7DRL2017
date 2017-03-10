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
        private Guidebook book;

        public IEnumerable<ActionClause> ActionClauses { get { return this.book.ActionClauses; } }

        public Component_AI()
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            this.book = new Guidebook(clauses);
        }

        public Component_AI(Guidebook book)
        {
            this.book = book;
        }

        protected override IImmutableSet<SubEntitiesSelector> _MatchingSelectors()
        {
            return ImmutableHashSet<SubEntitiesSelector>.Empty;
        }

        private void HandleQueryCommand(GameQuery_Command q)
        {
            this.book.TryRegisterCommand(q);
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Command)
                this.HandleQueryCommand((GameQuery_Command)q);

            return q;

        }
    }
}
