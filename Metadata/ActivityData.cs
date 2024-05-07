using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;

namespace WMFACS_Review_List.Metadata
{
    public class ActivityData
    {
        private readonly DataContext _context;
        
        public ActivityData(DataContext context) 
        {
            _context = context;
        }

        public List<ActivityItems> GetActivityItemsList(int id) 
        {
            var activityItemsList = _context.ActivityItems.Where(a => a.RefID == id).OrderBy(a => a.Date).ToList();

            return activityItemsList;
        }
    }
}
