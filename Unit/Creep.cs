using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaClosedAi.Unit
{
    class Creep
    {
        public double Hp { get; private set; }

        public Creep(double hp)
        {
            Hp = hp;
        }
    }
}
