using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    public class Signal:Equipment
    {
        /// <summary>
        /// 信号机类型（列车、调车）
        /// </summary>
        public string SignalType;

        /// <summary>
        /// 朝向的方向
        /// </summary>
        public string Direction;

        public Signal(string name, string signal_type, string direction, Tuple<string, string> conn_tuple_0, 
            Tuple<string, string> conn_tuple_1)
        {
            Name = name;
            SignalType = signal_type;
            Direction = direction;
            ConnEquipNames.Add(conn_tuple_0);
            ConnEquipNames.Add(conn_tuple_1);
        }
    }
}
