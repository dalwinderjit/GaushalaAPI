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
    public class VaccinationBrandContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public VaccinationBrandContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddVaccinationBrand(VaccinationBrandModel vaccinationBrand)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (vaccinationBrand.ValidateVaccinationBrand("Add") == true)
            {
                vaccinationBrand.Created = DateTime.Now;
                string query = "";// "Insert into Disease (Date,AnimalID,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.ID Values"+
                //"(@Date,@AnimalID,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertVaccinationBrandSqlQuery(vaccinationBrand);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccinationBrand.Title), vaccinationBrand.Title, "Title", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccinationBrand.Description), vaccinationBrand.Description, "Description", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccinationBrand.Created), vaccinationBrand.Created, "Created", System.Data.SqlDbType.DateTime);
                    //this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Updated), vaccination.Updated, "udpated", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("VaccinationBrand");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        vaccinationBrand.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (vaccinationBrand.Id > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Vaccination Brand Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Vaccination Brand Addition Failed";
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
                data2["message"] = "Vaccination Brand Addition Failed. Invalid Data Entered";
                data["errors"] = vaccinationBrand.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;

        }

        internal List<Dictionary<string, object>> GetVaccinationBrandList(VaccinationBrandFilter vaccinationBrandFilter)
        {
            List<Dictionary<string, object>> VaccinationBrandList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (vaccinationBrandFilter.Ids != null && vaccinationBrandFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', vaccinationBrandFilter.Ids)})";
                }
                if (vaccinationBrandFilter.Title != null && vaccinationBrandFilter.Title != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Title like @Title ";
                }
                if (vaccinationBrandFilter.Description != null && vaccinationBrandFilter.Description != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Description like @Description";
                }

                string orderBy = "";
                if (vaccinationBrandFilter.OrderBy != null && vaccinationBrandFilter.OrderBy != "" && vaccinationBrandFilter.Order != null && vaccinationBrandFilter.Order != "")
                {
                    orderBy += $" order by ID ASC ";
                }
                string offset = "";
                if (vaccinationBrandFilter.PageNo != null && vaccinationBrandFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (vaccinationBrandFilter.cols.Length > 0)
                {
                    cols = String.Join(",", vaccinationBrandFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from VaccinationBrands {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                //SqlCommand sqlcmd = new SqlCommand("Update [dbo].[Animals] set [TagNo] = @tagNo,[Name] = @name,[Category] = @Category where [Animals].[Id] = @ID", conn);
                if (vaccinationBrandFilter.Ids != null && vaccinationBrandFilter.Ids.Length > 0)
                {
                    //if (where != "") { where += " and "; }
                    //where += $" Id in (@Ids)";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Name;
                }
                if (vaccinationBrandFilter.Title != null && vaccinationBrandFilter.Title != "")
                {
                    sqlcmd.Parameters.Add("@Title", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Title"].Value = vaccinationBrandFilter.Title;
                }
                if (vaccinationBrandFilter.Description != null && vaccinationBrandFilter.Description != "")
                {
                    sqlcmd.Parameters.Add("@Description", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Description"].Value = vaccinationBrandFilter.Description;
                }
                if (vaccinationBrandFilter.OrderBy != null && vaccinationBrandFilter.OrderBy != "" && vaccinationBrandFilter.Order != null && vaccinationBrandFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = vaccinationBrandFilter.OrderBy;
                    sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Order"].Value = vaccinationBrandFilter.Order;
                }
                if (vaccinationBrandFilter.PageNo != null && vaccinationBrandFilter.RecordsPerPage != null)
                {
                    vaccinationBrandFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = vaccinationBrandFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = vaccinationBrandFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = vaccinationBrandFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> vaccinationBrand = new Dictionary<string, object>();
                        vaccinationBrand["sno"] = counter;
                        if (vaccinationBrandFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < vaccinationBrandFilter.cols.Length; i++)
                            {
                                vaccinationBrand[vaccinationBrandFilter.cols[i]] = sqlrdr[vaccinationBrandFilter.cols[i]];
                            }
                        }
                        else
                        {
                            vaccinationBrand["Id"] = sqlrdr["Id"];
                            vaccinationBrand["Description"] = sqlrdr["Description"].ToString();
                            vaccinationBrand["Title"] = sqlrdr["Title"].ToString();
                            vaccinationBrand["Created"] = Helper.FormatDate(sqlrdr["Created"]);
                            vaccinationBrand["Updated"] = Helper.FormatDate(sqlrdr["Updated"]);
                        }
                        VaccinationBrandList_.Add(vaccinationBrand);
                        counter++;
                    }
                    //sqlrdr.Close();
                    //conn.Close();
                    //return sqlrdr;
                    return VaccinationBrandList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    //return null;
                    return VaccinationBrandList_;
                }
            }
        }

        public string GenerateInsertVaccinationBrandSqlQuery(VaccinationBrandModel vaccinationBrand)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccinationBrand.Title), ref cols, ref params_, "Title");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccinationBrand.Description), ref cols, ref params_, "Description");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccinationBrand.Created), ref cols, ref params_, "Created");
            //this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.Updated), ref cols, ref params_, "Updated");
            addQuery = $"INSERT into [dbo].[VaccinationBrands] ({cols}) OUTPUT INSERTED.ID values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditVaccinationBrand(VaccinationBrandModel vaccinationBrandM)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (vaccinationBrandM.ValidateVaccinationBrand("Edit") == true)
            {
                /*string query = "UPdate Disease set Date = @Date,AnimalID = @AnimalID,Disease = @Disease,Symptoms = @Symptoms,Diagnosis = @Diagnosis," +
                    "Prognosis = @Prognosis,Treatment = @Treatment,Result = @Result,CostofTreatment2 = @CostofTreatment2,Remarks = @Remarks  where Id = @Id";*/
                vaccinationBrandM.Updated = DateTime.Now;
                string query = this.GenerateUpdateVaccinationBrandSqlQuery(vaccinationBrandM);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    //vaccination.Created = DateTime.Now;
                    
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccinationBrandM.Id), vaccinationBrandM.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccinationBrandM.Title), vaccinationBrandM.Title, "Title", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccinationBrandM.Description), vaccinationBrandM.Description, "Description", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccinationBrandM.Created), vaccinationBrandM.Created, "Created", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccinationBrandM.Updated), vaccinationBrandM.Updated, "Updated", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        //tran = conn.BeginTransaction("Disease");
                        //tran.Save("save1");
                        //sqlcmd.Transaction = tran;
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            //tran.Commit();
                            //Console.WriteLine("Commiting");
                            data2["message"] = "Vaccination Brand Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            //tran.Rollback();
                            //Console.WriteLine("rolling back");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Vaccination Brand Updation Failed";
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
                data2["message"] = "Vaccination Brand Udpation Failed.Invalid Data Entered";
                data["errors"] = vaccinationBrandM.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateVaccinationBrandSqlQuery(VaccinationBrandModel vaccinationBrandM)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccinationBrandM.Title), ref cols, "Title");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccinationBrandM.Description), ref cols, "Description");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccinationBrandM.Created), ref cols, "Created");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccinationBrandM.Updated), ref cols, "Updated");
            UpdateQuery = $"UPDATE [dbo].[VaccinationBrands] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal VaccinationBrandModel? GetVaccinationBrandDetailById(long id)
        {
            VaccinationBrandModel? vaccinationBrand = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from VaccinationBrands where Id = @Id";
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
                        vaccinationBrand = new VaccinationBrandModel(sqlrdr);
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
            return vaccinationBrand;
        }
        internal Dictionary<long, string> GetVaccinationBrandIdNamePairByVaccinationBrandName(VaccinationBrandFilter diseaseFilter)
        {
            List<Dictionary<string, object>> VaccinationBrandList_ = new List<Dictionary<string, object>>();
            Dictionary<long, string> VaccinationBrands = new Dictionary<long, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (diseaseFilter.Ids != null && diseaseFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', diseaseFilter.Ids)})";
                }
                if (diseaseFilter.Title != null && diseaseFilter.Title != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Title like @Title ";
                }
                if (diseaseFilter.Description != null && diseaseFilter.Description != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Description like @Description";
                }

                string orderBy = "";
                if (diseaseFilter.OrderBy != null && diseaseFilter.OrderBy != "" && diseaseFilter.Order != null && diseaseFilter.Order != "")
                {
                    orderBy += $" order by ID ASC ";
                }
                string offset = "";
                if (diseaseFilter.PageNo != null && diseaseFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (diseaseFilter.cols.Length > 0)
                {
                    cols = String.Join(",", diseaseFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from VaccinationBrands {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                //SqlCommand sqlcmd = new SqlCommand("Update [dbo].[Animals] set [TagNo] = @tagNo,[Name] = @name,[Category] = @Category where [Animals].[Id] = @ID", conn);
                if (diseaseFilter.Ids != null && diseaseFilter.Ids.Length > 0)
                {
                    //if (where != "") { where += " and "; }
                    //where += $" Id in (@Ids)";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Name;
                }
                if (diseaseFilter.Title != null && diseaseFilter.Title != "")
                {
                    sqlcmd.Parameters.Add("@Title", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Title"].Value = diseaseFilter.Title;
                }
                if (diseaseFilter.Description != null && diseaseFilter.Description != "")
                {
                    sqlcmd.Parameters.Add("@Description", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Description"].Value = diseaseFilter.Description;
                }
                if (diseaseFilter.OrderBy != null && diseaseFilter.OrderBy != "" && diseaseFilter.Order != null && diseaseFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = diseaseFilter.OrderBy;
                    sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Order"].Value = diseaseFilter.Order;
                }
                if (diseaseFilter.PageNo != null && diseaseFilter.RecordsPerPage != null)
                {
                    diseaseFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = diseaseFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = diseaseFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        VaccinationBrands[Convert.ToInt64(sqlrdr["Id"])] = sqlrdr["Title"].ToString();
                        
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return VaccinationBrands;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return VaccinationBrands;
                }
            }
        }


    }
}
