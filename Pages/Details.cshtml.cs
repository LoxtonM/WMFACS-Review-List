using Microsoft.AspNetCore.Mvc.RazorPages;
using WMFACS_Review_List.Data;
using WMFACS_Review_List.Metadata;
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
        private readonly DataServices _data;
        private readonly SqlServices _sql;
        
        public DetailsModel(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _data = new DataServices(_context);
            _sql = new SqlServices(_configuration);
        }
        public PatientReferrals patientReferrals { get; set; }
        public IEnumerable<AdminStatuses> adminStatusList { get; set; }
        public IEnumerable<ActivityItems> activityItemsList { get; set; }
        public StaffMembers? StaffUser { get; set; }
        

        [Authorize]
        public void OnGet(int id)
        {
            try
            {                
                patientReferrals = _data.GetReferralDetails(id);
                adminStatusList = _data.GetAdminStatusList();
                activityItemsList = _data.GetActivityItemsList(id);

            }
            catch(Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        [Authorize]
        public void OnPost(int id, string complete, string adminStatus)
        {
            try
            {
                patientReferrals = _data.GetReferralDetails(id);
                adminStatusList = _data.GetAdminStatusList();
                activityItemsList = _data.GetActivityItemsList(id);

                string staffCode = _data.GetStaffMemberDetails(User.Identity.Name).STAFF_CODE;

                _sql.UpdateReferralStatus(complete, adminStatus, staffCode, id);

                Response.Redirect("Index");
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }
                
    }
}