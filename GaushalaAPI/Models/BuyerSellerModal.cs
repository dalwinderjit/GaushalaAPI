using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace GaushalaAPI.Models
{
    public class BuyerSellerModal
    {
        public Dictionary<string, string>? errors { get; set; }
        [Key]
        [Required(ErrorMessage = "Plese select 1")]
        public long? Id { get; set; }
        public string? Name { get; set; }
        public int? Country { get; set; }
        public int? State { get; set; }
        public int? District { get; set; }
        public int? Tehsil { get; set; }
        public string? VillMohalla { get; set; }
        public string? StreetNo { get; set; }
        public string? HouseNo { get; set; }
        public string? PIN { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        
        public BuyerSellerModal()
        {
            
        }
        
        public bool ValidateBuyerSellerModal(string type="Add")
        {
            bool error = true;
            errors = new Dictionary<string, string>();
            Regex re;
            re = new Regex(@"^[a-zA-Z]{1,5}-[0-9]{1,}$");
            if(type=="Edit"){
                if(this.Id == null)
                {
                    errors.Add("Id", "Pleae Select the Record 123.");
                    error = false;
                }
            }
            re = new Regex(@"[0-9a-zA-Z ]{2,200}");
            if (this.Name == null) {
                errors.Add("Name", "Plasee Enter the name of User");
                error = false;
            }else if (re.IsMatch(this.Name)==false)
            {
                error = false;
                errors.Add("Name", "Invalid Name {2,200}");error = false;
            }
            if (this.Country == null)
            {
                errors.Add("Country", "Plasee select the Country");
                error = false;
            }
            
            if (this.State == null)
            {
                errors.Add("State", "Please select the State");
                error = false;
            }
            if (this.District == null)
            {
                errors.Add("District", "Please select the District");
                error = false;
            }
            if (this.Tehsil == null)
            {
                errors.Add("Tehsil", "Please select the Tehsil");
                error = false;
            }
            if (this.VillMohalla == null)
            {
                errors.Add("VillMohalla", "Please select the Vill/Mohalla");
                error = false;
            }
            if (this.StreetNo == null)
            {
                errors.Add("StreetNo", "Enter the Street Number");
                error = false;
            }
            if (this.HouseNo == null)
            {
                errors.Add("damIDHouseNo", "Enter the House Number");
                error = false;
            }
            if (this.PIN == null)
            {
                errors.Add("PIN", "Enter the PIN");
                error = false;
            }
            if (this.PhoneNumber == null)
            {
                errors.Add("PhoneNumber", "Enter the Phone Number");
                error = false;
            }
            if (this.Email == null)
            {
                errors.Add("Email", "Enter the Email");
                error = false;
            }
            return error;
        }
        
        public BuyerSellerModal(SqlDataReader sqlrdr) {
            
            Id = Convert.ToInt32(sqlrdr["Id"]);
            try
            {
                Name = Convert.ToString(sqlrdr["Name"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Country = Convert.ToInt32(sqlrdr["Country"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                State = Convert.ToInt32(sqlrdr["State"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                District = Convert.ToInt32(sqlrdr["District"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Tehsil = Convert.ToInt32(sqlrdr["Tehsil"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                VillMohalla = Convert.ToString(sqlrdr["Vill_Mohalla"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                StreetNo = Convert.ToString(sqlrdr["StreetNo"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                HouseNo = Convert.ToString(sqlrdr["HouseNo"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                PIN = Convert.ToString(sqlrdr["PIN"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                PhoneNumber = Convert.ToString(sqlrdr["PhoneNumber"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Email = Convert.ToString(sqlrdr["Email"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
        }
    }
}
