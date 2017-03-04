using Executor;
using Executor.AI;
using Executor.AI.Combat;
using Executor.Genetic;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ExecutorTests.Genetic
{
    [TestFixture]
    public class AIEvolverTest
    {
        private Random rand = new Random();
        private GeneListing<SingleClause> geneList;

        [SetUp]
        public void Initialize()
        {
            BlueprintListing.LoadAllBlueprints();
            this.geneList = new GeneListing<SingleClause>(AIUtils.DeriveAllClauses());
        }

        private void RandomMutation(Individual<SingleClause> mutant, Random rand)
        {
            mutant.SetGene(rand.Next(mutant.ChromosomeSize), this.geneList.SelectRandomGene(rand));
        }

        private bool IsSurvivor(Population<SingleClause> population, Individual<SingleClause> survivor)
        {
            // Kill off everything less than average fitness
            var avg = population.InspectIndividuals().Average(i => i.Fitness);
            return survivor.Fitness > avg;
        }

        private int Fitness(Individual<SingleClause> individual)
        {
            Guidebook book = new Guidebook(individual.InspectGenes());

            Entity individualMech = EntityBuilder.BuildFastPistolMech("", false, book);
            // var enemyMech = BuildRandomMech(this.rand);
            var enemyMech = EntityBuilder.BuildSlowKnifeMech("", false);

            var arena = ArenaBuilder.BuildArena(50, 50, "0", this.rand.Next(3).ToString(), this.rand.Next(),
                individualMech, enemyMech);
            while (!arena.IsMatchEnded() && arena.CurrentTick < 25000)
            {
                arena.TryFindAndExecuteNextCommand();
            }

            if (arena.IsMatchEnded() && arena.Mech1.EntityID == arena.WinnerID())
                return arena.Mech1.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;
            else if (arena.IsMatchEnded())
                return 0;
            else
                return -10;
        }

        /*
        [Test]
        public void TestMechEvolverArena()
        {
            // 7 minutes runtime
            var evolver = new Evolver<SingleClause>(200, this.Fitness, 10, 1, 100, this.geneList, 100);

            Console.WriteLine("Start: " + DateTime.Now.ToString());
            var winner = evolver.Evolve(ParentStrategies.Roulette, CrossoverStrategies.SinglePointCrossover,
                this.RandomMutation, this.IsSurvivor);
            Console.WriteLine("Winner: " + winner);
            Console.WriteLine("End: " + DateTime.Now.ToString());

            foreach (var g in winner.InspectGenes())
            {
                Console.WriteLine(g.ToString());
            }

            Console.Write(" Generation: " + evolver.CurrentGeneration);
            Console.WriteLine();

            int generation = 1;
            foreach (var p in evolver.InspectHistory())
            {
                Console.WriteLine("Gen " + generation + " Structure: " + p.HighestFitness());
                generation++;
            }

            Assert.IsFalse(true);
        }
        */
    }
}

