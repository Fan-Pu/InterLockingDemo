using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    public class Section
    {
        public string Name;

        public string BeginFrom;

        public string EndAt;

        public string SectionType;

        public Section(string name,string beginfrom,string endat,string section_type)
        {
            Name = name;
            BeginFrom = beginfrom;
            EndAt = endat;
            SectionType = section_type;
        }
    }
}
