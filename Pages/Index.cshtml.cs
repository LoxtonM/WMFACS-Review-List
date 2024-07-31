using Microsoft.AspNetCore.Mvc.RazorPages;
using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;
using Microsoft.AspNetCore.Authorization;
using WMFACS_Review_List.Metadata;

namespace WMFACS_Review_List.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        private readonly StaffData _staffData;
        private readonly StaticData _staticData;
        private readonly ReferralData _referralData;
        private readonly NotificationData _notificationData;
        private readonly SqlServices _sql;


        public IndexModel(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _staffData = new StaffData(_context);
            _staticData = new StaticData(_context);
            _referralData = new ReferralData(_context);
            _notificationData = new NotificationData(_context);
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
        public string notificationMessage { get; set; }
        public bool isLive;

        public string? staffCodeSelected;
        public string? adminStatusSelected;
        public string? pathwaySelected;
        public int? weeksSelected;
        public string? gcCodeSelected;


        [Authorize]
        public void OnGet(string? staffCode, string? adminStatus, string? pathway, int? weeks, string? gcFilter)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    StaffUser = _staffData.GetStaffMemberDetails(User.Identity.Name);
                    notificationMessage = _notificationData.GetMessage();
                    if (staffCode is null && gcFilter is null) // && StaffUser.POSITION != "Admin Team Leader")
                    {
                        staffCode = StaffUser.STAFF_CODE;
                    }
                    _sql.CreateUsageAudit(StaffUser.STAFF_CODE, "WMFACS-X - Home");
                }
                                
                
                AdminList = _staffData.GetStaffMemberListByRole("Admin");
                GCList = _staffData.GetStaffMemberListByRole("GC");
                ConsList = _staffData.GetStaffMemberListByRole("Consultant");

                AreaNamesList = _staticData.GetAreaNamesList();
                AdminStatusList = _staticData.GetAdminStatusList();
                PathwayList = _staticData.GetPathwayList();
                

                int days;

                if (weeks != null)
                {
                    days = weeks.GetValueOrDefault() * 7;
                    weeksSelected = weeks.GetValueOrDefault();
                }
                else
                {
                    days = 56;
                    weeks = 8;
                    weeksSelected = 8;
                }

                DateTime FromDate = DateTime.Now.AddDays(-days);
                //PatientReferralsList = _referralData.GetPatientReferralsList().Where(r => r.RefDate <= FromDate);
                PatientReferralsList = _referralData.GetPatientReferralsList().Where(r => r.WeeksFromReferral >= weeks);


                if (staffCode != null)
                {
                    staffCode = staffCode.ToUpper();
                    if (StaffUser.CLINIC_SCHEDULER_GROUPS == "Admin")
                    {
                        PatientReferralsList = PatientReferralsList.Where(r => r.AdminContactCode.ToUpper() == staffCode);
                    }
                    else
                    {
                        PatientReferralsList = PatientReferralsList.Where(r => r.GC_CODE.ToUpper() == staffCode);                        
                    }
                    StaffMemberName = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE.ToUpper() == staffCode).NAME;
                    staffCodeSelected = staffCode;
                }

                if (adminStatus != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.Status_Admin == adminStatus);
                    adminStatusSelected = adminStatus;
                }

                if (pathway != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.PATHWAY == pathway);
                    pathwaySelected = pathway;
                }

                if(gcFilter != null)
                {
                    PatientReferralsList = PatientReferralsList.Where(r => r.GC_CODE == gcFilter);
                    gcCodeSelected = gcFilter;
                    if(staffCode == null)
                    {
                        staffCodeSelected = null;
                    }
                }

                isLive = bool.Parse(_configuration.GetValue("IsLive", ""));
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }
        }

        public void OnPost(int? areaID, string? admin, string? gc, string? consultant, string? areaCode, int? weeks, string? gcFilter)
        {
            try
            {
                if (User.Identity.Name is null)
                {
                    Response.Redirect("Login");
                }
                else
                {
                    StaffUser = _staffData.GetStaffMemberDetails(User.Identity.Name);
                }
                
                _sql.UpdateAreaNames(areaID.GetValueOrDefault(), admin, gc, consultant, StaffUser.STAFF_CODE, areaCode);


                AdminList = _staffData.GetStaffMemberListByRole("Admin");
                GCList = _staffData.GetStaffMemberListByRole("GC");
                ConsList = _staffData.GetStaffMemberListByRole("Consultant");

                AreaNamesList = _staticData.GetAreaNamesList();
                AdminStatusList = _staticData.GetAdminStatusList();
                PathwayList = _staticData.GetPathwayList();
                //PatientReferralsList = _referralData.GetPatientReferralsList();
                PatientReferralsList = _referralData.GetPatientReferralsList().Where(r => r.WeeksFromReferral >= weeks);

                //DateTime FromDate = DateTime.Now.AddDays(-56);

                //PatientReferralsList = PatientReferralsList.Where(r => r.RefDate >= FromDate);
            }
            catch (Exception ex)
            {
                Response.Redirect("Error?sError=" + ex.Message);
            }

        }        
        
    }
}