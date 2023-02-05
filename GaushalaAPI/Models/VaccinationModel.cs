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
    public class VaccinationModel 
    {
        public long? ID { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public decimal? Qty{ get; set; }
        public string? State{ get; set; }
        public long? AddedBy{ get; set; }
        public DateTime? Created{ get; set; }
        public DateTime? Updated{ get; set; }
        public long? VaccinationBrandID{ get; set; }
        public string? VaccinationType{ get; set; }
        public decimal? AmountPerPiece{ get; set; }
        public Dictionary<string, string> errors { get; set; }
        public VaccinationModel()
        {

        }
        public VaccinationModel(IConfiguration configuration)
        {

        }
        public bool ValidateVaccination(string type = "Add") {
            bool error = true;
            if (type == "Edit")
            {
                if (this.ID == null)
                {
                    errors.Add("ID", "Please Select the Record");
                    error = false;
                }
            }
            if (this.Name == null)
            {
                errors.Add("Name", "Please enter the Name");
                error = false;
            }
            if (this.Description == null)
            {
                errors.Add("Description", "Please enter the Description");
                error = false;
            }
            if (this.Price == null)
            {
                errors.Add("Price", "Please enter the Price");
                error = false;
            }
            if (this.Qty == null)
            {
                errors.Add("Qty", "Please enter the Quantity");
                error = false;
            }
            if (this.State == null)
            {
                errors.Add("State", "Please select the State of Medicine(SOLID, LIQUID, GAS).");
                error = false;
            }
            if (this.VaccinationBrandID == null)
            {
                errors.Add("VaccinationBrandID", "Please Select the Vaccination Brand.");
                error = false;
            }
            if (this.VaccinationType== null)
            {
                errors.Add("VaccinationType", "Please Select the Vaccination Type(Allopathy,Homeopathy,Ayurvedic)");
                error = false;
            }
            if (this.AmountPerPiece== null)
            {
                errors.Add("AmountPerPiece", "Please Enter the Amount Per Piece");
                error = false;
            }
            return error;
        }
        public VaccinationModel(SqlDataReader sqlrdr) {
            ID = Convert.ToInt32(sqlrdr["Id"]);
            if (!Validations.IsNullOrEmpty(sqlrdr["Description"])) { 
                Description = sqlrdr["Description"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Name"]))
            {
                Name = sqlrdr["Name"].ToString();
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Price"]))
            {
                Price = Convert.ToDecimal(sqlrdr["Price"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["Qty"]))
            {
                Qty = Convert.ToDecimal(sqlrdr["Qty"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["AmountPerPiece"]))
            {
                AmountPerPiece = Convert.ToDecimal(sqlrdr["AmountPerPiece"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["State"]))
            {
                State = Convert.ToString(sqlrdr["State"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["AddedBy"]))
            {
                AddedBy = Convert.ToInt64(sqlrdr["AddedBy"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["VaccinationBrandID"]))
            {
                VaccinationBrandID= Convert.ToInt64(sqlrdr["VaccinationBrandID"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["VaccinationType"]))
            {
                VaccinationType = Convert.ToString(sqlrdr["VaccinationType"]);
            }
            if (!Validations.IsNullOrEmpty(sqlrdr["AmountPerPiece"]))
            {
                AmountPerPiece = Convert.ToDecimal(sqlrdr["AmountPerPiece"]);
            }
        }
        static public Dictionary<string,object> GetFormatedVaccination(VaccinationModel vaccination)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["ID"] = vaccination.ID;
            data["Name"] = vaccination.Name;
            data["Description"] = Helper.IsNullOrEmpty(vaccination.Description);//.ToString();
            data["Price"] = vaccination.Price;
            data["Qty"] = vaccination.Qty;
            data["Created"] = Helper.FormatDate3(vaccination.Created);
            data["Updated"] = Helper.FormatDate3(vaccination.Updated);
            data["VaccinationBrandID"] = vaccination.VaccinationBrandID;
            data["VaccinationType"] = vaccination.VaccinationType;
            data["AmountPerPiece"] = vaccination.AmountPerPiece;
            return data;
        }
    }
}
