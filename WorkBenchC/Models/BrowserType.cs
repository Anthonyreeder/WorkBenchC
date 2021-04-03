using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBenchC.Models
{
    class BrowserType
    {

        protected string agent = "";
        public string _agent
        {
            get { return agent; }
            set { agent = value; }
        }

        public string secchua = "";

        public string getAgent()
        {
            return agent;
        }
        public string getSecchua()
        {
            return secchua;
        }
    }
}