using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace IRProject.Controllers
{
    public class TermDocumentIncidenceMatrixController : Controller
    {
        public IActionResult Index()
        {

            string documentspath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\";

            RemoveStopWords x = new RemoveStopWords();

            //{ "hello world", "goodbye world", "hello goodbye" }  { "hello there", "goodbye for now", "hello again" }
            List<string> strings = x.NormalizeTerms(x.GetTerms(x.Files(documentspath))).ToList();
            strings.Sort();   
            List<string> documents = x.Files(documentspath);

            List<string> docs = new List<string>();

            (int[,] tdim, Dictionary<string, int> termIndices) = CreateTDIM(strings, documents);

            termIndices.OrderBy(x => x.Key).ToList();

            ViewBag.re = tdim;
            ViewBag.terms = termIndices;

            for (int j = 0; j < documents.Count / 2; j++)
            {
                docs.Add($"Document {j+1}");
            }


            return View();
        }



        public static (int[,], Dictionary<string, int>) CreateTDIM(List<string> strings, List<string> documents)
        {
            // normalize the strings and the documents
            List<List<string>> normalizedStrings = NormalizeStrings(strings);
            List<List<string>> normalizedDocuments = NormalizeStrings(documents);

            // create a set of all terms
            HashSet<string> terms = new HashSet<string>();
            foreach (List<string> document in normalizedDocuments)
            {
                terms.UnionWith(document);
            }
            foreach (List<string> stringList in normalizedStrings)
            {
                terms.UnionWith(stringList);
            }

            // create the TDIM matrix
            int[,] tdim = new int[terms.Count, documents.Count];
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
                    normalizedString.Add(word);
                }
                normalizedStrings.Add(normalizedString);
            }
            return normalizedStrings;
        }

    }
}

class RemoveStopWords
{
    List<string> documents = new List<string>();

    public List<string> Files(string documentsPath)
    {
        foreach (string fileName in Directory.GetFiles(documentsPath, "*.txt"))
        {
            documents.Add(File.ReadAllText(fileName));
        }
        return documents.ToList();
    }


    public List<string> GetTerms(List<string> documents)
    {
        var terms = new HashSet<string>();

        foreach (var document in documents)
        {
            var documentTerms = document.Split(' ');
            foreach (var term in documentTerms)
            {
                terms.Add(term);
            }
        }

        List<string> result = terms.ToList();
        result.Sort();
        return result;
    }

    public void display(HashSet<string> terms)
    {
        foreach (var term in terms)
        {
            Console.WriteLine(term);
        }
    }

    public HashSet<string> NormalizeTerms(List<string> terms)
    {
        List<string> normalizedTerms = new List<string>();
        HashSet<string> stopWords = new HashSet<string> { "the", "and", "a" };

        foreach (string term in terms)
        {
            // convert to lowercase and remove punctuation
            string normalizedTerm = Regex.Replace(term.ToLower(), @"[\p{P}\p{S}]", "");

            // remove stop words
            if (!stopWords.Contains(normalizedTerm))
            {
                // apply stemming or lemmatization
                // (omitted for simplicity in this example)
                normalizedTerms.Add(normalizedTerm);
            }
        }

        // remove digits
        HashSet<string> filteredStrings = normalizedTerms.Where(s => !s.Any(char.IsDigit) && !s.Any(char.IsWhiteSpace)).ToHashSet();

        return filteredStrings;
    }

}
