using System;
using System.Collections.Generic;
using System.Linq;
using KeyboardAutocompletion.AutocompleteProviders;

namespace KeyboardAutocompletion
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new PrefixDictionaryAutocompleteProvider();

            while (true)
            {
                var action = GetUserAction();

                if (action == Action.Train)
                {
                    var passage = GetPassageFromUser();
                    provider.Train(passage);
                }
                else
                {
                    var fragment = GetFragmentFromUser();
                    var candidates = provider.GetWords(fragment.ToLower());
                    Console.WriteLine("Here are the suggested words for fragment '{0}':", fragment);
                    Console.WriteLine(CandidatesStringRepresentation(candidates));
                }
            }
        }

        enum Action
        {
            Train = 1, Test = 2
        }

        static Action GetUserAction()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Type 'train' to train the autocomplete provider with a passage or 'test' " +
                              "to test a fragment for suggestions, then press enter to continue:");
            Console.ResetColor();
            var line = Console.ReadLine();
            Console.WriteLine();

            while (line != "train" && line != "test")
            {
                Console.WriteLine("I couldn't understand that. Try again:");
                line = Console.ReadLine();
                Console.WriteLine();
            }

            return line == "train" ? Action.Train : Action.Test;
        }

        static string GetPassageFromUser()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Type a passage and then press enter to continue:");
            Console.ResetColor();
            return Console.ReadLine();
        }

        static string GetFragmentFromUser()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Type a word fragment and press enter to continue:");
            Console.ResetColor();
            return Console.ReadLine();
        }

        static string CandidatesStringRepresentation(IEnumerable<ICandidate> candidates)
        {
            return string.Join(", ", candidates.Select(x => $"{x.Word} ({x.Confidence})"));
        }
    }
}
