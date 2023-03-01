using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;

namespace GaushalaAPI.Entities
{
    public class StaticData
    {
        Dictionary<long, string> PregnancyStatus;
        Dictionary<long, string> PregnancyStatus2 ;
        Dictionary<long, string> MatingType ;
        Dictionary<long, string> DeliveryType;
        Dictionary<long, string> DeliveryStatus;
        Dictionary<string, string> Genders;
        Dictionary<long, string> TeatsWorking;
        Dictionary<long, string> AnimalLocation;
        Dictionary<long, string> BirthStatusOptions;
        Dictionary<long, string> CountriesOptions;
        Dictionary<string, string> BullPerformancesTypes;
        Dictionary<string, string> MilkStatusOptions;
        public Dictionary<long, string> GetPregnancyStatusOptions()
        {
            return this.PregnancyStatus;
        }
        public Dictionary<long,string> GetPregnancyStatus2Options()
        {
            return this.PregnancyStatus2;
        }
        public Dictionary<long, string> GetMatingTypeOptions()
        {
            return this.MatingType;
        }
        public Dictionary<long, string> GetDeliveryTypeOptions()
        {
            return this.DeliveryType;
        }
        public Dictionary<string, string> GetGendersOptions()
        {
            return this.Genders;
        }
        public Dictionary<long, string> GetTeatsWorkingOptions()
        {
            return this.TeatsWorking;
        }
        public Dictionary<long, string> GetDeliveryStatus()
        {
            return this.DeliveryStatus;
        }
        public Dictionary<long, string> GetAnimalLocationsOptions()
        {
            return this.AnimalLocation;
        }
        public Dictionary<long, string> GetBirthStatusOptions()
        {
            return this.BirthStatusOptions;
        }
        public Dictionary<string,string> GetBullPerformances()
        {
            return this.BullPerformancesTypes;
        }
        public Dictionary<string,string> GetMilkStatusOptions()
        {
            return this.MilkStatusOptions;
        }
        
        public StaticData(){
            PregnancyStatus = new Dictionary<long, string>();
            PregnancyStatus[0] = "Pregnant";
            PregnancyStatus[1] = "Not Pregnant";
            //pregnancy Status for Conceive Data 
            PregnancyStatus2 = new Dictionary<long, string>();
            PregnancyStatus2[1] = "Confirmed";
            PregnancyStatus2[2] = "Pending";
            PregnancyStatus2[3] = "Failed";
            PregnancyStatus2[4] = "Successful";
            //matingType
            MatingType = new Dictionary<long, string>();
            MatingType[1] = "Natural";
            MatingType[2] = "Artificial";
            //deliveryType
            DeliveryType = new Dictionary<long, string>();
            DeliveryType[1] = "Natural";
            DeliveryType[2] = "Artificial";
            //gender
            Genders = new Dictionary<string, string>();
            Genders["Male"] = "Male";
            Genders["Female"] = "Female";
            //TeatsWorking
            TeatsWorking = new Dictionary<long, string>();
            TeatsWorking[0] = "0 Teats Working";
            TeatsWorking[1] = "1 Teats Working";
            TeatsWorking[2] = "2 Teats Working";
            TeatsWorking[3] = "3 Teats Working";
            TeatsWorking[4] = "4 Teats Working";
            //CowLocation
            AnimalLocation = new Dictionary<long, string>();
            AnimalLocation[1] = "Shed A";
            AnimalLocation[2] = "Shed B";
            AnimalLocation[3] = "Shed C";
            AnimalLocation[4] = "Shed D";
            AnimalLocation[5] = "Shed E";
            //Bull Performances Type
            BullPerformancesTypes = new Dictionary<string, string>();
            BullPerformancesTypes["SEMEN"] = "Artificial Intelligence";
            BullPerformancesTypes["NATURAL"] = "Natural";
            //Deliver Status
            DeliveryStatus = new Dictionary<long, string>();
            DeliveryStatus[1] = "Normal";
            DeliveryStatus[2] = "Abnormal";
            DeliveryStatus[3] = "Child Died";
            //Deliver Status
            MilkStatusOptions = new Dictionary<string, string>();
            MilkStatusOptions["START"] = "Start";
            MilkStatusOptions["STOP"] = "Stop";
            //Birth Status Options
            BirthStatusOptions= new Dictionary<long, string>();
            BirthStatusOptions[1] = "Normal";
            BirthStatusOptions[2] = "Abnormal";
            BirthStatusOptions[3] = "Child Died";
            
        }
    }
}
