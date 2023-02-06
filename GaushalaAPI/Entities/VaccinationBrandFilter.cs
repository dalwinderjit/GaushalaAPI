using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class VaccinationBrandFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string[]? cols { get; set; }

        public VaccinationBrandFilter()
        {
            this.cols = new string[] { };
        }
    }
}