using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class AddressStateFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? State { get; set; }
        public long? CountryID { get; set; }
        public string? Deleted { get; set; }
        public string[]? cols { get; set; }

        public AddressStateFilter()
        {
            this.cols = new string[] { };
        }
    }
}