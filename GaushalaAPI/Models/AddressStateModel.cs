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
    public class AddressStateModel 
    {
        public long? Id { get; set; }
        public string? State { get; set; }
        public long? CountryID { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public bool? Deleted { get; set; }
        public Dictionary<string, string> errors { get; set; }
        public AddressStateModel()
        {
            errors = new Dictionary<string, string>();
        }
        public AddressStateModel(IConfiguration configuration)
        {

        }
        public bool ValidateAddressState(string type = "Add") {
            bool error = true;
            if (type == "Edit")
            {
                if (this.Id == null)
                {
                    errors.Add("Id", "Please Select the Record");
                    error = false;
                }
            }
            if (this.State == null)
            {
                errors.Add("State", "Please enter the State");
                error = false;
            }
            if (this.CountryID == null)
            {
                errors.Add("CountryID", "Please select the Country");
                error = false;
            }
            if (this.Created == null)
            {
                errors.Add("Created", "Please enter the Created Datetime");
                error = false;
            }
            return error;
        }
        public AddressStateModel(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            if (!Validations.IsNullOrEmpty(sqlrdr["State"])) { 
                State = sqlrdr["State"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["CountryID"])) {
                CountryID = Convert.ToInt64(sqlrdr["CountryID"]);
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
        static public Dictionary<string,object> GetFormatedAddressState(AddressStateModel addressStateModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["Id"] = addressStateModel.Id;
            data["State"] = addressStateModel.State.ToString();
            data["CountryID"] = Helper.IsNullOrEmpty(addressStateModel.CountryID);
            data["Created"] = Helper.FormatDate3(addressStateModel.Created);
            data["Updated"] = Helper.FormatDate3(addressStateModel.Updated);
            data["State"] = addressStateModel.State.ToString();
            data["Deleted"] = Helper.IsNullOrEmpty(addressStateModel.Deleted);//.ToString();
            return data;
        }
    }
}
