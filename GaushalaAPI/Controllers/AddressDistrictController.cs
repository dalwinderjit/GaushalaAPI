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
    public class AddressDistrictController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        AddressDistrictContext addressDistrictModelContext = null;
        public AddressDistrictController(IConfiguration configuration)
        {
            _configuration = configuration;
            addressDistrictModelContext = new AddressDistrictContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddAddressDistrict(AddressDistrictModel addressDistrictModel)
        {
            return addressDistrictModelContext.AddAddressDistrict(addressDistrictModel);
        }
        [HttpPost]
        public Dictionary<string,object> EditAddressDistrict(AddressDistrictModel addressDistrictModel)
        {
            return addressDistrictModelContext.EditAddressDistrict(addressDistrictModel);
        }
        [HttpPost]
        public Dictionary<string,object> GetAddressDistrictDetailById(long id)
        {
            AddressDistrictModel? addressDistrictModel = addressDistrictModelContext.GetAddressDistrictDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (addressDistrictModel != null)
            {
                data["status"] = "success";
                data["addressDistrictModel"] = AddressDistrictModel.GetFormatedAddressDistrict(addressDistrictModel);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "District Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public Dictionary<string, object> GetAddressDistrictDetailByStateId(long stateID)
        {
            List<Dictionary<string, object>> addressDistrictsList = addressDistrictModelContext.GetAddressDistrictDetailByStatetId(stateID);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (addressDistrictsList.Count>0)
            {
                data["status"] = "success";
                data["data"] = addressDistrictsList;
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Districts not Found";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetAddressDistrictList(AddressDistrictFilter addressDistrictFilter){
            return addressDistrictModelContext.GetAddressDistrictList(addressDistrictFilter);
        }
        [HttpPost]
        public Dictionary<string, string> GetAddressDistrictIDNamePair(AddressDistrictFilter addressDistrictFilter)
        {
            return addressDistrictModelContext.GetAddressDistrictIdNamePair(addressDistrictFilter);
        }

    }
}
