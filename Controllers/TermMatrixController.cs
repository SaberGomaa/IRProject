using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace IRProject.Controllers
{
    public class TermMatrixController : Controller
    {
        public IActionResult Index()
        {

            string documentspath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section";

            RemoveStopWords x = new RemoveStopWords();

            indexingQRY indexingQRY = new indexingQRY();

            var dict = indexingQRY.docs();


            //{ "hello world", "goodbye world", "hello goodbye" }  { "hello there", "goodbye for now", "hello again" }
            List<string> strings = new List<string>();

            foreach(var d in dict)
            {
                strings.Add(d.Value);
            }

            strings.Sort();
            List<string> documents = strings;

            List<string> docs = new List<string>();

            (int[,] tdim, Dictionary<string, int> termIndices) = CreateTDIM(strings, documents);

            ViewBag.re = tdim;
            ViewBag.terms = termIndices;

            for (int j = 0; j < documents.Count; j++)
            {
                docs.Add($"Document {j + 1}");
            }

            ViewBag.docsCount = docs.Count();

            return View();
        }



        public static (int[,], Dictionary<string, int>) CreateTDIM(List<string> strs, List<string> Docs)
        {

            // normalize the strings and the documents
            List<List<string>> normalizedStrings = NormalizeStrings(strs);
            List<List<string>> normalizedDocuments = NormalizeStrings(Docs);

            // create a set of all terms
            HashSet<string> termss = new HashSet<string>();
            foreach (List<string> document in normalizedStrings)
            {
                termss.UnionWith(document);
            }
            foreach (List<string> stringList in normalizedStrings)
            {
                termss.UnionWith(stringList);
            }
            List<string> terms = termss.ToList();
            terms.Sort();


            // create the TDIM matrix
            int[,] tdim = new int[terms.Count, Docs.Count];
            Dictionary<string, int> termIndices = new Dictionary<string, int>();
            int termIndex = 0;
            foreach (string term in terms)
            {
                termIndices.Add(term, termIndex);
                int documentIndex = 0;
                foreach (List<string> document in normalizedDocuments)
                {
                    if (document.Contains(term))
                    {
                        tdim[termIndex, documentIndex] = 1;
                    }
                    documentIndex++;
                }
                termIndex++;
            }
            return (tdim, termIndices);
        }

        public static List<List<string>> NormalizeStrings(List<string> strings)
        {
            List<List<string>> normalizedStrings = new List<List<string>>();
            foreach (string str in strings)
            {
                List<string> normalizedString = new List<string>();
                string[] words = Regex.Split(str.ToLower(), @"\W+");
                foreach (string word in words)
                {
                    if (word.Length > 2)
                    {
                        normalizedString.Add(word);
                    }
                }
                normalizedStrings.Add(normalizedString);
            }
            return normalizedStrings;
        }

    }
}

