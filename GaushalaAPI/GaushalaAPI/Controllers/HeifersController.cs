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

namespace GaushalaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class HeifersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        HeifersContext heifersContext = null;
        public HeifersController(IConfiguration configuration)
        {
            _configuration = configuration;
            heifersContext = new HeifersContext(_configuration);
        }
        [HttpPost]
        public Dictionary<long, object> GetHeifersIDNamePairByTagNo(string tagNo, int pageNo, int recordsPerPage)
        {
            return heifersContext.GetHeifersIDNamePairByTagNo(tagNo, pageNo, recordsPerPage);
        }
        [HttpPost]
        public List<Dictionary<string, object>> GetHeifers(string tagNo, int pageNo, int recordsPerPage)
        {
            return heifersContext.GetHeifers(tagNo, pageNo, recordsPerPage);
        }
        [HttpPost]
        public Dictionary<string, object> AddHeifer(HeiferModel heifer)
        {
            if (heifer.ValidateHeifer(heifersContext, "Add") == true)
            {
                return heifersContext.AddHeifer(heifer);
            }
            else
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["message"] = "Validation Failed";
                data["errors"] = heifer.errors;
                return data;
            }
        }
        [HttpPost]
        public Dictionary<string, object> UpdateHeifer(HeiferModel heifer)
        {
            if (heifer.ValidateHeifer(heifersContext, "Update") == true)
            {
                return heifersContext.UpdateHeifer(heifer);
            }
            else
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["message"] = "Validation Failed";
                data["errors"] = heifer.errors;
                return data;
            }
        }
        [HttpPost]
        public Dictionary<string, object> GetHeiferById(long id)
        {
            return heifersContext.GetHeiferDetailById(id);
            //return new HeiferModel();
        }
    }
}
