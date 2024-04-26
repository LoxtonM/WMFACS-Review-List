using Microsoft.AspNetCore.Mvc.RazorPages;
using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Data.SqlClient;
using WMFACS_Review_List.Metadata;
using System.Xml;

namespace WMFACS_Review_List.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly DataServices _ds;
        private readonly SqlServices _sql;

        public IndexModel(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _ds = new DataServices(_context);
            _sql = new SqlServices(configuration);
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
        public bool isLive;
        

        [Authorize]
        public void OnGet(string? staffCode, string? adminStatus, string? pathway, int? weeks)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    StaffUser = _ds.GetStaffMemberDetails(User.Identity.Name);
                    if (staffCode is null && StaffUser.POSITION != "Admin Team Leader")
                    {
                        staffCode = StaffUser.STAFF_CODE;
                    }
                }

                AdminList = _ds.GetStaffMemberListByRole("Admin");
                GCList = _ds.GetStaffMemberListByRole("GC");
                ConsList = _ds.GetStaffMemberListByRole("Consultant");

                AreaNamesList = _ds.GetAreaNamesList();
                AdminStatusList = _ds.GetAdminStatusList();
                PathwayList = _ds.GetPathwayList();
                PatientReferralsList = _ds.GetPatientReferralsList();

                int days;

                if (weeks != null)
                {
                    days = weeks.GetValueOrDefault() * 7;
                }
                else
                {
                    days = 56;
                }

                DateTime FromDate = DateTime.Now.AddDays(-days);

                PatientReferralsList = PatientReferralsList.Where(r => r.RefDate >= FromDate);

                if (staffCode != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.Admin_Contact == staffCode);
                    StaffMemberName = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == staffCode).NAME;
                }

                if (adminStatus != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.Status_Admin == adminStatus);
                }

                if (pathway != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.PATHWAY == pathway);
                }
                isLive = bool.Parse(_configuration.GetValue("IsLive", ""));
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int? areaID, string? admin, string? gc, string? consultant, string? areaCode)
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
                
                _sql.UpdateAreaNames(areaID.GetValueOrDefault(), admin, gc, consultant, StaffUser.STAFF_CODE, areaCode);


                AdminList = _ds.GetStaffMemberListByRole("Admin");
                GCList = _ds.GetStaffMemberListByRole("GC");
                ConsList = _ds.GetStaffMemberListByRole("Consultant");

                AreaNamesList = _ds.GetAreaNamesList();
                AdminStatusList = _ds.GetAdminStatusList();
                PathwayList = _ds.GetPathwayList();
                PatientReferralsList = _ds.GetPatientReferralsList();

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