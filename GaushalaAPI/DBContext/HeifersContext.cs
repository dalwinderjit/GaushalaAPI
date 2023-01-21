using GaushalaAPI.Models;
using GaushalAPI.Models;
using GaushalAPI.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class HeifersContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        public HeifersContext(IConfiguration configuration) :base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string,object> AddHeifer(AnimalModel ani,SqlConnection? conn2=null,SqlTransaction? tran=null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //this.Testing(ani);
            //return data;
            string image = null;
            ani.Category = "HEIFER";
            ani.Gender= "FEMALE";
            ani.Lactation = 0;
            //ani.BirthLactationNumber = 1;
            ani.GenerateImageName(CowsContext.GetMaxAnimalId(this._configuration));
            Console.WriteLine("picture " + ani.Picture);
            bool addSire = false;
            bool addDam = false;
            if (ani.DamID == null)
            {
                addDam = true;
                Console.WriteLine("Need to add Dam");
            }
            if (ani.SireID == null)
            {
                Console.WriteLine("Need to add Sire");
                addSire = true;
            }
            data = base.AddAnimal(ani,addDam,addSire);
            if (((Dictionary<string,string>)data["data"])["status"] == "success") {
                ani.SaveImage2();
            }
            Console.WriteLine("Returnign again");
            return data;
        }
        internal Dictionary<string, object> UpdateHeifer(AnimalModel ani,SqlConnection? conn2=null, SqlTransaction? tran = null)
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
                            //tran = conn.BeginTransaction("AddHeifer");
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
        internal Dictionary<string, object> GetHeiferDetailById(long id)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, object> HeiferDetail = new Dictionary<string, object>();
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
                        HeiferDetail["id"] = Helper.IsNullOrEmpty(sqlrdr["Id"]);
                        HeiferDetail["conceiveId"] = Helper.IsNullOrEmpty(sqlrdr["conceiveId"]);
                        HeiferDetail["dateOfService"] = Helper.IsNullOrEmpty(sqlrdr["DateOfService"]);
                        HeiferDetail["damID"] = Helper.IsNullOrEmpty(sqlrdr["DamID"]);
                        HeiferDetail["sireID"] = Helper.IsNullOrEmpty(sqlrdr["SireID"]);
                        HeiferDetail["cowName"] =  Helper.IsNullOrEmpty(cowData["name"]);
                        HeiferDetail["cowTagNo"] =  Helper.IsNullOrEmpty(cowData["tagNo"]);
                        HeiferDetail["bullsName"] =  Helper.IsNullOrEmpty(bullData["name"]);
                        HeiferDetail["bullsTagNo"] =  Helper.IsNullOrEmpty(bullData["tagNo"]);
                        HeiferDetail["pregnancyStatus"] =  Helper.IsNullOrEmpty(sqlrdr["PregnancyStatus_"]);  
                        HeiferDetail["deliveryStatus"] =  Helper.IsNullOrEmpty(sqlrdr["DeliveryStatus"]);
                        HeiferDetail["birthLactationNumber"] =  Helper.IsNullOrEmpty(sqlrdr["BirthLactationNumber"]);
                        HeiferDetail["deliveryDate"] =  Helper.IsNullOrEmpty(sqlrdr["DeliveryDate"]);
                        HeiferDetail["DOB"] =  Helper.IsNullOrEmpty(sqlrdr["DOB"]);
                        HeiferDetail["birthWeight"] =  Helper.IsNullOrEmpty(sqlrdr["BirthWeight"]);
                        HeiferDetail["birthHeight"] =  Helper.IsNullOrEmpty(sqlrdr["Height"]);
                        HeiferDetail["damWeight"] =  Helper.IsNullOrEmpty(sqlrdr["DamWeight"]);
                        HeiferDetail["tagNo"] =  Helper.IsNullOrEmpty(sqlrdr["TagNo"]);
                        HeiferDetail["name"] =  Helper.IsNullOrEmpty(sqlrdr["Name"]);
                        HeiferDetail["colour"] =  Helper.IsNullOrEmpty(sqlrdr["Colour"]);
                        HeiferDetail["breed"] =  Helper.IsNullOrEmpty(sqlrdr["Breed"]);
                        HeiferDetail["gender"] =  Helper.IsNullOrEmpty(sqlrdr["Gender"]);
                        HeiferDetail["doctorID"] =  Helper.IsNullOrEmpty(sqlrdr["DoctorID"]);
                        HeiferDetail["matingProcessType"] =  Helper.IsNullOrEmpty(sqlrdr["MatingProcessType"]);
                        HeiferDetail["remarks"] =  Helper.IsNullOrEmpty(sqlrdr["Remarks"]);
                        HeiferDetail["picture"] =  Helper.IsNullOrEmpty(sqlrdr["Picture"]);
                        HeiferDetail["location"] =  Helper.IsNullOrEmpty(sqlrdr["Location"]);
                        data["status"] = "success";
                        data["data"] = HeiferDetail;
                        data["message"] = "Heifer Detail Exists";
                        if(HeiferDetail["doctorID"].ToString().Trim()!=""){
                            UsersContext users = new UsersContext(this._configuration);
                            List<long> doctor_ids = new List<long>();
                            doctor_ids.Add(Convert.ToInt64(HeiferDetail["doctorID"]));
                            Dictionary<long, string> doctorsIdName = users.GetDoctorsIdNameByIds(doctor_ids);
                            try
                            {
                                HeiferDetail["doctorsName"] = doctorsIdName[Convert.ToInt64(HeiferDetail["doctorID"])];
                            }catch(Exception e)
                            {
                                HeiferDetail["doctorsName"] = "";
                            }
                        }else{
                            HeiferDetail["doctorsName"] = "";
                        }
                        
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return data;
                }
                catch (Exception ex)
                {
                    data["status"] = "failure";
                    data["message"] = "Heifer Detail Do not Exists"+ex.StackTrace;
                    return data;
                }
            }
        }
        internal Dictionary<long, object> GetGetHeifersIDNamePairByTagNo(string tagNo, int pageNo, int recordsPerPage)
        {
            //Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<long, object> heifers = new Dictionary<long, object>();
            AnimalFilter animalFilter = new AnimalFilter();
            animalFilter.TagNo = "%"+tagNo+"%";
            animalFilter.PageNo = pageNo;
            animalFilter.RecordsPerPage = recordsPerPage;
            animalFilter.Category = "HEIFER";
            animalFilter.OrderBy = "TagNo";
            animalFilter.Order = "ASC";
            animalFilter.GetCategory = false;
            heifers = base.GetAnimalsIDNameTagNoPair(animalFilter);
            //data["data"] = heifers;
            return heifers;
        }
    }
}