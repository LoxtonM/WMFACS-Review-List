using Microsoft.AspNetCore.Mvc.RazorPages;
using WMFACS_Review_List.Data;
using WMFACS_Review_List.Metadata;
using WMFACS_Review_List.Models;
using Microsoft.AspNetCore.Authorization;
using ClinicalXPDataConnections.Meta;
using ClinicalXPDataConnections.Data;
using ClinicalXPDataConnections.Models;

namespace WMFACS_Review_List.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly ClinicalContext _context;
        private readonly DataContext _dataContext;
        private IConfiguration _configuration;
        private readonly ReviewItemData _reviewItemData;
        private readonly StaffUserData _staffData;
        private readonly StaticData _staticData;
        private readonly ReferralData _referralData;
        private readonly SqlServices _sql;
        
        public DetailsModel(ClinicalContext context, DataContext dataContext, IConfiguration configuration)
        {
            _context = context;
            _dataContext = dataContext;
            _configuration = configuration;
            _reviewItemData = new ReviewItemData(_dataContext);
            _staffData = new StaffUserData(_context);
            _staticData = new StaticData(_context, _dataContext);
            _referralData = new ReferralData(_context);
            _sql = new SqlServices(_configuration);
        }
        public Referral patientReferrals { get; set; }
        public IEnumerable<AdminStatuses> adminStatusList { get; set; }
        public IEnumerable<ReviewItems> activityItemsList { get; set; }
        public StaffMember? StaffUser { get; set; }
        

        [Authorize]
        public void OnGet(int id)
        {
            try
            {                
                patientReferrals = _referralData.GetReferralDetails(id);
                adminStatusList = _staticData.GetAdminStatusList();
                activityItemsList = _reviewItemData.GetActivityItemsList(id);
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
                activityItemsList = _reviewItemData.GetActivityItemsList(id);

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