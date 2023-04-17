using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class InvertedIndexController : Controller
    {
        public IActionResult Index(string query)
        {

            string documentspath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section";

            RemoveStopWords x = new RemoveStopWords();

            List<string> documents = x.Files(documentspath);

            var index = new Inverted();
            int docID = 1;

            foreach (var document in documents)
            {
                index.AddDocument(docID++, x.GetTermsForOneDocument(document));
            }

            string [] q = new[] { query };

            var result = index.Search(q);

            List<string> searchResult= new List<string>();

            foreach (var docId in result)
            {
                searchResult.Add(docId.ToString());
            }

            ViewBag.results = searchResult;

            return View();
        }

    }

    public class Inverted
    {
        private Dictionary<string, List<int>> index = new Dictionary<string, List<int>>();

        public void PrintIndex()
        {
            foreach (var kvp in index)
            {
                Console.Write($"{kvp.Key}: ");
                foreach (var docId in kvp.Value)
                {
                    Console.Write($"{docId}, ");
                }
                Console.WriteLine();
            }
        }
        public void AddDocument(int docId, List<string> terms)
        {
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
