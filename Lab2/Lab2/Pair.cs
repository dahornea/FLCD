using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2bun
{
    public class Pair<First, Second>
    {
        public First _first { get; }
        public Second _second { get; }

        public Pair(First first, Second second)
        {
            _first = first;
            _second = second;
        }

        public override string ToString()
        {
            return $"Pair{{ first = {_first}, second = {_second} }}";
        }
    }

}
