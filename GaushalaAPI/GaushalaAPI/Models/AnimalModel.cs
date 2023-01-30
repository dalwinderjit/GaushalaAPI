using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace GaushalaAPI.Models
{
    public class AnimalModel
    {
        public Dictionary<string, string>? errors { get; set; }
        public long? Id { get; set; }
        public string? TagNo { get; set; }
        public string? Name { get; set; }
        public DateTime? DOB { get; set; }
        public string? Category { get; set; }
        public string? Gender { get; set; }
        public int? Breed { get; set; }
        public string? Breed_ { get; set; }
        public string? Colour_ { get; set; }
        public int? Colour { get; set; }
        public long? SireID { get; set; }
        public long? DamID { get; set; }
        public string? SireNo { get; set; }
        public string? SireName { get; set; }
        public string? DamNo { get; set; }
        public string? DamName { get; set; }
        public string? DBLY { get; set; }
        public string? SDBLY { get; set; }
        public decimal? ButterFat { get; set; }
        public bool? PregnancyStatus { get; set; }
        public bool? Status { get; set; }
        public bool? ReproductiveStatus { get; set; }
        public bool? MilkingStatus { get; set; }
        public string? Remarks { get; set; }
        public string? AdditionalInfo { get; set; }
        public string? Picture { get; set; }
        public int? Lactation { get; set; }
        public int? BirthLactationNumber { get; set; }
        public string? Type { get; set; }
        //private int? _semenDoses;
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public int? SemenDoses { get; set; }
        public bool? Sold { get; set; }
        public int? NoOfTeatsWorking {get ; set; }
        public int? Location { get; set; }
        public IFormFile? formFile { get; set; }
        public bool? BelongsToGaushala { get; set; }
        public bool AnimalFound {get; set;}
        public AnimalModel()
        {
            this.AnimalFound = false;
        }
        public Boolean ValidateImage()
        {
            int ImageSize = 1024 * 200;   //200KB
            if (formFile != null)
            {
                string[] supportedTypes = { ".jpg", ".jpeg", ".JPG", ".JPEG", ".png", ".PNG" };
                string extension = System.IO.Path.GetExtension(formFile.FileName);
                //Console.WriteLine("EXT" + extension);
                if (!Array.Exists(supportedTypes, element => element == extension))
                {
                    this.errors.Add("picture", "Not valid Image");
                    return false;
                }
                if (formFile.Length > ImageSize)
                {
                    this.errors.Add("picture", $"Image size too large ({formFile.Length})");
                    return false;
                }
                if (formFile.Length <= 0)
                {
                    this.errors.Add("picture", "Image size too small");
                    return false;
                }
                return true;
            }
            else
            {
                return true;
            }
        }
        public bool ValidateCow(string type="Add")
        {
            return true;
        }
        public string SaveImage()
        {
            if (formFile != null)
            {
                string[] supportedTypes = { ".jpg", ".jpeg",".JPG", ".JPEG", ".png" ,".PNG"};
                string extension = System.IO.Path.GetExtension(formFile.FileName);
                //Console.WriteLine("EXT"+extension);
                if (!Array.Exists(supportedTypes,element=> element==extension) ){
                    return "Extension not matched";
                }
                if (formFile.Length > 0)
                {
                    //string path = Path.Combine(@"D:\dalwinder\projects\djjs-gaushala\soruce-code\gaushalaTemplate\", "images");
                    //string path = Path.Combine(@"F:\PROJECTS\djjs-gaushala\soruce-code\html\gaushalaTemplate\", "images");
                    string path = "images";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    //var filePath = Path.GetTempFileName();
                    int i = 0;
                    string fileName_ =this.Id+extension;
                    //Console.WriteLine(fileName_);
                    string fileNameWithPath = Path.Combine(path, fileName_);
                    while (System.IO.File.Exists(fileNameWithPath))
                    {
                        fileName_ = this.Id + "_" + i + extension;
                        fileNameWithPath = Path.Combine(path, fileName_);i++;
                        Console.Write(fileName_);
                    }
                    //Console.WriteLine(filePath);
                    using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    return fileName_;
                }
                else { return "Image length < 0"; }
                
            }
            return "Form file null";
        }
        public AnimalModel(SqlDataReader sqlrdr) {
            this.AnimalFound = true;
            Id = Convert.ToInt32(sqlrdr["Id"]);
            try
            {
                TagNo = Convert.ToString(sqlrdr["TagNo"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Name = Convert.ToString(sqlrdr["Name"]);
            }catch(Exception e) 
            {
                //Console.WriteLine("Name not found");
            }
            try
            {
                DOB= DateTime.Parse(Convert.ToString(sqlrdr["DOB"]));
            }catch(Exception e) 
            {
                //Console.WriteLine("dob not found");
            }
            try{
                Gender = Convert.ToString(sqlrdr["Gender"]);
            }catch (Exception e)
            {
                //Console.WriteLine("Gender not found");
            }
            try { 
                Breed_ = Convert.ToString(sqlrdr["Breed"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Breed not found");
            }
            try { 
                Colour_ = Convert.ToString(sqlrdr["Colour_"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Colour_ not found");
            }
            try
            {
                Colour = Convert.ToInt32(sqlrdr["Colour"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Colour not found");
            }
            try {
                Category = Convert.ToString(sqlrdr["Category"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Category not found");
            }
            try {
                SireID = Convert.ToInt64(sqlrdr["SireID"]); 
            }catch(Exception e)
            {
                //Console.WriteLine("Sire Not Found");
                SireID = null;
            }
            try
            {
                DamID = Convert.ToInt64(sqlrdr["DamID"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Dam Not Found");
                DamID = null;
            }
            try
            {
                Console.WriteLine(sqlrdr["SireTagNo_"].GetType());
                if (sqlrdr["SireTagNo_"].GetType().Equals(null)) {
                    //Console.WriteLine("NULL");
                }
                SireNo = Convert.ToString(sqlrdr["SireTagNo_"]);
                if (SireNo == "")
                {
                    try
                    {
                        SireNo = Convert.ToString(sqlrdr["SireNo"]);
                    }
                    catch (Exception e1)
                    {
                        //Console.WriteLine("Sire NO Not Found");
                    }
                }
                //Console.WriteLine("Set teh sire tag no first");
            }
            catch (Exception e)
            {
                //Console.WriteLine("First Sire No Not Found");
                try
                {
                    SireNo = Convert.ToString(sqlrdr["SireNo"]);
                }catch(Exception e1)
                {
                    //Console.WriteLine("Sire NO Not Found");
                }
                
            }
            
            try
            {
                if(SireID != null)
                {
                    SireName = Convert.ToString(sqlrdr["SireName_"]);
                }
                else
                {
                    SireName = Convert.ToString(sqlrdr["SireName"]);
                }
                
                //Console.WriteLine("Set sire");
            }
            catch (Exception e)
            {
                //Console.WriteLine("First Sire Name Not Found");
               
                
            }
            try
            {
                DamNo = Convert.ToString(sqlrdr["DamTagNo_"]);
            }
            catch (Exception e)
            {
                try {
                    DamNo = Convert.ToString(sqlrdr["DamNo"]);
                }catch(Exception e1)
                {
                    //Console.WriteLine("Dam Tag No Not Found");
                }
            }
            try
            {
                DamName = Convert.ToString(sqlrdr["DamName_"]);
                
            }
            catch (Exception e)
            {
                //Console.WriteLine("Dam name first time failed");
                try
                {
                    DamName = Convert.ToString(sqlrdr["DamName"]);
                }
                catch (Exception e1)
                {
                    //Console.WriteLine("Dam Name Not Found");
                }
                
            }
            try
            {
                DBLY = Convert.ToString(sqlrdr["DBLY"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Dam Name Not Found");
            }
            try
            {
                SDBLY = Convert.ToString(sqlrdr["SDBLY"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Dam Name Not Found");
            }
            try
            {
                ButterFat = Convert.ToDecimal(sqlrdr["ButterFat"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Dam Name Not Found");
            }
            try
            {
                PregnancyStatus = Convert.ToBoolean(sqlrdr["PregnancyStatus"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Pregnancy Status Not Found");
            }
            try
            {
                Status = Convert.ToBoolean(sqlrdr["Status"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Status Not Found");
            }
            try
            {
                ReproductiveStatus = Convert.ToBoolean(sqlrdr["ReproductiveStatus"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine(" Reproductive Status Not Found");
            }
            try
            {
                MilkingStatus = Convert.ToBoolean(sqlrdr["MilkingStatus"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine(" Milking Status Not Found");
            }
            try
            {
                Remarks = Convert.ToString(sqlrdr["Remarks"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Remarks Not Found");
            }
            try
            {
                AdditionalInfo = Convert.ToString(sqlrdr["AdditionalInfo"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("AdditionalInfo Not Found");
            }
            try
            {
                Picture = Convert.ToString(sqlrdr["Picture"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Picture Not Found");
            }
            try
            {
                Lactation = Convert.ToInt32(sqlrdr["Lactation"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Lactation Not Found");
            }
            try
            {
                Type = Convert.ToString(sqlrdr["Type"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Type Not Found");
            }
            try
            {
                SemenDoses = Convert.ToInt32(sqlrdr["SemenDoses"].ToString());
            }
            catch (Exception e)
            {
                //Console.WriteLine("Type Not Found");
            }
            try
            {
                Weight = Convert.ToDecimal(sqlrdr["Weight"]);
            }
            catch (Exception e)
            {
                Weight = null;
                //Console.WriteLine("Weight not found");
            }
            try
            {
                Height = Convert.ToDecimal(sqlrdr["Height"]);
            }
            catch (Exception e)
            {
                Height = null;
                //Console.WriteLine("Weight not found");
            }
            try
            {
                NoOfTeatsWorking = Convert.ToInt32(sqlrdr["NoOfTeatsWorking"]);
            }
            catch (Exception e)
            {
                NoOfTeatsWorking = 0;
            }
            try
            {
                Location = Convert.ToInt32(sqlrdr["Location"]);
            }
            catch (Exception e)
            {
                Location = 0;
            }
            try
            {
                Remarks = Convert.ToString(sqlrdr["Remarks"]);
            }
            catch (Exception e)
            {
                Remarks = "";
                //Console.WriteLine("Weight not found");
            }
            if (sqlrdr["Sold"] != null)
            {
                Console.WriteLine("Sold");
                Console.WriteLine(sqlrdr["Sold"]);
                Sold = Convert.ToBoolean(sqlrdr["Sold"]);
            }
            /*if (sqlrdr["SireID"].GetType() != DBNull) {
                
            }
            if (sqlrdr["DamID"].GetType() != DBNull)
            {
                DamID = Convert.ToInt32(sqlrdr["DamID"]);
            }*/
            /*m1.MilkingPK = Convert.ToInt64(sqlrdr["MilkingPK"]);
                m1.CowNo = sqlrdr["CowNo"].ToString();
                m1.Date = Convert.ToDateTime(sqlrdr["Date"]);
                m1.MorningValue = Convert.ToDecimal(sqlrdr["MorningValue"]);
                m1.EveningValue = Convert.ToDecimal(sqlrdr["EveningValue"]);
                m1.CalfValue = Convert.ToDecimal(sqlrdr["CalfValue"]);
                m1.Total = Convert.ToDecimal(sqlrdr["Total"]);
                m1.Breed = Convert.ToString(sqlrdr["Breed"]);
                m1.Remarks = Convert.ToString(sqlrdr["Remarks"]);
                m1.Lactation = Convert.ToString(sqlrdr["Lactation"]);*/
        }
        public static Dictionary<string, object> GetFormatedCowData(CowModel? cow_)
        {
            if(cow_!=null){
                Dictionary<string, object> cow = new Dictionary<string, object>();
                cow["id"] = cow_.Id;
                cow["tagNo"] = cow_.TagNo;
                cow["name"] = cow_.Name;
                if (cow_.DOB != null)
                {
                    cow["dob"] = ((DateTime)cow_.DOB).ToString("yyyy-MM-dd HH:mm:ss");// "2004-12-01T00:00:00",
                }
                else
                {
                    cow["dob"] = cow_.DOB.ToString();
                }
                cow["category"] = cow_.Category;
                cow["gender"] = cow_.Gender;
                cow["breed"] = cow_.Breed_;
                cow["colour"] = cow_.Colour;
                cow["colour_"] = cow_.Colour_;
                cow["sireID"] = cow_.SireID;
                cow["damID"] = cow_.DamID;
                cow["sireNo"] = cow_.SireNo;
                cow["sireName"] = cow_.SireName;
                cow["damNo"] = cow_.DamNo;
                cow["damName"] = cow_.DamName;
                cow["dbly"] = cow_.DBLY;
                cow["sdbly"] = cow_.SDBLY;
                cow["butterFat"] = cow_.ButterFat;
                cow["pregnancyStatus"] = cow_.PregnancyStatus;
                cow["status"] = cow_.Status;
                cow["reproductiveStatus"] = cow_.ReproductiveStatus;
                cow["milkingStatus"] = cow_.MilkingStatus;
                cow["remarks"] = cow_.Remarks;
                cow["additionalInfo"] = cow_.AdditionalInfo;
                if (cow_.Picture != null && cow_.Picture.Trim() != "")
                {
                    cow["picture"] = cow_.Picture;
                }
                else
                {
                    cow["picture"] = cow_.DefaultPicture;
                }

                cow["lactation"] = cow_.Lactation;
                cow["type"] = cow_.Type;
                cow["weight"] = cow_.Weight;
                cow["height"] = cow_.Height;
                cow["semenDoses"] = cow_.SemenDoses;
                cow["noOfTeatsWorking"] = cow_.NoOfTeatsWorking;
                cow["location"] = cow_.Location;
                cow["sold"] = cow_.Sold;
                Dictionary<string,object> data = new Dictionary<string,object>();
                data["success"] = true;
                data["data"] = cow;
                data["message"] = "Cow Found";
                return data;
            }else{
                Dictionary<string,object> data = new Dictionary<string,object>();
                data["success"] = false;
                data["message"] = "Cow Not Found";
                
                return data;
            }
            
        }
        public static Dictionary<string, object> GetFormatedBullData(BullModel? bull_)
        {
            if(bull_!=null){
                Dictionary<string, object> cow = new Dictionary<string, object>();
                cow["id"] = bull_.Id;
                cow["tagNo"] = bull_.TagNo;
                cow["name"] = bull_.Name;
                if (bull_.DOB != null)
                {
                    cow["dob"] = ((DateTime)bull_.DOB).ToString("yyyy-MM-dd");// "2004-12-01T00:00:00",
                }
                else
                {
                    cow["dob"] = bull_.DOB.ToString();
                }
                cow["category"] = bull_.Category;
                cow["gender"] = bull_.Gender;
                cow["breed"] = bull_.Breed_;
                cow["colour"] = bull_.Colour;
                cow["colour_"] = bull_.Colour_;
                cow["sireID"] = bull_.SireID;
                cow["damID"] = bull_.DamID;
                cow["sireNo"] = bull_.SireNo;
                cow["sireName"] = bull_.SireName;
                cow["damNo"] = bull_.DamNo;
                cow["damName"] = bull_.DamName;
                cow["dbly"] = bull_.DBLY;
                cow["sdbly"] = bull_.SDBLY;
                cow["status"] = bull_.Status;
                cow["reproductiveStatus"] = bull_.ReproductiveStatus;
                cow["remarks"] = bull_.Remarks;
                cow["additionalInfo"] = bull_.AdditionalInfo;
                if (bull_.Picture != null && bull_.Picture.Trim() != "")
                {
                    cow["picture"] = bull_.Picture;
                }
                else
                {
                    cow["picture"] = bull_.DefaultPicture;
                }

                cow["type"] = bull_.Type;
                cow["weight"] = bull_.Weight;
                cow["height"] = bull_.Height;
                cow["semenDoses"] = bull_.SemenDoses;
                Console.WriteLine("Semene Doses : " + bull_.SemenDoses);
                cow["location"] = bull_.Location;
                cow["sold"] = bull_.Sold;
                Dictionary<string,object> data = new Dictionary<string,object>();
                data["status"] = true;
                data["data"] = cow;
                data["message"] = "Bull Found";
                return data;
            }else{
                Dictionary<string,object> data = new Dictionary<string,object>();
                data["status"] = false;
                data["message"] = "Bull Not Found";
                return data;
            }
        }
        public void GenerateImageName(long id)
        {
            long max_id = ++id;
            //Console.WriteLine($"New Id {max_id}");
            if (this.formFile != null)
            {
                //Console.WriteLine("yew file provided");
                if (true)
                {
                    string[] supportedTypes = { ".jpg", ".jpeg", ".png", ".PNG" };
                    string extension = System.IO.Path.GetExtension(formFile.FileName);
                    //Console.Write("OK001");
                    if (formFile.Length > 0)
                    {
                        Console.Write("OK002");
                        string path = "images";// Path.Combine(@"F:\PROJECTS\djjs-gaushala\soruce-code\html\gaushalaTemplate\", "images");
                        //string path = Path.Combine(@"F:\PROJECTS\djjs-gaushala\soruce-code\html\gaushalaTemplate\", "images");
                        //string path = Path.Combine(@"D:\dalwinder\projects\djjs-gaushala\soruce-code\gaushalaTemplate\", "images");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        //var filePath = Path.GetTempFileName();
                        int i = 0;
                        string fileName_ = max_id + extension;
                        //Console.WriteLine(fileName_);
                        string fileNameWithPath = Path.Combine(path, fileName_);
                        //Console.WriteLine(fileNameWithPath);
                        //Console.WriteLine("OK000004");
                        while (System.IO.File.Exists(fileNameWithPath))
                        {
                            fileName_ = max_id + "_" + i + extension;
                            fileNameWithPath = Path.Combine(path, fileName_); i++;
                            //Console.Write(fileName_);
                        }
                        //Console.WriteLine(filePath);
                        this.Picture = fileName_;
                        //Console.Write("OK003");
                        //Console.Write(fileName_);
                        //Console.Write("picture"+this.Picture);
                    }
                    else
                    {
                        this.Picture = null;
                    }
                }
            }
            else
            {
                this.Picture = null;
            }
            Console.WriteLine(this.Picture);
        }
        public bool SaveImage2()
        {
            Console.WriteLine("Savign image");
            if (formFile != null)
            {
                //Console.WriteLine("Savign image 1");
                if (formFile.Length > 0)
                {
                    try{
                        //Console.WriteLine("Savign image2");
                        string path =  "images";
                        //string path = Path.Combine(@"F:\PROJECTS\djjs-gaushala\soruce-code\html\gaushalaTemplate\", "images");
                        //string path = Path.Combine(@"D:\dalwinder\projects\djjs-gaushala\soruce-code\gaushalaTemplate\", "images");
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        //var filePath = Path.GetTempFileName();
                        string fileNameWithPath = Path.Combine(path, this.Picture);
                        //Console.WriteLine(fileNameWithPath);
                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            //Console.WriteLine("HELLO");
                            formFile.CopyTo(stream);
                            //Console.WriteLine("Image saved");
                        }
                    }catch(Exception e){
                        //Console.WriteLine(e.Message);
                        //Console.WriteLine(e.StackTrace);
                        return false;
                    }
                    return true;
                }else{
                    return false;
                }
            }else{
                return false;
            }
        }
    }
}
