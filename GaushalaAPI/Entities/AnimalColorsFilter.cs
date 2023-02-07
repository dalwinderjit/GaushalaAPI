using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class AnimalColorsFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? Colour { get; set; }
        public string? Deleted { get; set; }
        public string[]? cols { get; set; }

        public AnimalColorsFilter()
        {
            this.cols = new string[] { };
        }
    }
}