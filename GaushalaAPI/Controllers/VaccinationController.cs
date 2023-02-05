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
    public class VaccinationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        VaccinationContext vaccinationContext = null;
        public VaccinationController(IConfiguration configuration)
        {
            _configuration = configuration;
            vaccinationContext = new VaccinationContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddVaccination(VaccinationModel medication)
        {
            return vaccinationContext.AddVaccination(medication);
        }
        [HttpPost]
        public Dictionary<string,object> EditVaccination(VaccinationModel vaccination)
        {
            return vaccinationContext.EditVaccination(vaccination);
        }
        [HttpPost]
        public Dictionary<string,object> GetVaccinationDetailById(long id)
        {
            VaccinationModel? vaccination = vaccinationContext.GetVaccinationDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (vaccination != null)
            {
                data["status"] = "success";
                data["vaccination"] = VaccinationModel.GetFormatedVaccination(vaccination);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Vaccination Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetVaccinationList(VaccinationFilter vaccinationFilter){
            return vaccinationContext.GetVaccinationList(vaccinationFilter);
        }
       
    }
}
