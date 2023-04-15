using Microsoft.AspNetCore.Mvc;

namespace IRProject.Controllers
{
    public class TermMatrixController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public List<List<int>> GenerateIncidenceMatrix(List<string> documents)
        {
            List<List<int>> incidenceMatrix = new List<List<int>>();
            List<string> vocabulary = new List<string>();

            foreach (var document in documents)
            {
                List<int> documentVector = new List<int>();
                string[] terms = document.Split(' '); // tokenize the document
                foreach (var term in terms)
                {
                    if (!vocabulary.Contains(term))
                    {
                        vocabulary.Add(term); // add new term to the vocabulary
                    }
                    int termIndex = vocabulary.IndexOf(term);
                    if (documentVector.Count <= termIndex)
                    {
                        documentVector.AddRange(new int[termIndex + 1 - documentVector.Count]);
                    }
                    documentVector[termIndex]++; // increment the term frequency
                }
                incidenceMatrix.Add(documentVector); // add the document vector to the matrix
            }
            return incidenceMatrix;
        }

        public IActionResult ShowMatrix()
        {
            List<string> documents = new List<string>();
            // fill the list of documents here

            string documentsPath = "C:\\Users\\saber\\OneDrive - Computer and Information Technology (Menofia University)\\Desktop\\IR\\IRProject\\wwwroot\\Attaches\\Documents\\Documents\\";

            foreach (string fileName in System.IO.Directory.GetFiles(documentsPath, "*.txt*"))
            {
                documents.Add(System.IO.File.ReadAllText(fileName));
            }

            List<List<int>> incidenceMatrix = GenerateIncidenceMatrix(documents);
            // do something with the incidence matrix
            ViewBag.matrix = incidenceMatrix;
            return View();
        }

    }
}
