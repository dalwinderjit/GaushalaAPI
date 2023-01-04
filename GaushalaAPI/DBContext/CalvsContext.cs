using GaushalaAPI.Models;
using GaushalAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class CalvsContext 
    {
        private readonly IConfiguration _configuration;
        public CalvsContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string,object> AddCalv(AnimalModel ani,SqlConnection? conn2=null,SqlTransaction? tran=null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            string image = null;
            if (true)//validate
            {
                
                ani.GenerateImageName(CowsContext.GetMaxAnimalId(this._configuration));
                Console.WriteLine("picture " + ani.Picture);
                Dictionary<string, string> errors = new Dictionary<string, string>();
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
                
                if(true)
                {
                    string cols = "[Category],[Gender],[TagNo],[Name],[Breed],[Lactation],[DOB],[DamID],[SireID],[Colour],[Weight],[Height],[BirthLactationNumber],[PregnancyStatus],[Status],[ReproductiveStatus],[MilkingStatus],[Remarks],[Location]";
                    string params_ = "@Category,@Gender,@tagNo,@name,@breed,0,@dob,@damID,@sireID,@colour,@weight,@height,@BirthLactationNumber,@PregnancyStatus,@Status,@ReproductiveStatus,@MilkingStatus,@Remarks,@Location";
                    if (ani.Picture != null && ani.Picture != "") {
                        cols += ",[Picture]";
                        params_ += ",@picture";
                    }
                    string query = $"INSERT into [dbo].[Animals] ({cols}) OUTPUT INSERTED.ID values({params_});";
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    if (tran != null)
                    {
                        tran.Save("save2");
                        sqlcmd.Transaction = tran;
                    }
                    //sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters.Add("@tagNo", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters.Add("@name", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters.Add("@Category", System.Data.SqlDbType.VarChar);

                    sqlcmd.Parameters["@tagNo"].Value = ani.TagNo;
                    sqlcmd.Parameters["@name"].Value = ani.Name;
                    sqlcmd.Parameters["@Category"].Value = ani.Category;

                    if (ani.Breed != null || true)
                    {
                        sqlcmd.Parameters.Add("@breed", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@breed"].Value = ani.Breed;
                    }
                    
                    if (ani.DOB != null || true)
                    {
                        sqlcmd.Parameters.Add("@dob", System.Data.SqlDbType.DateTime);
                        // sqlcmd.Parameters["@dob"].Value = ((DateTime)ani.DOB).ToString("yyyy-MM-dd 00:00:00");
                        sqlcmd.Parameters["@dob"].Value = ani.DOB;
                    }
                    if (ani.DamID != null || true)
                    {
                        sqlcmd.Parameters.Add("@damID", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@damID"].Value = ani.DamID;
                    }
                    if (ani.SireID != null || true)
                    {
                        sqlcmd.Parameters.Add("@sireID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@sireID"].Value = ani.SireID;
                    }
                    if (ani.Gender != null || true)
                    {
                        sqlcmd.Parameters.Add("@Gender", System.Data.SqlDbType.VarChar );
                        sqlcmd.Parameters["@Gender"].Value = ani.Gender;
                    }
                    if (ani.Colour != null || true)
                    {
                        sqlcmd.Parameters.Add("@colour", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@colour"].Value = ani.Colour;
                    }
                    if (ani.Weight != null || true)
                    {
                        sqlcmd.Parameters.Add("@weight", System.Data.SqlDbType.Decimal);
                        sqlcmd.Parameters["@weight"].Value = ani.Weight;
                    }
                    if (ani.Height != null || true)
                    {
                        sqlcmd.Parameters.Add("@height", System.Data.SqlDbType.Decimal);
                        sqlcmd.Parameters["@height"].Value = ani.Height;
                    }
                    if (ani.Picture != null && ani.Picture != "")
                    {
                        sqlcmd.Parameters.Add("@picture", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@picture"].Value = ani.Picture;
                    }
                    if (ani.BirthLactationNumber!= null )
                    {
                        sqlcmd.Parameters.Add("@BirthLactationNumber", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@BirthLactationNumber"].Value = ani.BirthLactationNumber;
                    }
                    if (ani.Location!= null )
                    {
                        sqlcmd.Parameters.Add("@Location", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@Location"].Value = ani.Location;
                    }
                    sqlcmd.Parameters.Add("@PregnancyStatus", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@PregnancyStatus"].Value = 0;
                    sqlcmd.Parameters.Add("@Status", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Status"].Value = 1;
                    sqlcmd.Parameters.Add("@ReproductiveStatus", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@ReproductiveStatus"].Value = 0;
                    sqlcmd.Parameters.Add("@MilkingStatus", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@MilkingStatus"].Value = 0;
                    sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Remarks"].Value = ani.Remarks;
                    try
                    {
                        Console.WriteLine("HI");
                       
                        if (tran == null)
                        {
                             conn.Open();
                            //tran = conn.BeginTransaction("AddCalv");
                            //tran.Save("save2");
                        }
                        else
                        {
                            sqlcmd.Transaction = tran;
                            tran.Save("save2");
                        }
                        ani.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        
                        data2["message"] = $"{ani.Category} Added successfully ID:" + ani.Id;
                        data2["status"] = "success";
                        data2["animalID"] = ""+ani.Id;
                        data["data"] = data2;
                        if (tran != null)
                        {
                            data["tran"] = tran;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Savin Failed");
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Animal Save Failed";
                        data2["status"] = "Failure";
                        data["data"] = data2;
                        if (tran != null)
                        {
                            data["tran"] = tran;
                        }
                    }

                }
                return data;
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Animal Save Failed Validations Failed";
                data2["status"] = "Failure";
                data["data"] = data2;
                return data;
            }
        }
        internal Dictionary<string, object> UpdateCalv(AnimalModel ani,SqlConnection? conn2=null, SqlTransaction? tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            string image = null;
            if (true)//validate
            {
                ani.GenerateImageName(CowsContext.GetMaxAnimalId(this._configuration));
                Console.WriteLine("picture " + ani.Picture);
                Dictionary<string, string> errors = new Dictionary<string, string>();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                SqlConnection conn;
                if (conn2 != null && tran!=null)
                {
                    conn = conn2;
                }
                else
                {
                    conn = new SqlConnection(connectionString);
                }
                if(true)
                {
                    string cols = "Category = case when @Gender = 'Male' then (case when Category = 'BULL' or Category = 'COW' then 'BULL' when Category = 'CALF' or Category = 'HEIFER' then 'CALF' else 'CALF' end)"+
                        "when @Gender = 'Female' then(case when Category = 'BULL' or Category = 'COW' then 'COW' when Category = 'CALF' or Category = 'HEIFER' then 'HEIFER' else 'HEIFER' end) end," +
                        "[Gender] = @Gender,[TagNo] = @TagNo,[Name] = @Name,[Breed]=@Breed,[DOB] = @DOB," +
                        "[DamID] = @DamID,[SireID] =@SireID,[BirthLactationNumber] = @BirthLactationNumber,[Location]=@Location";
                    if (ani.Picture != null && ani.Picture != "")
                    {
                        cols += ",[Picture] = @picture";
                    }
                    string query = $"Update [dbo].[Animals] set {cols} where Id = @Id";
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    if (tran != null)
                    {
                        tran.Save("save2");
                        sqlcmd.Transaction = tran;
                    }
                    //sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters.Add("@TagNo", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@TagNo"].Value = ani.TagNo;
                    sqlcmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Name"].Value = ani.Name;
                    sqlcmd.Parameters.Add("@Category", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Category"].Value = ani.Category;
                    sqlcmd.Parameters.Add("@Breed", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Breed"].Value = ani.Breed;
                    sqlcmd.Parameters.Add("@DOB", System.Data.SqlDbType.DateTime);
                    sqlcmd.Parameters["@DOB"].Value = ani.DOB;
                    sqlcmd.Parameters.Add("@DamID", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@DamID"].Value = ani.DamID;
                    sqlcmd.Parameters.Add("@SireID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@SireID"].Value = ani.SireID;
                    sqlcmd.Parameters.Add("@Gender", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Gender"].Value = ani.Gender;
                    /*sqlcmd.Parameters.Add("@Colour", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Colour"].Value = ani.Colour;
                    sqlcmd.Parameters.Add("@Weight", System.Data.SqlDbType.Decimal);
                    sqlcmd.Parameters["@Weight"].Value = ani.Weight;
                    sqlcmd.Parameters.Add("@Height", System.Data.SqlDbType.Decimal);
                    sqlcmd.Parameters["@Height"].Value = ani.Height;*/
                    if (ani.Picture != null && ani.Picture != "")
                    {
                        sqlcmd.Parameters.Add("@Picture", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@Picture"].Value = ani.Picture;
                    }
                    sqlcmd.Parameters.Add("@BirthLactationNumber", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@BirthLactationNumber"].Value = ani.BirthLactationNumber;
                    sqlcmd.Parameters.Add("@Location", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Location"].Value = ani.Location;
                    /*sqlcmd.Parameters.Add("@Remarks", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Remarks"].Value = ani.Remarks;*/
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Id"].Value = ani.Id;
                    try
                    {
                        Console.WriteLine("HI");
                        if (tran == null || conn2 == null)
                        {
                            conn.Open();
                            //tran = conn.BeginTransaction("AddCalv");
                            //tran.Save("save2");
                        }
                        else
                        {
                            sqlcmd.Transaction = tran;
                            tran.Save("save2");
                        }
                        sqlcmd.ExecuteNonQuery();
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = $"{ani.Category} Updated successfully ID:" + ani.Id;
                        data2["status"] = "success";
                        data2["animalID"] = "" + ani.Id;
                        data["data"] = data2;
                        if (tran != null && conn2!=null)
                        {
                            data["tran"] = tran;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Savin Failed");
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = $"{ani.Category} Updation Failed";
                        data2["status"] = "failure";
                        data["data"] = data2;
                        if (tran != null && conn2!=null)
                        {
                            data["tran"] = tran;
                        }
                    }
                }
                if (tran == null || conn2 == null)
                {
                    conn.Close();
                }
                return data;
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                string category = "";
                if (ani.Category!=null && ani.Category.Trim()!=""){
                    category =  ani.Category;
                }else
                {
                    category = "Child";
                }
                data2["message"] = $"{category} Save Failed Validations Failed";
                data2["status"] = "failure";
                data["data"] = data2;
                return data;
            }
        }
        internal Dictionary<string, object> GetCalvDetailById(long id)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> calvDetail = new Dictionary<string, object>();
            CowModel cowModel = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "Select Animals.*,CowConceiveData.Id as conceiveId,CowConceiveData.DateOfService,CowConceiveData.DeliveryDate,CowConceiveData.DeliveryStatus,CowConceiveData.PregnancyStatus as PregnancyStatus_," +
                        "CowConceiveData.DamWeight,CowConceiveData.MatingProcessType,CowConceiveData.BirthWeight,CowConceiveData.Remarks,CowConceiveData.DoctorID from Animals " +
                        "left Join CowConceiveData on CowConceiveData.AnimalId = Animals.Id where Animals.Id= @Id";
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                sqlcmd.Parameters["@ID"].Value = id;
                try
                {
                    CowsContext cow_context = new CowsContext(this._configuration);
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        Dictionary<string, string> cowData = cow_context.GetCowTagNoNameById(Convert.ToInt64(sqlrdr["DamID"]));
                        Dictionary<string, string> bullData = cow_context.GetCowTagNoNameById(Convert.ToInt64(sqlrdr["SireID"]));
                        string cowName = cowData["name"];
                        string TagNo = cowData["tagNo"];
                        calvDetail["id"] = Helper.IsNullOrEmpty(sqlrdr["Id"]);
                        calvDetail["conceiveId"] = Helper.IsNullOrEmpty(sqlrdr["conceiveId"]);
                        calvDetail["dateOfService"] = Helper.IsNullOrEmpty(sqlrdr["DateOfService"]);
                        calvDetail["damID"] = Helper.IsNullOrEmpty(sqlrdr["DamID"]);
                        calvDetail["sireID"] = Helper.IsNullOrEmpty(sqlrdr["SireID"]);
                        calvDetail["cowName"] =  Helper.IsNullOrEmpty(cowData["name"]);
                        calvDetail["cowTagNo"] =  Helper.IsNullOrEmpty(cowData["tagNo"]);
                        calvDetail["bullsName"] =  Helper.IsNullOrEmpty(bullData["name"]);
                        calvDetail["bullsTagNo"] =  Helper.IsNullOrEmpty(bullData["tagNo"]);
                        calvDetail["pregnancyStatus"] =  Helper.IsNullOrEmpty(sqlrdr["PregnancyStatus_"]);  
                        calvDetail["deliveryStatus"] =  Helper.IsNullOrEmpty(sqlrdr["DeliveryStatus"]);
                        calvDetail["birthLactationNumber"] =  Helper.IsNullOrEmpty(sqlrdr["BirthLactationNumber"]);
                        calvDetail["deliveryDate"] =  Helper.IsNullOrEmpty(sqlrdr["DeliveryDate"]);
                        calvDetail["DOB"] =  Helper.IsNullOrEmpty(sqlrdr["DOB"]);
                        calvDetail["birthWeight"] =  Helper.IsNullOrEmpty(sqlrdr["BirthWeight"]);
                        calvDetail["birthHeight"] =  Helper.IsNullOrEmpty(sqlrdr["Height"]);
                        calvDetail["damWeight"] =  Helper.IsNullOrEmpty(sqlrdr["DamWeight"]);
                        calvDetail["tagNo"] =  Helper.IsNullOrEmpty(sqlrdr["TagNo"]);
                        calvDetail["name"] =  Helper.IsNullOrEmpty(sqlrdr["Name"]);
                        calvDetail["colour"] =  Helper.IsNullOrEmpty(sqlrdr["Colour"]);
                        calvDetail["breed"] =  Helper.IsNullOrEmpty(sqlrdr["Breed"]);
                        calvDetail["gender"] =  Helper.IsNullOrEmpty(sqlrdr["Gender"]);
                        calvDetail["doctorID"] =  Helper.IsNullOrEmpty(sqlrdr["DoctorID"]);
                        calvDetail["matingProcessType"] =  Helper.IsNullOrEmpty(sqlrdr["MatingProcessType"]);
                        calvDetail["remarks"] =  Helper.IsNullOrEmpty(sqlrdr["Remarks"]);
                        calvDetail["picture"] =  Helper.IsNullOrEmpty(sqlrdr["Picture"]);
                        calvDetail["location"] =  Helper.IsNullOrEmpty(sqlrdr["Location"]);
                        data["status"] = "success";
                        data["data"] = calvDetail;
                        data["message"] = "Calv Detail Exists";
                        if(calvDetail["doctorID"].ToString().Trim()!=""){
                            UsersContext users = new UsersContext(this._configuration);
                            List<long> doctor_ids = new List<long>();
                            doctor_ids.Add(Convert.ToInt64(calvDetail["doctorID"]));
                            Dictionary<long, string> doctorsIdName = users.GetDoctorsIdNameByIds(doctor_ids);
                            try
                            {
                                calvDetail["doctorsName"] = doctorsIdName[Convert.ToInt64(calvDetail["doctorID"])];
                            }catch(Exception e)
                            {
                                calvDetail["doctorsName"] = "";
                            }
                        }else{
                            calvDetail["doctorsName"] = "";
                        }
                        
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return data;
                }
                catch (Exception ex)
                {
                    data["status"] = "failure";
                    data["message"] = "Calv Detail Do not Exists"+ex.StackTrace;
                    return data;
                }
            }
        }
    }
}