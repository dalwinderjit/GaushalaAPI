using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace GaushalaAPI.Models
{
    public class MilkComparison
    {
        public decimal? Quantity { get; set; }
        public decimal? OtherQuantity { get; set; }
        public int? LactaionNo { get; set; }
        public DateTime StartDate { get; set; }
        public float percentage { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        
        public MilkComparison()
        {
            Quantity = 0;
            OtherQuantity = 0;
            StartDate = new DateTime();
            percentage = 0;
        }
    }
}
