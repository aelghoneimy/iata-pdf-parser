namespace IataPdfParser.Parser
{
    public class IataSection
    {
        public int HeaderIndex { get; set; }
        
        public int HeaderPage { get; set; }
        
        public string Name { get; set; }
        
        public int FooterIndex { get; set; } = -1;
    }
}
