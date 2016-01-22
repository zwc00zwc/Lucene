using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lucenemvc.Models;
using Lucene.Net;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Search;
using System.IO;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search.Group;
using Lucenemvc.Common;

namespace Lucenemvc.Controllers
{
    public class LbwmController : Controller
    {
        //
        // GET: /Lbwm/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Search()
        {
            List<ProductInfo> list = new List<ProductInfo>();
            string keyword = "";
            if (Url.RequestContext.RouteData.Values.ContainsKey("key"))
            {
                if (!string.IsNullOrEmpty(Url.RequestContext.RouteData.Values["key"].ToString()))
                {
                    keyword = Url.RequestContext.RouteData.Values["key"].ToString();
                }
            }

            int curr = 0;

            if (Url.RequestContext.RouteData.Values.ContainsKey("page"))
            {
                if (!string.IsNullOrEmpty(keyword))
                {
                    if (!string.IsNullOrEmpty(Url.RequestContext.RouteData.Values["page"].ToString()))
                    {
                        curr = int.Parse(Url.RequestContext.RouteData.Values["page"].ToString());
                    }
                    list = searchdata(keyword, curr, 15);
                }
            }
            ViewData["pro"] = list;
            ViewData["currpage"] = curr;
            ViewData["key"] = keyword;
            return View();
        }

        private List<ProductInfo> searchdata(string Searchkey, int currpage, int pagesize)
        {
            List<ProductInfo> prolist = new List<ProductInfo>();
            string indexpath = "E:\\WmIndexdata";
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexpath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);

            BooleanQuery bqAllFilter = new BooleanQuery();
            string keyword = Common.SplitContent.GetKeyWordsSplitBySpace(Searchkey, new PanGuTokenizer());
            Query querytitle = new QueryParser("ProductName", new PanGuAnalyzer(true)).Parse(keyword);
            bqAllFilter.Add(querytitle, BooleanClause.Occur.MUST);
            //Query aquery = new TermQuery(new Term("Id", "322212"));
            //bqAllFilter.Add(aquery, BooleanClause.Occur.MUST);
            //NumericRangeQuery pricrQuery = NumericRangeQuery.NewDoubleRange("MinSalePrice", 500, 500, true, true);
            //bqAllFilter.Add(pricrQuery, BooleanClause.Occur.MUST);

            //SortField sf = new SortField("MinSalePrice", new MyPriceSortComparatorSource(), false);
            //Sort sort = new Sort(new SortField[] { sf });
            Hits hits = searcher.Search(bqAllFilter);
            //Hits hits = searcher.Search(bqAllFilter, sort);
            if (currpage >= 0 && pagesize > 0 && (currpage-1) * pagesize < hits.Length())
            {
                int i = (currpage - 1) * pagesize;
                while (i < currpage * pagesize && i < hits.Length())
                {
                    ProductInfo info = new ProductInfo();
                    info.Id = int.Parse(hits.Doc(i).Get("Id"));

                    info.ProductName = Common.SplitContent.HightLight(keyword, hits.Doc(i).Get("ProductName"));
                    info.SaleCounts = long.Parse(hits.Doc(i).Get("SaleCounts").ToString());
                    info.MinSalePrice = decimal.Parse(hits.Doc(i).Get("MinSalePrice").ToString());
                    prolist.Add(info);
                    i++;
                }
            }
            return prolist;
        }

        public string ConvertSearchNumver(string num)
        {
            return (double.Parse(num) + 100000000000).ToString();
        }
	}
}