using GaushalaAPI.Models;
using GaushalAPI.Models;
using GaushalAPI.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using GaushalaAPI.Helpers;

namespace GaushalaAPI.DBContext
{
    public class BaseContext 
    {
        protected readonly IConfiguration _configuration;
        private SqlConnection conn;
        public BaseContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public void addColToQuery(bool add,ref string cols,ref string params_,string colName)
        {
            if (add==true)
            {
                if (cols.Trim() != "")
                {
                    cols += ",";
                }
                cols += $"[{colName}]";
                if (params_.Trim() != "")
                {
                    params_ += ",";
                }
                params_ += $"@{colName}";
            }
        }
        public void addColToUpdateQuery(bool add, ref string cols, string colName)
        {
            if (add == true)
            {
                if (cols.Trim() != "")
                {
                    cols += ",";
                }
                cols += $"{colName} = @{colName}";
            }
        }
        
        public void AddColToSqlCommand(ref SqlCommand sqlcmd,bool add,object value, string colName, System.Data.SqlDbType type)
        {
            if (add == true)
            {
                sqlcmd.Parameters.Add($"@{colName}", type);
                sqlcmd.Parameters[$"@{colName}"].Value = value;
            }
        }
        protected void AddToWhereClause(bool add, object obj,ref string where,string columnName,string columnParam,string operator_,string concat="") {
            if (add == true)
            {
                //if (where != "") { where += ","; }
                where += $" {concat} {columnName} {operator_} @{columnParam} ";
            }
        }
    }
}
