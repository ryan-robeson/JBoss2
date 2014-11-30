using CsvHelper;
using JBOFarmersMkt.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity.Core;
using System.IO;
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

        /// <summary>
        /// Imports the given category from the given CSV stream.
        /// </summary>
        /// <param name="type">The type of import.</param>
        /// <param name="csv">The csv stream.</param>
        public static void FromCSV(ImportCategories type, Stream csv)
        {
            bool error = false;

            switch (type)
            {
                case ImportCategories.Sales:
                    SalesFromCSV(csv);
                    break;
                case ImportCategories.Products:
                    ProductsFromCSV(csv);
                    break;
                default:
                    error = true;
                    break;
            }

            if (error)
            {
                throw new Exception("Invalid import type.");
            }
        }

        private static void ProductsFromCSV(Stream csv)
        {
            List<Product> display = new List<Product>();

            using (var context = new JBOContext())
            {
                if (csv != null && csv.Length > 0)
                {
                    //var csvReader = new CsvReader(new StreamReader(csv));
                    //csvReader.Configuration.RegisterClassMap<ProductClassMap>();

                    //var productList = csvReader.GetRecords<ProductView>().ToList();


                    //foreach (var s in productList)
                    //{
                    //    Product productDisplay = new Product();

                    //    productDisplay.productCode = int.Parse(s.productCode);
                    //    productDisplay.description = s.description;
                    //    productDisplay.department = s.department;
                    //    productDisplay.category = s.category;
                    //    productDisplay.upc = s.upc;
                    //    productDisplay.storeCode = s.storeCode;
                    //    productDisplay.unitPrice = decimal.Parse(s.unitPrice);
                    //    productDisplay.discountable = Boolean.Parse(s.discountable);
                    //    productDisplay.taxable = Boolean.Parse(s.taxable);
                    //    productDisplay.inventoryMethod = s.inventoryMethod;
                    //    productDisplay.quantity = double.Parse(s.quantity);
                    //    productDisplay.orderTrigger = int.Parse(s.orderTrigger);
                    //    productDisplay.recommendedOrder = int.Parse(s.recommendedOrder);
                    //    productDisplay.lastSoldDate = s.lastSoldDate;
                    //    productDisplay.supplier = s.supplier;
                    //    productDisplay.liabilityItem = s.liabilityItem;
                    //    productDisplay.LRT = s.LRT;

                    //    display.Add(productDisplay);

                    //}

                    //List<Product> updated = new List<Product>();
                    //List<Product> newItems = new List<Product>();


                    //var products = from p in context.Products select p.productCode;
                    //var update = from p in display where products.Contains(p.productCode) select p;
                    //var newItem = from p in display where !products.Contains(p.productCode) select p;
                    //foreach (var i in update)
                    //{
                    //    updated.Add(i);
                    //}

                    //foreach (var i in newItem)
                    //{
                    //    context.Products.Add(i);
                    //    context.SaveChanges();
                    //}



                    //foreach (Product i in updated)
                    //{
                    //    Product prod = context.Products.Single(p => p.productCode == i.productCode);
                    //    prod.productCode = i.productCode;
                    //    prod.description = i.description;
                    //    prod.department = i.department;
                    //    prod.category = i.category;
                    //    prod.upc = i.upc;
                    //    prod.storeCode = i.storeCode;
                    //    prod.unitPrice = i.unitPrice;
                    //    prod.discountable = i.discountable;
                    //    prod.taxable = i.taxable;
                    //    prod.inventoryMethod = i.inventoryMethod;
                    //    prod.quantity = i.quantity;
                    //    prod.orderTrigger = i.orderTrigger;
                    //    prod.recommendedOrder = i.recommendedOrder;
                    //    prod.lastSoldDate = i.lastSoldDate;
                    //    prod.supplier = i.supplier;
                    //    prod.liabilityItem = i.liabilityItem;
                    //    prod.LRT = i.LRT;
                    //    try
                    //    {
                    //        context.SaveChanges();
                    //    }
                    //    catch (EntityException ex)
                    //    {

                    //    }
                    //}


                }
            }
            
        }

        private static void SalesFromCSV(Stream csv)
        {

        }
    }
}