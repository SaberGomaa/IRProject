using com.sun.corba.se.spi.orbutil.fsm;
using javax.swing.text;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.Collections.Generic;

public class InputData
{
    [LoadColumn(0)]
    public string Text { get; set; }
}

public class TokenizedData
{
    [LoadColumn(0)]
    public string[] Tokens { get; set; }
}

public class TokenizationController : Controller
{
    private readonly MLContext _mlContext;

    public TokenizationController()
    {
        _mlContext = new MLContext();
    }

    //public IActionResult Tokenize(string filePath)
    //{
    //    // Define the pipeline for tokenization
    //    var pipeline = _mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text");

    //    // Load the input data from the file
    //    var textLoader = _mlContext.Data.CreateTextLoader(new TextLoader.Options
    //    {
    //        Separators = new[] { '\t' },
    //        HasHeader = true,
    //        Columns = new[]
    //        {
    //            new TextLoader.Column("Text", DataKind.String, 0)
    //        }
    //    });

    //    filePath = "F:\\L4  S Semester\\IR\\Projects\\Project\\IRProject\\wwwroot\\Attaches\\Documents\\Documents\\t1.txt";

    //    var dataView = textLoader.Load(filePath);

    //    // Apply the pipeline to the input data and create the tokenized data
    //    var tokenizedData = pipeline.Fit(dataView).Transform(dataView);

    //    // Convert the tokenized data to a list of TokenizedData
    //    var result = _mlContext.Data.CreateEnumerable<TokenizedData>(tokenizedData, reuseRowObject: false).ToList();

    //    ViewBag.tokenz = result;

    //    return View();
    //}


    //public List<string> Tokenize()
    //{

    //    string documentsPath = "F:\\L4  S Semester\\IR\\Projects\\Project\\IRProject\\wwwroot\\Attaches\\Documents\\Documents\\";

    //    List<InputData> input = new List<InputData> 
    //    {
    //        new InputData { Text =""} ,
    //    }; 

    //    foreach (string file in System.IO.Directory.GetFiles(documentsPath, "*.txt*"))
    //    {
    //        InputData newData = new InputData();

    //        // Set the properties of the new InputData object
    //        newData.Text =System.IO.File.ReadAllText(file) ;

    //        // Add the new InputData object to the list
    //        input.Add(newData);
    //    }


    //    // Define the pipeline for tokenization
    //    var pipeline = _mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text");

    //    // Fit the pipeline to the input data
    //    var dataView = _mlContext.Data.LoadFromEnumerable(input);
    //    var tokenizedData = pipeline.Fit(dataView).Transform(dataView);

    //    // Convert the dataset to a list of TokenizedData
    //    List<TokenizedData> result = _mlContext.Data.CreateEnumerable<TokenizedData>(tokenizedData, reuseRowObject: false).ToList();

    //    HashSet<string> list = new HashSet<string>();

    //    foreach (var i in result)
    //    {
    //        if (i.Tokens == null) continue;
    //        foreach (var t in i.Tokens)
    //        {
    //            list.Add(t.ToLower());
    //        }
    //    }
    //    List<string> l = list.ToList();
    //    l.Sort();
    //    return l;
    //}

    public IActionResult Tokenize()
    {

        string documentsPath = "F:\\L4  S Semester\\Projects\\IR\\wwwroot\\Attaches\\Documents\\Documents\\";

        List<string> DocNames = new List<string>();   

        foreach(string fileName in System.IO.Directory.GetFiles(documentsPath, "*.txt*")){
            DocNames.Add(fileName);
        }

        ViewBag.docNames = DocNames;

        List<InputData> input = new List<InputData>
        {
            new InputData { Text =""} ,
        };

        foreach (string file in System.IO.Directory.GetFiles(documentsPath, "*.txt*"))
        {
            InputData newData = new InputData();

            // Set the properties of the new InputData object
            newData.Text = System.IO.File.ReadAllText(file);

            // Add the new InputData object to the list
            input.Add(newData);
        }


        // Define the pipeline for tokenization
        var pipeline = _mlContext.Transforms.Text.TokenizeIntoWords("Tokens", "Text");

        // Fit the pipeline to the input data
        var dataView = _mlContext.Data.LoadFromEnumerable(input);
        var tokenizedData = pipeline.Fit(dataView).Transform(dataView);

        // Convert the dataset to a list of TokenizedData
        List<TokenizedData> result = _mlContext.Data.CreateEnumerable<TokenizedData>(tokenizedData, reuseRowObject: false).ToList();

        List<string> list = new List<string>();

        foreach (var i in result)
        {
            if (i.Tokens == null) continue;
            foreach (var t in i.Tokens)
            {
                if (t[0] >= 'a' && t[0] <='z')
                    list.Add(t.ToLower());
            }
        }
        List<string> l = list.ToList();
        l.Sort();

        ViewBag.tokenz = l;

        return View();
    }
}

