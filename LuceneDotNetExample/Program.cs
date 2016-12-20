using Lucene.Net.Store;
using System;
using System.IO;

namespace LuceneDotNetExample
{
    class Program
    {
        static void Main()
        {
            using (var indexDirectory = FSDirectory.Open(new DirectoryInfo(@"C:\IndexExample")))
            using (var ei = new ExampleIndexer(indexDirectory))
            {
                Console.WriteLine("Indexing...");
                ei.Index();
                Console.WriteLine("Indexed");

                Console.WriteLine("Searching for value...");
                var totalHits = 0;
                var results = ei.Search("value", out totalHits);
                Console.WriteLine($"Total hits: {totalHits}");
                foreach(var result in results)
                {
                    Console.WriteLine(result.Name);
                }

                Console.ReadKey();
            }
        }
    }
}
