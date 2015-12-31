using System;
using System.Collections.Generic;
using System.Linq;
using KeyboardAutocompletion.AutocompleteProviders;
using KeyboardAutocompletion.Tries;
using NUnit.Framework;

namespace KeyboardAutocompletion.Tests
{
    public class AutocompleteProviderTests
    {
        [Test]
        public void TestSuggestions()
        {
            var providers = new IAutocompleteProvider[]
            {
                new PrefixDictionaryAutocompleteProvider(),
                new TrieAutocompleteProvider<DictionaryTrie>(),
                new TrieAutocompleteProvider<ArrayTrie>(),
                new TrieAutocompleteProvider<ListTrie>(),
            };

            var suggestionGroups = providers.Select(GetBasicSuggestions);

            foreach (var suggestionGroup in suggestionGroups)
            {
                var suggestionLine = string.Join(", ", suggestionGroup.Select(x => $"{x.Word} ({x.Confidence})"));
                Console.WriteLine(suggestionLine);
            }
        }

        private IEnumerable<ICandidate> GetBasicSuggestions(IAutocompleteProvider provider)
        {
            return GetSuggestions(provider, "The thing that I was thinking that was third in the thingamagij is a thinking cap.");
        }

        private IEnumerable<ICandidate> GetSuggestions(IAutocompleteProvider provider, string passage)
        {
            provider.Train(passage);
            return provider.GetWords("th");
        }
    }
}