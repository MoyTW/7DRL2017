using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MechArena.Genetic
{
    public interface IGene
    {
        IGene RandomGene(Random rand);
    }
}
