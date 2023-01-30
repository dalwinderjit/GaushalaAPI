using System;

namespace GaushalaAPI.Helpers
{
    public class Validations
    {
        public static string ValidTagNoRegEx = @"^[a-zA-Z]{1,5}-[0-9]{1,}$";
        public static bool ValidateCategory(object Category){
            if(Validations.IsNullOrEmpty(Category)==true){
                return true;
            }else{
                return false;
            }
        }
        public static bool IsNullOrEmpty(object obj){
            if(obj==null || Convert.ToString(obj).Trim()==""){
                return true;
            }else{
                return false;
            }
        }
    }
}