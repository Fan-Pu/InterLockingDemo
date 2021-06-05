using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterlockingDemo.Classes
{
    public class Structure
    {
        public Dictionary<string, Route> Routes;

        public Dictionary<string, Signal> Signals;

        public Dictionary<string, Switch> Switches;

        public Dictionary<string, Section> Sections;

        public Structure()
        {
            Routes = new Dictionary<string, Route>();
            Signals = new Dictionary<string, Signal>();
            Switches = new Dictionary<string, Switch>();
            Sections = new Dictionary<string, Section>();
        }
    }
}
