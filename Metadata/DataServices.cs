using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;

namespace WMFACS_Review_List.Metadata
{
    public class DataServices
    {
        private readonly DataContext _context;
        
        public DataServices(DataContext context) 
        {
            _context = context;
        }

        public StaffMembers GetStaffMemberDetails(string username)
        {
            var staffUser = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == username);

            return staffUser;
        }

        public StaffMembers GetStaffMemberDetailsByStaffCode(string staffCode)
        {
            var staffUser = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == staffCode);

            return staffUser;
        }

        public List<StaffMembers> GetStaffMemberListByRole(string jobRole)
        {
            var staffList = _context.StaffMembers.Where(s => s.CLINIC_SCHEDULER_GROUPS == jobRole && s.InPost == true).OrderBy(s => s.NAME).ToList();

            return staffList;
        }

        public List<AreaNames> GetAreaNamesList()
        {
            var areaNamesList = _context.AreaNames.Where(a => a.InUse == true).OrderBy(a => a.AreaID).ToList();

            return areaNamesList;
        }
        
        public List<AdminStatuses> GetAdminStatusList()
        {
            var adminStatusList = _context.AdminStatuses.ToList();

            return adminStatusList;
        }

        public List<Pathways> GetPathwayList()
        {
            var pathwayList = _context.Pathways.ToList();

            return pathwayList;
        }
        
        public List<PatientReferrals> GetPatientReferralsList()
        {
            var patientReferralsList = _context.PatientReferrals.Where(r => r.RefType.Contains("Refer") 
                                                                        && r.COMPLETE != "Complete" 
                                                                        && r.logicaldelete == false 
                                                                        && r.Admin_Contact != null).OrderBy(r => r.WeeksFromReferral).ToList();

            return patientReferralsList;
        }

        public List<ActivityItems> GetActivityItemsList(int id) 
        {
            var activityItemsList = _context.ActivityItems.Where(a => a.RefID == id).OrderBy(a => a.Date).ToList();

            return activityItemsList;
        }

        public PatientReferrals GetReferralDetails(int refID)
        {
            var referral = _context.PatientReferrals.FirstOrDefault(r => r.refid == refID);

            return referral;
        }


    }
}
