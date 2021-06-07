using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    public class Section
    {
        /// <summary>
        /// 区段名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 区段始端
        /// </summary>
        public string BeginFrom;
        /// <summary>
        /// 区段终端
        /// </summary>
        public string EndAt;
        /// <summary>
        /// 区段类型
        /// </summary>
        public string SectionType;
        /// <summary>
        /// 与该区段连接的其它区段的名字
        /// </summary>
        public List<Tuple<string, string>> ConnSectionNames { get; set; }

        public Section(string name, string beginfrom, string endat, string section_type, Tuple<string, string> conn_beijing,
            Tuple<string, string> conn_wuhan)
        {
            Name = name;
            BeginFrom = beginfrom;
            EndAt = endat;
            SectionType = section_type;
            ConnSectionNames = new List<Tuple<string, string>>();
            ConnSectionNames.Add(conn_beijing);
            ConnSectionNames.Add(conn_wuhan);
        }
    }
}
