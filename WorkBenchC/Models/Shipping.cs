using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkBenchC.Models
{
    public class Shipping
    {
        public string familyName { get; set; }
        public string givenName { get; set; }
        public string streetAddress { get; set; }
        public string extendedAddress { get; set; }
        public string locality { get; set; }
        public string region { get; set; }
        public string postalCode { get; set; }
        public string countryName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
    }
}
