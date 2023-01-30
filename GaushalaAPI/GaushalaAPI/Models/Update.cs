using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Models
{
    public class Update
    {
        [Required]
        [MaxLength(140)]
        public string Status { get; set; }

        public DateTime Date { get; set; }

    }
}
