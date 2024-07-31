using Microsoft.AspNetCore.Mvc;
using WMFACS_Review_List.Data;
using WMFACS_Review_List.Models;
using System.Data;
using WMFACS_Review_List.Metadata;


namespace PatientTrackingList.DataServices
{
    public class Exporter : Controller
    {
        public string dlFilePath;
        private readonly DataContext _context;
        private readonly ReferralData _referralData;
        
        public Exporter(DataContext context)
        {
            _context = context;
            _referralData = new ReferralData(_context);
        }
        public void ExportList(List<PatientReferrals> listToExport, string adminUsername)
        {
            DataTable table = new DataTable();
            
            table.Columns.Add("CGU Number", typeof(string));
            table.Columns.Add("Weeks From Ref", typeof(string));
            table.Columns.Add("Admin Status", typeof(string));
            table.Columns.Add("Clock Start", typeof(string));
            table.Columns.Add("Clock Stop", typeof(string));
            table.Columns.Add("Breach Date", typeof(string));
            table.Columns.Add("Pathway", typeof(string));
            table.Columns.Add("Clics?", typeof(string));
            table.Columns.Add("Admin Contact", typeof(string));
            table.Columns.Add("GC", typeof(string));
                       
            
            foreach (var item in listToExport) 
            {
                string statusAdmin = ""; //because there are ALWAYS nulls!!!
                string clockStartDate = "";
                string clockStopDate = "";
                string breachDate = "";
                string pathway = "";
                string clics = "N/A";
                string gc = "";

                if (item.Status_Admin != null) 
                {
                    statusAdmin = item.Status_Admin;
                }
                if (item.ClockStartDate != null)
                {
                    clockStartDate = item.ClockStartDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.ClockStopDate!= null)
                {
                    clockStopDate = item.ClockStopDate.Value.ToString("dd/MM/yyyy");
                }                
                if (item.BreachDate != null)
                {
                    breachDate = item.BreachDate.Value.ToString("dd/MM/yyyy");
                }
                if (item.PATHWAY != null)
                {
                    pathway = item.PATHWAY;
                }
                if (item.Clics != null)
                {
                    clics = item.Clics;
                }
                if (item.GC != null)
                {
                    gc = item.GC;
                }


                table.Rows.Add(item.CGU_No, 
                    item.WeeksFromReferral, 
                    statusAdmin,
                    clockStartDate,
                    clockStopDate, 
                    breachDate, 
                    pathway,
                    clics, 
                    item.AdminContact,                     
                    gc);
            }
            
            //return table;
            ToCSV(table, adminUsername);
        }

                

        public void ToCSV(DataTable table, string staffCode)
        {
            //string filePath = $"C:\\CGU_DB\\ptl-{username}.csv";
            string fileName = $"ReviewList-{staffCode}.csv";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Downloads\\" + fileName);
            StreamWriter sw = new StreamWriter(filePath, false);
            //headers
            for (int i = 0; i < table.Columns.Count; i++)
            {
                sw.Write(table.Columns[i]);
                if (i < table.Columns.Count - 1)
                {
                    sw.Write(",");
                }
            }
            sw.Write(sw.NewLine);
            foreach (DataRow dr in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(','))
                        {
                            value = String.Format("\"{0}\"", value);
                            sw.Write(value);
                        }
                        else
                        {
                            sw.Write(dr[i].ToString());
                        }
                    }
                    if (i < table.Columns.Count - 1)
                    {
                        sw.Write(",");
                    }
                }
                sw.Write(sw.NewLine);
            }            
            sw.Close();

            dlFilePath = filePath;

            //DownloadFile(filePath);
        }

        [HttpGet("download")]
        //public async Task<IActionResult> DownloadFile(string filePath)
        public async Task<IActionResult> DownloadFile(string staffCode, int weeksFromReferral, string? adminStatus, string? pathway, string? gcCode)
        {
            
            List<PatientReferrals> listToExport = new List<PatientReferrals>();

            if (gcCode != null)
            {
                staffCode = gcCode.ToUpper();
                listToExport = _referralData.GetPatientReferralsList().Where(r => r.GC_CODE.ToUpper() == staffCode & r.WeeksFromReferral >= weeksFromReferral).ToList();
            }
            else
            {
                listToExport = _referralData.GetPatientReferralsList().Where(r => r.AdminContactCode.ToUpper() == staffCode & r.WeeksFromReferral >= weeksFromReferral).ToList();
            }

            int days = weeksFromReferral * 7;
            DateTime FromDate = DateTime.Now.AddDays(-days);
            listToExport = listToExport.Where(r => r.RefDate <= FromDate).ToList();

            if (adminStatus != null)
            {
                listToExport = listToExport.Where(r => r.Status_Admin == adminStatus).ToList();
            }

            if (pathway != null)
            {
                listToExport = listToExport.Where(r => r.PATHWAY == pathway).ToList();
            }



            ExportList(listToExport, staffCode);

            if (System.IO.File.Exists(dlFilePath))
            {
                return File(System.IO.File.ReadAllBytes(dlFilePath), "text/csv", System.IO.Path.GetFileName(dlFilePath));
            }
            return Redirect("Error");
        }


    }
}
