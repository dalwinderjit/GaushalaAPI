using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using GaushalaAPI.DBContext;
using System.Text.RegularExpressions;

namespace GaushalaAPI.Models
{
    public class BullModel : AnimalModel
    {
        public string DefaultPicture = "bull_default.jpg";
        public BullModel()
        {

        }
        public BullModel(SqlDataReader sqlrdr) : base(sqlrdr)
        {
            //base(sqlrdr);
        }
        public bool ValidateBull(BullsContext bullsContext, string type = "Add")
        {
            bool error = true;
            errors = new Dictionary<string, string>();
            if(type=="Edit"){
                if(this.Id==null){
                    errors.Add("id", "Pleae Select the Bull");
                    error = false;    
                }
            }
            Regex re;
            re = new Regex(@"^[a-zA-Z]{1,5}-[0-9]{1,}$");
            Console.WriteLine(this.TagNo);
            if(this.TagNo == null)
            {
                errors.Add("tagNo", "Pleae Enter Tag Number of Bull");
                error = false;
            }
            else if(re.IsMatch(this.TagNo)==false)
            {
                //Console.WriteLine("ERROR");
                errors.Add("tagNo", "Invalid Tag Number eg. SW-123"); error = false;
            }
            else if (bullsContext.isTagNoUnique(this.TagNo,this.Id)==false) {
                //Console.WriteLine("Unique ERROR");
                errors.Add("tagNo", "Tag Number Already Occupied"); 
                error = false;
            }
            else
            {
                //Console.WriteLine("ERROR PASSSED");
            }
            re = new Regex(@"[0-9a-zA-Z ]{2,255}");
            if (this.Name == null) {
                errors.Add("name", "Plasee Enter the name of bull");
                error = false;
            }else if (re.IsMatch(this.Name)==false)
            {
                error = false;
                errors.Add("name", "Invalid Name {2,255}");error = false;
            }
            if (this.Breed == null)
            {
                errors.Add("breed", "Plasee select Breed of bull");
                error = false;
            }
            
            
            if (this.DOB == null)
            {
                errors.Add("dob", "Enter the Date of Birth");
                error = false;
            }
            if (this.DamID == null)
            {
                errors.Add("damID", "Select the Dam of Bull");
                error = false;
            }
            if (this.SireID == null)
            {
                errors.Add("sireID", "Select the Sire of Bull");
                error = false;
            }
            if (this.Colour == null)
            {
                errors.Add("colour", "Select the colour of Bull");
                error = false;
            }
            if (this.Weight == null)
            {
                errors.Add("weight", "Enter the weight of Bull");
                error = false;
            }
            if (this.Height == null)
            {
                errors.Add("height", "Enter the Height of Bull");
                error = false;
            }
            
            this.ValidateImage();
            return error;
        }
    }
}
