using IRProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IRProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        //public IActionResult Index()
        //{
        //    return RedirectToAction("indexing" , "indexing");
        //}

        public IActionResult Index()
        {
            return RedirectToAction("Welcome");
        }

        public IActionResult Welcome()
        {
            return View();
        }

        public IActionResult Searching()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Searching(string searchtext, string selected, string tok, string norm, string lemm, string stops, string stem)
        {

            List<string> searchtexts = searchtext.ToLower().Split(' ').ToList();

            List<string> strings= new List<string>();
            string boolWords = "";

            foreach(var i in searchtexts)
            {

                if(i == "and" || i == "or" || i == "not")
                {
                    boolWords = i;
                }
                else
                {
                    strings.Add(i);
                }
            }

            if (selected == "lucene")
            {
                return RedirectToAction("index", "InvertedIndex", new {t= searchtext , searchtext = strings , boolwords = boolWords, tok = tok, norm = norm, lemm = lemm , stops = stops , stem = stem});
            }
            else if(selected == "term") 
            {

                return RedirectToAction("index", "TermDocumentIncidenceMatrix", new { t = searchtext, searchtext = strings, boolwords = boolWords, norm = norm, lemm = lemm, stops = stops, stem = stem });
            }
            else if (selected == "invert")
            {
                return RedirectToAction("index", "InvertedIndex", new { t = searchtext, searchtext = strings, boolwords = boolWords, norm = norm, lemm = lemm, stops = stops, stem = stem });
            }
            else if (selected == "position")
            {
                return RedirectToAction("index", "PositionalIndex", new { t = searchtext, searchtext = strings, boolwords = boolWords, norm = norm, lemm = lemm, stops = stops, stem = stem });
            }
            else if (selected == "biword")
            {
                return RedirectToAction("index", "BiWordIndex", new { t = searchtext, searchtext = strings, boolwords = boolWords, norm = norm, lemm = lemm, stops = stops, stem = stem });
            }
            else
            {
                return View();
            }
        }

        //public IActionResult Searching()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Searching(string searchtext)
        //{
        //    string documentsPath = "F:\\L4  S Semester\\IR\\Projects\\Project\\IRProject\\wwwroot\\Attaches\\Documents\\Documents\\";

        //    var directory = FSDirectory.Open("F:\\L4  S Semester\\IR\\Projects\\Project\\IRProject\\wwwroot\\Attaches\\Documents\\");


        //    StandardAnalyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        //    IndexWriterConfig config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
        //    IndexWriter writer = new IndexWriter(directory, config);

        //    // Index all text files in documents directory
        //    foreach (string file in Directory.GetFiles(documentsPath, "*.txt"))
        //    {

        //        string text = ExtractText(file);
        //        Document doc = new Document();
        //        doc.Add(new TextField("filename", Path.GetFileName(file), Field.Store.YES));
        //        doc.Add(new TextField("content", text, Field.Store.YES));
        //        writer.AddDocument(doc);
        //    }

        //    writer.Commit();
        //    writer.Dispose();

        //    // Create index searcher
        //    IndexReader reader = DirectoryReader.Open(directory);
        //    IndexSearcher searcher = new IndexSearcher(reader);

        //    // Create search query
        //    StandardAnalyzer queryAnalyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
        //    QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, "content", queryAnalyzer);


        //    Query query = parser.Parse(searchtext);

        //    HashSet<string> list = new HashSet<string>();

        //    // Execute search and display results
        //    TopDocs results = searcher.Search(query, 10);
        //    //Console.WriteLine("Found {0} documents matching the query '{1}':", results.TotalHits, query.ToString());
        //    foreach (ScoreDoc scoreDoc in results.ScoreDocs)
        //    {
        //        Document doc = searcher.Doc(scoreDoc.Doc);
        //        //Console.WriteLine(" - {0} ({1})", doc.Get("filename"), scoreDoc.Score);
        //        list.Add(doc.Get("filename"));
        //    }

        //    // Close index reader
        //    reader.Dispose();

        //    ViewBag.result = list;
        //    ViewBag.text = searchtext;

        //    return View("LuceneResult");
        //}

        //private string ExtractText(string file)
        //{
        //    TextExtractor extractor = new TextExtractor();

        //    string r = System.IO.File.ReadAllText(file);

        //    return r;

        //}

        //public IActionResult Indexing()
        //{
        //    return View();
        //}

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}