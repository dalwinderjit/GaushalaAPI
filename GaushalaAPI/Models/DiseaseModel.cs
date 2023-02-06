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
    public class DiseaseModel 
    {
        public long? Id { get; set; }
        public string? DiseaseName { get; set; }
        public string? Description { get; set; }
        public long? AddedBy{ get; set; }
        public DateTime? Created{ get; set; }
        public DateTime? Updated{ get; set; }
        public Dictionary<string, string> errors { get; set; }
        public DiseaseModel()
        {

        }
        public DiseaseModel(IConfiguration configuration)
        {

        }
        public bool ValidateDisease(string type = "Add") {
            bool error = true;
            if (type == "Edit")
            {
                if (this.Id == null)
                {
                    errors.Add("ID", "Please Select the Record");
                    error = false;
                }
            }
            if (this.DiseaseName == null)
            {
                errors.Add("DiseaseName", "Please enter the Disease Name");
                error = false;
            }
            if (this.Description == null)
            {
                errors.Add("Description", "Please enter the Description");
                error = false;
            }
            return error;
        }
        public DiseaseModel(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            if (!Validations.IsNullOrEmpty(sqlrdr["Description"])) { 
                Description = sqlrdr["Description"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["DiseaseName"]))
            {
                DiseaseName = sqlrdr["DiseaseName"].ToString();
            }
            
            if (!Validations.IsNullOrEmpty(sqlrdr["AddedBy"]))
            {
                AddedBy = Convert.ToInt64(sqlrdr["AddedBy"]);
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
        static public Dictionary<string,object> GetFormatedDisease(DiseaseModel disease)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["ID"] = disease.Id;
            data["DiseaseName"] = disease.DiseaseName;
            data["Description"] = Helper.IsNullOrEmpty(disease.Description);//.ToString();
            data["Created"] = Helper.FormatDate3(disease.Created);
            data["Updated"] = Helper.FormatDate3(disease.Updated);
            return data;
        }
    }
}
