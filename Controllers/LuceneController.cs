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

namespace IRProject.Controllers
{
    public class LuceneController : Controller
    {

        public IActionResult Searching(string searchtext , List<string> boolWords, string tok, string norm, string lemm, string stops, string stem)
        {

            List<string> searchterms = new List<string>();
            string operators = "";

            foreach (string word in searchtext.Split(' '))
            {
                word.ToLower();
                if(word == "and" || word == "or" || word == "not") operators = word;
                else searchterms.Add(word);
            }

            string dirPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\lucene";

            Operations op = new Operations();

            RemoveStopWords stopWords= new RemoveStopWords();

            // Create index
            Lucene.Net.Store.Directory dir = FSDirectory.Open(dirPath);
            Lucene.Net.Analysis.Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            IndexWriterConfig iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            IndexWriter iw = new IndexWriter(dir, iwc);

            // Add documents from directory
            foreach (string file in System.IO.Directory.GetFiles(dirPath, "CISI*"))
            {
                Document doc = new Document();
                string text = System.IO.File.ReadAllText(file);
                string output = text;

                //if(tok == "on")
                //{
                //    output = stopWords.GetTermsForOneDocument(output);
                //}
                //if(norm == "on")
                //{
                //    output = stopWords.NormalizeOneDocument(output);
                //}
                //if(stem == "on")
                //{
                //    output =  op.StemOneDocument(output);
                //}
                //if (stops == "on")
                //{
                //    output = stopWords.StopWordsOneDocumentLucene(output);
                //}
                doc.Add(new TextField("content", text, Field.Store.YES));
                doc.Add(new TextField("filename", Path.GetFileName(file), Field.Store.YES));

                iw.AddDocument(doc);
            }

            iw.Commit();
            iw.Dispose();

            Dictionary<string , int> re = new Dictionary<string, int>();

            foreach(var i in System.IO.Directory.GetFiles(dirPath, "CISI*"))
            {
                re.Add(Path.GetFileName(i) , 0);
            }

            foreach (var st in searchterms)
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

            if(operators == "")
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


            ViewBag.text = searchtext;

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
