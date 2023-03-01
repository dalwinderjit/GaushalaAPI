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
    public class AddressTehsilContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public AddressTehsilContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddAddressTehsil(AddressTehsilModel addressTehsilModel)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            addressTehsilModel.Deleted = false;
            addressTehsilModel.Created = DateTime.Now;
            if (addressTehsilModel.ValidateAddressTehsil("Add") == true)
            {
                string query = "";// "Insert into AddressTehsil (Date,AnimalId,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.Id Values"+
                //"(@Date,@AnimalId,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertAddressTehsilSqlQuery(addressTehsilModel);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.Tehsil), addressTehsilModel.Tehsil, "Tehsil", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.DistrictID), addressTehsilModel.DistrictID, "DistrictID", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.Deleted), addressTehsilModel.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.Created), addressTehsilModel.Created, "Created", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("AddressTehsil");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        addressTehsilModel.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (addressTehsilModel.Id > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Tehsil Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Tehsil Addition Failed";
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
                data2["message"] = "Tehsil Addition Failed. Invalid Data Entered";
                data["errors"] = addressTehsilModel.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }

        internal List<Dictionary<string, object>> GetAddressTehsilList(AddressTehsilFilter addressTehsilModelFilter)
        {
            List<Dictionary<string, object>> AddressTehsilList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (addressTehsilModelFilter.Ids != null && addressTehsilModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', addressTehsilModelFilter.Ids)})";
                }
                if (addressTehsilModelFilter.Tehsil != null && addressTehsilModelFilter.Tehsil != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Tehsil like @Tehsil ";
                }
                if (addressTehsilModelFilter.DistrictID != null && addressTehsilModelFilter.DistrictID.ToString() != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" DistrictID  = @DistrictID  ";
                }
                if (addressTehsilModelFilter.Deleted != null && addressTehsilModelFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted ";
                }

                string orderBy = "";
                if (addressTehsilModelFilter.OrderBy != null && addressTehsilModelFilter.OrderBy != "" && addressTehsilModelFilter.Order != null && addressTehsilModelFilter.Order != "")
                {
                    addressTehsilModelFilter.Order = Helper.GetValidateOrderClause(addressTehsilModelFilter.Order);
                    orderBy += $" order by Case @OrderBy When 'ID' then CAST([ID] as varchar(256)) When 'Tehsil' then Tehsil when 'DistrictID' then cast(DistrictID as varchar(256)) when 'Deleted' then cast(Deleted as varchar(10)) else cast(ID as varchar(256))  END {addressTehsilModelFilter.Order} ";
                    //orderBy += $" order by case 1 when 1 then Tehsil else ID end {addressTehsilModelFilter.Order} ";
                    //orderBy += $" order by Id ASC ";
                }
                else
                {
                    orderBy += $" order by ID ASC ";
                }
                string offset = "";
                if (addressTehsilModelFilter.PageNo != null && addressTehsilModelFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (addressTehsilModelFilter.cols.Length > 0)
                {
                    cols = String.Join(",", addressTehsilModelFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AddressTehsils {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (addressTehsilModelFilter.Ids != null && addressTehsilModelFilter.Ids.Length > 0)
                {
                    
                    if (where != "") { where += " and "; }
                    where += $" Id in ("+String.Join(",",addressTehsilModelFilter.Ids)+")";
                }
                if (addressTehsilModelFilter.Tehsil != null && addressTehsilModelFilter.Tehsil != "")
                {
                    sqlcmd.Parameters.Add("@Tehsil", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Tehsil"].Value = "%"+addressTehsilModelFilter.Tehsil+"%";
                }
                if (addressTehsilModelFilter.DistrictID != null && addressTehsilModelFilter.DistrictID.ToString() != "")
                {
                    sqlcmd.Parameters.Add("@DistrictID ", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@DistrictID "].Value = addressTehsilModelFilter.DistrictID ;
                }
                if (addressTehsilModelFilter.Deleted != null && addressTehsilModelFilter.Deleted != "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.Bit);
                    sqlcmd.Parameters["@Deleted"].Value = addressTehsilModelFilter.Deleted;
                }
                
                if (addressTehsilModelFilter.OrderBy != null && addressTehsilModelFilter.OrderBy != "" && addressTehsilModelFilter.Order != null && addressTehsilModelFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = addressTehsilModelFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = addressTehsilModelFilter.Order;
                }
                if (addressTehsilModelFilter.PageNo != null && addressTehsilModelFilter.RecordsPerPage != null)
                {
                    addressTehsilModelFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = addressTehsilModelFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = addressTehsilModelFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = addressTehsilModelFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> breeds = new Dictionary<string, object>();
                        breeds["sno"] = counter;
                        if (addressTehsilModelFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < addressTehsilModelFilter.cols.Length; i++)
                            {
                                breeds[addressTehsilModelFilter.cols[i]] = sqlrdr[addressTehsilModelFilter.cols[i]];
                            }
                        }
                        else
                        {
                            breeds["Id"] = sqlrdr["Id"];
                            breeds["Tehsil"] = sqlrdr["Tehsil"].ToString();
                            breeds["DistrictID"] = Helper.IsNullOrEmpty(sqlrdr["DistrictID"]);
                            breeds["Created"] = Helper.FormatDate(sqlrdr["Created"]);
                            breeds["Updated"] = Helper.FormatDate(sqlrdr["Updated"]);
                            breeds["Deleted"] = Helper.IsNullOrEmpty(sqlrdr["Deleted"]);
                        }
                        AddressTehsilList_.Add(breeds);
                        counter++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AddressTehsilList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AddressTehsilList_;
                }
            }
        }

        public string GenerateInsertAddressTehsilSqlQuery(AddressTehsilModel addressTehsilModel)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(addressTehsilModel.Tehsil), ref cols, ref params_, "Tehsil");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressTehsilModel.DistrictID), ref cols, ref params_, "DistrictID");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressTehsilModel.Created), ref cols, ref params_, "Created");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressTehsilModel.Deleted), ref cols, ref params_, "Deleted");
            addQuery = $"INSERT into [dbo].[AddressTehsils] ({cols}) OUTPUT INSERTED.Id values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditAddressTehsil(AddressTehsilModel addressTehsilModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            addressTehsilModel.Updated = DateTime.Now;
            if (addressTehsilModel.ValidateAddressTehsil("Edit") == true)
            {
                string query = this.GenerateUpdateAddressTehsilSqlQuery(addressTehsilModel);
                Console.WriteLine(query);
                
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.Id), addressTehsilModel.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.Tehsil), addressTehsilModel.Tehsil, "Tehsil", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.DistrictID), addressTehsilModel.DistrictID, "DistrictID", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.Deleted), addressTehsilModel.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.Created), addressTehsilModel.Created, "Created", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressTehsilModel.Updated), addressTehsilModel.Updated, "Updated", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Tehsil Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Tehsil Updation Failed";
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
                data2["message"] = "Tehsil Udpation Failed.Invalid Data Entered";
                data["errors"] = addressTehsilModel.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateAddressTehsilSqlQuery(AddressTehsilModel addressTehsilModel)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressTehsilModel.Tehsil), ref cols, "Tehsil");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressTehsilModel.DistrictID), ref cols, "DistrictID");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressTehsilModel.Deleted), ref cols, "Deleted");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressTehsilModel.Created), ref cols, "Created");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressTehsilModel.Updated), ref cols, "Updated");
            UpdateQuery = $"UPDATE [dbo].[AddressTehsils] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal AddressTehsilModel? GetAddressTehsilDetailById(long id)
        {
            AddressTehsilModel? addressTehsilModel = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from AddressTehsils where Id = @Id";
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
                        addressTehsilModel = new AddressTehsilModel(sqlrdr);
                    }
                    sqlrdr.Close();
                    conn.Close();
                    Console.WriteLine("Connection closed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Falied sds" + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            return addressTehsilModel;
        }
        internal List<Dictionary<string, object>> GetAddressTehsilDetailByDistrictId(long countryID)
        {
            AddressTehsilFilter addressTehsilFilter = new AddressTehsilFilter();
            addressTehsilFilter.DistrictID = countryID;
            return this.GetAddressTehsilList(addressTehsilFilter);
        }

        internal Dictionary<string, string> GetAddressTehsilIdNamePair(AddressTehsilFilter addressTehsilModelFilter)
        {
            List<Dictionary<string, object>> AddressTehsilList_ = new List<Dictionary<string, object>>();
            Dictionary<string, string> AddressTehsil = new Dictionary<string, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                string where = "";
                if (addressTehsilModelFilter.Ids != null && addressTehsilModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', addressTehsilModelFilter.Ids)})";
                }
                if (addressTehsilModelFilter.Tehsil != null && addressTehsilModelFilter.Tehsil != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Tehsil like @Tehsil ";
                }
                if (addressTehsilModelFilter.DistrictID != null)
                {
                    if (where != "") { where += " and "; }
                    where += $" DistrictID = @DistrictID ";
                }
                if (addressTehsilModelFilter.Deleted != null && addressTehsilModelFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted";
                }
                string orderBy = "";
                if (addressTehsilModelFilter.OrderBy != null && addressTehsilModelFilter.OrderBy != "" && addressTehsilModelFilter.Order != null && addressTehsilModelFilter.Order != "")
                {
                    addressTehsilModelFilter.Order = Helper.GetValidateOrderClause(addressTehsilModelFilter.Order);
                    //orderBy += $" order by Case @OrderBy When 'ID' then ID When 'Tehsil' then Tehsil when 'Deleted' then Deleted END {addressTehsilModelFilter.Order} ";
                    orderBy += $" order by Case @OrderBy When 'ID' then CAST([ID] as varchar(256)) When 'Tehsil' then Tehsil when 'Deleted' then cast(Deleted as varchar(10)) else cast(ID as varchar(256))  END {addressTehsilModelFilter.Order} ";
                }
                else
                {
                    orderBy += $" order by Id ASC ";
                }
                string offset = "";
                if (addressTehsilModelFilter.PageNo != null && addressTehsilModelFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (addressTehsilModelFilter.cols.Length > 0)
                {
                    cols = String.Join(",", addressTehsilModelFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AddressTehsils {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (addressTehsilModelFilter.Ids != null && addressTehsilModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in (" + String.Join(",", addressTehsilModelFilter.Ids) + ")";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Tehsil;
                }
                if (addressTehsilModelFilter.Tehsil != null && addressTehsilModelFilter.Tehsil != "")
                {
                    sqlcmd.Parameters.Add("@Tehsil", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Tehsil"].Value = "%"+addressTehsilModelFilter.Tehsil+"%";
                }
                if (addressTehsilModelFilter.DistrictID != null )
                {
                    sqlcmd.Parameters.Add("@DistrictID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@DistrictID"].Value = addressTehsilModelFilter.DistrictID ;
                }
                if (addressTehsilModelFilter.Deleted != null && addressTehsilModelFilter.Deleted!= "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = addressTehsilModelFilter.Deleted;
                }
                Console.WriteLine("Order by " + addressTehsilModelFilter.OrderBy);
                Console.WriteLine("Order by " + addressTehsilModelFilter.Order);
                if (addressTehsilModelFilter.OrderBy != null && addressTehsilModelFilter.OrderBy != "" && addressTehsilModelFilter.Order != null && addressTehsilModelFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = addressTehsilModelFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = addressTehsilModelFilter.Order;
                }
                if (addressTehsilModelFilter.PageNo != null && addressTehsilModelFilter.RecordsPerPage != null)
                {
                    addressTehsilModelFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = addressTehsilModelFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = addressTehsilModelFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Console.WriteLine(sqlrdr["Id"].ToString()+" "+sqlrdr["Tehsil"].ToString());
                        AddressTehsil[sqlrdr["Id"].ToString()] = sqlrdr["Tehsil"].ToString();
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AddressTehsil;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AddressTehsil;
                }
            }
        }
    }
}
    