using Executor.AI;
using RogueSharp.Random;
using System;
using System.Collections.Generic;

namespace Executor
{
    public static class EntityBuilderEnemies
    {
        public static Entity BuildRentACop(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_SeekMaxWeaponRange());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var mech = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Entity(),
                new Guidebook(clauses));

            EntityBuilder.MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildPistol());
            EntityBuilder.MountOntoArm(mech, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return mech;
        }
        
        public static Entity BuildMarksman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var mech = EntityBuilder.BuildNakedMech("Marksman " + designation, false, new Entity(),
                new Guidebook(clauses));

            EntityBuilder.MountOntoArm(mech, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildCombatRifle());
            EntityBuilder.MountOntoArm(mech, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            return mech;
        }

        public static Entity BuildRandomEnemy(IRandom rand, string designation)
        {
            var selection = rand.Next(1);
            switch (selection)
            {
                case 0:
                    return BuildRentACop(designation);
                case 1:
                    return BuildMarksman(designation);
                default:
                    throw new InvalidOperationException("BLAH");
            }
        }
    }
}

