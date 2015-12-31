using System.Collections.Generic;

namespace KeyboardAutocompletion.AutocompleteProviders
{
    public interface IAutocompleteProvider
    {
        IEnumerable<ICandidate> GetWords(string fragment);
        void Train(string passage);
    }
}