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
    public class Route
    {
        /// <summary>
        /// 进路名字（始端按钮-终端按钮）
        /// </summary>
        public string Name
        {
            get
            {
                return StartButton + "-" + EndButton;
            }
        }

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
        /// <summary>
        /// 进路的朝向
        /// </summary>
        public string HeadToDirection;

        public Route()
        {
            Switches = new List<string>();
            ConflictSignals = new List<string>();
            Sections = new List<string>();
        }

        public static void GenSectionString(Structure structure, string string_return, string start_point, string end_point, string direction)
        {
            //判断起始点类型
            //信号机
            if (start_point.Contains("X") || start_point.Contains("S") || start_point.Contains("D"))
            {
                Signal signal = structure.Signals[start_point];
                //查找

            }
        }

        /// <summary>
        /// 格式化字符串为设备名字
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string FormateStringIntoEquipmentName(string input)
        {
            string output = string.Copy(input);
            output = output.Replace("LA", "");
            output = output.Replace("TA", "");
            output = output.Replace("DA", "");
            output = output.Replace("A", "");
            output = output.Replace("信号机-", "");
            output = output.Replace("道岔-", "");
            return output;
        }
    }
}
