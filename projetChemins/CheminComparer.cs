using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace algoDarwin
{
    class CheminComparer : IEqualityComparer<Chemin>
    {
        public bool Equals(Chemin x, Chemin y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Chemin obj)
        {
            return 0;
        }
    }
}
