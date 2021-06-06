using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    public abstract class Equipment
    {
        public string Name { get; set; }

        public List<Tuple<string, string>> ConnEquipNames { get; set; } 

        public Equipment()
        {
            ConnEquipNames = new List<Tuple<string, string>>();
        }
    }
}
