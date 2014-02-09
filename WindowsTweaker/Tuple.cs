using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsTweaker
{
    class Tuple<X, Y> {
        public X x;
        public Y y;
        public Tuple(X x, Y y) {
            this.x = x;
            this.y = y;
        }
    }
}
