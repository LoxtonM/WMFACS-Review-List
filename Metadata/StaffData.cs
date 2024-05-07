using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;

namespace WMFACS_Review_List.Metadata
{
    public class StaffData
    {
        private readonly DataContext _context;
        
        public StaffData(DataContext context) 
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


    }
}
