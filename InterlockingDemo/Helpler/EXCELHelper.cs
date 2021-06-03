using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using NPOI;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace InterlockingDemo.Helpler
{
    class EXCELHelper
    {
        public static DataTable ReadEXCELFile()
        {
            DataTable dt = new DataTable();

            //打开客流数据文件
            string localFilePath = "";
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = RootPathHelper.GetRootPath();
            fileDialog.Filter = "excel files|*.xls;*.xlsx";
            fileDialog.RestoreDirectory = true;

            if (fileDialog.ShowDialog() == true)
            {
                localFilePath = fileDialog.FileName.ToString();
                MessageBox.Show("打开成功！", "通知", MessageBoxButton.OK, MessageBoxImage.Information);

                //读取文件数据
                string fileExt = Path.GetExtension(localFilePath).ToLower();
                IWorkbook workbook;
                using (FileStream fs = new FileStream(localFilePath, FileMode.Open, FileAccess.Read))
                {
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

                    //获取数据表
                    ISheet sheet = workbook.GetSheetAt(0);
                    //表头
                    IRow header = sheet.GetRow(sheet.FirstRowNum);
                    header.RemoveCell(header.GetCell(0));
                    List<int> columns = new List<int>();
                    for (int i = 1; i < header.LastCellNum; i++)
                    {
                        columns.Add(i);
                    }

                    //获取时间列表
                    for (int i = 1; i <= sheet.LastRowNum; i++)
                    {
                        var timePerid = GetValueType(sheet.GetRow(i).GetCell(0)).ToString().Split('-');

                    }

                    //获取车站列表
                    for (int i = 0; i < columns.Count; i++)
                    {
                        var temp = GetValueType(sheet.GetRow(0).GetCell(i + 1)).ToString().Replace("->", "-");
                        var sectionRange = temp.Split('-');
                    }             
                }
            }
            else
            {
                throw new NullReferenceException();
            }

            return dt;
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
    }
}
