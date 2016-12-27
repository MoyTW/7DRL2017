using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena
{
    class Component_AI : Component
    {
        private void HandleQueryCommand(GameQuery_Command q)
        {
            if (q.CommandEntity.HasComponentOfType<Component_MechSkeleton>())
            {
                throw new NotImplementedException();
            }
            else if (q.CommandEntity.HasComponentOfType<Component_Weapon>())
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new ArgumentException("Entity " + q.CommandEntity + " is not Mech, Weapon! Cannot command!");
            }
        }

        protected override GameQuery _HandleQuery(GameQuery q)
        {
            if (q is GameQuery_Command)
                this.HandleQueryCommand((GameQuery_Command)q);

            return q;

        }
    }
}
