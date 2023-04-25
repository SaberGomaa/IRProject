using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class PositionalIndexController : Controller
    {
        public IActionResult Index(string searchtext)
        {

            string documentsPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section";
            RemoveStopWords x = new RemoveStopWords();

            List<string> documents = x.Files(documentsPath);

            var po = new PositionalIndex();
            po.AddDocument(1, documents[0]);
            po.AddDocument(2, documents[1]);
            po.AddDocument(3, documents[2]);
            po.AddDocument(4, documents[3]);

            var query = new string[] {};
            query = searchtext.Split(' ');
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
