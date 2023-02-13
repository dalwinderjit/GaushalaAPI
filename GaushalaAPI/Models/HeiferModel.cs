using GaushalaAPI.DBContext;
using GaushalaAPI.Helpers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace GaushalaAPI.Models
{
    public class HeiferModel : AnimalModel
    {
        public string DefaultPicture = "heifer_default.jpg";
        public HeiferModel()
        {

        }
        public HeiferModel(SqlDataReader sqlrdr) : base(sqlrdr)
        {
            //base(sqlrdr);
        }
        public bool ValidateHeifer(HeifersContext heifersContext, string type = "Add")
        {
            bool error = true;
            errors = new Dictionary<string, string>();
            Regex re;
            re = new Regex(Validations.ValidTagNoRegEx);
            Console.WriteLine(this.TagNo);
            if (type == "Edit" || type=="Update") {
                if (this.Id == null)
                {
                    errors.Add("Id", "Pleae Select the Heifer");
                    error = false;
                }
            }
            if(this.TagNo == null)
            {
                errors.Add("tagNo", "Pleae Enter Tag Number of Heifer");
                error = false;
            }
            else if(re.IsMatch(this.TagNo)==false)
            {
                //Console.WriteLine("ERROR");
                errors.Add("tagNo", "Invalid Tag Number eg. SW-123"); error = false;
            }
            else if (heifersContext.isTagNoUnique(this.TagNo)==false) {
                //Console.WriteLine("Unique ERROR");
                errors.Add("tagNo", "Tag Number Already Occupied"); 
                error = false;
            }
            re = new Regex(@"[0-9a-zA-Z ]{2,255}");
            if (this.Name == null) {
                errors.Add("name", "Plasee Enter the name of heifer");
                error = false;
            }else if (re.IsMatch(this.Name)==false)
            {
                errors.Add("name", "Invalid Name {2,255}");error = false;
                error = false;
            }
            if (this.Breed == null)
            {
                errors.Add("breed", "Plasee select Breed of heifer");
                error = false;
            }
            if (this.DOB == null)
            {
                errors.Add("dob", "Enter the Date of Birth");
                error = false;
            }
            if (this.DamID == null)
            {
                if(this.DamNo==null && this.DamName == null)
                {
                    errors.Add("damID", "Select the Dam of Heifer or Enter the DamName/TagNumber");
                    error = false;
                }
                else
                {
                    re = new Regex(Validations.ValidTagNoRegEx);
                    //validate DamNo
                    if (this.DamNo == null)
                    {
                        errors.Add("DamNo", "Pleae Enter Tag Number of Dam");
                        error = false;
                    }
                    else if (re.IsMatch(this.DamNo) == false)
                    {
                        //Console.WriteLine("ERROR");
                        errors.Add("DamNo", "Invalid Dam's Tag Number eg. SW-123"); error = false;
                    }
                    else if (heifersContext.isTagNoUnique(this.DamNo) == false)
                    {
                        //Console.WriteLine("Unique ERROR");
                        errors.Add("DamNo", "Dam's Tag Number Already Occupied");
                        error = false;
                    }
                }
            }
            if (this.SireID == null)
            {
                if (this.SireNo == null && this.SireName == null)
                {
                    errors.Add("sireID", "Select the Sire of Heifer or Enter the Name/Tag No of Sire");
                    error = false;
                }
                else
                {
                    re = new Regex(Validations.ValidTagNoRegEx);
                    //validate SireNo
                    if (this.SireNo == null)
                    {
                        errors.Add("SireNo", "Pleae Enter Tag Number of Sire");
                        error = false;
                    }
                    else if (re.IsMatch(this.SireNo) == false)
                    {
                        //Console.WriteLine("ERROR");
                        errors.Add("SireNo", "Invalid Sire's Tag Number eg. SW-123"); error = false;
                    }
                    else if (heifersContext.isTagNoUnique(this.SireNo) == false)
                    {
                        //Console.WriteLine("Unique ERROR");
                        errors.Add("SireNo", "Sire's Tag Number Already Occupied");
                        error = false;
                    }
                }
            }
            if (this.Colour == null)
            {
                errors.Add("colour", "Select the colour of Heifer");
                error = false;
            }
            if (this.Weight == null)
            {
                errors.Add("weight", "Enter the weight of Heifer");
                error = false;
            }
            if (this.Height == null)
            {
                errors.Add("height", "Enter the Height of Heifer");
                error = false;
            }
            if (this.BirthLactationNumber == null)
            {
                errors.Add("birthLactationNumber", "Enter the BirthLactationNumber of Heifer");
                error = false;
            } else if(this.DamID!=null) {
                if(type=="Update" || type == "Edit")
                {
                    if (!heifersContext.IsBirthLactionNumberUnique((int)this.BirthLactationNumber, (long)this.DamID, this.Id))
                    {
                        errors.Add("birthLactationNumber", "BirthLactationNumber Occupied");
                        error = false;
                    }
                }
                else if (!heifersContext.IsBirthLactionNumberUnique((int)this.BirthLactationNumber, (long)this.DamID))
                {
                    errors.Add("birthLactationNumber", "BirthLactationNumber Occupied");
                    error = false;
                }
            }
            if (this.Location == null)
            {
                errors.Add("Location", "Select the Location of Heifer");
                error = false;
            }
            if (this.ValidateImage() == false)
            {
                error = false;
            };
            return error;
        }
        public static Dictionary<string, object> GetFormatedHeiferData(HeiferModel? heifer_)
        {
            if (heifer_ != null)
            {
                Dictionary<string, object> cow = new Dictionary<string, object>();
                cow["id"] = heifer_.Id;
                cow["tagNo"] = heifer_.TagNo;
                cow["name"] = heifer_.Name;
                if (heifer_.DOB != null)
                {
                    cow["dob"] = ((DateTime)heifer_.DOB).ToString("yyyy-MM-dd HH:mm:ss");// "2004-12-01T00:00:00",
                }
                else
                {
                    cow["dob"] = heifer_.DOB.ToString();
                }
                cow["category"] = heifer_.Category;
                cow["gender"] = heifer_.Gender;
                cow["breed"] = heifer_.Breed_;
                cow["colour"] = heifer_.Colour;
                cow["colour_"] = heifer_.Colour_;
                cow["sireID"] = heifer_.SireID;
                cow["damID"] = heifer_.DamID;
                cow["sireNo"] = heifer_.SireNo;
                cow["sireName"] = heifer_.SireName;
                cow["damNo"] = heifer_.DamNo;
                cow["damName"] = heifer_.DamName;
                cow["dbly"] = heifer_.DBLY;
                cow["sdbly"] = heifer_.SDBLY;
                cow["butterFat"] = heifer_.ButterFat;
                cow["pregnancyStatus"] = heifer_.PregnancyStatus;
                cow["status"] = heifer_.Status;
                cow["reproductiveStatus"] = heifer_.ReproductiveStatus;
                cow["milkingStatus"] = heifer_.MilkingStatus;
                cow["remarks"] = heifer_.Remarks;
                cow["additionalInfo"] = heifer_.AdditionalInfo;
                if (heifer_.Picture != null && heifer_.Picture.Trim() != "")
                {
                    cow["picture"] = heifer_.Picture;
                }
                else
                {
                    cow["picture"] = heifer_.DefaultPicture;
                }

                cow["lactation"] = heifer_.Lactation;
                cow["type"] = heifer_.Type;
                cow["weight"] = heifer_.Weight;
                cow["height"] = heifer_.Height;
                cow["semenDoses"] = heifer_.SemenDoses;
                cow["noOfTeatsWorking"] = heifer_.NoOfTeatsWorking;
                cow["location"] = heifer_.Location;
                cow["sold"] = heifer_.Sold;
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["success"] = true;
                data["data"] = cow;
                data["message"] = "Cow Found";
                return data;
            }
            else
            {
                Dictionary<string, object> data = new Dictionary<string, object>();
                data["success"] = false;
                data["message"] = "Cow Not Found";

                return data;
            }

        }
    }
}
