using Lucene.Net.Documents;
using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class InvertedIndexController : Controller
    {
        Dictionary<string, List<int>> xx = new Dictionary<string, List<int>>();
        public IActionResult Index(string t,List<string> searchtext, string boolWords, string tok, string norm, string lemm, string stops, string stem)
        {

            string operators = boolWords;

            RemoveStopWords x = new RemoveStopWords();
            indexingQRY indexingQRY = new indexingQRY();

            var index = new Inverted();

            allPre a = new allPre();
            var dict = a.Indexing("lucene", tok, norm, lemm, stops, stem);

            foreach (var document in dict)
            {
                 xx = index.AddDocument(document.Key,x.GetTermsForOneDocumentInverted(document.Value));
            }

            Dictionary<int, int> r = new Dictionary<int, int>();

            for (int i = 0; i <= dict.Count; i++)
            {
                r[i] = 0;
            }

            List<int> result = new List<int>();
            List<int> res = new List<int>();

            foreach (string q in searchtext)
            {
                string[] s = { q };
                res = index.Search(s);

                foreach (var i in res)
                {
                    r[i]++;
                }
            }
            if (operators == null)
            {
                foreach (var i in r)
                {
                    if (i.Value >= 1)
                    {
                        result.Add(i.Key);
                    }
                }
            }

            if (operators == "and")
            {
                foreach (var i in r)
                {
                    if (i.Value >= 2)
                    {
                        result.Add(i.Key);
                    }
                }
            }
            if (operators == "or")
            {
                foreach (var i in r)
                {
                    if (i.Value >= 1)
                    {
                        result.Add(i.Key);
                    }
                }
            }

            List<string> searchResult= new List<string>();

            foreach (var docId in result)
            {
                searchResult.Add(docId.ToString());
            }

            ViewBag.results = searchResult;
            ViewBag.word = t;

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
