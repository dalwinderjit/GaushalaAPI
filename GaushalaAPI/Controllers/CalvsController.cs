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
    public class CalvsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        CalvsContext calvsContext = null;
        public CalvsController(IConfiguration configuration)
        {
            _configuration = configuration;
            calvsContext = new CalvsContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string,object> GetCalvingDetailById(long id)
        {
            return calvsContext.GetCalvDetailById(id);
        }
    }
}
