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
    public class AddressCountryContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public AddressCountryContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddAddressCountry(AddressCountryModel addressCountryModel)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            addressCountryModel.Deleted = false;
            addressCountryModel.Created = DateTime.Now;
            if (addressCountryModel.ValidateAddressCountry("Add") == true)
            {
                string query = "";// "Insert into AddressCountry (Date,AnimalId,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.Id Values"+
                //"(@Date,@AnimalId,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertAddressCountrySqlQuery(addressCountryModel);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressCountryModel.Country), addressCountryModel.Country, "Country", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressCountryModel.Deleted), addressCountryModel.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressCountryModel.Created), addressCountryModel.Created, "Created", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("AddressCountry");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        addressCountryModel.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (addressCountryModel.Id > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Country Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Country Addition Failed";
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
                data2["message"] = "Country Addition Failed. Invalid Data Entered";
                data["errors"] = addressCountryModel.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }

        internal List<Dictionary<string, object>> GetAddressCountryList(AddressCountryFilter addressCountryModelFilter)
        {
            List<Dictionary<string, object>> AddressCountryList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (addressCountryModelFilter.Ids != null && addressCountryModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', addressCountryModelFilter.Ids)})";
                }
                if (addressCountryModelFilter.Country != null && addressCountryModelFilter.Country != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Country like @Country ";
                }
                if (addressCountryModelFilter.Deleted != null && addressCountryModelFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted ";
                }

                string orderBy = "";
                if (addressCountryModelFilter.OrderBy != null && addressCountryModelFilter.OrderBy != "" && addressCountryModelFilter.Order != null && addressCountryModelFilter.Order != "")
                {
                    addressCountryModelFilter.Order = Helper.GetValidateOrderClause(addressCountryModelFilter.Order);
                    orderBy += $" order by Case @OrderBy When 'ID' then [Country] When 'Country' then Country when 'Deleted' then Deleted else ID END {addressCountryModelFilter.Order} ";
                    //orderBy += $" order by Id ASC ";

                }
                string offset = "";
                if (addressCountryModelFilter.PageNo != null && addressCountryModelFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (addressCountryModelFilter.cols.Length > 0)
                {
                    cols = String.Join(",", addressCountryModelFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AddressCountries {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (addressCountryModelFilter.Ids != null && addressCountryModelFilter.Ids.Length > 0)
                {
                    
                    if (where != "") { where += " and "; }
                    where += $" Id in ("+String.Join(",",addressCountryModelFilter.Ids)+")";
                }
                if (addressCountryModelFilter.Country != null && addressCountryModelFilter.Country != "")
                {
                    sqlcmd.Parameters.Add("@Country", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Country"].Value = "%"+addressCountryModelFilter.Country+"%";
                }
                if (addressCountryModelFilter.Deleted != null && addressCountryModelFilter.Deleted != "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.Bit);
                    sqlcmd.Parameters["@Deleted"].Value = addressCountryModelFilter.Deleted;
                }
                
                if (addressCountryModelFilter.OrderBy != null && addressCountryModelFilter.OrderBy != "" && addressCountryModelFilter.Order != null && addressCountryModelFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = addressCountryModelFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = addressCountryModelFilter.Order;
                }
                if (addressCountryModelFilter.PageNo != null && addressCountryModelFilter.RecordsPerPage != null)
                {
                    addressCountryModelFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = addressCountryModelFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = addressCountryModelFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = addressCountryModelFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> breeds = new Dictionary<string, object>();
                        breeds["sno"] = counter;
                        if (addressCountryModelFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < addressCountryModelFilter.cols.Length; i++)
                            {
                                breeds[addressCountryModelFilter.cols[i]] = sqlrdr[addressCountryModelFilter.cols[i]];
                            }
                        }
                        else
                        {
                            breeds["Id"] = sqlrdr["Id"];
                            breeds["Country"] = sqlrdr["Country"].ToString();
                            breeds["Created"] = Helper.FormatDate(sqlrdr["Created"]);
                            breeds["Updated"] = Helper.FormatDate(sqlrdr["Updated"]);
                            breeds["Deleted"] = Helper.IsNullOrEmpty(sqlrdr["Deleted"]);
                        }
                        AddressCountryList_.Add(breeds);
                        counter++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AddressCountryList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AddressCountryList_;
                }
            }
        }

        public string GenerateInsertAddressCountrySqlQuery(AddressCountryModel addressCountryModel)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(addressCountryModel.Country), ref cols, ref params_, "Country");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressCountryModel.Created), ref cols, ref params_, "Created");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressCountryModel.Deleted), ref cols, ref params_, "Deleted");
            addQuery = $"INSERT into [dbo].[AddressCountries] ({cols}) OUTPUT INSERTED.Id values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditAddressCountry(AddressCountryModel addressCountryModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            addressCountryModel.Updated = DateTime.Now;
            if (addressCountryModel.ValidateAddressCountry("Edit") == true)
            {
                string query = this.GenerateUpdateAddressCountrySqlQuery(addressCountryModel);
                Console.WriteLine(query);
                
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressCountryModel.Id), addressCountryModel.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressCountryModel.Country), addressCountryModel.Country, "Country", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressCountryModel.Deleted), addressCountryModel.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressCountryModel.Created), addressCountryModel.Created, "Created", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressCountryModel.Updated), addressCountryModel.Updated, "Updated", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Country Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Country Updation Failed";
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
                data2["message"] = "Country Udpation Failed.Invalid Data Entered";
                data["errors"] = addressCountryModel.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateAddressCountrySqlQuery(AddressCountryModel addressCountryModel)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressCountryModel.Country), ref cols, "Country");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressCountryModel.Deleted), ref cols, "Deleted");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressCountryModel.Created), ref cols, "Created");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressCountryModel.Updated), ref cols, "Updated");
            UpdateQuery = $"UPDATE [dbo].[AddressCountries] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal AddressCountryModel? GetAddressCountryDetailById(long id)
        {
            AddressCountryModel? addressCountryModel = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from AddressCountries where Id = @Id";
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
                        addressCountryModel = new AddressCountryModel(sqlrdr);
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
            return addressCountryModel;
        }

        internal Dictionary<long, string> GetAddressCountryIdNamePair(AddressCountryFilter addressCountryModelFilter)
        {
            List<Dictionary<string, object>> AddressCountryList_ = new List<Dictionary<string, object>>();
            Dictionary<long, string> AddressCountry = new Dictionary<long, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                string where = "";
                if (addressCountryModelFilter.Ids != null && addressCountryModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', addressCountryModelFilter.Ids)})";
                }
                if (addressCountryModelFilter.Country != null && addressCountryModelFilter.Country != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Country like @Country ";
                }
                if (addressCountryModelFilter.Deleted != null && addressCountryModelFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted";
                }
                string orderBy = "";
                if (addressCountryModelFilter.OrderBy != null && addressCountryModelFilter.OrderBy != "" && addressCountryModelFilter.Order != null && addressCountryModelFilter.Order != "")
                {
                    addressCountryModelFilter.Order = Helper.GetValidateOrderClause(addressCountryModelFilter.Order);
                    orderBy += $" order by Case @OrderBy When 'ID' then ID When 'Country' then Country when 'Deleted' then Deleted END {addressCountryModelFilter.Order} ";
                }
                else
                {
                    orderBy += $" order by Id ASC ";
                }
                string offset = "";
                if (addressCountryModelFilter.PageNo != null && addressCountryModelFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (addressCountryModelFilter.cols.Length > 0)
                {
                    cols = String.Join(",", addressCountryModelFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AddressCountries {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (addressCountryModelFilter.Ids != null && addressCountryModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in (" + String.Join(",", addressCountryModelFilter.Ids) + ")";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.Country;
                }
                if (addressCountryModelFilter.Country != null && addressCountryModelFilter.Country != "")
                {
                    sqlcmd.Parameters.Add("@Country", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Country"].Value = "%"+addressCountryModelFilter.Country+"%";
                }
                if (addressCountryModelFilter.Deleted != null && addressCountryModelFilter.Deleted!= "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = addressCountryModelFilter.Deleted;
                }
                if (addressCountryModelFilter.OrderBy != null && addressCountryModelFilter.OrderBy != "" && addressCountryModelFilter.Order != null && addressCountryModelFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = addressCountryModelFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = addressCountryModelFilter.Order;
                }
                if (addressCountryModelFilter.PageNo != null && addressCountryModelFilter.RecordsPerPage != null)
                {
                    addressCountryModelFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = addressCountryModelFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = addressCountryModelFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        AddressCountry[Convert.ToInt64(sqlrdr["Id"])] = sqlrdr["Country"].ToString();
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AddressCountry;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AddressCountry;
                }
            }
        }
    }
}
