using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class PositionalIndexController : Controller
    {
        public IActionResult Index(string t, List<string> searchtext, string boolWords, string tok, string norm, string lemm, string stops, string stem)
        {

            indexingQRY indexingQRY = new indexingQRY();

            allPre a = new allPre();
            var dict = a.Indexing("lucene", tok, norm, lemm, stops, stem);

            var po = new PositionalIndex();
            int i = 1;
            foreach(var d in dict)
            {
                po.AddDocument(i++, d.Value);
            }
           
            var query = new string[] {};
            query = t.Split(' ');
            var docNums = po.Search(query);

            ViewBag.result = docNums;
            ViewBag.word = searchtext;

            return View();
        }
    }
}
class PositionalIndex
{
    private readonly Dictionary<string, Dictionary<int, List<int>>> index = new Dictionary<string, Dictionary<int, List<int>>>();

    public void AddDocument(int docNum, string document)
    {
        var words = document.Split(' ');
        for (int i = 0; i < words.Length; i++)
        {
            var word = words[i];
            if (!index.ContainsKey(word))
            {
                index[word] = new Dictionary<int, List<int>>();
            }
            if (!index[word].ContainsKey(docNum))
            {
                index[word][docNum] = new List<int>();
            }
            index[word][docNum].Add(i);
        }
    }

    public List<int> Search(string [] query)
    {
        var docNums = new HashSet<int>();
        foreach (var word in query)
        {
            if (index.ContainsKey(word))
            {
                var postings = index[word];
                if (docNums.Count == 0)
                {
                    docNums.UnionWith(postings.Keys);
                }
                else
                {
                    docNums.IntersectWith(postings.Keys);
                }
            }
            else
            {
                return new List<int>();
            }
        }
        var result = new List<int>();
        foreach (var docNum in docNums)
        {
            var positions = index[query[0]][docNum];
            for (int i = 1; i < query.Length; i++)
            {
                positions = Intersect(positions, index[query[i]][docNum]);
            }
            if (positions.Count > 0)
            {
                result.Add(docNum);
            }
        }
        return result;
    }

    private List<int> Intersect(List<int> a, List<int> b)
    {
        var result = new List<int>();
        int i = 0, j = 0;
        while (i < a.Count && j < b.Count)
        {
            if (a[i] == b[j] - 1)
            {
                result.Add(b[j]);
                i++;
                j++;
            }
            else if (a[i] < b[j] - 1)
            {
                i++;
            }
            else
            {
                j++;
            }
        }
        return result;
    }
}
