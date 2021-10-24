using IataPdfParser.Output.Text;
using IataPdfParser.Parser;
using System.Text;

namespace IataToExcel.Console
{

    class Program
    {
        static void Main(string[] args)
        {
            var inputDirectory = new DirectoryInfo("./input");
            var outputDirectory= new DirectoryInfo("./output");
            
            if (!Directory.Exists(inputDirectory.FullName))
            {
                return;
            }

            if (!outputDirectory.Exists)
            {
                Directory.CreateDirectory(outputDirectory.FullName);
            }
            
            foreach (var s in inputDirectory.EnumerateFiles("*.pdf", SearchOption.AllDirectories))
            {
                if (string.IsNullOrEmpty(s.DirectoryName))
                {
                    continue;
                }

                var output = IataParser.Parse(s.FullName).AsString();

                var outputFilePath = s.DirectoryName.Replace(inputDirectory.FullName, outputDirectory.FullName);
                var fileName = s.Name.Replace(s.Extension, ".txt");

                if (!Directory.Exists(outputFilePath))
                {
                    Directory.CreateDirectory(outputFilePath);
                }
                
                File.WriteAllText($"{outputFilePath}\\{fileName}", output, Encoding.UTF8);
            }
        }


    }
}
