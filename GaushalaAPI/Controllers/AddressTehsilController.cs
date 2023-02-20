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
using GaushalAPI.Entities;

namespace GaushalaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class AddressTehsilController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        AddressTehsilContext addressTehsilModelContext = null;
        public AddressTehsilController(IConfiguration configuration)
        {
            _configuration = configuration;
            addressTehsilModelContext = new AddressTehsilContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddAddressTehsil(AddressTehsilModel addressTehsilModel)
        {
            return addressTehsilModelContext.AddAddressTehsil(addressTehsilModel);
        }
        [HttpPost]
        public Dictionary<string,object> EditAddressTehsil(AddressTehsilModel addressTehsilModel)
        {
            return addressTehsilModelContext.EditAddressTehsil(addressTehsilModel);
        }
        [HttpPost]
        public Dictionary<string,object> GetAddressTehsilDetailById(long id)
        {
            AddressTehsilModel? addressTehsilModel = addressTehsilModelContext.GetAddressTehsilDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (addressTehsilModel != null)
            {
                data["status"] = "success";
                data["addressTehsilModel"] = AddressTehsilModel.GetFormatedTehsil(addressTehsilModel);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Tehsil Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public Dictionary<string, object> GetAddressTehsilDetailByDistrictId(long districtID)
        {
            List<Dictionary<string, object>> addressTehsilsList = addressTehsilModelContext.GetAddressTehsilDetailByDistrictId(districtID);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (addressTehsilsList.Count>0)
            {
                data["status"] = "success";
                data["data"] = addressTehsilsList;
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Tehsils not Found";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetAddressTehsilList(AddressTehsilFilter addressTehsilFilter){
            return addressTehsilModelContext.GetAddressTehsilList(addressTehsilFilter);
        }
        [HttpPost]
        public Dictionary<string, string> GetAddressTehsilIDNamePair(AddressTehsilFilter addressTehsilFilter)
        {
            return addressTehsilModelContext.GetAddressTehsilIdNamePair(addressTehsilFilter);
        }

    }
}
