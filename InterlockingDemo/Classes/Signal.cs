using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    class Signal:Equipment
    {
        public string SignalType;

        public string Direction;

        public Signal(string name,string signal_type, string direction)
        {
            Name = name;
            SignalType = signal_type;
            Direction = direction;
        }
    }
}
