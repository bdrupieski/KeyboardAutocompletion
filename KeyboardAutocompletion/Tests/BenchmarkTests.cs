using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using KeyboardAutocompletion.AutocompleteProviders;
using KeyboardAutocompletion.Tries;
using NUnit.Framework;

namespace KeyboardAutocompletion.Tests
{
    public class BenchmarkTests
    {
        [Test]
        public void BenchmarkAutocompleteProviders()
        {
            BenchmarkAutocompleteProvider(new PrefixDictionaryAutocompleteProvider());
            BenchmarkAutocompleteProvider(new TrieAutocompleteProvider<DictionaryTrie>());
            BenchmarkAutocompleteProvider(new TrieAutocompleteProvider<ArrayTrie>());
            BenchmarkAutocompleteProvider(new TrieAutocompleteProvider<ListTrie>());
        }

        private void BenchmarkAutocompleteProvider(IAutocompleteProvider provider)
        {
            var warAndPeacePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "war_and_peace.txt");
            if (!File.Exists(warAndPeacePath))
                return;

            Console.WriteLine("Benchmarking {0}", provider.GetType().GetFriendlyName());

            var passage = File.ReadAllText(warAndPeacePath);
            var prefixInput = "th";

            var stopwatch = Stopwatch.StartNew();
            provider.Train(passage);

            stopwatch.Stop();
            Console.WriteLine("{0} ms to train war_and_peace.txt", stopwatch.ElapsedMilliseconds);

            stopwatch = Stopwatch.StartNew();
            int n = 1000;
            for (int i = 0; i < n; i++)
            {
                var candidates = provider.GetWords(prefixInput).ToList();
            }
            stopwatch.Stop();
            Console.WriteLine("{0} ms to get autocompletions {1} times for '{2}'.", stopwatch.ElapsedMilliseconds, n, prefixInput);
            long bytesCount = VeryRoughEstimateOfMemoryConsumption(provider);
            Console.WriteLine("Trained provider uses roughly {0} KB.", bytesCount / 1024);
            Console.WriteLine();
        }

        private long VeryRoughEstimateOfMemoryConsumption(object o)
        {
            using (Stream s = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(s, o);
                return s.Length;
            }
        }
    }
}