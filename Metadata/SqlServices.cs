using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Reflection.Metadata;
using System;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using WMFACS_Review_List.Models;
using System.Xml;

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

            CreateAudit(staffCode, "WMFACSUpdateAreas", areaID);
        }

        public void UpdateReferralStatus(string complete, string adminStatus, string staffCode, int id)
        {
            string cmdText = $"Update MasterActivityTable set COMPLETE='{complete}', Status_Admin='{adminStatus}', " +
                $"updateddate = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', updatedby='{staffCode}' where RefID={id}";
            
            _cmd.CommandText = cmdText;
            _con.Open();
            _cmd.ExecuteNonQuery();
            _con.Close();

            CreateAudit(staffCode, "WMFACSUpdateReferralStatus", id);
        }
        public void CreateAudit(string staffCode, string formName, int recordPrimaryKey)
        {
            _con.Open();
            SqlCommand cmd = new SqlCommand("dbo.sp_CreateAudit", _con);            
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@staffCode", SqlDbType.VarChar).Value = staffCode;
            cmd.Parameters.Add("@form", SqlDbType.VarChar).Value = formName;
            cmd.Parameters.Add("@recordkey", SqlDbType.Int).Value = recordPrimaryKey;            
            cmd.ExecuteNonQuery();
            _con.Close();
        }



        
         
        

    }
}
