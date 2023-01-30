using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace GaushalaAPI.Models
{
    public class LactationDetail
    {
        public int lactation { get; set; }
        public decimal totalQuantity { get; set; }
        public decimal PeakYield { get; set; }
        public int WetDays { get; set; }
        public decimal Quantity305days { get; set; }
        public LactationDetail()
        {
            lactation = 0;
            totalQuantity = 0;
            PeakYield = 0;
            WetDays = 0;
            Quantity305days = 0;
        }
    }
}
