using Microsoft.AspNetCore.Mvc;
using org.apache.xmlbeans.impl.xb.xsdschema;

namespace IRProject.Controllers
{
    public class PositionalIndexController : Controller
    {
        Dictionary<string, List<(int DocumentId, int WordIndex)>> positionalIndex = new Dictionary<string, List<(int, int)>>();

        public IActionResult PositionalResults() 
		{
            List<string> documents = new List<string>();

            indexingQRY ind = new indexingQRY();

			Preprocessing  p = new Preprocessing();

            var r = ind.docs();

            foreach (var doc in r)
            {
                documents.Add(p.StopWords(p.NormalizeOneDocument(doc.Value)).ToLower());
            }

            for (int i = 0; i < documents.Count; i++)
            {
                string[] words = documents[i].Split(' ');

                for (int j = 0; j < words.Length; j++)
                {
                    string word = words[j];

                    if (!positionalIndex.ContainsKey(word))
                    {
                        positionalIndex[word] = new List<(int, int)>();
                    }

                    positionalIndex[word].Add((i, j));
                }
            }

			ViewBag.indexes = positionalIndex;

            string searchWord = "approximate";
            HashSet<int> docsIDs = new HashSet<int>();
            if (positionalIndex.ContainsKey(searchWord))
            {
                Console.WriteLine($"Word '{searchWord}' appears in the following documents:");

                List<(int docID, int index)> documentIds = positionalIndex[searchWord];

                foreach (var documentId in documentIds)
                {
                    docsIDs.Add(documentId.docID);
                }
            }

            ViewBag.docs = docsIDs;

			return View();

        }

        public IActionResult Index(string t, List<string> searchtext, string boolWords, string tok, string norm, string lemm, string stops, string stem)
        {
            List<string> operations = new List<string>();
            if (tok == "on") operations.Add("Tokenization");
            if (norm == "on") operations.Add("Normalization");
            if (stops == "on") operations.Add("Remove Stop Words");
            if (lemm == "on") operations.Add("Lemmetization");
            if (stem == "on") operations.Add("Stemming");

            norm= "on";
            stem= "on";

            ViewBag.operations = operations;
            indexingQRY indexingQRY = new indexingQRY();
			stops = "on";
            allPre a = new allPre();
            var dict = a.Indexing("lucene", tok, norm, lemm, stops, stem);

			PositionalIndex index = new PositionalIndex();
			var x = index.index;

			Preprocessing preprocessing = new Preprocessing();

			if(stem == "on")
			{
				t = preprocessing.StemmingOneDocument(t);
			}

			foreach (var o in dict)
			{
				string [] ss = o.Value.Split(' ');
				index.AddDocument(o.Key+1, ss);
			}

			string[] s = t.Split(' ');

            //string[] s = new string[2];

            //for (int i = 0; i < searchtext.Count; i++)
            //{
            //	s[i] = searchtext[i];
            //}

            List<int> results = new List<int>();


            if (t.Length > 4)
			{
				results = index.Search(s);
			}
			else
			{
                results = new List<int> ();
            }

			ViewBag.pos = x; 

			ViewBag.result = results;
            ViewBag.word = t;

            return View();
        }
    }
}




public class PositionalIndex
{
	public Dictionary<string, Dictionary<int, List<int>>> index;

	public PositionalIndex()
	{
		index = new Dictionary<string, Dictionary<int, List<int>>>();
	}

	public void AddDocument(int docId, string[] terms)
	{
		foreach (string term in terms)
		{
			if (!index.ContainsKey(term))
			{
				index[term] = new Dictionary<int, List<int>>();
			}

			if (!index[term].ContainsKey(docId))
			{
				index[term][docId] = new List<int>();
			}

			index[term][docId].Add(Array.IndexOf(terms, term));
		}
	}
	public List<int> Search(string[] terms)
	{
		List<int> results = new List<int>();

		if (terms.Length == 0)
		{
			return results;
		}

		// Find the documents that contain the first term
		string firstTerm = terms[0];
		if (index.ContainsKey(firstTerm))
		{
			foreach (int docId in index[firstTerm].Keys)
			{
				List<int> positions = index[firstTerm][docId];

				// Check if the document also contains the other terms in sequence
				bool containsTerms = true;
				int lastPosition = -1;
				for (int i = 1; i < terms.Length; i++)
				{
					string term = terms[i];
					if (!index.ContainsKey(term) || !index[term].ContainsKey(docId))
					{
						containsTerms = false;
						break;
					}

					List<int> termPositions = index[term][docId];
					int nextPosition = termPositions.FirstOrDefault(p => p > lastPosition);

					if (nextPosition == -1)
					{
						containsTerms = false;
						break;
					}

					lastPosition = nextPosition;
				}

				if (containsTerms)
				{
					results.Add(docId);
				}
			}
		}

		return results;
	}
}


//class PositionalIndex
//{
//    private readonly Dictionary<string, Dictionary<int, List<int>>> index = new Dictionary<string, Dictionary<int, List<int>>>();

//    public void AddDocument(int docNum, string document)
//    {
//        var words = document.Split(' ');
//        for (int i = 0; i < words.Length; i++)
//        {
//            var word = words[i];
//            if (!index.ContainsKey(word))
//            {
//                index[word] = new Dictionary<int, List<int>>();
//            }
//            if (!index[word].ContainsKey(docNum))
//            {
//                index[word][docNum] = new List<int>();
//            }
//            index[word][docNum].Add(i);
//        }
//    }

//    public List<int> Search(string [] query)
//    {
//        var docNums = new HashSet<int>();
//        foreach (var word in query)
//        {
//            if (index.ContainsKey(word))
//            {
//                var postings = index[word];
//                if (docNums.Count == 0)
//                {
//                    docNums.UnionWith(postings.Keys);
//                }
//                else
//                {
//                    docNums.IntersectWith(postings.Keys);
//                }
//            }
//            else
//            {
//                return new List<int>();
//            }
//        }
//        var result = new List<int>();
//        foreach (var docNum in docNums)
//        {
//            var positions = index[query[0]][docNum];
//            for (int i = 1; i < query.Length; i++)
//            {
//                positions = Intersect(positions, index[query[i]][docNum]);
//            }
//            if (positions.Count > 0)
//            {
//                result.Add(docNum);
//            }
//        }
//        return result;
//    }

//    private List<int> Intersect(List<int> a, List<int> b)
//    {
//        var result = new List<int>();
//        int i = 0, j = 0;
//        while (i < a.Count && j < b.Count)
//        {
//            if (a[i] == b[j] - 1)
//            {
//                result.Add(b[j]);
//                i++;
//                j++;
//            }
//            else if (a[i] < b[j] - 1)
//            {
//                i++;
//            }
//            else
//            {
//                j++;
//            }
//        }
//        return result;
//    }
//}
