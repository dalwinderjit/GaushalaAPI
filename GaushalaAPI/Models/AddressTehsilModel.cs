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
    public class AddressTehsilModel 
    {
        public long? Id { get; set; }
        public string? Tehsil { get; set; }
        public long? DistrictID { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool? Deleted { get; set; }
        public Dictionary<string, string> errors { get; set; }
        public AddressTehsilModel()
        {

        }
        public AddressTehsilModel(IConfiguration configuration)
        {

        }
        public bool ValidateAnimalTehsils(string type = "Add") {
            bool error = true;
            if (type == "Edit")
            {
                if (this.Id == null)
                {
                    errors.Add("Id", "Please Select the Record");
                    error = false;
                }
            }
            if (this.Tehsil == null)
            {
                errors.Add("Tehsil", "Please enter the Tehsil");
                error = false;
            }
            if (this.DistrictID == null)
            {
                errors.Add("DistrictID", "Please select the State");
                error = false;
            }
            if (this.Created == null)
            {
                errors.Add("Created", "Please enter the Created Datetime");
                error = false;
            }
            return error;
        }
        public AddressTehsilModel(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            if (!Validations.IsNullOrEmpty(sqlrdr["Tehsil"])) { 
                Tehsil = sqlrdr["Description"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["DistrictID"])) {
                DistrictID = Convert.ToInt64(sqlrdr["DistrictID"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Deleted"]))
            {
                Deleted = Convert.ToBoolean(sqlrdr["Name"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Created"]))
            {
                Created = (DateTime)sqlrdr["Crearted"];
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Updated"]))
            {
                Updated = (DateTime)sqlrdr["Updated"];
            }
        }
        static public Dictionary<string,object> GetFormatedTehsil(AddressTehsilModel addressTehsilModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Id"] = addressTehsilModel.Id;
            data["Tehsil"] = addressTehsilModel.Tehsil.ToString();
            data["DistrictID"] = Helper.IsNullOrEmpty(addressTehsilModel.DistrictID);
            data["Created"] = Helper.FormatDate3(addressTehsilModel.Created);
            data["Updated"] = Helper.FormatDate3(addressTehsilModel.Updated);
            data["Tehsil"] = addressTehsilModel.Tehsil.ToString();
            data["Deleted"] = Helper.IsNullOrEmpty(addressTehsilModel.Deleted);//.ToString();
            return data;
        }
    }
}
