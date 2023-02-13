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
    public class AddressCountryModel 
    {
        public long? Id { get; set; }
        public string? Country { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool? Deleted { get; set; }
        public Dictionary<string, string> errors { get; set; }
        public AddressCountryModel()
        {

        }
        public AddressCountryModel(IConfiguration configuration)
        {

        }
        public bool ValidateAddressCountry(string type = "Add") {
            bool error = true;
            if (type == "Edit")
            {
                if (this.Id == null)
                {
                    errors.Add("Id", "Please Select the Record");
                    error = false;
                }
            }
            if (this.Country == null)
            {
                errors.Add("Country", "Please enter the Country");
                error = false;
            }
            if (this.Created == null)
            {
                errors.Add("Created", "Please enter the Created Datetime");
                error = false;
            }
            return error;
        }
        public AddressCountryModel(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            if (!Validations.IsNullOrEmpty(sqlrdr["Country"])) { 
                Country = sqlrdr["Country"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Deleted"]))
            {
                Deleted = Convert.ToBoolean(sqlrdr["Deleted"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Created"]))
            {
                Created = (DateTime)sqlrdr["Created"];
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Updated"]))
            {
                Updated = (DateTime)sqlrdr["Updated"];
            }
        }
        static public Dictionary<string,object> GetFormatedAddressCountry(AddressCountryModel addressCountryModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Id"] = addressCountryModel.Id;
            data["Country"] = addressCountryModel.Country.ToString();
            data["Created"] = Helper.FormatDate3(addressCountryModel.Created);
            data["Updated"] = Helper.FormatDate3(addressCountryModel.Updated);
            data["Country"] = addressCountryModel.Country.ToString();
            data["Deleted"] = Helper.IsNullOrEmpty(addressCountryModel.Deleted);//.ToString();
            return data;
        }
    }
}
