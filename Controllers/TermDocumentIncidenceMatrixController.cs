using com.sun.org.apache.regexp.@internal;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace IRProject.Controllers
{
    public class TermDocumentIncidenceMatrixController : Controller
    {
        public IActionResult Index(string t, List<string> searchtext, string boolWords, string tok, string norm, string lemm, string stops, string stem)
        {
            string operators = boolWords;

            indexingQRY indexingQRY = new indexingQRY();
            RemoveStopWords removeStop = new RemoveStopWords();

            allPre a = new allPre();
            var dict = a.Indexing("lucene", tok, norm, lemm, stops, stem);

            // Sample documents
            List<string> documents = new List<string>();

            foreach (var i in dict)
            {
                string s = removeStop.NormalizeOneDocument(i.Value);
                documents.Add(i.Value.ToLower());
            }


            // Get unique terms
            HashSet<string> terms = new HashSet<string>();
            foreach (string doc in documents)
            {
                string[] words = doc.Split(' ');
                foreach (string word in words)
                {
                    terms.Add(word);
                }
            }

            // Create term-document incidence matrix
            int[,] matrix = new int[terms.Count, documents.Count];
            for (int j = 0; j < documents.Count; j++)
            {
                string[] words = documents[j].Split(' ');
                for (int i = 0; i < terms.Count; i++)
                {
                    if (Array.IndexOf(words, terms.ElementAt(i)) >= 0)
                    {
                        matrix[i, j] = 1;
                    }
                }
            }

            // Search for a word and print document IDs
            Dictionary<int, int> keyValuePairs = new Dictionary<int, int>();
            for (int i = 0; i <= dict.Count; i++)
            {
                keyValuePairs.Add(i, 0);
            }

            foreach (string searchWord in searchtext)
            {

                int rowIndex = -1;
                for (int i = 0; i < terms.Count; i++)
                {
                    if (terms.ElementAt(i) == searchWord)
                    {
                        rowIndex = i;
                        break;
                    }
                }

                List<int> r = new List<int>();

                for (int j = 0; j < documents.Count; j++)
                {
                    if (matrix[rowIndex, j] == 1)
                    {
                        r.Add((j));
                    }
                }

                foreach(var i in r)
                {
                    keyValuePairs[i] ++;
                }

            }

            List<int> result = new List<int>();

            if (operators == null)
            {
                foreach (var i in keyValuePairs)
                {
                    if (i.Value >= 1)
                    {
                        result.Add(i.Key);
                    }
                }
            }

            if (operators == "and")
            {
                foreach (var i in keyValuePairs)
                {
                    if (i.Value >= 2)
                    {
                        result.Add(i.Key);
                    }
                }
            }
            if (operators == "or")
            {
                foreach (var i in keyValuePairs)
                {
                    if (i.Value >= 1)
                    {
                        result.Add(i.Key);
                    }
                }
            }



            // Print matrix
            //Console.WriteLine("Term-Document Incidence Matrix:");
            //Console.Write("\t");
            //for (int j = 0; j < documents.Count; j++)
            //{
            //    Console.Write("D" + (j + 1) + "\t");
            //}
            //Console.WriteLine();
            //for (int i = 0; i < terms.Count; i++)
            //{
            //    Console.Write(terms.ElementAt(i) + "\t");
            //    for (int j = 0; j < documents.Count; j++)
            //    {
            //        Console.Write(matrix[i, j] + "\t");
            //    }
            //    Console.WriteLine();
            //}


            ViewBag.result = result;

            //string operators = boolWords;

            //indexingQRY indexingQRY = new indexingQRY();

            //var dict = indexingQRY.docs();

            //string documentsPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section";
            //RemoveStopWords x = new RemoveStopWords();


            //// Define the list of documents and terms
            //List<string> documents = new List<string>();
            ////List<string> documents = x.Files(documentsPath);

            //foreach(var d in dict)
            //{
            //    documents.Add(d.Value);
            //}

            ////List<string> documents = new List<string> { "saber elsayed saber"  , "saner maher elsayed saber", "saner maher elsayed saber" };

            //HashSet<string> terms = new HashSet<string>();

            //foreach (string document in documents)
            //{
            //    string[] words = document.Split(' ');
            //    string term = "";
            //    // Convert the words to lowercase and store them in a list of terms
            //    foreach (string w in words)
            //    {
            //        term = w.ToLower();
            //        terms.Add(term);
            //    }
            //}

            //List<string> termes = terms.ToList();

            //// Create the incidence matrix
            //int[,] incidenceMatrix = new int[termes.Count, documents.Count];
            //for (int i = 0; i < documents.Count; i++)
            //{
            //    string document = documents[i];
            //    string[] words = document.Split(' ');

            //    for (int j = 0; j < termes.Count; j++)
            //    {
            //        string term = termes[j];
            //        int count = 0;

            //        foreach (string w in words)
            //        {
            //            if (w == term)
            //            {
            //                count = 1;
            //            }
            //        }

            //        incidenceMatrix[j, i] = count;
            //    }
            //}

            //ViewBag.terms = incidenceMatrix;


            ////for (int i = 0; i < termes.Count; i++)
            ////{
            ////    Console.Write(termes[i] + ": ");
            ////    for (int j = 0; j < documents.Count; j++)
            ////    {
            ////        Console.Write(incidenceMatrix[i, j] + " ");
            ////    }
            ////    Console.WriteLine();
            ////}

            //// Search for a specific word

            //Dictionary<int , int> r = new Dictionary<int, int>();

            //for(int i = 1; i<=3; i++)
            //{
            //    r[i] = 0;
            //}

            //foreach (string word in searchtext)
            //{
            //    int wordIndex = termes.IndexOf(word);
            //    if (wordIndex != -1)
            //    {
            //        for (int i = 1; i <= 3; i++)
            //        {
            //            if (incidenceMatrix[wordIndex, i-1] > 0)
            //            {
            //                r[i]++;
            //            }
            //        }
            //    }
            //}

            //List<int> result = new List<int>();

            //if (operators == null)
            //{
            //    foreach (var i in r)
            //    {
            //        if (i.Value >= 1)
            //        {
            //            result.Add(i.Key);
            //        }
            //    }
            //}

            //if (operators == "and")
            //{
            //    foreach (var i in r)
            //    {
            //        if (i.Value >= 2)
            //        {
            //            result.Add(i.Key);
            //        }
            //    }
            //}
            //if (operators == "or")
            //{
            //    foreach (var i in r)
            //    {
            //        if (i.Value >= 1)
            //        {
            //            result.Add(i.Key);
            //        }
            //    }
            //}

            ViewBag.word = t;

            //ViewBag.result = result;


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
    List<string> documents = new List<string>();

    public List<string> StopWords(List<string> documents)
    {
        HashSet<string> stopWords = new HashSet<string> { "the", "and", "a", "of", "an", "as", "with", "at", "yes", "yet", "you" };
        HashSet<string> ReTerms = new HashSet<string>();

        foreach (var t in documents)
        {
            if (!stopWords.Contains(t) && !t.Contains("\n") && !t.Contains("\t") && t.Length > 2)
            {
                ReTerms.Add(t);
            }
        }

        return ReTerms.ToList();
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
                ReTerms+= t +" ";
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
                if (outputWord.Length > 2 )
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
            string outputWord = Regex.Replace(t, @"[\p{P}\d]", "");

            result += " " + t;
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

//class RemoveStopWords
//{

//    public List<string> GetTermsForOneDocument(string documents)
//    {
//        var terms = new HashSet<string>();

//        var documentTerms = documents.Split(' ');
//        foreach (var term in documentTerms)
//        {
//            if (term.Length > 0)
//            {
//                if (term[0] >= 'a' && term[0] <= 'z')
//                {
//                    terms.Add(term);
//                }
//            }

//        }

//        List<string> result = terms.ToList();
//        result.Sort();
//        return result;
//    }



//    public List<string> Files(string documentsPath)
//    {
//        List<string> documents = new List<string>();

//        documents.Clear();
//        foreach (string fileName in Directory.GetFiles(documentsPath, "*.*"))
//        {
//            documents.Add(File.ReadAllText(fileName).ToLower());
//        }
//        return documents.ToList();
//    }


//    public List<string> GetTerms(List<string> documents)
//    {
//        var terms = new HashSet<string>();

//        foreach (var document in documents)
//        {
//            var documentTerms = document.Split(' ');
//            foreach (var term in documentTerms)
//            {
//                terms.Add(term);
//            }
//        }


//        return terms.ToList();
//    }


//    public HashSet<string> NormalizeTerms(List<string> terms)
//    {
//        List<string> normalizedTerms = new List<string>();
//        HashSet<string> stopWords = new HashSet<string> {};

//        foreach (string term in terms)
//        {
//            // convert to lowercase and remove punctuation
//            string normalizedTerm = Regex.Replace(term.ToLower(), @"[\p{P}\p{S}]", "");

//            // remove stop words
//            if (!stopWords.Contains(normalizedTerm))
//            {
//                // apply stemming or lemmatization
//                term
//                // (omitted for simplicity in this example)
//                normalizedTerms.Add(normalizedTerm);
//            }
//        }

//        // remove digits
//        HashSet<string> filteredStrings = normalizedTerms.Where(s => !s.Any(char.IsDigit) && !s.Any(char.IsWhiteSpace)).ToHashSet();

//        return filteredStrings;
//    }

//}
