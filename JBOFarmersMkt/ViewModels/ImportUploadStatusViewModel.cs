using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JBOFarmersMkt.ViewModels
{
    /// <summary>
    /// Models a specific upload for error handling.
    /// </summary>
    /// <example> 
    /// Failing products import
    /// <code>
    ///     ImportUploadStatusViewModel p = new ImportUploadStatusViewModel { name = "products" };
    ///     p.success = false;
    ///     p.dbErrors.Add("The database did not approve because...");
    /// </code>
    /// </example>
    /// <example>
    /// Successful sales import
    /// <code>
    ///     ImportUploadStatusViewModel s = new ImportUploadStatusViewModel { name = "sales" };
    ///     s.success = true;
    ///     s.message = "Successfully imported sales. 120,000 records updated. 12,000 records created.";
    /// </code>
    /// </example>
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