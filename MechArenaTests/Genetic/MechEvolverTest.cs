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

        private int Fitness(Individual<Action<Entity>> individual)
        {
            var genes = individual.InspectGenes();

            var testMech = EntityBuilder.BuildNakedMech("", false);
            foreach(var f in genes)
            {
                f(testMech);
            }

            return testMech.TryGetAttribute(EntityAttributeType.STRUCTURE).Value;
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

        [TestMethod]
        public void TestMechEvolver()
        {
            // It's REALLY important that you have some selection pressure during the culling phase! I couldn't get it
            // to 200 until I added a "Kill every member of the population whose fitness is under average" to the
            // culling phase, at which point it actually achieved the target, and fairly quickly!
            var evolver = new Evolver<Action<Entity>>(200, this.Fitness, 300, 1, 160, this.factory, 80);

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
    }
}
