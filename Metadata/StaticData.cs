using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;
using ClinicalXPDataConnections.Models;
using ClinicalXPDataConnections.Data;

namespace WMFACS_Review_List.Metadata
{
    public class StaticData
    {
        private readonly ClinicalContext _context;
        private readonly DataContext _dataContext;

        public StaticData(ClinicalContext context, DataContext dataContext) 
        {
            _context = context;
            _dataContext = dataContext;
        }
               

        public List<AreaNames> GetAreaNamesList()
        {
            var areaNamesList = _dataContext.AreaNames.Where(a => a.InUse == true).OrderBy(a => a.AreaID).ToList();

            return areaNamesList;
        }
        
        public List<AdminStatuses> GetAdminStatusList()
        {
            var adminStatusList = _dataContext.AdminStatuses.ToList();

            return adminStatusList;
        }

        public List<Pathway> GetPathwayList()
        {
            var pathwayList = _context.Pathways.ToList();

            return pathwayList;
        }
        

    }
}
