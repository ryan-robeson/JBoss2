using JBOFarmersMkt.Context;
using JBOFarmersMkt.Models;
using JBOFarmersMkt.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace JBOFarmersMkt.ViewModels
{
    [CannotAllBeEmpty("products", "sales", ErrorMessage = "The products and sales files cannot both be empty.")]
    public class ImportViewModel
    {
        private HttpPostedFileBase _products;
        private HttpPostedFileBase _sales;

        public string productsHash { get; set; }

        [ValidName(@"stock_items.*\.csv$")]
        [UniqueFile("productsHash")]
        public HttpPostedFileBase products
        {
            get { return _products; }
            set
            {
                _products = value;
                // Compute and store the file's hash so it can be used for validation
                if (_products != null)
                {
                    productsHash = StreamHasher.ComputeHash(_products.InputStream);
                }
            }
        }

        public string salesHash { get; set; }

        [ValidName(@"sales_from_.+_to_.+\.csv$")]
        [UniqueFile("salesHash")]
        public HttpPostedFileBase sales
        {
            get
            {
                return _sales;
            }
            set
            {
                _sales = value;
                // Compute and store the file's hash so it can be used for validation
                if (_sales != null)
                {
                    salesHash = StreamHasher.ComputeHash(_sales.InputStream);
                }
            }
        }

        /// <summary>
        /// ValidName requires that an HttpPostedFileBase has a file name matching the given regex.
        /// </summary>
        private class ValidName : ValidationAttribute
        {
            private readonly string _r;

            /// <summary>
            /// ValidName requires that an HttpPostedFileBase has a file name matching the given regex.
            /// </summary>
            /// <param name="r">The regex to compare the filename to.</param>
            public ValidName(string r)
                : base("Invalid filename: {0}")
            {
                _r = r;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                HttpPostedFileBase file = value as HttpPostedFileBase;
                if (value != null)
                {
                    if (!Regex.IsMatch(file.FileName, _r))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }

        /// <summary>
        /// Ensures that the file is unique based on the hash stored in the given
        /// property name.
        /// </summary>
        private class UniqueFile : ValidationAttribute
        {
            private readonly string _computedHashProperty;

            /// <summary>
            /// Ensures that the file is unique based on the hash stored in the given
            /// property name.
            /// </summary>
            /// <param name="computedHashProperty">The name of the property that stores the file's hash.</param>
            public UniqueFile(string computedHashProperty)
                : base("This {0} file has already been uploaded.")
            {
                _computedHashProperty = computedHashProperty;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                string h = validationContext
                    .ObjectType
                    .GetProperty(_computedHashProperty)
                    .GetValue(validationContext.ObjectInstance) as string;

                if (h != null)
                {
                    if (!Import.IsUniqueContentHash(h))
                    {
                        var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                        return new ValidationResult(errorMessage);
                    }
                }
                return ValidationResult.Success;
            }
        }
    }

    /// <summary>
    /// CannotAllBeEmpty ensures that at least one of the given fields is not null.
    /// </summary>
    public class CannotAllBeEmpty : ValidationAttribute
    {
        private readonly string[] _fieldNames;

        /// <summary>
        /// CannotAllBeEmpty ensures that at least one of the given fields is not null.
        /// </summary>
        /// <param name="fieldNames">Names of properties in the model of which at least one must not be null.</param>
        public CannotAllBeEmpty(params string[] fieldNames)
            : base("Some fields are missing.")
        {
            _fieldNames = fieldNames;
        }

        public override bool IsValid(object value)
        {
            var valueType = value.GetType();

            int count = 0;

            foreach (var field in _fieldNames)
            {
                // This will throw an exception if field is not defined within the given object.
                // This should be fine since we want to catch that problem as early as possible.
                if (valueType.GetProperty(field).GetValue(value) == null)
                {
                    count = count + 1;
                }
            }

            return count < _fieldNames.Length;
        }
    }
}