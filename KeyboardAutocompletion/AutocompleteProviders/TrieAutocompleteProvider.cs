using System;
using System.Collections.Generic;
using System.Linq;
using KeyboardAutocompletion.Tries;

namespace KeyboardAutocompletion.AutocompleteProviders
{
    [Serializable]
    public class TrieAutocompleteProvider<T> : BaseAutocompleteProvider where T : ITrie, new()
    {
        private readonly T _rootTrie = new T();

        public override IEnumerable<ICandidate> GetWords(string fragment)
        {
            return _rootTrie.CollectCandidatesAtPrefix(fragment)
                            .OrderByDescending(x => x.Confidence)
                            .ThenBy(x => x.Word);
        }

        public override void Train(string passage)
        {
            var words = TokenizeWords(passage);
            _rootTrie.TrainTrie(words);
        }
    }
}