using Lucene.Net.Analysis;
using Lucene.Net.Analysis.CharFilters;
using System.IO;

namespace LuceneDotNetExample
{
    class HtmlStripAnalyzerWrapper : Analyzer
    {
        private readonly Analyzer _wrappedAnalyzer;

        public HtmlStripAnalyzerWrapper(Analyzer wrappedAnalyzer)
        {
            _wrappedAnalyzer = wrappedAnalyzer;
        }

        public override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {
            return _wrappedAnalyzer.CreateComponents(fieldName, new HTMLStripCharFilter(reader));
        }
    }
}
