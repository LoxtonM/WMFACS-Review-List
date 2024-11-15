using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;

namespace WMFACS_Review_List.Metadata
{
    public interface IReviewItemData
    {
        public List<ReviewItems> GetActivityItemsList(int id);
    }
    public class ReviewItemData : IReviewItemData
    {
        private readonly DataContext _context;
        
        public ReviewItemData(DataContext context) 
        {
            _context = context;
        }

        public List<ReviewItems> GetActivityItemsList(int id) 
        {
            var activityItemsList = _context.ReviewItems.Where(a => a.RefID == id).OrderBy(a => a.Date).ToList();

            return activityItemsList;
        }
    }
}
