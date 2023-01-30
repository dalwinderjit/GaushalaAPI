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
    public class BullServiceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        BullServiceContext bullServiceContext = null;
        public BullServiceController(IConfiguration configuration)
        {
            _configuration = configuration;
            bullServiceContext = new BullServiceContext(_configuration);
            // d1.SetConfiguration(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> GetServiceDetailByBullId(long id)
        {
            return bullServiceContext.GetServiceDetailByBullId(id);
        } 
        [HttpPost]
        public Dictionary<string,object> AddBullServiceData(CowServiceDataModel conceive)
        {
            return bullServiceContext.AddBullServiceData(conceive);
        }
        [HttpPost]
        public Dictionary<string,object> EditBullServiceData(CowServiceDataModel conceive)
        {
            return bullServiceContext.UpdateBullServiceData(conceive);
        }
        [HttpPost]
        public Dictionary<string, object> GetServiceDetailById(long id)
        {
            return bullServiceContext.GetServiceDetailById(id);
        }
        /*
       
        
        [HttpPost]
        public Dictionary<string,object> GetSummaryByCowId(long id)
        {
            return bullServiceContext.GetSmmaryByCowId(id);
        }*/
    }
}
