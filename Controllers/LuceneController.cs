using IRProject.Models;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Directory = System.IO.Directory;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Analysis;
using sun.swing;
using TikaOnDotNet.TextExtraction;
using Microsoft.VisualBasic;
using Porter2Stemmer;
using System.Text.RegularExpressions;
using Porter2Stemmer;

using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Util;
using Lucene.Net.QueryParsers.Classic;
using System.IO;

namespace IRProject.Controllers
{
    public class LuceneController : Controller
    {

        public IActionResult Searching(List<string> searchtext , List<string> boolWords, string tok, string norm, string lemm, string stops, string stem)
        {

            string dirPath = "c:\\users\\saber\\onedrive - computer and information technology (menofia university)\\desktop\\ir\\irproject\\wwwroot\\attaches\\documents\\documents\\after";

            // Create index
            Lucene.Net.Store.Directory dir = FSDirectory.Open(dirPath);
            Lucene.Net.Analysis.Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            IndexWriterConfig iwc = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            IndexWriter iw = new IndexWriter(dir, iwc);

            // Add documents from directory
            foreach (string file in System.IO.Directory.GetFiles(dirPath, "*.txt*"))
            {
                Document doc = new Document();
                doc.Add(new TextField("content", System.IO.File.ReadAllText(file), Field.Store.YES));
                doc.Add(new TextField("filename", Path.GetFileName(file), Field.Store.YES));

                iw.AddDocument(doc);
            }

            iw.Commit();
            iw.Dispose();



            // Get query from user
            string query = searchtext.ToString();

            // Search 
            QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, "content", analyzer);
            Query q = parser.Parse(searchtext.FirstOrDefault());
            IndexReader reader = DirectoryReader.Open(dir);
            IndexSearcher searcher = new IndexSearcher(reader);
            TopDocs results = searcher.Search(q, 10);

            // Display results
            HashSet<string> keys = new HashSet<string>();
            foreach (ScoreDoc scoreDoc in results.ScoreDocs)
            {
                Document doc = searcher.Doc(scoreDoc.Doc);
                keys.Add(doc.Get("filename"));
            }

        
            reader.Dispose();
            dir.Dispose();

            ViewBag.result = keys;
            ViewBag.text = searchtext;

            return View("LuceneResult");

        }
        private string ExtractText(string file)
        {
            TextExtractor extractor = new TextExtractor();

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
    public List<string> StemOneDocument(string document)
    {

        List<string> result = new List<string>();


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

        result.Add(stemmedText.Trim());



        return result;
    }
}
