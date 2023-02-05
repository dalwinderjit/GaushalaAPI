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
    public class CalvsContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        //public CalvsContext(IConfiguration configuration)
        //{
        //   _configuration = configuration;
        //}
        public CalvsContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
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
                        "left Join CowConceiveData on CowConceiveData.AnimalId = Animals.Id where Animals.Id= @Id and Animals.Category='CALF'";
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
                        calvDetail["cowName"] = Helper.IsNullOrEmpty(cowData["name"]);
                        calvDetail["cowTagNo"] = Helper.IsNullOrEmpty(cowData["tagNo"]);
                        calvDetail["bullsName"] = Helper.IsNullOrEmpty(bullData["name"]);
                        calvDetail["bullsTagNo"] = Helper.IsNullOrEmpty(bullData["tagNo"]);
                        calvDetail["pregnancyStatus"] = Helper.IsNullOrEmpty(sqlrdr["PregnancyStatus_"]);
                        calvDetail["deliveryStatus"] = Helper.IsNullOrEmpty(sqlrdr["DeliveryStatus"]);
                        calvDetail["birthLactationNumber"] = Helper.IsNullOrEmpty(sqlrdr["BirthLactationNumber"]);
                        calvDetail["deliveryDate"] = Helper.IsNullOrEmpty(sqlrdr["DeliveryDate"]);
                        calvDetail["DOB"] = Helper.IsNullOrEmpty(sqlrdr["DOB"]);
                        calvDetail["birthWeight"] = Helper.IsNullOrEmpty(sqlrdr["BirthWeight"]);
                        calvDetail["birthHeight"] = Helper.IsNullOrEmpty(sqlrdr["Height"]);
                        calvDetail["damWeight"] = Helper.IsNullOrEmpty(sqlrdr["DamWeight"]);
                        calvDetail["tagNo"] = Helper.IsNullOrEmpty(sqlrdr["TagNo"]);
                        calvDetail["name"] = Helper.IsNullOrEmpty(sqlrdr["Name"]);
                        calvDetail["colour"] = Helper.IsNullOrEmpty(sqlrdr["Colour"]);
                        calvDetail["breed"] = Helper.IsNullOrEmpty(sqlrdr["Breed"]);
                        calvDetail["gender"] = Helper.IsNullOrEmpty(sqlrdr["Gender"]);
                        calvDetail["doctorID"] = Helper.IsNullOrEmpty(sqlrdr["DoctorID"]);
                        calvDetail["matingProcessType"] = Helper.IsNullOrEmpty(sqlrdr["MatingProcessType"]);
                        calvDetail["remarks"] = Helper.IsNullOrEmpty(sqlrdr["Remarks"]);
                        calvDetail["picture"] = Helper.IsNullOrEmpty(sqlrdr["Picture"]);
                        calvDetail["location"] = Helper.IsNullOrEmpty(sqlrdr["Location"]);
                        data["status"] = "success";
                        data["data"] = calvDetail;
                        data["message"] = "Calv Detail Exists";
                        if (calvDetail["doctorID"].ToString().Trim() != "")
                        {
                            UsersContext users = new UsersContext(this._configuration);
                            List<long> doctor_ids = new List<long>();
                            doctor_ids.Add(Convert.ToInt64(calvDetail["doctorID"]));
                            Dictionary<long, string> doctorsIdName = users.GetDoctorsIdNameByIds(doctor_ids);
                            try
                            {
                                calvDetail["doctorsName"] = doctorsIdName[Convert.ToInt64(calvDetail["doctorID"])];
                            }
                            catch (Exception e)
                            {
                                calvDetail["doctorsName"] = "";
                            }
                        }
                        else
                        {
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
                    data["message"] = "Calv Detail Do not Exists" + ex.StackTrace;
                    return data;
                }
            }
        }
        public Dictionary<long, object> GetCalvDetailByIdTagNo(string tagNo, int? pageno = 1,int? recordsPerPage=10)
        {
            Dictionary<long, object> cows = new Dictionary<long, object>();
            AnimalFilter animalFilter = new AnimalFilter();
            animalFilter.TagNo = "%" + tagNo + "%";
            if (pageno != null)
            {
                animalFilter.PageNo = (int)pageno;
            }
            if (recordsPerPage != null)
            {
                animalFilter.RecordsPerPage = (int)recordsPerPage;
            }
            animalFilter.Category = "CALF";
            animalFilter.OrderBy = "TagNo";
            animalFilter.Order = "ASC";
            animalFilter.GetCategory = false;
            cows = base.GetAnimalsIDNameTagNoPair(animalFilter);
            return cows;
        }
        internal Dictionary<string, object> AddCalv(AnimalModel ani, SqlConnection? conn2 = null, SqlTransaction? tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //this.Testing(ani);
            //return data;
            string image = null;
            ani.Category = "CALF";//"HEIFER";
            ani.Gender = "Male";
            ani.Lactation = 0;
            ani.BelongsToGaushala = true;
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
            data = base.AddAnimal(ani, addDam, addSire);
            if (((Dictionary<string, string>)data["data"])["status"] == "success")
            {
                ani.SaveImage2();
            }
            Console.WriteLine("Returnign again");
            return data;
        }

        internal Dictionary<string, object> UpdateCalv(AnimalModel ani, SqlConnection? conn2 = null, SqlTransaction? tran = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            //this.Testing(ani);
            //return data;
            string image = null;
            ani.Category = "CALF";
            ani.Gender = "Male";
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
            data = base.UpdateAnimal(ani, addDam, addSire);
            if (((Dictionary<string, string>)data["data"])["status"] == "success")
            {
                ani.SaveImage2();
            }
            Console.WriteLine("Returnign again");
            return data;
        }






    }
}