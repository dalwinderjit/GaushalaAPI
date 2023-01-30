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

namespace GaushalaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CalvsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        CalvsContext calvsContext = null;
        //CowsContext cowContext = null;
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
        [HttpPost]
        public Dictionary<string, object> GetCalvingDetailByIdTagNo(string tagno)
        {
            return calvsContext.GetCalvDetailByIdTagNo(tagno);
        }

        [HttpPost]
        public Dictionary<long, object> GetCowsIDNamePairByTagNo1(string tagNo, int pageNo)
        {
            //return "HELLO";
            return calvsContext.GetCalvDetailByIdTagNo1(tagNo,pageNo);
        }

        [HttpPost]
        public Dictionary<string, object> AddCalv(CalfModel calv)
        {
            if (calv.ValidateCalv(calvsContext, "Add") == true)
            {
                return calvsContext.AddCalv(calv);
            }
            else
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["message"] = "Validation Failed";
                data["errors"] = calv.errors;
                return data;
            }
        }

        [HttpPost]
        public Dictionary<string, object> UpdateCalv(CalfModel calv)
        {
            if (calv.ValidateCalv(calvsContext, "Update") == true)
            {
                return calvsContext.UpdateCalv(calv);
            }
            else
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["message"] = "Validation Failed";
                data["errors"] = calv.errors;
                return data;
            }
        }




    }
}
