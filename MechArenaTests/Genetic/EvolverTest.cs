using System;
using System.Linq;
using MechArena.Genetic;
using NUnit.Framework;

namespace MechArenaTests.Genetic
{
    [TestFixture]
    public class EvolverTest
    {
        private Random rand = new Random();
        private string target = "Batman";
        private int chromosomeSize = 6;
        private GeneFactory<char> factory = new GeneFactory<char>("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ");

        private int Fitness(Individual<char> individual)
        {
            int fitness = 0;

            var genes = individual.InspectGenes();
            for (int i = 0; i < this.target.Count(); i++)
            {
                if (target.ElementAt(i) == genes.ElementAt(i))
                    fitness++;
            }

            return fitness;
        }

        private void RandomMutation(Individual<char> mutant, Random rand)
        {
            mutant.SetGene(rand.Next(this.chromosomeSize), this.factory.SelectRandomGene(rand));
        }

        private bool IsSurvivor(Population<char> population, Individual<char> survivor)
        {
            return this.Fitness(survivor) >= 2;
        }

        [Test]
        public void TestEvolver()
        {
            var e = new Evolver<char>(this.target.Count(), this.Fitness, 150, 0.25, 50, this.factory, this.chromosomeSize);
            var individual = e.Evolve(ParentStrategies.Roulette, CrossoverStrategies.SinglePointCrossover,
                this.RandomMutation, this.IsSurvivor);
            Console.WriteLine("Winner: ");
            foreach (char g in individual.InspectGenes())
                Console.Write(g);
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
