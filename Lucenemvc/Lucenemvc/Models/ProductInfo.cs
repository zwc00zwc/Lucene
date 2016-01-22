using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lucenemvc.Models
{
    public class ProductInfo
    {
        public long Id { get; set; }
        public long ShopId { get; set; }
        public long CategoryId { get; set; }
        public string CategoryPath { get; set; }
        public long TypeId { get; set; }
        public long BrandId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ShortDescription { get; set; }
        public int SaleStatus { get; set; }
        public System.DateTime AddedDate { get; set; }
        public long DisplaySequence { get; set; }
        private string imagePath { get; set; }
        public decimal MarketPrice { get; set; }
        public decimal MinSalePrice { get; set; }
        public bool HasSKU { get; set; }
        public long VistiCounts { get; set; }
        public long SaleCounts { get; set; }
        public int AuditStatus { get; set; }
        public long FreightTemplateId { get; set; }
        public Nullable<decimal> Weight { get; set; }
        public Nullable<decimal> Volume { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string MeasureUnit { get; set; }
        public Nullable<int> StartingMass { get; set; }
        public Nullable<int> PriceStrategy { get; set; }
        public string FreightDescription { get; set; }
    }
}