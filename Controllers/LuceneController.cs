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

namespace IRProject.Controllers
{
    public class LuceneController : Controller
    {

        public IActionResult Searching(List<string> searchtext , List<string> boolWords)
        {

            string documentsPath = "F:\\L4  S Semester\\Projects\\IR\\wwwroot\\Attaches\\Documents\\Documents\\Section\\";

            var directory = FSDirectory.Open("F:\\L4  S Semester\\Projects\\IR\\wwwroot\\Attaches\\Documents\\Documents\\Lucene\\");


            StandardAnalyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            IndexWriterConfig config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);
            IndexWriter writer = new IndexWriter(directory, config);

            // Index all text files in documents directory
            foreach (string file in Directory.GetFiles(documentsPath, "*.*"))
            {

                string text = ExtractText(file);
                Document doc = new Document();
                doc.Add(new TextField("filename", Path.GetFileName(file), Field.Store.YES));
                doc.Add(new TextField("content", text, Field.Store.YES));
                writer.AddDocument(doc);
            }

            writer.Commit();
            writer.Dispose();

            // Create index searcher
            IndexReader reader = DirectoryReader.Open(directory);
            IndexSearcher searcher = new IndexSearcher(reader);

            // Create search query
            StandardAnalyzer queryAnalyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);
            QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, "content", queryAnalyzer);


            Query query = parser.Parse(searchtext.FirstOrDefault());

            HashSet<string> list = new HashSet<string>();

            // Execute search and display results
            TopDocs results = searcher.Search(query, 10);
            //Console.WriteLine("Found {0} documents matching the query '{1}':", results.TotalHits, query.ToString());
            foreach (ScoreDoc scoreDoc in results.ScoreDocs)
            {
                Document doc = searcher.Doc(scoreDoc.Doc);
                //Console.WriteLine(" - {0} ({1})", doc.Get("filename"), scoreDoc.Score);
                list.Add(doc.Get("filename"));
            }

            // Close index reader
            reader.Dispose();

            ViewBag.result = list;
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

