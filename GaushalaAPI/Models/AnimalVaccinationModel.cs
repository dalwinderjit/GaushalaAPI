using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using GaushalaAPI.DBContext;
using GaushalaAPI.Helpers;

namespace GaushalaAPI.Models
{
    public class AnimalVaccinationModel : AnimalMedicationModel
    {
        public AnimalVaccinationModel()
        {

        }
        /*public bool ValidateVaccination(string type = "Add",string type2="Vaccination") {
            bool error = true;
            error = this.ValidateMedication(type,type2);
            if (this.VaccinationID == null)
            {
                errors.Add("VaccinationID", "Please Select the Vaccination");
                error = false;
            }
            if (this.AnimalIDs == null)
            {
                errors.Add("AnimalIDs", "Please select the Animals");
                error = false;
            }
            return error;
        }
        public bool ValidateMedication(string type = "Add",string type2="Medication")
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
                errors.Add("Date", "Enter the Date of Vaccination");
                error = false;
            }
            if (type2 == "Medication")
            {
                if (this.AnimalID == null)
                {
                    errors.Add("AnimalID", "Please select the Animal");
                    error = false;
                }
            }
      
            if (this.DiseaseID == null)
            {
                errors.Add("DiseaseID", "Please select the Disease");
                error = false;
                if (this.DiseaseID == 1)    //other id
                {
                    errors.Add("Disease", "Please enter the Other Disease");
                    error = false;
                }
            }
            if (this.Symptoms == null)
            {
                errors.Add("Symptoms", "Please enter the Symptoms");
                error = false;
            }
            if (this.Diagnosis == null)
            {
                errors.Add("Diagnosis", "Please enter the Diagnosis");
                error = false;
            }
            if (this.Treatment == null)
            {
                errors.Add("Treatment", "Please enter the Treatment");
                error = false;
            }if (this.Prognosis == null)
            {
                errors.Add("Prognosis", "Please select the Prognosis.");
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
        }*/
        public AnimalVaccinationModel(SqlDataReader sqlrdr) {
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

            if (!Validations.IsNullOrEmpty(sqlrdr["VaccinationID"])) {
                VaccinationID = Convert.ToInt32(sqlrdr["VaccinationID"]);
            }
            Doctors = new Dictionary<long,Dictionary<string, object>>();
            Animals = new Dictionary<long,Dictionary<string, object>>();
        }
        static public Dictionary<string,object> GetFormatedVaccination(AnimalVaccinationModel vaccination)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["id"] = vaccination.Id;
            data["animalID"] = vaccination.AnimalID;
            data["animalNo"] = vaccination.AnimalNo.ToString();
            data["costOfTreatment"] = vaccination.CostOfTreatment;
            data["costOfTreatment2"] = vaccination.CostOfTreatment2;
            data["date"] = Helper.FormatDate3(vaccination.Date);
            data["diagnosis"] = vaccination.Diagnosis.ToString();
            data["disease"] = vaccination.Disease.ToString();
            data["doctorDetail"] = vaccination.DoctorDetail.ToString();
            data["doctorIDs"] = vaccination.DoctorIDs;
            data["animalIDs"] = vaccination.AnimalIDs;
            data["prognosis"] = vaccination.Prognosis;
            data["remarks"] = vaccination.Remarks.ToString();
            data["result"] = vaccination.Result.ToString();
            data["symptoms"] = vaccination.Symptoms.ToString();
            data["treatment"] = vaccination.Treatment.ToString();
            data["doctors"] = Helper.IsNullOrEmpty(vaccination.Doctors);
            data["animals"] = Helper.IsNullOrEmpty(vaccination.Animals);
            data["vaccinationID"] = Helper.IsNullOrEmpty(vaccination.VaccinationID);
            data["errors"] = vaccination.errors;
            return data;
        }
    }
}
