using GaushalaAPI.Models;
using GaushalAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class MedicationContext 
    {
        private readonly IConfiguration _configuration;
        public MedicationContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddMedicationDetail(MedicationModel medication)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            
            if (medication.ValidateMedication("Add") == true)
            {
                string query = "Insert into Medication (Date,AnimalID,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.ID Values"+
                "(@Date,@AnimalID,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    sqlcmd.Parameters.Add("@Date", System.Data.SqlDbType.DateTime);
                    sqlcmd.Parameters["@Date"].Value = medication.Date;
                    sqlcmd.Parameters.Add("@AnimalID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@AnimalID"].Value =medication.AnimalID;
                    sqlcmd.Parameters.Add("@Disease", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Disease"].Value = medication.Disease;
                    sqlcmd.Parameters.Add("@Symptoms", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Symptoms"].Value = medication.Symptoms;
                    sqlcmd.Parameters.Add("@Diagnosis", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Diagnosis"].Value = medication.Diagnosis;
                    sqlcmd.Parameters.Add("@Treatment", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Treatment"].Value = medication.Treatment;
                    sqlcmd.Parameters.Add("@Prognosis", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Prognosis"].Value = medication.Prognosis;
                    sqlcmd.Parameters.Add("@Result", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Result"].Value = medication.Result;
                    sqlcmd.Parameters.Add("@CostofTreatment2", System.Data.SqlDbType.Decimal);
                    sqlcmd.Parameters["@CostofTreatment2"].Value = medication.CostOfTreatment2;
                    sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Remarks"].Value = medication.Remarks;

                    //sqlcmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@category"].Value = cow.Category;
                    SqlTransaction tran = null;
                    try
                    {
                        
                        conn.Open();
                        tran = conn.BeginTransaction("Medication");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        medication.Id = (Int64)sqlcmd.ExecuteScalar();
                        Dictionary<string,object> mediDocData= this.AddMedicationDoctors(medication.Id,medication.DoctorIDs,conn,tran);
                        Dictionary<string, string> mediDocData_ = (Dictionary<string, string>)mediDocData["data"];
                        if(mediDocData_["status"]=="success"){
                            if(medication.Id >0){
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Cow Medication Data Saved Successfully."+mediDocData_["message"];
                                data2["status"] = "success";
                                data["data"] = data2;
                            }else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Cow Medication Date Saving Failed";
                                data2["status"] = "failure";
                                data["data"] = data2;
                            }
                        }
                        else
                        {
                            tran.Rollback();
                            Console.WriteLine("rolling back second");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Medication Date Saving Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }catch(Exception e)
                    {
                        Console.WriteLine("Exception : "+e.Message);
                    }
                    
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Cow Medication Date Saving Failed.Invalid Data Entered";
                data2["status"] = "failure";
                data["errors"] = medication.errors;
                data["data"] = data2;
            }
            return data;
            
        }

        internal Dictionary<string, object> EditMedicationDetail(MedicationModel medication)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (medication.ValidateMedication("Edit") == true)
            {
                string query = "UPdate Medication set Date = @Date,AnimalID = @AnimalID,Disease = @Disease,Symptoms = @Symptoms,Diagnosis = @Diagnosis," +
                    "Prognosis = @Prognosis,Treatment = @Treatment,Result = @Result,CostofTreatment2 = @CostofTreatment2,Remarks = @Remarks  where Id = @Id";
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = medication.Id;
                    sqlcmd.Parameters.Add("@Date", System.Data.SqlDbType.DateTime);
                    sqlcmd.Parameters["@Date"].Value = medication.Date;
                    sqlcmd.Parameters.Add("@AnimalID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@AnimalID"].Value = medication.AnimalID;
                    sqlcmd.Parameters.Add("@Disease", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Disease"].Value = medication.Disease;
                    sqlcmd.Parameters.Add("@Symptoms", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Symptoms"].Value = medication.Symptoms;
                    sqlcmd.Parameters.Add("@Diagnosis", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Diagnosis"].Value = medication.Diagnosis;
                    sqlcmd.Parameters.Add("@Treatment", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Treatment"].Value = medication.Treatment;
                    sqlcmd.Parameters.Add("@Prognosis", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Prognosis"].Value = medication.Prognosis;
                    sqlcmd.Parameters.Add("@Result", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Result"].Value = medication.Result;
                    sqlcmd.Parameters.Add("@CostofTreatment2", System.Data.SqlDbType.Decimal);
                    sqlcmd.Parameters["@CostofTreatment2"].Value = medication.CostOfTreatment2;
                    sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Remarks"].Value = medication.Remarks;
                    SqlTransaction tran = null;
                    try
                    {

                        conn.Open();
                        tran = conn.BeginTransaction("Medication");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        int i = sqlcmd.ExecuteNonQuery();
                        Dictionary<string, object> mediDocData = this.UpdateMedicationDoctors(medication.Id, medication.DoctorIDs,conn,tran);
                        Dictionary<string, string> mediDocData_ = (Dictionary<string, string>)mediDocData["data"];
                        if (mediDocData_["status"] == "success")
                        {
                            if (i > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Cow Medication Data Saved Successfully." + mediDocData_["message"];
                                data2["status"] = "success";
                                data["data"] = data2;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Cow Medication Date Saving Failed";
                                data2["status"] = "failure";
                                data["data"] = data2;
                            }
                        }
                        else
                        {
                            tran.Rollback();
                            Console.WriteLine("rolling back second");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Medication Date Saving Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message);
                    }
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Cow Medication Date Saving Failed.Invalid Data Entered";
                data2["status"] = "failure";
                data["errors"] = medication.errors;
                data["data"] = data2;
            }
            return data;
        }

        private Dictionary<string, object> UpdateMedicationDoctors(long? medication_id, List<long> newDoctorIDs, SqlConnection conn2, SqlTransaction tran)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //get the old doctor ids
            List<long> medication_ids = new List<long>();
            medication_ids.Add((long)medication_id);
            Dictionary<long, object> medication_data = MedicationContext.GetDoctorIdsByMedicationIDs2(this._configuration, medication_ids);
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

            Dictionary<long, long> OldMedicationIds = (Dictionary<long, long>)doc_data["Relations"];
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
                    OldMedicationIds.TryGetValue(doc_id, out med_id);
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
                        OldMedicationIds.TryGetValue(doc_id.Key, out med_id);
                        DoctorIdsToBeDeleted.Add(med_id);
                    }
                }
            }
            bool addData_ = true;
            bool deleteData_ = true;
            bool recoverData_ = true;
            string id_message = "";
            if (DoctorIdsToBeAdded.Count > 0) {
                Dictionary<string, object> addData = this.AddMedicationDoctors(medication_id, DoctorIdsToBeAdded, conn2, tran);
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
                Dictionary<string, object>  deleteData = this.DeleteMedicationDoctors(medication_id, DoctorIdsToBeDeleted, conn2, tran);
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
                Dictionary<string, object>  recoverData = this.RecoverMedicationDoctors(medication_id, DoctorIdsToBeRecovered, conn2, tran);
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

        internal MedicationModel? GetMedicationDetailById(long id)
        {
            MedicationModel? medication = null;
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
                        medication = new MedicationModel(sqlrdr);
                        medication.DoctorIDs = new List<long>();
                        
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
                            Dictionary<long, object> medication_data = MedicationContext.GetDoctorIdsByMedicationIDs2(this._configuration, med_ids);
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
                        }
                        else
                        {
                            Console.WriteLine("no med ids found");
                        }
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

        public Dictionary<string,object> AddMedicationDoctors(long? id,List<long> doctorIds,SqlConnection conn2=null, SqlTransaction tran=null){
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
                            tran.Save("addMedicationDoctors");
                            sqlcmd.Transaction = tran;
                        }
                        int j = sqlcmd.ExecuteNonQuery();
                        if (j > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Medication Data Saved Successfully";
                            data2["status"] = "success";
                            data["data"] = data2;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Medication Date Saving Failed";
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
                    data2["message"] = "Adding Medication Doctors Failed! Invaid Data Enterred";
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }
            return data;
        }

        public Dictionary<string, object> DeleteMedicationDoctors(long? medicationID, List<long> doctorIdsToBeDeleted, SqlConnection conn2 = null, SqlTransaction tran = null)
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
                            tran.Save("deleteMedicationDoctors");
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
        }public Dictionary<string, object> RecoverMedicationDoctors(long? medicationID, List<long> doctorIdsToBeRecovered, SqlConnection conn2 = null, SqlTransaction tran = null)
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
                            tran.Save("deleteMedicationDoctors");
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
        public Dictionary<string, object> GetMedicationDetailByAnimalId(long id,int page=1,int limit=10){
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
                string query = "select * from Medication where AnimalId = @AnimalId order by Date Desc OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";
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
                        medication["AnimalID"] = sqlrdr["AnimalID"];
                        medication["AnimalNo"] = animalDetail["tagNo"]+" / "+ animalDetail["name"];
                        medication["Disease"] = sqlrdr["Disease"];
                        medication["Symptoms"] = sqlrdr["Symptoms"];
                        medication["Diagnosis"] = sqlrdr["Diagnosis"];
                        medication["Treatment"] = sqlrdr["Treatment"];
                        medication["Prognosis"] = sqlrdr["Prognosis"].ToString();
                        medication["Result"] = sqlrdr["Result"];
                        medication["DoctorDetail"] = sqlrdr["DoctorDetail"].ToString();
                        medication["DoctorDetail2"] = "";
                        medication["CostofTreatment"] = sqlrdr["CostofTreatment2"];
                        medication["Remarks"] = sqlrdr["Remarks"].ToString();
                        ar.Add(medication);
                        med_ids.Add(Convert.ToInt64(medication["Id"]));
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("COnnection closed");
                    if(med_ids.Count>0){
                        Dictionary<long, object> medication_data = MedicationContext.GetDoctorIdsByMedicationIDs2(this._configuration,med_ids);
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
                            /*foreach(var m in doc_names)
                            {
                                Dictionary<string, object> dd = (Dictionary<string, object>)m.Value;
                                foreach(var m1 in dd)
                                {
                                    Console.WriteLine(m1.Key.ToString() + " " + m1.Value.ToString());
                                }
                                //Console.WriteLine(dd["Name"] + " " + dd["Label"]);
                                Console.WriteLine(m.Key.ToString() + " " + m.Value.ToString());
                            }*/
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
            data["recordsTotal"] = this.GetTotalMedicationDetailByAnimalId(id);
            data["recordsFiltered"] = data["recordsTotal"];//Filteration pending
            return data;
        }
        public long GetTotalMedicationDetailByAnimalId(long id)
        {
            long total = 0;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            Dictionary<string, string> animalDetail = CowsContext.GetCowNameTagNoById(this._configuration, id);
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select count(*) as total from Medication where AnimalId = @AnimalId";
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
        public static Dictionary<string, object> GetDoctorIdsByMedicationIDs(IConfiguration _configuration, List<long> med_ids)
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
                    //Dictionary<long, Dictionary<long, long>> med_doc_ids_relation = this.GetDoctoreIdsByMedicationIDs(med_ids);
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
        public static Dictionary<long, object> GetDoctorIdsByMedicationIDs2(IConfiguration _configuration, List<long> med_ids)
        {
            Dictionary<string, object> ret_data = new Dictionary<string, object>();
            Dictionary<long, List<long>> data = new Dictionary<long, List<long>>();
            Dictionary<long,int> DoctorIds = new Dictionary<long, int>();
            Dictionary<long,int> MedicationIds = new Dictionary<long, int>();
            Dictionary<long, object> MedicationIDS = new Dictionary<long, object>();

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
                            data__ = (Dictionary<string, object>)MedicationIDS[Convert.ToInt64(sqlrdr["MedicationID"])];
                            DocIDs_ = (Dictionary<long, int>)data__["DocIDS"];
                            Relations = (Dictionary<long, long>)data__["Relations"];
                            //data_ = data[Convert.ToInt64(sqlrdr["MedicationID"])];
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
                        MedicationIDS[Convert.ToInt64(sqlrdr["MedicationID"])] = data__;
                        /*if (DoctorIds.ContainsKey((Convert.ToInt64(sqlrdr["DoctorID"]))) == false)
                        {
                            DoctorIds[Convert.ToInt64(sqlrdr["DoctorID"])] = Convert.ToInt32(sqlrdr["Deleted"]);
                            
                        }
                        if (MedicationIds.ContainsKey((Convert.ToInt64(sqlrdr["DoctorID"]))) == false)
                        {
                            MedicationIds[Convert.ToInt64(sqlrdr["DoctorID"])] = Convert.ToInt32(sqlrdr["Id"]);
                        }
                        data[Convert.ToInt64(sqlrdr["MedicationID"])] = data_;*/
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("COnnection closed");
                    //Dictionary<long, Dictionary<long, long>> med_doc_ids_relation = this.GetDoctoreIdsByMedicationIDs(med_ids);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Falied sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            //ret_data["Relations"] = data;
            //ret_data["DoctorIds"] = DoctorIds;
            //ret_data["MedicationIds"] = MedicationIds;
            return MedicationIDS;
        }
    }
}
