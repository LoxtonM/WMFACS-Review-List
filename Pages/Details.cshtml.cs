using Microsoft.AspNetCore.Mvc.RazorPages;
using WMFACS_Review_List.Data;
using WMFACS_Review_List.Metadata;
using WMFACS_Review_List.Models;
using Microsoft.AspNetCore.Authorization;

namespace WMFACS_Review_List.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly DataContext _context;
        private IConfiguration _configuration;
        private readonly ActivityData _activityData;
        private readonly StaffData _staffData;
        private readonly StaticData _staticData;
        private readonly ReferralData _referralData;
        private readonly SqlServices _sql;
        
        public DetailsModel(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _activityData = new ActivityData(_context);
            _staffData = new StaffData(_context);
            _staticData = new StaticData(_context);
            _referralData = new ReferralData(_context);
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
                patientReferrals = _referralData.GetReferralDetails(id);
                adminStatusList = _staticData.GetAdminStatusList();
                activityItemsList = _activityData.GetActivityItemsList(id);
                string staffCode = _staffData.GetStaffMemberDetails(User.Identity.Name).STAFF_CODE;
                _sql.CreateUsageAudit(staffCode, "WMFACS-X - Review Details", "RefID=" + id.ToString());
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
                patientReferrals = _referralData.GetReferralDetails(id);
                adminStatusList = _staticData.GetAdminStatusList();
                activityItemsList = _activityData.GetActivityItemsList(id);

                string staffCode = _staffData.GetStaffMemberDetails(User.Identity.Name).STAFF_CODE;

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