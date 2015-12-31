using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyboardAutocompletion.Tries
{
    [Serializable]
    public class ListTrie : ITrie
    {
        private readonly List<ListTrie> _subTries = new List<ListTrie>();
        private char _character;
        private bool _isTerminatedWord = false;
        private int _wordOccurenceCount = 0;

        public void TrainTrie(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                ListTrie currentTrie = this;

                foreach (char c in word)
                {
                    var sub = currentTrie._subTries.FirstOrDefault(x => x._character == c);
                    if (sub == null)
                    {
                        sub = new ListTrie();
                        sub._character = c;
                        currentTrie._subTries.Add(sub);
                    }

                    currentTrie = sub;
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

        private ListTrie FindLastSubTrieIfExists(string word)
        {
            ListTrie currentTrie = this;

            foreach (var c in word)
            {
                var sub = currentTrie._subTries.FirstOrDefault(x => x._character == c);
                if (sub != null)
                {
                    currentTrie = sub;
                }
                else
                {
                    return null;
                }
            }

            return currentTrie;
        }

        private IEnumerable<ICandidate> CollectCandidatesAtTrie(string prefix, ListTrie trie)
        {
            var candidates = new List<ICandidate>();
            var trieQueue = new Queue<Tuple<string, ListTrie>>();
            trieQueue.Enqueue(new Tuple<string, ListTrie>(prefix, trie));

            while (trieQueue.Count != 0)
            {
                var nextTrieTuple = trieQueue.Dequeue();
                var nextPrefix = nextTrieTuple.Item1;
                var nextTrie = nextTrieTuple.Item2;

                foreach (ListTrie subTrie in nextTrie._subTries)
                {
                    if (subTrie._isTerminatedWord)
                    {
                        candidates.Add(new Candidate(nextPrefix + subTrie._character, subTrie._wordOccurenceCount));
                    }

                    trieQueue.Enqueue(new Tuple<string, ListTrie>(nextPrefix + subTrie._character, subTrie));
                }
            }

            return candidates;
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

            foreach (ListTrie subTrie in _subTries)
            {
                var characterValue = _character == default(char) ? ' ' : _character;
                sb.Append("{ " + level + " " + characterValue + " : " + subTrie.ToString(level) + "}");
            }

            return sb.ToString();
        }
    }
}