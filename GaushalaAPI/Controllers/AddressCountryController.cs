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
    public class AddressCountryController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        AddressCountryContext addressCountryModelContext = null;
        public AddressCountryController(IConfiguration configuration)
        {
            _configuration = configuration;
            addressCountryModelContext = new AddressCountryContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddAddressCountry(AddressCountryModel addressCountryModel)
        {
            return addressCountryModelContext.AddAddressCountry(addressCountryModel);
        }
        [HttpPost]
        public Dictionary<string,object> EditAddressCountry(AddressCountryModel addressCountryModel)
        {
            return addressCountryModelContext.EditAddressCountry(addressCountryModel);
        }
        [HttpPost]
        public Dictionary<string,object> GetAddressCountryDetailById(long id)
        {
            AddressCountryModel? addressCountryModel = addressCountryModelContext.GetAddressCountryDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (addressCountryModel != null)
            {
                data["status"] = "success";
                data["addressCountryModel"] = AddressCountryModel.GetFormatedAddressCountry(addressCountryModel);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Country Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetAddressCountryList(AddressCountryFilter addressCountryFilter){
            return addressCountryModelContext.GetAddressCountryList(addressCountryFilter);
        }
        [HttpPost]
        public Dictionary<string, string> GetAddressCountryIDNamePair(AddressCountryFilter addressCountryFilter)
        {
            return addressCountryModelContext.GetAddressCountryIdNamePair(addressCountryFilter);
        }

    }
}
