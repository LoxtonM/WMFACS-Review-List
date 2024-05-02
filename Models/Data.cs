using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace WMFACS_Review_List.Models //replace with your own data where appropriate
{
    [Table("STAFF", Schema = "dbo")]
    public class StaffMembers
    {
        [Key]
        public string STAFF_CODE { get; set; }
        public string? EMPLOYEE_NUMBER { get; set; }
        public string? NAME { get; set; }        
        public string? POSITION { get; set; }
        public bool InPost { get; set; }  
        public string CLINIC_SCHEDULER_GROUPS { get; set; }
    }

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

    [Table("PATHWAY", Schema = "dbo")]
    public class Pathways
    {
        [Key]
        public string CGU_Pathway { get; set; }
    }

    [Table("ViewPatientReferralDetails", Schema = "dbo")]   
    public class PatientReferrals
    {
        [Key]
        public int refid { get; set; }
        public string? CGU_No { get; set; }
        public int? WeeksFromReferral { get; set; }
        public int? DaysFromReferral { get; set; }
        public string? Status_Admin { get; set; }
        public DateTime? ClockStartDate { get; set; }
        public DateTime? ClockStopDate { get; set; }
        public DateTime? BreachDate { get; set; }
        public DateTime? RefDate { get; set; }
        public string? PATHWAY { get; set; }
        public string? Clics { get; set; }
        public string? RefType { get; set; }
        public string? Admin_Contact { get; set; }
        public string? COMPLETE { get; set; }
        public bool logicaldelete { get; set; }
        public string? FIRSTNAME { get; set; }
        public string? LASTNAME { get; set; }
    }

    [Table("vw_Appts_Diary_for_Review", Schema="dbo")]
    public class ActivityItems
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
