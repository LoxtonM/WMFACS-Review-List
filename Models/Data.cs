using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace WMFACS_Review_List.Models //replace with your own data where appropriate
{    
    
    [Table("ListAreaNames", Schema = "dbo")]
    public class AreaNames
    {
        [Key]
        public int AreaID { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string? FHCStaffCode { get; set; }
        public string? ConsCode { get; set; }
        public string? GC { get; set; }
        public bool InUse { get; set; }
        public string WhoLastEdit { get; set; }
        public DateTime DateLastEdit { get; set; }
    }

    [Table("ListStatusAdmin", Schema = "dbo")]
    public class AdminStatuses
    {
        [Key]
        public string? Status_Admin { get; set; }
    }
        
    [Table("vw_Appts_Diary_for_Review", Schema="dbo")]
    public class ReviewItems
    {
        [Key]
        public int ID { get; set; }
        public int RefID { get; set; }
        public string CGU_No { get; set; }
        public DateTime Date { get; set; }
        public string AppType_DiaryAction { get; set; }
        public string? Attended_DocCode { get; set; }
        public string DiaryText { get; set; }
        public string Staff1 { get; set; }
        public string Staff2 { get; set; }
        public string Staff3 { get; set; }
    }  

}
