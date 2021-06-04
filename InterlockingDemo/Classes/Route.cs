using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    /// <summary>
    /// 进路基类
    /// </summary>
    class Route
    {
        /// <summary>
        /// 始端按钮
        /// </summary>
        public string StartButton;
        /// <summary>
        /// 终端按钮
        /// </summary>
        public string EndButton;
        /// <summary>
        /// 信号机名称
        /// </summary>
        public string SignalName;
        /// <summary>
        /// 信号机显示
        /// </summary>
        public string SignalLight;
        /// <summary>
        /// 道岔
        /// </summary>
        public List<string> Switches;
        /// <summary>
        /// 敌对信号
        /// </summary>
        public List<string> ConflictSignals;
        /// <summary>
        /// 轨道区段
        /// </summary>
        public List<string> Sections;
        /// <summary>
        /// 迎面进路（列车）
        /// </summary>
        public string HeadonTrainRoute;
        /// <summary>
        /// 迎面进路（调车）
        /// </summary>
        public string HeadonShuntRoute;

        public Route()
        {
            Switches = new List<string>();
            ConflictSignals = new List<string>();
            Sections = new List<string>();
        }
    }
}
