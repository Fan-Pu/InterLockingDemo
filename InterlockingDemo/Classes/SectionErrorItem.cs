using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    class SectionErrorItem
    {
        public string ID { get; set; }

        public string PrimalSectionString { get; set; }

        public string GeneratedSectionString { get; set; }

        public SectionErrorItem(string id, string primal, string generated)
        {
            ID = id;
            PrimalSectionString = primal;
            GeneratedSectionString = generated;
        }
    }
}
