using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class AddressTehsilFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? Tehsil { get; set; }
        public long? DistrictID { get; set; }
        public string? Deleted { get; set; }
        public string[]? cols { get; set; }

        public AddressTehsilFilter()
        {
            this.cols = new string[] { };
        }
    }
}