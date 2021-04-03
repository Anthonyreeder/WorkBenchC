using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBenchC.Models.Browsers
{
    class Chrome : BrowserType
    {
        public Chrome()
        {
            agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/89.0.4389.114 Safari/537.36";
            secchua = "Google Chrome\";v=\"89\", \"Chromium\";v=\"89\", \"; Not A Brand\";v=\"99";
        }
        //might be useful later?
        string mac = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0.3 Safari/605.1.15";
    }
}

