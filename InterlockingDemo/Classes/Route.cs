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
        //始端按钮
        public string StartButton;
        //终端按钮
        public string EndButton;
        //信号机名称
        public string SignalName;
        //信号机显示
        public string SignalLight;
        //道岔
        public List<string> Switches;
        //敌对信号
        public List<string> ConflictSignals;
        //轨道区段
        public List<string> Sections;
        //迎面进路（列车）
        public string HeadonTrainRoute;
        //迎面进路（调车）
        public string HeadonShuntRoute;

        public Route()
        {
            Switches = new List<string>();
            ConflictSignals = new List<string>();
            Sections = new List<string>();
        }
    }
}
