using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class AddressCountryFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? Country { get; set; }
        public string? Deleted { get; set; }
        public string[]? cols { get; set; }

        public AddressCountryFilter()
        {
            this.cols = new string[] { };
        }
    }
}