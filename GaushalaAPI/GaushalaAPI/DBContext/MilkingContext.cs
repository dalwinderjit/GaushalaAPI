using GaushalaAPI.Models;
using GaushalAPI.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GaushalaAPI.DBContext
{
    public class MilkingContext 
    {
        private readonly IConfiguration _configuration;
        public MilkingContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public ArrayList GetMilkingDetailByCowId(long CowId)
        {
            ArrayList ar = new ArrayList();
            CowsContext cowsContext = new CowsContext(this._configuration);
            Dictionary<string,string> cow_detail = cowsContext.GetCowTagNoNameById(CowId);
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                //string query = $"Select * from MilkingDetail where CowID = @CowID order by Date ASC";
                //string query = $"Select Date,Lactation,CowId,Total as qty from CowMilking where CowID = @CowID  order by Date Asc";
                string query = $"Select Date,Lactation,CowId,sum(Total) as qty from CowMilking where CowID = @CowID  group by Date,Lactation,CowID order by Date Asc";
                Console.Write(query);
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@CowID"].Value = CowId;
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int lactationNo = 0;
                    
                    Dictionary<int, LactationDetail> milking = new Dictionary<int, LactationDetail>();
                    decimal quantity = 0;
                    decimal peakYield = 0;
                    int days = 0;
                    LactationDetail lactation;
                    while (sqlrdr.Read())
                    {
                        Console.WriteLine(sqlrdr["Lactation"]);
                        try
                        {
                            lactationNo = Convert.ToInt16(sqlrdr["Lactation"]);
                        }catch(Exception e)
                        {
                            lactationNo = 0;
                        }
                        Console.WriteLine(lactationNo);
                        try
                        {
                            lactation = milking[lactationNo];
                            decimal qty = Convert.ToDecimal(sqlrdr["qty"]);
                            //lactation.lactation = lactationNo;
                            if (lactation.PeakYield < qty)
                            {
                                lactation.PeakYield = qty;
                            }
                            lactation.totalQuantity += qty;
                            if (lactation.WetDays < 305)
                            {
                                lactation.Quantity305days += qty;
                            }
                            ++lactation.WetDays;
                        }
                        catch(Exception e)
                        {
                            lactation = new LactationDetail();
                            lactation.lactation = lactationNo;
                            decimal qty = Convert.ToDecimal(sqlrdr["qty"]);
                            lactation.PeakYield = qty;
                            lactation.totalQuantity += qty;
                            if (lactation.WetDays < 365)
                            {
                                lactation.Quantity305days += qty;
                            }
                            lactation.WetDays++;
                        }
                        milking[lactationNo] = lactation;
                    }
                    foreach(var m in milking)
                    {
                        Dictionary<string, object> data = new Dictionary<string, object>();
                        data["CowName"] = cow_detail["name"] + " / " + cow_detail["tagNo"];
                        lactation = (LactationDetail)m.Value;
                        data["LactationNo"] = lactation.lactation;
                        data["TotalYield"] = lactation.totalQuantity;
                        data["Total305DaysYield"] = lactation.Quantity305days;
                        data["WetDays"] = lactation.WetDays;
                        data["PeakYield"] = lactation.PeakYield;
                        data["Average"] = lactation.totalQuantity / lactation.WetDays;
                        ar.Add(data);
                        data = null;
                    }
                    sqlrdr.Close();
                    conn.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + ex.StackTrace);
                }

            }
            return ar;
        }

        internal Dictionary<string, object> GetCowMilkingComaprisonData(List<long> cowIDs, int comparisonType,List<int> lactations, int? dayFrom, int? dayTo, DateTime? DateFrom, DateTime? DateTo,int? daysSeparator)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            string cow_ids = "";
            Dictionary<int,string> labels = new Dictionary<int, string>();
            if (cowIDs.Count > 0)
            {
                foreach (long id in cowIDs)
                {
                    if (cow_ids != "")
                    {
                        cow_ids += ",";
                    }
                    cow_ids += id.ToString();
                }
            }
            else
            {
                return data;
            }
            CowsContext cowsContext = new CowsContext(this._configuration);
            Dictionary<long, object> cows = cowsContext.GetCowTagNoNameByIds(cowIDs);
            Dictionary<long, object> cows_ = new Dictionary<long, object>();
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            int max_lactation_no = 0;
            decimal max_quantity_overall = 0;
            Dictionary<string, object> cowLactaionDetailData = MilkingContext.GetCowLactaionDetail(this._configuration, cowIDs);
            Dictionary<string, string> cowLactaionDetailData_ = (Dictionary<string, string>)cowLactaionDetailData["data"];
            Dictionary<long, Dictionary<int, DateTime>> cowLactationDetail; ;
            if (cowLactaionDetailData_["status"] == "success")
            {
                cowLactationDetail = (Dictionary<long, Dictionary<int, DateTime>>)cowLactaionDetailData["records"];
            }
            else
            {
                cowLactationDetail = new Dictionary<long, Dictionary<int, DateTime>>();
            }
            
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "";
                if(comparisonType == 1)//Lactation
                {
                    string lactations_ = "";
                    if (lactations.Count > 0)
                    {
                        foreach(int l in lactations)
                        {
                            if (lactations_ != "")
                            {
                                lactations_ += ",";
                            }
                            lactations_ += l.ToString();
                        }
                    }
                    if (lactations_ != "")
                    {
                        lactations_ = $" and Lactation in ({lactations_})";
                    }
                    if(dayFrom==null || dayTo==null){
                        query = $"Select Lactation,CowID,sum(Total) as qty from CowMilking where CowID in ({cow_ids})  {lactations_} " +
                    " group by Lactation,CowId";
                    }else{
                        query = $"Select Date,Lactation,CowID,sum(Total) as qty from CowMilking where CowID in ({cow_ids})  {lactations_} " +
                    " group by Date,Lactation,CowId order by Date Asc";
                    }
                    Console.Write(query);
                    SqlCommand sqlcmd = new SqlCommand(query, conn);

                    Dictionary<long, MilkComparison> cows__ = new Dictionary<long, MilkComparison>();

                    try
                    {
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        int lactationNo = 0;
                        Dictionary<int, MilkComparison> milking;// = new Dictionary<int, decimal>();
                        MilkComparison comparison;
                        int i = 0;

                        while (sqlrdr.Read())
                        {
                            DateTime date = new DateTime();
                            Console.WriteLine(i + "  " + sqlrdr["CowID"]); i++;
                            int cowID = 0;
                            try
                            {
                                cowID = Convert.ToInt32(sqlrdr["CowID"]);
                            }
                            catch (Exception e)
                            {
                                cowID = 0;
                            }
                            try
                            {
                                milking = (Dictionary<int, MilkComparison>)cows_[cowID];
                            }
                            catch (Exception e)
                            {
                                milking = new Dictionary<int, MilkComparison>();
                            }
                            try
                            {
                                Console.WriteLine("Lactaion " + sqlrdr["Lactation"]);
                                lactationNo = Convert.ToInt32(sqlrdr["Lactation"]);
                            }
                            catch
                            {
                                lactationNo = 0;
                            }
                            if (max_lactation_no < lactationNo)
                            {
                                max_lactation_no = lactationNo;
                            }
                            try
                            {
                                comparison = milking[lactationNo];
                                Console.WriteLine("Comparison FOUND");
                            }
                            catch (Exception e)
                            {
                                comparison = new MilkComparison();
                                comparison.Quantity = 0;
                                comparison.OtherQuantity = 0;
                                comparison.LactaionNo = lactationNo;
                            }
                            Dictionary<int, DateTime> dd;
                            if (dayFrom != null && dayTo != null)
                            {
                                date = (DateTime)sqlrdr["Date"];
                                Console.WriteLine("Date: " + date);
                            }
                            if (cowID != 0 && lactationNo != 0 && dayFrom != null && dayTo != null)
                            {
                                try
                                {
                                    if (cowID != 0)
                                    {
                                        dd = cowLactationDetail[cowID];
                                        try
                                        {
                                            DateTime date_ = dd[lactationNo];
                                            comparison.StartDate = date_;
                                        }
                                        catch (Exception e2)
                                        {

                                        }
                                    }
                                }
                                catch (Exception e3)
                                {

                                }
                            }
                            if (comparison.StartDate.ToString("dd/MM/yyyy") == "01/01/0001")
                            {
                                comparison.StartDate = date;
                            }
                            Console.WriteLine("Start Date" + comparison.StartDate.ToString("dd/MM/yyyy"));

                            decimal qty;
                            if (dayFrom != null && dayTo != null)
                            {
                                //10 20
                                //Console.WriteLine("Subtraction : " + comparison.StartDate + ", Date " + date);
                                //Console.WriteLine(date.Subtract(comparison.StartDate).Days + " >= " + dayFrom + " && " + date.Subtract(comparison.StartDate).Days + " < " +  dayTo);
                                if (date.Subtract(comparison.StartDate).Days >= dayFrom && date.Subtract(comparison.StartDate).Days < dayTo)
                                {
                                    try
                                    {
                                        qty = Convert.ToDecimal(sqlrdr["qty"]);
                                    }
                                    catch (Exception e)
                                    {
                                        qty = 0;
                                    }
                                    Console.WriteLine("Date " + date.ToString("dd/MM/yyyy") + " Qty " + qty);
                                    comparison.Quantity += qty;
                                    if (comparison.Quantity > max_quantity_overall)
                                    {
                                        max_quantity_overall = (decimal)comparison.Quantity;
                                    }
                                    milking[lactationNo] = comparison;
                                    cows_[cowID] = milking;
                                }
                                else
                                {
                                    try
                                    {
                                        qty = Convert.ToDecimal(sqlrdr["qty"]);
                                    }
                                    catch (Exception e)
                                    {
                                        qty = 0;
                                    }
                                    Console.WriteLine($"Date {date} Qty " + qty);
                                    comparison.OtherQuantity += qty;
                                    milking[lactationNo] = comparison;
                                    cows_[cowID] = milking;
                                }
                            }
                            else
                            {
                                try
                                {
                                    qty = Convert.ToDecimal(sqlrdr["qty"]);
                                }
                                catch (Exception e)
                                {
                                    qty = 0;
                                }
                                Console.WriteLine("Qty " + qty);
                                comparison.Quantity += qty;
                                if (comparison.Quantity > max_quantity_overall)
                                {
                                    max_quantity_overall = (decimal)comparison.Quantity;
                                }
                                milking[lactationNo] = comparison;
                                cows_[cowID] = milking;
                            }
                        }
                        sqlrdr.Close();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace);
                    }
                }
                else if(comparisonType == 2)
                {
                    string fromDate = ((DateTime)DateFrom).ToString("yyyy-MM-dd HH:mm:ss");
                    string toDate = ((DateTime)DateTo).ToString("yyyy-MM-dd HH:mm:ss");
                    query = $"Select Date,CowId,sum(Quantity) as qty from MilkingDetail where CowID in ({cow_ids}) and Date >= '{fromDate}' " +
                        $" and Date <= '{toDate}' group by Date,CowId order by Date ASC";
                    Console.Write(query);
                    int TotalNoOfDays = ((DateTime)DateTo).Subtract(((DateTime)DateFrom)).Days;
                    Console.WriteLine("TOtal NO of Days " + TotalNoOfDays);
                    int blocks;
                    Dictionary<int, MilkComparison> milking;
                    MilkComparison milk_comparison;
                    Console.WriteLine("days Separator " + daysSeparator);
                    if (daysSeparator != null && daysSeparator!=0)
                    {
                        blocks = TotalNoOfDays / (int)daysSeparator;
                        int rem = TotalNoOfDays % (int)daysSeparator;
                        if (rem > 0)
                        {
                            blocks++;
                        }
                    }
                    else
                    {
                        blocks = 1;
                        daysSeparator = TotalNoOfDays;
                    }
                    Console.WriteLine("BLOCKS " + blocks);
                    foreach(long cow_id in cowIDs)
                    {
                        milking = new Dictionary<int, MilkComparison>();
                        for(int j = 1; j <= blocks; j++)
                        {
                            
                            milk_comparison = new MilkComparison();
                            milk_comparison.DateFrom = ((DateTime)DateFrom).AddDays((j - 1) * (int)daysSeparator);
                            if (j == blocks)
                            {
                                milk_comparison.DateTo = ((DateTime)DateTo);
                            }
                            else
                            {
                                milk_comparison.DateTo = ((DateTime)DateFrom).AddDays(j * (int)daysSeparator-1);
                            }
                            labels[j] = milk_comparison.DateFrom.ToString("dd MMM y") + " - " + milk_comparison.DateTo.ToString("dd MMM y");
                            milking[j] = milk_comparison;
                        }
                        cows_[cow_id] = milking;
                    }
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    Dictionary<long, MilkComparison> cows__ = new Dictionary<long, MilkComparison>();
                    try
                    {
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        int lactationNo = 0;
                        // = new Dictionary<int, decimal>();
                        MilkComparison comparison;
                        int i = 0;
                        while (sqlrdr.Read())
                        {
                            DateTime date = (DateTime)sqlrdr["Date"];
                            Console.WriteLine(i + "  " + sqlrdr["CowID"]); i++;
                            int cowID = 0;
                            
                            try
                            {
                                cowID = Convert.ToInt32(sqlrdr["CowID"]);
                            }
                            catch (Exception e)
                            {
                                cowID = 0;
                            }
                            try
                            {
                                milking = (Dictionary<int, MilkComparison>)cows_[cowID];
                            }
                            catch (Exception e)
                            {
                                milking = new Dictionary<int, MilkComparison>();
                            }
                            int day = date.Subtract((DateTime)DateFrom).Days+1;
                            int block = this.GetSeparatorBlock(TotalNoOfDays, (int)daysSeparator, day);
                            try
                            {
                                Console.WriteLine("Lactaion " + sqlrdr["Lactation"]);
                                lactationNo = Convert.ToInt32(sqlrdr["Lactation"]);
                            }
                            catch
                            {
                                lactationNo = 0;
                            }
                            if (max_lactation_no < lactationNo)
                            {
                                max_lactation_no = lactationNo;
                            }
                            try
                            {
                                comparison = milking[block];
                                comparison.LactaionNo = lactationNo;
                                Console.WriteLine("Comparison FOUND");
                            }
                            catch (Exception e)
                            {
                                comparison = new MilkComparison();
                                comparison.Quantity = 0;
                                comparison.OtherQuantity = 0;
                                comparison.LactaionNo = lactationNo;
                            }

                            Dictionary<int, DateTime> dd;
                            
                            decimal qty;
                            try
                            {
                                qty = Convert.ToDecimal(sqlrdr["qty"]);
                            }
                            catch (Exception e)
                            {
                                qty = 0;
                            }
                            Console.WriteLine("Qty " + qty);
                            comparison.Quantity += qty;
                            if (comparison.Quantity > max_quantity_overall)
                            {
                                max_quantity_overall = (decimal)comparison.Quantity;
                            }
                            milking[block] = comparison;
                            cows_[cowID] = milking;
                        }
                        sqlrdr.Close();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message + ex.StackTrace);
                    }
                }
                //string query = $"Select * from MilkingDetail where CowID = @CowID order by Date ASC";
                
                
            }
            Console.WriteLine("Max Quantity : " + max_quantity_overall);
            foreach(var m in cows_){
                var n = (Dictionary<int, MilkComparison>)m.Value;
                float percentage = 0;
                List<int> lactation_numbers = new List<int>();
                foreach (var g in n)
                {
                    lactation_numbers.Add(g.Key);
                    if (max_quantity_overall > 0 && g.Value.Quantity > 0)
                    {
                        percentage = (float)Math.Round((decimal)((g.Value.Quantity / max_quantity_overall) * 100),2);
                    }
                    g.Value.percentage = percentage;
                }
                if (comparisonType == 1)//Lactation
                {
                    if (lactations != null && lactations.Count > 0)
                    {
                        foreach (int i in lactations)
                        {
                            labels[i] = $"Lactation {i}";
                            if (lactation_numbers.Contains(i) == false)
                            {
                                MilkComparison m_c = new MilkComparison();
                                m_c.LactaionNo = i;
                                m_c.Quantity = 0;
                                n[i] = m_c;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i <= max_lactation_no; i++)
                        {
                            labels[i] = $"Lactation {i}";
                            if (lactation_numbers.Contains(i) == false)
                            {
                                MilkComparison m_c = new MilkComparison();
                                m_c.LactaionNo = i;
                                m_c.Quantity = 0;
                                n[i] = m_c;
                            }
                        }
                    }
                }else if (comparisonType == 2)
                {

                }
                cows_[m.Key] = n;
            }
            data["data"] = cows_;
            data["cow_detail"] = cows;
            data["labels"] = labels;
            return data;
        }
        public int GetSeparatorBlock(int NumberOfBlocks,int Separator,int day)
        {
            int i = day / Separator;
            int rem = day % Separator;
            if (rem > 0) {
                i++;
            }
            return i;
        }
        private static Dictionary<string, object> GetCowLactaionDetail(IConfiguration _configuration, List<long> cowIDs)
        {
            Dictionary<long, Dictionary<int, DateTime>> data = new Dictionary<long, Dictionary<int, DateTime>>();
            Dictionary<string, object> data_ = new Dictionary<string, object>();
            ArrayList rows = new ArrayList();
            string cow_ids = "";
            if (cowIDs.Count > 0)
            {
                foreach (long id in cowIDs)
                {
                    if (cow_ids != "")
                    {
                        cow_ids += ",";
                    }
                    cow_ids += id.ToString();
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Failed TO retrieve Data.";
                data2["status"] = "failure";
                data_["data"] = data2;
                return data_;
            }
            string query = $"select CowID, DeliveryDate,LactationNo from  CowConceiveData where CowID in ({cow_ids}) and (PregnancyStatus = 'Successful' or PregnancyStatus='4') and (DeliveryStatus in ('1','2','Normal','Abnormal'))";
            Console.WriteLine(query);
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    while (sqlrdr.Read())
                    {
                        Dictionary<int, DateTime> lactaionDetail;
                        try
                        {
                            lactaionDetail = data[Convert.ToInt64(sqlrdr["CowID"])];
                        }catch(Exception e)
                        {
                            lactaionDetail = new Dictionary<int, DateTime>();
                        }
                        lactaionDetail[Convert.ToInt32(sqlrdr["LactationNo"])] = (DateTime)sqlrdr["DeliveryDate"];
                        data[Convert.ToInt64(sqlrdr["CowID"])] = lactaionDetail;
                    }
                    conn.Close();
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Data Retrieved Successfully.";
                    data2["status"] = "success";
                    data_["data"] = data2;
                    data_["records"] = data;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e.Message + e.StackTrace);
                    Console.WriteLine("Stack " + e.StackTrace);
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Failed TO retrieve Data (" + e.Message + ")";
                    data2["status"] = "failure";
                    data_["data"] = data2;
                }
            }
            return data_;
        }

        internal Dictionary<string, object> GetTotalMilkStartStopById(long id)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            ArrayList rows = new ArrayList();
            string query = $"select * from  CowMilkStartStopDetail where Id = @Id";
            Console.WriteLine(query);
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@Id"].Value = id;
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    if (sqlrdr.Read())
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        row["Id"] = sqlrdr["Id"];
                        row["CowID"] = Helper.IsNullOrEmpty(sqlrdr["CowID"]);
                        row["LactationNo"] = Helper.IsNullOrEmpty(sqlrdr["LactationNo"]);
                        row["MilkStatus"] = sqlrdr["MilkStatus"].ToString();
                        row["Date"] = Helper.FormatDate3(sqlrdr["Date"]);
                        row["Reason"] = Helper.IsNullOrEmpty(sqlrdr["Reason"].ToString()) ;
                        rows.Add(row);
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Record Found";
                        data2["status"] = "success";
                        data["milkingDetail"] = row;
                        data["data"] = data2;
                    }
                    else
                    {
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Record Not Found";
                        data2["status"] = "failure";
                        data["data"] = data2;
                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e.Message + e.StackTrace);
                    Console.WriteLine("Stack " + e.StackTrace);
                    Dictionary<string, string> data2 = new Dictionary<string, string>();
                    data2["message"] = "Failed TO retrieve Data (" + e.Message + ")";
                    data2["status"] = "failure";
                    data["data"] = data2;
                }
            }
            return data;
        }

        internal Dictionary<string, object> GetCowMilkingStartStopDetailByCowId(long cowID)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            ArrayList rows = new ArrayList();
            string query = $"select * from  CowMilkStartStopDetail where CowID = @CowID";
            Console.WriteLine(query);
            string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlcmd = new SqlCommand(query, conn);
                sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                sqlcmd.Parameters["@CowID"].Value = cowID;
                try
                {
                    conn.Open();
                    SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                    int sno = 1;
                    while (sqlrdr.Read())
                    {
                        Dictionary<string, object> row = new Dictionary<string, object>();
                        row["sno"] = sno;
                        row["id"] = sqlrdr["Id"];
                        row["cowID"] = sqlrdr["CowID"];
                        row["lactationNo"] = sqlrdr["LactationNo"];
                        row["MilkStatus"] = sqlrdr["MilkStatus"].ToString();
                        row["Date"] = Helper.FormatDate(sqlrdr["Date"]);
                        row["Reason"] = sqlrdr["Reason"].ToString(); ;
                        rows.Add(row);
                        sno++;
                    }
                    conn.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception : " + e.Message + e.StackTrace);
                    Console.WriteLine("Stack " + e.StackTrace);
                }
                data["data"] = rows;
                data["recordsTotal"] = this.GetTotalMilkStartStopByCowId(cowID);
                data["recordsFiltered"] = data["recordsTotal"];
            }
            return data;
        }
        public long GetTotalMilkStartStopByCowId(long? cowID)
        {
            long counter = 0;
            if (cowID != null)
            {
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    //Console.WriteLine("Select count(*) as total from Animals where TagNo = @TagNo");
                    SqlCommand sqlcmd = new SqlCommand("Select count(*) as total from CowMilkStartStopDetail where CowID = @CowID", conn);
                    try
                    {
                        sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                        sqlcmd.Parameters["@CowID"].Value = cowID;
                        conn.Open();
                        SqlDataReader sqlrdr = sqlcmd.ExecuteReader();
                        while (sqlrdr.Read())
                        {
                            counter = Convert.ToInt64(sqlrdr["total"]);
                        }
                        sqlrdr.Close();
                        conn.Close();
                    }
                    catch (Exception ex)
                    {
                        return counter;
                    }
                }
            }
            return counter;
        }


        internal Dictionary<string, object> SaveCowMilkingStartStop(MilkingStartStop milkingss)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            
            if (milkingss.Validate("Add") == true)
            {
                string cols = "CowID,LactationNo,Date,MilkStatus";
                string values = "@CowID,@LactationNo,@Date,@MilkStatus";
                
                if (milkingss.Reason != null)
                {
                    cols += ",Reason";
                    values += ",@Reason";
                }
                string query = $"Insert into CowMilkStartStopDetail ({cols}) OUTPUT INSERTED.ID Values" +
                $"({values})";
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@CowID"].Value =milkingss.CowID;
                    sqlcmd.Parameters.Add("@LactationNo", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@LactationNo"].Value =milkingss.LactationNo;
                    sqlcmd.Parameters.Add("@Date", System.Data.SqlDbType.DateTime);
                    sqlcmd.Parameters["@Date"].Value = milkingss.Date;
                    sqlcmd.Parameters.Add("@MilkStatus", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@MilkStatus"].Value = milkingss.MilkStatus;
                    if (milkingss.Reason != null)
                    {
                        sqlcmd.Parameters.Add("@Reason", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@Reason"].Value = milkingss.Reason;
                    }
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("milkingss");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        Console.WriteLine("HELLO");
                        milkingss.Id = (Int64)sqlcmd.ExecuteScalar();
                        Console.WriteLine("Bi");
                        Dictionary<string,object> cowMilkStatus= CowsContext.SetCowMilkingStatus(this._configuration,milkingss.CowID,milkingss.MilkStatus,conn,tran);
                        Dictionary<string, string> cowMilkStatsus_ = (Dictionary<string, string>)cowMilkStatus["data"];
                        if(cowMilkStatsus_["status"]=="success"){
                            if(milkingss.Id >0){
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Cow milkingss Start/Stop Data Saved Successfully."+cowMilkStatsus_["message"];
                                data2["status"] = "success";
                                data["MilkingStatus"] = cowMilkStatus["MilkingStatus"];
                                data["data"] = data2;
                            }else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Cow milkingss Date Saving Failed";
                                data2["status"] = "failure";
                                data["data"] = data2;
                            }
                            
                        }
                        else
                        {
                            tran.Rollback();
                            Console.WriteLine("rolling back second");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow milkingss Date Saving Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                        conn.Close();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Exception : "+e.Message + e.StackTrace);
                        Console.WriteLine("Stack " + e.StackTrace);
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Cow milkingss Data Saving Failed ("+e.Message+")";
                        data2["status"] = "failure";
                        data["data"] = data2;
                    }
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Cow milkingss Date Saving Failed.Invalid Data Entered";
                data2["status"] = "failure";
                data["errors"] = milkingss.errors;
                data["data"] = data2;
            }
            return data;
        }
        internal Dictionary<string, object> UpdateCowMilkingStartStop(MilkingStartStop milkingss)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();

            if (milkingss.Validate("Edit") == true)
            {
                string setCols = "CowID = @CowID, LactationNo = @LactationNo, Date = @Date, MilkStatus = @MilkStatus";
                if (milkingss.Reason != null)
                {
                    setCols += ",Reason = @Reason";
                }
                string query = $"Update CowMilkStartStopDetail set {setCols} where Id = @Id";
                Console.WriteLine(query);
                string connectionString = _configuration.GetConnectionString("GaushalaDatabaseConnectionString");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand sqlcmd = new SqlCommand(query, conn);
                    sqlcmd.Parameters.Add("@Id", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@Id"].Value = milkingss.Id;

                    sqlcmd.Parameters.Add("@CowID", System.Data.SqlDbType.BigInt);
                    sqlcmd.Parameters["@CowID"].Value = milkingss.CowID;
                    sqlcmd.Parameters.Add("@LactationNo", System.Data.SqlDbType.Int);
                    sqlcmd.Parameters["@LactationNo"].Value = milkingss.LactationNo;
                    sqlcmd.Parameters.Add("@Date", System.Data.SqlDbType.DateTime);
                    sqlcmd.Parameters["@Date"].Value = milkingss.Date;
                    sqlcmd.Parameters.Add("@MilkStatus", System.Data.SqlDbType.VarChar);
                    sqlcmd.Parameters["@MilkStatus"].Value = milkingss.MilkStatus;
                    if (milkingss.Reason != null)
                    {
                        sqlcmd.Parameters.Add("@Reason", System.Data.SqlDbType.VarChar);
                        sqlcmd.Parameters["@Reason"].Value = milkingss.Reason;
                    }
                    SqlTransaction tran = null;
                    try
                    {
                        conn.Open();
                        tran = conn.BeginTransaction("milkingss");
                        tran.Save("save1");
                        sqlcmd.Transaction = tran;
                        Console.WriteLine("HELLO");
                        int i = sqlcmd.ExecuteNonQuery();
                        Console.WriteLine("Bi");
                        Dictionary<string, object> cowMilkStatus = CowsContext.SetCowMilkingStatus(this._configuration, milkingss.CowID, milkingss.MilkStatus, conn, tran);
                        Dictionary<string, string> cowMilkStatsus_ = (Dictionary<string, string>)cowMilkStatus["data"];
                        if (cowMilkStatsus_["status"] == "success")
                        {
                            if (i > 0)
                            {
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                tran.Commit();
                                Console.WriteLine("Commiting");
                                data2["message"] = "Cow milkingss Start/Stop Data Updated Successfully." + cowMilkStatsus_["message"];
                                data2["status"] = "success";
                                data["MilkingStatus"] = cowMilkStatus["MilkingStatus"];
                                data["data"] = data2;
                            }
                            else
                            {
                                tran.Rollback();
                                Console.WriteLine("rolling back");
                                Dictionary<string, string> data2 = new Dictionary<string, string>();
                                data2["message"] = "Cow milkingss Date Saving Failed";
                                data2["status"] = "failure";
                                data["data"] = data2;
                            }

                        }
                        else
                        {
                            tran.Rollback();
                            Console.WriteLine("rolling back second");
                            Dictionary<string, string> data2 = new Dictionary<string, string>();
                            data2["message"] = "Cow milkingss Date Saving Failed";
                            data2["status"] = "failure";
                            data["data"] = data2;
                        }
                        conn.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception : " + e.Message + e.StackTrace);
                        Console.WriteLine("Stack " + e.StackTrace);
                        Dictionary<string, string> data2 = new Dictionary<string, string>();
                        data2["message"] = "Cow milkingss Data Saving Failed (" + e.Message + ")";
                        data2["status"] = "failure";
                        data["data"] = data2;
                    }
                }
            }
            else
            {
                Dictionary<string, string> data2 = new Dictionary<string, string>();
                data2["message"] = "Cow milkingss Date Saving Failed.Invalid Data Entered";
                data2["status"] = "failure";
                data["errors"] = milkingss.errors;
                data["data"] = data2;
            }
            return data;
        }
    }
}
