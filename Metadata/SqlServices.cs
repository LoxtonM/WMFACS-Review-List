using Microsoft.Data.SqlClient;
using System.Data;


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
            string dateEdited = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string adminOldValue = "";
            string gcOldValue = "";
            string consultantOldValue = "";

            string cmdText = $"select FHCStaffCode, GC, ConsCode from ListAreaNames where AreaID = {areaID}";
            _cmd.CommandText = cmdText;
            _con.Open();
            using (var reader = _cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    adminOldValue = reader.GetString("FHCStaffCode");
                    gcOldValue = reader.GetString("GC");
                    consultantOldValue = reader.GetString("ConsCode");
                }
            }
            _con.Close();

            List<int> areaIDs = new List<int>();

            if (areaCode != null)
            {
                cmdText = $"select AreaID from ListAreaNames where AreaCode = '{areaCode}'";
                _cmd.CommandText = cmdText;
                _con.Open();
                using (var reader = _cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        areaIDs.Add(reader.GetInt32("AreaID"));
                    }
                }
                _con.Close();
            }
            else
            {
                areaIDs.Add(areaID);
            }
                        
            foreach (var aid in areaIDs)
            {
                cmdText = $"Update ListAreaNames set FHCStaffCode='{admin}', GC='{gc}', ConsCode='{consultant}', WhoLastEdit='{staffCode}', " +
                    $"DateLastEdit='{dateEdited}' where AreaID={aid}";

                //}

                _cmd.CommandText = cmdText;
                _con.Open();
                _cmd.ExecuteNonQuery();
                _con.Close();

                CreateUsageAudit(staffCode, "WMFACS-X - Update Areas", $"AreaID={aid.ToString()}");

                if (adminOldValue != admin)
                {
                    WriteAuditUpdate(staffCode, "FHCStaffCode", aid, adminOldValue, admin, System.Environment.MachineName, "ListAreaNames");
                }
                if (gcOldValue != gc)
                {
                    WriteAuditUpdate(staffCode, "GC", aid, gcOldValue, gc, System.Environment.MachineName, "ListAreaNames");
                }
                if (consultantOldValue != consultant)
                {
                    WriteAuditUpdate(staffCode, "ConsCode", aid, consultantOldValue, consultant, System.Environment.MachineName, "ListAreaNames");
                }
            }
        }

        public void UpdateReferralStatus(string complete, string adminStatus, string staffCode, int id)
        {
            string cmdText;
            string completeOldValue = "";
            string adminStatusOldValue = "";
            
            cmdText = $"select complete, status_admin from masteractivitytable where refid = {id}";
            _cmd.CommandText = cmdText;
            _con.Open();
            using (var reader = _cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    completeOldValue = reader.GetString("COMPLETE");
                    adminStatusOldValue = reader.GetString("Status_Admin");
                }
            }
            _con.Close();


            cmdText = $"Update MasterActivityTable set COMPLETE='{complete}', Status_Admin='{adminStatus}', " +
                $"updateddate = '{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', updatedby='{staffCode}' where RefID={id}";
            
            _cmd.CommandText = cmdText;
            _con.Open();
            _cmd.ExecuteNonQuery();
            _con.Close();

            CreateUsageAudit(staffCode, "WMFACS-X - Update Referral Status", $"RefID={id.ToString()}");
            if (complete != completeOldValue)
            {
                WriteAuditUpdate(staffCode, "COMPLETE", id, completeOldValue, complete, System.Environment.MachineName, "MasterActivityTable");
            }
            if (adminStatus != adminStatusOldValue)
            {
                WriteAuditUpdate(staffCode, "Status_Admin", id, adminStatusOldValue, adminStatus, System.Environment.MachineName, "MasterActivityTable");
            }
        }
        public void CreateUsageAudit(string staffCode, string formName, string? searchTerm="")
        {
            _con.Open();
            SqlCommand cmd = new SqlCommand("dbo.sp_CreateAudit", _con);            
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@staffCode", SqlDbType.VarChar).Value = staffCode;
            cmd.Parameters.Add("@form", SqlDbType.VarChar).Value = formName;
            cmd.Parameters.Add("@database", SqlDbType.VarChar).Value = "CGU_DB";
            cmd.Parameters.Add("@machine", SqlDbType.VarChar).Value = System.Environment.MachineName;
            cmd.Parameters.Add("@searchTerm", SqlDbType.VarChar).Value = searchTerm;      
            cmd.ExecuteNonQuery();
            _con.Close();
        }

        public void WriteAuditUpdate(string staffCode, string fieldName, int recordPrimaryKey, string oldValue, string newValue, string machineName, string tableName)
        {
            _con.Open();
            SqlCommand cmd = new SqlCommand("dbo.sp_WriteAuditUpdate", _con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("@staffCode", SqlDbType.VarChar).Value = staffCode;
            cmd.Parameters.Add("@form", SqlDbType.VarChar).Value = fieldName;
            cmd.Parameters.Add("@recordkey", SqlDbType.VarChar).Value = recordPrimaryKey.ToString();
            cmd.Parameters.Add("@oldValue", SqlDbType.VarChar).Value = oldValue;
            cmd.Parameters.Add("@newValue", SqlDbType.VarChar).Value = newValue;
            cmd.Parameters.Add("@machineName", SqlDbType.VarChar).Value = machineName;
            cmd.Parameters.Add("@tableName", SqlDbType.VarChar).Value = tableName;
            cmd.ExecuteNonQuery();
            _con.Close();
        }

        
         
        

    }
}
