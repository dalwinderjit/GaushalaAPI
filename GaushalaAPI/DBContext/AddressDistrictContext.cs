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
    public class AddressDistrictContext : AnimalContext
    {
        private readonly IConfiguration _configuration;
        private SqlConnection conn;
        public AddressDistrictContext(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }
        internal Dictionary<string, object> AddAddressDistrict(AddressDistrictModel addressDistrictModel)
        {

            Dictionary<string, object> data = new Dictionary<string, object>();
            addressDistrictModel.Deleted = false;
            addressDistrictModel.Created = DateTime.Now;
            if (addressDistrictModel.ValidateAddressDistrict("Add") == true)
            {
                string query = "";// "Insert into AddressDistrict (Date,AnimalId,Disease,Symptoms,Diagnosis,Prognosis,Treatment,Result,CostofTreatment2,Remarks) OUTPUT INSERTED.Id Values"+
                //"(@Date,@AnimalId,@Disease,@Symptoms,@Diagnosis,@Prognosis,@Treatment,@Result,@CostofTreatment2,@Remarks)";
                query = this.GenerateInsertAddressDistrictSqlQuery(addressDistrictModel);
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.District), addressDistrictModel.District, "District", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.StateID), addressDistrictModel.StateID, "StateID", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.Deleted), addressDistrictModel.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.Created), addressDistrictModel.Created, "Created", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("AddressDistrict");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        addressDistrictModel.Id = (Int64)sqlcmd.ExecuteScalar();
                        
                            if (addressDistrictModel.Id > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "District Added Successfully! ";
                                data["data"] = data2;
                                data["status"] = true;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "District Addition Failed";
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
                data2["message"] = "District Addition Failed. Invalid Data Entered";
                data["errors"] = addressDistrictModel.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }

        internal List<Dictionary<string, object>> GetAddressDistrictList(AddressDistrictFilter addressDistrictModelFilter)
        {
            List<Dictionary<string, object>> AddressDistrictList_ = new List<Dictionary<string, object>>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                Console.WriteLine("HIE");
                string where = "";
                if (addressDistrictModelFilter.Ids != null && addressDistrictModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', addressDistrictModelFilter.Ids)})";
                }
                if (addressDistrictModelFilter.District != null && addressDistrictModelFilter.District != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" District like @District ";
                }
                if (addressDistrictModelFilter.StateID != null && addressDistrictModelFilter.StateID.ToString() != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" StateID  =  @StateID  ";
                }
                if (addressDistrictModelFilter.Deleted != null && addressDistrictModelFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted ";
                }

                string orderBy = "";
                if (addressDistrictModelFilter.OrderBy != null && addressDistrictModelFilter.OrderBy != "" && addressDistrictModelFilter.Order != null && addressDistrictModelFilter.Order != "")
                {
                    addressDistrictModelFilter.Order = Helper.GetValidateOrderClause(addressDistrictModelFilter.Order);
                    orderBy += $" order by Case @OrderBy When 'ID' then CAST([ID] as varchar(256)) When 'District' then District when 'StateID' then cast(StateID as varchar(256)) when 'Deleted' then cast(Deleted as varchar(10)) else cast(ID as varchar(256))  END {addressDistrictModelFilter.Order} ";
                    //orderBy += $" order by case 1 when 1 then District else ID end {addressDistrictModelFilter.Order} ";
                    //orderBy += $" order by Id ASC ";
                }
                else
                {
                    orderBy += $" order by ID ASC ";
                }
                string offset = "";
                if (addressDistrictModelFilter.PageNo != null && addressDistrictModelFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (addressDistrictModelFilter.cols.Length > 0)
                {
                    cols = String.Join(",", addressDistrictModelFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AddressDistricts {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (addressDistrictModelFilter.Ids != null && addressDistrictModelFilter.Ids.Length > 0)
                {
                    
                    if (where != "") { where += " and "; }
                    where += $" Id in ("+String.Join(",",addressDistrictModelFilter.Ids)+")";
                }
                if (addressDistrictModelFilter.District != null && addressDistrictModelFilter.District != "")
                {
                    sqlcmd.Parameters.Add("@District", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@District"].Value = "%"+addressDistrictModelFilter.District+"%";
                }
                if (addressDistrictModelFilter.StateID != null && addressDistrictModelFilter.StateID.ToString() != "")
                {
                    sqlcmd.Parameters.Add("@StateID ", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@StateID "].Value = addressDistrictModelFilter.StateID ;
                }
                if (addressDistrictModelFilter.Deleted != null && addressDistrictModelFilter.Deleted != "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.Bit);
                    sqlcmd.Parameters["@Deleted"].Value = addressDistrictModelFilter.Deleted;
                }
                
                if (addressDistrictModelFilter.OrderBy != null && addressDistrictModelFilter.OrderBy != "" && addressDistrictModelFilter.Order != null && addressDistrictModelFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = addressDistrictModelFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = addressDistrictModelFilter.Order;
                }
                if (addressDistrictModelFilter.PageNo != null && addressDistrictModelFilter.RecordsPerPage != null)
                {
                    addressDistrictModelFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = addressDistrictModelFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = addressDistrictModelFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int counter = addressDistrictModelFilter.GetStart();
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> breeds = new Dictionary<string, object>();
                        breeds["sno"] = counter;
                        if (addressDistrictModelFilter.cols.Length > 0)
                        {
                            for (int i = 0; i < addressDistrictModelFilter.cols.Length; i++)
                            {
                                breeds[addressDistrictModelFilter.cols[i]] = sqlrdr[addressDistrictModelFilter.cols[i]];
                            }
                        }
                        else
                        {
                            breeds["Id"] = sqlrdr["Id"];
                            breeds["District"] = sqlrdr["District"].ToString();
                            breeds["StateID"] = Helper.IsNullOrEmpty(sqlrdr["StateID"]);
                            breeds["Created"] = Helper.FormatDate(sqlrdr["Created"]);
                            breeds["Updated"] = Helper.FormatDate(sqlrdr["Updated"]);
                            breeds["Deleted"] = Helper.IsNullOrEmpty(sqlrdr["Deleted"]);
                        }
                        AddressDistrictList_.Add(breeds);
                        counter++;
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AddressDistrictList_;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AddressDistrictList_;
                }
            }
        }

        public string GenerateInsertAddressDistrictSqlQuery(AddressDistrictModel addressDistrictModel)
        {
            string addQuery = "";
            string cols = "";
            string params_ = "";
            this.addColToQuery(!Validations.IsNullOrEmpty(addressDistrictModel.District), ref cols, ref params_, "District");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressDistrictModel.StateID), ref cols, ref params_, "StateID");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressDistrictModel.Created), ref cols, ref params_, "Created");
            this.addColToQuery(!Validations.IsNullOrEmpty(addressDistrictModel.Deleted), ref cols, ref params_, "Deleted");
            addQuery = $"INSERT into [dbo].[AddressDistricts] ({cols}) OUTPUT INSERTED.Id values({params_});";
            return addQuery;
        }
        internal Dictionary<string, object> EditAddressDistrict(AddressDistrictModel addressDistrictModel)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            addressDistrictModel.Updated = DateTime.Now;
            if (addressDistrictModel.ValidateAddressDistrict("Edit") == true)
            {
                string query = this.GenerateUpdateAddressDistrictSqlQuery(addressDistrictModel);
                Console.WriteLine(query);
                
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.Id), addressDistrictModel.Id, "Id", System.Data.SqlDbType.BigInt);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.District), addressDistrictModel.District, "District", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.StateID), addressDistrictModel.StateID, "StateID", System.Data.SqlDbType.VarChar);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.Deleted), addressDistrictModel.Deleted, "Deleted", System.Data.SqlDbType.Bit);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.Created), addressDistrictModel.Created, "Created", System.Data.SqlDbType.DateTime);
                    this.AddColToSqlCommand(ref sqlcmd, !Validations.IsNullOrEmpty(addressDistrictModel.Updated), addressDistrictModel.Updated, "Updated", System.Data.SqlDbType.DateTime);
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        int i = sqlcmd.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "District Updated Successfully.";
                            data["data"] = data2;
                            data["status"] = true;
                        }
                        else
                        {
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "District Updation Failed";
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
                data2["message"] = "District Udpation Failed.Invalid Data Entered";
                data["errors"] = addressDistrictModel.errors;
                data["data"] = data2;
                data["status"] = false;
            }
            return data;
        }
        public string GenerateUpdateAddressDistrictSqlQuery(AddressDistrictModel addressDistrictModel)
        {
            string UpdateQuery = "";
            string cols = "";
            string where = "";
            //Build Where Clause for Animal Filter ????
            where = "where Id = @Id";
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressDistrictModel.District), ref cols, "District");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressDistrictModel.StateID), ref cols, "StateID");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressDistrictModel.Deleted), ref cols, "Deleted");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressDistrictModel.Created), ref cols, "Created");
            this.addColToUpdateQuery(!Validations.IsNullOrEmpty(addressDistrictModel.Updated), ref cols, "Updated");
            UpdateQuery = $"UPDATE [dbo].[AddressDistricts] set {cols} {where};";
            Console.WriteLine(UpdateQuery);
            return UpdateQuery;
        }
        internal AddressDistrictModel? GetAddressDistrictDetailById(long id)
        {
            AddressDistrictModel? addressDistrictModel = null;
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "select * from AddressDistricts where Id = @Id";
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
                        addressDistrictModel = new AddressDistrictModel(sqlrdr);
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
            return addressDistrictModel;
        }
        internal List<Dictionary<string, object>> GetAddressDistrictDetailByStatetId(long stateID)
        {
            AddressDistrictFilter addressDistrictFilter = new AddressDistrictFilter();
            addressDistrictFilter.StateID = stateID;
            return this.GetAddressDistrictList(addressDistrictFilter);
        }

        internal Dictionary<string, string> GetAddressDistrictIdNamePair(AddressDistrictFilter addressDistrictModelFilter)
        {
            List<Dictionary<string, object>> AddressDistrictList_ = new List<Dictionary<string, object>>();
            Dictionary<string, string> AddressDistrict = new Dictionary<string, string>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (conn = new SqlConnection(connectionString))
            {
                string where = "";
                if (addressDistrictModelFilter.Ids != null && addressDistrictModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in ({String.Join(',', addressDistrictModelFilter.Ids)})";
                }
                if (addressDistrictModelFilter.District != null && addressDistrictModelFilter.District != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" District like @District ";
                }
                if (addressDistrictModelFilter.StateID != null )
                {
                    if (where != "") { where += " and "; }
                    where += $" StateID = @StateID ";
                }
                if (addressDistrictModelFilter.Deleted != null && addressDistrictModelFilter.Deleted != "")
                {
                    if (where != "") { where += " and "; }
                    where += $" Deleted like @Deleted";
                }
                string orderBy = "";
                if (addressDistrictModelFilter.OrderBy != null && addressDistrictModelFilter.OrderBy != "" && addressDistrictModelFilter.Order != null && addressDistrictModelFilter.Order != "")
                {
                    addressDistrictModelFilter.Order = Helper.GetValidateOrderClause(addressDistrictModelFilter.Order);
                    //orderBy += $" order by Case @OrderBy When 'ID' then ID When 'District' then District when 'Deleted' then Deleted END {addressDistrictModelFilter.Order} ";
                    orderBy += $" order by Case @OrderBy When 'ID' then CAST([ID] as varchar(256)) When 'District' then District when 'Deleted' then cast(Deleted as varchar(10)) else cast(ID as varchar(256))  END {addressDistrictModelFilter.Order} ";
                }
                else
                {
                    orderBy += $" order by Id ASC ";
                }
                string offset = "";
                if (addressDistrictModelFilter.PageNo != null && addressDistrictModelFilter.RecordsPerPage != null)
                {
                    offset += $" OFFSET @Start ROWS FETCH NEXT @Length ROWS ONLY";
                }
                string cols = "*";
                if (addressDistrictModelFilter.cols.Length > 0)
                {
                    cols = String.Join(",", addressDistrictModelFilter.cols);
                }
                if (where != "")
                {
                    where = " where " + where;
                }
                string query = $"Select {cols} from AddressDistricts {where} {orderBy}  {offset}";
                Console.WriteLine(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                if (addressDistrictModelFilter.Ids != null && addressDistrictModelFilter.Ids.Length > 0)
                {
                    if (where != "") { where += " and "; }
                    where += $" Id in (" + String.Join(",", addressDistrictModelFilter.Ids) + ")";
                    //sqlcmd.Parameters.Add("@Ids", System.Data.SqlDbType.);
                    //sqlcmd.Parameters["@Ids"].Value = animalFilter.District;
                }
                if (addressDistrictModelFilter.District != null && addressDistrictModelFilter.District != "")
                {
                    sqlcmd.Parameters.Add("@District", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@District"].Value = "%"+addressDistrictModelFilter.District+"%";
                }
                if (addressDistrictModelFilter.StateID != null)
                {
                    sqlcmd.Parameters.Add("@StateID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@StateID"].Value = addressDistrictModelFilter.StateID;
                }
                if (addressDistrictModelFilter.Deleted != null && addressDistrictModelFilter.Deleted!= "")
                {
                    sqlcmd.Parameters.Add("@Deleted", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@Deleted"].Value = addressDistrictModelFilter.Deleted;
                }
                Console.WriteLine("Order by " + addressDistrictModelFilter.OrderBy);
                Console.WriteLine("Order by " + addressDistrictModelFilter.Order);
                if (addressDistrictModelFilter.OrderBy != null && addressDistrictModelFilter.OrderBy != "" && addressDistrictModelFilter.Order != null && addressDistrictModelFilter.Order != "")
                {
                    sqlcmd.Parameters.Add("@OrderBy", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@OrderBy"].Value = addressDistrictModelFilter.OrderBy;
                    //sqlcmd.Parameters.Add("@Order", System.Data.SqlDbType.VarChar);
                    //sqlcmd.Parameters["@Order"].Value = addressDistrictModelFilter.Order;
                }
                if (addressDistrictModelFilter.PageNo != null && addressDistrictModelFilter.RecordsPerPage != null)
                {
                    addressDistrictModelFilter.CalculateStartLength();
                    sqlcmd.Parameters.Add("@Start", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Start"].Value = addressDistrictModelFilter.Start;
                    sqlcmd.Parameters.Add("@Length", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@Length"].Value = addressDistrictModelFilter.Length;
                }
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Console.WriteLine(sqlrdr["Id"].ToString()+" "+sqlrdr["District"].ToString());
                        AddressDistrict[sqlrdr["Id"].ToString()] = sqlrdr["District"].ToString();
                    }
                    sqlrdr.Close();
                    conn.Close();
                    return AddressDistrict;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(ex.StackTrace);
                    return AddressDistrict;
                }
            }
        }
    }
}
    