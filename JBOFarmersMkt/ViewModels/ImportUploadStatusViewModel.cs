using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JBOFarmersMkt.ViewModels
{
    public class ImportUploadStatusViewModel
    {
        public bool success { get; set; }
        // Errors that occur during import
        public List<string> dbErrors { get; set; }
        public string name { get; set; }
        // Message on successful import.
        public string message { get; set; }

        public ImportUploadStatusViewModel()
        {
            success = false;
            dbErrors = new List<string>();
        }
    }
}