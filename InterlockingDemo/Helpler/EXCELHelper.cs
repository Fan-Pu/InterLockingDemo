using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using InterlockingDemo.Classes;
using Microsoft.Win32;
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace InterlockingDemo.Helpler
{
    class EXCELHelper
    {
        public static IWorkbook GetIWorkbook(FileStream fs)
        {
            List<Route> route_list = new List<Route>();
            //打开文件
            string localFilePath = "";
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = RootPathHelper.GetRootPath() + "\\Datas";
            fileDialog.Filter = "excel files|*.xls;*.xlsx";
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == true)
            {
                localFilePath = fileDialog.FileName.ToString();
                MessageBox.Show("打开成功！", "通知", MessageBoxButton.OK, MessageBoxImage.Information);

                //读取文件数据
                string fileExt = Path.GetExtension(localFilePath).ToLower();
                IWorkbook workbook;
                fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read);
                if (fileExt == ".xlsx")
                {
                    workbook = new XSSFWorkbook(fs);
                }
                else if (fileExt == ".xls")
                {
                    workbook = new HSSFWorkbook(fs);
                }
                else
                {
                    workbook = null;
                }
                return workbook;
            }
            else
            {
                throw new NullReferenceException();
            }
        }

        public static Dictionary<string, Route> ReadRouteEXCELFile()
        {
            Dictionary<string, Route> route_list = new Dictionary<string, Route>();
            FileStream fs = null;
            IWorkbook workbook = GetIWorkbook(fs);

            //获取数据表
            ISheet sheet = workbook.GetSheetAt(0);
            //表头
            IRow header = sheet.GetRow(sheet.FirstRowNum);
            IRow header_below = sheet.GetRow(1);
            Dictionary<int, string> columns = new Dictionary<int, string>();
            int last_resort_idx = 0;
            for (int i = 0; i < header.LastCellNum; i++)
            {
                ICell cell = header.GetCell(i);
                if (cell.IsMergedCell)
                {
                    ICell below_cell = header_below.GetCell(i);
                    string below_value = "";
                    if (below_cell.CellType == CellType.Blank)
                    {
                        below_value = (i - last_resort_idx).ToString();
                    }
                    else if (below_cell.CellType == CellType.String)
                    {
                        below_value = below_cell.StringCellValue;
                    }

                    if (cell.StringCellValue == "")
                    {
                        string value = header.GetCell(last_resort_idx).StringCellValue + "-" + below_value;
                        columns.Add(i, value);
                    }
                    else if (cell.StringCellValue != "")
                    {
                        string value = cell.StringCellValue + "-" + below_value;
                        columns.Add(i, value);
                        last_resort_idx = i;
                    }
                }
                else
                {
                    columns.Add(i, cell.StringCellValue);
                    last_resort_idx = i;
                }
            }

            Dictionary<int, int> last_resort_map = new Dictionary<int, int>();
            for (int i = 0; i < columns.Count; i++)
            {
                //初始化为0
                last_resort_map.Add(i, 0);
            }

            for (int i = 2; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                TrainRoute train_route = null;
                ShuntRoute shunt_route = null;
                bool is_initialized = false;
                foreach (var colum_idx in columns.Keys)
                {
                    string colum_name = columns[colum_idx];
                    if (!is_initialized)
                    {
                        if (colum_name == "方向-0")
                        {
                            //判断是列车进路还是调车进路
                            string str_value = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            if (str_value == "列车进路")
                            {
                                train_route = new TrainRoute();
                                is_initialized = true;
                            }
                            else if (str_value == "调车进路")
                            {
                                shunt_route = new ShuntRoute();
                                is_initialized = true;
                            }
                        }
                    }
                    else if (is_initialized)
                    {
                        if (colum_name == "方向-1")
                        {
                            if (train_route != null)
                            {
                                train_route.HeadFromDirection = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            }
                        }
                        else if (colum_name == "方向-2")
                        {
                            if (train_route != null)
                            {
                                train_route.TrainRouteType = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            }
                            else if (shunt_route != null)
                            {
                                shunt_route.StartShuntSignal = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            }
                        }
                        else if (colum_name == "进路")
                        {
                            string str = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            if (train_route != null)
                            {
                                if (train_route.TrainRouteType == "通过")
                                {
                                    str = str.Split('向').ToList()[1];
                                }
                                else
                                {
                                    str = str.Replace("由", "");
                                    str = str.Replace("至", "");
                                    str = str.Replace("股道", "G");
                                }
                                train_route.HeadToOrDepartFrom = str;
                            }
                            else if (shunt_route != null)
                            {
                                str = str.Replace("由", "");
                                str = str.Replace("至", "");
                                str = str.Replace("股道", "G");
                                shunt_route.Termination = str;
                            }
                        }
                        else if (colum_name == "排列进路按下按钮")
                        {
                            string str = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            var str_array = str.Split('、').ToArray();
                            if (train_route != null)
                            {
                                train_route.StartButton = str_array[0];
                                train_route.EndButton = str_array[1];
                            }
                            else if (shunt_route != null)
                            {
                                shunt_route.StartButton = str_array[0];
                                shunt_route.EndButton = str_array[1];
                            }
                        }
                        else if (colum_name == "信号机-名称")
                        {
                            string str = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            if (train_route != null)
                            {
                                train_route.SignalName = str;
                            }
                            else if (shunt_route != null)
                            {
                                shunt_route.SignalName = str;
                            }
                        }
                        else if (colum_name == "信号机-显示")
                        {
                            string str = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            if (train_route != null)
                            {
                                train_route.SignalLight = str;
                            }
                            else if (shunt_route != null)
                            {
                                shunt_route.SignalLight = str;
                            }
                        }
                        else if (colum_name == "道岔")
                        {
                            string str = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            var str_array = str.Split('、').ToArray();
                            if (train_route != null)
                            {
                                foreach (string s in str_array)
                                {
                                    train_route.Switches.Add(s);
                                }
                            }
                            else if (shunt_route != null)
                            {
                                foreach (string s in str_array)
                                {
                                    shunt_route.Switches.Add(s);
                                }
                            }
                        }
                        else if (colum_name == "敌对信号")
                        {
                            string str = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            var str_array = str.Split('、').ToArray();
                            if (train_route != null)
                            {
                                foreach (string s in str_array)
                                {
                                    train_route.ConflictSignals.Add(s);
                                }
                            }
                            else if (shunt_route != null)
                            {
                                foreach (string s in str_array)
                                {
                                    shunt_route.ConflictSignals.Add(s);
                                }
                            }
                        }
                        else if (colum_name == "轨道区段")
                        {
                            string str = GetCellStringValue(sheet, i, colum_idx, last_resort_map);
                            var str_array = str.Split('、').ToArray();
                            if (train_route != null)
                            {
                                foreach (string s in str_array)
                                {
                                    train_route.Sections.Add(s);
                                }
                            }
                            else if (shunt_route != null)
                            {
                                foreach (string s in str_array)
                                {
                                    shunt_route.Sections.Add(s);
                                }
                            }
                        }
                        else if (colum_name == "迎面进路-列车")
                        {
                            ICell cell = row.GetCell(colum_idx);
                            if (cell == null || cell.CellType != CellType.String)
                            {
                                continue;
                            }
                            else
                            {
                                if (train_route != null)
                                {
                                    train_route.HeadonTrainRoute = cell.StringCellValue;
                                }
                                else if (shunt_route != null)
                                {
                                    shunt_route.HeadonTrainRoute = cell.StringCellValue;
                                }
                            }
                        }
                        else if (colum_name == "迎面进路-调车")
                        {
                            ICell cell = row.GetCell(colum_idx);
                            if (cell == null || cell.CellType != CellType.String)
                            {
                                continue;
                            }
                            else
                            {
                                if (train_route != null)
                                {
                                    train_route.HeadonShuntRoute = cell.StringCellValue;
                                }
                                else if (shunt_route != null)
                                {
                                    shunt_route.HeadonShuntRoute = cell.StringCellValue;
                                }
                            }
                        }
                        else if (colum_name == "朝向")
                        {
                            ICell cell = row.GetCell(colum_idx);
                            if (train_route != null)
                            {
                                train_route.HeadToDirection = cell.StringCellValue;
                            }
                            else if (shunt_route != null)
                            {
                                shunt_route.HeadToDirection = cell.StringCellValue;
                            }
                        }
                        else if (colum_name == "进路号码")
                        {
                            ICell cell = row.GetCell(colum_idx);
                            if (train_route != null)
                            {
                                train_route.ID = (int)cell.NumericCellValue;
                            }
                            else if (shunt_route != null)
                            {
                                shunt_route.ID = (int)cell.NumericCellValue;
                            }
                        }
                    }
                }
                if (is_initialized)
                {
                    if (train_route != null)
                    {
                        route_list.Add(train_route.Name, train_route);
                    }
                    else if (shunt_route != null)
                    {
                        route_list.Add(shunt_route.Name, shunt_route);
                    }
                }
            }

            workbook.Close();
            return route_list;
        }

        public static Dictionary<string, Signal> ReadSignalEXCELFile()
        {
            Dictionary<string, Signal> signal_list = new Dictionary<string, Signal>();
            FileStream fs = null;
            IWorkbook workbook = GetIWorkbook(fs);

            //获取数据表
            ISheet sheet = workbook.GetSheetAt(0);
            //表头
            IRow header = sheet.GetRow(sheet.FirstRowNum);
            Dictionary<int, string> columns = new Dictionary<int, string>();
            for (int i = 0; i < header.LastCellNum; i++)
            {
                ICell cell = header.GetCell(i);
                columns.Add(i, cell.StringCellValue);
            }

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                string signal_name = "";
                string signal_type = "";
                string direction = "";
                Tuple<string, string> conn_tuple_0 = null;
                Tuple<string, string> conn_tuple_1 = null;
                foreach (var colum_idx in columns.Keys)
                {
                    ICell cell = row.GetCell(colum_idx);
                    string colum_name = columns[colum_idx];
                    if (colum_name == "名称")
                    {
                        signal_name = cell.StringCellValue;
                    }
                    else if (colum_name == "类型")
                    {
                        signal_type = cell.StringCellValue;
                    }
                    else if (colum_name == "朝向")
                    {
                        direction = cell.StringCellValue;
                    }
                    else if (colum_name == "北京方向连接")
                    {
                        if (cell != null)
                        {
                            conn_tuple_0 = new Tuple<string, string>(colum_name, cell.StringCellValue);
                        }
                    }
                    else if (colum_name == "武汉方向连接")
                    {
                        if (cell != null)
                        {
                            conn_tuple_1 = new Tuple<string, string>(colum_name, cell.StringCellValue);
                        }
                    }
                }
                Signal signal = new Signal(signal_name, signal_type, direction, conn_tuple_0, conn_tuple_1);
                signal_list.Add(signal.Name, signal);
            }

            workbook.Close();
            return signal_list;
        }

        public static Dictionary<string, Switch> ReadSwitchEXCELFile ()
        {
            Dictionary<string, Switch> switch_list = new Dictionary<string, Switch>();
            FileStream fs = null;
            IWorkbook workbook = GetIWorkbook(fs);

            //获取数据表
            ISheet sheet = workbook.GetSheetAt(0);
            //表头
            IRow header = sheet.GetRow(sheet.FirstRowNum);
            Dictionary<int, string> columns = new Dictionary<int, string>();
            for (int i = 0; i < header.LastCellNum; i++)
            {
                ICell cell = header.GetCell(i);
                columns.Add(i, cell.StringCellValue);
            }

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                string switch_name = "";
                string double_switch = "";
                string direction = "";
                Tuple<string, string> yingmian_dingwei = null;
                Tuple<string, string> yingmian_fanwei = null;
                Tuple<string, string> duixiang_dingwei = null;
                Tuple<string, string> duixiang_fanwei = null;
                foreach (var colum_idx in columns.Keys)
                {
                    ICell cell = row.GetCell(colum_idx);
                    string colum_name = columns[colum_idx];
                    if (colum_name == "名称")
                    {
                        switch_name = cell.StringCellValue;
                    }
                    else if (colum_name == "双动道岔")
                    {
                        if (cell != null)
                        {
                            double_switch = cell.StringCellValue;
                        }
                    }
                    else if (colum_name == "朝向")
                    {
                        direction = cell.StringCellValue;
                    }
                    else if (colum_name == "迎面定位开向")
                    {
                        if (cell != null)
                        {
                            yingmian_dingwei = new Tuple<string, string>(colum_name, cell.StringCellValue);
                        }
                    }
                    else if (colum_name == "迎面反位开向")
                    {
                        if (cell != null)
                        {
                            yingmian_fanwei = new Tuple<string, string>(colum_name, cell.StringCellValue);
                        }
                    }
                    else if(colum_name == "对向定位开向")
                    {
                        if (cell != null)
                        {
                            duixiang_dingwei = new Tuple<string, string>(colum_name, cell.StringCellValue);
                        }
                        else
                        {
                            duixiang_dingwei = new Tuple<string, string>(colum_name, null);
                        }
                    }
                    else if (colum_name == "对向反位开向")
                    {
                        if (cell != null)
                        {
                            duixiang_fanwei = new Tuple<string, string>(colum_name, cell.StringCellValue);
                        }
                        else
                        {
                            duixiang_fanwei = new Tuple<string, string>(colum_name, null);
                        }
                    }
                }
                Switch _switch = new Switch(switch_name, double_switch, direction, yingmian_dingwei, yingmian_fanwei,
                    duixiang_dingwei, duixiang_fanwei);
                switch_list.Add(_switch.Name, _switch);
            }

            workbook.Close();
            return switch_list;
        }

        public static Dictionary<string, Section> ReadSectionEXCELFile()
        {
            Dictionary<string, Section> section_list = new Dictionary<string, Section>();
            FileStream fs = null;
            IWorkbook workbook = GetIWorkbook(fs);

            //获取数据表
            ISheet sheet = workbook.GetSheetAt(0);
            //表头
            IRow header = sheet.GetRow(sheet.FirstRowNum);
            Dictionary<int, string> columns = new Dictionary<int, string>();
            for (int i = 0; i < header.LastCellNum; i++)
            {
                ICell cell = header.GetCell(i);
                columns.Add(i, cell.StringCellValue);
            }

            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                IRow row = sheet.GetRow(i);
                string section_name = "";
                string begin_from = "";
                string end_at = "";
                string section_type = "";
                foreach (var colum_idx in columns.Keys)
                {
                    ICell cell = row.GetCell(colum_idx);
                    string colum_name = columns[colum_idx];
                    if (colum_name == "名称")
                    {
                        section_name = cell.StringCellValue;
                    }
                    else if (colum_name == "始端")
                    {
                        begin_from = cell.StringCellValue;
                    }
                    else if (colum_name == "终端")
                    {
                        end_at = cell.StringCellValue;
                    }
                    else if (colum_name == "类型")
                    {
                        if (cell != null)
                        {
                            section_type = cell.StringCellValue;
                        }
                    }
                }
                Section section = new Section(section_name, begin_from, end_at, section_type);
                section_list.Add(section.Name, section);
            }

            workbook.Close();
            return section_list;
        }

        /// <summary>
        /// 获取单元格类型
        /// </summary>
        /// <param name="cell">目标单元格</param>
        /// <returns></returns>
        private static object GetValueType(ICell cell)
        {
            if (cell == null)
                return null;
            switch (cell.CellType)
            {
                case CellType.Blank:
                    return null;
                case CellType.Boolean:
                    return cell.BooleanCellValue;
                case CellType.Numeric:
                    return cell.NumericCellValue;
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Error:
                    return cell.ErrorCellValue;
                case CellType.Formula:
                default:
                    return "=" + cell.CellFormula;
            }
        }

        /// <summary>
        /// 获取指定行指定列的字符串数据
        /// </summary>
        /// <param name="row_idx"></param>
        /// <param name="colum_idx"></param>
        /// <returns></returns>
        private static string GetCellStringValue(ISheet sheet, int row_idx, int colum_idx, Dictionary<int, int> last_resort_map)
        {
            IRow row = sheet.GetRow(row_idx);
            string str_value = row.GetCell(colum_idx).StringCellValue;
            if (str_value == "")
            {
                IRow last_resort_row = sheet.GetRow(last_resort_map[colum_idx]);
                str_value = last_resort_row.GetCell(colum_idx).StringCellValue;
            }
            else
            {
                last_resort_map[colum_idx] = row_idx;
            }
            return str_value;
        }
    }
}
