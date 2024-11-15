using WMFACS_Review_List.Data;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;

namespace WMFACS_Review_List.Metadata
{
    public class StaffData
    {
        private readonly ClinicalContext _context;
        
        public StaffData(ClinicalContext context) 
        {
            _context = context;
        }

        public StaffMember GetStaffMemberDetails(string username)
        {
            var staffUser = _context.StaffMembers.FirstOrDefault(s => s.EMPLOYEE_NUMBER == username);

            return staffUser;
        }

        public StaffMember GetStaffMemberDetailsByStaffCode(string staffCode)
        {
            var staffUser = _context.StaffMembers.FirstOrDefault(s => s.STAFF_CODE == staffCode);

            return staffUser;
        }

        public List<StaffMember> GetStaffMemberListByRole(string jobRole)
        {
            var staffList = _context.StaffMembers.Where(s => s.CLINIC_SCHEDULER_GROUPS == jobRole && s.InPost == true).OrderBy(s => s.NAME).ToList();
            
            return staffList;
        }


    }
}
