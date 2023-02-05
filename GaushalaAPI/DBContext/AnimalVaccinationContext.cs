using GaushalaAPI.Helpers;
using GaushalaAPI.Models;
using GaushalAPI.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class AnimalVaccinationContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        public AnimalVaccinationContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddVaccinationDetail(AnimalVaccinationModel vaccination)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (vaccination.ValidateVaccination("Add") == true)
            {
                if (vaccination.AnimalIDs == null)
                {
                    vaccination.AnimalIDs = new List<long>();
                    vaccination.AnimalIDs.Add((long)vaccination.AnimalID);
                }else if (vaccination.AnimalIDs.Count == 0)
                {
                    vaccination.AnimalIDs.Add((long)vaccination.AnimalID);
                }
                string query = "";// "Insert into Vaccination (Date,AnimalID,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.ID Values"+
                //"(@Date,@AnimalID,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertAnimalVaccinationSqlQuery(vaccination);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Date), vaccination.Date, "Date", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.AnimalID), vaccination.AnimalID, "AnimalID", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Disease), vaccination.Disease, "Disease", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Symptoms), vaccination.Symptoms, "Symptoms", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Diagnosis), vaccination.Diagnosis, "Diagnosis", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Treatment), vaccination.Treatment, "Treatment", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Prognosis), vaccination.Prognosis, "Prognosis", System.Data.SqlDbType.Int);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Result), vaccination.Result, "Result", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.CostOfTreatment2), vaccination.CostOfTreatment2, "CostofTreatment2", System.Data.SqlDbType.Decimal);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Remarks), vaccination.Remarks, "Remarks", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.DiseaseID), vaccination.DiseaseID, "DiseaseID", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.VaccinationID), vaccination.VaccinationID, "VaccinationID", System.Data.SqlDbType.BigInt);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("Vaccination");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        vaccination.Id = (Int64)sqlcmd.ExecuteScalar();
                        Dictionary<string,object> mediDocData= this.AddVaccinationDoctors(vaccination.Id,vaccination.DoctorIDs,conn,tran);
                        Dictionary<string, object> mediAniData = this.AddVaccinationAnimals(vaccination.Id, vaccination.AnimalIDs, conn, tran);
                        Dictionary<string, string> mediDocData_ = (Dictionary<string, string>)mediDocData["data"];
                        Dictionary<string, string> mediAniData_ = (Dictionary<string, string>)mediAniData["data"];
                        if(mediDocData_["status"] == "success" && mediAniData_["status"].ToString() == "success")
                        {
                            if(vaccination.Id >0){
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Animal Vaccination Data Saved Successfully! "+mediDocData_["message"] +"! " + mediAniData_["message"];
                                data2["status"] = "success";
                                data["data"] = data2;
                            }else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Animal Vaccination Date Saving Failed";
                                data2["status"] = "failure";
                                data["data"] = data2;
                            }
                        }
                        else
                        {
                            tran.Rollback();
                            Console.WriteLine("rolling back second");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animal Vaccination Date Saving Failed"+mediDocData["message"]+mediAniData["message"];
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }catch(Exception e)
                    {
                        Console.WriteLine("Exception : "+e.Message);
                        Console.WriteLine("Exception : "+e.StackTrace);
                    }
                    
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Animal Vaccination Date Saving Failed.Invalid Data Entered";
                data2["status"] = "failure";
                data["errors"] = vaccination.errors;
                data["data"] = data2;
            }
            return data;
            
        }
        public string GenerateInsertAnimalVaccinationSqlQuery(AnimalVaccinationModel medi)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.Date), ref cols, ref params_, "Date");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.AnimalID), ref cols, ref params_, "AnimalID");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.Disease), ref cols, ref params_, "Disease");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.DiseaseID), ref cols, ref params_, "DiseaseID");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.Symptoms), ref cols, ref params_, "Symptoms");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.Diagnosis), ref cols, ref params_, "Diagnosis");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.Prognosis), ref cols, ref params_, "Prognosis");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.Treatment), ref cols, ref params_, "Treatment");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.Result), ref cols, ref params_, "Result");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.Remarks), ref cols, ref params_, "Remarks");
            this.addColToQuery(!Validations.IsNullOrEmpty(medi.VaccinationID), ref cols, ref params_, "VaccinationID");
            addQuery = $"INSERT into [dbo].[Medication] ({cols}) OUTPUT INSERTED.ID values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditVaccinationDetail(AnimalVaccinationModel medication)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (medication.ValidateVaccination("Edit") == true)
            {
                /*string query = "UPdate Vaccination set Date = @Date,AnimalID = @AnimalID,Disease = @Disease,Symptoms = @Symptoms,Diagnosis = @Diagnosis," +
                    "Prognosis = @Prognosis,Treatment = @Treatment,Result = @Result,CostofTreatment2 = @CostofTreatment2,Remarks = @Remarks  where Id = @Id";*/
                string query = this.GenerateUpdateAnimalSqlQuery(medication);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Id), medication.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Date), medication.Date, "Date", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.AnimalID), medication.AnimalID, "AnimalID", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Disease), medication.Disease, "Disease", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Symptoms), medication.Symptoms, "Symptoms", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Diagnosis), medication.Diagnosis, "Diagnosis", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Treatment), medication.Treatment, "Treatment", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Prognosis), medication.Prognosis, "Prognosis", System.Data.SqlDbType.Int);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Result), medication.Result, "Result", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.CostOfTreatment2), medication.CostOfTreatment2, "CostofTreatment2", System.Data.SqlDbType.Decimal);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.Remarks), medication.Remarks, "Remarks", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.DiseaseID), medication.DiseaseID, "DiseaseID", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(medication.VaccinationID), medication.VaccinationID, "VaccinationID", System.Data.SqlDbType.BigInt);

                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("Vaccination");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        int i = sqlcmd.ExecuteNonQuery();
                        Dictionary<string, object> mediDocData = this.UpdateVaccinationDoctors(medication.Id, medication.DoctorIDs,conn,tran);
                        Dictionary<string, object> mediAniData = this.UpdateVaccinationAnimalIDs(medication.Id, medication.AnimalIDs,conn,tran);
                        Dictionary<string, string> mediDocData_ = (Dictionary<string, string>)mediDocData["data"];
                        Dictionary<string, string> mediAniData_ = (Dictionary<string, string>)mediAniData["data"];
                        if (mediDocData_["status"] == "success" && mediAniData_["status"] == "success")
                        {
                            if (i > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Animal Vaccination Data Saved Successfully." + mediDocData_["message"] + mediAniData_["message"];
                                data2["status"] = "success";
                                data["data"] = data2;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Animal Vaccination Date Updating Failed";
                                data2["status"] = "failure";
                                data["data"] = data2;
                            }
                        }
                        else
                        {
                            tran.Rollback();
                            Console.WriteLine("rolling back second");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animal Vaccination Dat Saving Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message);
                        Console.WriteLine("Exception : " + e.StackTrace);
                    }
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Animal Vaccination Date Saving Failed.Invalid Data Entered";
                data2["status"] = "failure";
                data["errors"] = medication.errors;
                data["data"] = data2;
            }
            return data;
        }

        private Dictionary<string, object> UpdateVaccinationAnimalIDs(long? medication_id, List<long> newAnimalIds, SqlConnection conn2, SqlTransaction tran)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //get the old doctor ids
            List<long> medication_ids = new List<long>();
            medication_ids.Add((long)medication_id);
            Dictionary<long, object> medication_data = AnimalVaccinationContext.GetAnimalIdsByVaccinationIDs2(this._configuration, medication_ids);
            Dictionary<string, object> ani_data;
            try
            {
                ani_data = (Dictionary<string, object>)medication_data[(long)medication_id];
            }
            catch (Exception e)
            {
                ani_data = new Dictionary<string, object>();
                Console.WriteLine("No Data Found");
                return data;
            }

            Dictionary<long, long> OldVaccinationIds = (Dictionary<long, long>)ani_data["Relations"];
            Dictionary<long, int> OldAnimalIds = (Dictionary<long, int>)ani_data["AniIDS"];


            List<long> AlreadyAddedAnimalIds = new List<long>();
            List<long> AnimalIdsToBeAdded = new List<long>();
            List<long> AnimalIdsToBeDeleted = new List<long>();
            List<long> AnimalIdsToBeRecovered = new List<long>();
            foreach (long doc_id in newAnimalIds)
            {
                if (OldAnimalIds.ContainsKey(doc_id) == true)
                {
                    int deleted = 0;
                    long med_id = 0;
                    OldAnimalIds.TryGetValue(doc_id, out deleted);
                    OldVaccinationIds.TryGetValue(doc_id, out med_id);
                    if (deleted == 0)
                    {
                        AlreadyAddedAnimalIds.Add(doc_id);
                    }
                    else if (deleted == 1)
                    {
                        if (med_id != 0)
                        {
                            AnimalIdsToBeRecovered.Add(med_id);
                        }
                    }
                }
                else
                {
                    AnimalIdsToBeAdded.Add(doc_id);
                }
            }
            foreach (var doc_id in OldAnimalIds)
            {
                if (newAnimalIds.Contains(doc_id.Key) == false)
                {
                    if (doc_id.Value == 0)
                    {
                        long med_id = 0;
                        OldVaccinationIds.TryGetValue(doc_id.Key, out med_id);
                        AnimalIdsToBeDeleted.Add(med_id);
                    }
                }
            }
            bool addData_ = true;
            bool deleteData_ = true;
            bool recoverData_ = true;
            string id_message = "";
            if (AnimalIdsToBeAdded.Count > 0)
            {
                Dictionary<string, object> addData = this.AddVaccinationAnimals(medication_id, AnimalIdsToBeAdded, conn2, tran);
                if (((Dictionary<string, string>)addData["data"])["status"] == "success")
                {
                    addData_ = true;
                }
                else
                {
                    addData_ = false;
                    if (id_message != "")
                    {
                        id_message = ",";
                    }
                    id_message += ((Dictionary<string, string>)addData["data"])["message"];
                }
            }
            else
            {
                addData_ = true;
            }
            if (AnimalIdsToBeDeleted.Count > 0)
            {
                Dictionary<string, object> deleteData = this.DeleteVaccinationAnimals(medication_id, AnimalIdsToBeDeleted, conn2, tran);
                if (((Dictionary<string, string>)deleteData["data"])["status"] == "success")
                {
                    deleteData_ = true;
                }
                else
                {
                    deleteData_ = false;
                    if (id_message != "")
                    {
                        id_message = ",";
                    }
                    id_message += ((Dictionary<string, string>)deleteData["data"])["message"];
                }
            }
            else
            {
                deleteData_ = true;
            }
            //to be recovered
            if (AnimalIdsToBeRecovered.Count > 0)
            {
                Dictionary<string, object> recoverData = this.RecoverVaccinationAnimals(medication_id, AnimalIdsToBeRecovered, conn2, tran);
                if (((Dictionary<string, string>)recoverData["data"])["status"] == "success")
                {
                    recoverData_ = true;
                }
                else
                {
                    recoverData_ = false;
                    if (id_message != "")
                    {
                        id_message = ",";
                    }
                    id_message += ((Dictionary<string, string>)recoverData["data"])["message"];
                }
            }
            if (addData_ == true && deleteData_ == true && recoverData_ == true)
            {
                Dictionary<string, string> data__ = new Dictionary<string, string>();
                data__["message"] = "Animal IDs Updated Successfully";
                data__["status"] = "success";
                data["data"] = data__;
            }
            else
            {
                Dictionary<string, string> data__ = new Dictionary<string, string>();
                data__["message"] = "Animal IDs Updation Failed.(" + id_message + ")";
                data__["status"] = "failure";
                data["data"] = data__;
            }
            return data;
        }

        public string GenerateUpdateAnimalSqlQuery(AnimalVaccinationModel medi)
        {
            string UpdateQuery = "";
            string cols = "";// "[Gender],[TagNo],[Name],[Breed],[Lactation],[DOB],[Colour],[Weight],[Height],[BirthLactationNumber],[PregnancyStatus],[Status],[ReproductiveStatus],[MilkingStatus],[Location]";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";

            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.Date), ref cols, "Date");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.AnimalID), ref cols, "AnimalID");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.Disease), ref cols, "Disease");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.Symptoms), ref cols, "Symptoms");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.Diagnosis), ref cols, "Diagnosis");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.Prognosis), ref cols, "Prognosis");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.Treatment), ref cols, "Treatment");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.Result), ref cols, "Result");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.CostOfTreatment2), ref cols, "CostofTreatment2");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.Remarks), ref cols, "Remarks");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.VaccinationID), ref cols, "VaccinationID");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(medi.DiseaseID), ref cols, "DiseaseID");
            
            UpdateQuery = $"UPDATE [dbo].[Medication] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
                    
        }
        private Dictionary<string, object> UpdateVaccinationDoctors(long? medication_id, List<long> newDoctorIDs, SqlConnection conn2, SqlTransaction tran)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //get the old doctor ids
            List<long> medication_ids = new List<long>();
            medication_ids.Add((long)medication_id);
            Dictionary<long, object> medication_data = AnimalVaccinationContext.GetDoctorIdsByVaccinationIDs2(this._configuration, medication_ids);
            Dictionary<string, object> doc_data;
            try
            {
                doc_data = (Dictionary<string, object>)medication_data[(long)medication_id];
            }catch(Exception e)
            {
                doc_data = new Dictionary<string, object>();
                Console.WriteLine("No Data Found");
                return data;
            }

            Dictionary<long, long> OldVaccinationIds = (Dictionary<long, long>)doc_data["Relations"];
            Dictionary<long, int> OldDoctorIds = (Dictionary<long, int>)doc_data["DocIDS"];
            

            List<long> AlreadyAddedDoctorIds = new List<long>();
            List<long> DoctorIdsToBeAdded = new List<long>();
            List<long> DoctorIdsToBeDeleted = new List<long>();
            List<long> DoctorIdsToBeRecovered = new List<long>();
            foreach (long doc_id in newDoctorIDs)
            {
                if (OldDoctorIds.ContainsKey(doc_id) == true) {
                    int deleted = 0;
                    long med_id = 0;
                    OldDoctorIds.TryGetValue(doc_id, out deleted);
                    OldVaccinationIds.TryGetValue(doc_id, out med_id);
                    if (deleted == 0)
                    {
                        AlreadyAddedDoctorIds.Add(doc_id);
                    } else if (deleted == 1)
                    {
                        if (med_id != 0)
                        {
                            DoctorIdsToBeRecovered.Add(med_id);
                        }
                    }
                }
                else
                {
                    DoctorIdsToBeAdded.Add(doc_id);
                }
            }
            foreach (var doc_id in OldDoctorIds)
            {
                if (newDoctorIDs.Contains(doc_id.Key) == false)
                {
                    if (doc_id.Value == 0)
                    {
                        long med_id = 0;
                        OldVaccinationIds.TryGetValue(doc_id.Key, out med_id);
                        DoctorIdsToBeDeleted.Add(med_id);
                    }
                }
            }
            bool addData_ = true;
            bool deleteData_ = true;
            bool recoverData_ = true;
            string id_message = "";
            if (DoctorIdsToBeAdded.Count > 0) {
                Dictionary<string, object> addData = this.AddVaccinationDoctors(medication_id, DoctorIdsToBeAdded, conn2, tran);
                if (((Dictionary<string, string>)addData["data"])["status"] == "success") {
                    addData_ = true;
                }
                else
                {
                    addData_ = false;
                    if (id_message != "")
                    {
                        id_message = ",";
                    }
                    id_message += ((Dictionary<string, string>)addData["data"])["message"];
                }
            }
            else
            {
                addData_ = true;
            }
            if (DoctorIdsToBeDeleted.Count > 0) {
                Dictionary<string, object>  deleteData = this.DeleteVaccinationDoctors(medication_id, DoctorIdsToBeDeleted, conn2, tran);
                if (((Dictionary<string, string>)deleteData["data"])["status"] == "success") {
                    deleteData_ = true;
                }
                else
                {
                    deleteData_ = false;
                    if (id_message != "")
                    {
                        id_message = ",";
                    }
                    id_message += ((Dictionary<string, string>)deleteData["data"])["message"];
                }
            }
            else
            {
                deleteData_ = true;
            }
            //to be recovered
            if (DoctorIdsToBeRecovered.Count > 0) {
                Dictionary<string, object>  recoverData = this.RecoverVaccinationDoctors(medication_id, DoctorIdsToBeRecovered, conn2, tran);
                if (((Dictionary<string, string>)recoverData["data"])["status"] == "success")
                {
                    recoverData_ = true;
                }
                else
                {
                    recoverData_ = false;
                    if (id_message != "")
                    {
                        id_message = ",";
                    }
                    id_message += ((Dictionary<string, string>)recoverData["data"])["message"];
                }
            }
            if (addData_ == true && deleteData_ == true && recoverData_ == true) {
                Dictionary<string, string> data__ = new Dictionary<string, string>();
                data__["message"] = "Doctor IDs Updated Successfully";
                data__["status"] = "success";
                data["data"] = data__;
            }
            else
            {
                Dictionary<string, string> data__ = new Dictionary<string, string>();
                data__["message"] = "Doctor IDs Updation Failed.("+id_message+")";
                data__["status"] = "failure";
                data["data"] = data__;
            }
            return data;
        }

        internal AnimalVaccinationModel? GetVaccinationDetailById(long id)
        {
            AnimalVaccinationModel? medication = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from Medication where Id = @Id";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = id;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        medication = new AnimalVaccinationModel(sqlrdr);
                        medication.DoctorIDs = new List<long>();
                        medication.AnimalIDs = new List<long>();
                        
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("COnnection closed");
                    if (medication != null)
                    {
                        List<long> med_ids = new List<long>();
                        med_ids.Add((long)medication.Id);
                        if (med_ids.Count > 0)
                        {
                            Dictionary<long, object> medication_data = AnimalVaccinationContext.GetDoctorIdsByVaccinationIDs2(this._configuration, med_ids);
                            Dictionary<string, object> doc_data;
                            try
                            {
                                doc_data = (Dictionary<string, object>)medication_data[id];
                            }
                            catch(Exception e)
                            {
                                doc_data = new Dictionary<string, object>();
                            }
                            //Dictionary<long, long> Relations = (Dictionary<long, long>)doc_data["Relations"];
                            Dictionary<long,int> DoctorIds = (Dictionary<long, int>)doc_data["DocIDS"];
                            Console.WriteLine("doc count " + DoctorIds.Count);
                            List<long> Doc_IDS = new List<long>();
                            foreach (var doc_id in DoctorIds)
                            {
                                if (doc_id.Value == 0)
                                {
                                    //Doc_IDS.Add(doc_id.Key);
                                    medication.DoctorIDs.Add(doc_id.Key);
                                }
                            }
                            if (DoctorIds.Count > 0)
                            {

                                Dictionary<long, Dictionary<string, object>> doc_names = UsersContext.GetDoctorsDataByIds(this._configuration, medication.DoctorIDs);
                                
                                foreach (var doc_id in DoctorIds)
                                {
                                    if (doc_id.Value == 0)
                                    {
                                        try
                                        {
                                            Dictionary<string, object> doc_name_ = doc_names[doc_id.Key];
                                            if (medication.DoctorDetail != "")
                                            {
                                                medication.DoctorDetail += ",";
                                            }
                                            medication.DoctorDetail += doc_name_["label"].ToString() + " " + doc_name_["name"];
                                            doc_name_["name"] = doc_name_["label"].ToString() + " " + doc_name_["name"];
                                            try
                                            {
                                                medication.Doctors[Convert.ToInt64(doc_name_["id"])] = doc_name_;
                                            }catch(Exception e2)
                                            {
                                                Console.WriteLine(e2.StackTrace);
                                                Console.WriteLine(e2.Message);
                                            }
                                        }
                                        catch (Exception e1)
                                        {
                                            Console.WriteLine("Doc not found doc id" + doc_id.Key);
                                            Console.WriteLine(e1.StackTrace);
                                            Console.WriteLine(e1.Message);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Doc Deleted" + doc_id);
                                    }
                                }
                            }
                            //Animals
                            
                            Dictionary<long, object> medication_data_ani = AnimalVaccinationContext.GetAnimalIdsByVaccinationIDs2(this._configuration, med_ids);
                            Dictionary<string, object> ani_data;
                            try
                            {
                                ani_data = (Dictionary<string, object>)medication_data_ani[id];
                            }
                            catch (Exception e)
                            {
                                ani_data = new Dictionary<string, object>();
                            }
                            //Dictionary<long, long> Relations = (Dictionary<long, long>)ani_data["Relations"];
                            Dictionary<long, int> AnimalIds = (Dictionary<long, int>)ani_data["AniIDS"];
                            Console.WriteLine("ani count " + AnimalIds.Count);
                            List<long> Ani_IDS = new List<long>();
                            foreach (var ani_id in AnimalIds)
                            {
                                if (ani_id.Value == 0)
                                {
                                    //Ani_IDS.Add(ani_id.Key);
                                    medication.AnimalIDs.Add(ani_id.Key);
                                }
                            }
                            if (medication.AnimalIDs.Count > 0)
                            {
                                //get animals Data here
                                AnimalContext animal_context = new AnimalContext(this._configuration);
                                AnimalFilter animalFilter = new AnimalFilter();
                                animalFilter.cols = new string[] { "Id","Name","TagNo"};
                                Dictionary<long, Dictionary<string, object>> animal_names = animal_context.GetAnimalsDataByIds(animalFilter,medication.AnimalIDs);
                                //Dictionary<long, Dictionary<string, object>> animal_names = UsersContext.GetAnimalsDataByIds(this._configuration, medication.AnimalsIDs);

                                foreach (var ani_id in AnimalIds)
                                {
                                    if (ani_id.Value == 0)
                                    {
                                        try
                                        {
                                            Dictionary<string, object> animal_name_ = animal_names[ani_id.Key];
                                            /*if (medication.AnimalDetail != "")
                                            {
                                                medication.AnimalDetail += ",";
                                            }*/
                                            //focus here
                                            //medication.AnimalDetail += animal_name_["label"].ToString() + " " + animal_name_["name"];
                                            animal_name_["name"] = animal_name_["TagNo"].ToString() + " " + animal_name_["Name"];
                                            animal_name_["Name"] = Helper.IsNullOrEmpty(animal_name_["Name"]);
                                            try
                                            {
                                                medication.Animals[Convert.ToInt64(animal_name_["Id"])] = animal_name_;
                                            }
                                            catch (Exception e2)
                                            {
                                                Console.WriteLine(e2.StackTrace);
                                                Console.WriteLine(e2.Message);
                                            }
                                        }
                                        catch (Exception e1)
                                        {
                                            Console.WriteLine("Animal not found ani id" + ani_id.Key);
                                            Console.WriteLine(e1.StackTrace);
                                            Console.WriteLine(e1.Message);
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Animal Deleted" + ani_id);
                                    }
                                }
                            }
                        }
                    }
                        else
                        {
                            Console.WriteLine("no med ids found");
                        }
                    }
                catch (Exception ex)
                {
                    Console.WriteLine("Falied sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return medication;
        }

        public Dictionary<string,object> AddVaccinationDoctors(long? id,List<long> doctorIds,SqlConnection conn2=null, SqlTransaction tran=null){
           Dictionary<string, object> data = new Dictionary<string, object>();
            if (true)
            {
                string query = "";
                SqlConnection conn = null;
                if(conn2==null){
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    conn = new SqlConnection(connectionString);
                }else{
                    conn = conn2;
                }
                if(true)
                {
                    
                    string values = "";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    try
                    {
                        if(conn2==null){
                            conn.Open();
                        }
                        int i = 0;
                        foreach(var m in doctorIds){
                            if(values!=""){
                                values+=",";
                            }
                            values += $"(@MedicationID{i},@DoctorID{i})";
                            i++;
                        }
                        query = $"Insert into MedicationDoctors (MedicationID,DoctorID) values {values}";
                        Console.WriteLine(query);
                        sqlcmd.CommandText = query;
                        i=0;
                        foreach(var m in doctorIds){
                            sqlcmd.Parameters.Add($"@MedicationID{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@MedicationID{i}"].Value = id;
                            sqlcmd.Parameters.Add($"@DoctorID{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@DoctorID{i}"].Value = (long)m;
                            i++;
                        }
                        if(tran!=null){
                            tran.Save("addVaccinationDoctors");
                            sqlcmd.Transaction = tran;
                        }
                        int j = sqlcmd.ExecuteNonQuery();
                        if (j > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animal Vaccination Data Saved Successfully";
                            data2["status"] = "success";
                            data["data"] = data2;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animal Vaccination Data Saving Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }catch(Exception e)
                    {
                        Console.WriteLine("Exception : "+e.Message);
                    }
                }
                else
                {
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Adding Vaccination Doctors Failed! Invaid Data Enterred";
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }
            return data;
        }
        public Dictionary<string, object> AddVaccinationAnimals(long? id, List<long> animalIDs, SqlConnection conn2 = null, SqlTransaction tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (true)
            {
                string query = "";
                SqlConnection conn = null;
                if (conn2 == null)
                {
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    conn = new SqlConnection(connectionString);
                }
                else
                {
                    conn = conn2;
                }
                if (true)
                {

                    string values = "";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    try
                    {
                        if (conn2 == null)
                        {
                            conn.Open();
                        }
                        int i = 0;
                        foreach (var m in animalIDs)
                        {
                            if (values != "")
                            {
                                values += ",";
                            }
                            values += $"(@MedicationID{i},@AnimalID{i},@Deleted{i})";
                            i++;
                        }
                        query = $"Insert into MedicationAnimalIDs (MedicationID,AnimalID,Deleted) values {values}";
                        Console.WriteLine(query);
                        sqlcmd.CommandText = query;
                        i = 0;
                        foreach (var m in animalIDs)
                        {
                            sqlcmd.Parameters.Add($"@MedicationID{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@MedicationID{i}"].Value = id;
                            sqlcmd.Parameters.Add($"@AnimalID{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@AnimalID{i}"].Value = (long)m;
                            sqlcmd.Parameters.Add($"@Deleted{i}", System.Data.SqlDbType.Bit);
                            sqlcmd.Parameters[$"@Deleted{i}"].Value = false;
                            i++;
                        }
                        if (tran != null)
                        {
                            tran.Save("addVaccinationAnimals");
                            sqlcmd.Transaction = tran;
                        }
                        int j = sqlcmd.ExecuteNonQuery();
                        if (j > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animals Added Successfully to Vaccination";
                            data2["status"] = "success";
                            data["data"] = data2;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animals Addition Failed Vaccination";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message);
                    }
                }
                else
                {
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Adding Animals to Vaccination Failed! Invaid Data Enterred";
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }
            return data;
        }
        
        public Dictionary<string, object> DeleteVaccinationDoctors(long? medicationID, List<long> doctorIdsToBeDeleted, SqlConnection conn2 = null, SqlTransaction tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (true)
            {
                string query = "";
                SqlConnection conn = null;
                if (conn2 == null)
                {
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    conn = new SqlConnection(connectionString);
                }
                else
                {
                    conn = conn2;
                }
                if (true)
                {

                    string values = "";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    try
                    {
                        if (conn2 == null)
                        {
                            conn.Open();
                        }
                        int i = 0;
                        foreach (long m in doctorIdsToBeDeleted)
                        {
                            if (values != "")
                            {
                                values += ",";
                            }
                            values += $"@Id{i}";
                            i++;
                        }
                        query = $"Update MedicationDoctors set Deleted = 1 where Id in ({values}) and MedicationID = @MedicationID";
                        Console.WriteLine(query);
                        sqlcmd.CommandText = query;
                        i = 0;
                        foreach (long m in doctorIdsToBeDeleted)
                        {
                            sqlcmd.Parameters.Add($"@Id{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@Id{i}"].Value = m;
                            Console.WriteLine("id del : " + m);
                            i++;
                        }
                        sqlcmd.Parameters.Add($"@MedicationID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters[$"@MedicationID"].Value = medicationID;
                        Console.WriteLine("Medication ID "+ medicationID);
                        if (tran != null)
                        {
                            tran.Save("deleteVaccinationDoctors");
                            sqlcmd.Transaction = tran;
                        }
                        int j = sqlcmd.ExecuteNonQuery();
                        if (j > 0)
                        {
                            Console.WriteLine("Delete successfullly " + j);
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Doctors Deleted Successfully from this medication";
                            data2["status"] = "success";
                            data["data"] = data2;
                        }
                        else
                        {
                            Console.WriteLine("Deletion Failed " + j);
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Doctor Deletion Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message);
                    }
                }
                else
                {
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Doctor Deletion Failed";
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }
            return data;
        }
        public Dictionary<string, object> RecoverVaccinationDoctors(long? medicationID, List<long> doctorIdsToBeRecovered, SqlConnection conn2 = null, SqlTransaction tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (true)
            {
                string query = "";
                SqlConnection conn = null;
                if (conn2 == null)
                {
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    conn = new SqlConnection(connectionString);
                }
                else
                {
                    conn = conn2;
                }
                if (true)
                {

                    string values = "";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    try
                    {
                        if (conn2 == null)
                        {
                            conn.Open();
                        }
                        int i = 0;
                        foreach (long m in doctorIdsToBeRecovered)
                        {
                            if (values != "")
                            {
                                values += ",";
                            }
                            values += $"@Id{i}";
                            i++;
                        }
                        query = $"Update MedicationDoctors set Deleted = 0 where Id in ({values}) and MedicationID = @MedicationID";
                        Console.WriteLine(query);
                        sqlcmd.CommandText = query;
                        i = 0;
                        foreach (long m in doctorIdsToBeRecovered)
                        {
                            sqlcmd.Parameters.Add($"@Id{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@Id{i}"].Value = m;
                            Console.WriteLine("REC " +m);
                            i++;
                        }
                        sqlcmd.Parameters.Add($"@MedicationID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters[$"@MedicationID"].Value = medicationID;
                        Console.WriteLine(medicationID);
                        if (tran != null)
                        {
                            tran.Save("deleteVaccinationDoctors");
                            sqlcmd.Transaction = tran;
                        }
                        int j = sqlcmd.ExecuteNonQuery();
                        if (j > 0)
                        {
                            Console.WriteLine("Recovered Successful " + j);
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Doctors Recovered Successfully from this medication";
                            data2["status"] = "success";
                            data["data"] = data2;
                        }
                        else
                        {
                            Console.WriteLine("Recovered Failed " + j);
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Doctor Recover Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message);
                    }
                }
                else
                {
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Doctor Deletion Failed";
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }
            return data;
        }
        public Dictionary<string, object> GetVaccinationDetailByAnimalId(long id,int page=1,int limit=10){
            int sno = 1;
            Dictionary<string, object> data = new Dictionary<string, object>();
            ArrayList ar = new ArrayList();
            List<long> med_ids = new List<long>();
            int offset = 0;
            if(page>1){
                offset = ((page -1) *limit);
            }else{
                offset = 0;
            }
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            Dictionary<string,string> animalDetail = CowsContext.GetCowNameTagNoById(this._configuration,id);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //get Medication IDS by AnimalI
                List<long> medi_IDS = this.GetMedicationIDsByAnimalId(id);
                string ids = "";
                string where = "where ";
                string vaccinationClause = " VaccinationID is not NULL ";
                if (medi_IDS.Count > 0)
                {
                    foreach (long id_ in medi_IDS)
                    {
                        if (ids != "")
                        {
                            ids += ",";
                        }
                        ids += id_.ToString();
                    }
                    if (ids.Length != 0)
                    {
                        where += $" (AnimalID = @AnimalID  or Id in ({ids}) )";
                    }
                    else
                    {
                        where += $" AnimalID = @AnimalID ";
                    }
                    where += " and " + vaccinationClause;
                }
                else
                {
                    where += $" AnimalID = @AnimalID and " + vaccinationClause;
                }
                string query = $"select * from Medication {where} order by Date Desc OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@AnimalId", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@AnimalId"].Value = id;
                    sqlcmd.Parameters.Add("@Offset", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Offset"].Value = offset;
                    sqlcmd.Parameters.Add("@Limit", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Limit"].Value = limit;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> medication = new Dictionary<string, object>();
                        medication["sno"] = sno;
                        sno++;
                        medication["Id"] = sqlrdr["Id"];
                        try{
                            medication["Date"] = Helper.FormatDate((DateTime)sqlrdr["Date"]);
                        }catch{
                            medication["Date"] = "";
                        }
                        //medication["AnimalID"] = sqlrdr["AnimalID"];
                        medication["AnimalID"] = id;
                        medication["AnimalNo"] = animalDetail["tagNo"]+" / "+ animalDetail["name"];
                        medication["Disease"] = Helper.IsNullOrEmpty(sqlrdr["Disease"]);
                        medication["Symptoms"] = sqlrdr["Symptoms"];
                        medication["Diagnosis"] = sqlrdr["Diagnosis"];
                        medication["Treatment"] = sqlrdr["Treatment"];
                        medication["Prognosis"] = sqlrdr["Prognosis"].ToString();
                        medication["Result"] = sqlrdr["Result"];
                        medication["DoctorDetail"] = sqlrdr["DoctorDetail"].ToString();
                        medication["DoctorDetail2"] = "";
                        medication["CostofTreatment"] = Helper.IsNullOrEmpty(sqlrdr["CostofTreatment2"]);
                        medication["VaccinationID"] = Helper.IsNullOrEmpty(sqlrdr["VaccinationID"]);
                        medication["Remarks"] = sqlrdr["Remarks"].ToString();
                        ar.Add(medication);
                        med_ids.Add(Convert.ToInt64(medication["Id"]));
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("COnnection closed");
                    if(med_ids.Count>0){
                        Dictionary<long, object> medication_data = AnimalVaccinationContext.GetDoctorIdsByVaccinationIDs2(this._configuration,med_ids);
                        List<long> Doc_Ids = new List<long>();
                        foreach(var m in medication_data)
                        {
                            Dictionary<string, object> doc_data_ = (Dictionary<string, object>)m.Value;
                            Dictionary<long, int> Doc_IDS = (Dictionary<long, int>)doc_data_["DocIDS"];
                            foreach(var n in Doc_IDS)
                            {
                                Doc_Ids.Add(n.Key);
                            }
                        }
                        Console.Write("Doc IDS Count " + Doc_Ids.Count);
                        if (Doc_Ids.Count > 0)
                        {
                            Dictionary<long, Dictionary<string, object>> doc_names = UsersContext.GetDoctorsDataByIds(this._configuration,Doc_Ids);
                            
                            foreach(Dictionary<string,object>m in ar)
                            {
                                try
                                {
                                    Dictionary<long, int> docIds;// = relations[Convert.ToInt64(m["Id"])];
                                    try
                                    {
                                        Dictionary<string, object> doc_data__ = (Dictionary<string, object>)medication_data[Convert.ToInt64(m["Id"])];
                                        docIds = (Dictionary<long, int>)doc_data__["DocIDS"];
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                    foreach (var n in docIds)
                                    {
                                        if (n.Value == 0)
                                        {
                                            Console.WriteLine("doc id " + n.Key);
                                            try
                                            {
                                                Dictionary<string, object> doc_name_ = doc_names[n.Key];
                                                if (m["DoctorDetail2"].ToString() != "")
                                                {
                                                    m["DoctorDetail2"] += ",";
                                                }
                                                m["DoctorDetail2"] += doc_name_["label"].ToString() + " " + doc_name_["name"];
                                                m["DoctorDetail"] = m["DoctorDetail2"];
                                            }
                                            catch (Exception e1)
                                            {
                                                Console.WriteLine("Doc not found doc id" + n.Key);
                                                Console.WriteLine(e1.StackTrace);
                                                Console.WriteLine(e1.Message);
                                            }
                                        }
                                        else
                                        {
                                            Console.WriteLine("Doc Deleted doc id" + n.Key);
                                        }
                                    }
                                }
                                catch(Exception e)
                                {
                                    Console.WriteLine("Doc ids not found");
                                }
                            }
                        }
                    }else{
                        Console.WriteLine("no med ids found");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            data["data"] = ar;
            data["recordsTotal"] = this.GetTotalVaccinationDetailByAnimalId(id);
            data["recordsFiltered"] = data["recordsTotal"];//Filteration pending
            return data;
        }
        public List<long> GetMedicationIDsByAnimalId(long id)
        {
            List<long> medicationIds = new List<long>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            //Dictionary<string, string> animalDetail = CowsContext.GetCowNameTagNoById(this._configuration, id);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from MedicationAnimalIDs where AnimalId = @AnimalId and Deleted = 0";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@AnimalId", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@AnimalId"].Value = id;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        medicationIds.Add(Convert.ToInt32(sqlrdr["MedicationID"]));
                    }
                    sqlrdr.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return medicationIds;
        }
        public long GetTotalVaccinationDetailByAnimalId(long id)
        {
            long total = 0;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            Dictionary<string, string> animalDetail = CowsContext.GetCowNameTagNoById(this._configuration, id);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                List<long> medi_IDS = this.GetMedicationIDsByAnimalId(id);
                string ids = "";
                string where = "where ";
                string vaccinationClause = " VaccinationID is not NULL ";
                if (medi_IDS.Count > 0)
                {
                    foreach (long id_ in medi_IDS)
                    {
                        if (ids != "")
                        {
                            ids += ",";
                        }
                        ids += id_.ToString();
                    }
                    if (ids.Length != 0)
                    {
                        where += $" (AnimalID = @AnimalID  or Id in ({ids}) )";
                    }
                    else
                    {
                        where += $" AnimalID = @AnimalID ";
                    }
                    where += " and "+vaccinationClause;
                }
                else
                {
                    where += $" AnimalID = @AnimalID and "+vaccinationClause;
                }
                
                //string query = $"select * from Medication {where} order by Date Desc OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
                string query = $"select count(*) as total from Medication {where} ";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@AnimalId", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@AnimalId"].Value = id;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        total = Convert.ToInt64(sqlrdr["total"]);
                    }
                    sqlrdr.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return total;
        }
        public static Dictionary<string, object> GetDoctorIdsByVaccinationIDs(IConfiguration _configuration, List<long> med_ids)
        {
            Dictionary<string, object> ret_data = new Dictionary<string, object>();
            Dictionary<long, List<long>> data = new Dictionary<long, List<long>>();
            List<long> DoctorIds = new List<long>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            string ids = "";
            foreach(long m in med_ids)
            {
                if (ids != "")
                {
                    ids += ",";
                }
                ids += m.ToString();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = $"select * from MedicationDoctors where MedicationID in ({ids})";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        List<long> data_;
                        try
                        {
                            data_ = data[Convert.ToInt64(sqlrdr["MedicationID"])];
                        }
                        catch(Exception e)
                        {
                            data_ = new List<long>();
                        }
                        data_.Add(Convert.ToInt64(sqlrdr["DoctorID"]));
                        if (!DoctorIds.Contains(Convert.ToInt64(sqlrdr["DoctorID"])))
                        {
                            DoctorIds.Add(Convert.ToInt64(sqlrdr["DoctorID"]));
                        }
                        data[Convert.ToInt64(sqlrdr["MedicationID"])] = data_;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("COnnection closed");
                    //Dictionary<long, Dictionary<long, long>> med_doc_ids_relation = this.GetDoctoreIdsByVaccinationIDs(med_ids);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Falied sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            ret_data["Relations"] = data;
            ret_data["DoctorIds"] = DoctorIds;
            return ret_data;
        }
        public static Dictionary<long, object> GetDoctorIdsByVaccinationIDs2(IConfiguration _configuration, List<long> med_ids)
        {
            Dictionary<string, object> ret_data = new Dictionary<string, object>();
            Dictionary<long, List<long>> data = new Dictionary<long, List<long>>();
            Dictionary<long,int> DoctorIds = new Dictionary<long, int>();
            Dictionary<long,int> VaccinationIds = new Dictionary<long, int>();
            Dictionary<long, object> VaccinationIDS = new Dictionary<long, object>();

            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            string ids = "";
            foreach (long m in med_ids)
            {
                if (ids != "")
                {
                    ids += ",";
                }
                ids += m.ToString();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = $"select * from MedicationDoctors where MedicationID in ({ids})";
                
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        List<long> data_;
                        Dictionary<long, int> DocIDs_;
                        Dictionary<long, long> Relations;
                        Dictionary<string, object> data__;
                        try
                        {
                            data__ = (Dictionary<string, object>)VaccinationIDS[Convert.ToInt64(sqlrdr["MedicationID"])];
                            DocIDs_ = (Dictionary<long, int>)data__["DocIDS"];
                            Relations = (Dictionary<long, long>)data__["Relations"];
                            //data_ = data[Convert.ToInt64(sqlrdr["VaccinationID"])];
                        }
                        catch (Exception e)
                        {
                            data__ = new Dictionary<string, object>();
                            DocIDs_ = new Dictionary<long, int>();
                            Relations = new Dictionary<long, long>();
                            //data_ = new List<long>();
                        }
                        //Console.WriteLine("deleted : "+ sqlrdr["Deleted"] + Convert.ToInt32(sqlrdr["Deleted"]));
                        if (Convert.ToInt32(sqlrdr["Deleted"]) == 0 || Convert.ToInt32(sqlrdr["Deleted"]) == 1)
                        {
                            //data_.Add(Convert.ToInt64(sqlrdr["DoctorID"]));
                            DocIDs_[Convert.ToInt64(sqlrdr["DoctorID"])] = Convert.ToInt32(sqlrdr["Deleted"]);
                            Relations[Convert.ToInt64(sqlrdr["DoctorID"])] = Convert.ToInt64(sqlrdr["Id"]);
                        }
                        data__["DocIDS"] = DocIDs_;
                        data__["Relations"] = Relations;
                        VaccinationIDS[Convert.ToInt64(sqlrdr["MedicationID"])] = data__;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("COnnection closed");
                    //Dictionary<long, Dictionary<long, long>> med_doc_ids_relation = this.GetDoctoreIdsByVaccinationIDs(med_ids);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Falied sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            //ret_data["Relations"] = data;
            //ret_data["DoctorIds"] = DoctorIds;
            //ret_data["VaccinationIds"] = VaccinationIds;
            return VaccinationIDS;
        }
        public static Dictionary<long, object> GetAnimalIdsByVaccinationIDs2(IConfiguration _configuration, List<long> med_ids)
        {
            Dictionary<string, object> ret_data = new Dictionary<string, object>();
            Dictionary<long, List<long>> data = new Dictionary<long, List<long>>();
            Dictionary<long, int> AnimalIds = new Dictionary<long, int>();
            Dictionary<long, int> VaccinationIds = new Dictionary<long, int>();
            Dictionary<long, object> VaccinationIDS = new Dictionary<long, object>();

            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            string ids = "";
            foreach (long m in med_ids)
            {
                if (ids != "")
                {
                    ids += ",";
                }
                ids += m.ToString();
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = $"select * from MedicationAnimalIds where MedicationID in ({ids})";

                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        List<long> data_;
                        Dictionary<long, int> AniIDs_;
                        Dictionary<long, long> Relations;
                        Dictionary<string, object> data__;
                        try
                        {
                            data__ = (Dictionary<string, object>)VaccinationIDS[Convert.ToInt64(sqlrdr["MedicationID"])];
                            AniIDs_ = (Dictionary<long, int>)data__["AniIDS"];
                            Relations = (Dictionary<long, long>)data__["Relations"];
                            //data_ = data[Convert.ToInt64(sqlrdr["VaccinationID"])];
                        }
                        catch (Exception e)
                        {
                            data__ = new Dictionary<string, object>();
                            AniIDs_ = new Dictionary<long, int>();
                            Relations = new Dictionary<long, long>();
                            //data_ = new List<long>();
                        }
                        //Console.WriteLine("deleted : "+ sqlrdr["Deleted"] + Convert.ToInt32(sqlrdr["Deleted"]));
                        if (Convert.ToInt32(sqlrdr["Deleted"]) == 0 || Convert.ToInt32(sqlrdr["Deleted"]) == 1)
                        {
                            //data_.Add(Convert.ToInt64(sqlrdr["DoctorID"]));
                            AniIDs_[Convert.ToInt64(sqlrdr["AnimalID"])] = Convert.ToInt32(sqlrdr["Deleted"]);
                            Relations[Convert.ToInt64(sqlrdr["AnimalID"])] = Convert.ToInt64(sqlrdr["Id"]);
                        }
                        data__["AniIDS"] = AniIDs_;
                        data__["Relations"] = Relations;
                        VaccinationIDS[Convert.ToInt64(sqlrdr["MedicationID"])] = data__;
                        /*if (DoctorIds.ContainsKey((Convert.ToInt64(sqlrdr["DoctorID"]))) == false)
                        {
                            DoctorIds[Convert.ToInt64(sqlrdr["DoctorID"])] = Convert.ToInt32(sqlrdr["Deleted"]);
                            
                        }
                        if (VaccinationIds.ContainsKey((Convert.ToInt64(sqlrdr["DoctorID"]))) == false)
                        {
                            VaccinationIds[Convert.ToInt64(sqlrdr["DoctorID"])] = Convert.ToInt32(sqlrdr["Id"]);
                        }
                        data[Convert.ToInt64(sqlrdr["VaccinationID"])] = data_;*/
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("COnnection closed");
                    //Dictionary<long, Dictionary<long, long>> med_doc_ids_relation = this.GetDoctoreIdsByVaccinationIDs(med_ids);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Falied sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            //ret_data["Relations"] = data;
            //ret_data["DoctorIds"] = DoctorIds;
            //ret_data["VaccinationIds"] = VaccinationIds;
            return VaccinationIDS;
        }
        public Dictionary<string, object> RecoverVaccinationAnimals(long? medicationID, List<long> animalIdsToBeRecovered, SqlConnection conn2 = null, SqlTransaction tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (true)
            {
                string query = "";
                SqlConnection conn = null;
                if (conn2 == null)
                {
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    conn = new SqlConnection(connectionString);
                }
                else
                {
                    conn = conn2;
                }
                if (true)
                {

                    string values = "";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    try
                    {
                        if (conn2 == null)
                        {
                            conn.Open();
                        }
                        int i = 0;
                        foreach (long m in animalIdsToBeRecovered)
                        {
                            if (values != "")
                            {
                                values += ",";
                            }
                            values += $"@Id{i}";
                            i++;
                        }
                        query = $"Update MedicationAnimalIDs set Deleted = 0 where Id in ({values}) and MedicationID = @MedicationID";
                        Console.WriteLine(query);
                        sqlcmd.CommandText = query;
                        i = 0;
                        foreach (long m in animalIdsToBeRecovered)
                        {
                            sqlcmd.Parameters.Add($"@Id{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@Id{i}"].Value = m;
                            Console.WriteLine("REC " + m);
                            i++;
                        }
                        sqlcmd.Parameters.Add($"@MedicationID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters[$"@MedicationID"].Value = medicationID;
                        Console.WriteLine(medicationID);
                        if (tran != null)
                        {
                            tran.Save("deleteVaccinationanimals");
                            sqlcmd.Transaction = tran;
                        }
                        int j = sqlcmd.ExecuteNonQuery();
                        if (j > 0)
                        {
                            Console.WriteLine("Recovered Successful " + j);
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animals Recovered Successfully from this medication";
                            data2["status"] = "success";
                            data["data"] = data2;
                        }
                        else
                        {
                            Console.WriteLine("Recovered Failed " + j);
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animal Recover Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message);
                    }
                }
                else
                {
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Animal Recover Failed";
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }
            return data;
        }
        public Dictionary<string, object> DeleteVaccinationAnimals(long? medicationID, List<long> animalIdsToBeDeleted, SqlConnection conn2 = null, SqlTransaction tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (true)
            {
                string query = "";
                SqlConnection conn = null;
                if (conn2 == null)
                {
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    conn = new SqlConnection(connectionString);
                }
                else
                {
                    conn = conn2;
                }
                if (true)
                {

                    string values = "";
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    try
                    {
                        if (conn2 == null)
                        {
                            conn.Open();
                        }
                        int i = 0;
                        foreach (long m in animalIdsToBeDeleted)
                        {
                            if (values != "")
                            {
                                values += ",";
                            }
                            values += $"@Id{i}";
                            i++;
                        }
                        query = $"Update MedicationAnimalIDs set Deleted = 1 where Id in ({values}) and MedicationID = @MedicationID";
                        Console.WriteLine(query);
                        sqlcmd.CommandText = query;
                        i = 0;
                        foreach (long m in animalIdsToBeDeleted)
                        {
                            sqlcmd.Parameters.Add($"@Id{i}", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters[$"@Id{i}"].Value = m;
                            Console.WriteLine("id del : " + m);
                            i++;
                        }
                        sqlcmd.Parameters.Add($"@MedicationID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters[$"@MedicationID"].Value = medicationID;
                        Console.WriteLine("Vaccination ID " + medicationID);
                        if (tran != null)
                        {
                            tran.Save("deleteVaccinationAnimals");
                            sqlcmd.Transaction = tran;
                        }
                        int j = sqlcmd.ExecuteNonQuery();
                        if (j > 0)
                        {
                            Console.WriteLine("Delete successfullly " + j);
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animals Deleted Successfully from this medication";
                            data2["status"] = "success";
                            data["data"] = data2;
                        }
                        else
                        {
                            Console.WriteLine("Deletion Failed " + j);
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Animal Deletion Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message);
                    }
                }
                else
                {
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Animal Deletion Failed";
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }
            return data;
        }

    }
}
