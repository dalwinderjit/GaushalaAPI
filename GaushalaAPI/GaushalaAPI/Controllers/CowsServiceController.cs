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
    public class CowsServiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        CowsServiceContext cowServiceContext = null;
        public CowsServiceController(IConfiguration configuration)
        {
            _configuration = configuration;
            cowServiceContext = new CowsServiceContext(_configuration);
            // d1.SetConfiguration(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> GetServiceDetailByCowId(long id)
        {
            return cowServiceContext.GetServiceDetailByCowId(id);
        } 
        [HttpPost]
        public Dictionary<string, object> GetServiceDetailById(long id)
        {
            return cowServiceContext.GetServiceDetailById(id);
        }
        [HttpPost]
        public Dictionary<string,object> AddCowServiceData(CowServiceDataModel conceive)
        {
            return cowServiceContext.AddCowServiceData(conceive);
        }
        [HttpPost]
        public Dictionary<string,object> EditCowServiceData(CowServiceDataModel conceive)
        {
            return cowServiceContext.UpdateCowServiceData(conceive);
        }
        [HttpPost]
        public Dictionary<string,object> GetSummaryByCowId(long id)
        {
            return cowServiceContext.GetSmmaryByCowId(id);
        }
    }
}
