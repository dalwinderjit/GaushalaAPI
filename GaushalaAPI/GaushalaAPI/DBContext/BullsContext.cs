using GaushalaAPI.Models;
using GaushalAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class BullsContext 
    {
        private readonly IConfiguration _configuration;
        public BullsContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Dictionary<long, string> GetBullsNamesByIds(List<long> bulls)
        {
            Dictionary<long, string> bullsIdName = new Dictionary<long, string>();
            string ids = "";
            foreach(var m in bulls)
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
                    string query = $"Select Id,TagNo from Animals where Category = 'Bull' and Id in ({ids})";
                    Console.Write(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    try
                    {
                        Console.WriteLine("HI");
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        while (sqlrdr.Read())
                        {
                            bullsIdName[Convert.ToInt64(sqlrdr["Id"])] = sqlrdr["TagNo"].ToString();
                        }
                        sqlrdr.Close();
                        conn.Close();
                        return bullsIdName;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return bullsIdName;
                    }
                }
            }
            return bullsIdName;

        }
        public static Dictionary<long, Dictionary<string, string>> GetBullsTagNoNamesByIds(IConfiguration _configuration,List<long> bulls)
        {
            
            Dictionary<long, Dictionary<string, string>> bullsIdName = new Dictionary<long, Dictionary<string, string>>();
            string ids = "";
            foreach (var m in bulls)
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
                    string query = $"Select Id,Name,TagNo from Animals where Category = 'BULL' and Id in ({ids})";
                    Console.WriteLine(query);
                    Console.Write(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    try
                    {
                        Console.WriteLine("HI");
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        while (sqlrdr.Read())
                        {
                            Dictionary<string, string> data = new Dictionary<string, string>();
                            data["Name"] = sqlrdr["Name"].ToString();
                            data["TagNo"] = sqlrdr["TagNo"].ToString();
                            bullsIdName[Convert.ToInt64(sqlrdr["Id"])] = data;
                        }
                        sqlrdr.Close();
                        conn.Close();
                        return bullsIdName;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return bullsIdName;
                    }
                }
            }
            return bullsIdName;

        }
        internal Dictionary<string,Dictionary<string,string>> SaveBull(BullModel bull)
        {
            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
            Console.WriteLine(bull.Id);
            if (bull.ValidateBull(this,"Add") == true)
            {
                string image = null;
                if (bull.formFile != null)
                {
                    Console.WriteLine("Image Fetched");
                    bull.Picture = bull.SaveImage();
                    if (bull.Picture == "")
                    {
                        ///Failed
                    }
                    Console.WriteLine(bull.Picture);
                }
                //validate data over here
                Console.WriteLine($"ID={bull.Id};");
                if (bull.Id == null || bull.Id == 0)
                {
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Bull Save Failed";
                    data2["status"] = "Failure";
                    data["data"] = data2;
                }
                else
                {
                    //bull.DOB = null;
                    //bull.Weight = null;
                    //Console.WriteLine(bull.Weight);
                    //bull.Height = null;
                    //bull.ButterFat = null;
                    //bull.Lactation = null;
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
                        if (bull.Breed != null)
                        {
                            query += ",[Breed] = @breed";
                        }
                        
                        if (bull.DOB != null)
                        {
                            query += ",[DOB] = @dob";
                        }
                        if (bull.DamID != null)
                        {
                            query += ",[DamID] = @damID";
                        }
                        if (bull.SireID != null)
                        {
                            query += ",[SireID] = @sireID";
                        }
                        if (bull.Colour != null)
                        {
                            query += ",[Colour] = @colour";
                        }
                        if (bull.Weight != null)
                        {
                            query += ",[Weight] = @weight";
                        }
                        if (bull.Height != null)
                        {
                            query += ",[Height] = @height";
                        }
                        if(bull.SemenDoses !=null){
                            query += ",[SemenDoses] = @semenDoses";
                        }
                        if(bull.Type !=null){
                            query += ",[Type] = @type";
                        }
                        
                        if (bull.Location != null)
                        {
                            query += ",[Location] = @location";
                        }
                        if (bull.Remarks != null)
                        {
                            query += ",[Remarks] = @remarks";
                        }
                        if (bull.Picture != null && bull.Picture != "")
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
                        sqlcmd.Parameters["@ID"].Value = bull.Id;
                        sqlcmd.Parameters["@tagNo"].Value = bull.TagNo;
                        sqlcmd.Parameters["@name"].Value = bull.Name;
                        //sqlcmd.Parameters["@category"].Value = bull.Category;
                        if (bull.Breed != null)
                        {
                            sqlcmd.Parameters.Add("@breed", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@breed"].Value = bull.Breed;
                        }
                        
                        if (bull.DOB != null)
                        {
                            sqlcmd.Parameters.Add("@dob", System.Data.SqlDbType.DateTime);
                            // sqlcmd.Parameters["@dob"].Value = ((DateTime)bull.DOB).ToString("yyyy-MM-dd 00:00:00");
                            sqlcmd.Parameters["@dob"].Value = bull.DOB;
                        }
                        if (bull.DamID != null)
                        {
                            sqlcmd.Parameters.Add("@damID", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@damID"].Value = bull.DamID;
                        }
                        if (bull.SireID != null)
                        {
                            sqlcmd.Parameters.Add("@sireID", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@sireID"].Value = bull.SireID;
                        }
                        if (bull.Colour != null)
                        {
                            sqlcmd.Parameters.Add("@colour", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@colour"].Value = bull.Colour;
                        }
                        if (bull.Weight != null)
                        {
                            sqlcmd.Parameters.Add("@weight", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@weight"].Value = bull.Weight;
                        }
                        if (bull.Height != null)
                        {
                            sqlcmd.Parameters.Add("@height", System.Data.SqlDbType.Decimal);
                            sqlcmd.Parameters["@height"].Value = bull.Height;
                        }
                        if (bull.Picture != null && bull.Picture != "")
                        {
                            sqlcmd.Parameters.Add("@picture", System.Data.SqlDbType.VarChar);
                            sqlcmd.Parameters["@picture"].Value = bull.Picture;
                        }
                        if (bull.Location != null)
                        {
                            sqlcmd.Parameters.Add("@location", System.Data.SqlDbType.SmallInt);
                            sqlcmd.Parameters["@location"].Value = bull.Location;
                        }
                        if (bull.Remarks != null)
                        {
                            sqlcmd.Parameters.Add("@remarks", System.Data.SqlDbType.VarChar);
                            sqlcmd.Parameters["@remarks"].Value = bull.Remarks;
                        }
                        if(bull.SemenDoses !=null){
                            sqlcmd.Parameters.Add("@semenDoses", System.Data.SqlDbType.Int);
                            sqlcmd.Parameters["@semenDoses"].Value = bull.SemenDoses;
                        }
                        if(bull.Type !=null){
                            sqlcmd.Parameters.Add("@type", System.Data.SqlDbType.VarChar);
                            sqlcmd.Parameters["@type"].Value = bull.Type;
                        }
                        try
                        {
                            Console.WriteLine("HI");
                            conn.Open();
                            sqlcmd.ExecuteNonQuery();
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Bull Saved successfully";
                            data2["status"] = "Success";
                            data["data"] = data2;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Console.WriteLine("Savin Failed");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Bull Save Failed";
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
                data2["message"] = "Bull Save Failed validation errror";
                data2["status"] = "Failure";
                data["data"] = data2;
                data["errors"] = bull.errors;
                return data;
            }
        }
        internal Dictionary<string, Dictionary<string, string>> AddBull(BullModel bull)
        {
            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
            string image = null;
            if (bull.ValidateBull(this,"Add") == true)
            {
                bull.GenerateImageName(CowsContext.GetMaxAnimalId(_configuration));
                Dictionary<string, string> errors = new Dictionary<string, string>();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    System.Console.WriteLine("HELLO");
                    string query = "INSERT into [dbo].[Animals] ([Category],[Gender],[TagNo],[Name],[Breed],[DOB],[DamID],[SireID],[Colour],[Weight],[Height],[Picture],[Type],[SemenDoses],[Location],[Remarks])"
                    + " OUTPUT INSERTED.ID values('BULL','Male',@tagNo,@name,@breed,@dob,@damID,@sireID,@colour,@weight,@height,@picture,@type,@semenDoses,@location,@remarks);";//set [TagNo] = @tagNo,[Name] = @name";
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    //sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters.Add("@tagNo", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters.Add("@name", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters.Add("@category", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@ID"].Value = bull.Id;
                    sqlcmd.Parameters["@tagNo"].Value = bull.TagNo;
                    sqlcmd.Parameters["@name"].Value = bull.Name;

                    //sqlcmd.Parameters["@category"].Value = bull.Category;
                    if (bull.Breed != null || true)
                    {
                        sqlcmd.Parameters.Add("@breed", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@breed"].Value = bull.Breed;
                    }
                    if (bull.DOB != null || true)
                    {
                        sqlcmd.Parameters.Add("@dob", System.Data.SqlDbType.DateTime);
                        // sqlcmd.Parameters["@dob"].Value = ((DateTime)bull.DOB).ToString("yyyy-MM-dd 00:00:00");
                        sqlcmd.Parameters["@dob"].Value = bull.DOB;
                    }
                    if (bull.DamID != null || true)
                    {
                        sqlcmd.Parameters.Add("@damID", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@damID"].Value = bull.DamID;
                    }
                    if (bull.SireID != null || true)
                    {
                        sqlcmd.Parameters.Add("@sireID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@sireID"].Value = bull.SireID;
                    }
                    if (bull.Colour != null || true)
                    {
                        sqlcmd.Parameters.Add("@colour", System.Data.SqlDbType.Int);
                        sqlcmd.Parameters["@colour"].Value = bull.Colour;
                    }
                    if (bull.Weight != null || true)
                    {
                        sqlcmd.Parameters.Add("@weight", System.Data.SqlDbType.Decimal);
                        sqlcmd.Parameters["@weight"].Value = bull.Weight;
                    }
                    if (bull.Height != null || true)
                    {
                        sqlcmd.Parameters.Add("@height", System.Data.SqlDbType.Decimal);
                        sqlcmd.Parameters["@height"].Value = bull.Height;
                    }
                    if (bull.Picture != null && bull.Picture != "" || true)
                    {
                        sqlcmd.Parameters.Add("@picture", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@picture"].Value = bull.Picture;
                    }
                    sqlcmd.Parameters.Add("@semenDoses", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@semenDoses"].Value = bull.SemenDoses;

                    sqlcmd.Parameters.Add("@type", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@type"].Value = bull.Type;

                    sqlcmd.Parameters.Add("@location", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@location"].Value = bull.Location;

                    sqlcmd.Parameters.Add("@remarks", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@remarks"].Value = bull.Remarks;
                    try
                    {
                        Console.WriteLine("HI");
                        conn.Open();
                        bull.Id = (Int64) sqlcmd.ExecuteScalar();
                        bull.SaveImage2();
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Bull Added successfully ID:"+bull.Id;
                        data2["status"] = "Success";
                        data["data"] = data2;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        Console.WriteLine("Savin Failed");
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Bull Save Failed";
                        data2["status"] = "Failure";
                        data["data"] = data2;
                    }
                }
                return data;
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Bull Save Failed Validations Failed";
                data2["status"] = "Failure";
                data["data"] = data2;
                data["errors"] = bull.errors;
                return data;
            }
        }
        public IEnumerable<BullModel> GetBulls(BullFilterModel bullFilter)
        {
            int length = 10;
            int start = 0;
            string tagNo = null;
            string name = null;
            if (bullFilter != null)
            {
                try{
                    length = (int)bullFilter.length;
                    start = (int)bullFilter.start;
                }catch(Exception e)
                {

                }
                BullModel bull_ = bullFilter.bull;
                if (bull_.Name != null && bull_.Name.Trim() != "")
                {
                    name = "%" + bull_.Name + "%";
                }
                if (bull_.TagNo != null && bull_.TagNo.Trim() != "")
                {
                    tagNo = "%" + bull_.TagNo + "%";
                }
            }
            List<BullModel> bullsList = new List<BullModel>();
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
                string query = $"Select * from Animals where Category = 'BULL' {where} order by TagNo  OFFSET {start} ROWS FETCH NEXT {length} ROWS ONLY";
                Console.Write(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
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
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        BullModel BullModel = new BullModel(sqlrdr);
                        bullsList.Add(BullModel);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return bullsList;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return bullsList;
                }
            }
        }
        internal object GetTotalBulls()
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
        internal int GetTotalFilteredBulls(BullFilterModel bullFilters)
        {
            if (bullFilters != null)
            {
                BullModel bull_ = bullFilters.bull;
            }
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
        public Dictionary<int, string> GetBullsIdNamePairByTagNo(string tagNo,int pageno=1)
        {
            Dictionary<int, string> BullIdPair = new Dictionary<int, string>();
            int limit = 20;
            int offset = (pageno-1)*limit;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                System.Console.WriteLine("HELLO"+tagNo);
                tagNo = "%"+tagNo+"%";
                SqlCommand sqlcmd = new SqlCommand("Select * from Animals where Category  ='Bull' and TagNo like @TagNo order by TagNo  OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY", conn);
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
                        BullIdPair.Add(Convert.ToInt32(sqlrdr["Id"]), Convert.ToString(sqlrdr["TagNo"]));
                    }
                    sqlrdr.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
                return BullIdPair;
            }
        }
        internal BullModel GetBullById(int id)
        {
            BullModel bullModel = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand("Select Animals.*,Dam.TagNo as DamTagNo_,Dam.Name as DamName_,Sire.TagNo as SireTagNo_,Sire.Name as SireName_ from Animals left join Animals Dam on Dam.Id = Animals.DamID Left join Animals Sire on Sire.Id = Animals.SireID where Animals.Id = @ID and Animals.Gender = 'Male' and Animals.Category = 'BULL'", conn);
                sqlcmd.Parameters.Add("@ID", System.Data.SqlDbType.Int);
                sqlcmd.Parameters["@ID"].Value = id;
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        bullModel = new BullModel(sqlrdr);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return bullModel;
                }
                catch (Exception ex)
                {
                    return bullModel;
                }

            }
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
        public static Dictionary<string, string> GetBullNameTagNoById(IConfiguration _configuration, long id)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand("Select Name,TagNo from Animals where Category='BULL' and Id = @Id", conn);
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
        public Dictionary<string, object> GetCalvingDetailByBullId(long id)
        {
            bool GetIntercalvperiod = true;
            Dictionary<int, DateTime> intercalv = new Dictionary<int, DateTime>();

            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<long, Dictionary<string, object>> calvs = new Dictionary<long, Dictionary<string, object>>();
            List<long> cows = new List<long>();
            if (id != null)
            {
                ArrayList cowConcieveData = new ArrayList();
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    string query = "Select Animals.*,CowConceiveData.Id as cow_service_id,CowConceiveData.DateOfService,CowConceiveData.DeliveryStatus," +
                        "CowConceiveData.DamWeight,CowConceiveData.BirthWeight,CowConceiveData.Remarks from Animals " +
                        "left Join CowConceiveData on CowConceiveData.AnimalId = Animals.Id where SireID = @SireID order by BirthLactationNumber ASC";
                    Console.WriteLine(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    try
                    {
                        sqlcmd.Parameters.Add("@SireID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@SireID"].Value = id;
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        long counter = 1;
                        //string cowTagNumber = this.GetCowTagNameById(id);
                        Dictionary<string,string> bullTagNumber = BullsContext.GetBullNameTagNoById(_configuration, id);
                        while (sqlrdr.Read())
                        {
                            Console.WriteLine("Sire ID " + sqlrdr["DamID"]);
                            if (sqlrdr["DamID"] != null && sqlrdr["DamID"].ToString().Trim()!="" && !cows.Contains(Convert.ToInt64(sqlrdr["DamID"])))
                            {
                                cows.Add(Convert.ToInt64(sqlrdr["DamID"]));
                            }
                            Dictionary<string, object> calv = new Dictionary<string, object>();
                            Console.WriteLine("COW SERVICE ID'"+sqlrdr["cow_service_id"]+"'");
                            calv["sno"] = counter;
                            calv["id"] = Helper.IsNullOrEmpty(sqlrdr["Id"]);
                            calv["cow_service_id"] = Helper.FormatLong(sqlrdr["cow_service_id"]);
							calv["dateOfService"] = Helper.FormatDate(sqlrdr["DateOfService"]);
                            calv["bullID"] = Helper.IsNullOrEmpty(sqlrdr["SireID"]);
                            calv["DamID"] = Helper.IsNullOrEmpty(sqlrdr["DamID"]);
                            // calv["battalion_name"]
                            calv["cowNo"] = "";
                            calv["bullSemenNo"] = bullTagNumber["tagNo"]+"/"+bullTagNumber["name"];
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
                            //Console.WriteLine(" Lactation no "+calv["lactationNo"].ToString().Trim());
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
                        CowsContext cowsContext = new CowsContext(this._configuration);
                        Dictionary<long, object>  cowsNameTagNoId = cowsContext.GetCowTagNoNameByIds(cows);
                        foreach(var m in calvs)
                        {
                            try
                            {
                                Dictionary<string, string> cowsNameTagNo = (Dictionary<string, string>)cowsNameTagNoId[Convert.ToInt64(calvs[m.Key]["DamID"])];
                                calvs[m.Key]["cowNo"] = cowsNameTagNo["tagNo"] + "/" + cowsNameTagNo["name"];
                            }catch(Exception e)
                            {
                                calvs[m.Key]["cowNo"] = "-";
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
        public Dictionary<string,object> AddSellBull(SalePurchaseAnimal salePurchaseAnimal, BuyerSellerModal buyerSellerModal){

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
                        data2["message"] = "Bull Sold successfully";
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
        public Dictionary<string, object> GetSellBullDetailById(long id){
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
                        data["status"] = "success";
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
        public Dictionary<string, object> GetSellBullDetailByBullId(long id){
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
        public Dictionary<string,object> EditSellBull(SalePurchaseAnimal salePurchaseAnimal, BuyerSellerModal buyerSellerModal){

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
                        ",[SupervisorId] = @SupervisorId,[SalePurchase] = @SalePurchase,[Remarks] = @Remarks WHERE Id = @Id";
                        
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
                            data2["message"] = "Bull Sold successfully Updated";
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
