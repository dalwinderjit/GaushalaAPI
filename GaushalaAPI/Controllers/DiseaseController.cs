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
    public class DiseaseController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        DiseaseContext diseaseContext = null;
        public DiseaseController(IConfiguration configuration)
        {
            _configuration = configuration;
            diseaseContext = new DiseaseContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddDisease(DiseaseModel medication)
        {
            return diseaseContext.AddDisease(medication);
        }
        [HttpPost]
        public Dictionary<string,object> EditDisease(DiseaseModel disease)
        {
            return diseaseContext.EditDisease(disease);
        }
        [HttpPost]
        public Dictionary<string,object> GetDiseaseDetailById(long id)
        {
            DiseaseModel? disease = diseaseContext.GetDiseaseDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (disease != null)
            {
                data["status"] = "success";
                data["disease"] = DiseaseModel.GetFormatedDisease(disease);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Disease Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetDiseaseList(DiseaseFilter diseaseFilter){
            return diseaseContext.GetDiseaseList(diseaseFilter);
        }
        [HttpPost]
        public Dictionary<long, string> GetDiseaseIdNamePairByDiseaseName(DiseaseFilter diseaseFilter)
        {
            return diseaseContext.GetDiseaseIdNamePairByDiseaseName(diseaseFilter);
        }

    }
}
