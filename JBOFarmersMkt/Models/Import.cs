using JBOFarmersMkt.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace JBOFarmersMkt.Models
{
    public enum ImportCategories
    {
        Sales,
        Products
    }

    public class Import : IAuditedEntity
    {
        public int Id { get; set; }
        public string filename { get; set; }
        public ImportCategories type { get; set; }
        public string contentHash { get; set; }

        [DisplayName("Imported By")]
        public string CreatedBy
        {
            get;
            set;
        }

        [DisplayName("Imported At")]
        public DateTime CreatedAt
        {
            get;
            set;
        }

        public string LastModifiedBy
        {
            get;
            set;
        }

        public DateTime LastModifiedAt
        {
            get;
            set;
        }

        public static Import FindByContentHash(string h)
        {
            using (JBOContext context = new JBOContext())
            {
                return context.Imports.Where(i => i.contentHash == h).DefaultIfEmpty(null).FirstOrDefault(i => i.contentHash == h);
            }
        }

        public static bool IsUniqueContentHash(string h)
        {
            if (FindByContentHash(h) == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}