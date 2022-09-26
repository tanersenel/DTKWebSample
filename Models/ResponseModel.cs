using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DTKWebSample.Models
{
    public class ResponseModel
    {
        public bool Status { get; set; }
        public string Error { get; set; }
        public object Response { get; set; }
        public string Plate { get; set; }
        public string PlatePhoto { get; set; }
        public string UploadedPhoto { get; set; }
        public DateTime Date { get; set; }

    }
}

