using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class BiWordIndexController : Controller
    {
        public IActionResult Index(string t, List<string> searchtext, string boolWords, string tok, string norm, string lemm, string stops, string stem)
        {
            indexingQRY indexingQRY = new indexingQRY();

            allPre a = new allPre();
            var dict = a.Indexing("invert", tok, norm, lemm, stops, stem);

            List<string> documents = new List<string>();
            foreach (var i in dict)
            {
                documents.Add(i.Value);
            }

            // Create bi-word index
            Dictionary<string, List<int>> biwordIndex = new Dictionary<string, List<int>>();
            for (int i = 0; i < documents.Count; i++)
            {
                string[] words = documents[i].Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < words.Length - 1; j++)
                {
                    string biword = words[j] + " " + words[j + 1];
                    if (!biwordIndex.ContainsKey(biword))
                    {
                        biwordIndex[biword] = new List<int>();
                    }
                    biwordIndex[biword].Add(i);
                }
            }

            // Search for a bi-word and return document IDs
            string biwordQuery = t;
            if (biwordIndex.ContainsKey(biwordQuery))
            {
                List<int> docIDs = biwordIndex[biwordQuery];
                ViewBag.result = docIDs;
            }
      

            ViewBag.word = t;
            return View();
        }

    }
}
class BiWordIndex
{
    private readonly Dictionary<string, List<int>> index = new Dictionary<string, List<int>>();

    public void AddDocuments(List<string> documents)
    {
        for (int docNum = 0; docNum < documents.Count; docNum++)
        {
            var words = documents[docNum].Split(' ');
            for (int i = 0; i < words.Length - 1; i++)
            {
                var biword = words[i] + " " + words[i + 1];
                if (!index.ContainsKey(biword))
                {
                    index[biword] = new List<int>();
                }
                if (!index[biword].Contains(docNum))
                {
                    index[biword].Add(docNum+1);
                }
            }
        }
    }

    public List<int> Search(string query)
    {
        var words = query.Split(' ');
        var biwords = new List<string>();
        for (int i = 0; i < words.Length - 1; i++)
        {
            biwords.Add(words[i] + " " + words[i + 1]);
        }
        var docNums = new HashSet<int>();
        foreach (var biword in biwords)
        {
            if (index.ContainsKey(biword))
            {
                docNums.UnionWith(index[biword]);
            }
        }
        return docNums.ToList();
    }
}

