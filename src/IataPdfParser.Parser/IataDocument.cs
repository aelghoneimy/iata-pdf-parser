namespace IataPdfParser.Parser
{
    public class IataDocument
    {
        public ICollection<IataRow> Rows { get; } = Array.Empty<IataRow>();

        public IataDocument(ICollection<IataRow> rows)
        {
            Rows = rows;
        }
    }
}
