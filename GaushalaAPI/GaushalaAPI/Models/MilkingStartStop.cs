using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace GaushalaAPI.Models
{
    public class MilkingStartStop
    {
        public Dictionary<string, string>? errors { get; set; }
        public long Id { get; set; }
        public long CowID { get; set; }
        public int? LactationNo { get; set; }
        public DateTime? Date { get; set; }
        public string? MilkStatus { get; set; }
        public string? Reason { get; set; }
        
        public MilkingStartStop()
        {

        }
        public bool Validate(string type="Add")
        {
            bool error = true;
            errors = new Dictionary<string, string>();
            if (type == "Edit")
            {
                if (this.Id == null)
                {
                    errors.Add("Id", "Please Select the Record");
                    error = false;
                }
            }
            if (this.Date == null)
            {
                errors.Add("Date", "Enter the Milk Start/Stop Date");
                error = false;
            }
            if (this.CowID == null)
            {
                errors.Add("CowID", "Please select the Cow");
                error = false;
            }
            if (this.LactationNo == null)
            {
                errors.Add("LactationNo", "Please enter the Lactation Number");
                error = false;
            }
            /*else if(){
                errors.Add("LactationNo", "This Lactaion No Are");
                error = false;
            }*/
            if (this.Reason == null)
            {
                errors.Add("Reason", "Please enter the Reason");
                error = false;
            }
            return error;
        }
       
        public MilkingStartStop(SqlDataReader sqlrdr) {
            Id = Convert.ToInt32(sqlrdr["Id"]);
            try
            {
                CowID = Convert.ToInt64(sqlrdr["CowID"]);
            }catch(Exception e)
            {
                //Console.WriteLine("TagNo not found");
            }
            try
            {
                LactationNo = Convert.ToInt32(sqlrdr["LactationNo"]);
            }catch(Exception e) 
            {
                //Console.WriteLine("Name not found");
            }
            try
            {
                Date= DateTime.Parse(Convert.ToString(sqlrdr["Date"]));
            }catch(Exception e) 
            {
                //Console.WriteLine("dob not found");
            }
            try{
                MilkStatus = Convert.ToString(sqlrdr["MilkStatus"]);
            }catch (Exception e)
            {
                //Console.WriteLine("Gender not found");
            }
            try { 
                Reason = Convert.ToString(sqlrdr["Reason"]);
            }
            catch (Exception e)
            {
                //Console.WriteLine("Breed not found");
            }

        }
        public static Dictionary<string, object> GetFormatedData(MilkingStartStop milkingStartStop)
        {
            Dictionary<string, object> milking = new Dictionary<string, object>();
            milking["id"] = milkingStartStop.Id;
            milking["cowID"] = milkingStartStop.CowID;
            milking["lactationNo"] = milkingStartStop.LactationNo;
            milking["milkStatus"] = milkingStartStop.MilkStatus;
            if (milkingStartStop.Date != null)
            {
                milking["Date"] = ((DateTime)milkingStartStop.Date).ToString("dd/MM/yyyy");// "2004-12-01T00:00:00",
            }
            else
            {
                milking["dob"] = milkingStartStop.Date.ToString();
            }
            milking["reason"] = milkingStartStop.Reason;
            return milking;
        }
    }
}
