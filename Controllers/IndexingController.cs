using Microsoft.AspNetCore.Mvc;
using Porter2Stemmer;
using System.Text.RegularExpressions;

namespace IRProject.Controllers
{
    public class IndexingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Indexing() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Indexing(string selected, string tok, string norm, string lemm, string stops, string stem)
        {
            
            Preprocessing preprocessing = new Preprocessing();

            string dirPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section\\CISI.QRY";
            string dirPath2 = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section\\CISI.ALL";

            string text = System.IO.File.ReadAllText(dirPath);
            text += System.IO.File.ReadAllText(dirPath2);

            string[] files = new string[] { };

            files = text.Split(".I");

            List<int> l = new List<int>();

            Dictionary<int, string> dict = new Dictionary<int, string>();
            int c = 0;

            foreach (string file in files)
            {
                string result = file; 
                if(stops == "on")
                {
                    result = preprocessing.StopWords(file);
                }
                if(norm == "on")
                {
                    result = preprocessing.NormalizeOneDocument(file);
                }

                if(stem == "on")
                {
                    result = preprocessing.StemmingOneDocument(result);
                }
                if (stem == "on")
                {
                    result = preprocessing.StemmingOneDocument(result);
                }
                string s = "";
                if (file.Length > 0)
                {
                    for (int i = 1; i < file.Length; i++)
                    {
                        if (file[i] >= '0' && file[i] <= '9')
                            s += file[i];
                        else
                            break;
                    }
                    if (s != "")
                        l.Add(int.Parse(s));

                }
                dict.Add(c++, file);

            }

            return RedirectToAction("searching","home");
        }
    }
}
class allPre
{
    public Dictionary<int,string> Indexing(string selected, string tok, string norm, string lemm, string stops, string stem)
    {

        Preprocessing preprocessing = new Preprocessing();

        string dirPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section\\CISI.QRY";

        string text = System.IO.File.ReadAllText(dirPath);

        string[] files = new string[] { };

        files = text.Split(".I");

        List<int> l = new List<int>();

        Dictionary<int, string> dict = new Dictionary<int, string>();
        int c = 0;

        foreach (string file in files)
        {
            string result = file;
            if (stops == "on")
            {
                result = preprocessing.StopWords(file);
            }
            if (norm == "on")
            {
                result = preprocessing.NormalizeOneDocument(file);
            }

            if (stem == "on")
            {
                result = preprocessing.StemmingOneDocument(result);
            }

            string s = "";
            if (file.Length > 0)
            {
                for (int i = 1; i < file.Length; i++)
                {
                    if (file[i] >= '0' && file[i] <= '9')
                        s += file[i];
                    else
                        break;
                }
                if (s != "")
                    l.Add(int.Parse(s));

            }
            dict.Add(c++, result);

        }

        return dict; 
    }

}
class Preprocessing
{
    List<string> documents = new List<string>();

    public string StemmingOneDocument(string document)
    {

        string text = document;
        var stemmer = new EnglishPorter2Stemmer();
        string[] words = text.Split(' ');
        string stemmedText = string.Empty;

        foreach (string word in words)
        {
            word.ToLower();
            if (word.Length > 0 )
            {
                stemmedText += stemmer.Stem(word).Value + " ";
            }
        }

        string result = stemmedText;

        return result;
    }


    public string Lemmatization(string document)
    {

        string text = document;
        var Lemmatization = new EnglishPorter2Stemmer();
        string[] words = text.Split(' ');
        string LemmatizationText = string.Empty;

        foreach (string word in words)
        {
            word.ToLower();
            if (word.Length > 0)
            {
                LemmatizationText += Lemmatization.Stem(word).Value + " ";
            }
        }
        string result = LemmatizationText;

        return result;
    }


    public string StopWords(string d)
    {
        HashSet<string> stopWords = new HashSet<string> { "the", "and", "a", "of", "an", "as", "with", "at", "yes", "yet", "you" };
        string ReTerms = "";

        string[] documents = d.Split(' ');

        foreach (var t in documents)
        {
            if (!stopWords.Contains(t) && t.Length > 2)
            {
                ReTerms += t + " ";
            }
        }

        return ReTerms ;
    }

