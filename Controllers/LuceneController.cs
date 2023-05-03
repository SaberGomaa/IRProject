using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Microsoft.AspNetCore.Mvc;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.QueryParsers.Classic;
using TikaOnDotNet.TextExtraction;
using Porter2Stemmer;
using org.apache.poi.ss.formula.functions;

namespace IRProject.Controllers
{
    public class LuceneController : Controller
    {

        public IActionResult Searching(string t, List<string> searchtext , string boolWords, string tok, string norm, string lemm, string stops, string stem)
        {

            allPre a = new allPre();
            var dict = a.Indexing("lucene", tok, norm, lemm, stops, stem);

            List<string> searchterms = new List<string>();
            string operators = boolWords;

            indexingQRY indexingQRY = new indexingQRY();

            string dirPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\lucene";

            Operations op = new Operations();

            RemoveStopWords stopWords= new RemoveStopWords();

            // Create index
            Lucene.Net.Store.Directory dir = FSDirectory.Open(dirPath);
            Lucene.Net.Analysis.Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            IndexWriterConfig iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            IndexWriter iw = new IndexWriter(dir, iwc);

            foreach (var file in dict)
            {
                Document doc = new Document();
              
                doc.Add(new TextField("content", file.Value, Field.Store.YES));
                doc.Add(new TextField("filename",file.Key.ToString(), Field.Store.YES));

                iw.AddDocument(doc);
            }

            iw.Commit();
            iw.Dispose();

            Dictionary<string , int> re = new Dictionary<string, int>();

            foreach(var i in dict)
            {
                re.Add(i.Key.ToString() , 0);
            }

            foreach (var st in searchtext)
            {
                // Search 
                QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, "content", analyzer);

                Query q = parser.Parse(st);
                IndexReader reader = DirectoryReader.Open(dir);
                IndexSearcher searcher = new IndexSearcher(reader);
                TopDocs results = searcher.Search(q, 10);
                HashSet<string> keys = new HashSet<string>();

                // Display results
                foreach (ScoreDoc scoreDoc in results.ScoreDocs)
                {
                    Document doc = searcher.Doc(scoreDoc.Doc);
                    keys.Add(doc.Get("filename"));
                }

                foreach (var i in keys)
                {
                    re[i]++;
                }

            }



            List<string > result = new List<string>();

            if(operators == null)
            {
                foreach (var i in re)
                {
                    if (i.Value >= 1)
                    {
                        result.Add(i.Key);
                    }
                }
            }

            if(operators == "and")
            {
                foreach(var i in re)
                {
                    if(i.Value >= 2)
                    {
                        result.Add(i.Key);
                    }
                }
            }
            if (operators == "or")
            {
                foreach (var i in re)
                {
                    if (i.Value >= 1)
                    {
                        result.Add(i.Key);
                    }
                }
            }

            List<string> operations = new List<string>();
            if (tok == "on") operations.Add("Tokenization");
            if (norm == "on") operations.Add("Normalization");
            if (stops == "on") operations.Add("Remove Stop Words");
            if (lemm == "on") operations.Add("Lemmetization");
            if (stem == "on") operations.Add("Stemming");

            ViewBag.operations = operations;

            dir.Dispose();

            ViewBag.result = result;


            ViewBag.text = t;

            return View("LuceneResult");

        }
        private string ExtractText(string file)
        {

            string r = System.IO.File.ReadAllText(file);

            return r;

        }


    }
}

class Operations
{
    public List<string> StemDocuments(List<string> documents)
    {

        List<string> result = new List<string>();

        for (int i = 0; i < documents.Count; i++)
        {

            string text = documents[i];
            var stemmer = new EnglishPorter2Stemmer();
            string[] words = text.Split(' ');
            string stemmedText = string.Empty;

            foreach (string word in words)
            {
                word.ToLower();
                if (word.Length > 0 && word.Length < 15 && word[0] >= 'a' && word[0] <= 'z')
                {
                    stemmedText += stemmer.Stem(word).Value + " ";
                }
            }

            result.Add(stemmedText.Trim());

        }

        return result;
    }
    public string StemOneDocument(string document)
    {


        string text = document;
        var stemmer = new EnglishPorter2Stemmer();
        string[] words = text.Split(' ');
        string stemmedText = string.Empty;

        foreach (string word in words)
        {
            word.ToLower();
            if (word.Length > 0 && word.Length < 15 && word[0] >= 'a' && word[0] <= 'z')
            {
                stemmedText += stemmer.Stem(word).Value + " ";
            }
        }

        string result = stemmedText;

        return result;
    }
}
class indexingQRY
{
    public Dictionary<int, string> docs()
    {
        string dirPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\section\\CISI.QRY";

        string text = File.ReadAllText(dirPath);

        string[] files = new string[] { };

        files = text.Split(".I");

        List<int> l = new List<int>();

        Dictionary<int, string> dict = new Dictionary<int, string>();
        int c = 0;

        foreach (string file in files)
        {
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
        return dict;
    }
}

