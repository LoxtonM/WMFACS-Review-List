using Microsoft.AspNetCore.Mvc.RazorPages;
using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;

namespace WMFACS_Review_List.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;
        

        public DetailsModel(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public PatientReferrals patientReferrals { get; set; }
        public IEnumerable<AdminStatuses> adminStatusList { get; set; }
        public IEnumerable<ActivityItems> activityItemsList { get; set; }
        public StaffMembers? StaffUser { get; set; }
        

        [Authorize]
        public void OnGet(int ID)
        {
            try
            {
                patientReferrals = _context.PatientReferrals.FirstOrDefault(r => r.refid == ID);
                adminStatusList = _context.AdminStatuses.ToList();
                activityItemsList = _context.ActivityItems.Where(a => a.RefID == ID).OrderBy(a => a.Date).ToList();
            }
            catch(Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        [Authorize]
        public void OnPost(int ID, string sComplete, string sAdminStatus)
        {
            try
            {
                patientReferrals = _context.PatientReferrals.FirstOrDefault(r => r.refid == ID);
                adminStatusList = _context.AdminStatuses.ToList();
                activityItemsList = _context.ActivityItems.Where(a => a.RefID == ID).OrderBy(a => a.Date).ToList();


                StaffUser = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == User.Identity.Name);
                string sStaffCode = StaffUser.STAFF_CODE;

                //SqlConnection conn = new SqlConnection("Server=spinners;DataBase=Clinical_Dev;User Id=shire_user;Password=shire1;TrustServerCertificate=True");
                SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConString"));
                conn.Open();
                SqlCommand cmd = new SqlCommand("Update MasterActivityTable set COMPLETE='" + sComplete + "', Status_Admin='" + sAdminStatus +
                "', updateddate = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', updatedby='" + sStaffCode + "' where RefID=" + ID, conn);
                cmd.ExecuteNonQuery();

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
                
    }
}