    public List<string> StopWordsOneDocument(string document)
    {
        HashSet<string> stopWords = new HashSet<string> { "the", "and", "a", "of", "an", "as", "with", "at", "yes", "yet", "you" };
        HashSet<string> ReTerms = new HashSet<string>();

        string[] terms = document.Split(' ');

        foreach (var t in terms)
        {
            if (!stopWords.Contains(t) && !t.Contains("\n") && !t.Contains("\t") && t.Length > 2)
            {
                ReTerms.Add(t);
            }
        }

        return ReTerms.ToList();
    }
    public string StopWordsOneDocumentLucene(string document)
    {
        HashSet<string> stopWords = new HashSet<string> { "the", "and", "a", "of", "an", "as", "with", "at", "yes", "yet", "you" };
        string ReTerms = "";

        string[] terms = document.Split(' ');

        foreach (var t in terms)
        {
            if (!stopWords.Contains(t) && !t.Contains("\n") && !t.Contains("\t") && t.Length > 2)
            {
                ReTerms += t + " ";
            }
        }

        return ReTerms;
    }
    public List<string> NormalizeDocuments(List<string> documents)
    {
        List<string> result = new List<string>();

        foreach (string document in documents)
        {
            string[] terms = document.Split(' ');

            foreach (string t in terms)
            {
                //string outputWord = Regex.Replace(t, @"[\p{P}\d]", "");
                string outputWord = Regex.Replace(t, @"\b\w*\d\w*\b", "");
                if (outputWord.Length > 2)
                {
                    result.Add(outputWord);
                }
            }
        }
        return result;
    }

    public string NormalizeOneDocument(string document)
    {
        string result = "";

        string[] terms = document.Split(' ');

        foreach (string t in terms)
        {
            string pattern = "[\r\n]";
            string output1 = Regex.Replace(t, pattern, "");
            string p = "\\s+";
            string output2 = Regex.Replace(output1, p, " ");

            string outputWord = Regex.Replace(output2, @"[\p{P}\d]", " ");

            result += outputWord + " ";
        }
        return result;
    }

    public List<string> Files(string documentsPath)
    {
        List<string> docu = new List<string>();
        foreach (string fileName in Directory.GetFiles(documentsPath, "CISI.*"))
        {
            docu.Add(File.ReadAllText(fileName).ToLower());
        }
        return docu.ToList();
    }
    public List<string> GetTerms(List<string> documents)
    {
        var terms = new HashSet<string>();

        foreach (var document in documents)
        {
            var documentTerms = document.Split(' ');
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
        }

        List<string> result = terms.ToList();
        result.Sort();
        return result;
    }
    public List<string> GetTermsForOneDocumentInverted(string documents)
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

    public string GetTermsForOneDocument(string documents)
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

        string result = terms.ToString();

        return result;
    }
    public void display(HashSet<string> terms)
    {
        foreach (var term in terms)
        {
            Console.WriteLine(term);
        }
    }

    //public HashSet<string> NormalizeTerms(List<string> terms)
    //{
    //    List<string> normalizedTerms = new List<string>();
    //    HashSet<string> stopWords = new HashSet<string> { "the", "and", "a" , "of" , "an"};

    //    foreach (var t in terms)
    //    {
    //        string[] strings = t.Split(' ');
    //        foreach (string term in strings)
    //        {
    //            // convert to lowercase and remove punctuation
    //            string normalizedTerm = Regex.Replace(term.ToLower(), @"[\p{P}\p{S}]", "");

    //            // remove stop words
    //            if (!stopWords.Contains(normalizedTerm))
    //            {
    //                // apply stemming or lemmatization
    //                // (omitted for simplicity in this example)
    //                normalizedTerms.Add(normalizedTerm);
    //            }
    //        }
    //    }

    //    // remove digits
    //    HashSet<string> filteredStrings = normalizedTerms.Where(s => !s.Any(char.IsDigit)&& !s.Any(char.IsWhiteSpace)).ToHashSet();

    //    return filteredStrings;
    //}

}
