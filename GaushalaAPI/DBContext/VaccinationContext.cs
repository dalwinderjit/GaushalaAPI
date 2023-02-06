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
    public class VaccinationContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public VaccinationContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddVaccination(VaccinationModel vaccination)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            if (vaccination.ValidateVaccination("Add") == true)
            {
                vaccination.Created = DateTime.Now;
                string query = "";// "Insert into Vaccination (Date,AnimalID,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.ID Values"+
                //"(@Date,@AnimalID,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertVaccinationSqlQuery(vaccination);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Name), vaccination.Name, "Name", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Description), vaccination.Description, "Description", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Created), vaccination.Created, "Created", System.Data.SqlDbType.DateTime);
                    //this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Updated), vaccination.Updated, "udpated", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Price), vaccination.Price, "Price", System.Data.SqlDbType.Decimal);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Qty), vaccination.Qty, "Qty", System.Data.SqlDbType.Decimal);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.State), vaccination.State, "State", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.AddedBy), vaccination.AddedBy, "AddedBy", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.VaccinationBrandID), vaccination.VaccinationBrandID, "VaccinationBrandID", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.VaccinationType), vaccination.VaccinationType, "VaccinationType", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.AmountPerPiece), vaccination.AmountPerPiece, "AmountPerPiece", System.Data.SqlDbType.Decimal);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("Vaccination");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        vaccination.ID = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (vaccination.ID > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Vaccination Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Vaccination Addition Failed";
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
                data2["message"] = "Vaccination Addition Failed. Invalid Data Entered";
                data["errors"] = vaccination.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;

        }

        internal List<Dictionary<string, object>> GetVaccinationList(VaccinationFilter vaccinationFilter)
        {
            List<Dictionary<string, object>> VaccinationList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (vaccinationFilter.Ids != null && vaccinationFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', vaccinationFilter.Ids)})";
                }
                if (vaccinationFilter.Name != null && vaccinationFilter.Name != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Name like @Name ";
                }
                if (vaccinationFilter.Description != null && vaccinationFilter.Description != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Description like @Description";
                }
                if (vaccinationFilter.State != null && vaccinationFilter.State != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" State like @State";
                }
                if (vaccinationFilter.VaccinationBrandID != null && vaccinationFilter.VaccinationBrandID != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" VaccinationBrandID = @VaccinationBrandID";
                }
                if (vaccinationFilter.VaccinationType!= null && vaccinationFilter.VaccinationType != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" VaccinationType = @VaccinationType";
                }

                string orderBy = "";
                if (vaccinationFilter.OrderBy != null && vaccinationFilter.OrderBy != "" && vaccinationFilter.Order != null && vaccinationFilter.Order != "")
                {
                    orderBy += $" order by ID ASC ";
                }
                string offset = "";
                if (vaccinationFilter.PageNo != null && vaccinationFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (vaccinationFilter.cols.Length > 0)
                {
                    cols = String.Join(",", vaccinationFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from VaccinationStock {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                //SqlCommand sqlcmd = new SqlCommand("Update [dbo].[Animals] set [TagNo] = @tagNo,[Name] = @name,[Category] = @Category where [Animals].[Id] = @ID", conn);
                if (vaccinationFilter.Ids != null && vaccinationFilter.Ids.Length > 0)
                {
                    //if (where != "") { where += " and "; }
                    //where += $" Id in (@Ids)";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Name;
                }
                if (vaccinationFilter.Name != null && vaccinationFilter.Name != "")
                {
                    sqlcmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Name"].Value = vaccinationFilter.Name;
                }
                if (vaccinationFilter.Description != null && vaccinationFilter.Description != "")
                {
                    sqlcmd.Parameters.Add("@Description", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Description"].Value = vaccinationFilter.Description;
                }
                if (vaccinationFilter.State != null && vaccinationFilter.State != "")
                {
                    sqlcmd.Parameters.Add("@State", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@State"].Value = vaccinationFilter.State;
                }
                if (vaccinationFilter.VaccinationBrandID!= null && vaccinationFilter.VaccinationBrandID != "")
                {
                    sqlcmd.Parameters.Add("@VaccinationBrandID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@VaccinationBrandID"].Value = vaccinationFilter.VaccinationBrandID;
                }
                if (vaccinationFilter.VaccinationType != null && vaccinationFilter.VaccinationType != "")
                {
                    sqlcmd.Parameters.Add("@VaccinationType", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@VaccinationType"].Value = vaccinationFilter.VaccinationType;
                }
                if (vaccinationFilter.OrderBy != null && vaccinationFilter.OrderBy != "" && vaccinationFilter.Order != null && vaccinationFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = vaccinationFilter.OrderBy;
                    sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Order"].Value = vaccinationFilter.Order;
                }
                if (vaccinationFilter.PageNo != null && vaccinationFilter.RecordsPerPage != null)
                {
                    vaccinationFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = vaccinationFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = vaccinationFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = vaccinationFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> animal = new Dictionary<string, object>();
                        animal["sno"] = counter;
                        if (vaccinationFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < vaccinationFilter.cols.Length; i++)
                            {
                                animal[vaccinationFilter.cols[i]] = sqlrdr[vaccinationFilter.cols[i]];
                            }
                        }
                        else
                        {
                            animal["ID"] = sqlrdr["Id"];
                            animal["Description"] = sqlrdr["Description"].ToString();
                            animal["Name"] = sqlrdr["Name"].ToString();
                            animal["Created"] = Helper.FormatDate(sqlrdr["Created"]);
                            animal["Updated"] = Helper.FormatDate(sqlrdr["Updated"]);
                            animal["Price"] = sqlrdr["Price"].ToString();
                            animal["Qty"] = sqlrdr["Qty"].ToString();
                            animal["State"] = sqlrdr["State"].ToString();
                            animal["AddedBy"] = sqlrdr["AddedBy"].ToString();
                            animal["VaccinationBrandID"] = sqlrdr["VaccinationBrandID"].ToString();
                            animal["VaccinationType"] = sqlrdr["VaccinationType"].ToString();
                            animal["AmountPerPiece"] = sqlrdr["AmountPerPiece"].ToString();
                        }
                        VaccinationList_.Add(animal);
                        counter++;
                    }
                    //sqlrdr.Close();
                    //conn.Close();
                    //return sqlrdr;
                    return VaccinationList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    //return null;
                    return VaccinationList_;
                }
            }
        }

        public string GenerateInsertVaccinationSqlQuery(VaccinationModel vaccination)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.Name), ref cols, ref params_, "Name");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.Description), ref cols, ref params_, "Description");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.Price), ref cols, ref params_, "Price");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.Qty), ref cols, ref params_, "Qty");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.State), ref cols, ref params_, "State");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.AddedBy), ref cols, ref params_, "AddedBy");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.Created), ref cols, ref params_, "Created");
            //this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.Updated), ref cols, ref params_, "Updated");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.VaccinationBrandID), ref cols, ref params_, "VaccinationBrandID");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.VaccinationType), ref cols, ref params_, "VaccinationType");
            this.addColToQuery(!Validations.IsNullOrEmpty(vaccination.AmountPerPiece), ref cols, ref params_, "AmountPerPiece");
            addQuery = $"INSERT into [dbo].[VaccinationStock] ({cols}) OUTPUT INSERTED.ID values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditVaccination(VaccinationModel vaccination)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (vaccination.ValidateVaccination("Edit") == true)
            {
                /*string query = "UPdate Vaccination set Date = @Date,AnimalID = @AnimalID,Disease = @Disease,Symptoms = @Symptoms,Diagnosis = @Diagnosis," +
                    "Prognosis = @Prognosis,Treatment = @Treatment,Result = @Result,CostofTreatment2 = @CostofTreatment2,Remarks = @Remarks  where Id = @Id";*/
                string query = this.GenerateUpdateAnimalSqlQuery(vaccination);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    //vaccination.Created = DateTime.Now;
                    vaccination.Updated = DateTime.Now;
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.ID), vaccination.ID, "ID", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Name), vaccination.Name, "Name", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Description), vaccination.Description, "Description", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Created), vaccination.Created, "Created", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Updated), vaccination.Updated, "udpated", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Price), vaccination.Price, "Price", System.Data.SqlDbType.Decimal);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.Qty), vaccination.Qty, "Qty", System.Data.SqlDbType.Decimal);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.State), vaccination.State, "State", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.AddedBy), vaccination.AddedBy, "AddedBy", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.VaccinationBrandID), vaccination.VaccinationBrandID, "VaccinationBrandID", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.VaccinationType), vaccination.VaccinationType, "VaccinationType", System.Data.SqlDbType.VarChar);
                    Console.WriteLine(vaccination.AmountPerPiece);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(vaccination.AmountPerPiece), vaccination.AmountPerPiece, "AmountPerPiece", System.Data.SqlDbType.Decimal);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        //tran = conn.BeginTransaction("Vaccination");
                        //tran.Save("save1");
                        //sqlcmd.Transaction = tran;
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            //tran.Commit();
                            //Console.WriteLine("Commiting");
                            data2["message"] = "Vaccination Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            //tran.Rollback();
                            //Console.WriteLine("rolling back");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Vaccination Updation Failed";
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
                data2["message"] = "Vaccination Udpation Failed.Invalid Data Entered";
                data["errors"] = vaccination.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateAnimalSqlQuery(VaccinationModel vaccination)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.Name), ref cols, "Name");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.Description), ref cols, "Description");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.Price), ref cols, "Price");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.Qty), ref cols, "Qty");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.State), ref cols, "State");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.AddedBy), ref cols, "AddedBy");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.Created), ref cols, "Created");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.Updated), ref cols, "Updated");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.VaccinationBrandID), ref cols, "VaccinationBrandID");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.VaccinationType), ref cols, "VaccinationType");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(vaccination.AmountPerPiece), ref cols, "AmountPerPiece");
            UpdateQuery = $"UPDATE [dbo].[VaccinationStock] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal VaccinationModel? GetVaccinationDetailById(long id)
        {
            VaccinationModel? medication = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from VaccinationStock where Id = @Id";
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
                        medication = new VaccinationModel(sqlrdr);
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

        internal Dictionary<long, string> GetVaccinationIdNamePair(VaccinationFilter vaccination)
        {
            List<Dictionary<string, object>> VaccinationBrandList_ = new List<Dictionary<string, object>>();
            Dictionary<long, string> Vaccination = new Dictionary<long, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (vaccination.Ids != null && vaccination.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', vaccination.Ids)})";
                }
                if (vaccination.Name != null && vaccination.Name != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Name like @Name ";
                }
                if (vaccination.Description != null && vaccination.Description != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Description like @Description";
                }

                string orderBy = "";
                if (vaccination.OrderBy != null && vaccination.OrderBy != "" && vaccination.Order != null && vaccination.Order != "")
                {
                    orderBy += $" order by ID ASC ";
                }
                string offset = "";
                if (vaccination.PageNo != null && vaccination.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (vaccination.cols.Length > 0)
                {
                    cols = String.Join(",", vaccination.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from VaccinationStock {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                //SqlCommand sqlcmd = new SqlCommand("Update [dbo].[Animals] set [TagNo] = @tagNo,[Name] = @name,[Category] = @Category where [Animals].[Id] = @ID", conn);
                if (vaccination.Ids != null && vaccination.Ids.Length > 0)
                {
                    //if (where != "") { where += " and "; }
                    //where += $" Id in (@Ids)";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Name;
                }
                if (vaccination.Name != null && vaccination.Name != "")
                {
                    sqlcmd.Parameters.Add("@Name", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Name"].Value = vaccination.Name;
                }
                if (vaccination.Description != null && vaccination.Description != "")
                {
                    sqlcmd.Parameters.Add("@Description", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Description"].Value = vaccination.Description;
                }
                if (vaccination.OrderBy != null && vaccination.OrderBy != "" && vaccination.Order != null && vaccination.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = vaccination.OrderBy;
                    sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Order"].Value = vaccination.Order;
                }
                if (vaccination.PageNo != null && vaccination.RecordsPerPage != null)
                {
                    vaccination.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = vaccination.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = vaccination.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Vaccination[Convert.ToInt64(sqlrdr["Id"])] = sqlrdr["Name"].ToString();

                    }
                    sqlrdr.Close();
                    conn.Close();
                    return Vaccination;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return Vaccination;
                }
            }
        }

    }
}
