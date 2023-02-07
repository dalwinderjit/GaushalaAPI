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
    public class AnimalBreedsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        AnimalBreedsContext animalBreedsContext = null;
        public AnimalBreedsController(IConfiguration configuration)
        {
            _configuration = configuration;
            animalBreedsContext = new AnimalBreedsContext(_configuration);
        }
        [HttpPost]
        public Dictionary<string, object> AddAnimalBreeds(AnimalBreedsModel medication)
        {
            return animalBreedsContext.AddAnimalBreeds(medication);
        }
        [HttpPost]
        public Dictionary<string,object> EditAnimalBreeds(AnimalBreedsModel animalBreeds)
        {
            return animalBreedsContext.EditAnimalBreeds(animalBreeds);
        }
        [HttpPost]
        public Dictionary<string,object> GetAnimalBreedsDetailById(long id)
        {
            AnimalBreedsModel? animalBreeds = animalBreedsContext.GetAnimalBreedsDetailById(id);
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (animalBreeds != null)
            {
                data["status"] = "success";
                data["animalBreeds"] = AnimalBreedsModel.GetFormatedAnimalBreeds(animalBreeds);
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "AnimalBreeds Record not Exists";
            }
            return data;
        }
        [HttpPost]
        public List<Dictionary<string,object>> GetAnimalBreedsList(AnimalBreedsFilter animalBreedsFilter){
            return animalBreedsContext.GetAnimalBreedsList(animalBreedsFilter);
        }
        [HttpPost]
        public Dictionary<long, string> GetAnimalBreedsIDNamePair(AnimalBreedsFilter animalBreedsFilter)
        {
            return animalBreedsContext.GetAnimalBreedsIdNamePair(animalBreedsFilter);
        }

    }
}
