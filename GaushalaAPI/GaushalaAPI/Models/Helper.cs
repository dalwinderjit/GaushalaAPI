using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;

namespace GaushalaAPI.Models
{
    public class Helper
    {
        public static string PasswordRegEx { get; internal set; } = @"^[A-Za-z0-9.#@$]{8,20}$";
        public static string UsernameRegEx { get; internal set; } = @"^[a-zA-Z0-9.]{3,25}$";

        //public static string PasswordRegEx = "^[A-Za-z]$";
        public static string FormatDate(object date){
            try
            {
                if (date != null)
                {
                    DateTime date_ = (DateTime)date;
                    return date_.ToString("dd/MM/yyyy");// "2004-12-01T00:00:00",
                }
                else
                {
                    return "";
                }
            }
            catch(Exception e)
            {
                return "";
            }
        }
        
        public static string FormatLong(object number){
            string number_ = "";
            if (number != null)
            {
                try{
                    number_ = Convert.ToInt64(number).ToString();
                    return number_;
                }catch{
                    return number_;
                }
            }
            else
            {
                return "";
            }
        }
        
        public static object IsNullOrEmpty(object obj){
            if(obj==null || Convert.ToString(obj).Trim()==""){
                return "";
            }else{
                return obj;
            }
        }
        internal static object FormatDate2(object date)
        {
            try
            {
                if (date != null)
                {
                    DateTime date_ = (DateTime)date;
                    return date_.ToString("dd/MM/yyyy HH:mm:ss");// "2004-12-01T00:00:00",
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }
        internal static object FormatDate3(object date,string format="yyyy-MM-dd HH:mm:ss")
        {
            try
            {
                if (date != null)
                {
                    DateTime date_ = (DateTime)date;
                    return date_.ToString(format);// "2004-12-01T00:00:00",
                }
                else
                {
                    return "";
                }
            }
            catch (Exception e)
            {
                return "";
            }
        }
        
    }
}
