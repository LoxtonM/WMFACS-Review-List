using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System;

namespace WMFACS_Review_List.Metadata
{
    public class SqlServices
    {
        private readonly IConfiguration _config;
        private readonly SqlConnection _con;
        private readonly SqlCommand _cmd;

        public SqlServices(IConfiguration config)
        {
            _config = config;
            _con = new SqlConnection(_config.GetConnectionString("ConString"));
            _cmd = new SqlCommand("", _con);
        }

        public void UpdateAreaNames(int areaID, string admin, string gc, string consultant, string staffCode, string? areaCode)
        {
            string cmdText="";
            string dateEdited = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            if (areaCode != null)
            {               
                cmdText = $"Update ListAreaNames set FHCStaffCode='{admin}', GC='{gc}', ConsCode='{consultant}', WhoLastEdit='{staffCode}', " +
                    $"DateLastEdit='{dateEdited}' where AreaCode='{areaCode}'";
            }
            else
            {                
                cmdText = $"Update ListAreaNames set FHCStaffCode='{admin}', GC='{gc}', ConsCode='{consultant}', WhoLastEdit='{staffCode}', " +
                    $"DateLastEdit='{dateEdited}' where AreaID={areaID}";
            }

            _cmd.CommandText = cmdText;
            _con.Open();
            _cmd.ExecuteNonQuery();
            _con.Close();            
        }



        
         
        

    }
}
