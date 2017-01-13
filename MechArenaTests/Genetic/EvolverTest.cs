using System;
using System.Linq;
using MechArena.Genetic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MechArenaTests.Genetic
{
    [TestClass]
    public class EvolverTest
    {
        private Random rand = new Random();
        private string target = "Batman";
        private int chromosomeSize = 6;

        private class Gene : IGene
        {
            private const string possibleGeneValues = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            public char Value { get; }

            public Gene(char value)
            {
                this.Value = value;
            }

            public IGene RandomGene(Random rand)
            {
                var i = rand.Next(Gene.possibleGeneValues.Count());
                return new Gene(Gene.possibleGeneValues.ElementAt(i));
            }

            public override string ToString()
            {
                return this.Value.ToString();
            }
        }

        private int Fitness(Individual individual)
        {
            int fitness = 0;

            var genes = individual.InspectGenes().Cast<Gene>();
            for (int i = 0; i < this.target.Count(); i++)
            {
                if (target.ElementAt(i) == genes.ElementAt(i).Value)
                    fitness++;
            }

            return fitness;
        }

        private Individual Roulette(Population pop, Random rand)
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

        private Individual SinglePointCrossover(Individual parentA, Individual parentB, Random rand)
        {
            Individual child = new Individual(parentA);

            int crossoverAt = rand.Next(this.chromosomeSize);
            for (int i = crossoverAt; i < this.chromosomeSize; i++)
            {
                child.SetGene(i, parentB.GetGene(i));
            }

            return child;
        }

        private void RandomMutation(Individual mutant, Random rand)
        {
            int mutateAt = rand.Next(this.chromosomeSize);
            mutant.SetGene(mutateAt, mutant.GetGene(mutateAt).RandomGene(rand));
        }

        private bool IsSurvivor(Individual survivor)
        {
            return this.Fitness(survivor) >= 2;
        }

        [TestMethod]
        public void TestEvolver()
        {
            var e = new Evolver(this.target.Count(), 150, 0.25, 50, new Gene('a'), this.chromosomeSize);
            var individual = e.Evolve(this.Fitness, this.Roulette, this.SinglePointCrossover, this.RandomMutation, this.IsSurvivor);
            Console.WriteLine("Winner: ");
            foreach (Gene g in individual.InspectGenes().Cast<Gene>())
                Console.Write(g.Value);
            Console.Write(" Generation: " + e.CurrentGeneration);
            Console.WriteLine();

            int generation = 1;
            foreach(var p in e.InspectHistory())
            {
                Console.WriteLine("Next Population, Generation " + generation);
                foreach(var i in p.InspectIndividuals())
                {
                    Console.Write(i);
                    Console.Write(" ");
                }
                Console.WriteLine();
                generation++;
            }

            Assert.AreEqual(this.target, individual.ToString());
        }
    }
}
