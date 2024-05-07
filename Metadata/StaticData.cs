using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;

namespace WMFACS_Review_List.Metadata
{
    public class StaticData
    {
        private readonly DataContext _context;
        
        public StaticData(DataContext context) 
        {
            _context = context;
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
        

    }
}
