using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using GaushalaAPI.DBContext;

namespace GaushalaAPI.Models
{
    public class MedicationModel
    {
        public Dictionary<string, string>? errors { get; set; }
        public long? Id { get; set; }
        public DateTime? Date { get;set;}
        public long? AnimalID { get; set; } 
        public string? AnimalNo { get; set; } 
        public string? Disease { get; set; } 
        public string? Symptoms { get; set; } 
        public string? Diagnosis { get; set; }
        public string? Treatment { get; set; }
        public int? Prognosis { get; set; }
        public string? Result { get; set; } 
        public string? CostOfTreatment { get; set; } 
        public string? Remarks { get; set; } 
        public decimal? CostOfTreatment2{ get; set; } 
        public string? DoctorDetail { get; set; } 
        public List<long>? DoctorIDs { get; set; } 
        public Dictionary<long,Dictionary<string,object>>? Doctors { get; set; }
        
        public MedicationModel()
        {

        }
        
        public bool ValidateMedication(string type = "Add")
        {
            bool error = true;
            errors = new Dictionary<string, string>();
            if (type == "Edit") {
                if (this.Id == null)
                {
                    errors.Add("Id", "Please Select the Record");
                    error = false;
                }
            }
            if (this.Date == null)
            {
                errors.Add("Date", "Enter the Date of Medication");
                error = false;
            }
            if (this.AnimalID == null)
            {
                errors.Add("AnimalID", "Please select the Animal");
                error = false;
            }
            if (this.Disease == null)
            {
                errors.Add("Disease", "Please enter the Disease");
                error = false;
            }
            if (this.Symptoms == null)
            {
                errors.Add("Symptoms", "Please enter the Symptoms");
                error = false;
            }
            if (this.Diagnosis == null)
            {
                errors.Add("Diagnosis", "Please enter the Symptoms");
                error = false;
            }
            if (this.Treatment == null)
            {
                errors.Add("Treatment", "Please enter the Treatment");
                error = false;
            }if (this.Prognosis == null)
            {
                errors.Add("Prognosis", "Please select the prognosis.");
                error = false;
            }
            if (this.Result == null)
            {
                errors.Add("Result", "Please Select the Result.");
                error = false;
            }
            if (this.DoctorIDs == null || DoctorIDs.Count==0)
            {
                errors.Add("DoctorIDs", "Please Select the Doctor(s)");
                error = false;
            }
            if (this.CostOfTreatment2 == null)
            {
                errors.Add("CostOfTreatment2", "Please Enter the Cost of Treatment");
                error = false;
            }
            if (this.Remarks == null)
            {
                errors.Add("Remarks", "Please Enter the Remarks");
                error = false;
            }
            return error;
        }
        public MedicationModel(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            try
            {
                AnimalID = Convert.ToInt64(sqlrdr["AnimalID"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                Date= DateTime.Parse(Convert.ToString(sqlrdr["Date"]));
            }catch(Exception e) 
            {
                //Console.WriteLine("dob not found");
            }
            try
            {
                AnimalNo = Convert.ToString(sqlrdr["AnimalNo"]);
            }catch(Exception e) 
            {
                //Console.WriteLine("Name not found");
            }
            try{
                Disease = Convert.ToString(sqlrdr["Disease"]);
            }catch (Exception e)
            {
                //Console.WriteLine("Gender not found");
            }
            try { 
                Symptoms = Convert.ToString(sqlrdr["Symptoms"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Breed not found");
            }
            try { 
                Diagnosis = Convert.ToString(sqlrdr["Diagnosis"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Diagnosis not found");
            }
            try
            {
                Treatment = Convert.ToString(sqlrdr["Treatment"]);
            }
            catch (Exception e)
            {
                Console.WriteLine("Treatment not found");
            }
            try
            {
                Prognosis = Convert.ToInt32(sqlrdr["Prognosis"]);
            }catch(Exception e)
            {
                Prognosis = null;
            }
            try {
                Result = Convert.ToString(sqlrdr["Result"]);
            }
            catch (Exception e){            }
            try {
                CostOfTreatment = Convert.ToString(sqlrdr["CostOfTreatment"]); 
            }catch(Exception e){            }
            try
            {
                Remarks = Convert.ToString(sqlrdr["Remarks"]);
            }
            catch (Exception e)
            {
                
            }
            try
            {
                CostOfTreatment2 = Convert.ToDecimal(sqlrdr["CostOfTreatment2"]);
            }
            catch (Exception e)
            {
                CostOfTreatment2 = (decimal)0.0;
            }
            try
            {
                DoctorDetail = Convert.ToString(sqlrdr["DoctorDetail"]);
            }
            catch (Exception e){}
            Doctors = new Dictionary<long,Dictionary<string, object>>();
        }
        static public Dictionary<string,object> GetFormatedMedication(MedicationModel medication)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["id"] = medication.Id;
            data["animalID"] = medication.AnimalID;
            data["animalNo"] = medication.AnimalNo.ToString();
            data["costOfTreatment"] = medication.CostOfTreatment;
            data["costOfTreatment2"] = medication.CostOfTreatment2;
            data["date"] = Helper.FormatDate3(medication.Date);
            data["diagnosis"] = medication.Diagnosis.ToString();
            data["disease"] = medication.Disease.ToString();
            data["doctorDetail"] = medication.DoctorDetail.ToString();
            data["doctorIDs"] = medication.DoctorIDs;
            data["prognosis"] = medication.Prognosis;
            data["remarks"] = medication.Remarks.ToString();
            data["result"] = medication.Result.ToString();
            data["symptoms"] = medication.Symptoms.ToString();
            data["treatment"] = medication.Treatment.ToString();
            data["doctors"] = Helper.IsNullOrEmpty(medication.Doctors);
            data["errors"] = medication.errors;
            return data;
        }
    }
}
