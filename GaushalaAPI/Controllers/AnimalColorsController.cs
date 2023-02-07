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
    public class AnimalColorsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        AnimalColorsContext animalColorsContext = null;
        public AnimalColorsController(IConfiguration configuration)
        {
            _configuration = configuration;
            animalColorsContext = new AnimalColorsContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddAnimalColors(AnimalColorsModel animalColorsModel)
        {
            return animalColorsContext.AddAnimalColors(animalColorsModel);
        }
        [HttpPost]
        public Dictionary<string,object> EditAnimalColors(AnimalColorsModel animalColors)
        {
            return animalColorsContext.EditAnimalColors(animalColors);
        }
        [HttpPost]
        public Dictionary<string,object> GetAnimalColorsDetailById(long id)
        {
            AnimalColorsModel? animalColors = animalColorsContext.GetAnimalColorsDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (animalColors != null)
            {
                data["status"] = "success";
                data["animalColors"] = AnimalColorsModel.GetFormatedAnimalColors(animalColors);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "AnimalColors Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetAnimalColorsList(AnimalColorsFilter animalColorsFilter){
            return animalColorsContext.GetAnimalColorsList(animalColorsFilter);
        }
        [HttpPost]
        public Dictionary<long, string> GetAnimalColorsIDNamePair(AnimalColorsFilter animalColorsFilter)
        {
            return animalColorsContext.GetAnimalColorsIdNamePair(animalColorsFilter);
        }

    }
}
