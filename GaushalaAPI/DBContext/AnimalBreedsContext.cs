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
    public class AnimalBreedsContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public AnimalBreedsContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddAnimalBreeds(AnimalBreedsModel animalBreeds)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (animalBreeds.ValidateAnimalBreeds("Add") == true)
            {
                animalBreeds.Deleted = false;
                string query = "";// "Insert into AnimalBreeds (Date,AnimalId,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.Id Values"+
                //"(@Date,@AnimalId,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertAnimalBreedsSqlQuery(animalBreeds);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalBreeds.Breed), animalBreeds.Breed, "Breed", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalBreeds.Deleted), animalBreeds.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("AnimalBreeds");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        animalBreeds.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (animalBreeds.Id > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "AnimalBreeds Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "AnimalBreeds Addition Failed";
                                data["data"] = data2;
                                data["status"] = false;
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
                data2["message"] = "AnimalBreeds Addition Failed. Invalid Data Entered";
                data["errors"] = animalBreeds.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;

        }

        internal List<Dictionary<string, object>> GetAnimalBreedsList(AnimalBreedsFilter animalBreedsFilter)
        {
            List<Dictionary<string, object>> AnimalBreedsList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (animalBreedsFilter.Ids != null && animalBreedsFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', animalBreedsFilter.Ids)})";
                }
                if (animalBreedsFilter.Breed != null && animalBreedsFilter.Breed != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Breed like @Breed ";
                }
                if (animalBreedsFilter.Deleted != null && animalBreedsFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted ";
                }

                string orderBy = "";
                if (animalBreedsFilter.OrderBy != null && animalBreedsFilter.OrderBy != "" && animalBreedsFilter.Order != null && animalBreedsFilter.Order != "")
                {
                    orderBy += $" order by Id ASC ";
                }
                string offset = "";
                if (animalBreedsFilter.PageNo != null && animalBreedsFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (animalBreedsFilter.cols.Length > 0)
                {
                    cols = String.Join(",", animalBreedsFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from Breeds {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                //SqlCommand sqlcmd = new SqlCommand("Update [dbo].[Animals] set [TagNo] = @tagNo,[Breed] = @name,[Category] = @Category where [Animals].[Id] = @Id", conn);
                if (animalBreedsFilter.Ids != null && animalBreedsFilter.Ids.Length > 0)
                {
                    
                    if (where != "") { where += " and "; }
                    where += $" Id in ("+String.Join(",",animalBreedsFilter.Ids)+")";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Breed;
                }
                if (animalBreedsFilter.Breed != null && animalBreedsFilter.Breed != "")
                {
                    sqlcmd.Parameters.Add("@Breed", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Breed"].Value = animalBreedsFilter.Breed;
                }
                if (animalBreedsFilter.Deleted != null && animalBreedsFilter.Deleted != "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.Bit);
                    sqlcmd.Parameters["@Deleted"].Value = animalBreedsFilter.Deleted;
                }
                
                if (animalBreedsFilter.OrderBy != null && animalBreedsFilter.OrderBy != "" && animalBreedsFilter.Order != null && animalBreedsFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = animalBreedsFilter.OrderBy;
                    sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Order"].Value = animalBreedsFilter.Order;
                }
                if (animalBreedsFilter.PageNo != null && animalBreedsFilter.RecordsPerPage != null)
                {
                    animalBreedsFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = animalBreedsFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = animalBreedsFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = animalBreedsFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> breeds = new Dictionary<string, object>();
                        breeds["sno"] = counter;
                        if (animalBreedsFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < animalBreedsFilter.cols.Length; i++)
                            {
                                breeds[animalBreedsFilter.cols[i]] = sqlrdr[animalBreedsFilter.cols[i]];
                            }
                        }
                        else
                        {
                            breeds["Id"] = sqlrdr["Id"];
                            breeds["Breed"] = sqlrdr["Breed"].ToString();
                            breeds["Deleted"] = Helper.FormatDate(sqlrdr["Deleted"]);
                        }
                        AnimalBreedsList_.Add(breeds);
                        counter++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AnimalBreedsList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AnimalBreedsList_;
                }
            }
        }

        public string GenerateInsertAnimalBreedsSqlQuery(AnimalBreedsModel animalBreeds)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(animalBreeds.Breed), ref cols, ref params_, "Breed");
            this.addColToQuery(!Validations.IsNullOrEmpty(animalBreeds.Deleted), ref cols, ref params_, "Deleted");
            addQuery = $"INSERT into [dbo].[Breeds] ({cols}) OUTPUT INSERTED.Id values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditAnimalBreeds(AnimalBreedsModel animalBreeds)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (animalBreeds.ValidateAnimalBreeds("Edit") == true)
            {
                string query = this.GenerateUpdateAnimalSqlQuery(animalBreeds);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalBreeds.Id), animalBreeds.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalBreeds.Breed), animalBreeds.Breed, "Breed", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalBreeds.Deleted), animalBreeds.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "AnimalBreeds Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "AnimalBreeds Updation Failed";
                            data["data"] = data2;
                            data["status"] = false;
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
                data2["message"] = "AnimalBreeds Udpation Failed.Invalid Data Entered";
                data["errors"] = animalBreeds.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateAnimalSqlQuery(AnimalBreedsModel animalBreeds)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(animalBreeds.Breed), ref cols, "Breed");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(animalBreeds.Deleted), ref cols, "Deleted");
            UpdateQuery = $"UPDATE [dbo].[Breeds] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal AnimalBreedsModel? GetAnimalBreedsDetailById(long id)
        {
            AnimalBreedsModel? medication = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from Breeds where Id = @Id";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = id;
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if(sqlrdr.Read())
                    {
                        medication = new AnimalBreedsModel(sqlrdr);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("COnnection closed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Falied sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return medication;
        }

        internal Dictionary<long, string> GetAnimalBreedsIdNamePair(AnimalBreedsFilter animalBreedsFilter)
        {
            List<Dictionary<string, object>> AnimalBreedsBrandList_ = new List<Dictionary<string, object>>();
            Dictionary<long, string> AnimalBreeds = new Dictionary<long, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                string where = "";
                if (animalBreedsFilter.Ids != null && animalBreedsFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', animalBreedsFilter.Ids)})";
                }
                if (animalBreedsFilter.Breed != null && animalBreedsFilter.Breed != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Breed like @Breed ";
                }
                if (animalBreedsFilter.Deleted != null && animalBreedsFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted";
                }
                string orderBy = "";
                if (animalBreedsFilter.OrderBy != null && animalBreedsFilter.OrderBy != "" && animalBreedsFilter.Order != null && animalBreedsFilter.Order != "")
                {
                    orderBy += $" order by @OrderBy @Order ";
                }
                else
                {
                    orderBy += $" order by Id ASC ";
                }
                string offset = "";
                if (animalBreedsFilter.PageNo != null && animalBreedsFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (animalBreedsFilter.cols.Length > 0)
                {
                    cols = String.Join(",", animalBreedsFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from Breeds {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                //SqlCommand sqlcmd = new SqlCommand("Update [dbo].[Animals] set [TagNo] = @tagNo,[Breed] = @name,[Category] = @Category where [Animals].[Id] = @Id", conn);
                if (animalBreedsFilter.Ids != null && animalBreedsFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in (" + String.Join(",", animalBreedsFilter.Ids) + ")";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Breed;
                }
                if (animalBreedsFilter.Breed != null && animalBreedsFilter.Breed != "")
                {
                    sqlcmd.Parameters.Add("@Breed", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Breed"].Value = animalBreedsFilter.Breed;
                }
                if (animalBreedsFilter.Deleted != null && animalBreedsFilter.Deleted!= "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = animalBreedsFilter.Deleted;
                }
                if (animalBreedsFilter.OrderBy != null && animalBreedsFilter.OrderBy != "" && animalBreedsFilter.Order != null && animalBreedsFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = animalBreedsFilter.OrderBy;
                    sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Order"].Value = animalBreedsFilter.Order;
                }
                if (animalBreedsFilter.PageNo != null && animalBreedsFilter.RecordsPerPage != null)
                {
                    animalBreedsFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = animalBreedsFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = animalBreedsFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        AnimalBreeds[Convert.ToInt64(sqlrdr["Id"])] = sqlrdr["Breed"].ToString();

                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AnimalBreeds;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AnimalBreeds;
                }
            }
        }
    }
}
