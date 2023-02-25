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
using System.Collections;
using GaushalAPI.Entities;
using GaushalaAPI.Entities;

namespace GaushalaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class CowsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        CowsContext cowContext = null;
        public CowsController(IConfiguration configuration)
        {
            _configuration = configuration;
            cowContext = new CowsContext(_configuration);
            // d1.SetConfiguration(_configuration);
        }
        [HttpGet]
        public Dictionary<string,object> GetDataForCowProfile()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<int,string> colors = cowContext.getColors();
            Dictionary<int, string> breeds = cowContext.getBreeds();
            data["colors"] = colors;
            data["breeds"] = breeds;
            return data;
        }
        [HttpPost(Name = "SaveCow")]
        public Dictionary<string, Dictionary<string, string>> SaveCow([FromForm] CowModel cow) {
            /*Console.WriteLine("ID"+cow.Id);
            Console.WriteLine("ID"+cow.Name);
            Console.WriteLine("ID"+cow.TagNo);
            Console.WriteLine("ID"+cow.Category);
            Console.WriteLine("ID"+cow.DOB.ToString());
            Console.WriteLine("ID"+cow.Breed);
            Console.WriteLine("ID"+cow.Lactation);
            Console.WriteLine("ID"+cow.DamID);
            Console.WriteLine("ID"+cow.SireID);
            Console.WriteLine("ID"+cow.Colour);
            Console.WriteLine("ID"+cow.Weight);
            Console.WriteLine("ID"+cow.Height);
            Console.WriteLine("ID"+cow.ButterFat);*/
            if (ModelState.IsValid || true) {
                //Console.WriteLine("HELO");
                return cowContext.SaveCow(cow);
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                var m = string.Join(",", ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList());
                Console.WriteLine(message + m);
            }
            Console.WriteLine(ModelState.ErrorCount);
            Console.WriteLine("NOt Okd");
            return new Dictionary<string, Dictionary<string, string>>();
        }
        [HttpPost(Name = "AddCow")]
        public Dictionary<string, Dictionary<string, string>> AddCow([FromForm] CowModel cow)
        {
            /*Console.WriteLine("ID" + cow.Id);
            Console.WriteLine("ID" + cow.Name);
            Console.WriteLine("ID" + cow.TagNo);
            Console.WriteLine("ID" + cow.Category);
            Console.WriteLine("ID" + cow.DOB.ToString());
            Console.WriteLine("ID" + cow.Breed);
            Console.WriteLine("ID" + cow.Lactation);
            Console.WriteLine("ID" + cow.DamID);
            Console.WriteLine("ID" + cow.SireID);
            Console.WriteLine("ID" + cow.Colour);
            Console.WriteLine("ID" + cow.Weight);
            Console.WriteLine("ID" + cow.Height);
            Console.WriteLine("ID" + cow.ButterFat);*/
            cow.Id = 0;
            if (ModelState.IsValid)
            {
                //Console.WriteLine("HELO");
                return cowContext.AddCow(cow);
            }
            else
            {
                var message = string.Join(" | ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                var m = string.Join(",", ModelState.Select(x => x.Value.Errors)
                           .Where(y => y.Count > 0)
                           .ToList());
                Console.WriteLine(message + m);
            }
            Console.WriteLine(ModelState.ErrorCount);
            Console.WriteLine("NOt Okd");
            return new Dictionary<string, Dictionary<string, string>>();
        }
        [HttpPost(Name = "SaveCow2")]
        public Dictionary<string, Dictionary<string, object>> SaveCow2([FromQuery] int id, [FromForm] string value)
        {
            //Send OK Response to Client.
            //return Request.CreateResponse(HttpStatusCode.OK, fileName);
            /*
            Update update = new Update()
            {
                Status = HttpUtility.HtmlEncode(value),
                Date = DateTime.UtcNow
            };
            Console.WriteLine("Hello "+value);
            Console.WriteLine(update.Status + update.Date);
            var id = Guid.NewGuid();
            //updates[id] = update;

            var response = new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(update.Status)
            };
            /*response.Headers.Location =
                new Uri(Url.Link("DefaultApi", new { action = "status", id = id }));*/
            //return response;
            Dictionary<string, Dictionary<string, object>> data = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, object> dd = new Dictionary<string, object>();
            dd.Add("ID", id);
            dd.Add("value", value);
            data.Add("data", dd);
            
            return data;
        }
        // GET: HomeController
        [HttpPost(Name = "GetCows")]
        public Dictionary<string, object> Get(CowFilterModel cowFilters)
        {
            Dictionary<string, object> collection = new Dictionary<string, object>();
            /*if (cowFilters != null)
            {
                Console.WriteLine(cowFilters.length);
                
                return cowFilters;
            }
            else
            {
                Console.WriteLine("cowFilters not set");
                return cowFilters;
            }*/
            //return "HELLO";
            
            collection.Add("data", cowContext.GetCows(cowFilters));
            collection.Add("recordsTotal", cowContext.GetTotalCows());
            collection.Add("recordsFiltered", cowContext.GetTotalFilteredCows(cowFilters));
            return collection;
            //return cowContext.GetCows();
        }
        //[EnableCors(origins: "http://127.0.0.1:4000/", headers: "*", methods: "get,post")]
        [HttpPost(Name = "GetSires")]
        public Dictionary<string, object> GetSires(BullFilterModel bullFilters)
        {
            Dictionary<string, object> collection = new Dictionary<string, object>();
            
            collection.Add("data", cowContext.GetSires(bullFilters));
            collection.Add("recordsTotal", cowContext.GetTotalSires());
            collection.Add("recordsFiltered", cowContext.GetTotalFilteredSires(bullFilters));
            return collection;
            //return cowContext.GetCows();
        }
        [HttpPost]
        public Dictionary<long, object> GetCowsIDNamePairByTagNo(string tagNo,int pageNo)
        {
            //return "HELLO";
            return cowContext.GetCowsIdNamePairByTagNo(tagNo,pageNo);
        }
        [HttpGet("{id}")]
        public Dictionary<string,object> GetCowById(int id) {
            return CowModel.GetFormatedCowData(cowContext.GetCowById(id));
        }
        [HttpPost]
        public Dictionary<string, object> GetCalvesByCowId(long id)
        {
            return cowContext.GetCalvesByCowId(id);
        }
        [HttpPost]
        public Dictionary<string, object> GetCalvingDetailByCowId(long id)
        {
            return cowContext.GetCalvingDetailByCowId(id);
        }
        
        [HttpPost]
        public Dictionary<string, object> SetCowMilking(long id, bool milking)
        {
            return cowContext.SetCowMilking(id,milking);
        }
        [HttpPost]
        public Dictionary<string, object> GetMilkingDetailByCowId(long cowId)
        {
            MilkingContext milkingContext = new MilkingContext(this._configuration);
            ArrayList ar = milkingContext.GetMilkingDetailByCowId(cowId);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["data"] = ar;
            data["recordsTotal"] = ar.Count;
            data["recordsFiltered"] = ar.Count;
            return data;
        }
        [HttpPost]
        public Dictionary<string, object> AddSellCow(SalePurchaseAnimal salePurchaseAnimal, BuyerSellerModal buyerSellerModal)
        {
             Dictionary<string,object> data;
            if(salePurchaseAnimal.AnimalId!=null){
                data = this.GetSellCowDetailByCowId((long)salePurchaseAnimal.AnimalId);
            }else{
                data = new Dictionary<string,object>();
                data["status"] = "failure";
            }
            if(data["status"]=="success"){
                SalePurchaseAnimal spa = (SalePurchaseAnimal)data["salePurchaseAnimal"];
                salePurchaseAnimal.Id =spa.Id;
                BuyerSellerModal bsd = (BuyerSellerModal)data["buyerSellerDetail"];
                buyerSellerModal.Id= bsd.Id;
                return cowContext.EditSellCow(salePurchaseAnimal,buyerSellerModal);
            }else{
                return cowContext.AddSellCow(salePurchaseAnimal,buyerSellerModal);
            }
            //return cowContext.AddSellCow(salePurchaseAnimal,buyerSellerModal);
        }
        [HttpPost]
        public Dictionary<string, object> GetSellCowDetailById(long id)
        {
            return cowContext.GetSellCowDetailById(id);
        }
        [HttpPost]
        public Dictionary<string, object> GetSellCowDetailByCowId(long cow_id)
        {
            return cowContext.GetSellCowDetailByCowId(cow_id);
        }
        [HttpPost]
        public Dictionary<string, object> EditSellCow(SalePurchaseAnimal salePurchaseAnimal, BuyerSellerModal buyerSellerModal)
        {
            return cowContext.EditSellCow(salePurchaseAnimal,buyerSellerModal);
        }
        [HttpPost]
        public Dictionary<string,object> GetDataForCowProfilePage()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //breeds
            AnimalBreedsContext animalBreedsContext = new AnimalBreedsContext(_configuration);
            AddressCountryContext addressCountryContext = new AddressCountryContext(_configuration);
            AddressCountryFilter addressCountryFilter = new AddressCountryFilter();
            
            AnimalBreedsFilter animalBreedsFilter = new AnimalBreedsFilter();
            animalBreedsFilter.PageNo = 1;
            animalBreedsFilter.RecordsPerPage = 50;
            Dictionary<long, string> breeds = animalBreedsContext.GetAnimalBreedsIdNamePair(animalBreedsFilter);
            data["breeds"] = breeds;
            //colors
            AnimalColorsContext animalColorsContext = new AnimalColorsContext(_configuration);
            AnimalColorsFilter animalColorsFilter = new AnimalColorsFilter();
            animalColorsFilter.PageNo = 1;
            animalColorsFilter.RecordsPerPage = 50;
            Dictionary<long, string> colors = animalColorsContext.GetAnimalColorsIdNamePair(animalColorsFilter);
            data["colors"] = colors;
            StaticData staticData = new StaticData();
            //pregnancyStatus
            data["pregnancyStatus"] = staticData.GetPregnancyStatusOptions();
            //pregnancyStatus 2 for conceive data
            data["pregnancyStatus2"] = staticData.GetPregnancyStatus2Options();
            //matingType
            data["matingType"] = staticData.GetMatingTypeOptions();
            //deliveryType
            data["deliveryType"] = staticData.GetDeliveryTypeOptions();
            //gender
            data["Gender"] = staticData.GetGendersOptions();
            //TeatsWorking
            data["teatsWorking"] = staticData.GetTeatsWorkingOptions();
            //CowLocation
            data["animalLocations"] = staticData.GetAnimalLocationsOptions();
            data["countries"] = addressCountryContext.GetAddressCountryIdNamePair(addressCountryFilter);
            return data;
        }
        [HttpGet]
        public Dictionary<string, object> Test()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<int, string> colors = cowContext.getColors();
            Dictionary<int, string> breeds = cowContext.getBreeds();
            data["colors"] = colors;
            data["breeds"] = breeds;
            return data;
        }
    }
}
