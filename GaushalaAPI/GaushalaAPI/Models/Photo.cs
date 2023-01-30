using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Models
{
    public class Photo
    {
        public IFormFile formFile { get; set; }
    }
}
