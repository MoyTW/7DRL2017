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

        private Individual<Action<Entity>> Roulette(Population<Action<Entity>> pop, Random rand)
        {
            int totalweight = pop.InspectIndividuals().Sum(i => this.Fitness(i));
            int choice = rand.Next(totalweight);
            int weightIndex = 0;

            foreach (var individual in pop.InspectIndividuals())
            {
                weightIndex += this.Fitness(individual);

                if (weightIndex > choice)
                    return individual;
            }

            return null;
        }

        private Individual<Action<Entity>> SinglePointCrossover(Individual<Action<Entity>> parentA,
            Individual<Action<Entity>> parentB, Random rand)
        {
            Individual<Action<Entity>> child = new Individual<Action<Entity>>(parentA);

            int crossoverAt = rand.Next(parentA.ChromosomeSize);
            for (int i = crossoverAt; i < parentA.ChromosomeSize; i++)
            {
                child.SetGene(i, parentB.GetGene(i));
            }

            return child;
        }

        private void RandomMutation(Individual<Action<Entity>> mutant, Random rand)
        {
            mutant.SetGene(rand.Next(mutant.ChromosomeSize), this.factory.SelectRandomGene(rand));
        }

        private bool IsSurvivor(Individual<Action<Entity>> survivor)
        {
            return true;
        }

        [TestMethod]
        public void TestMechEvolver()
        {
            var evolver = new Evolver<Action<Entity>>(200, 60, 0.25, 20, this.factory, 80);
            var individual = evolver.Evolve(this.Fitness, this.Roulette, this.SinglePointCrossover, this.RandomMutation, this.IsSurvivor);
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
                Console.WriteLine("Gen " + generation + " Structure: " + p.HighestFitness(this.Fitness));
                generation++;
            }
            
            Assert.IsTrue(false);
        }
    }
}
