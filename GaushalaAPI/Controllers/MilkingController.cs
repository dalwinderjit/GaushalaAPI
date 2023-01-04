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
using System.Collections;

namespace GaushalaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class MilkingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        MilkingContext milkingContext = null;
        public MilkingController(IConfiguration configuration)
        {
            _configuration = configuration;
            milkingContext = new MilkingContext(_configuration);
            // d1.SetConfiguration(_configuration);
        }
        [HttpPost]
        public Dictionary<string,object> AddCowMilkingStartStopDetail(MilkingStartStop milkingss)
        {
            return milkingContext.SaveCowMilkingStartStop(milkingss);
        }
        [HttpPost]
        public Dictionary<string,object> EditCowMilkingStartStopDetail(MilkingStartStop milkingss)
        {
            return milkingContext.UpdateCowMilkingStartStop(milkingss);
        }
        [HttpPost]
        public Dictionary<string, object> GetCowMilkingStartStopDetailById(long Id)
        {
            return milkingContext.GetTotalMilkStartStopById(Id);
        }
        [HttpPost]
        public Dictionary<string,object> GetCowMilkingStartStopDetailByCowId(long CowID)
        {
            return milkingContext.GetCowMilkingStartStopDetailByCowId(CowID);
        }
        [HttpPost]
        public Dictionary<string, object> GetCowMilkingComaprisonData(List<long> CowIds, int comparisonType, List<int>? lactations,int? dayFrom, int? dayTo,DateTime? DateFrom, DateTime? DateTo,int daysSeparator) {
            //return new Dictionary<string, object>();
            return milkingContext.GetCowMilkingComaprisonData(CowIds, comparisonType, lactations, dayFrom, dayTo, DateFrom, DateTo, daysSeparator);
        }
    }
}
