using System.Collections.Generic;

namespace KeyboardAutocompletion.Tries
{
    public interface ITrie
    {
        void TrainTrie(IEnumerable<string> words);
        IEnumerable<ICandidate> CollectCandidatesAtPrefix(string prefix);
    }
}