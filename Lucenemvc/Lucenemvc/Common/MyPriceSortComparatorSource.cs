using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lucene.Net;
using Lucene.Net.Search;
using Lucene.Net.Index;

namespace Lucenemvc.Common
{
    public class MyPriceSortComparatorSource : SortComparatorSource
    {
        public ScoreDocComparator NewComparator(IndexReader reader, string fieldname)
        {
            if (fieldname.Equals("MinSalePrice"))
            {
                return new MyPriceScoreDocComparator("MinSalePrice", reader, fieldname);
            }

            return null;
        }
    }
}