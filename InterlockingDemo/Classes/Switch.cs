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

        public Switch(string name, string double_switch, string direction, Tuple<string, string> yingmian_dingwei,
            Tuple<string, string> yingmian_fanwei, Tuple<string, string> duixiang_dingwei, Tuple<string, string> duixiang_fanwei)
        {
            Name = name;
            DoubleSwitch = double_switch;
            Direction = direction;
            ConnEquipNames.Add(yingmian_dingwei);
            ConnEquipNames.Add(yingmian_fanwei);
            ConnEquipNames.Add(duixiang_dingwei);
            ConnEquipNames.Add(duixiang_fanwei);
        }
    }
}
