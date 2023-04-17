using Lucene.Net.Documents;
using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class InvertedIndexController : Controller
    {
        Dictionary<string, List<int>> xx = new Dictionary<string, List<int>>();

        public IActionResult Index(string searchtext)
        {

            string documentspath = "F:\\L4  S Semester\\Projects\\IR\\wwwroot\\Attaches\\Documents\\Documents\\Section\\";

            RemoveStopWords x = new RemoveStopWords();

            List<string> fNames = new List<string>();


            foreach (string fileName in Directory.GetFiles(documentspath, "*.*"))
            {
                fNames.Add(fileName);
            }

            List<string> documents = x.Files(documentspath);

            var index = new Inverted();
            int docID = 1;

            foreach (var document in documents)
            {
                 xx = index.AddDocument(docID++, x.GetTermsForOneDocument(document));
            }


            ViewBag.index = xx;

            
                foreach(var y in xx)
                {
                    var ss = y.Key;
                    foreach(var a in y.Value)
                    {
                        var s = a;
                    }
                }
            

            string [] q = new[] { searchtext };

            var result = index.Search(q);

            List<string> searchResult= new List<string>();

            foreach (var docId in result)
            {
                searchResult.Add(docId.ToString());
            }

            ViewBag.results = searchResult;
            ViewBag.word = searchtext;

            return View();
        }

        public ActionResult PrintMatrix()
        {

            var i = new Inverted();

            return View();
        }

    }

    

    public class Inverted
    {
        private Dictionary<string, List<int>> index = new Dictionary<string, List<int>>();

       
        public Dictionary<string, List<int>> AddDocument(int docId, List<string> terms)
        {
            List<Dictionary<string, List<int>>> list = new List<Dictionary<string, List<int>>>();
            foreach (string term in terms)
            {
                if (!index.ContainsKey(term))
                {
                    index[term] = new List<int>();
                }
                if (!index[term].Contains(docId))
                {
                    index[term].Add(docId);
                }
            }
            return index;
        }

        public List<int> Search(string[] terms)
        {
            List<int> result = null;
            foreach (string term in terms)
            {
                if (index.ContainsKey(term))
                {
                    if (result == null)
                    {
                        result = new List<int>(index[term]);
                    }
                    else
                    {
                        result = result.Intersect(index[term]).ToList();
                    }
                }
                else
                {
                    return new List<int>();
                }
            }
            return result ?? new List<int>();
        }
    }

}
