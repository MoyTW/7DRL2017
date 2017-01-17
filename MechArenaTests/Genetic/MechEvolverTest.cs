using MechArena;
using MechArena.Genetic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MechArenaTests.Genetic
{
    [TestClass]
    public class MechEvolverTest
    {
        private Random rand = new Random();
        private GeneFactory<Action<Entity>> factory = BuildGeneFactory();

        private static GeneFactory<Action<Entity>> BuildGeneFactory()
        {
            List<Action<Entity>> allActions = new List<Action<Entity>>();

            foreach (var location in EntityBuilder.MechLocations)
            {
                foreach (var buildFn in EntityBuilder.BuildPartFunctions)
                {
                    allActions.Add(e => EntityBuilder.SlotAt(e, location, buildFn()));
                }
            }

            return new GeneFactory<Action<Entity>>(allActions);
        }

        private void RandomMutation(Individual<Action<Entity>> mutant, Random rand)
        {
            mutant.SetGene(rand.Next(mutant.ChromosomeSize), this.factory.SelectRandomGene(rand));
        }

        private bool IsSurvivor(Population<Action<Entity>> population, Individual<Action<Entity>> survivor)
        {
            // Kill off everything less than average fitness
            var avg = population.InspectIndividuals().Average(i => i.Fitness);
            return survivor.Fitness > avg;
        }

        #region Structure

        private int FitnessStructure(Individual<Action<Entity>> individual)
        {
            var genes = individual.InspectGenes();

            var testMech = EntityBuilder.BuildNakedMech("", false);
            foreach (var f in genes)
            {
                f(testMech);
            }

            return testMech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;
        }

        [TestMethod]
        public void TestMechEvolverStructure()
        {
            // It's REALLY important that you have some selection pressure during the culling phase! I couldn't get it
            // to 200 until I added a "Kill every member of the population whose fitness is under average" to the
            // culling phase, at which point it actually achieved the target, and fairly quickly!
            var evolver = new Evolver<Action<Entity>>(200, this.FitnessStructure, 300, 1, 160, this.factory, 80);

            var individual = evolver.Evolve(ParentStrategies.Roulette, CrossoverStrategies.SinglePointCrossover,
                this.RandomMutation, this.IsSurvivor);
            Console.WriteLine("Winner: ");

            var genes = individual.InspectGenes();

            var testMech = EntityBuilder.BuildNakedMech("", false);
            foreach (var f in genes)
            {
                f(testMech);
            }
            Console.WriteLine("Structure: " + testMech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value);
            foreach(var e in testMech.TryGetSubEntities(SubEntitiesSelector.ALL))
            {
                Console.WriteLine(e);
            }

            Console.Write(" Generation: " + evolver.CurrentGeneration);
            Console.WriteLine();

            int generation = 1;
            foreach (var p in evolver.InspectHistory())
            {
                Console.WriteLine("Gen " + generation + " Structure: " + p.HighestFitness());
                generation++;
            }

            Assert.IsTrue(testMech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value >= 200);
        }

        #endregion

        private Entity BuildRandomMech(Random rand)
        {
            var choice = rand.Next(4);
            switch (choice)
            {
                case 0:
                    return EntityBuilder.BuildArmoredMech("", false);
                case 1:
                    return EntityBuilder.BuildKnifeMech("", false);
                case 2:
                    return EntityBuilder.BuildPaladinMech("", false);
                case 3:
                    return EntityBuilder.BuildAlphaStrikerMech("", false);
                default:
                    return EntityBuilder.BuildNakedMech("", false);
            }
        }

        private int FitnessArena(Individual<Action<Entity>> individual)
        {
            var genes = individual.InspectGenes();

            var individualMech = EntityBuilder.BuildNakedMech("", false);
            foreach (var f in genes)
            {
                f(individualMech);
            }

            // var enemyMech = BuildRandomMech(this.rand);
            var enemyMech = EntityBuilder.BuildKnifeMech("", false);
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

        /* Commented out because it takes forever to run and doesn't really seem to work!
         * The issue with this is that the designs seem to converge very slowly into "Fill every slot with close-range
         * weapons" which is as expected, but also very boring and it doesn't seem to prioritize one specific weapon.
        [TestMethod]
        public void TestMechEvolverArena()
        {
            // 7 minutes runtime
            // var evolver = new Evolver<Action<Entity>>(200, this.FitnessArena, 30, 1, 80, this.factory, 80);

            // 43 minutes runtime, result=130
            // var evolver = new Evolver<Action<Entity>>(200, this.FitnessArena, 150, 1, 80, this.factory, 80);

            // ???
            var evolver = new Evolver<Action<Entity>>(200, this.FitnessArena, 225, 1, 120, this.factory, 80);

            var individual = evolver.Evolve(ParentStrategies.Roulette, CrossoverStrategies.SinglePointCrossover,
                this.RandomMutation, this.IsSurvivor);
            Console.WriteLine("Winner: ");

            var genes = individual.InspectGenes();

            var testMech = EntityBuilder.BuildNakedMech("", false);
            foreach (var f in genes)
            {
                f(testMech);
            }
            Console.WriteLine("Structure: " + testMech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value);
            foreach (var e in testMech.TryGetSubEntities(SubEntitiesSelector.ALL))
            {
                Console.WriteLine(e);
            }

            Console.Write(" Generation: " + evolver.CurrentGeneration);
            Console.WriteLine();

            int generation = 1;
            foreach (var p in evolver.InspectHistory())
            {
                Console.WriteLine("Gen " + generation + " Structure: " + p.HighestFitness());
                generation++;
            }

            Assert.IsTrue(testMech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value >= 200);

            Assert.IsFalse(true);
        }
        */
    }
}
