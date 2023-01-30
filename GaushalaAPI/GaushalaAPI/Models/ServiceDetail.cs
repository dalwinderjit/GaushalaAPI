using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Models
{
    public class ServiceDetail
    {
        public long CowId { get; set; }
        public int Total { get; set; }
        public int Failed { get; set; }
        public int Pending { get; set; }
        public int Sold { get; set; }
        public int Confirmed { get; set; }
        public int Successful { get; set; }
        public int Female { get; set; }
        public int Male { get; set; }

        public int Died { get; set; }
        public ServiceDetail()
        {
            Total = 0;
            Failed = 0;
            Pending = 0;
            Sold = 0;
            Confirmed = 0;
            Successful = 0;
            Female = 0;
            Male = 0;
        }
        
    }
}
