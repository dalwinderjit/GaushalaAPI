using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class AddressDistrictFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? District { get; set; }
        public long? StateID { get; set; }
        public string? Deleted { get; set; }
        public string[]? cols { get; set; }

        public AddressDistrictFilter()
        {
            this.cols = new string[] { };
        }
    }
}