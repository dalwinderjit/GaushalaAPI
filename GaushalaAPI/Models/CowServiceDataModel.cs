using GaushalaAPI.DBContext;
using GaushalaAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalAPI.Models
{
    public class CowServiceDataModel
    {
        public static IConfiguration _configuration;
        public AnimalModel animal;

        public string? Name { get; set; }
        public Dictionary<string, string> errors { get; set; }
        public long? Id{ get; set; }
        public long? CowID{ get; set; }
        public long? BullID{ get; set; }
        public string? CowNo{ get; set; }
        public string? BullSemenNo{ get; set; }
        public string? MatingProcessType{ get; set; }
        public string? PregnancyStatus{ get; set; }
        public DateTime? DateOfService{ get; set; }
        public string? DeliveryStatus{ get; set; }
        public DateTime? DeliveryDate{ get; set; }
        public long? AnimalID{ get; set; }
        public int? LactationNo{ get; set; }
        public decimal? BirthWeight{ get; set; }
        public decimal? BirthHeight{ get; set; }
        public string? Remarks{ get; set; }
        public string? AdditionalInfo{ get; set; }
        public decimal? DamWeight{ get; set; }
        public string? TagNo{ get; set; }
        public int? Colour { get; set; }
        public string? gender { get; set; }
        public int? Breed { get; set; }
        public int? Location { get; set; }

        public string? action { get; set; }
        public string? Deleted { get; set; }
        public int? DoctorID { get; set; }

        public IFormFile? formFile { get; set; }
        public CowServiceDataModel()
        {
            this.animal = new AnimalModel();
            this.errors = new Dictionary<string, string>();
        }
        public CowServiceDataModel(SqlDataReader sqlrdr)
        {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            try
            {
                CowID = Convert.ToInt64(sqlrdr["CowID"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                BullID = Convert.ToInt64(sqlrdr["BullID"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                CowNo = Convert.ToString(sqlrdr["CowNo"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                BullSemenNo = Convert.ToString(sqlrdr["BullSemenNo"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                MatingProcessType = Convert.ToString(sqlrdr["MatingProcessType"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                PregnancyStatus = Convert.ToString(sqlrdr["PregnancyStatus"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                DateOfService = (DateTime)(sqlrdr["DateOfService"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                DeliveryStatus = Convert.ToString(sqlrdr["DeliveryStatus"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                DeliveryDate = (DateTime)(sqlrdr["DeliveryDate"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                AnimalID = Convert.ToInt64(sqlrdr["AnimalID"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                LactationNo = Convert.ToInt32(sqlrdr["LactationNo"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                BirthWeight = Convert.ToDecimal(sqlrdr["BirthWeight"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Remarks = Convert.ToString(sqlrdr["Remarks"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                AdditionalInfo = Convert.ToString(sqlrdr["AdditionalInfo"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                DamWeight = Convert.ToDecimal(sqlrdr["DamWeight"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                TagNo = Convert.ToString(sqlrdr["TagNo"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            gender = "Male";
            action = "edit";
        }

        internal bool ValidateEdit()
        {
            bool error = false;
            if (this.Id == null)
            {
                this.errors["Id"] = "Please Select the Record"; error = true;
            }
            error = this.ValidateAdd();
            Console.WriteLine("Error" + error);
            return error;
        }

        internal bool ValidateAdd()
        {
            bool error = false;
            if (this.CowID == null)
            {
                errors["cowID"] = "Please Select the Cow"; error = true;

            }
            if(this.BullID == null)
            {
                errors["bullID"] = "Please Select the Bull"; error = true;
            }
            if(this.DateOfService == null)
            {
                errors["dateOfService"] = "Please Enter the Date of Service"; error = true;
            }
            if (this.PregnancyStatus == null)
            {
                errors["pregnancyStatus"] = "Please Select the Pregnancy Status"; error = true;
            }
            else if (this.PregnancyStatus == "4") {
                if (this.DeliveryStatus == null)
                {
                    errors["deliveryStatus"] = "Please Select the Delivery Status"; error = true;
                }else if (this.DeliveryStatus == "1" || this.DeliveryStatus == "2")
                {
                    AnimalModel ani = new AnimalModel();
                    if (this.LactationNo == null)
                    {
                        errors["lactationNo"] = "Please Enter the Lactation Number"; error = true;
                    } else if (this.LactationNo <= 0) {
                        errors["lactationNo"] = "Invalid Lactation Number"; error = true;
                    }
                    else if(this.DoLactationNumberExists(CowServiceDataModel._configuration,(long)this.CowID,(int)this.LactationNo,this.Id) ==false)
                    {
                        errors["lactationNo"] = "Lactation Number already Exists"; error = true;
                    }
                    if(this.DeliveryDate == null)
                    {
                        errors["deliveryDate"] = "Please Enter the Delivery Date"; error = true;
                    }
                    if(this.BirthWeight == null)
                    {
                        errors["birthWeight"] = "Please Enter the Birth Weight"; error = true;
                    }
                    if(this.BirthHeight== null)
                    {
                        errors["birthHeight"] = "Please Enter the Birth Height"; error = true;
                    }
                    if(this.DamWeight == null)
                    {
                        errors["damWeight"] = "Please Enter the Dams Weight"; error = true;
                    }
                    if(this.TagNo == null)
                    {
                        errors["tagNo"] = "Please Enter the Tag Number"; error = true;
                    }
                    else if (CowsContext.IsTagNoUnique_(CowServiceDataModel._configuration, this.TagNo, this.AnimalID) == false)
                    {
                        errors["tagNo"] = "Tag Number Already Occupied"; error = true;
                    }
                    if (this.Name== null)
                    {
                        errors["name"] = "Please Enter the Child Name"; error = true;
                    }
                    if(this.Breed == null)
                    {
                        errors["breed"] = "Please Select the breed"; error = true;
                    }
                    if (this.gender == null)
                    {
                        errors["gender"] = "Please Select the Gender"; error = true;
                    }
                    if (this.Location == null)
                    {
                        errors["location"] = "Please Select Location"; error = true;
                    }
                    if (this.formFile != null)
                    {
                        
                        if (this.gender == "Male")
                        {
                            this.animal.Category = "CALF";
                        }
                        else {
                            this.animal.Category = "HEIFER";
                         }
                        this.animal.SireID = this.CowID;
                        this.animal.DamID = this.BullID;
                        bool a = this.animal.ValidateImage();
                        if (a == false)
                        {
                            errors["picture"] = ani.errors["picture"].ToString();
                            error = false;
                        }
                    }
                }
                else if (this.DeliveryStatus == "3")
                {
                    if (this.DeliveryDate == null)
                    {
                        errors["deliveryDate"] = "Please Enter the Delivery Date"; error = true;
                    }
                }
            }
            if(this.MatingProcessType == null)
            {
                errors["matingProcessType"] = "Please Select the Mating Process Type"; error = true;
            }
            if(this.DoctorID == null)
            {
                errors["doctorID"] = "Please Select the Doctor"; error = true;
            }
            if(this.Remarks == null)
            {
                errors["remarks"] = "Please Enter Remarks"; error = true;
            }
            return !error;
            
        }

        public bool DoLactationNumberExists(IConfiguration _configuration, long cow_id,int lactation_number,long? Id)
        {
            bool result = CowsServiceContext.DoLactationNumberExists(_configuration,cow_id,lactation_number,Id);
            //select max(lacation_no) form cowcon where cow_id = cow_id;
            return result;
        }
    }
}
