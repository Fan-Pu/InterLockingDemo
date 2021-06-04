using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    class ShuntRoute:Route
    {
        //起始调车信号机
        public string StartShuntSignal;
        //终点（股道号或信号机）
        public string Termination;

        public ShuntRoute()
        {

        }
    }
}
