using GaushalaAPI.Models;
using GaushalaAPI.DBContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System;
using GaushalAPI.Models;
using System.Web;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Linq;
using System.Data.SqlClient;
using GaushalAPI.Entities;

namespace GaushalaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AnimalsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        AnimalContext animalsContext = null;
        //CowsContext cowContext = null;
        public AnimalsController(IConfiguration configuration)
        {
            _configuration = configuration;
            animalsContext = new AnimalContext(_configuration);
        }

        [HttpPost]
        public Dictionary<long, object> GetAnimalsIDNameTagNoCategoryPairByTagNo(string tagNo, int pageNo, int recordsPerPage)
        {
            AnimalFilter ani = new AnimalFilter();
            ani.TagNo = "%" + tagNo + "%";
            ani.PageNo = pageNo;
            ani.RecordsPerPage = recordsPerPage;
            ani.cols = new string[] { "Id","Name","TagNo","Category" };
            ani.OrderBy = "TagNo";
            ani.Order = "ASC";
            ani.GetCategory = true;
            return animalsContext.GetAnimalsIDNameTagNoPair(ani);
        }
        [HttpPost]
        public List<Dictionary<string, object>> GetAnimals(AnimalFilter animalFilter, int pageNo, int recordsPerPage)
        {
            animalFilter.TagNo = "%" + animalFilter.TagNo + "%";
            animalFilter.PageNo = pageNo;
            animalFilter.RecordsPerPage = recordsPerPage;
            animalFilter.cols = new string[] { "Id","Name","TagNo","Category","DOB","Breed","Colour" };
            animalFilter.OrderBy = "TagNo";
            animalFilter.Order = "ASC";
            return animalsContext.GetAnimals(animalFilter);
        }


    }
}
