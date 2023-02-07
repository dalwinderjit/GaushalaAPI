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

namespace GaushalaAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    public class BullsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        BullsContext bullsContext = null;
        CowsContext cowsContext = null;
        public BullsController(IConfiguration configuration)
        {
            _configuration = configuration;
            bullsContext = new BullsContext(_configuration);
            cowsContext = new CowsContext(_configuration);
            // d1.SetConfiguration(_configuration);
        }
        [HttpGet]
        public Dictionary<string,object> GetDataForBullProfile()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<int,string> colors = cowsContext.getColors();
            Dictionary<int, string> breeds = cowsContext.getBreeds();
            data["colors"] = colors;
            data["breeds"] = breeds;
            return data;
        }
        [HttpPost(Name = "SaveBull")]
        public Dictionary<string, Dictionary<string, string>> SaveBull([FromForm] BullModel bull) {
            if (ModelState.IsValid || true) {
                //Console.WriteLine("HELO");
                return bullsContext.SaveBull(bull);
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
        [HttpPost(Name = "AddBull")]
        public Dictionary<string, Dictionary<string, string>> AddBull([FromForm] BullModel bull)
        {
            bull.Id = 0;
            if (ModelState.IsValid)
            {
                //Console.WriteLine("HELO");
                Console.WriteLine("SD"+bull.SemenDoses);
                return bullsContext.AddBull(bull);
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
        // GET: HomeController
        [HttpPost(Name = "GetBulls1")]
        public Dictionary<string, object> Get(BullFilterModel bullFilters)
        {
            Dictionary<string, object> collection = new Dictionary<string, object>();
            collection.Add("data", bullsContext.GetBulls(bullFilters));
            collection.Add("recordsTotal", bullsContext.GetTotalBulls());
            collection.Add("recordsFiltered", bullsContext.GetTotalFilteredBulls(bullFilters));
            return collection;
            //return bullsContext.GetBulls();
        }
        //[EnableCors(origins: "http://127.0.0.1:4000/", headers: "*", methods: "get,post")]
        [HttpPost(Name = "GetSires1")]
        public Dictionary<string, object> GetSires(BullFilterModel bullFilters)
        {
            Dictionary<string, object> collection = new Dictionary<string, object>();
            //exclude itself            
            collection.Add("data", cowsContext.GetSires(bullFilters));
            collection.Add("recordsTotal", cowsContext.GetTotalSires());
            collection.Add("recordsFiltered", cowsContext.GetTotalFilteredSires(bullFilters));
            return collection;
        }
        [HttpPost]
        public Dictionary<int, string> GetBullsIDNamePairByTagNo(string tagNo,int pageNo)
        {
            //return "HELLO";
            return bullsContext.GetBullsIdNamePairByTagNo(tagNo,pageNo);
        }
        [HttpGet("{id}")]
        public Dictionary<string,object> GetBullById(int id) {
            return BullModel.GetFormatedBullData(bullsContext.GetBullById(id));
        }
        [HttpPost]
        public Dictionary<string, object> GetCalvingDetailByBullId(long id)
        {
            return bullsContext.GetCalvingDetailByBullId(id);
        }
        [HttpPost]
        public Dictionary<string, object> AddSellBull(SalePurchaseAnimal salePurchaseAnimal, BuyerSellerModal buyerSellerModal)
        {
            Dictionary<string,object> data;
            if(salePurchaseAnimal.AnimalId!=null){
                data = this.GetSellBullDetailByBullId((long)salePurchaseAnimal.AnimalId);
            }else{
                data = new Dictionary<string,object>();
                data["status"] = "failure";
            }
            if(data["status"]=="success"){
                SalePurchaseAnimal spa = (SalePurchaseAnimal) data["salePurchaseAnimal"];
                salePurchaseAnimal.Id =spa.Id;
                BuyerSellerModal bsd = (BuyerSellerModal)data["buyerSellerDetail"];
                buyerSellerModal.Id= bsd.Id;
                return bullsContext.EditSellBull(salePurchaseAnimal,buyerSellerModal);
            }else{
                return bullsContext.AddSellBull(salePurchaseAnimal,buyerSellerModal);
            }
        }
        [HttpPost]
        public Dictionary<string, object> GetSellBullDetailById(long id)
        {
            return bullsContext.GetSellBullDetailById(id);
        }
        [HttpPost]
        public Dictionary<string, object> GetSellBullDetailByBullId(long bull_id)
        {
            return bullsContext.GetSellBullDetailByBullId(bull_id);
        }
        [HttpPost]
        public Dictionary<string, object> EditSellBull(SalePurchaseAnimal salePurchaseAnimal, BuyerSellerModal buyerSellerModal)
        {
            return bullsContext.EditSellBull(salePurchaseAnimal,buyerSellerModal);
        }
        [HttpPost]
        public Dictionary<string, object> GetDataForBullProfilePageAPI()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //breeds
            AnimalBreedsContext animalBreedsContext = new AnimalBreedsContext(_configuration);
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
            //performance
            Dictionary<string, string> performance = new Dictionary<string, string>();
            performance["SEMEN"] = "Artificial Intelligence";
            performance["NATURAL"] = "Natural";
            data["performance"] = performance;
            //BullLocation
            Dictionary<long, string> animalLocation = new Dictionary<long, string>();
            animalLocation[1] = "Shed A";
            animalLocation[2] = "Shed B";
            animalLocation[3] = "Shed C";
            animalLocation[4] = "Shed D";
            animalLocation[5] = "Shed E";
            data["animalLocations"] = animalLocation;
            return data;
        }
    }
}
