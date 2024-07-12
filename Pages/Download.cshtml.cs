using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PatientTrackingList.Pages
{
    public class DownloadModel : PageModel
    {
        

        private readonly IWebHostEnvironment environment;
        public DownloadModel(IWebHostEnvironment hostEnvironment)
        {
            environment = hostEnvironment;
        }

        public void OnGet()
        {
            Download();
        }


        [HttpGet("download")]
        public IActionResult Download()
        {
            var filepath = Path.Combine(environment.WebRootPath, "images", "Image1.png");
            return File(System.IO.File.ReadAllBytes(filepath), "image/png", System.IO.Path.GetFileName(filepath));
        }
    }
}
