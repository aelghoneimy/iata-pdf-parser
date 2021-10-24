using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using System.Text.RegularExpressions;

namespace IataPdfParser.Parser
{
    public static class IataParser
    {
        private static readonly Regex _headerRegex = new(@"\*{3} (.+)");
        private static readonly Regex _airRegex = new("^AIR");


        private static readonly Regex _dateRegex = new("^\\d{2}(JAN|FEB|MAR|APR|MAY|JUN|JUL|AUG|SEP|OCT|NOV|DEC)\\d{2}$");

        public static IataDocument Parse(string fileName)
        {
            var pdfReader = new PdfReader(fileName);
            var pdfDocument = new PdfDocument(pdfReader);
            var rows = new List<IataRow>();
            IataSection? section = null;

            var numberOfPages = pdfDocument.GetNumberOfPages();

            for (var pageNum = 1; pageNum <= numberOfPages; pageNum++)
            {
                var page = pdfDocument.GetPage(pageNum);
                var pageContentRaw = PdfTextExtractor.GetTextFromPage(page);
                var pageContent = pageContentRaw.Split('\n');

                var startIndex = 0;

                while (true)
                {
                    // Search for header
                    if (section == null && (section = SearchForHeader(pageContent, startIndex)) != null)
                    {
                        section.HeaderPage = pageNum;
                        startIndex = section.HeaderIndex + 1;
                    }

                    // Search for footer
                    if (section != null && (section.FooterIndex = SearchForFooter(pageContent, section.Name, startIndex)) > -1)
                    {
                        startIndex = section.FooterIndex + 1;
                    }

                    if (section == null)
                    {
                        break;
                    }

                    var start = Array.FindIndex(pageContent, 0, x => _airRegex.IsMatch(x)) + 1;
                    start = pageNum == section.HeaderPage ? Math.Max(start, section.HeaderIndex + 1) : start;

                    var end = section.FooterIndex > -1 ? section.FooterIndex : pageContent.Length - 1;
                    var searchableContent = pageContent.Skip(start).Take(end - start + 1).ToList();

                    rows.AddRange(GetRows(searchableContent));

                    if (end == pageContent.Length - 1)
                    {
                        break;
                    }

                    section = null;
                }
            }

            return new IataDocument(rows);
        }

        private static IataSection? SearchForHeader(string[] content, int startIndex = 0)
        {
            var index = Array.FindIndex(content, startIndex, x => _headerRegex.IsMatch(x));

            if (index == -1)
            {
                return null;
            }

            return new IataSection
            {
                HeaderIndex = index,
                Name = _headerRegex.Matches(content[index])[0].Groups[1].Value
            };
        }

        private static int SearchForFooter(string[] content, string name, int startIndex)
        {
            var regex = new Regex($"^{name} TOTAL$");

            return Array.FindIndex(content, startIndex, x => regex.IsMatch(x)) - 2;
        }

        private static IEnumerable<IataRow> GetRows(List<string> searchableContent)
        {
            var result = new List<IataRow>();

            foreach (var searchableRow in searchableContent)
            {
                var values = searchableRow.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                var row = new IataRow();

                if (values[0].StartsWith("+"))
                {
                    // + Row
                    row.Trnc = $"'{values[0]}'";

                    if (values.Length > 1)
                    {
                        row.DocumentNumber = values[1];
                    }

                    if (values.Length > 2 && _dateRegex.IsMatch(values[2]))
                    {
                        row.Date = values[2];
                    }
                }
                else if (values.Length > 5)
                {
                    // normal row
                    row.Air = values[0];
                    row.Trnc = $"'{values[1]}'";
                    row.DocumentNumber = values[2];
                    row.Date = values[3];
                    row.Value = values.Last();
                }
                else
                {
                    continue;
                }

                result.Add(row);
            }

            return result;
        }
    }
}
