using GaushalaAPI.Models;
using GaushalAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class CowsServiceContext 
    {
        private readonly IConfiguration _configuration;
        public CowsServiceContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        internal Dictionary<string, object> AddCowServiceData(CowServiceDataModel conceive)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            CowServiceDataModel._configuration = this._configuration;
            if (conceive.ValidateAdd() == true)
            {
                string cols = "CowID,BullID, MatingProcessType,PregnancyStatus,DateOfService,Remarks,Deleted,DoctorID";
                string params_ = "@CowID,@BullID,@MatingProcessType,@PregnancyStatus,@DateOfService,@Remarks,@Deleted,@DoctorID";
                if (conceive.PregnancyStatus == "4" || conceive.PregnancyStatus == "Successful")
                {
                    if (conceive.DeliveryStatus == "1" || conceive.DeliveryStatus == "2") {
                        cols += ",DamWeight,DeliveryStatus,DeliveryDate,AnimalID,LactationNo,BirthWeight,BirthHeight";
                        params_ += ",@DamWeight,@DeliveryStatus,@DeliveryDate,@AnimalID,@LactationNo,@BirthWeight,@BirthHeight";
                    }else if (conceive.DeliveryStatus == "3")
                    {
                        cols += ",DeliveryStatus,DeliveryDate";
                        params_ += ",@DeliveryStatus,@DeliveryDate";
                    }
                }
                string query = $"INSERT into [dbo].[CowConceiveData] ({cols}) OUTPUT INSERTED.ID values({params_});";
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@CowID"].Value = conceive.CowID;
                    sqlcmd.Parameters.Add("@BullID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@BullID"].Value = conceive.BullID;
                    sqlcmd.Parameters.Add("@MatingProcessType", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@MatingProcessType"].Value = conceive.MatingProcessType;
                    sqlcmd.Parameters.Add("@PregnancyStatus", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@PregnancyStatus"].Value = conceive.PregnancyStatus;
                    sqlcmd.Parameters.Add("@DateOfService", System.Data.SqlDbType.DateTime);
                    sqlcmd.Parameters["@DateOfService"].Value = conceive.DateOfService;
                    sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Remarks"].Value = conceive.Remarks;
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = "false";
                    sqlcmd.Parameters.Add("@DoctorID", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@DoctorID"].Value = conceive.DoctorID;
                    if (conceive.PregnancyStatus == "4" || conceive.PregnancyStatus == "Successful")
                    {
                        if (conceive.DeliveryStatus == "1" || conceive.DeliveryStatus == "2")
                        {
                            conceive.animal.TagNo = conceive.TagNo;
                            conceive.animal.Name = conceive.Name;
                            conceive.animal.Gender = conceive.gender;
                            conceive.animal.DOB = conceive.DeliveryDate;
                            if (conceive.gender == "Male")
                            {
                                conceive.animal.Category = "CALF";
                            }
                            else
                            {
                                conceive.animal.Category = "HEIFER";
                            }
                            conceive.animal.Gender = conceive.gender;
                            conceive.animal.Breed = conceive.Breed;
                            conceive.animal.Colour = conceive.Colour;
                            conceive.animal.SireID = conceive.BullID;
                            conceive.animal.DamID = conceive.CowID;
                            //conceive.animal.Status = "1";
                            conceive.animal.BirthLactationNumber = conceive.LactationNo;
                            conceive.animal.Weight = conceive.BirthWeight;
                            conceive.animal.Height = conceive.BirthHeight;
                            conceive.animal.Remarks = conceive.Remarks;
                            conceive.animal.Location = conceive.Location;
                            if (conceive.formFile != null)
                            {
                                Console.Write("FORM FILE SETING");
                                conceive.animal.formFile = conceive.formFile;
                            }
                            sqlcmd.Parameters.Add("@DamWeight", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@DamWeight"].Value = conceive.DamWeight;
                            //,@DeliveryStatus,@DeliveryDate,@AnimalID,@LactationNo,@BirthWeight
                            sqlcmd.Parameters.Add("@DeliveryStatus", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@DeliveryStatus"].Value = conceive.DeliveryStatus;
                            sqlcmd.Parameters.Add("@DeliveryDate", System.Data.SqlDbType.DateTime);
                            sqlcmd.Parameters["@DeliveryDate"].Value = conceive.DeliveryDate;
                            sqlcmd.Parameters.Add("@LactationNo", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@LactationNo"].Value = conceive.LactationNo;
                            sqlcmd.Parameters.Add("@BirthWeight", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@BirthWeight"].Value = conceive.BirthWeight;
                            sqlcmd.Parameters.Add("@BirthHeight", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@BirthHeight"].Value = conceive.BirthHeight;
                            //sqlcmd.Parameters.Add("@BirthHeight", System.Data.SqlDbType.Decimal);
                            //sqlcmd.Parameters["@BirthHeight"].Value = conceive.BirthHeight;
                        }else if (conceive.DeliveryStatus == "3") //child died
                        {
                            sqlcmd.Parameters.Add("@DeliveryStatus", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@DeliveryStatus"].Value = conceive.DeliveryStatus;
                            sqlcmd.Parameters.Add("@DeliveryDate", System.Data.SqlDbType.DateTime);
                            sqlcmd.Parameters["@DeliveryDate"].Value = conceive.DeliveryDate;
                        }
                    }
                    //sqlcmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@category"].Value = cow.Category;
                    SqlTransaction tran = null;
                    try
                    {
                        Dictionary<string, object> calv_Data = new Dictionary<string, object>();
                        conn.Open();
                        tran = conn.BeginTransaction("Service");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        if (conceive.PregnancyStatus == "4" || conceive.PregnancyStatus == "Successful")
                        {
                            if ((conceive.DeliveryStatus == "1" || conceive.DeliveryStatus == "2"))
                            {
                                CalvsContext calvcon = new CalvsContext(_configuration);
                                calv_Data = calvcon.AddCalv(conceive.animal,conn,tran);
                                string message = "";
                                long animalId = 0;
                                string lac_message = "";
                                int newLactaionNumber = 0;
                                CowsContext cows_ = new CowsContext(_configuration);
                                Dictionary<string, object> lac_data;
                                try
                                {
                                    lac_data = cows_.UpdateCowLactationNo((long)conceive.CowID, (int)conceive.LactationNo);
                                }catch(Exception e)
                                {
                                    lac_data = null;
                                }
                                int pregnancy_status_ = 0;
                                bool pregnancy_status = CowsContext.setCowPregnancyStatusById(_configuration, Convert.ToInt64(conceive.CowID), pregnancy_status_, conn, tran);
                                if (calv_Data != null && lac_data!=null && pregnancy_status==true)
                                {
                                    try
                                    {

                                        Console.WriteLine("HELLO");
                                        Dictionary<string, string> dd = (Dictionary<string, string>)calv_Data["data"];
                                        Dictionary<string, string> l_dd = (Dictionary<string, string>)lac_data["data"];
                                        if (Convert.ToString(dd["status"]) == "success" && Convert.ToString(l_dd["status"])=="success")
                                        {
                                            newLactaionNumber = Convert.ToInt32(lac_data["newLactationNumber"]);
                                            message = dd["message"];
                                            lac_message = l_dd["message"];
                                            Console.WriteLine("MEssage:" + message + lac_message);
                                            animalId = Convert.ToInt64(dd["animalID"]);
                                            Console.WriteLine("Animal ID " + animalId);
                                        }
                                        else if (dd["status"] == "failure")
                                        {
                                            message = dd["message"];
                                        }
}
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Exception" + e.Message);
                                    }
                                    if (animalId != 0)
                                    {
                                        sqlcmd.Parameters.Add("@AnimalID", System.Data.SqlDbType.BigInt);
                                        sqlcmd.Parameters["@AnimalID"].Value = animalId;
                                    }
                                    else
                                    {
                                        sqlcmd.Parameters.Add("@AnimalID", System.Data.SqlDbType.BigInt);
                                        sqlcmd.Parameters["@AnimalID"].Value = 0;
                                    }
                                }
                                if (message == "" || pregnancy_status == false)
                                {
                                    tran.Rollback();
                                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                                    data2["message"] = "Cow Service Data Added Failed, Unable to add Calv";
                                    data2["status"] = "failure";
                                    data["data"] = data2;
                                }
                                else
                                {
                                    int i = sqlcmd.ExecuteNonQuery();
                                    //conceive.animal.GenerateImageName(i);
                                    conceive.animal.SaveImage2();
                                    if(conceive.animal.SaveImage2()==true){
                                        tran.Commit();
                                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                                        data2["message"] = $"Cow Service {i} Data Added Successfully" + message;
                                        data2["status"] = "success";
                                        data["NewLactaionNumber"] = conceive.LactationNo;
                                        data["PregnancyStatus"] = pregnancy_status_;
                                        data["data"] = data2;
                                    }else{
                                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                                        data2["message"] = $"Cow Service Data Addition Failed Image error" + message;
                                        data2["status"] = "failure";
                                        data["data"] = data2;
                                    }
                                }
                            }
                            else if (conceive.DeliveryStatus == "3") //CHild died while birth
                            {
                                int pregnancy_status_ = 0;
                                bool pregnancy_status = CowsContext.setCowPregnancyStatusById(_configuration, Convert.ToInt64(conceive.CowID), pregnancy_status_, conn, tran);
                                int i = sqlcmd.ExecuteNonQuery();
                                if (pregnancy_status == true && i > 1) { 
                                    tran.Commit();
                                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                                    data2["message"] = $"Cow Service {i} Data Added Successfully.Child Died";
                                    data2["status"] = "success";
                                    data["PregnancyStatus"] = pregnancy_status_;
                                    //data["NewLactationNumber"] = pregnancy_status_;
                                    data["data"] = data2;
                                }
                                else
                                {
                                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                                    data2["message"] = $"Cow Service {i} Data Updation Failed.";
                                    data2["status"] = "failure";
                                    data["data"] = data2;
                                }
                            }
                            else
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Cow Service Data Addition Failed. Invalid Delivery Status";
                                data2["status"] = "failure";
                                data["data"] = data2;
                            }
                        }
                        else
                        {
                            //{1:'Confirmed',2:'Pending',3:'Failed'
                            int pregnancy_status_ = 0;
                            if (conceive.PregnancyStatus == "1")
                            {
                                pregnancy_status_ = 1;
                            }else if (conceive.PregnancyStatus == "2" || conceive.PregnancyStatus == "3")
                            {
                                pregnancy_status_ = 0;
                            }
                            
                            bool pregnancy_status = CowsContext.setCowPregnancyStatusById(_configuration, Convert.ToInt64(conceive.CowID), pregnancy_status_, conn, tran);
                            int i = sqlcmd.ExecuteNonQuery();
                            if (i > 0 && pregnancy_status == true)
                            {
                                tran.Commit();
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Cow Service Data Added Successfully";
                                data2["status"] = "success";
                                data["data"] = data2;
                                data["PregnancyStatus"] = pregnancy_status_;
                            }
                            else
                            {
                                tran.Rollback();
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Cow Service Data Addition Failed";
                                data2["status"] = "failure";
                                data["data"] = data2;
                            }
                        }
                    }catch(Exception e)
                    {
                        Console.WriteLine("Exception : 567 "+e.Message + e.StackTrace);
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Cow Service Data Addition Failed"+e.Message;
                        data2["status"] = "failure";
                        data["data"] = data2;
                    }
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Adding Service Detail Failed! Invaid Data Enterred";
                data2["status"] = "failure";
                data["data"] = data2;
                data["errors"] = conceive.errors;
            }
            return data;
        }

        internal static bool DoLactationNumberExists(IConfiguration _configuration, long cow_id, int lactation_Number,long? id=null)
        {
            if (cow_id != null && lactation_Number != null)
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    string query = $"Select count(*) as total from CowConceiveData where CowID = @CowID and LactationNo = @LactationNo";
                    if (id != null)
                    {
                        query += " and Id != @ID";
                    }
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = query;
                    try
                    {
                        sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@CowID"].Value = cow_id;
                        sqlcmd.Parameters.Add("@LactationNo", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@LactationNo"].Value = lactation_Number;
                        if (id != null)
                        {
                            sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters["@ID"].Value = id;
                        }
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        if (sqlrdr.Read())
                        {
                            int total = Convert.ToInt32(sqlrdr["total"]);
                            sqlrdr.Close();
                            conn.Close();
                            if (total <= 0)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        internal Dictionary<string, object> GetSmmaryByCowId(long id)
        {
            return new Dictionary<string, object>();
            
        }

        internal Dictionary<string, object> UpdateCowServiceData(CowServiceDataModel conceive)
        {
            Console.WriteLine("Updating");
            Dictionary<string, object> data = new Dictionary<string, object>();
            CowServiceDataModel._configuration = this._configuration;
            long AnimalID = this.GetAnimalIDServiceDetailById(Convert.ToInt64(conceive.Id));
            if (AnimalID != 0)
            {
                conceive.AnimalID = AnimalID;
            }
            if (conceive.ValidateEdit() == true)
            {
                string cols = "BullID = @BullID, MatingProcessType = @MatingProcessType,PregnancyStatus = @PregnancyStatus,DateOfService = @DateOfService,Remarks =@Remarks," +
                    "DoctorID = @DoctorID";
                if (conceive.PregnancyStatus == "4" || conceive.PregnancyStatus == "Successful")
                {
                    if (conceive.DeliveryStatus == "1" || conceive.DeliveryStatus == "2")
                    {
                        cols += ",DamWeight =@DamWeight,DeliveryStatus = @DeliveryStatus,DeliveryDate= @DeliveryDate,AnimalID = @AnimalID,LactationNo = @LactationNo," +
                            "BirthWeight = @BirthWeight,BirthHeight = @BirthHeight";
                    }
                    else if (conceive.DeliveryStatus == "3")
                    {
                        cols += ",DeliveryStatus = @DeliveryStatus,DeliveryDate = @DeliveryDate";
                    }
                }
                string query = $"Update CowConceiveData set {cols} where Id = @Id" ;
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    Dictionary<string, object> calv_Data = new Dictionary<string, object>();
                    SqlCommand sqlcmd = new SqlCommand(query, conn);

                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = conceive.Id;
                    sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@CowID"].Value = conceive.CowID;
                    sqlcmd.Parameters.Add("@BullID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@BullID"].Value = conceive.BullID;
                    sqlcmd.Parameters.Add("@MatingProcessType", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@MatingProcessType"].Value = conceive.MatingProcessType;
                    sqlcmd.Parameters.Add("@PregnancyStatus", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@PregnancyStatus"].Value = conceive.PregnancyStatus;
                    Console.WriteLine("\n\nPregnancy Status  = " + conceive.PregnancyStatus);
                    sqlcmd.Parameters.Add("@DateOfService", System.Data.SqlDbType.DateTime);
                    sqlcmd.Parameters["@DateOfService"].Value = conceive.DateOfService;
                    sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Remarks"].Value = conceive.Remarks;
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = "false";
                    sqlcmd.Parameters.Add("@DoctorID", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@DoctorID"].Value = conceive.DoctorID;
                    //sqlcmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@category"].Value = cow.Category;
                    int pregnancy_status_ = 0;
                    SqlTransaction tran = null;
                    if (conceive.PregnancyStatus == "4" || conceive.PregnancyStatus == "Successful")
                    {
                        if (conceive.DeliveryStatus == "1" || conceive.DeliveryStatus == "2")
                        {
                            if (AnimalID != 0)
                            {
                                conceive.animal.Id = AnimalID;
                            }
                            conceive.animal.TagNo = conceive.TagNo;
                            conceive.animal.Name = conceive.Name;
                            conceive.animal.Gender = conceive.gender;
                            conceive.animal.DOB = conceive.DeliveryDate;
                            if (conceive.gender == "Male")
                            {
                                conceive.animal.Category = "CALF";
                            }
                            else
                            {
                                conceive.animal.Category = "HEIFER";
                            }
                            conceive.animal.Gender = conceive.gender;
                            conceive.animal.Breed = conceive.Breed;
                            conceive.animal.Colour = conceive.Colour;
                            conceive.animal.SireID = conceive.BullID;
                            conceive.animal.DamID = conceive.CowID;
                            //conceive.animal.Status = "1";
                            conceive.animal.BirthLactationNumber = conceive.LactationNo;
                            conceive.animal.Weight = conceive.BirthWeight;
                            conceive.animal.Height = conceive.BirthHeight;
                            conceive.animal.Remarks = conceive.Remarks;
                            conceive.animal.Location = conceive.Location;
                            if (conceive.formFile != null)
                            {
                                conceive.animal.formFile = conceive.formFile;
                            }
                            sqlcmd.Parameters.Add("@DamWeight", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@DamWeight"].Value = conceive.DamWeight;
                            //,@DeliveryStatus,@DeliveryDate,@AnimalID,@LactationNo,@BirthWeight
                            sqlcmd.Parameters.Add("@DeliveryStatus", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@DeliveryStatus"].Value = conceive.DeliveryStatus;
                            sqlcmd.Parameters.Add("@DeliveryDate", System.Data.SqlDbType.DateTime);
                            sqlcmd.Parameters["@DeliveryDate"].Value = conceive.DeliveryDate;
                            sqlcmd.Parameters.Add("@LactationNo", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@LactationNo"].Value = conceive.LactationNo;
                            sqlcmd.Parameters.Add("@BirthWeight", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@BirthWeight"].Value = conceive.BirthWeight;
                            sqlcmd.Parameters.Add("@BirthHeight", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@BirthHeight"].Value = conceive.BirthHeight;
                            //sqlcmd.Parameters.Add("@BirthHeight", System.Data.SqlDbType.Decimal);
                            CalvsContext calvcon = new CalvsContext(_configuration);
                            
                            conn.Open();
                            tran = conn.BeginTransaction("Service");
                            tran.Save("save1");
                            sqlcmd.Transaction = tran;
                            string message = "";
                            string status = "";
                            if (AnimalID == 0)
                            {
                                calv_Data = calvcon.AddCalv(conceive.animal, conn, tran);
                            }
                            else
                            {
                                calv_Data = calvcon.UpdateCalv(conceive.animal, conn, tran);
                            }
                            CowsContext cows_ = new CowsContext(_configuration);
                            Dictionary<string, object> lac_data;
                            try
                            {
                                lac_data = cows_.UpdateCowLactationNo((long)conceive.CowID, (int)conceive.LactationNo,conn,tran);
                            }
                            catch (Exception e)
                            {
                                lac_data = null;
                            }
                            if (calv_Data != null && lac_data != null)
                            {
                                int newLactaionNumber = 0;
                                try
                                {
                                    newLactaionNumber = Convert.ToInt32(lac_data["newLactationNumber"]);
                                    if(newLactaionNumber==0){
                                        newLactaionNumber =1;
                                    }
                                }
                                catch(Exception e)
                                {
                                    newLactaionNumber = 1;
                                }
                                try
                                {
                                    Console.WriteLine("HELLO");
                                    Dictionary<string, string> dd = (Dictionary<string, string>)calv_Data["data"];
                                    Dictionary<string, string> ld = (Dictionary<string, string>)lac_data["data"];
                                    if (Convert.ToString(dd["status"]) == "success" && Convert.ToString(ld["status"]) == "success")
                                    {
                                        message = dd["message"];
                                        status = dd["status"];
                                        Console.WriteLine("MEssage:" + message);
                                        AnimalID = Convert.ToInt64(dd["animalID"]);
                                        Console.WriteLine("Animal ID " + AnimalID);
                                    }
                                    else if (dd["status"] == "failure" || ld["status"] == "failure")
                                    {
                                        status = "failure";
                                        message = dd["message"]+ " " + ld["message"];
                                    }
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Exception" + e.Message+e.StackTrace);
                                }
                                if (AnimalID != 0)
                                {
                                    sqlcmd.Parameters.Add("@AnimalID", System.Data.SqlDbType.BigInt);
                                    sqlcmd.Parameters["@AnimalID"].Value = AnimalID;
                                }
                                else
                                {
                                    sqlcmd.Parameters.Add("@AnimalID", System.Data.SqlDbType.BigInt);
                                    sqlcmd.Parameters["@AnimalID"].Value = 0;
                                }
                                if (status == "success" && AnimalID!=0 && message!="") {
                                    try
                                    {
                                        pregnancy_status_ = 0;
                                        bool pregnancy_status = CowsContext.setCowPregnancyStatusById(_configuration, Convert.ToInt64(conceive.CowID), pregnancy_status_, conn, tran);
                                        int i = sqlcmd.ExecuteNonQuery();
                                        Console.WriteLine(i);
                                        Console.WriteLine(pregnancy_status);
                                        Console.WriteLine(newLactaionNumber);
                                        if (i > 0 && pregnancy_status==true && newLactaionNumber!=0)
                                        {
                                            Console.WriteLine("Commiting");
                                            tran.Commit();
                                            conceive.animal.SaveImage2();
                                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                                            data2["message"] = "Cow Service Data Updated Successfully. "+message;
                                            data2["status"] = "success";
                                            data["PregnancyStatus"] = pregnancy_status_;
                                            data["NewLactationNumber"] = conceive.LactationNo;
                                            data["data"] = data2;
                                        }
                                        else
                                        {
                                            Console.WriteLine("Rolling Back");
                                            tran.Rollback();
                                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                                            data2["message"] = "Cow Updation Failed";
                                            data2["status"] = "failure";
                                            data["data"] = data2;
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine("Exception : 123 " + e.Message + e.StackTrace+ ". Rolling Back!");
                                        tran.Rollback();
                                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                                        data2["message"] = "Cow Updation Failed. "+e.Message;
                                        data2["status"] = "failure";
                                        data["data"] = data2;
                                    }
                                }
                                else
                                {
                                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                                    data2["message"] = "Cow Service Data UPdation Failed . " + message;
                                    data2["status"] = "failure";
                                    data["data"] = data2;
                                }
                                return data;
                            }
                            
                        }
                        else if (conceive.DeliveryStatus == "3") //child died
                        {
                            pregnancy_status_ = 0;
                            sqlcmd.Parameters.Add("@DeliveryStatus", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@DeliveryStatus"].Value = conceive.DeliveryStatus;
                            sqlcmd.Parameters.Add("@DeliveryDate", System.Data.SqlDbType.DateTime);
                            sqlcmd.Parameters["@DeliveryDate"].Value = conceive.DeliveryDate;
                        }
                    }
                    try
                    {
                        if (conceive.PregnancyStatus == "2" || conceive.PregnancyStatus == "3")
                        {
                            pregnancy_status_ = 0;
                        }
                        else if (conceive.PregnancyStatus == "1") {
                            pregnancy_status_ = 1;
                        }
                        if (!((conceive.PregnancyStatus == "4" || conceive.PregnancyStatus == "Successful") &&
                        (conceive.DeliveryStatus == "1" || conceive.DeliveryStatus == "2")))
                        {
                            conn.Open();
                            tran = conn.BeginTransaction("Service");
                        }
                        tran.Save("save2");
                        sqlcmd.Transaction = tran;
                        bool pregnancy_status = CowsContext.setCowPregnancyStatusById(_configuration, Convert.ToInt64(conceive.CowID), pregnancy_status_, conn, tran);
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0 && pregnancy_status ==true)
                        {
                            tran.Commit();
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Service Data Updated Successfully";
                            data2["status"] = "success";
                            data["PregnancyStatus"] = pregnancy_status_;
                            data["data"] = data2;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Updation Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : 235 " + e.Message + e.StackTrace);
                    }
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Updating Service Detail Failed! Invaid Data Enterred";
                data2["status"] = "failure";
                data["errors"] = conceive.errors;
                data["data"] = data2;
            }
            return data;
        }
        public Dictionary<string, object> GetServiceDetailByCowId(long id)
        {
            bool FetchServiceDetail = true;
            Dictionary<string, object> data = new Dictionary<string, object>();
            ServiceDetail serviceDetail =null;
            if (FetchServiceDetail == true) { 
                serviceDetail = new ServiceDetail();
            }
            if (id != null)
            {
                ArrayList cowConcieveData = new ArrayList();
                Dictionary<int, Dictionary<string, object>> conceive_data = new Dictionary<int, Dictionary<string, object>>();
                List<long> bull_ids = new List<long>();
                List<long> animal_ids = new List<long>();
                List<long> doctor_ids = new List<long>();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    SqlCommand sqlcmd = new SqlCommand("Select * from CowConceiveData where CowID = @CowID ", conn);
                    try
                    {
                        sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@CowID"].Value = id;

                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        long counter = 0;
                        int sno = 1;
                        long bull_Id = 0;
                        long doctor_Id = 0;
                        //string cow_name = this.GetCowTagNameById(id);
                        
                        while (sqlrdr.Read())
                        {
                            bull_Id = 0;
                            if (FetchServiceDetail == true)
                            {
                                switch (sqlrdr["PregnancyStatus"].ToString())
                                {
                                    case "1":
                                        serviceDetail.Confirmed++;
                                        break;
                                    case "2":
                                        serviceDetail.Pending++;
                                        break;
                                    case "3":
                                        serviceDetail.Failed++;
                                        break;
                                    case "4":
                                        //serviceDetail.Successful++;
                                        string delivery_status = sqlrdr["DeliveryStatus"].ToString();
                                        if (delivery_status == "1" || delivery_status == "2")
                                        {
                                            serviceDetail.Successful++;
                                            animal_ids.Add(Convert.ToInt64(sqlrdr["AnimalID"]));
                                        }
                                        else
                                        {
                                            serviceDetail.Died++;
                                        }
                                        break;
                                }
                                serviceDetail.Total++;
                            }
                            Dictionary<string, object> temp = new Dictionary<string, object>();
                            temp["id"] = sqlrdr["Id"];
                            temp["sno"] = sno;
                            sno++;
                            temp["cowID"] = sqlrdr["CowID"];
                            temp["bullID"] = sqlrdr["BullID"];
                            temp["matingProcessType"] = sqlrdr["MatingProcessType"];
                            temp["pregnancyStatus"] = sqlrdr["PregnancyStatus"];
                            temp["dateOfService"] = Helper.FormatDate(sqlrdr["DateOfService"]);
                            temp["time"] = sqlrdr["DateOfService"];
                            temp["remarks"] = sqlrdr["Remarks"].ToString();
                            temp["bullsName"] = sqlrdr["BullSemenNo"];
                            Dictionary<string, string> cow_data  = CowsContext.GetCowNameTagNoById(_configuration, Convert.ToInt64(sqlrdr["CowID"]));
                            temp["cowName"] = cow_data["tagNo"]+"/"+cow_data["name"];
                            temp["doctorsName"] = "";
                            try
                            {
                                temp["doctorID"] = sqlrdr["DoctorID"];
                            }catch(Exception e)
                            {
                                //temp["doctorId"] = null;
                            }
                            try { bull_Id = Convert.ToInt64(sqlrdr["BullID"]); if (bull_Id != 0) { bull_ids.Add(bull_Id); } } catch(Exception e) { Console.WriteLine("Bull not found"); }
                            try { doctor_Id = Convert.ToInt64(sqlrdr["DoctorID"]); if (doctor_Id != 0) { doctor_ids.Add(doctor_Id); } } catch(Exception e) { Console.WriteLine("doctor not found"); }
                            
                            string category = Convert.ToString(sqlrdr.GetValue(4));
                            //CowConceiveDataModel conceive = new CowConceiveDataModel(sqlrdr);
                            conceive_data[Convert.ToInt32(temp["id"])] = temp;
                            cowConcieveData.Add(temp);
                            temp = null;
                        }
                        if(FetchServiceDetail == true)
                        {
                            //get the genders of animals
                            Dictionary<long,string> genders = this.GetIdGenderByAnimalIds(animal_ids);
                            foreach(var m in genders)
                            {
                                switch (m.Value.ToString())
                                {
                                    case "Male":
                                        serviceDetail.Male++;
                                        break;
                                    case "Female":
                                        serviceDetail.Female++;
                                        break;
                                }
                            }
                            data["serviceDetail"] = serviceDetail;
                        }
                        sqlrdr.Close();
                        conn.Close();
                        //fetch all the bulls
                        if (bull_ids.Count > 0)
                        {
                            Dictionary<long, Dictionary<string, string>> bullsIdName = BullsContext.GetBullsTagNoNamesByIds(_configuration,bull_ids);
                            foreach(var m in cowConcieveData)
                            {
                                Dictionary<string, object> a = (Dictionary<string, object>)m;
                                try
                                {
                                    a["bullsName"] = bullsIdName[Convert.ToInt64(a["bullID"])]["TagNo"]+"/"+bullsIdName[Convert.ToInt64(a["bullID"])]["Name"];
                                }catch(Exception e)
                                {
                                    Console.WriteLine("Bull name not found " + e.Message);
                                    a["bullsName"] = "";
                                }
                            }
                        }
                        if (doctor_ids.Count > 0)
                        {
                            UsersContext users = new UsersContext(this._configuration);
                            Dictionary<long, string> doctorsIdName = users.GetDoctorsIdNameByIds(doctor_ids);
                            foreach(var m in cowConcieveData)
                            {
                                Dictionary<string, object> a = (Dictionary<string, object>)m;
                                
                                try
                                {
                                    a["doctorsName"] = doctorsIdName[Convert.ToInt64(a["doctorID"])];
                                }catch(Exception e)
                                {
                                    Console.WriteLine("doctor name not found " + e.Message);
                                    //a["doctorsName"] = "";
                                }
                            }
                        }
                        data["status"] = "success";
                        data["message"] = "Service Detail Found";
                        data["data"] = cowConcieveData;
                        data["recordsTotal"] = this.GetTotalServiceDetailByCowId(id);
                        data["recordsFiltered"] = this.GetTotalServiceDetailByCowId(id);
                    }
                    catch (Exception ex)
                    {
                        //return false;
                        data["status"] = "failure";
                        data["message"] = "Connection failure" + ex.ToString();
                        data["data"] = cowConcieveData;
                        data["recordsTotal"] = 0;
                        data["recordsFiltered"] = 0;
                    }
                }
                //return false;
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Provide the ID of Cow";
            }
            return data;
        }

        private Dictionary<long, string> GetIdGenderByAnimalIds(List<long> animal_ids)
        {
            Dictionary<long, string> data = new Dictionary<long, string>();
            if(animal_ids!=null && animal_ids.Count > 0)
            {
                string ids = "";
                foreach(long m in animal_ids)
                {
                    if (ids != "")
                    {
                        ids += ",";
                    }
                    ids += m.ToString();
                }
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = $"Select Id,Gender from Animals where Id in ({ids})";
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    try
                    {
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        while (sqlrdr.Read())
                        {
                            data[Convert.ToInt64(sqlrdr["Id"])] = Convert.ToString(sqlrdr["Gender"]);
                        }
                        sqlrdr.Close();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace);
                    }
                }
            }
            return data;
        }

        public long GetAnimalIDServiceDetailById(long id)
        {
            long AnimalID = 0;
            if (id != null)
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand("Select AnimalID from CowConceiveData where Id = @Id", conn);
                    try
                    {
                        sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@Id"].Value = id;
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        if (sqlrdr.Read())
                        {
                            AnimalID = Convert.ToInt64(sqlrdr["AnimalID"]);
                        }
                        sqlrdr.Close();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return AnimalID ;
        }
        public Dictionary<string, object> GetServiceDetailById(long id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (id != null)
            {
                List<long> bull_ids = new List<long>();
                List<long> cow_ids = new List<long>();
                List<long> doctor_ids = new List<long>();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                Dictionary<string, object> temp = new Dictionary<string, object>();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                   
                    SqlCommand sqlcmd = new SqlCommand("Select * from CowConceiveData where Id = @Id", conn);
                    try
                    {
                        sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@Id"].Value = id;
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        long bull_Id = 0;
                        long doctor_Id = 0;
                        long cow_Id = 0;
                        
                        if (sqlrdr.Read())
                        {
                            temp["id"] = sqlrdr["Id"];
                            temp["cowID"] = Helper.FormatLong(sqlrdr["CowID"]);
                            temp["bullID"] = Helper.FormatLong(sqlrdr["BullID"]);
                            temp["matingProcessType"] = Helper.IsNullOrEmpty(sqlrdr["MatingProcessType"]);
                            temp["pregnancyStatus"] = Helper.IsNullOrEmpty(sqlrdr["PregnancyStatus"]);
                            temp["deliveryStatus"] = Helper.IsNullOrEmpty(sqlrdr["DeliveryStatus"]);
                            temp["deliveryDate"] = Helper.FormatDate3(sqlrdr["DeliveryDate"]);
                            temp["lactationNo"] = Helper.IsNullOrEmpty(sqlrdr["LactationNo"]);
                            temp["dateOfService"] = Helper.FormatDate3(sqlrdr["DateOfService"]);
                            temp["birthWeight"] = Helper.IsNullOrEmpty(sqlrdr["BirthWeight"]);
                            temp["birthHeight"] = Helper.IsNullOrEmpty(sqlrdr["BirthHeight"]);
                            temp["damWeight"] = Helper.IsNullOrEmpty(sqlrdr["DamWeight"]);
                            temp["time"] = Helper.IsNullOrEmpty(sqlrdr["DateOfService"]);
                            temp["remarks"] = Helper.IsNullOrEmpty(sqlrdr["Remarks"].ToString());
                            temp["bullsName"] = Helper.IsNullOrEmpty(sqlrdr["BullSemenNo"]);
                            temp["bullsTagNo"] = "";
                            temp["cowName"] = "";
                            temp["doctorsName"] = "";
                            long? animalId;
                            try
                            {
                                animalId = Convert.ToInt64(sqlrdr["AnimalID"]);
                            }catch(Exception e)
                            {
                                animalId = null;
                            }
                            temp["animalId"] = sqlrdr["AnimalId"];
                            try
                            {
                                temp["doctorID"] = Helper.FormatLong(sqlrdr["DoctorID"]);
                            }catch(Exception e)
                            {
                                //temp["doctorId"] = null;
                            }
                            if(animalId!=null && animalId != 0)
                            {
                                Dictionary<string, object>  ani_detail = CowsContext.GetDetailById(_configuration, (long)animalId);
                                //temp["id"] = ani_detail["Id"];
                                temp["name"] = ani_detail["name"].ToString();
                                temp["tagNo"] = ani_detail["tagNo"].ToString();
                                temp["dob"] = Helper.IsNullOrEmpty(ani_detail["dob"]);
                                temp["category"] = Helper.IsNullOrEmpty(ani_detail["category"]);
                                temp["gender"] = Helper.IsNullOrEmpty(ani_detail["gender"]);
                                //temp["sireID"] = ani_detail["SireID"];
                                //temp["damID"] = ani_detail["DamID"];
                                //temp["butterFat"] = ani_detail["ButterFat"];
                                //temp["pregnancyStatus"] = ani_detail["PregnancyStatus"];
                                //temp["status"] = ani_detail["Status"];
                               // temp["reporductiveStatus"] = ani_detail["ReporductiveStatus"];
                                //temp["milkingStatus"] = ani_detail["MilkingStatus"];
                                temp["remarks"] = Helper.IsNullOrEmpty(ani_detail["remarks"]);
                                //temp["additionalInfo"] = ani_detail["AdditionalInfo"];
                                temp["picture"] = Helper.IsNullOrEmpty(ani_detail["picture"]);
                                temp["lactation"] = Helper.IsNullOrEmpty(ani_detail["lactation"]);
                                //temp["type"] = ani_detail["Type"];
                                //temp["semenDoses"] = ani_detail["SemenDoses"];
                                temp["weight"] = Helper.IsNullOrEmpty(ani_detail["weight"]);
                                //temp["weight"] = ani_detail["weight"];
                                temp["alive"] = Helper.IsNullOrEmpty(ani_detail["alive"]);
                                temp["birthLactationNumber"] = Helper.IsNullOrEmpty(ani_detail["birthLactationNumber"]);
                                temp["height"] = Helper.IsNullOrEmpty(ani_detail["height"]);
                                temp["dateOfDeath"] = Helper.IsNullOrEmpty(ani_detail["dateOfDeath"]);
                                temp["colour"] = Helper.IsNullOrEmpty(ani_detail["colour"]);
                                temp["breed"] = Helper.IsNullOrEmpty(ani_detail["breed"]);
                                temp["location"] = Helper.IsNullOrEmpty(ani_detail["location"]);
                            }
                            try { bull_Id = Convert.ToInt64(sqlrdr["BullID"]); if (bull_Id != 0) { bull_ids.Add(bull_Id); } } catch(Exception e) { Console.WriteLine("Bull not found"); }
                            try { cow_Id = Convert.ToInt64(sqlrdr["CowID"]); if (cow_Id != 0) { cow_ids.Add(cow_Id); } } catch(Exception e) { Console.WriteLine("cow not found"); }
                            try { doctor_Id = Convert.ToInt64(sqlrdr["DoctorID"]); if (doctor_Id != 0) { doctor_ids.Add(doctor_Id); } } catch(Exception e) { Console.WriteLine("doctor not found"); }
                            
                            string category = Convert.ToString(sqlrdr.GetValue(4));
                            //CowConceiveDataModel conceive = new CowConceiveDataModel(sqlrdr);
                            //conceive_data[Convert.ToInt32(temp["id"])] = temp;
                            //cowConcieveData.Add(temp);
                           
                        }
                        sqlrdr.Close();
                        conn.Close();
                        //fetch all the bulls
                        if (bull_ids.Count > 0)
                        {
                            //BullsContext bulls = new BullsContext(this._configuration);
                            //Dictionary<long, string> bullsIdName = bulls.GetBullsNamesByIds(bull_ids);
                            Dictionary<long, Dictionary<string, string>> bullsIdNameTagNo = BullsContext.GetBullsTagNoNamesByIds(this._configuration,bull_ids);
                            
                            try
                            {
                                temp["bullsName"] = bullsIdNameTagNo[Convert.ToInt64(temp["bullID"])]["Name"];
                                temp["bullsTagNo"] = bullsIdNameTagNo[Convert.ToInt64(temp["bullID"])]["TagNo"];
                            }catch(Exception e)
                            {
                                temp["bullsName"] = "";
                                temp["bullsTagNo"] = "";
                            }
                        }if (cow_ids.Count > 0)
                        {
                            Dictionary<long, string> cowsIdName = CowsContext.GetCowsNamesByIds(_configuration, cow_ids);
                            try
                            {
                                temp["cowName"] = cowsIdName[Convert.ToInt64(temp["cowID"])];
                            }catch(Exception e)
                            {
                                temp["cowName"] = "";
                            }
                        }
                        if (doctor_ids.Count > 0)
                        {
                            UsersContext users = new UsersContext(this._configuration);
                            Dictionary<long, string> doctorsIdName = users.GetDoctorsIdNameByIds(doctor_ids);
                            try
                            {
                                temp["doctorsName"] = doctorsIdName[Convert.ToInt64(temp["doctorID"])];
                            }catch(Exception e)
                            {
                                temp["doctorsName"] = "";
                            }
                        }
                        data["status"] = "success";
                        data["message"] = "Service Detail Found";
                        data["data"] = temp;
                        //data["recordsTotal"] = this.GetTotalServiceDetailByCowId(id);
                        //data["recordsFiltered"] = this.GetTotalServiceDetailByCowId(id);
                    }
                    catch (Exception ex)
                    {
                        //return false;
                        data["status"] = "failure";
                        data["message"] = "Connection failure" + ex.ToString();
                        data["data"] = temp;
                        
                        
                    }
                }
                //return false;
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Provide the ID of Cow";
            }
            return data;
        }
        public long GetTotalServiceDetailByCowId(long? id)
        {
            long counter=0;
            if (id != null)
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    SqlCommand sqlcmd = new SqlCommand("Select count(*) as total from CowConceiveData where CowID = @CowID", conn);
                    try
                    {
                        sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@CowID"].Value = id;
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        while (sqlrdr.Read())
                        {
                            counter = Convert.ToInt64(sqlrdr["total"]);
                        }
                        sqlrdr.Close();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        return counter;
                    }
                }
            }
            return counter;
        }
       
       
    }
}
