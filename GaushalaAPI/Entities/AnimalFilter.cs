using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class AnimalFilter : Filter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? TagNo { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public bool? GetCategory { get; set; }
        public string[]? cols { get; set; }
    }
}