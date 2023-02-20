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
    public class DiseaseContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public DiseaseContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddDisease(DiseaseModel disease)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (disease.ValidateDisease("Add") == true)
            {
                disease.Created = DateTime.Now;
                string query = "";// "Insert into Disease (Date,AnimalID,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.ID Values"+
                //"(@Date,@AnimalID,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertDiseaseSqlQuery(disease);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.DiseaseName), disease.DiseaseName, "DiseaseName", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.Description), disease.Description, "Description", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.Created), disease.Created, "Created", System.Data.SqlDbType.DateTime);
                    //this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Updated), vaccination.Updated, "udpated", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.AddedBy), disease.AddedBy, "AddedBy", System.Data.SqlDbType.BigInt);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("Disease");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        disease.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (disease.Id > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Disease Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Disease Addition Failed";
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
                data2["message"] = "Disease Addition Failed. Invalid Data Entered";
                data["errors"] = disease.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;

        }

        internal List<Dictionary<string, object>> GetDiseaseList(DiseaseFilter diseaseFilter)
        {
            List<Dictionary<string, object>> DiseaseList_ = new List<Dictionary<string, object>>();
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
                if (diseaseFilter.DiseaseName != null && diseaseFilter.DiseaseName != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" DiseaseName like @DiseaseName ";
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
                string query = $"Select {cols} from AnimalDiseases {where} {orderBy}  {offset}";
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
                if (diseaseFilter.DiseaseName != null && diseaseFilter.DiseaseName != "")
                {
                    sqlcmd.Parameters.Add("@DiseaseName", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@DiseaseName"].Value = diseaseFilter.DiseaseName;
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
                    int counter = diseaseFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> animal = new Dictionary<string, object>();
                        animal["sno"] = counter;
                        if (diseaseFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < diseaseFilter.cols.Length; i++)
                            {
                                animal[diseaseFilter.cols[i]] = sqlrdr[diseaseFilter.cols[i]];
                            }
                        }
                        else
                        {
                            animal["ID"] = sqlrdr["Id"];
                            animal["Description"] = sqlrdr["Description"].ToString();
                            animal["DiseaseName"] = sqlrdr["DiseaseName"].ToString();
                            animal["Created"] = Helper.FormatDate(sqlrdr["Created"]);
                            animal["Updated"] = Helper.FormatDate(sqlrdr["Updated"]);
                            animal["AddedBy"] = sqlrdr["AddedBy"].ToString();
                        }
                        DiseaseList_.Add(animal);
                        counter++;
                    }
                    //sqlrdr.Close();
                    //conn.Close();
                    //return sqlrdr;
                    return DiseaseList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    //return null;
                    return DiseaseList_;
                }
            }
        }

        public string GenerateInsertDiseaseSqlQuery(DiseaseModel disease)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(disease.DiseaseName), ref cols, ref params_, "DiseaseName");
            this.addColToQuery(!Validations.IsNullOrEmpty(disease.Description), ref cols, ref params_, "Description");
            this.addColToQuery(!Validations.IsNullOrEmpty(disease.AddedBy), ref cols, ref params_, "AddedBy");
            this.addColToQuery(!Validations.IsNullOrEmpty(disease.Created), ref cols, ref params_, "Created");
            //this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.Updated), ref cols, ref params_, "Updated");
            addQuery = $"INSERT into [dbo].[AnimalDiseases] ({cols}) OUTPUT INSERTED.ID values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditDisease(DiseaseModel disease)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (disease.ValidateDisease("Edit") == true)
            {
                /*string query = "UPdate Disease set Date = @Date,AnimalID = @AnimalID,Disease = @Disease,Symptoms = @Symptoms,Diagnosis = @Diagnosis," +
                    "Prognosis = @Prognosis,Treatment = @Treatment,Result = @Result,CostofTreatment2 = @CostofTreatment2,Remarks = @Remarks  where Id = @Id";*/
                disease.Updated = DateTime.Now;
                string query = this.GenerateUpdateDiseaseSqlQuery(disease);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    //vaccination.Created = DateTime.Now;
                    
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.Id), disease.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.DiseaseName), disease.DiseaseName, "DiseaseName", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.Description), disease.Description, "Description", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.Created), disease.Created, "Created", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.Updated), disease.Updated, "Updated", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(disease.AddedBy), disease.AddedBy, "AddedBy", System.Data.SqlDbType.BigInt);
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
                            data2["message"] = "Disease Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            //tran.Rollback();
                            //Console.WriteLine("rolling back");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Disease Updation Failed";
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
                data2["message"] = "Disease Udpation Failed.Invalid Data Entered";
                data["errors"] = disease.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateDiseaseSqlQuery(DiseaseModel disease)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(disease.DiseaseName), ref cols, "DiseaseName");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(disease.Description), ref cols, "Description");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(disease.AddedBy), ref cols, "AddedBy");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(disease.Created), ref cols, "Created");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(disease.Updated), ref cols, "Updated");
            UpdateQuery = $"UPDATE [dbo].[AnimalDiseases] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal DiseaseModel? GetDiseaseDetailById(long id)
        {
            DiseaseModel? disease = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from AnimalDiseases where ID = @Id";
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
                        disease = new DiseaseModel(sqlrdr);
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
            return disease;
        }
        internal Dictionary<long, string> GetDiseaseIdNamePairByDiseaseName(DiseaseFilter diseaseFilter)
        {
            List<Dictionary<string, object>> DiseaseList_ = new List<Dictionary<string, object>>();
            Dictionary<long, string> Diseases = new Dictionary<long, string>();
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
                if (diseaseFilter.DiseaseName != null && diseaseFilter.DiseaseName != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" DiseaseName like @DiseaseName ";
                }
                if (diseaseFilter.Description != null && diseaseFilter.Description != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Description like @Description";
                }

                string orderBy = "";
                if (diseaseFilter.OrderBy != null && diseaseFilter.OrderBy != "" && diseaseFilter.Order != null && diseaseFilter.Order != "")
                {
                    diseaseFilter.Order = Helper.GetValidateOrderClause(diseaseFilter.Order);
                    orderBy += $" order by Case @OrderBy When 'ID' then CAST([ID] as varchar(256)) When 'DiseaseName' then DiseaseName when 'Description' then cast(Description as varchar(256)) else cast(ID as varchar(256))  END {diseaseFilter.Order} ";
                    //orderBy += $" order by ID ASC ";
                }
                else
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
                string query = $"Select {cols} from AnimalDiseases {where} {orderBy}  {offset}";
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
                if (diseaseFilter.DiseaseName != null && diseaseFilter.DiseaseName != "")
                {
                    sqlcmd.Parameters.Add("@DiseaseName", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@DiseaseName"].Value = "%"+diseaseFilter.DiseaseName+"%";
                }
                if (diseaseFilter.Description != null && diseaseFilter.Description != "")
                {
                    sqlcmd.Parameters.Add("@Description", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Description"].Value = "%" + diseaseFilter.Description+ "%" ;
                }
                if (diseaseFilter.OrderBy != null && diseaseFilter.OrderBy != "" && diseaseFilter.Order != null && diseaseFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = diseaseFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = diseaseFilter.Order;
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
                        Diseases[Convert.ToInt64(sqlrdr["Id"])] = sqlrdr["DiseaseName"].ToString();
                        
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return Diseases;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return Diseases;
                }
            }
        }


    }
}
