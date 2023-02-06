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
    public class VaccinationBrandController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        VaccinationBrandContext vaccinationBrandContext = null;
        public VaccinationBrandController(IConfiguration configuration)
        {
            _configuration = configuration;
            vaccinationBrandContext = new VaccinationBrandContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddVaccination(VaccinationBrandModel vaccinationBrand)
        {
            return vaccinationBrandContext.AddVaccinationBrand(vaccinationBrand);
        }
        [HttpPost]
        public Dictionary<string,object> EditVaccination(VaccinationBrandModel vaccinationBrand)
        {
            return vaccinationBrandContext.EditVaccinationBrand(vaccinationBrand);
        }
        [HttpPost]
        public Dictionary<string,object> GetVaccinationDetailById(long id)
        {
            VaccinationBrandModel? vaccinationBrand = vaccinationBrandContext.GetVaccinationBrandDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (vaccinationBrand != null)
            {
                data["status"] = "success";
                data["vaccination"] = VaccinationBrandModel.GetFormatedVaccinationBrand(vaccinationBrand);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Vaccination Brand Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetVaccinationBrandList(VaccinationBrandFilter vaccinationBrandFilter){
            return vaccinationBrandContext.GetVaccinationBrandList(vaccinationBrandFilter);
        }
        [HttpPost]
        public Dictionary<long, string> GetVaccinationBrandIdNamePair(VaccinationBrandFilter vaccinationBrandFilter)
        {
            return vaccinationBrandContext.GetVaccinationBrandIdNamePairByVaccinationBrandName(vaccinationBrandFilter);
        }

    }
}
