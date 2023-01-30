using GaushalaAPI.Models;
using GaushalAPI.Entities;
using GaushalAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class CowsContext : AnimalContext
    {
        //private readonly IConfiguration _configuration;
        public CowsContext(IConfiguration configuration) :base(configuration)
        {
            //base._configuration = configuration;
        }
        public IEnumerable<Dictionary<string,object>> GetCows(CowFilterModel cowFilter)
        {
            int length = 10;
            int start = 0;
            string tagNo = null;
            string name = null;
            if (cowFilter != null)
            {
                try{
                    length = (int)cowFilter.length;
                    start = (int)cowFilter.start;
                }catch(Exception e)
                {

                }
                CowModel cow_ = cowFilter.cow;
                if (cow_.Name != null && cow_.Name.Trim() != "")
                {
                    name = "%" + cow_.Name + "%";
                }
                if (cow_.TagNo != null && cow_.TagNo.Trim() != "")
                {
                    tagNo = "%" + cow_.TagNo + "%";
                }
            }
            //List<CowModel> cowsList = new List<CowModel>();
            List<Dictionary<string,object>> cowsList_ = new List<Dictionary<string,object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string where = "";
                if (name != null)
                {
                    where = $" and Name like @Name ";
                }
                if (tagNo != null)
                {
                    where += $" and  TagNo like @TagNo";
                }
                string query = $"Select * from Animals where Category = 'COW' {where} order by TagNo  OFFSET {start} ROWS FETCH NEXT {length} ROWS ONLY";
                Console.Write(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                //SqlCommand sqlcmd = new SqlCommand("Update [dbo].[Animals] set [TagNo] = @tagNo,[Name] = @name,[Category] = @Category where [Animals].[Id] = @ID", conn);
                if (name != null)
                {
                    sqlcmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Name"].Value = name;
                }
                if (tagNo != null)
                {
                    sqlcmd.Parameters.Add("@TagNo", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@TagNo"].Value = tagNo;
                }
                //SqlCommand sqlcmd = new SqlCommand($"Select  * from Animals where Category = 'COW' order by TagNo  OFFSET {start} ROWS FETCH NEXT {length} ROWS ONLY", conn);
                try
                {
                    //Console.WriteLine("HI");
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = start;
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> bull = new Dictionary<string, object>();
                        bull["id"] = sqlrdr["Id"];
                        bull["sno"] = counter;
                        bull["tagNo"] = sqlrdr["TagNo"].ToString();
                        bull["name"] = sqlrdr["Name"].ToString();
                        bull["dob"] = Helper.FormatDate(sqlrdr["DOB"]) ;
                        bull["breed"] = sqlrdr["Breed"].ToString();
                        bull["weight"] = sqlrdr["Weight"].ToString();
                        //bull["colour"] = sqlrdr["Colour"].ToString();
                        bull["colour"] = sqlrdr["Colour"].ToString();
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

                        //cowsList.Add(BullModel);
                        cowsList_.Add(bull);
                        counter++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return cowsList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return cowsList_;
                }
                
            }

        }

        internal int GetTotalFilteredCows(CowFilterModel cowFilters)
        {
            if (cowFilters != null)
            {
                CowModel cow_ = cowFilters.cow;
            }
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                System.Console.WriteLine("HELLO");
                SqlCommand sqlcmd = new SqlCommand($"Select  count(*) as total from Animals where Category = 'COW'", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        return Convert.ToInt32(sqlrdr["total"]);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return 0;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return 0;
        }

        internal object GetTotalCows()
        {
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand($"Select  count(*) as total from Animals where Category = 'COW'", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        return Convert.ToInt32(sqlrdr["total"]);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return 0;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return 0;
        }
        internal Dictionary<string,Dictionary<string,string>> SaveCow(CowModel cow)
        {
            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
            Console.WriteLine(cow.Id);
            if (cow.ValidateCow(this,"Add") == true)
            {
                string image = null;
                if (cow.formFile != null)
                {
                    Console.WriteLine("Image Fetched");
                    cow.Picture = cow.SaveImage();
                    if (cow.Picture == "")
                    {
                        ///Failed
                    }
                    Console.WriteLine(cow.Picture);
                }
                //validate data over here
                Console.WriteLine($"ID={cow.Id};");
                if (cow.Id == null || cow.Id == 0)
                {
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Cow Save Failed";
                    data2["status"] = "Failure";
                    data["data"] = data2;
                }
                else
                {
                    //cow.DOB = null;
                    //cow.Weight = null;
                    //Console.WriteLine(cow.Weight);
                    //cow.Height = null;
                    //cow.ButterFat = null;
                    //cow.Lactation = null;
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        System.Console.WriteLine("HELLO");
                        //
                        /*SqlCommand sqlcmd = new SqlCommand("INSERT INTO[dbo].[Animals] ([TagNo],[Name],[DOB],[Category],[Gender],[Breed],[Colour],[SireID],[DamID],[SireNo]"+
                   ",[SireName],[DamNo],[DamName],[DBLY],[SDBLY],[ButterFat],[PregnancyStatus],[Status],[ReproductiveStatus],[MilkingStatus],[Remarks],[AdditionalInfo],[Picture]"+
                   ",[Lactation],[Type],[SemenDoses],[Weight],[Alive],[BirthLactationNumber],[Height])VALUES"+
                   "(", conn);*/
                        string query = "Update [dbo].[Animals] set [TagNo] = @tagNo,[Name] = @name";
                        if (cow.Breed != null)
                        {
                            query += ",[Breed] = @breed";
                        }
                        if (cow.Lactation != null)
                        {
                            query += ",[Lactation] = @lactation";
                        }
                        if (cow.DOB != null)
                        {
                            query += ",[DOB] = @dob";
                        }
                        if (cow.DamID != null)
                        {
                            query += ",[DamID] = @damID";
                        }
                        if (cow.SireID != null)
                        {
                            query += ",[SireID] = @sireID";
                        }
                        if (cow.Colour != null)
                        {
                            query += ",[Colour] = @colour";
                        }
                        if (cow.Weight != null)
                        {
                            query += ",[Weight] = @weight";
                        }
                        if (cow.Height != null)
                        {
                            query += ",[Height] = @height";
                        }
                        if (cow.ButterFat != null)
                        {
                            query += ",[ButterFat] = @butterFat";
                        }
                        if (cow.NoOfTeatsWorking != null)
                        {
                            query += ",[NoOfTeatsWorking] = @noOfTeatsWorking";
                        }
                        if (cow.Location != null)
                        {
                            query += ",[Location] = @location";
                        }
                        if (cow.Remarks != null)
                        {
                            query += ",[Remarks] = @remarks";
                        }
                        if (cow.Picture != null && cow.Picture != "")
                        {
                            query += ",[Picture] = @picture";
                        }
                        query += " where [Animals].[Id] = @Id";
                        Console.WriteLine(query);
                        SqlCommand sqlcmd = new SqlCommand(query, conn);
                        sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters.Add("@tagNo", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters.Add("@name", System.Data.SqlDbType.VarChar);
                        //sqlcmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@ID"].Value = cow.Id;
                        sqlcmd.Parameters["@tagNo"].Value = cow.TagNo;
                        sqlcmd.Parameters["@name"].Value = cow.Name;
                        //sqlcmd.Parameters["@category"].Value = cow.Category;
                        if (cow.Breed != null)
                        {
                            sqlcmd.Parameters.Add("@breed", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@breed"].Value = cow.Breed;
                        }
                        if (cow.Lactation != null)
                        {
                            sqlcmd.Parameters.Add("@lactation", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@lactation"].Value = cow.Lactation;
                        }
                        if (cow.DOB != null)
                        {
                            sqlcmd.Parameters.Add("@dob", System.Data.SqlDbType.DateTime);
                            // sqlcmd.Parameters["@dob"].Value = ((DateTime)cow.DOB).ToString("yyyy-MM-dd 00:00:00");
                            sqlcmd.Parameters["@dob"].Value = cow.DOB;
                        }
                        if (cow.DamID != null)
                        {
                            sqlcmd.Parameters.Add("@damID", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@damID"].Value = cow.DamID;
                        }
                        if (cow.SireID != null)
                        {
                            sqlcmd.Parameters.Add("@sireID", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@sireID"].Value = cow.SireID;
                        }
                        if (cow.Colour != null)
                        {
                            sqlcmd.Parameters.Add("@colour", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@colour"].Value = cow.Colour;
                        }
                        if (cow.Weight != null)
                        {
                            sqlcmd.Parameters.Add("@weight", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@weight"].Value = cow.Weight;
                        }
                        if (cow.Height != null)
                        {
                            sqlcmd.Parameters.Add("@height", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@height"].Value = cow.Height;
                        }
                        if (cow.ButterFat != null)
                        {
                            sqlcmd.Parameters.Add("@butterFat", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@butterFat"].Value = cow.ButterFat;
                        }
                        if (cow.Picture != null && cow.Picture != "")
                        {
                            sqlcmd.Parameters.Add("@picture", System.Data.SqlDbType.VarChar);
                            sqlcmd.Parameters["@picture"].Value = cow.Picture;
                        }
                        if (cow.NoOfTeatsWorking != null)
                        {
                            sqlcmd.Parameters.Add("@noOfTeatsWorking", System.Data.SqlDbType.SmallInt);
                            sqlcmd.Parameters["@noOfTeatsWorking"].Value = cow.NoOfTeatsWorking;
                        }
                        if (cow.Location != null)
                        {
                            sqlcmd.Parameters.Add("@location", System.Data.SqlDbType.SmallInt);
                            sqlcmd.Parameters["@location"].Value = cow.Location;
                        }
                        if (cow.Remarks != null)
                        {
                            sqlcmd.Parameters.Add("@remarks", System.Data.SqlDbType.VarChar);
                            sqlcmd.Parameters["@remarks"].Value = cow.Remarks;
                        }
                        try
                        {
                            Console.WriteLine("HI");
                            conn.Open();
                            sqlcmd.ExecuteNonQuery();
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Save successfully";
                            data2["status"] = "Success";
                            data["data"] = data2;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Savin Failed");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Save Failed";
                            data2["status"] = "Failure";
                            data["data"] = data2;
                        }
                    }
                }
                return data;
            }
            else
            {
                
                Console.WriteLine("Saving Failed Validation error");
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Cow Save Failed validation errror";
                data2["status"] = "Failure";
                data["data"] = data2;
                data["errors"] = cow.errors;
                return data;
            }
        }

        internal static Dictionary<string, object> SetCowMilkingStatus(IConfiguration _configuration, long cowID, string milkStatus, SqlConnection conn2, SqlTransaction tran)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            int milkingStatus = 0;
            if (milkStatus == "START")
            {
                milkingStatus = 1;
            }
            else if (milkStatus == "STOP")
            {
                milkingStatus = 0;
            }
            else
            {
                Console.WriteLine("Invalid Cow milk Status" + milkStatus);
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Invalid Cow Milk Status";
                data2["status"] = "failure";
                data["data"] = data2;
                return data;
            }
            string query = "Update Animals set MilkingStatus = @MilkingStatus where Id = @ID";
            Console.WriteLine(query);
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            try
            {
                SqlConnection conn;
                if (conn2 == null)
                {
                    conn = new SqlConnection(connectionString);
                    conn.Open();
                }
                else
                {
                    conn = conn2;
                
                }
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if(conn2!=null && tran != null)
                {
                    sqlcmd.Transaction = tran;
                    tran.Save("UpdateCowMilkingStatus");
                }
                sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@ID"].Value = cowID;
                sqlcmd.Parameters.Add("@MilkingStatus", System.Data.SqlDbType.Int);
                sqlcmd.Parameters["@MilkingStatus"].Value = milkingStatus;
                try
                {
                    
                    int i = sqlcmd.ExecuteNonQuery();
                    if (conn2 == null)
                    {
                        conn.Close();
                    }
                    if (i > 0)
                    {
                        Console.WriteLine(Convert.ToBoolean(milkingStatus));
                        Console.WriteLine(milkingStatus);
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = $"Cow MilkingStatus is set to {milkStatus}";
                        data2["status"] = "success";
                        data["data"] = data2;
                        data["MilkingStatus"] = Convert.ToBoolean(milkingStatus);
                    }
                    else
                    {
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Cow Milking Status UPdate Failed";
                        data2["status"] = "failure";
                        data["data"] = data2;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e.Message);
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Cow Milking Status UPdate Failed"+e.Message;
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }catch(Exception e)
            {
                Console.WriteLine("Error " + e.Message + e.StackTrace);
                Console.WriteLine("Exception : " + e.Message);
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Cow Milking Status UPdate Failed" + e.Message;
                data2["status"] = "failure";
                data["data"] = data2;
            }
            return data;
        }

        internal Dictionary<string, object> AddCowsServiceData(CowServiceDataModel conceive)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (conceive.ValidateAdd() == true)
            {
                string query = "Insert into CowConceiveData (CowID,BullID, MatingProcessType,PregnancyStatus,DateOfService,Remarks,Deleted,DoctorID) Values(@CowID,@BullID,@MatingProcessType,@PregnancyStatus,@DateOfService,@Remarks,@Deleted,@DoctorID)";
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
                    sqlcmd.Parameters.Add("@DateOfService", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@DateOfService"].Value = conceive.DateOfService;
                    sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Remarks"].Value = conceive.Remarks;
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = "false";
                    sqlcmd.Parameters.Add("@DoctorID", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@DoctorID"].Value = conceive.DoctorID;

                    //sqlcmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@category"].Value = cow.Category;
                    try
                    {
                        conn.Open();
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Conceive Data Saved Successfully";
                            data2["status"] = "success";
                            data["data"] = data2;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Saving Failed";
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
                data["errors"] = conceive.errors;
                data["message"] = "Adding Service Detail Failed! Invaid Data Enterred";
                data["status"] = false;

            }
            return data;
            
        }

        internal Dictionary<string, object> SetCowMilking(long? id, bool? milking)
        {
            Dictionary<string, object> data_ = new Dictionary<string, object>();
            Dictionary<string, string> errors = new Dictionary<string, string>();
            bool error = false;
            
            if (id != null && milking != null)
            {
                if (error == true)
                {
                    data_["errors"] = errors;
                    data_["status"] = "failure";
                }
                else
                {
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query;
                        if (milking == true)
                        {
                            query = $"Update Animals set MilkingStatus = 1 where Category = 'COW' and Id = @Id";
                        }
                        else
                        {
                            query = $"Update Animals set MilkingStatus = 0 where Category = 'COW' and Id = @Id";
                        }
                        Console.Write(query);
                        SqlCommand sqlcmd = new SqlCommand(query, conn);
                        sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@Id"].Value = id;
                        try
                        {
                            conn.Open();
                            int i = sqlcmd.ExecuteNonQuery();
                            Console.Write(i);
                            if (i > 0)
                            {
                                data_["status"] = "success";
                                if (milking == true)
                                {
                                    data_["message"] = "Cow is Set to Milking";
                                }
                                else
                                {
                                    data_["message"] = "Cow is Off From Milking " + i;
                                }
                            }
                            else
                            {
                                data_["status"] = "failure";
                                data_["message"] = "Failed To Update Milking " + i;
                            }

                        }
                        catch (Exception e1)
                        {
                            Console.WriteLine(e1.Message);
                            errors["errors"] = e1.Message;
                            data_["status"] = "failure";
                            data_["message"] = "Failed To Update Milking " ;
                            data_["errors"] = errors;
                        }


                    }
                }
                return data_;
            }
            else
            {
                data_["status"] = "failure";
                data_["message"] = "Not Data found";
                return data_;
            }
        }
        internal Dictionary<string, Dictionary<string, string>> AddCow(CowModel cow)
        {
            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
            string image = null;
            if (cow.ValidateCow(this,"Add") == true)
            {
                cow.GenerateImageName(this.GetMaxAnimalId());
                Dictionary<string, string> errors = new Dictionary<string, string>();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                        System.Console.WriteLine("HELLO");
                            string query = "INSERT into [dbo].[Animals] ([Category],[Gender],[TagNo],[Name],[Breed],[Lactation],[DOB],[DamID],[SireID],[Colour],[Weight],[Height],[ButterFat],[Picture])"
                    + " OUTPUT INSERTED.ID values('COW','Female',@tagNo,@name,@breed,@lactation,@dob,@damID,@sireID,@colour,@weight,@height,@butterFat,@picture);";//set [TagNo] = @tagNo,[Name] = @name";
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    //sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters.Add("@tagNo", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters.Add("@name", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@ID"].Value = cow.Id;
                    sqlcmd.Parameters["@tagNo"].Value = cow.TagNo;
                    sqlcmd.Parameters["@name"].Value = cow.Name;

                    //sqlcmd.Parameters["@category"].Value = cow.Category;
                    if (cow.Breed != null || true)
                    {
                        sqlcmd.Parameters.Add("@breed", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@breed"].Value = cow.Breed;
                    }
                    if (cow.Lactation != null || true)
                    {
                        sqlcmd.Parameters.Add("@lactation", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@lactation"].Value = cow.Lactation;
                    }
                    if (cow.DOB != null || true)
                    {
                        sqlcmd.Parameters.Add("@dob", System.Data.SqlDbType.DateTime);
                        // sqlcmd.Parameters["@dob"].Value = ((DateTime)cow.DOB).ToString("yyyy-MM-dd 00:00:00");
                        sqlcmd.Parameters["@dob"].Value = cow.DOB;
                    }
                    if (cow.DamID != null || true)
                    {
                        sqlcmd.Parameters.Add("@damID", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@damID"].Value = cow.DamID;
                    }
                    if (cow.SireID != null || true)
                    {
                        sqlcmd.Parameters.Add("@sireID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@sireID"].Value = cow.SireID;
                    }
                    if (cow.Colour != null || true)
                    {
                        sqlcmd.Parameters.Add("@colour", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@colour"].Value = cow.Colour;
                    }
                    if (cow.Weight != null || true)
                    {
                        sqlcmd.Parameters.Add("@weight", System.Data.SqlDbType.Decimal);
                        sqlcmd.Parameters["@weight"].Value = cow.Weight;
                    }
                    if (cow.Height != null || true)
                    {
                        sqlcmd.Parameters.Add("@height", System.Data.SqlDbType.Decimal);
                        sqlcmd.Parameters["@height"].Value = cow.Height;
                    }
                    if (cow.ButterFat != null || true)
                    {
                        sqlcmd.Parameters.Add("@butterFat", System.Data.SqlDbType.Decimal);
                        sqlcmd.Parameters["@butterFat"].Value = cow.ButterFat;
                    }
                    if (cow.Picture != null && cow.Picture != "" || true)
                    {
                        sqlcmd.Parameters.Add("@picture", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@picture"].Value = cow.Picture;
                    }
                    try
                    {
                        Console.WriteLine("HI");
                        conn.Open();
                        cow.Id = (Int64) sqlcmd.ExecuteScalar();
                        cow.SaveImage2();
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Cow Added successfully ID:"+cow.Id;
                        data2["status"] = "Success";
                        data["data"] = data2;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Savin Failed");
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Cow Save Failed";
                        data2["status"] = "Failure";
                        data["data"] = data2;
                    }
                }
                return data;
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Cow Save Failed Validations Failed";
                data2["status"] = "Failure";
                data["data"] = data2;
                data["errors"] = cow.errors;
                return data;
            }
        }
        internal CowModel GetCowById(int id)
        {
            CowModel cowModel = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                System.Console.WriteLine("HELLO");
                //
                SqlCommand sqlcmd = new SqlCommand("Select Animals.*,Dam.TagNo as DamTagNo_,Dam.Name as DamName_,Sire.TagNo as SireTagNo_,Sire.Name as SireName_ from Animals left join Animals Dam on Dam.Id = Animals.DamID Left join Animals Sire on Sire.Id = Animals.SireID where Animals.Id = @ID and Animals.Gender = 'Female' and Animals.Category = 'COW'", conn);
                sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                sqlcmd.Parameters["@ID"].Value = id;
                try
                {
                    Console.WriteLine("HI");
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        cowModel = new CowModel(sqlrdr);
                        /*cowModel.CowID = Convert.ToInt32(sqlrdr["Id"]);
                        cowModel.CowTagNo = Convert.ToString(sqlrdr["CowTagNo"]);
                        cowModel.CowName = Convert.ToString(sqlrdr["CowName"]);
                        cowModel.Breed = Convert.ToString(sqlrdr["Breed"]);
                        cowModel.Colour = Convert.ToString(sqlrdr["Colour"]);
                        //cowModel.SireID = Convert.ToInt32(sqlrdr["SireID"]);
                        //cowModel.DamID = Convert.ToInt32(sqlrdr["DamID"]);
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
                    sqlrdr.Close();
                    conn.Close();
                    return cowModel;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HELO");
                    Console.WriteLine(ex.ToString());
                    return cowModel;
                }

            }
        }

        public Dictionary<long, object> GetCowsIdNamePairByTagNo(string tagNo,int pageno=1)
        {

            Dictionary<long, object> cows = new Dictionary<long, object>();
            AnimalFilter animalFilter = new AnimalFilter();
            animalFilter.TagNo = "%" + tagNo + "%";
            //animalFilter.TagNo = "GR 018";
            animalFilter.PageNo = pageno;
            animalFilter.RecordsPerPage = 20;
            animalFilter.Category = "COW";
            animalFilter.OrderBy = "TagNo";
            animalFilter.Order = "ASC";
            animalFilter.GetCategory = false;
            cows = base.GetAnimalsIDNameTagNoPair(animalFilter);
            //data["data"] = heifers;
            return cows;
            /*Dictionary<int, string> CowIdPair = new Dictionary<int, string>();
            int limit = 20;
            int offset = (pageno-1)*limit;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                System.Console.WriteLine("HELLO"+tagNo);
                tagNo = "%"+tagNo+"%";
                SqlCommand sqlcmd = new SqlCommand("Select * from Animals where Category  ='COW' and TagNo like @TagNo order by TagNo  OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", conn);
                sqlcmd.Parameters.Add("@TagNo", System.Data.SqlDbType.VarChar);
                sqlcmd.Parameters["@TagNo"].Value = tagNo;

                sqlcmd.Parameters.Add("@Offset", System.Data.SqlDbType.Int);
                sqlcmd.Parameters["@Offset"].Value = offset;

                sqlcmd.Parameters.Add("@Limit", System.Data.SqlDbType.Int);
                sqlcmd.Parameters["@Limit"].Value = limit;

                try
                {
                    conn.Open();
                    //sqlcmd.Prepare();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Console.WriteLine("Date FOund" + Convert.ToString(sqlrdr["TagNo"]));
                        CowIdPair.Add(Convert.ToInt32(sqlrdr["Id"]), Convert.ToString(sqlrdr["TagNo"]));
                    }
                    sqlrdr.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("HELO");
                    Console.WriteLine(ex.ToString());
                }
                return CowIdPair;
            }
            */
        }
        public List<Dictionary<string, object>> GetSires(BullFilterModel bullFilter)
        {
            int length = 10;
            int start = 0;
            string name = null;
            string tagNo = null;
            if (bullFilter != null)
            {
                try
                {
                    length = (int)bullFilter.length;
                    start = (int)bullFilter.start;

                }
                catch (Exception e)
                {

                }
                BullModel bull_ = bullFilter.bull;
                if (bull_.Name != null && bull_.Name.Trim() != "")
                {
                    name = "%"+bull_.Name+ "%";
                }
                if (bull_.TagNo != null && bull_.TagNo.Trim() != "")
                {
                    tagNo = "%" + bull_.TagNo+ "%" ;
                }
            }
           // List<BullModel> cowsList = new List<BullModel>();
            List<Dictionary<string, object>> cowsList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string where = "";
                if(name != null)
                {
                    where = $" and Name like @Name ";
                }
                if(tagNo != null)
                {
                    where += $" and  TagNo like @TagNo";
                }
                string query = $"Select * from Animals where Category = 'Bull' {where} order by TagNo  OFFSET {start} ROWS FETCH NEXT {length} ROWS ONLY";
                Console.Write(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                //SqlCommand sqlcmd = new SqlCommand("Update [dbo].[Animals] set [TagNo] = @tagNo,[Name] = @name,[Category] = @Category where [Animals].[Id] = @ID", conn);
                if (name != null)
                {
                    sqlcmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Name"].Value = name;
                }
                if (tagNo != null)
                {
                    sqlcmd.Parameters.Add("@TagNo", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@TagNo"].Value = tagNo;
                }
                try
                {
                    //Console.WriteLine("HI");
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        //BullModel BullModel = new BullModel(sqlrdr);
                        Dictionary<string, object> bull = new Dictionary<string, object>();
                        bull["id"] = sqlrdr["Id"];
                        bull["tagNo"] = sqlrdr["TagNo"];
                        bull["name"] = sqlrdr["Name"];
                        bull["dob"] = Helper.FormatDate(sqlrdr["DOB"]) ;
                        bull["breed"] = sqlrdr["Breed"].ToString();
                        bull["weight"] = sqlrdr["Weight"].ToString();
                        //bull["colour"] = sqlrdr["Colour"].ToString();
                        bull["colour"] = sqlrdr["Colour"].ToString();
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

                        //cowsList.Add(BullModel);
                        cowsList_.Add(bull);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return cowsList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return cowsList_;
                }
            }
        }

        internal int GetTotalFilteredSires(BullFilterModel cowFilters)
        {
            if (cowFilters != null)
            {
                BullModel cow_ = cowFilters.bull;
            }
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                System.Console.WriteLine("HELLO");
                SqlCommand sqlcmd = new SqlCommand($"Select  count(*) as total from Animals where Category = 'BULL'", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        return Convert.ToInt32(sqlrdr["total"]);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return 0;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return 0;
        }

        internal object GetTotalSires()
        {
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand($"Select  count(*) as total from Animals where Category = 'BULL'", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        return Convert.ToInt32(sqlrdr["total"]);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return 0;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
            return 0;
        }
        public static long GetMaxAnimalId(IConfiguration _configuration)
        {
            long Id = 0;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand($"Select  max(Id) as Id from Animals", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Id = Convert.ToInt64(sqlrdr["Id"]);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return Id;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed in fetching Max id"+ex.Message);
                    return Id;
                }
            }
            Console.WriteLine("Failed in fetching Max id 123");
            return 0;
        }
        internal long GetMaxAnimalId()
        {
            long Id = 0;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand($"Select  max(Id) as Id from Animals", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Id= Convert.ToInt64(sqlrdr["Id"]);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return Id;
                }
                catch (Exception ex)
                {
                    return Id;
                }
            }
            return 0;
        }
        internal Dictionary<int,string> getBreeds()
        {
            Dictionary<int, string> breeds = new Dictionary<int, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand($"Select Id,Breed from Breeds", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int i = 0;
                    while (sqlrdr.Read())
                    {
                        Breeds breed_ = new Breeds();
                        breed_.Id = Convert.ToInt32(sqlrdr.GetValue(0));
                        breed_.Breed = sqlrdr.GetValue(1).ToString();
                        breeds[(int)breed_.Id] = breed_.Breed.ToString();
                        i++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return breeds;
                }
                catch (Exception ex)
                {
                    return breeds;
                }
            }
            return breeds;
        }
        internal Dictionary<int, string> getColors()
        {
            Dictionary<int, string> colors = new Dictionary<int, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand($"Select Id,Colour from AnimalColors", conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int i = 0;
                    
                    while (sqlrdr.Read())
                    {
                       
                        Colors colour_ = new Colors();
                        colour_.Id = Convert.ToInt32(sqlrdr.GetValue(0));
                        colour_.Color = sqlrdr.GetValue(1).ToString();
                        Console.WriteLine(colour_.Id+","+colour_.Color);
                        colors[(int)colour_.Id] = colour_.Color.ToString();
                        i++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return colors;
                }
                catch (Exception ex)
                {
                    return colors;
                }
            }
            return colors;
        }
        public bool isTagNoUnique(string tagNo,long? id=null)
        {
            if (tagNo != null)
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    string query = $"Select count(*) as total from Animals where TagNo = @TagNo";
                    if (id != null)
                    {
                        query += " and ID != @Id";
                    }
                    //Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = query;
                    try
                    {
                        sqlcmd.Parameters.Add("@TagNo", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@TagNo"].Value = tagNo.Trim();
                        if (id != null)
                        {
                            sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters["@Id"].Value = id;
                        }
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        if (sqlrdr.Read())
                        {
                            int total = Convert.ToInt32(sqlrdr.GetValue(0));
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
        public Dictionary<string, object> GetCalvingDetailByCowId(long id)
        {
            bool GetIntercalvperiod = true;
            Dictionary<int, DateTime> intercalv = new Dictionary<int, DateTime>();

            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<long, Dictionary<string, object>> calvs = new Dictionary<long, Dictionary<string, object>>();
            List<long> bulls = new List<long>();
            if (id != null)
            {
                ArrayList cowConcieveData = new ArrayList();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    string query = "Select Animals.*,CowConceiveData.Id as cow_service_id,CowConceiveData.DateOfService,CowConceiveData.DeliveryStatus," +
                        "CowConceiveData.DamWeight,CowConceiveData.BirthWeight,CowConceiveData.Remarks from Animals " +
                        "left Join CowConceiveData on CowConceiveData.AnimalId = Animals.Id where DamID = @DamID order by BirthLactationNumber ASC";
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    try
                    {
                        sqlcmd.Parameters.Add("@DamID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@DamID"].Value = id;
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        long counter = 1;
                        string cowTagNumber = this.GetCowTagNameById(id);
                        while (sqlrdr.Read())
                        {
                            Console.WriteLine("Sire ID " + sqlrdr["SireID"]);
                            if (sqlrdr["SireID"] != null && sqlrdr["SireID"].ToString().Trim()!="" && !bulls.Contains(Convert.ToInt64(sqlrdr["SireID"])))
                            {
                                bulls.Add(Convert.ToInt64(sqlrdr["SireID"]));
                            }
                            Dictionary<string, object> calv = new Dictionary<string, object>();
                            Console.WriteLine("COW SERVICE ID'"+sqlrdr["cow_service_id"]+"'");
                            calv["sno"] = counter;
                            calv["id"] = Helper.IsNullOrEmpty(sqlrdr["Id"]);
                            calv["cow_service_id"] = Helper.FormatLong(sqlrdr["cow_service_id"]);
							calv["dateOfService"] = Helper.FormatDate(sqlrdr["DateOfService"]);
                            calv["bullID"] = Helper.IsNullOrEmpty(sqlrdr["SireID"]);
                            // calv["battalion_name"]
                            calv["cowNo"] = cowTagNumber;
                            //calv["bullSemenNo"]
                            calv["deliveryDate"] = sqlrdr["DOB"];
                            calv["lactationNo"] = Helper.IsNullOrEmpty(sqlrdr["BirthLactationNumber"]);
                            calv["deliveryStatus"] = Helper.IsNullOrEmpty(sqlrdr["DeliveryStatus"]);
                            calv["damWeight"] = Helper.IsNullOrEmpty(sqlrdr["DamWeight"]);
                            calv["gender"] = sqlrdr["Gender"];
                            calv["tagNo"] = sqlrdr["TagNo"];
                            calv["birthWeight"] = Helper.IsNullOrEmpty(sqlrdr["BirthWeight"]);
                            calv["remarks"] = Helper.IsNullOrEmpty(sqlrdr["Remarks"]);
                            //calv["action"]
                            calvs[Convert.ToInt64(sqlrdr["Id"])] = calv;
                            counter++;
                            Console.WriteLine(" Lactation no "+calv["lactationNo"].ToString().Trim());
                            if(GetIntercalvperiod == true && calv["lactationNo"].ToString().Trim()!="")
                            {
                                intercalv[Convert.ToInt32(calv["lactationNo"])] = (DateTime)(calv["deliveryDate"]);
                            }
                            calv["deliveryDate"] = Helper.FormatDate(calv["deliveryDate"]);
                            //CowServiceDataModel conceive = new CowServiceDataModel(sqlrdr);
                            //cowConcieveData.Add(calv);
                            //cowConcieveData.Add(conceive);
                        }
                        sqlrdr.Close();
                        conn.Close();
                        Dictionary<string, string> interCalvPeriod = new Dictionary<string, string>();
                        if (intercalv.Count > 1)
                        {
                            int i = 0;
                            int? lactation = null;
                            DateTime lastDate = DateTime.Now;
                            
                            foreach(var m in intercalv)
                            {
                                if (i > 0)
                                {
                                    if (lastDate != null)
                                    {
                                        //DateTime temp = new DateTime(((DateTime)m.Value).Subtract(lastDate).TotalMilliseconds);
                                        int days = ((DateTime)m.Value).Subtract(lastDate).Days;
                                        int months = days / 30;
                                        days = days % 30;
                                        interCalvPeriod[lactation.ToString() + "-" + m.Key.ToString()] = $"{months} Months, {days} Days";
                                    }
                                }
                                lactation = Convert.ToInt32(m.Key);
                                lastDate = (DateTime)m.Value;
                                i++;
                                
                            }
                        }
                        BullsContext bullsContext = new BullsContext(this._configuration);
                        Dictionary<long, string>  bullsNameId = bullsContext.GetBullsNamesByIds(bulls);
                        foreach(var m in calvs)
                        {
                            try
                            {
                                calvs[m.Key]["bullSemenNo"] = bullsNameId[Convert.ToInt64(calvs[m.Key]["bullID"])];
                            }catch(Exception e)
                            {
                                calvs[m.Key]["bullSemenNo"] = "-";
                            }
                            cowConcieveData.Add(calvs[m.Key]);
                        }
                        data["status"] = "success";
                        data["message"] = "calves Found";
                        data["data"] = cowConcieveData;
                        data["recordsTotal"] = cowConcieveData.Count;
                        data["recordsFiltered"] = cowConcieveData.Count;
                        data["interCalvPeriod"] = interCalvPeriod;
                    }
                    catch (Exception ex)
                    {
                        //return false;
                        data["status"] = "failure";
                        data["message"] = "Connection failure" + ex.ToString() ;
                        data["exception"] = ex.StackTrace;
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
        public Dictionary<string, object> GetServiceDetailByCowId(long id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (id != null)
            {
                ArrayList cowConcieveData = new ArrayList();
                Dictionary<int, Dictionary<string, object>> conceive_data = new Dictionary<int, Dictionary<string, object>>();
                List<long> bull_ids = new List<long>();
                List<long> doctor_ids = new List<long>();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    SqlCommand sqlcmd = new SqlCommand("Select * from CowConceiveData where CowID = @CowID", conn);
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
                        string cow_name = this.GetCowTagNameById(id);
                        while (sqlrdr.Read())
                        {
                            bull_Id = 0;
                            Dictionary<string, object> temp = new Dictionary<string, object>();
                            temp["id"] = sqlrdr["Id"];
                            temp["sno"] = sno;
                            sno++;
                            temp["cowID"] = sqlrdr["CowID"];
                            temp["bullID"] = sqlrdr["BullID"];
                            temp["matingProcessType"] = sqlrdr["MatingProcessType"];
                            temp["pregnancyStatus"] = sqlrdr["PregnancyStatus"];
                            temp["dateOfService"] = sqlrdr["DateOfService"];
                            temp["time"] = sqlrdr["DateOfService"];
                            temp["remarks"] = sqlrdr["Remarks"].ToString();
                            temp["bullsName"] = sqlrdr["BullSemenNo"];
                            temp["cowName"] = cow_name;
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
                        sqlrdr.Close();
                        conn.Close();
                        //fetch all the bulls
                        
                        if (bull_ids.Count > 0)
                        {
                            BullsContext bulls = new BullsContext(this._configuration);
                            Dictionary<long, string> bullsIdName = bulls.GetBullsNamesByIds(bull_ids);
                            foreach(var m in cowConcieveData)
                            {
                                Dictionary<string, object> a = (Dictionary<string, object>)m;
                                
                                try
                                {
                                    a["bullsName"] = bullsIdName[Convert.ToInt64(a["bullID"])];
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
                        data["message"] = "calves Found";
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
        public Dictionary<long, object> GetCowTagNoNameByIds(List<long> ids)
        {
            
            Dictionary<long, object> cows = new Dictionary<long, object>();
            string values = "";
            if (ids.Count > 0)
            {
                foreach(long id in ids)
                {
                    if (values != "")
                    {
                        values += ",";
                    }
                    values += id.ToString();
                }
            }
            else
            {
                return cows;
            }
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                string query = $"Select Id,Name,TagNo from Animals where Id in  ({values})";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, string> cowData = new Dictionary<string, string>();
                        cowData["name"] = Convert.ToString(sqlrdr["Name"]);
                        cowData["tagNo"] = Convert.ToString(sqlrdr["TagNo"]);
                        cows[Convert.ToInt64(sqlrdr["Id"])] = cowData;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                }
            }
            return cows;
        }
        public Dictionary<string,string> GetCowTagNoNameById(long id)
        {
            Dictionary<string, string> cowData = new Dictionary<string, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                SqlCommand sqlcmd = new SqlCommand("Select Name,TagNo from Animals where Id = @Id", conn);
                try
                {
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = id;
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        cowData["name"] = Convert.ToString(sqlrdr["Name"]);
                        cowData["tagNo"] = Convert.ToString(sqlrdr["TagNo"]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                }
            }
            return cowData;
        }
        private string GetCowTagNameById(long id)
        {
            string cow_name = "";
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                SqlCommand sqlcmd = new SqlCommand("Select TagNo from Animals where Id = @Id", conn);
                try
                {
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = id;
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        cow_name = Convert.ToString(sqlrdr["TagNo"]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                }
            }
            return cow_name;
        }
        public static Dictionary<string, string> GetCowNameTagNoById(IConfiguration _configuration, long id)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand("Select Name,TagNo from Animals where Id = @Id", conn);
                try
                {
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = id;
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        data["name"] = Convert.ToString(sqlrdr["Name"]);
                        data["tagNo"] = Convert.ToString(sqlrdr["TagNo"]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                }
            }
            return data;
        }
        public static Dictionary<string, object> GetDetailById(IConfiguration _configuration,long id)
        {
            Dictionary<string, object> cowData = new Dictionary<string, object>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                SqlCommand sqlcmd = new SqlCommand("Select * from Animals where Id = @Id", conn);
                try
                {
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = id;
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        cowData["id"] = sqlrdr["Id"];
                        cowData["name"] = sqlrdr["Name"];
                        cowData["tagNo"] = sqlrdr["TagNo"];
                        cowData["dob"] = sqlrdr["DOB"];
                        cowData["category"] = sqlrdr["Category"];
                        cowData["gender"] = sqlrdr["Gender"];
                        cowData["sireID"] = sqlrdr["SireID"];
                        cowData["damID"] = sqlrdr["DamID"];
                        cowData["butterFat"] = sqlrdr["ButterFat"];
                        cowData["pregnancyStatus"] = sqlrdr["PregnancyStatus"];
                        cowData["status"] = sqlrdr["Status"];
                        cowData["reproductiveStatus"] = sqlrdr["ReproductiveStatus"];
                        cowData["milkingStatus"] = sqlrdr["MilkingStatus"];
                        cowData["remarks"] = sqlrdr["Remarks"];
                        cowData["additionalInfo"] = sqlrdr["AdditionalInfo"];
                        cowData["picture"] = sqlrdr["Picture"];
                        cowData["lactation"] = sqlrdr["Lactation"];
                        cowData["type"] = sqlrdr["Type"];
                        cowData["semenDoses"] = sqlrdr["SemenDoses"];
                        cowData["weight"] = sqlrdr["Weight"];
                        cowData["alive"] = sqlrdr["Alive"];
                        cowData["birthLactationNumber"] = sqlrdr["BirthLactationNumber"];
                        cowData["height"] = sqlrdr["Height"];
                        cowData["dateOfDeath"] = sqlrdr["DateOfDeath"];
                        cowData["colour"] = sqlrdr["Colour"];
                        cowData["breed"] = sqlrdr["Breed"];
                        cowData["location"] = sqlrdr["Location"];
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error " + e.Message);
                }
            }
            return cowData;
        }
        public Dictionary<string, object> GetCalvesByCowId(long id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (id != null)
            {
                AnimalModel[] calves = { };
                ArrayList calves_ = new ArrayList();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    SqlCommand sqlcmd = new SqlCommand("Select * from Animals where DamID = @DamID", conn);
                    try
                    {
                        sqlcmd.Parameters.Add("@DamID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@DamID"].Value = id;
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        long counter = 0;
                        while (sqlrdr.Read())
                        {
                            string category = Convert.ToString(sqlrdr.GetValue(4));
                            AnimalModel ani = null;
                            switch (category)
                            {
                                case "COW":
                                    ani = new CowModel(sqlrdr);
                                    break;
                                case "BULL":
                                    ani = new BullModel(sqlrdr);
                                    break;
                                case "CALF":
                                    ani = new CalfModel(sqlrdr);
                                    break;
                                case "HEIFER":
                                    ani = new HeiferModel(sqlrdr);
                                    break;
                            }
                            if (ani != null)
                            {
                                //calves[counter] = ani;
                                calves_.Add(ani);
                                counter++;
                            }
                        }
                        sqlrdr.Close();
                        conn.Close();
                        data["status"] = "success";
                        data["message"] = "calves Found";
                        data["data"] = calves_;
                        data["recordsTotal"] = 100;
                        data["recordsFiltered"] = 100;
                    }
                    catch (Exception ex)
                    {
                        //return false;
                        data["status"] = "failure";
                        data["message"] = "Connection failure"+ex.ToString();
                        data["data"] = calves;
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
        public static Dictionary<long, string> GetCowsNamesByIds(IConfiguration _configuration, List<long> cows)
        {
            Dictionary<long, string> cowsIdName = new Dictionary<long, string>();
            string ids = "";
            foreach (var m in cows)
            {
                if (ids != "")
                {
                    ids += ",";
                }
                ids += m.ToString();
            }
            if (ids != "")
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = $"Select Id,TagNo from Animals where Category = 'COW' and Id in ({ids})";
                    Console.Write(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    try
                    {
                        Console.WriteLine("HI");
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        while (sqlrdr.Read())
                        {
                            cowsIdName[Convert.ToInt64(sqlrdr["Id"])] = sqlrdr["TagNo"].ToString();
                        }
                        sqlrdr.Close();
                        conn.Close();
                        return cowsIdName;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return cowsIdName;
                    }
                }
            }
            return cowsIdName;
        }

        public static bool IsTagNoUnique_(IConfiguration _configuration, string tagNo, long? id = null)
        {
            if (tagNo != null)
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    string query = $"Select count(*) as total from Animals where TagNo = @TagNo";
                    if (id != null)
                    {
                        query += " and ID != @Id";
                    }
                    //Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand();
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = query;
                    try
                    {
                        sqlcmd.Parameters.Add("@TagNo", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@TagNo"].Value = tagNo.Trim();
                        if (id != null)
                        {
                            sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                            sqlcmd.Parameters["@Id"].Value = id;
                        }
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        if (sqlrdr.Read())
                        {
                            int total = Convert.ToInt32(sqlrdr.GetValue(0));
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
        internal Dictionary<string, object> UpdateCowLactationNo(long CowID,int lactationNumber=-1, SqlConnection? conn2 = null, SqlTransaction? tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            int maxLactaionNumber = CowsContext.GetMaxLactationNumber(_configuration, CowID);
            maxLactaionNumber++;
            Console.WriteLine("Max Lactation Number Before = " + maxLactaionNumber);
            Console.WriteLine("Lactation Number = " + lactationNumber);
            if (lactationNumber != -1 && lactationNumber>=maxLactaionNumber)
            {
                Console.WriteLine("Greator");
                maxLactaionNumber = lactationNumber;
            }
            else
            {
                Console.WriteLine("No need update");
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                maxLactaionNumber--;
                data2["message"] = $"No need To Update Lactation Number ({maxLactaionNumber})";
                data2["status"] = "success";
                data["newLactationNumber"] = maxLactaionNumber;
                data["data"] = data2;
                if (tran != null)
                {
                    data["tran"] = tran;
                }
                return data;
            }
            Console.WriteLine("Max Lactation Number = " + maxLactaionNumber);
            Dictionary<string, string> errors = new Dictionary<string, string>();
            if (maxLactaionNumber > 0)
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                SqlConnection conn;
                if (conn2 != null)
                {
                    conn = conn2;
                }
                else
                {
                    conn = new SqlConnection(connectionString);
                }
                string query = "Update Animals set Lactation = @LactationNo where Animals.Id = @Id";
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (tran != null)
                {
                    tran.Save("UpdateCowLactationNumber");
                    sqlcmd.Transaction = tran;
                }
                //sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                sqlcmd.Parameters.Add("@LactationNo", System.Data.SqlDbType.Int);
                sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@LactationNo"].Value = maxLactaionNumber;
                sqlcmd.Parameters["@Id"].Value = CowID;
                try
                {
                    Console.WriteLine("HI");
                    if (tran == null)
                    {
                        conn.Open();
                    }
                    else
                    {
                        sqlcmd.Transaction = tran;
                        tran.Save("save2");
                    }
                    int i = sqlcmd.ExecuteNonQuery();
                    if (i > 0)
                    {
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = $"Lactation Number Updated Successfully Set To ({maxLactaionNumber})";
                        data2["status"] = "success";
                        data["newLactationNumber"] = maxLactaionNumber;
                        data["data"] = data2;
                        if (tran != null)
                        {
                            data["tran"] = tran;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Lactaion Number updation failed "+i);
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Lacation Number Updation Failed";
                        data2["status"] = "failure";
                        data["data"] = data2;
                        if (tran != null)
                        {
                            data["tran"] = tran;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Lactaion Number updation failed");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Lacation Number Updation Failed "+e.Message;
                    data2["status"] = "failure";
                    data["data"] = data2;
                    if (tran != null)
                    {
                        data["tran"] = tran;
                    }
                }
                return data;
            }
            else
            {
                Console.WriteLine("Lactaion Number updation failed less than 0");
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Lacation Number Updation Failed";
                data2["status"] = "Failure";
                data["data"] = data2;
                return data;
            }
        }
        public static int GetMaxLactationNumber(IConfiguration _configuration, long CowID)
        {
            int LactationNo = 0;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString)) { 
                string query = "Select max(LactationNo) as LactationNo from CowConceiveData where CowID = @CowID";
                //string query = "Select max(LactationNo) as LactationNo from CowConceiveData where Id = @CowID";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@CowID"].Value = CowID;
                try
                {
                    Console.WriteLine("HI");
                    conn.Open();
                    SqlDataReader sqlrdr =  sqlcmd.ExecuteReader();
                    if (sqlrdr.Read()) {
                        try {
                            LactationNo = Convert.ToInt32(sqlrdr["LactationNo"]);
                        }catch(Exception e)
                        {
                            Console.Write("Max Lactation No error" + e.Message + "\n" + e.StackTrace);
                            LactationNo = 0;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                }
            }
            return LactationNo;
        }
        public static bool setCowPregnancyStatusById(IConfiguration _configuration,long cow_id, int pregnancyStatus,SqlConnection conn2=null, SqlTransaction tran=null)
        {
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            SqlConnection conn = null;
            if (conn2 == null)
            {
                conn = new SqlConnection(connectionString);
            }
            else
            {
                conn = conn2;
            }
            string query = "Update Animals set PregnancyStatus = @pregnancyStatus where Animals.Id = @Id";
            Console.WriteLine(query);
            SqlCommand sqlcmd = new SqlCommand(query, conn);
            if (tran == null)
            {
                conn.Open();
                //tran = conn.BeginTransaction("AddCalv");
                //tran.Save("save2");
            }
            else
            {
                sqlcmd.Transaction = tran;
                tran.Save("save3");
            }
            sqlcmd.Parameters.Add("@pregnancyStatus", System.Data.SqlDbType.Int);
            sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
            sqlcmd.Parameters["@pregnancyStatus"].Value = pregnancyStatus;
            sqlcmd.Parameters["@Id"].Value = cow_id;
            try
            {
                Console.WriteLine("HI");
                int i = sqlcmd.ExecuteNonQuery();
                if (i > 0)
                {
                    Console.WriteLine("PS Updation Success");
                    return true;
                }
                else
                {
                    Console.WriteLine("PS Updation failed 123.");
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("PS Updation failed");
                return false;
            }
            
            return false;
        }
        public Dictionary<string,object> AddSellCow(SalePurchaseAnimal salePurchaseAnimal, BuyerSellerModal buyerSellerModal){

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (salePurchaseAnimal !=null && buyerSellerModal!=null) //
            {
                if(salePurchaseAnimal.BuyerSellerId ==null){
                    if(salePurchaseAnimal.ValidateSalePurchaseWithoudBuyerSellerId("Add")==false || buyerSellerModal.ValidateBuyerSellerModal("Add")==false){
                        //Dictionary<string,string> errors = Helper.MergeDictionary(salePurchaseAnimal.errors,buyerSellerModal.errors);
                        data["errors"] = salePurchaseAnimal.errors;
                        data["errors2"] = buyerSellerModal.errors;
                        data["status"] = false;
                        data["message"] = "Validation Error";
                        return data;
                    }
                }else{
                    if(salePurchaseAnimal.ValidateSalePurchase("Add")==false){
                        data["errors"] = salePurchaseAnimal.errors;
                        data["errors2"] = null;
                        data["status"] = false;
                        data["message"] = "Validation Error";
                        return data;
                    }
                }
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                SqlTransaction tran = null;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "";
                    if(salePurchaseAnimal.Remarks!=null){
                        query = "INSERT INTO [dbo].[AnimalSalePurchaseDetail] ([AnimalId],[Price],[BuyerSellerId],[Date],[SupervisorId],[SalePurchase],[Remarks])"+
                            " VALUES (@AnimalId,@Price,@BuyerSellerId,@Date,@SupervisorId,@SalePurchase,@Remarks)";
                    }else{
                        query = "INSERT INTO [dbo].[AnimalSalePurchaseDetail] ([AnimalId],[Price],[BuyerSellerId],[Date],[SupervisorId],[SalePurchase])"+
                            " VALUES (@AnimalId,@Price,@BuyerSellerId,@Date,@SupervisorId,@SalePurchase)";
                    }
                    string query2="";
                    if(salePurchaseAnimal.BuyerSellerId!=null){
                        
                    }else{
                        query2 = "INSERT INTO [dbo].[BuyerSellerDetail] ([Name],[Country],[State],[District],[Vill_Mohalla],[StreetNo],[HouseNo],[PIN],[PhoneNumber],[Email]) OUTPUT INSERTED.Id "+
                            "VALUES(@Name,@Country,@State,@District,@Vill_Mohalla,@StreetNo,@HouseNo,@PIN,@PhoneNumber,@Email) ";
                        tran = conn.BeginTransaction("newSeller");
                        
                        tran.Save("save1");
                        SqlCommand sqlcmd2 = new SqlCommand(query2, conn);
                        sqlcmd2.Transaction = tran;
                        sqlcmd2.Parameters.Add("@Name", System.Data.SqlDbType.VarChar);
                        sqlcmd2.Parameters["@Name"].Value = buyerSellerModal.Name;
                        sqlcmd2.Parameters.Add("@Country", System.Data.SqlDbType.SmallInt);
                        sqlcmd2.Parameters["@Country"].Value = buyerSellerModal.Country;
                        sqlcmd2.Parameters.Add("@State", System.Data.SqlDbType.SmallInt);
                        sqlcmd2.Parameters["@State"].Value = buyerSellerModal.State;
                        sqlcmd2.Parameters.Add("@District", System.Data.SqlDbType.SmallInt);
                        sqlcmd2.Parameters["@District"].Value = buyerSellerModal.District;
                        sqlcmd2.Parameters.Add("@Vill_Mohalla", System.Data.SqlDbType.VarChar);
                        sqlcmd2.Parameters["@Vill_Mohalla"].Value = buyerSellerModal.VillMohalla;
                        sqlcmd2.Parameters.Add("@StreetNo", System.Data.SqlDbType.VarChar);
                        sqlcmd2.Parameters["@StreetNo"].Value = buyerSellerModal.StreetNo;
                        sqlcmd2.Parameters.Add("@HouseNo", System.Data.SqlDbType.VarChar);
                        sqlcmd2.Parameters["@HouseNo"].Value = buyerSellerModal.HouseNo;
                        sqlcmd2.Parameters.Add("@PIN", System.Data.SqlDbType.VarChar);
                        sqlcmd2.Parameters["@PIN"].Value = buyerSellerModal.PIN;
                        sqlcmd2.Parameters.Add("@PhoneNumber", System.Data.SqlDbType.VarChar);
                        sqlcmd2.Parameters["@PhoneNumber"].Value = buyerSellerModal.PhoneNumber;
                        sqlcmd2.Parameters.Add("@Email", System.Data.SqlDbType.VarChar);
                        sqlcmd2.Parameters["@Email"].Value = buyerSellerModal.Email;
                        Console.WriteLine(query2);
                        salePurchaseAnimal.BuyerSellerId = (Int64) sqlcmd2.ExecuteScalar();
                    }
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    if(tran!=null){
                        sqlcmd.Transaction = tran;
                    }
                    //sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters.Add("@AnimalId", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@AnimalId"].Value = salePurchaseAnimal.AnimalId;
                    sqlcmd.Parameters.Add("@Price", System.Data.SqlDbType.Decimal);
                    sqlcmd.Parameters["@Price"].Value = salePurchaseAnimal.Price;
                    sqlcmd.Parameters.Add("@BuyerSellerId", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@BuyerSellerId"].Value = salePurchaseAnimal.BuyerSellerId;
                    sqlcmd.Parameters.Add("@Date", System.Data.SqlDbType.Date);
                    sqlcmd.Parameters["@Date"].Value = salePurchaseAnimal.Date;
                    sqlcmd.Parameters.Add("@SupervisorId", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@SupervisorId"].Value = salePurchaseAnimal.SupervisorId;
                    sqlcmd.Parameters.Add("@SalePurchase", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@SalePurchase"].Value = "SELL";
                    if(salePurchaseAnimal.Remarks!=null){
                        sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@Remarks"].Value = salePurchaseAnimal.Remarks;
                    }
                    Console.WriteLine(query);
                    query = "Update Animals Set Sold = 1 where Id = @Id";
                    SqlCommand sqlcmd3 = new SqlCommand(query, conn);
                    if(tran!=null){
                        sqlcmd3.Transaction = tran;
                    }
                    //sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                    sqlcmd3.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd3.Parameters["@Id"].Value = salePurchaseAnimal.AnimalId;
                    try
                    {
                        sqlcmd.ExecuteNonQuery();
                        sqlcmd3.ExecuteNonQuery();
                        if(tran != null){
                            tran.Commit();
                        }
                        conn.Close();
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Cow Sold successfully";
                        data2["status"] = "success";
                        data["data"] = data2;
                        return data;
                    }
                    catch (Exception ex)
                    {
                        if(tran != null){
                            tran.Rollback();
                        }
                        data["status"] = "failure";
                        data["message"] = "Connection failure" + ex.ToString() ;
                        data["exception"] = ex.StackTrace;
                        return data;
                    }
                }
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Empty Form";
                return data;
            }
        }
        public Dictionary<string,object> GetSalePurchaseById(long id){
            Dictionary<string,object> data = new Dictionary<string,object>();
            SalePurchaseAnimal? salePurchaseAnimal = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                
                string query = $"Select * from  AnimalSalePurchaseDetail where Id = @Id";
                Console.Write(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@Id"].Value = id;
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        salePurchaseAnimal = new SalePurchaseAnimal(sqlrdr);
                        data["status"] = "success";
                        data["salePurchaseAnimal"] = salePurchaseAnimal;
                    }else{
                        data["status"] = "failure";
                        data["message"] = "Record Donot Exists";
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    data["status"] = "failure";
                    data["message"] = "Record Not Found";
                    return data;
                }
            }
        }
        public Dictionary<string, object> GetSellCowDetailById(long id){
            SalePurchaseAnimal? salePurchaseAnimal = null;
            BuyerSellerModal buyerSellerDetail = null;
            Dictionary<string, object> data = new Dictionary<string,object>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                
                string query = $"Select * from  AnimalSalePurchaseDetail where Id = @Id";
                Console.Write(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@Id"].Value = id;
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        salePurchaseAnimal = new SalePurchaseAnimal(sqlrdr);
                        UsersContext userContext = new UsersContext(_configuration);
                        if(salePurchaseAnimal.SupervisorId!=null){
                            UserModel user = userContext.GetUserById((long)salePurchaseAnimal.SupervisorId);
                            salePurchaseAnimal.SupervisorName = user.GetDesignatedName();
                        }
                        data["salePurchaseAnimal"] = salePurchaseAnimal;
                        query = $"Select * from  BuyerSellerDetail where Id = @Id";
                        sqlcmd = new SqlCommand(query, conn);
                        sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@Id"].Value = salePurchaseAnimal.BuyerSellerId;
                        sqlrdr.Close();
                        sqlrdr = sqlcmd.ExecuteReader();
                        if(sqlrdr.Read()){
                            buyerSellerDetail = new BuyerSellerModal(sqlrdr);
                            data["buyerSellerDetail"] = buyerSellerDetail;
                        }
                    }else{
                        data["status"] = "failure";
                        data["message"] = "Record Donot Exists";
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    data["status"] = "failure";
                    data["message"] = "Record Not Found";
                    return data;
                }
            }
        }
        public Dictionary<string, object> GetSellCowDetailByCowId(long id){
            SalePurchaseAnimal? salePurchaseAnimal = null;
            BuyerSellerModal buyerSellerDetail = null;
            Dictionary<string, object> data = new Dictionary<string,object>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                
                string query = $"Select * from  AnimalSalePurchaseDetail where AnimalId = @AnimalId";
                Console.Write(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                sqlcmd.Parameters.Add("@AnimalId", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@AnimalId"].Value = id;
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        salePurchaseAnimal = new SalePurchaseAnimal(sqlrdr);
                        UsersContext userContext = new UsersContext(_configuration);
                        if(salePurchaseAnimal.SupervisorId!=null){
                            UserModel user = userContext.GetUserById((long)salePurchaseAnimal.SupervisorId);
                            salePurchaseAnimal.SupervisorName = user.GetDesignatedName();
                        }
                        data["salePurchaseAnimal"] = salePurchaseAnimal;
                        query = $"Select * from  BuyerSellerDetail where Id = @Id";
                        sqlcmd = new SqlCommand(query, conn);
                        sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@Id"].Value = salePurchaseAnimal.BuyerSellerId;
                        sqlrdr.Close();
                        sqlrdr = sqlcmd.ExecuteReader();
                        if(sqlrdr.Read()){
                            buyerSellerDetail = new BuyerSellerModal(sqlrdr);
                            data["buyerSellerDetail"] = buyerSellerDetail;
                            data["status"] = "success";
                        }
                    }else{
                        data["status"] = "failure";
                        data["message"] = "Record Donot Exists";
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return data;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    data["status"] = "failure";
                    data["message"] = "Record Not Found";
                    return data;
                }
            }
        }
        public Dictionary<string,object> EditSellCow(SalePurchaseAnimal salePurchaseAnimal, BuyerSellerModal buyerSellerModal){

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (salePurchaseAnimal !=null && buyerSellerModal!=null) //
            {
                Dictionary<string,object> dd = this.GetSalePurchaseById((long)salePurchaseAnimal.Id);
                if(dd["status"]=="success"){
                    SalePurchaseAnimal spA = (SalePurchaseAnimal)dd["salePurchaseAnimal"];
                    if(salePurchaseAnimal.BuyerSellerId ==null){
                        if(salePurchaseAnimal.ValidateSalePurchaseWithoudBuyerSellerId("Edit")==false || buyerSellerModal.ValidateBuyerSellerModal("Edit")==false){
                            //Dictionary<string,string> errors = Helper.MergeDictionary(salePurchaseAnimal.errors,buyerSellerModal.errors);
                            data["errors"] = salePurchaseAnimal.errors;
                            data["errors2"] = buyerSellerModal.errors;
                            data["status"] = false;
                            data["message"] = "Validation Error";
                            return data;
                        }
                    }else{
                        if(salePurchaseAnimal.ValidateSalePurchase("Add")==false){
                            data["errors"] = salePurchaseAnimal.errors;
                            data["errors2"] = null;
                            data["status"] = false;
                            data["message"] = "Validation Error";
                            return data;
                        }
                    }
                    string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                    SqlTransaction tran = null;
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = "UPDATE [dbo].[AnimalSalePurchaseDetail] SET [AnimalId] = @AnimalId,[Price] = @Price,[BuyerSellerId] = @BuyerSellerId,[Date] = @Date"+
                        ",[SupervisorId] = @SupervisorId,[SalePurchase] = @SalePurchase ,[Remarks] = @Remarks WHERE Id = @Id";
                        string query2="";
                        Console.WriteLine("BuyerSellerID"+salePurchaseAnimal.BuyerSellerId);
                        if(salePurchaseAnimal.BuyerSellerId==null){
                            Console.WriteLine("NULL");
                        }else{
                            query2 = "UPDATE [dbo].[BuyerSellerDetail]  SET [Name] = @Name,[Country] = @Country,[State] = @State,[District] = @District,[Vill_Mohalla] = @Vill_Mohalla,"+
                        "[StreetNo] = @StreetNo,[HouseNo] = @HouseNo,[PIN] = @PIN,[PhoneNumber] = @PhoneNumber,[Email] = @Email WHERE Id = @Id";
                            tran = conn.BeginTransaction("newSeller");
                            
                            tran.Save("save1");
                            SqlCommand sqlcmd2 = new SqlCommand(query2, conn);
                            sqlcmd2.Transaction = tran;
                            sqlcmd2.Parameters.Add("@Name", System.Data.SqlDbType.VarChar);
                            sqlcmd2.Parameters["@Name"].Value = buyerSellerModal.Name;
                            sqlcmd2.Parameters.Add("@Country", System.Data.SqlDbType.SmallInt);
                            sqlcmd2.Parameters["@Country"].Value = buyerSellerModal.Country;
                            sqlcmd2.Parameters.Add("@State", System.Data.SqlDbType.SmallInt);
                            sqlcmd2.Parameters["@State"].Value = buyerSellerModal.State;
                            sqlcmd2.Parameters.Add("@District", System.Data.SqlDbType.SmallInt);
                            sqlcmd2.Parameters["@District"].Value = buyerSellerModal.District;
                            sqlcmd2.Parameters.Add("@Vill_Mohalla", System.Data.SqlDbType.VarChar);
                            sqlcmd2.Parameters["@Vill_Mohalla"].Value = buyerSellerModal.VillMohalla;
                            sqlcmd2.Parameters.Add("@StreetNo", System.Data.SqlDbType.VarChar);
                            sqlcmd2.Parameters["@StreetNo"].Value = buyerSellerModal.StreetNo;
                            sqlcmd2.Parameters.Add("@HouseNo", System.Data.SqlDbType.VarChar);
                            sqlcmd2.Parameters["@HouseNo"].Value = buyerSellerModal.HouseNo;
                            sqlcmd2.Parameters.Add("@PIN", System.Data.SqlDbType.VarChar);
                            sqlcmd2.Parameters["@PIN"].Value = buyerSellerModal.PIN;
                            sqlcmd2.Parameters.Add("@PhoneNumber", System.Data.SqlDbType.VarChar);
                            sqlcmd2.Parameters["@PhoneNumber"].Value = buyerSellerModal.PhoneNumber;
                            sqlcmd2.Parameters.Add("@Email", System.Data.SqlDbType.VarChar);
                            sqlcmd2.Parameters["@Email"].Value = buyerSellerModal.Email;
                            sqlcmd2.Parameters.Add("@Id", System.Data.SqlDbType.VarChar);
                            sqlcmd2.Parameters["@Id"].Value = spA.BuyerSellerId;
                            Console.WriteLine(query2);
                            Console.WriteLine(buyerSellerModal.Name);
                            sqlcmd2.ExecuteNonQuery();
                            //salePurchaseAnimal.BuyerSellerId = (Int64) sqlcmd2.ExecuteScalar();
                        }
                        SqlCommand sqlcmd = new SqlCommand(query, conn);
                        if(tran!=null){
                            sqlcmd.Transaction = tran;
                        }
                        //sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters.Add("@AnimalId", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@AnimalId"].Value = salePurchaseAnimal.AnimalId;
                        sqlcmd.Parameters.Add("@Price", System.Data.SqlDbType.Decimal);
                        sqlcmd.Parameters["@Price"].Value = salePurchaseAnimal.Price;
                        sqlcmd.Parameters.Add("@BuyerSellerId", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@BuyerSellerId"].Value = spA.BuyerSellerId;
                        sqlcmd.Parameters.Add("@Date", System.Data.SqlDbType.Date);
                        sqlcmd.Parameters["@Date"].Value = salePurchaseAnimal.Date;
                        sqlcmd.Parameters.Add("@SupervisorId", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@SupervisorId"].Value = salePurchaseAnimal.SupervisorId;
                        sqlcmd.Parameters.Add("@SalePurchase", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@SalePurchase"].Value = "SELL";
                        sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@Id"].Value = salePurchaseAnimal.Id;
                        sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                        if(salePurchaseAnimal.Remarks!=null){
                            sqlcmd.Parameters["@Remarks"].Value = salePurchaseAnimal.Remarks;
                        }else{
                            sqlcmd.Parameters["@Remarks"].Value = "";
                        }
                        Console.WriteLine(query);
                        try
                        {
                            
                            sqlcmd.ExecuteNonQuery();
                            if(tran != null){
                                tran.Commit();
                                Console.WriteLine("COmmited");
                            }
                            conn.Close();
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow Sold successfully Updated";
                            data2["status"] = "success";
                            data["data"] = data2;
                            return data;
                        }
                        catch (Exception ex)
                        {
                            if(tran != null){
                                tran.Rollback();
                            }
                            data["status"] = "failure";
                            data["message"] = "Connection failure" + ex.ToString() ;
                            data["exception"] = ex.StackTrace;
                            return data;
                        }
                    }
                }else{
                    data["status"] = "failure";
                    data["message"] = "Record donot exists";    
                    return data;
                }
            }
            else
            {
                data["status"] = "failure";
                data["message"] = "Empty Form";
                return data;
            }
        }
    }
}
