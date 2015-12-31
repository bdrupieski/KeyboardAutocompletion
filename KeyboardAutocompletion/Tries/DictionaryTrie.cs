using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyboardAutocompletion.Tries
{
    [Serializable]
    public class DictionaryTrie : ITrie
    {
        private readonly Dictionary<char, DictionaryTrie> _subTries = new Dictionary<char, DictionaryTrie>();
        private bool _isTerminatedWord = false;
        private int _wordOccurenceCount = 0;

        public void TrainTrie(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                DictionaryTrie currentTrie = this;

                foreach (char c in word)
                {
                    if (!currentTrie._subTries.ContainsKey(c))
                    {
                        currentTrie._subTries[c] = new DictionaryTrie();
                    }

                    currentTrie = currentTrie._subTries[c];
                }

                currentTrie._isTerminatedWord = true;
                currentTrie._wordOccurenceCount++;
            }
        }

        public IEnumerable<ICandidate> CollectCandidatesAtPrefix(string prefix)
        {
            var trieForPrefix = FindLastSubTrieIfExists(prefix);

            if (trieForPrefix == null)
            {
                return Enumerable.Empty<ICandidate>();
            }
            else
            {
                return CollectCandidatesAtTrie(prefix, trieForPrefix);
            }
        }

        private DictionaryTrie FindLastSubTrieIfExists(string word)
        {
            DictionaryTrie currentTrie = this;

            foreach (var c in word)
            {
                if (!currentTrie._subTries.TryGetValue(c, out currentTrie))
                {
                    return null;
                }
            }

            return currentTrie;
        }

        private IEnumerable<ICandidate> CollectCandidatesAtTrie(string prefix, DictionaryTrie trie)
        {
            foreach (var charAndSubTrie in trie._subTries)
            {
                var character = charAndSubTrie.Key;
                var subTrie = charAndSubTrie.Value;

                if (subTrie._isTerminatedWord)
                {
                    yield return new Candidate(prefix + character, subTrie._wordOccurenceCount);
                }
                
                foreach (var candidate in CollectCandidatesAtTrie(prefix + character, subTrie))
                {
                    yield return candidate;
                }
            }
        }

        public override string ToString()
        {
            return ToString(0);
        }

        private string ToString(int level)
        {
            if (_subTries == null)
            {
                return "";
            }

            level++;

            var sb = new StringBuilder();

            foreach (char c in _subTries.Keys)
            {
                sb.Append("{ " + level + " " + c + " : " + _subTries[c].ToString(level) + "}");
            }

            return sb.ToString();
        }
    }
}