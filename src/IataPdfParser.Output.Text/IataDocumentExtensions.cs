using IataPdfParser.Parser;
using System.Text;

namespace IataPdfParser.Output.Text
{
    public static class IataDocumentExtensions
    {
        public static string AsString(this IataDocument document)
        {
            var stringBuilder = new StringBuilder();

            foreach (var row in document.Rows)
            {
                stringBuilder.AppendLine($"{row.Air}\t" +
                                         $"{row.Trnc}\t" +
                                         $"{row.DocumentNumber}\t" +
                                         $"{row.Date}\t" +
                                         $"{row.Value}");
            }

            return stringBuilder.ToString();
        }
    }
}
