using GaushalaAPI.Models;
using GaushalAPI.Models;
using GaushalAPI.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class HeifersContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        public HeifersContext(IConfiguration configuration) :base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string,object> AddHeifer(AnimalModel ani,SqlConnection? conn2=null,SqlTransaction? tran=null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //this.Testing(ani);
            //return data;
            string image = null;
            ani.Category = "HEIFER";
            ani.Gender= "FEMALE";
            ani.Lactation = 0;
            ani.BelongsToGaushala= true;
            //ani.BirthLactationNumber = 1;
            ani.GenerateImageName(CowsContext.GetMaxAnimalId(this._configuration));
            Console.WriteLine("picture " + ani.Picture);
            bool addSire = false;
            bool addDam = false;
            if (ani.DamID == null)
            {
                addDam = true;
                Console.WriteLine("Need to add Dam");
            }
            if (ani.SireID == null)
            {
                Console.WriteLine("Need to add Sire");
                addSire = true;
            }
            data = base.AddAnimal(ani,addDam,addSire);
            if (((Dictionary<string,string>)data["data"])["status"] == "success") {
                ani.SaveImage2();
            }
            Console.WriteLine("Returnign again");
            return data;
        }
        internal Dictionary<string,object> UpdateHeifer(AnimalModel ani,SqlConnection? conn2=null,SqlTransaction? tran=null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //this.Testing(ani);
            //return data;
            string image = null;
            ani.Category = "HEIFER";
            ani.Gender= "FEMALE";
            ani.Lactation = 0;
            //ani.BirthLactationNumber = 1;
            ani.GenerateImageName(CowsContext.GetMaxAnimalId(this._configuration));
            Console.WriteLine("picture " + ani.Picture);
            bool addSire = false;
            bool addDam = false;
            if (ani.DamID == null)
            {
                addDam = true;
                Console.WriteLine("Need to add Dam");
            }
            if (ani.SireID == null)
            {
                Console.WriteLine("Need to add Sire");
                addSire = true;
            }
            data = base.UpdateAnimal(ani,addDam,addSire);
            if (((Dictionary<string,string>)data["data"])["status"] == "success") {
                ani.SaveImage2();
            }
            Console.WriteLine("Returnign again");
            return data;
        }
        internal Dictionary<string, object> GetHeiferDetailById(long id)
        {
            AnimalFilter ani = new AnimalFilter();
            //ani.cols = ["Id",''];
            ani.Id = id;
            ani.Category = "Heifer";
            bool fetchConceiveData = true;
            return base.GetAnimalById(id,ani,fetchConceiveData);
        }
        internal Dictionary<long, object> GetHeifersIDNamePairByTagNo(string tagNo, int pageNo, int recordsPerPage)
        {
            //Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<long, object> heifers = new Dictionary<long, object>();
            AnimalFilter animalFilter = new AnimalFilter();
            animalFilter.TagNo = "%"+tagNo+"%";
            animalFilter.PageNo = pageNo;
            animalFilter.RecordsPerPage = recordsPerPage;
            animalFilter.Category = "HEIFER";
            animalFilter.OrderBy = "TagNo";
            animalFilter.Order = "ASC";
            animalFilter.GetCategory = false;
            animalFilter.cols = new String[]{ "Id", "Name", "TagNo", "Category" };
            heifers = base.GetAnimalsIDNameTagNoPair(animalFilter);
            //data["data"] = heifers;
            return heifers;
        }
        internal List<Dictionary<string, object>> GetHeifers(string tagNo, int pageNo, int recordsPerPage)
        {
            //Dictionary<string, object> data = new Dictionary<string, object>();
            List<Dictionary<string, object>> heifers = new List<Dictionary<string, object>>();
            AnimalFilter animalFilter = new AnimalFilter();
            animalFilter.TagNo = "%"+tagNo+"%";
            animalFilter.PageNo = pageNo;
            animalFilter.RecordsPerPage = recordsPerPage;
            animalFilter.Category = "HEIFER";
            animalFilter.OrderBy = "TagNo";
            animalFilter.Order = "ASC";
            animalFilter.GetCategory = false;
            animalFilter.cols = new String[]{ "Id", "Name", "TagNo", "Category" };
            heifers = base.GetAnimals(animalFilter);
            //data["data"] = heifers;
            return heifers;
        }
    }
}