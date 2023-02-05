using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class VaccinationFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? State { get; set; }
        public string? VaccinationBrandID { get; set; }
        public string? VaccinationType { get; set; }
        public string[]? cols { get; set; }

        public VaccinationFilter()
        {
            this.cols = new string[] { };
        }
    }
}