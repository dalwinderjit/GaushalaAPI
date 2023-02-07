using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class AnimalBreedsFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? Breed { get; set; }
        public string? Deleted { get; set; }
        public string[]? cols { get; set; }

        public AnimalBreedsFilter()
        {
            this.cols = new string[] { };
        }
    }
}