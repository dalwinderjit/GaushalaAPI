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
    public class AddressStateContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public AddressStateContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddAddressState(AddressStateModel addressStateModel)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            addressStateModel.Deleted = false;
            addressStateModel.Created = DateTime.Now;
            if (addressStateModel.ValidateAddressState("Add") == true)
            {
                string query = "";// "Insert into AddressState (Date,AnimalId,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.Id Values"+
                //"(@Date,@AnimalId,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertAddressStateSqlQuery(addressStateModel);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.State), addressStateModel.State, "State", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.CountryID), addressStateModel.CountryID, "CountryID", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.Deleted), addressStateModel.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.Created), addressStateModel.Created, "Created", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("AddressState");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        addressStateModel.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (addressStateModel.Id > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "State Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "State Addition Failed";
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
                data2["message"] = "State Addition Failed. Invalid Data Entered";
                data["errors"] = addressStateModel.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }

        internal List<Dictionary<string, object>> GetAddressStateList(AddressStateFilter addressStateModelFilter)
        {
            List<Dictionary<string, object>> AddressStateList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (addressStateModelFilter.Ids != null && addressStateModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', addressStateModelFilter.Ids)})";
                }
                if (addressStateModelFilter.State != null && addressStateModelFilter.State != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" State like @State ";
                }
                if (addressStateModelFilter.CountryID != null && addressStateModelFilter.CountryID.ToString() != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" CountryID  =  @CountryID  ";
                }
                if (addressStateModelFilter.Deleted != null && addressStateModelFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted ";
                }

                string orderBy = "";
                if (addressStateModelFilter.OrderBy != null && addressStateModelFilter.OrderBy != "" && addressStateModelFilter.Order != null && addressStateModelFilter.Order != "")
                {
                    addressStateModelFilter.Order = Helper.GetValidateOrderClause(addressStateModelFilter.Order);
                    orderBy += $" order by Case @OrderBy When 'ID' then CAST([ID] as varchar(256)) When 'State' then State when 'CountryID' then cast(CountryID as varchar(256)) when 'Deleted' then cast(Deleted as varchar(10)) else cast(ID as varchar(256))  END {addressStateModelFilter.Order} ";
                    //orderBy += $" order by case 1 when 1 then State else ID end {addressStateModelFilter.Order} ";
                    //orderBy += $" order by Id ASC ";
                }
                else
                {
                    orderBy += $" order by ID ASC ";
                }
                string offset = "";
                if (addressStateModelFilter.PageNo != null && addressStateModelFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (addressStateModelFilter.cols.Length > 0)
                {
                    cols = String.Join(",", addressStateModelFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AddressStates {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (addressStateModelFilter.Ids != null && addressStateModelFilter.Ids.Length > 0)
                {
                    
                    if (where != "") { where += " and "; }
                    where += $" Id in ("+String.Join(",",addressStateModelFilter.Ids)+")";
                }
                if (addressStateModelFilter.State != null && addressStateModelFilter.State != "")
                {
                    sqlcmd.Parameters.Add("@State", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@State"].Value = "%"+addressStateModelFilter.State+"%";
                }
                if (addressStateModelFilter.CountryID != null && addressStateModelFilter.CountryID.ToString() != "")
                {
                    sqlcmd.Parameters.Add("@CountryID ", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@CountryID "].Value = addressStateModelFilter.CountryID ;
                }
                if (addressStateModelFilter.Deleted != null && addressStateModelFilter.Deleted != "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.Bit);
                    sqlcmd.Parameters["@Deleted"].Value = addressStateModelFilter.Deleted;
                }
                
                if (addressStateModelFilter.OrderBy != null && addressStateModelFilter.OrderBy != "" && addressStateModelFilter.Order != null && addressStateModelFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = addressStateModelFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = addressStateModelFilter.Order;
                }
                if (addressStateModelFilter.PageNo != null && addressStateModelFilter.RecordsPerPage != null)
                {
                    addressStateModelFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = addressStateModelFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = addressStateModelFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = addressStateModelFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> breeds = new Dictionary<string, object>();
                        breeds["sno"] = counter;
                        if (addressStateModelFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < addressStateModelFilter.cols.Length; i++)
                            {
                                breeds[addressStateModelFilter.cols[i]] = sqlrdr[addressStateModelFilter.cols[i]];
                            }
                        }
                        else
                        {
                            breeds["Id"] = sqlrdr["Id"];
                            breeds["State"] = sqlrdr["State"].ToString();
                            breeds["CountryID"] = sqlrdr["CountryID"].ToString();
                            breeds["Created"] = Helper.FormatDate(sqlrdr["Created"]);
                            breeds["Updated"] = Helper.FormatDate(sqlrdr["Updated"]);
                            breeds["Deleted"] = Helper.IsNullOrEmpty(sqlrdr["Deleted"]);
                        }
                        AddressStateList_.Add(breeds);
                        counter++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AddressStateList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AddressStateList_;
                }
            }
        }

        public string GenerateInsertAddressStateSqlQuery(AddressStateModel addressStateModel)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(addressStateModel.State), ref cols, ref params_, "State");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressStateModel.CountryID), ref cols, ref params_, "CountryID");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressStateModel.Created), ref cols, ref params_, "Created");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressStateModel.Deleted), ref cols, ref params_, "Deleted");
            addQuery = $"INSERT into [dbo].[AddressStates] ({cols}) OUTPUT INSERTED.Id values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditAddressState(AddressStateModel addressStateModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            addressStateModel.Updated = DateTime.Now;
            if (addressStateModel.ValidateAddressState("Edit") == true)
            {
                string query = this.GenerateUpdateAddressStateSqlQuery(addressStateModel);
                Console.WriteLine(query);
                
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.Id), addressStateModel.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.State), addressStateModel.State, "State", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.CountryID), addressStateModel.CountryID, "CountryID", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.Deleted), addressStateModel.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.Created), addressStateModel.Created, "Created", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressStateModel.Updated), addressStateModel.Updated, "Updated", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "State Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "State Updation Failed";
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
                data2["message"] = "State Udpation Failed.Invalid Data Entered";
                data["errors"] = addressStateModel.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateAddressStateSqlQuery(AddressStateModel addressStateModel)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressStateModel.State), ref cols, "State");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressStateModel.CountryID), ref cols, "CountryID");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressStateModel.Deleted), ref cols, "Deleted");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressStateModel.Created), ref cols, "Created");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressStateModel.Updated), ref cols, "Updated");
            UpdateQuery = $"UPDATE [dbo].[AddressStates] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal AddressStateModel? GetAddressStateDetailById(long id)
        {
            AddressStateModel? addressStateModel = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from AddressStates where Id = @Id";
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
                        addressStateModel = new AddressStateModel(sqlrdr);
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
            return addressStateModel;
        }
        internal List<Dictionary<string, object>> GetAddressStateDetailByCountryId(long countryID)
        {
            AddressStateFilter addressStateFilter = new AddressStateFilter();
            addressStateFilter.CountryID = countryID;
            return this.GetAddressStateList(addressStateFilter);
        }

        internal Dictionary<string, string> GetAddressStateIdNamePair(AddressStateFilter addressStateModelFilter)
        {
            List<Dictionary<string, object>> AddressStateList_ = new List<Dictionary<string, object>>();
            Dictionary<string, string> AddressState = new Dictionary<string, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                string where = "";
                if (addressStateModelFilter.Ids != null && addressStateModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', addressStateModelFilter.Ids)})";
                }
                if (addressStateModelFilter.State != null && addressStateModelFilter.State != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" State like @State ";
                }
                if (addressStateModelFilter.CountryID != null)
                {
                    if (where != "") { where += " and "; }
                    where += $" CountryID = @CountryID ";
                }
                if (addressStateModelFilter.Deleted != null && addressStateModelFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted";
                }
                string orderBy = "";
                if (addressStateModelFilter.OrderBy != null && addressStateModelFilter.OrderBy != "" && addressStateModelFilter.Order != null && addressStateModelFilter.Order != "")
                {
                    addressStateModelFilter.Order = Helper.GetValidateOrderClause(addressStateModelFilter.Order);
                    //orderBy += $" order by Case @OrderBy When 'ID' then ID When 'State' then State when 'Deleted' then Deleted END {addressStateModelFilter.Order} ";
                    orderBy += $" order by Case @OrderBy When 'ID' then CAST([ID] as varchar(256)) When 'State' then State when 'Deleted' then cast(Deleted as varchar(10)) else cast(ID as varchar(256))  END {addressStateModelFilter.Order} ";
                }
                else
                {
                    orderBy += $" order by Id ASC ";
                }
                string offset = "";
                if (addressStateModelFilter.PageNo != null && addressStateModelFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (addressStateModelFilter.cols.Length > 0)
                {
                    cols = String.Join(",", addressStateModelFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AddressStates {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (addressStateModelFilter.Ids != null && addressStateModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in (" + String.Join(",", addressStateModelFilter.Ids) + ")";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.State;
                }
                if (addressStateModelFilter.State != null && addressStateModelFilter.State != "")
                {
                    sqlcmd.Parameters.Add("@State", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@State"].Value = "%"+addressStateModelFilter.State+"%";
                }
                if (addressStateModelFilter.CountryID != null)
                {
                    sqlcmd.Parameters.Add("@CountryID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@CountryID"].Value =  addressStateModelFilter.CountryID;
                }
                if (addressStateModelFilter.Deleted != null && addressStateModelFilter.Deleted!= "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = addressStateModelFilter.Deleted;
                }
                Console.WriteLine("Order by " + addressStateModelFilter.OrderBy);
                Console.WriteLine("Order by " + addressStateModelFilter.Order);
                if (addressStateModelFilter.OrderBy != null && addressStateModelFilter.OrderBy != "" && addressStateModelFilter.Order != null && addressStateModelFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = addressStateModelFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = addressStateModelFilter.Order;
                }
                if (addressStateModelFilter.PageNo != null && addressStateModelFilter.RecordsPerPage != null)
                {
                    addressStateModelFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = addressStateModelFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = addressStateModelFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Console.WriteLine(sqlrdr["Id"].ToString()+" "+sqlrdr["State"].ToString());
                        AddressState[sqlrdr["Id"].ToString()] = sqlrdr["State"].ToString();
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AddressState;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AddressState;
                }
            }
        }
    }
}
    