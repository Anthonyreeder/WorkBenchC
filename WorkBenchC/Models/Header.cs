using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBenchC.Models
{
    class Header
    {
        public Header(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
        private string name { get; set; }
        private string value { get; set; }

        public string getName()
        {
            return name;
        }
        public string getValue()
        {
            return value;
        }
    }
}
