using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lucenemvc.Common;
using Lucenemvc.Models;
using Lucene.Net.Store;
using System.IO;
using Lucene.Net;
using Lucene.Net.Index;
using Lucene.Net.Analysis.PanGu;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.QueryParsers;
using System.Threading;

namespace Lucenemvc.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string btncreate)
        {
            MultiSite_GangGuanEntities Bll = new Models.MultiSite_GangGuanEntities();
            
            int pageSize = 200;  //页大小
            int totalNum=Bll.Products_SellOffer.Count();  //总条数
            int pageNum = totalNum % pageSize == 0 ? totalNum / pageSize : totalNum / pageSize + 1;   //页数
            int i = 0;
            try
            {
                for (i = 0; i < pageNum; i++)
                {
                    List<Products_SellOffer> list = new List<Models.Products_SellOffer>();
                    list = Bll.Products_SellOffer.OrderBy(o => o.Id > 0).Skip(i * pageSize).Take(pageSize).ToList();
                    //list = Bll.Products_SellOffer.OrderBy(o => o.Id > 0).Take(pageSize).ToList();
                    Thread.Sleep(1500);
                    CreateIndex(list);
                    //totalNum = totalNum - list.Count;
                }
            }
            catch (Exception)
            {
                Response.Write("第+" + i + "+页出问题出错");
            }
            //if (totalNum == 0)
            //{
            //    Response.Write("创建完成");
            //}
            Response.Write("创建完成");
            return View();
        }

        public ActionResult Index1()
        {
            string Title = "供应子母门　非标门　钢质门　进户门　金属门";
            CreateTestIndex(Title);
            return View();
        }

        public void CreateTestIndex(string name)
        {
            MultiSite_GangGuanEntities Bll = new Models.MultiSite_GangGuanEntities();
            string indexpath = HttpContext.Server.MapPath("/Indexdata");
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexpath), new NoLockFactory());
            bool isExist = IndexReader.IndexExists(directory); //是否存在索引库文件夹以及索引库特征文件
            if (isExist)
            {
                //如果索引目录被锁定（比如索引过程中程序异常退出或另一进程在操作索引库），则解锁
                //Q:存在问题 如果一个用户正在对索引库写操作 此时是上锁的 而另一个用户过来操作时 将锁解开了 于是产生冲突 --解决方法后续
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }

            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);

                Document document = new Document();

                Field title = new Field("title", name, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                document.Add(title);
                writer.AddDocument(document); //文档写入索引库
            
            writer.Optimize();
            writer.Close();//会自动解锁
            directory.Close(); //不要忘了Close，否则索引结果搜不到
        }

        public void CreateIndex(List<Products_SellOffer> prolist)
        {
            MultiSite_GangGuanEntities Bll = new Models.MultiSite_GangGuanEntities();
            string indexpath = HttpContext.Server.MapPath("/Indexdata");
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexpath), new NoLockFactory());
            bool isExist = IndexReader.IndexExists(directory); //是否存在索引库文件夹以及索引库特征文件
            if (isExist)
            {
                //如果索引目录被锁定（比如索引过程中程序异常退出或另一进程在操作索引库），则解锁
                //Q:存在问题 如果一个用户正在对索引库写操作 此时是上锁的 而另一个用户过来操作时 将锁解开了 于是产生冲突 --解决方法后续
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }

            IndexWriter writer = new IndexWriter(directory, new PanGuAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);
            
            foreach (var pitem in prolist)
            {
                Document document = new Document();
                Field id = new Field("id", pitem.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED);

                Field title = new Field("title", pitem.Title, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                title.SetBoost(1.0f);

                Field keywords = new Field("keywords", pitem.Keywords, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                keywords.SetBoost(0.7f);

                Field detail = new Field("detail", Bll.Products_SellOffer_Detail.Where(d => d.SellOfferId == pitem.Id).FirstOrDefault().Detail, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);

                Field sysattr = new Field("sysattr", pitem.SysAttr, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                sysattr.SetBoost(0.4f);

                Field cusattr = new Field("cusattr", pitem.CusAttr, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS);
                cusattr.SetBoost(0.1f);

                document.Add(id);
                document.Add(title);
                document.Add(keywords);
                document.Add(detail);
                document.Add(sysattr);
                document.Add(cusattr);
                writer.AddDocument(document); //文档写入索引库
            }
            writer.Optimize();
            writer.Close();//会自动解锁
            directory.Close(); //不要忘了Close，否则索引结果搜不到
        }

        public ActionResult Search()
        {
            List<ShowInfo> showlist = new List<ShowInfo>();
            //string keyword = Request.QueryString["key"];
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
                    showlist = searchdata(keyword, curr, 15);
                }
            }
            
            ViewData["pro"] = showlist;
            ViewData["currpage"] = curr;
            ViewData["key"] = keyword;
            return View();
        }

        [HttpPost]
        public ActionResult Search(string Searchkey)
        {
            List<ShowInfo> prolist = new List<ShowInfo>();
            if (!string.IsNullOrEmpty(Searchkey))
            {
                prolist = searchdata(Searchkey, 0, 15);
            }
            ViewData["pro"] = prolist;
            ViewData["currpage"] = 1;
            ViewData["key"] = Searchkey;
            return View();
        }
        
        public ActionResult Search1()
        {
            List<ShowInfo> prolist = new List<ShowInfo>();
            prolist = searchTestdata("金属门");
            ViewData["pro"] = prolist;
            ViewData["currpage"] = 1;
            ViewData["key"] = "金属门";
            return View();
        }

        private List<ShowInfo> searchTestdata(string Searchkey)
        {
            List<ShowInfo> prolist = new List<ShowInfo>();
            //string indexpath = HttpContext.Server.MapPath("/Indexdata");
            string indexpath = HttpContext.Server.MapPath("/Indexdata");
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexpath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);

            BooleanQuery bqAllFilter = new BooleanQuery();
            string keyword = Common.SplitContent.GetKeyWordsSplitBySpace(Searchkey, new PanGuTokenizer());
            //TermQuery tquery = new TermQuery(new Term("title", keyword));
            Query querytitle = new QueryParser("title", new PanGuAnalyzer(true)).Parse(keyword);
            bqAllFilter.Add(querytitle, BooleanClause.Occur.MUST);
            //Hits hits = searcher.Search(bqAllFilter);
            Hits hits = searcher.Search(bqAllFilter);
            ShowInfo info = new ShowInfo();
            info.Title = Common.SplitContent.HightLight(keyword, hits.Doc(0).Get("title"));
            prolist.Add(info);
            
            return prolist;
        }
        private List<ShowInfo> searchdata(string Searchkey, int currpage, int pagesize)
        {
            List<ShowInfo> prolist = new List<ShowInfo>();
            //string indexpath = HttpContext.Server.MapPath("/Indexdata");
            string indexpath = "E:\\Indexdata1";
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexpath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);

            BooleanQuery bqAllFilter = new BooleanQuery();
            string keyword = Common.SplitContent.GetKeyWordsSplitBySpace(Searchkey, new PanGuTokenizer());
            //TermQuery tquery = new TermQuery(new Term("title", keyword));
            Query querytitle = new QueryParser("title", new PanGuAnalyzer(true)).Parse(keyword);
            Query querycontent = new QueryParser("keywords", new PanGuAnalyzer(true)).Parse(keyword);
            Query querysysattr = new QueryParser("sysattr", new PanGuAnalyzer(true)).Parse(keyword);
            bqAllFilter.Add(querytitle, BooleanClause.Occur.MUST);
            bqAllFilter.Add(querycontent, BooleanClause.Occur.SHOULD);
            bqAllFilter.Add(querysysattr, BooleanClause.Occur.SHOULD);
            SortField sorttitle = new SortField("title", SortField.SCORE, false);
            SortField sortkeywords = new SortField("keywords", SortField.SCORE, false);
            SortField sortsysattr = new SortField("sysattr", SortField.SCORE, false);
            Sort sort = new Sort(new SortField[] { sorttitle, sortkeywords, sortsysattr });
            //Hits hits = searcher.Search(bqAllFilter);
            Hits hits = searcher.Search(bqAllFilter, sort);
            if (currpage >= 0 && pagesize > 0 && currpage * pagesize < hits.Length()) 
            {
                int i = currpage * pagesize;
                while (i < (currpage + 1) * pagesize)
                {
                    ShowInfo info = new ShowInfo();
                    info.Id = int.Parse(hits.Doc(i).Get("id"));

                    info.Title = Common.SplitContent.HightLight(keyword, hits.Doc(i).Get("title"));
                    //pro.Keywords = hits.Doc(i).Get("keywords");
                    info.Keywords = Common.SplitContent.HightLight(keyword, hits.Doc(i).Get("keywords"));
                    if (string.IsNullOrEmpty(info.Keywords))
                    {
                        info.Keywords = hits.Doc(i).Get("keywords");
                    }
                    info.Detail = Common.SplitContent.HightLight(keyword, hits.Doc(i).Get("detail"));
                    if (string.IsNullOrEmpty(info.Detail))
                    {
                        info.Detail = hits.Doc(i).Get("detail");
                    }
                    prolist.Add(info);
                    i++;
                }
            }
            return prolist;
        }
	}
}