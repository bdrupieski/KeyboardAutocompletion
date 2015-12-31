using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyboardAutocompletion.Tries
{
    [Serializable]
    public class ArrayTrie : ITrie
    {
        private readonly ArrayTrie[] _subTries = new ArrayTrie[26];
        private bool _isTerminatedWord = false;
        private int _wordOccurenceCount = 0;

        public void TrainTrie(IEnumerable<string> words)
        {
            foreach (var word in words)
            {
                ArrayTrie currentTrie = this;

                foreach (char c in word)
                {
                    int index = MapCharToInt(c);
                    if (currentTrie._subTries[index] == null)
                    {
                        currentTrie._subTries[index] = new ArrayTrie();
                    }

                    currentTrie = currentTrie._subTries[index];
                }

                currentTrie._isTerminatedWord = true;
                currentTrie._wordOccurenceCount++;
            }
        }

        private static int MapCharToInt(char c)
        {
            return c - 97;
        }

        private static char MapIntToChar(int i)
        {
            return (char)(i + 97);
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

        private ArrayTrie FindLastSubTrieIfExists(string word)
        {
            ArrayTrie currentTrie = this;

            foreach (var c in word)
            {
                int index = MapCharToInt(c);
                if (currentTrie._subTries[index] != null)
                {
                    currentTrie = currentTrie._subTries[index];
                }
                else
                {
                    return null;
                }
            }

            return currentTrie;
        }

        private IEnumerable<ICandidate> CollectCandidatesAtTrie(string prefix, ArrayTrie trie)
        {
            var candidates = new List<ICandidate>();
            var trieQueue = new Queue<Tuple<string, ArrayTrie>>();
            trieQueue.Enqueue(new Tuple<string, ArrayTrie>(prefix, trie));

            while (trieQueue.Count != 0)
            {
                var nextTrieTuple = trieQueue.Dequeue();
                var nextPrefix = nextTrieTuple.Item1;
                var nextTrie = nextTrieTuple.Item2;

                for (int i = 0; i < nextTrie._subTries.Length; i++)
                {
                    ArrayTrie subTrie = nextTrie._subTries[i];
                    if (subTrie != null)
                    {
                        char character = MapIntToChar(i);
                        if (subTrie._isTerminatedWord)
                        {
                            candidates.Add(new Candidate(nextPrefix + character, subTrie._wordOccurenceCount));
                        }

                        trieQueue.Enqueue(new Tuple<string, ArrayTrie>(nextPrefix + character, subTrie));
                    }
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

            for (int i = 0; i < _subTries.Length; i++)
            {
                ArrayTrie subTrie = _subTries[i];
                if (subTrie != null)
                {
                    char c = MapIntToChar(i);
                    sb.Append("{ " + level + " " + c + " : " + _subTries[c].ToString(level) + "}");
                }
            }

            return sb.ToString();
        }
    }
}