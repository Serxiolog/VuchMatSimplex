using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VuchMatSimplex
{
    public class Values
    {
        public int n {  get; set; }
        public int m { get; set; }
        public List<List<int>> aRaw { get; set; } = new();
        public List<int> bRaw { get; set; } = new();
        public List<string> znak { get; set; } = new();
        public List<int> fRaw { get; set; } = new(); 
        public bool isMaxim { get; set; }
    }
}
