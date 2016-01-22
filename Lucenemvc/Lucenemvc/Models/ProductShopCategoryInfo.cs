using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lucenemvc.Models
{
    public class ProductShopCategoryInfo
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public long ShopCategoryId { get; set; }
    }
}