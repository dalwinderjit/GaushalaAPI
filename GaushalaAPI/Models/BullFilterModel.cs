using System;
using System.Data.SqlClient;
using GaushalaAPI.Models;
namespace GaushalaAPI.Models
{
    public class BullFilterModel 
    {
        public int? length { get; set; }
        public int? start { get; set; }


        public BullModel? bull { get; set; }
        public BullFilterModel()
        {
            //base(sqlrdr);
            length = 10;
            start = 0;
            bull = new BullModel();
        }
    }
}
