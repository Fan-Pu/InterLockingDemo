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
        /// 进路序号
        /// </summary>
        public int ID;

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
        /// <summary>
        /// 进路的由哪个方向来
        /// </summary>
        public string HeadFromDirection;

        public Route()
        {
            Switches = new List<string>();
            ConflictSignals = new List<string>();
            Sections = new List<string>();
        }

        /// <summary>
        /// 生成该进路途径的所有设备名称
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="string_list_return_tree"></param>
        /// <param name="index"></param>
        /// <param name="start_point"></param>
        /// <param name="end_point"></param>
        /// <param name="direction"></param>
        public static void GenEquipmentString(Structure structure, List<List<string>> string_list_return_tree, int index,
            string start_point, string end_point, string direction)
        {
            List<string> to_be_returned = new List<string>();
            if (string_list_return_tree.Count == 0)
            {
                index = 0;
                string_list_return_tree.Add(to_be_returned);
            }
            else
            {
                to_be_returned = string_list_return_tree[index];
            }
            //判断起始点类型
            //如果到达终点则返回
            start_point = Route.FormateStringIntoEquipmentName(start_point);
            if (start_point == end_point)
            {
                to_be_returned.Add(end_point);
                return;
            }
            //终点不为安全线、编组线、武汉/北京方向，但下一个点为其中之一，则返回
            else if ((start_point.Contains("安全线") || start_point.Contains("编组线") || start_point.Contains("方向")))
            {
                to_be_returned.Clear();
                return;
            }
            //信号机
            else if (start_point.Contains("X") || start_point.Contains("S") || start_point.Contains("D"))
            {
                start_point = start_point.Replace("信号机", "");
                to_be_returned.Add(start_point);
                Signal signal = structure.Signals[start_point];
                //查找
                string next_start_point = signal.ConnEquipNames.Where(m => m.Item1.Contains(direction)).ToList()[0].Item2;
                GenEquipmentString(structure, string_list_return_tree, index, next_start_point, end_point, direction);
            }
            //道岔
            else
            {
                Switch _switch = structure.Switches[start_point];
                to_be_returned.Add(start_point);
                //判断此时迎面朝向道岔还是背向道岔
                if (direction == _switch.Direction)
                {
                    //迎面朝向道岔，同时试探该道岔的定反位
                    string next_point_dingwei = _switch.ConnEquipNames.Where(m => m.Item1.Contains("迎面定位开向")).ToList()[0].Item2;
                    string next_point_fanwei = _switch.ConnEquipNames.Where(m => m.Item1.Contains("迎面反位开向")).ToList()[0].Item2;
                    //新增一颗子树
                    List<string> sub_tree = new List<string>(to_be_returned);
                    //定位探寻原树
                    GenEquipmentString(structure, string_list_return_tree, index, next_point_dingwei, end_point, direction);
                    //反位探寻新的子树
                    string_list_return_tree.Add(sub_tree);
                    int next_index = string_list_return_tree.Count - 1;
                    GenEquipmentString(structure, string_list_return_tree, next_index, next_point_fanwei, end_point, direction);
                }
                else
                {
                    //背对道岔，只试探道岔定位
                    string next_point_dingwei = _switch.ConnEquipNames.Where(m => m.Item1.Contains("对向定位开向")).ToList()[0].Item2;
                    GenEquipmentString(structure, string_list_return_tree, index, next_point_dingwei, end_point, direction);
                }
            }
        }

        /// <summary>
        /// 根据EquipmentString获取其中经过的道岔条件
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="string_list_return_tree"></param>
        /// <returns></returns>
        public static string GenSwitchString(Structure structure, List<List<string>> string_list_return_tree)
        {
            string str_return = "";
            List<string> equip_string_list = null;
            foreach (var string_list in string_list_return_tree)
            {
                if (string_list.Count != 0)
                {
                    equip_string_list = new List<string>(string_list);
                    break;
                }
            }
            //只留下道岔设备
            List<string> switch_list = new List<string>(equip_string_list);
            switch_list.RemoveAll(m => m.Contains("X") || m.Contains("S") || m.Contains("D") || m.Contains("方向") || m.Contains("线"));

            for (int i = 0; i < equip_string_list.Count;)
            {
                string equip_name = equip_string_list[i];
                //当前设备为道岔
                if (switch_list.Contains(equip_name))
                {
                    Switch _switch = structure.Switches[equip_name];
                    //是双动道岔
                    if (_switch.DoubleSwitch != "")
                    {
                        string str = "";
                        //走的是反位
                        if (equip_string_list[i + 1] == _switch.DoubleSwitch)
                        {
                            if (int.Parse(_switch.Name) < int.Parse(_switch.DoubleSwitch))
                            {
                                str = string.Format("({0}/{1})、", _switch.Name, _switch.DoubleSwitch);
                            }
                            else
                            {
                                str = string.Format("({0}/{1})、", _switch.DoubleSwitch, _switch.Name);
                            }
                            i = i + 2;
                        }
                        //走的是定位
                        else
                        {
                            if (int.Parse(_switch.Name) < int.Parse(_switch.DoubleSwitch))
                            {
                                str = string.Format("{0}/{1}、", _switch.Name, _switch.DoubleSwitch);
                            }
                            else
                            {
                                str = string.Format("{0}/{1}、", _switch.DoubleSwitch, _switch.Name);
                            }
                            i++;
                        }
                        str_return += str;
                    }
                    //单动道岔
                    else
                    {
                        string str = "";
                        //该道岔反位方向的下一个设备名称
                        string fanwei_name = _switch.ConnEquipNames.Where(m => m.Item1 == "迎面反位开向").ToList()[0].Item2;
                        //走的是反位
                        if (Route.FormateStringIntoEquipmentName(fanwei_name) == equip_string_list[i + 1])
                        {
                            str = string.Format("({0})、", _switch.Name);
                        }
                        //走的是定位
                        else
                        {
                            str = string.Format("{0}、", _switch.Name);
                        }
                        str_return += str;
                        //跳过该双动道岔的另一端
                        i++;
                    }
                }
                //当前设备为信号机
                else
                {
                    //如果该信号机后面还有设备
                    if (i != equip_string_list.Count - 1)
                    {
                        string next_equip = equip_string_list[i + 1];
                        //如果下一个设备是道岔
                        if (switch_list.Contains(next_equip))
                        {
                            Switch next_switch = structure.Switches[next_equip];
                            //跳过双动道岔
                            if (next_switch.DoubleSwitch != "")
                            {
                                i++;
                            }
                            //单动道岔
                            else
                            {
                                string yingmian_dingwei =
                                    Route.FormateStringIntoEquipmentName(next_switch.ConnEquipNames.Where(m => m.Item1 == "迎面定位开向").ToList()[0].Item2);
                                string yingmian_fanwei =
                                    Route.FormateStringIntoEquipmentName(next_switch.ConnEquipNames.Where(m => m.Item1 == "迎面反位开向").ToList()[0].Item2);
                                //经过道岔定位
                                if (equip_name == yingmian_dingwei)
                                {
                                    str_return += string.Format("{0}、", next_switch.Name);
                                }
                                //经过道岔反位
                                else
                                {
                                    str_return += string.Format("({0})、", next_switch.Name);
                                }
                                i = i + 2;
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }
                    else
                    {
                        i++;
                    }
                }
            }
            return str_return.Substring(0, str_return.Length - 1);
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
            output = output.Replace(" ", "");
            output = output.Replace("TA", "");
            output = output.Replace("DA", "");
            output = output.Replace("A", "");
            output = output.Replace("信号机", "");
            output = output.Replace("道岔", "");
            return output;
        }
    }
}
