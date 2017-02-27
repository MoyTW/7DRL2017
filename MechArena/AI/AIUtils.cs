using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MechArena.Genetic;
using MechArena.AI.Combat;
using MechArena.AI.Hanger;

namespace MechArena.AI
{
    public static class AIUtils
    {
        public static GeneListing<SingleClause> geneList;

        public static IEnumerable<SingleClause> DeriveAllClauses()
        {
            return Assembly.GetAssembly(typeof(SingleClause))
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(SingleClause)))
                .Where(t => !t.IsAbstract)
                .SelectMany(t => ((SingleClause)Activator.CreateInstance(t)).EnumerateClauses());
        }

        public static void Initialize()
        {
            AIUtils.geneList = new GeneListing<SingleClause>(AIUtils.DeriveAllClauses());
        }

        public static Entity AssembleMech(string label, bool player, Entity pilot, IEnumerable<SingleClause> rawRules)
        {
            var steps = rawRules.Where(c => c is Step);
            Entity mech = EntityBuilder.BuildNakedMech(label, player, pilot, new Guidebook(rawRules));
            foreach (Step step in steps)
            {
                step.ApplyStep(mech);
            }
            return mech;
        }

        public static void RandomMutation(Individual<SingleClause> mutant, Random rand)
        {
            mutant.SetGene(rand.Next(mutant.ChromosomeSize), AIUtils.geneList.SelectRandomGene(rand));
        }

        public static bool IsSurvivor(Population<SingleClause> population, Individual<SingleClause> survivor)
        {
            // Kill off everything less than average fitness
            var avg = population.InspectIndividuals().Average(i => i.Fitness);
            return survivor.Fitness >= avg;
        }

        public static int SimpleArenaFitness(Individual<SingleClause> individual, Random rand, int mapRange=3)
        {
            //Console.WriteLine("INVOKING SIMPLE ARENA FITNESS");
            int maxTicks = 25000;
            Entity individualMech = AssembleMech("UnpilotedFitnessMech", false, new Entity(), individual.InspectGenes());
            var rogueRand = new RogueSharp.Random.DotNetRandom(rand.Next()); // TODO: Well this is all kinds of silly!
            var enemyMech = EntityBuilder.BuildRandomMech("UnpilotedEntityMech", false, rogueRand);

            var arena = ArenaBuilder.BuildArena(50, 50, "0", rand.Next(mapRange).ToString(), rand.Next(),
                individualMech, enemyMech);
            arena.RunArena(maxTicks);

            // TODO: Your roulette function can't deal with zero/negative returns here!
            if (arena.IsMatchEnded() && arena.Mech1.EntityID == arena.WinnerID())
                return 5 + arena.Mech1.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;
            else if (arena.IsMatchEnded())
                return 1;
            else
                return 5;
        }
    }
}

