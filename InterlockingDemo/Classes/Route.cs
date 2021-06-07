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
        /// 联锁表中的道岔检查条件
        /// </summary>
        public string SwitchString;
        /// <summary>
        /// 联锁表中的区段条件
        /// </summary>
        public string SectionString;
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
        /// <summary>
        /// 由软件自动生成的道岔检查条件
        /// </summary>
        public string GeneratedSwitchString;
        /// <summary>
        /// 由软件自动生成的区段条件
        /// </summary>
        public string GeneratedSectionString;

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
                    //背对道岔，根据背向开通的位置探查
                    var duixiang_dingwei = _switch.ConnEquipNames.Where(m => m.Item1.Contains("对向定位开向")).ToList()[0].Item2;
                    var duixiang_fanwei = _switch.ConnEquipNames.Where(m => m.Item1.Contains("对向反位开向")).ToList()[0].Item2;
                    if (duixiang_dingwei != null)
                    {
                        string next_point_dingwei = duixiang_dingwei;
                        GenEquipmentString(structure, string_list_return_tree, index, next_point_dingwei, end_point, direction);
                    }
                    else if (duixiang_fanwei != null)
                    {
                        string next_point_fanwei = duixiang_fanwei;
                        GenEquipmentString(structure, string_list_return_tree, index, next_point_fanwei, end_point, direction);
                    }
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
                        //该道岔迎面反位方向的下一个设备名称
                        string yingmian_fanwei_name = _switch.ConnEquipNames.Where(m => m.Item1 == "迎面反位开向").ToList()[0].Item2;
                        //走的是迎面反位
                        if (Route.FormateStringIntoEquipmentName(yingmian_fanwei_name) == equip_string_list[i + 1])
                        {
                            str = string.Format("({0})、", _switch.Name);
                        }

                        //该道岔迎面定位方向的下一个设备名称
                        string yingmian_dingwei_name = _switch.ConnEquipNames.Where(m => m.Item1 == "迎面定位开向").ToList()[0].Item2;
                        //走的是迎面定位
                        if(Route.FormateStringIntoEquipmentName(yingmian_dingwei_name) == equip_string_list[i + 1])
                        {
                            str = string.Format("{0}、", _switch.Name);
                        }

                        //该道岔对向定位方向的下一个设备名称
                        var duixiang_dingwei = _switch.ConnEquipNames.Where(m => m.Item1 == "对向定位开向").ToList()[0].Item2;
                        if (duixiang_dingwei != null)
                        {
                            //走的是对向定位
                            if (Route.FormateStringIntoEquipmentName(duixiang_dingwei) == equip_string_list[i + 1])
                            {
                                str = string.Format("{0}、", _switch.Name);
                            }
                        }

                        //该道岔对向反位方向的下一个设备名称
                        var duixiang_fanwei = _switch.ConnEquipNames.Where(m => m.Item1 == "对向反位开向").ToList()[0].Item2;
                        if (duixiang_fanwei != null)
                        {
                            //走的是对向反位
                            if (Route.FormateStringIntoEquipmentName(duixiang_fanwei) == equip_string_list[i + 1])
                            {
                                str = string.Format("({0})、", _switch.Name);
                            }
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
        /// 根据EquipmentString获取其中经过的区段
        /// </summary>
        /// <param name="structure"></param>
        /// <param name="string_list_return_tree"></param>
        /// <returns></returns>
        public static string GenSectionString(Structure structure, List<List<string>> string_list_return_tree)
        {
            List<string> section_names = new List<string>();
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
            for (int i = 0; i < equip_string_list.Count;) 
            {
                //当前设备名称
                string curr_equip_name = equip_string_list[i];
                //已包含的设备名称
                List<string> included_equip_names = new List<string>();
                //找出以该设备为端头的区段
                var sections = structure.Sections.Values.Where(section => {
                    bool flag = false;
                    List<string> begin_equip = new List<string>();
                    List<string> end_equip = new List<string>();
                    //如果始端为绝缘节，则添加描述该绝缘节的两个设备名
                    if (section.BeginFrom.Contains("绝缘节"))
                    {
                        string temp_string = section.BeginFrom.Replace("绝缘节", "");
                        var strs = temp_string.Split('+');
                        foreach (string str in strs)
                        {
                            begin_equip.Add(str);
                        }
                    }
                    else
                    {
                        begin_equip.Add(Route.FormateStringIntoEquipmentName(section.BeginFrom));
                    }

                    //如果终端为绝缘节，则添加描述该绝缘节的两个设备名
                    if (section.EndAt.Contains("绝缘节"))
                    {
                        string temp_string = section.EndAt.Replace("绝缘节", "");
                        var strs = temp_string.Split('+');
                        foreach (string str in strs)
                        {
                            begin_equip.Add(str);
                        }
                    }
                    else
                    {
                        begin_equip.Add(Route.FormateStringIntoEquipmentName(section.EndAt));
                    }

                    foreach (var begin_equip_name in begin_equip)
                    {
                        if (begin_equip_name == curr_equip_name)
                        {
                            flag = true;
                            return flag;
                        }
                    }
                    foreach (var end_equip_name in end_equip)
                    {
                        if (end_equip_name == curr_equip_name)
                        {
                            flag = true;
                            return flag;
                        }
                    }
                    return flag;
                }).ToList();
                //找出secions中包含curr_equip_name后续设备名称的section
                bool finded = false;
                for (int index = 0; index < sections.Count && !finded; index++)
                {
                    Section section = sections[index];
                    List<string> section_contains = new List<string>();
                    if (section.Name.Contains("-"))
                    {
                        string first_term = section.Name.Split('-').ToList()[0];
                        section_contains = section_names.Where(m => Route.GetFormmatedSectionName(m) ==
                        Route.GetFormmatedSectionName(first_term)).ToList();
                    }
                    //对于添加了某DG-定位，就不添加某DG-反位
                    if (section_contains.Count > 0)
                    {
                        continue;
                    }
                    List<string> begin_equip = new List<string>();
                    List<string> end_equip = new List<string>();
                    //如果始端为绝缘节，则添加描述该绝缘节的两个设备名
                    if (section.BeginFrom.Contains("绝缘节"))
                    {
                        string temp_string = section.BeginFrom.Replace("绝缘节", "");
                        var strs = temp_string.Split('+');
                        foreach (string str in strs)
                        {
                            begin_equip.Add(str);
                        }
                    }
                    else
                    {
                        begin_equip.Add(Route.FormateStringIntoEquipmentName(section.BeginFrom));
                    }

                    //如果终端为绝缘节，则添加描述该绝缘节的两个设备名
                    if (section.EndAt.Contains("绝缘节"))
                    {
                        string temp_string = section.EndAt.Replace("绝缘节", "");
                        var strs = temp_string.Split('+');
                        foreach (string str in strs)
                        {
                            end_equip.Add(str);
                        }
                    }
                    else
                    {
                        end_equip.Add(Route.FormateStringIntoEquipmentName(section.EndAt));
                    }
    
                    //检查begin_equip
                    for (int j = i + 1; j < equip_string_list.Count && !finded; j++)
                    {
                        string following_equip_name = equip_string_list[j];
                        foreach (var begin_equip_name in begin_equip)
                        {
                            if (begin_equip_name == following_equip_name)
                            {
                                string added_sec_name = Route.GetFormmatedSectionName(section.Name);
                                section_names.Add(added_sec_name);
                                finded = true;
                                //更新下一次查找的起点
                                if (!IsEquipSignal(curr_equip_name) && !IsEquipSignal(equip_string_list[j]))
                                {
                                    Switch _switch_1 = structure.Switches[curr_equip_name];
                                    //如果是双动道岔则不跳过
                                    if(_switch_1.DoubleSwitch== equip_string_list[j])
                                    {
                                        i = j;
                                    }
                                    else
                                    {
                                        i = j + 1;
                                    }
                                }
                                else
                                {
                                    i = j;
                                }
                                break;
                            }
                        }
                    }
                    //检查end_equip
                    for (int j = i + 1; j < equip_string_list.Count && !finded; j++)
                    {
                        string following_equip_name = equip_string_list[j];
                        foreach (var end_equip_name in end_equip)
                        {
                            if (end_equip_name == following_equip_name)
                            {
                                string added_sec_name = Route.GetFormmatedSectionName(section.Name);
                                section_names.Add(added_sec_name);
                                finded = true;
                                //更新下一次查找的起点
                                if (!IsEquipSignal(curr_equip_name) && !IsEquipSignal(equip_string_list[j]))
                                {
                                    Switch _switch_1 = structure.Switches[curr_equip_name];
                                    //如果是双动道岔则不跳过
                                    if (_switch_1.DoubleSwitch == equip_string_list[j])
                                    {
                                        i = j;
                                    }
                                    else
                                    {
                                        i = j + 1;
                                    }
                                }
                                else
                                {
                                    i = j;
                                }
                                break;
                            }
                        }
                    }
                }

                if (!finded)
                {
                    i++;
                }
            }

            //找到以最后一个设备(信号机)为始端的区段
            string last_equip_name = equip_string_list[equip_string_list.Count - 1];
            var last_secs = structure.Sections.Values.Where(m =>
            {
                bool last_flag = false;
                string begin_equip_name = Route.FormateStringIntoEquipmentName(m.BeginFrom);
                string end_equip_name = Route.FormateStringIntoEquipmentName(m.EndAt);
                string splited_section_name = Route.GetFormmatedSectionName(m.Name);
                if ((begin_equip_name == last_equip_name || end_equip_name == last_equip_name) &&
                !section_names.Contains(splited_section_name))
                {
                    last_flag = true;
                }
                return last_flag;
            }).ToList();
            if (last_secs.Count != 0)
            {
                Section last_sec = last_secs[0];
                section_names.Add(Route.GetFormmatedSectionName(last_sec.Name));
            }

            foreach(string str in section_names)
            {
                str_return += str + "、";
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

        /// <summary>
        /// 判断是否为信号机
        /// </summary>
        /// <param name="equip_name"></param>
        /// <returns></returns>
        public static bool IsEquipSignal(string equip_name)
        {
            bool is_signal = false;
            if (equip_name.Contains("S") || equip_name.Contains("X") || equip_name.Contains("D"))
            {
                is_signal = true;
            }
            return is_signal;
        }

        /// <summary>
        /// 不包含字符“定位”、“反位”的区段名字
        /// </summary>
        /// <param name="section_name"></param>
        /// <returns></returns>
        public static string GetFormmatedSectionName(string section_name)
        {
            string splited_section_name = section_name.Replace("-定位", "");
            splited_section_name = splited_section_name.Replace("-反位", "");
            return splited_section_name;
        }
    }
}
