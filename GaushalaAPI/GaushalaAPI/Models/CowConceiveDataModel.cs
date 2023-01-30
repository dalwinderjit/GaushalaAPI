using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalAPI.Models
{
    public class CowConceiveDataModel
    {
        
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
        public string? BirthWeight{ get; set; }
        public string? Remarks{ get; set; }
        public string? AdditionalInfo{ get; set; }
        public string? DamWeight{ get; set; }
        public string? TagNo{ get; set; }
    
        public string? gender { get; set; }

        public string? action { get; set; }
        public string? Deleted { get; set; }
        public int? DoctorID { get; set; }
        public CowConceiveDataModel()
        {
            this.errors = new Dictionary<string, string>();
        }
        public CowConceiveDataModel(SqlDataReader sqlrdr)
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
                BirthWeight = Convert.ToString(sqlrdr["BirthWeight"]);
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
                DamWeight = Convert.ToString(sqlrdr["DamWeight"]);
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
                errors["DateOfService"] = "Please Enter the Date of Service"; error = true;
            }
            if(this.PregnancyStatus == null)
            {
                errors["PregnancyStatus"] = "Please Select the Pregnancy Status"; error = true;
            }
            if(this.MatingProcessType == null)
            {
                errors["MatingProcessType"] = "Please Select the Mating Process Type"; error = true;
            }
            if(this.DoctorID == null)
            {
                errors["DoctorID"] = "Please Select the Doctor"; error = true;
            }
            if(this.Remarks == null)
            {
                errors["Remarks"] = "Please Enter Remarks"; error = true;
            }
            return !error;
            
        }
    }
}
