using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Helpler
{
    class RootPathHelper
    {
        public static string GetRootPath()
        {
            string rootPath = "";
            string BaseDirectoryPath = AppDomain.CurrentDomain.BaseDirectory; // 向上回退三级，才能到项目的根目录
            rootPath = BaseDirectoryPath.Substring(0, BaseDirectoryPath.LastIndexOf("\\")); // 第一个\是转义符，所以要写两个
            rootPath = rootPath.Substring(0, rootPath.LastIndexOf("\\"));
            rootPath = rootPath.Substring(0, rootPath.LastIndexOf("\\"));
            rootPath = rootPath.Substring(0, rootPath.LastIndexOf("\\"));
            return rootPath;
        }
    }
}
