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
    public class MedicationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        MedicationContext medicationContext = null;
        public MedicationController(IConfiguration configuration)
        {
            _configuration = configuration;
            medicationContext = new MedicationContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string,object> AddMedicationDetail(AnimalMedicationModel medication)
        {
            return medicationContext.AddMedicationDetail(medication);
        }
        [HttpPost]
        public Dictionary<string,object> EditMedicationDetail(AnimalMedicationModel medication)
        {
            return medicationContext.EditMedicationDetail(medication);
        }
        [HttpPost]
        public Dictionary<string,object> GetMedicationDetailById(long id)
        {
            AnimalMedicationModel? medication = medicationContext.GetMedicationDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (medication != null)
            {
                data["status"] = "success";
                data["medication"] = AnimalMedicationModel.GetFormatedMedication(medication);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Medication Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public Dictionary<string,object> GetMedicationDetailByAnimalId(long id,int start=1, int length=10){
            return medicationContext.GetMedicationDetailByAnimalId(id,start,length);
        }
    }
}
