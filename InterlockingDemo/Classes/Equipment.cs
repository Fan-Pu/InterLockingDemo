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

        public Tuple<string,string>[] ConnEquipNames { get; set; } 

        public Equipment()
        {
            ConnEquipNames = new Tuple<string, string>[2];
        }
    }
}
