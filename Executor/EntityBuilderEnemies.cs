using Executor.AI;
using System;
using System.Collections.Generic;

namespace Executor
{
    public static class EntityBuilderEnemies
    {
        public static Entity BuildRifleman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var mech = EntityBuilder.BuildNakedMech("Rifleman " + designation, false, new Entity(),
                new Guidebook(clauses));

            EntityBuilder.MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildRifle());

            return mech;
        }
    }
}

