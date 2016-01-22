using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Lucene.Net;
using PanGu;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.PanGu;
using System.IO;

namespace Lucenemvc.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        public ActionResult Index()
        {
            PanGuAnalyzer analyzer = new PanGuAnalyzer();
            TokenStream ts = analyzer.ReusableTokenStream("", new StringReader("供应子母门　非标门　钢质门　进户门　金属门"));
            Token token;
            while ((token = ts.Next()) != null)
            {
                Response.Write(token.TermText() + ";");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(string content)
        {
            return View();
        }
	}
}