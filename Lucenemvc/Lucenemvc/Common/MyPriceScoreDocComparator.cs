using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lucenemvc.Common
{
    public class MyPriceScoreDocComparator : ScoreDocComparator
    {
        private double[] sort;

        public MyPriceScoreDocComparator(String s, IndexReader reader, String fieldname)
        {
            sort = new double[reader.MaxDoc()];
            for (int i = 0; i < sort.Length; i++)
            {
                Document doc = reader.Document(i);
                sort[i] = Converter.ChangeType<double>(doc.Get("MinSalePrice"));
            }
        }

        public int Compare(ScoreDoc i, ScoreDoc j)
        {
            if (sort[i.doc] > sort[j.doc])
            {
                return 1;
            }
            if (sort[i.doc] < sort[j.doc])
            {
                return -1;
            }
            return 0;
        }

        public int SortType()
        {
            return SortField.DOUBLE;
        }
        public IComparable SortValue(ScoreDoc i)
        {
            return Converter.ChangeType<double>(sort[i.doc]);
        }
    }
}