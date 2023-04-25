using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class BiWordIndexController : Controller
    {
        public IActionResult Index(string searchtext)
        {
            string documentsPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section";
            RemoveStopWords x = new RemoveStopWords();

            List<string> documents = x.Files(documentsPath);

            BiWordIndex bi = new BiWordIndex();

            bi.AddDocuments(documents);

            List<int> l = bi.Search(searchtext);

            ViewBag.result = l;
            ViewBag.word = searchtext;
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

