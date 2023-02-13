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
    public class AddressStateController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        AddressStateContext addressStateModelContext = null;
        public AddressStateController(IConfiguration configuration)
        {
            _configuration = configuration;
            addressStateModelContext = new AddressStateContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddAddressState(AddressStateModel addressCountryModel)
        {
            return addressStateModelContext.AddAddressState(addressCountryModel);
        }
        [HttpPost]
        public Dictionary<string,object> EditAddressState(AddressStateModel addressCountryModel)
        {
            return addressStateModelContext.EditAddressState(addressCountryModel);
        }
        [HttpPost]
        public Dictionary<string,object> GetAddressStateDetailById(long id)
        {
            AddressStateModel? addressCountryModel = addressStateModelContext.GetAddressStateDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (addressCountryModel != null)
            {
                data["status"] = "success";
                data["addressCountryModel"] = AddressStateModel.GetFormatedAddressState(addressCountryModel);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "State Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public Dictionary<string, object> GetAddressStateDetailByCountryId(long countryId)
        {
            List<Dictionary<string, object>> addressStatesList = addressStateModelContext.GetAddressStateDetailByCountryId(countryId);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (addressStatesList.Count>0)
            {
                data["status"] = "success";
                data["data"] = addressStatesList;
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "States not Found";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetAddressStateList(AddressStateFilter addressStateFilter){
            return addressStateModelContext.GetAddressStateList(addressStateFilter);
        }
        [HttpPost]
        public Dictionary<string, string> GetAddressStateIDNamePair(AddressStateFilter addressStateFilter)
        {
            return addressStateModelContext.GetAddressStateIdNamePair(addressStateFilter);
        }

    }
}
