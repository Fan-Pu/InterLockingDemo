using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    class SwitchErrorItem
    {
        public string ID { get; set; }

        public string PrimalSwitchString { get; set; }

        public string GeneratedSwitchString { get; set; }

        public SwitchErrorItem(string id, string primal, string generated)
        {
            ID = id;
            PrimalSwitchString = primal;
            GeneratedSwitchString = generated;
        }
    }
}
