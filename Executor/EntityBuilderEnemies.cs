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

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildPistol());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildRentACopBrave(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildPistol());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildRentACopCowardly(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Flee());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildPistol());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildRentACopBaton(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildBaton());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildRentACopBatonCowardly(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_Flee());

            var entity = EntityBuilder.BuildNakedMech("Rent-A-Cop " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildBaton());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildPhoneScanner());

            return entity;
        }

        public static Entity BuildPoliceMarksman(string designation)
        {
            List<ActionClause> clauses = new List<ActionClause>();
            clauses.Add(new ActionClause_AvoidMelee());
            clauses.Add(new ActionClause_Attack());
            clauses.Add(new ActionClause_Approach());

            var entity = EntityBuilder.BuildNakedMech("Marksman " + designation, false, new Guidebook(clauses));

            EntityBuilder.MountOntoArm(entity, BodyPartLocation.RIGHT_ARM, EntityBuilderWeapons.BuildCombatRifle());
            EntityBuilder.MountOntoArm(entity, BodyPartLocation.LEFT_ARM, EntityBuilder.BuildHandheldScanner());

            return entity;
        }

        public static Entity BuildLevel0Entity(IRandom rand, string designation)
        {
            var selection = rand.Next(4);
            switch (selection)
            {
                case 0:
                    return EntityBuilderEnemies.BuildRentACop(designation);
                case 1:
                    return EntityBuilderEnemies.BuildRentACopCowardly(designation);
                case 2:
                    return EntityBuilderEnemies.BuildRentACopBrave(designation);
                case 3:
                    return EntityBuilderEnemies.BuildRentACopBaton(designation);
                case 4:
                    return EntityBuilderEnemies.BuildRentACopBatonCowardly(designation);
                default:
                    throw new InvalidOperationException();
            }
        }


        public static Entity BuildRandomEnemy(IRandom rand, string designation)
        {
            throw new NotImplementedException();
        }
    }
}

