using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class DiseaseFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? DiseaseName { get; set; }
        public string? Description { get; set; }
        public string[]? cols { get; set; }

        public DiseaseFilter()
        {
            this.cols = new string[] { };
        }
    }
}