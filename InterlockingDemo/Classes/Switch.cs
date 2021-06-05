using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    public class Switch:Equipment
    {
        /// <summary>
        /// 双动道岔的另一端，如果为单动道岔此字段为空
        /// </summary>
        public string DoubleSwitch;

        /// <summary>
        /// 朝向的方向
        /// </summary>
        public string Direction;

        public Switch(string name, string double_switch, string direction, Tuple<string, string> conn_tuple_0,
            Tuple<string, string> conn_tuple_1)
        {
            Name = name;
            DoubleSwitch = double_switch;
            Direction = direction;
            ConnEquipNames[0] = conn_tuple_0;
            ConnEquipNames[1] = conn_tuple_1;
        }
    }
}
