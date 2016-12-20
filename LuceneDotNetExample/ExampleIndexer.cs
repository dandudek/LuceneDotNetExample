using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Miscellaneous;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;

namespace LuceneDotNetExample
{
    public class ExampleIndexer : IDisposable
    {
        private readonly PerFieldAnalyzerWrapper analyzer;
        private readonly IndexWriter indexWriter;
        private readonly SearcherManager searcherManager;
        private readonly QueryParser queryParser;

        public ExampleIndexer(Directory indexDirectory)
        {
            analyzer = new PerFieldAnalyzerWrapper(new StandardAnalyzer(LuceneVersion.LUCENE_48));

            //analyzer = new PerFieldAnalyzerWrapper(new HtmlStripAnalyzerWrapper(new StandardAnalyzer(LuceneVersion.LUCENE_48)),
            //    new Dictionary<string, Analyzer>
            //    {
            //        {"owner", new LowercaseKeywordAnalyzer()},
            //        {"name", new RepositoryNamesAnalyzer()},
            //    });

            queryParser = new MultiFieldQueryParser(LuceneVersion.LUCENE_48,
                new[] { "name", "description" }, analyzer);


            indexWriter = new IndexWriter(indexDirectory, new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer));
            searcherManager = new SearcherManager(indexWriter, true);
        }

        public void Index()
        {
            //indexWriter.AddDocument(new Document
            //{
            //    new StringField("name", "foo", Field.Store.YES),
            //    new TextField("description", "value", Field.Store.NO)
            //});

            var doc = new Document
            {
                new StringField("name", "foo", Field.Store.YES),
                new TextField("description", "value", Field.Store.NO)
            };

            // Better to use update as it is open for reindexing
            indexWriter.UpdateDocument(new Term("name", "foo"), doc);

            indexWriter.Flush(true, true);
            indexWriter.Commit();
        }

        public List<SearchResult> Search(string queryString, out int totalHits)
        {
            var l = new List<SearchResult>();

            // Parse the query - assuming it's not a single term but an actual query string
            // Note the QueryParser used is using the same analyzer used for indexing
            var query = queryParser.Parse(queryString);

            var _totalHits = 0;

            // Execute the search with a fresh indexSearcher
            searcherManager.MaybeRefreshBlocking();
            searcherManager.ExecuteSearch(searcher =>
            {
                var topDocs = searcher.Search(query, 10);
                _totalHits = topDocs.TotalHits;
                foreach (var result in topDocs.ScoreDocs)
                {
                    var doc = searcher.Doc(result.Doc);
                    l.Add(new SearchResult
                    {
                        Name = doc.GetField("name")?.StringValue,
                    });
                }
            }, exception => { Console.WriteLine(exception.ToString()); });

            totalHits = _totalHits;
            return l;
        }

        public void Dispose()
        {
            indexWriter?.Dispose();
            searcherManager?.Dispose();
        }
    }
}
