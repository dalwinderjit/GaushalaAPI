using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using GaushalaAPI.DBContext;
using GaushalaAPI.Helpers;
using Microsoft.Extensions.Configuration;

namespace GaushalaAPI.Models
{
    public class AnimalBreedsModel 
    {
        public long? Id { get; set; }
        public string? Breed { get; set; }
        public bool? Deleted { get; set; }
        public Dictionary<string, string> errors { get; set; }
        public AnimalBreedsModel()
        {

        }
        public AnimalBreedsModel(IConfiguration configuration)
        {

        }
        public bool ValidateAnimalBreeds(string type = "Add") {
            bool error = true;
            if (type == "Edit")
            {
                if (this.Id == null)
                {
                    errors.Add("Id", "Please Select the Record");
                    error = false;
                }
            }
            if (this.Breed == null)
            {
                errors.Add("Breed", "Please enter the Name");
                error = false;
            }
            return error;
        }
        public AnimalBreedsModel(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            if (!Validations.IsNullOrEmpty(sqlrdr["Breed"])) { 
                Breed = sqlrdr["Description"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Deleted"]))
            {
                Deleted = Convert.ToBoolean(sqlrdr["Name"]);
            }
        }
        static public Dictionary<string,object> GetFormatedAnimalBreeds(AnimalBreedsModel vaccination)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Id"] = vaccination.Id;
            data["Breed"] = vaccination.Breed.ToString();
            data["Deleted"] = Helper.IsNullOrEmpty(vaccination.Deleted);//.ToString();
            return data;
        }
    }
}
