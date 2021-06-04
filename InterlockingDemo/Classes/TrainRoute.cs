using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    /// <summary>
    /// 列车进路
    /// </summary>
    class TrainRoute:Route
    {
        /// <summary>
        /// 方向（北京方向，武汉方向）
        /// </summary>
        public string Direction;
        /// <summary>
        /// 类型（接车，发车，通过）
        /// </summary>
        public string TrainRouteType;
        /// <summary>
        /// 从哪个股道/信号机来，或者去往哪个股道/信号机
        /// </summary>
        public string HeadToOrDepartFrom;

        public TrainRoute()
        {

        }
    }
}
