using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuchMatSimplex
{
    public class SaveData
    {
        public List<List<(int, int)>> A { get; set; } = new();
        public List<(int, int)> B { get; set; } = new();
        public List<string> znak { get; set; } = new();
        public List<(double, double)> delta { get; set; } = new();
        public List<int> basis { get; set; } = new();
        public List<int> F {  get; set; } = new();
        public int k { get; set; }

    }
}
