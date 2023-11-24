using Microsoft.AspNetCore.Mvc.RazorPages;
using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Data.SqlClient;

namespace WMFACS_Review_List.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;

        public IndexModel(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public IEnumerable<StaffMembers?> AdminList { get; set; }
        public IEnumerable<StaffMembers?> GCList { get; set; }
        public IEnumerable<StaffMembers?> ConsList { get; set; }
        public StaffMembers? StaffUser { get; set; }
        public IEnumerable<PatientReferrals?> PatientReferralsList { get; set; }
        public IEnumerable<AreaNames?> AreaNamesList { get; set; }
        public IEnumerable<AdminStatuses?> AdminStatusList { get; set; }
        public IEnumerable<Pathways?> PathwayList { get; set; }

        public string? StaffMemberName { get; set; }
       
        

        [Authorize]
        public void OnGet(string? sStaffCode, string? sAdminStatus, string? sPathway, int? iWeeks)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    StaffUser = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == User.Identity.Name);
                    if (sStaffCode is null && StaffUser.POSITION != "Admin Team Leader")
                    {
                        sStaffCode = StaffUser.STAFF_CODE;
                    }
                }

                AdminList = _context.StaffMembers.Where(s => s.CLINIC_SCHEDULER_GROUPS == "Admin").OrderBy(s => s.NAME).ToList();
                GCList = _context.StaffMembers.Where(s => s.CLINIC_SCHEDULER_GROUPS == "GC" && s.InPost == true).OrderBy(s => s.NAME).ToList();
                ConsList = _context.StaffMembers.Where(s => s.CLINIC_SCHEDULER_GROUPS == "Consultant" && s.InPost == true).OrderBy(s => s.NAME).ToList();

                AreaNamesList = _context.AreaNames.Where(a => a.InUse == true).OrderBy(a => a.AreaID);
                AdminStatusList = _context.AdminStatuses.ToList();
                PathwayList = _context.Pathways.ToList();
                PatientReferralsList = _context.PatientReferrals.Where(r => r.RefType.Contains("Refer") && r.COMPLETE != "Complete" && r.logicaldelete == false && r.Admin_Contact != null).ToList().OrderBy(r => r.WeeksFromReferral);

                int iDays;

                if (iWeeks != null)
                {
                    iDays = iWeeks.GetValueOrDefault() * 7;
                }
                else
                {
                    iDays = 56;
                }

                DateTime FromDate = DateTime.Now.AddDays(-iDays);

                PatientReferralsList = PatientReferralsList.Where(r => r.RefDate >= FromDate);

                if (sStaffCode != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.Admin_Contact == sStaffCode);
                    StaffMemberName = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == sStaffCode).NAME;
                }

                if (sAdminStatus != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.Status_Admin == sAdminStatus);
                }

                if (sPathway != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.PATHWAY == sPathway);
                }
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int? iAreaID, string? sAdmin, string? sGC, string? sConsultant, string? sAreaCode)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    StaffUser = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == User.Identity.Name);
                }

                //SqlConnection conn = new SqlConnection("Server=spinners;DataBase=Clinical_Dev;User Id=shire_user;Password=shire1;TrustServerCertificate=True");
                SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConString"));
                conn.Open();

                SqlCommand cmd = new SqlCommand("", conn);

                if (sAreaCode != null)
                {
                    cmd.CommandText = "Update ListAreaNames set FHCStaffCode='" + sAdmin + "', GC='" + sGC + "', ConsCode='" + sConsultant + "', WhoLastEdit='" + StaffUser.STAFF_CODE +
                "', DateLastEdit='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where AreaCode='" + sAreaCode + "'";
                }
                else
                {
                    cmd.CommandText = "Update ListAreaNames set FHCStaffCode='" + sAdmin + "', GC='" + sGC + "', ConsCode='" + sConsultant + "', WhoLastEdit='" + StaffUser.STAFF_CODE +
                    "', DateLastEdit='" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' where AreaID=" + iAreaID;
                }
                cmd.ExecuteNonQuery();

                AdminList = _context.StaffMembers.Where(s => s.CLINIC_SCHEDULER_GROUPS == "Admin").OrderBy(s => s.NAME).ToList();
                GCList = _context.StaffMembers.Where(s => s.CLINIC_SCHEDULER_GROUPS == "GC" && s.InPost == true).OrderBy(s => s.NAME).ToList();
                ConsList = _context.StaffMembers.Where(s => s.CLINIC_SCHEDULER_GROUPS == "Consultant" && s.InPost == true).OrderBy(s => s.NAME).ToList();

                AreaNamesList = _context.AreaNames.Where(a => a.InUse == true).OrderBy(a => a.AreaID);
                AdminStatusList = _context.AdminStatuses.ToList();
                PathwayList = _context.Pathways.ToList();
                PatientReferralsList = _context.PatientReferrals.Where(r => r.RefType.Contains("Refer") && r.COMPLETE != "Complete" && r.logicaldelete == false && r.Admin_Contact != null).ToList().OrderBy(r => r.WeeksFromReferral);

                DateTime FromDate = DateTime.Now.AddDays(-56);

                PatientReferralsList = PatientReferralsList.Where(r => r.RefDate >= FromDate);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }

        }        
        
    }
}