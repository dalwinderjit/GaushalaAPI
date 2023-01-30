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
    public class AdminCowsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        CowsContext cowContext = null;
        public AdminCowsController(IConfiguration configuration)
        {
            _configuration = configuration;
            cowContext = new CowsContext(_configuration);
            // d1.SetConfiguration(_configuration);
        }
        
    }
}
