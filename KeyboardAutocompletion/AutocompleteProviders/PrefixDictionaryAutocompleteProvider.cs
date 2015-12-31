using System;
using System.Collections.Generic;
using System.Linq;

namespace KeyboardAutocompletion.AutocompleteProviders
{
    [Serializable]
    public class PrefixDictionaryAutocompleteProvider : BaseAutocompleteProvider
    {
        private readonly Dictionary<string, Dictionary<string, int>> _prefixesToWordFrequencies = new Dictionary<string, Dictionary<string, int>>();

        public override IEnumerable<ICandidate> GetWords(string fragment)
        {
            if (!_prefixesToWordFrequencies.ContainsKey(fragment))
            {
                return Enumerable.Empty<ICandidate>();
            }
            else
            {
                var possibleWordsAndFrequencies = _prefixesToWordFrequencies[fragment];

                return possibleWordsAndFrequencies.Select(x => new Candidate(x.Key, x.Value))
                                                  .OrderByDescending(x => x.Confidence)
                                                  .ThenBy(x => x.Word);
            }
        }

        public override void Train(string passage)
        {
            var words = TokenizeWords(passage);

            foreach (var word in words)
            {
                var wordPrefixes = GetAllPrefixes(word);

                foreach (var wordPrefix in wordPrefixes)
                {
                    if (!_prefixesToWordFrequencies.ContainsKey(wordPrefix))
                    {
                        _prefixesToWordFrequencies[wordPrefix] = new Dictionary<string, int>();
                    }

                    if (!_prefixesToWordFrequencies[wordPrefix].ContainsKey(word))
                    {
                        _prefixesToWordFrequencies[wordPrefix][word] = 1;
                    }
                    else
                    {
                        _prefixesToWordFrequencies[wordPrefix][word]++;
                    }
                }
            }
        }

        private IEnumerable<string> GetAllPrefixes(string s)
        {
            for (int i = 1; i < s.Length; i++)
            {
                yield return s.Substring(0, i);
            }
        }
    }
}