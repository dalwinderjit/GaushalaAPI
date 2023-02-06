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
    public class VaccinationBrandModel
    {
        public long? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Created{ get; set; }
        public DateTime? Updated{ get; set; }
        public Dictionary<string, string> errors { get; set; }
        public VaccinationBrandModel()
        {

        }
        public VaccinationBrandModel(IConfiguration configuration)
        {

        }
        public bool ValidateVaccinationBrand(string type = "Add") {
            bool error = true;
            if (type == "Edit")
            {
                if (this.Id == null)
                {
                    errors.Add("ID", "Please Select the Record");
                    error = false;
                }
            }
            if (this.Title == null)
            {
                errors.Add("Title ", "Please enter the Brand Title");
                error = false;
            }
            if (this.Description == null)
            {
                errors.Add("Description", "Please enter the Brand Description");
                error = false;
            }
            return error;
        }
        public VaccinationBrandModel(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            if (!Validations.IsNullOrEmpty(sqlrdr["Description"])) { 
                Description = sqlrdr["Description"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Title"]))
            {
                Title = sqlrdr["Title"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Created"]))
            {
                Created = (DateTime)(sqlrdr["Created"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Updated"]))
            {
                Updated = (DateTime)(sqlrdr["Updated"]);
            }

        }
        static public Dictionary<string,object> GetFormatedVaccinationBrand(VaccinationBrandModel vaccinationBrand)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Id"] = vaccinationBrand.Id;
            data["Title"] = vaccinationBrand.Title;
            data["Description"] = Helper.IsNullOrEmpty(vaccinationBrand.Description);//.ToString();
            data["Created"] = Helper.FormatDate3(vaccinationBrand.Created);
            data["Updated"] = Helper.FormatDate3(vaccinationBrand.Updated);
            return data;
        }
    }
}
