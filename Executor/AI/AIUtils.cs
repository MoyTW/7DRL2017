using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Executor.Genetic;
using Executor.AI.Combat;
using Executor.AI.Hanger;

namespace Executor.AI
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

        private static int SingleArena(Individual<SingleClause> individual, Random rand, int mapRange)
        {
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

        public static int SimpleArenaFitness(Individual<SingleClause> individual, Random rand, int mapRange=3, int numMatches=5)
        {
            int score = 0;

            for (int i = 0; i < numMatches; i++)
            {
                score += SingleArena(individual, rand, mapRange);
            }

            Console.Write(".");

            return score;
        }
    }
}

