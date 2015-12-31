using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace KeyboardAutocompletion.AutocompleteProviders
{
    [Serializable]
    public abstract class BaseAutocompleteProvider : IAutocompleteProvider
    {
        private readonly Regex _lowercaseRegex = new Regex("[^a-z]");

        protected IEnumerable<string> TokenizeWords(string passage)
        {
            return passage.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                          .Select(x => _lowercaseRegex.Replace(x.ToLower(), ""));
        }

        public abstract IEnumerable<ICandidate> GetWords(string fragment);
        public abstract void Train(string passage);
    }
}