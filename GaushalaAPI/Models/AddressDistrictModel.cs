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
    public class AddressDistrictModel 
    {
        public long? Id { get; set; }
        public string? District { get; set; }
        public long? StateID { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool? Deleted { get; set; }
        public Dictionary<string, string> errors { get; set; }
        public AddressDistrictModel()
        {

        }
        public AddressDistrictModel(IConfiguration configuration)
        {

        }
        public bool ValidateAnimalDistricts(string type = "Add") {
            bool error = true;
            if (type == "Edit")
            {
                if (this.Id == null)
                {
                    errors.Add("Id", "Please Select the Record");
                    error = false;
                }
            }
            if (this.District == null)
            {
                errors.Add("District", "Please enter the District");
                error = false;
            }
            if (this.StateID == null)
            {
                errors.Add("StateID", "Please select the State");
                error = false;
            }
            if (this.Created == null)
            {
                errors.Add("Created", "Please enter the Created Datetime");
                error = false;
            }
            return error;
        }
        public AddressDistrictModel(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            if (!Validations.IsNullOrEmpty(sqlrdr["District"])) { 
                District = sqlrdr["Description"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["StateID"])) {
                StateID = Convert.ToInt64(sqlrdr["StateID"]);
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
        static public Dictionary<string,object> GetFormatedDistrict(AddressDistrictModel addressDistrictModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Id"] = addressDistrictModel.Id;
            data["District"] = addressDistrictModel.District.ToString();
            data["StateID"] = Helper.IsNullOrEmpty(addressDistrictModel.StateID);
            data["Created"] = Helper.FormatDate3(addressDistrictModel.Created);
            data["Updated"] = Helper.FormatDate3(addressDistrictModel.Updated);
            data["District"] = addressDistrictModel.District.ToString();
            data["Deleted"] = Helper.IsNullOrEmpty(addressDistrictModel.Deleted);//.ToString();
            return data;
        }
    }
}
