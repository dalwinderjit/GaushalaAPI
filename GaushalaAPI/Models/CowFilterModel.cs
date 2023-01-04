using System;
using System.Data.SqlClient;
using GaushalaAPI.Models;
namespace GaushalaAPI.Models
{
    public class CowFilterModel 
    {
        public int? length { get; set; }
        public int? start { get; set; }


        public CowModel? cow { get; set; }
        public CowFilterModel()
        {
            //base(sqlrdr);
            length = 10;
            start = 0;
            cow = new CowModel();
        }
    }
}
