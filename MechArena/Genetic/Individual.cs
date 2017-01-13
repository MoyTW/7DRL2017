using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public class Individual
    {
        public int ChromosomeSize { get; }
        private List<IGene> genes;

        public Individual(IGene gene, int chromosomeSize, Random rand)
        {
            this.ChromosomeSize = chromosomeSize;

            this.genes = new List<IGene>(chromosomeSize);
            for(int i = 0; i < chromosomeSize; i++)
            {
                this.genes.Add(gene.RandomGene(rand));
            }
        }

        public Individual(Individual parent)
        {
            this.ChromosomeSize = parent.ChromosomeSize;
            this.genes = new List<IGene>(parent.InspectGenes());
        }

        public IList<IGene> InspectGenes()
        {
            return this.genes.AsReadOnly();
        }

        public IGene GetGene(int idx)
        {
            return this.genes[idx];
        }

        public void SetGene(int idx, IGene gene)
        {
            this.genes[idx] = gene;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var gene in this.genes)
            {
                builder.Append(gene.ToString());
            }
            return builder.ToString();
        }
    }
}
