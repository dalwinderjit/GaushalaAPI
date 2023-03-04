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
    public class SalePurchaseAnimal
    {
        public Dictionary<string, string>? errors { get; set; }
        [Key]
        [Required(ErrorMessage = "Plese select 1")]
        public long? Id { get; set; }
        public long? AnimalId { get; set; }
        public decimal? Price { get; set; }
        public long? BuyerSellerId { get; set; }
        public DateTime? Date { get; set; }
        public long? SupervisorId { get; set; }
        public string? SupervisorName { get; set; }
        public string? SalePurchase { get; set; }
        public string? AnimalNo { get; set; }
        public string? Remarks { get; set; }
        
        public SalePurchaseAnimal()
        {
            
        }
        public bool ValidateSalePurchaseWithoudBuyerSellerId(string type="Add"){
            return this.ValidateSalePurchase(type,false);
        }
        public bool ValidateSalePurchase(string type="Add",bool buyerSellerIdCheck=true)
        {
            bool error = true;
            errors = new Dictionary<string, string>();
            Regex re;
            re = new Regex(@"^[a-zA-Z]{1,5}-[0-9]{1,}$");
            if(type=="Edit"){
                if(this.Id == null)
                {
                    errors.Add("Id", "Pleae Select the Record.");
                    error = false;
                }
            }
            re = new Regex(@"[0-9a-zA-Z ]{2,200}");
            if (this.AnimalId == null) {
                errors.Add("AnimalId", "Plasee Select the Animal");
                error = false;
            }
            if (this.Price == null)
            {
                errors.Add("Price", "Plasee enter the Price");
                error = false;
            }
            if(buyerSellerIdCheck==true){
                if (this.BuyerSellerId == null)
                {
                    errors.Add("BuyerSellerId", "Please enter the Buyer Seller Details");
                    error = false;
                }
            }
            if (this.Date == null)
            {
                errors.Add("Date", "Please enter the Date");
                error = false;
            }
            if (this.SupervisorId == null)
            {
                errors.Add("SupervisorId", "Please select the Super Visor");
                error = false;
            }
            /*if (this.SalePurchase == null)
            {
                errors.Add("SalePurchase", "Please select the Transaction Type Sale/Purchase");
                error = false;
            }*/
            /*if (this.AnimlaNo == null)
            {
                errors.Add("AnimlaNo", "Please enter the Animal Number");
                error = false;
            }*/
            return error;
        }
        
        public SalePurchaseAnimal(SqlDataReader sqlrdr) {
            
            Id = Convert.ToInt32(sqlrdr["Id"]);
            try
            {
                AnimalId = Convert.ToInt64(sqlrdr["AnimalId"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Price = Convert.ToDecimal(sqlrdr["Price"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                BuyerSellerId = Convert.ToInt64(sqlrdr["BuyerSellerId"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Date= DateTime.Parse(Convert.ToString(sqlrdr["Date"]));
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                SupervisorId = Convert.ToInt64(sqlrdr["SupervisorId"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                SalePurchase = Convert.ToString(sqlrdr["SalePurchase"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                AnimalNo = Convert.ToString(sqlrdr["AnimalNo"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Remarks = Convert.ToString(sqlrdr["Remarks"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
        }
        public Dictionary<string,object> GetFormatedSalePurchaseAnimal()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["animalId"] = this.AnimalId;
            data["animalNo"] = this.AnimalNo;
            data["buyerSellerId"] = this.BuyerSellerId;
            data["date"] = Helper.FormatDate3(this.Date);
            data["errors"] = this.errors;
            data["id"] = this.Id;
            data["price"] = this.Price;
            data["remarks"] = this.Remarks;
            data["salePurchase"] = this.SalePurchase;
            data["supervisorId"] = this.SupervisorId;
            data["supervisorName"] = this.SupervisorName;
            return data;
        }
    }
}
