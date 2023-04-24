using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace IRProject.Controllers
{
    public class TermDocumentIncidenceMatrixController : Controller
    {
        public IActionResult Index(string searchtext)
        {

            string documentsPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section";
            RemoveStopWords x = new RemoveStopWords();


            // Define the list of documents and terms
            List<string> documents = x.Files(documentsPath);

            //List<string> documents = new List<string> { "saber elsayed saber"  , "saner maher elsayed saber", "saner maher elsayed saber" };

            HashSet<string> terms = new HashSet<string>();

            foreach (string document in documents)
            {
                string[] words = document.Split(' ');

                // Convert the words to lowercase and store them in a list of terms
                foreach (string w in words)
                {
                    string term = w.ToLower();
                    terms.Add(term);
                }
            }

            List<string> termes = terms.ToList();

            // Create the incidence matrix
            int[,] incidenceMatrix = new int[termes.Count, documents.Count];
            for (int i = 0; i < documents.Count; i++)
            {
                string document = documents[i];
                string[] words = document.Split(' ');

                for (int j = 0; j < termes.Count; j++)
                {
                    string term = termes[j];
                    int count = 0;

                    foreach (string w in words)
                    {
                        if (w == term)
                        {
                            count = 1;
                        }
                    }

                    incidenceMatrix[j, i] = count;
                }
            }

            //for (int i = 0; i < termes.Count; i++)
            //{
            //    Console.Write(termes[i] + ": ");
            //    for (int j = 0; j < documents.Count; j++)
            //    {
            //        Console.Write(incidenceMatrix[i, j] + " ");
            //    }
            //    Console.WriteLine();
            //}

            // Search for a specific word
            List<int> r = new List<int>();
            int wordIndex = termes.IndexOf(searchtext);
            if (wordIndex != -1)
            {
                //Console.Write("Document ");

                for (int i = 0; i < documents.Count; i++)
                {
                    if (incidenceMatrix[wordIndex, i] > 0)
                    {
                        r.Add(i + 1);
                    }
                }
            }
            else
            {
                Console.WriteLine("Not Found");
            }

            ViewBag.r = r;


            //string documentspath = "F:\\L4  S Semester\\Projects\\IR\\wwwroot\\Attaches\\Documents\\Documents\\Section\\";

            //RemoveStopWords x = new RemoveStopWords();

            ////{ "hello world", "goodbye world", "hello goodbye" }  { "hello there", "goodbye for now", "hello again" }
            //List<string> strings = x.NormalizeTerms(x.GetTerms(x.Files(documentspath))).ToList();
            //strings.Sort();   
            //List<string> documents = x.Files(documentspath);

            //List<string> docs = new List<string>();

            //(int[,] tdim, Dictionary<string, int> termIndices) = CreateTDIM(strings, documents);



            //ViewBag.re = tdim;
            //ViewBag.terms = termIndices;

            //for (int j = 0; j < documents.Count; j++)
            //{
            //    docs.Add($"Document {j+1}");
            //}

            //ViewBag.docsCount = docs.Count();

            return View();
        }


        public static (int[,], Dictionary<string, int>) CreateTDIM(List<string> strs, List<string> Docs)
        {
           
            // normalize the strings and the documents
            List<List<string>> normalizedStrings = NormalizeStrings(strs);
            List<List<string>> normalizedDocuments = NormalizeStrings(Docs);

            // create a set of all terms
            HashSet<string> termss = new HashSet<string>();
            foreach (List<string> document in normalizedDocuments)
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

    public List<string> GetTermsForOneDocument(string documents)
    {
        var terms = new HashSet<string>();

        var documentTerms = documents.Split(' ');
        foreach (var term in documentTerms)
        {
            if (term.Length > 0)
            {
                if (term[0] >= 'a' && term[0] <= 'z')
                {
                    terms.Add(term);
                }
            }

        }

        List<string> result = terms.ToList();
        result.Sort();
        return result;
    }



    public List<string> Files(string documentsPath)
    {
        List<string> documents = new List<string>();

        documents.Clear();
        foreach (string fileName in Directory.GetFiles(documentsPath, "*.*"))
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

       
        return terms.ToList();
    }


    public HashSet<string> NormalizeTerms(List<string> terms)
    {
        List<string> normalizedTerms = new List<string>();
        HashSet<string> stopWords = new HashSet<string> {};

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
