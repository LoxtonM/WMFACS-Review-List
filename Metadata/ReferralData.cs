using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;

namespace WMFACS_Review_List.Metadata
{
    public class ReferralData
    {
        private readonly DataContext _context;
        
        public ReferralData(DataContext context) 
        {
            _context = context;
        }

        
        public List<PatientReferrals> GetPatientReferralsList()
        {
            var patientReferralsList = _context.PatientReferrals.Where(r => r.RefType.Contains("Refer") 
                                                                        && r.COMPLETE != "Complete" 
                                                                        && r.logicaldelete == false 
                                                                        && r.Admin_Contact != null).OrderBy(r => r.WeeksFromReferral).ToList();

            return patientReferralsList;
        }
               
        public PatientReferrals GetReferralDetails(int refID)
        {
            var referral = _context.PatientReferrals.FirstOrDefault(r => r.refid == refID);

            return referral;
        }


    }
}
