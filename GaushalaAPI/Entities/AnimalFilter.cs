using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GaushalAPI.Entities
{
    public class AnimalFilter
    {
        public long? Id { get; set; }
        public long[]? Ids { get; set; }
        public string? TagNo { get; set; }
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? OrderBy { get; set; }
        public string? Order { get; set; }
        public bool? GetCategory { get; set; }
        public int? PageNo { get; set; }
        public int? RecordsPerPage { get; set; }
        public int? Start { get; set; }
        public int? Length { get; set; }

        public string[]? cols { get; set; }
        
        
        public void CalculateStartLength(){
            if(this.PageNo!=null && this.RecordsPerPage!=null){
                this.Length = this.RecordsPerPage;
                this.Start = (int)((this.PageNo-1)*this.RecordsPerPage);   //0 2 3 
            }
        }
        public int GetStart(){
            if(this.Start!=null){
                return (int)this.Start;
            }else{
                return 1;
            }
        }
    }
}