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
    public class AnimalColorsContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public AnimalColorsContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddAnimalColors(AnimalColorsModel animalColors)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (animalColors.ValidateAnimalColors("Add") == true)
            {
                animalColors.Deleted = false;
                string query = "";// "Insert into AnimalColors (Date,AnimalId,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.Id Values"+
                //"(@Date,@AnimalId,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertAnimalColorsSqlQuery(animalColors);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalColors.Colour), animalColors.Colour, "Colour", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalColors.Deleted), animalColors.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("AnimalColors");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        animalColors.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (animalColors.Id > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "AnimalColors Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "AnimalColors Addition Failed";
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
                data2["message"] = "AnimalColors Addition Failed. Invalid Data Entered";
                data["errors"] = animalColors.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;

        }

        internal List<Dictionary<string, object>> GetAnimalColorsList(AnimalColorsFilter animalColorsFilter)
        {
            List<Dictionary<string, object>> AnimalColorsList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (animalColorsFilter.Ids != null && animalColorsFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', animalColorsFilter.Ids)})";
                }
                if (animalColorsFilter.Colour != null && animalColorsFilter.Colour != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Colour like @Colour ";
                }
                if (animalColorsFilter.Deleted != null && animalColorsFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted ";
                }

                string orderBy = "";
                if (animalColorsFilter.OrderBy != null && animalColorsFilter.OrderBy != "" && animalColorsFilter.Order != null && animalColorsFilter.Order != "")
                {
                    orderBy += $" order by Id ASC ";
                }
                string offset = "";
                if (animalColorsFilter.PageNo != null && animalColorsFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (animalColorsFilter.cols.Length > 0)
                {
                    cols = String.Join(",", animalColorsFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AnimalColors {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (animalColorsFilter.Ids != null && animalColorsFilter.Ids.Length > 0)
                {
                    
                    if (where != "") { where += " and "; }
                    where += $" Id in ("+String.Join(",",animalColorsFilter.Ids)+")";
                }
                if (animalColorsFilter.Colour != null && animalColorsFilter.Colour != "")
                {
                    sqlcmd.Parameters.Add("@Colour", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Colour"].Value = animalColorsFilter.Colour;
                }
                if (animalColorsFilter.Deleted != null && animalColorsFilter.Deleted != "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.Bit);
                    sqlcmd.Parameters["@Deleted"].Value = animalColorsFilter.Deleted;
                }
                
                if (animalColorsFilter.OrderBy != null && animalColorsFilter.OrderBy != "" && animalColorsFilter.Order != null && animalColorsFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = animalColorsFilter.OrderBy;
                    sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Order"].Value = animalColorsFilter.Order;
                }
                if (animalColorsFilter.PageNo != null && animalColorsFilter.RecordsPerPage != null)
                {
                    animalColorsFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = animalColorsFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = animalColorsFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = animalColorsFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> breeds = new Dictionary<string, object>();
                        breeds["sno"] = counter;
                        if (animalColorsFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < animalColorsFilter.cols.Length; i++)
                            {
                                breeds[animalColorsFilter.cols[i]] = sqlrdr[animalColorsFilter.cols[i]];
                            }
                        }
                        else
                        {
                            breeds["Id"] = sqlrdr["Id"];
                            breeds["Colour"] = sqlrdr["Colour"].ToString();
                            breeds["Deleted"] = Helper.FormatDate(sqlrdr["Deleted"]);
                        }
                        AnimalColorsList_.Add(breeds);
                        counter++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AnimalColorsList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AnimalColorsList_;
                }
            }
        }

        public string GenerateInsertAnimalColorsSqlQuery(AnimalColorsModel animalColors)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(animalColors.Colour), ref cols, ref params_, "Colour");
            this.addColToQuery(!Validations.IsNullOrEmpty(animalColors.Deleted), ref cols, ref params_, "Deleted");
            addQuery = $"INSERT into [dbo].[AnimalColors] ({cols}) OUTPUT INSERTED.Id values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditAnimalColors(AnimalColorsModel animalColors)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (animalColors.ValidateAnimalColors("Edit") == true)
            {
                string query = this.GenerateUpdateAnimalSqlQuery(animalColors);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalColors.Id), animalColors.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalColors.Colour), animalColors.Colour, "Colour", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(animalColors.Deleted), animalColors.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "AnimalColors Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "AnimalColors Updation Failed";
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
                data2["message"] = "AnimalColors Udpation Failed.Invalid Data Entered";
                data["errors"] = animalColors.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateAnimalSqlQuery(AnimalColorsModel animalColors)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(animalColors.Colour), ref cols, "Colour");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(animalColors.Deleted), ref cols, "Deleted");
            UpdateQuery = $"UPDATE [dbo].[AnimalColors] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal AnimalColorsModel? GetAnimalColorsDetailById(long id)
        {
            AnimalColorsModel? medication = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from AnimalColors where Id = @Id";
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
                        medication = new AnimalColorsModel(sqlrdr);
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

        internal Dictionary<long, string> GetAnimalColorsIdNamePair(AnimalColorsFilter animalColorsFilter)
        {
            List<Dictionary<string, object>> AnimalColorsList_ = new List<Dictionary<string, object>>();
            Dictionary<long, string> AnimalColors = new Dictionary<long, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                string where = "";
                if (animalColorsFilter.Ids != null && animalColorsFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', animalColorsFilter.Ids)})";
                }
                if (animalColorsFilter.Colour != null && animalColorsFilter.Colour != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Colour like @Colour ";
                }
                if (animalColorsFilter.Deleted != null && animalColorsFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted";
                }
                string orderBy = "";
                if (animalColorsFilter.OrderBy != null && animalColorsFilter.OrderBy != "" && animalColorsFilter.Order != null && animalColorsFilter.Order != "")
                {
                    orderBy += $" order by @OrderBy @Order ";
                }
                else
                {
                    orderBy += $" order by Id ASC ";
                }
                string offset = "";
                if (animalColorsFilter.PageNo != null && animalColorsFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (animalColorsFilter.cols.Length > 0)
                {
                    cols = String.Join(",", animalColorsFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AnimalColors {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (animalColorsFilter.Ids != null && animalColorsFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in (" + String.Join(",", animalColorsFilter.Ids) + ")";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Colour;
                }
                if (animalColorsFilter.Colour != null && animalColorsFilter.Colour != "")
                {
                    sqlcmd.Parameters.Add("@Colour", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Colour"].Value = animalColorsFilter.Colour;
                }
                if (animalColorsFilter.Deleted != null && animalColorsFilter.Deleted!= "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = animalColorsFilter.Deleted;
                }
                if (animalColorsFilter.OrderBy != null && animalColorsFilter.OrderBy != "" && animalColorsFilter.Order != null && animalColorsFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = animalColorsFilter.OrderBy;
                    sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Order"].Value = animalColorsFilter.Order;
                }
                if (animalColorsFilter.PageNo != null && animalColorsFilter.RecordsPerPage != null)
                {
                    animalColorsFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = animalColorsFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = animalColorsFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        AnimalColors[Convert.ToInt64(sqlrdr["Id"])] = sqlrdr["Colour"].ToString();

                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AnimalColors;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AnimalColors;
                }
            }
        }
    }
}
