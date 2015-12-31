I needed to implement a keyboard autocompletion algorithm recently. Given a set of training words, it can provide autocomplete suggestions for a given typed word. For example, if trained on previously typed words like "the thing I like is the thing over there", typing the prefix "th" would result in "the", "thing", and "there" as possible autocompletions, with "the" and "there" weighted more heavily since they appear twice in the set of all previously typed words.

If you had to design this algorithm for a relatively resource-constrained device, like a smartphone keyboard, how would you do it? You would need something that could recommend autocompleted words fast - fast enough to keep up with a user typing - and not take up much memory with whatever data structure(s) you need to maintain stored autocompletions.

This repository is my attempt at a design for such a system. There are two main approaches:

- Use a dictionary (associative array) to map each possible word prefix to the set of possible autocompletions and their frequencies for that word prefix.
- Use a trie to efficiently store all prefixes and obtain autocompletions by traversing the subtrie depth-first for a given prefix.

The dictionary approach is simple to implement but I thought might require too much memory to be practical, so I tried another implementation with tries in order to compare performance and memory usage. I initially implemented the trie by storing children nodes for each node in a `Dictionary<char, Trie>`. I benchmarked the two approaches and found that this initial trie approach actually used more memory. 

I then tried two different trie implementations, one using arrays and one using `List<T>`. I experienced much lower memory usage with both over the approach using `Dictionary<char, Trie>`. 

The array approach builds an ad-hoc dictionary by creating an array of 26 tries and mapping each character to an index in the array. However, if a node only uses, say, 3 of those buckets, then 23 of the array elements are null and a waste of space. That's 92 bytes in 32-bit land I could've saved if I had a more compact representation. 

I tried implementing a trie using a `List<T>` (internally backed by a resized array) of tries at each node to attempt to get a more compact trie. Without using an array index to store the character for the node, I needed a way to explicitly store the character for each node. This meant adding a `char` instance field to each trie. Hopefully the space saved by using a `List<T>`, I thought, would make up for the extra field in each trie. Unfortunately this wasn't the case, and it ends up using just barely more memory than the approach using an array to store tries for each node. Additionally, searching children using a `List<T>` means parts of trie traversal are now O(n) instead of O(1).

I benchmarked each approach on a copy of War and Peace by Leo Tolstoy obtained from [Project Gutenberg](http://www.gutenberg.org/ebooks/2600), which is about 568,000 words. The results are below.

```
Benchmarking PrefixDictionaryAutocompleteProvider
1526 ms to train war_and_peace.txt
5020 ms to get autocompletions 1000 times for 'th'.
Trained provider uses roughly 60603 KB.

Benchmarking TrieAutocompleteProvider<DictionaryTrie>
822 ms to train war_and_peace.txt
9908 ms to get autocompletions 1000 times for 'th'.
Trained provider uses roughly 78895 KB.

Benchmarking TrieAutocompleteProvider<ArrayTrie>
682 ms to train war_and_peace.txt
8720 ms to get autocompletions 1000 times for 'th'.
Trained provider uses roughly 18441 KB.

Benchmarking TrieAutocompleteProvider<ListTrie>
914 ms to train war_and_peace.txt
7863 ms to get autocompletions 1000 times for 'th'.
Trained provider uses roughly 20288 KB.
```

3071 autocompletions are returned for 'th' for War and Peace.

`PrefixDictionaryAutocompleteProvider` was the first approach I tried. It uses a whopping 60.6 MB to store its autocompletions. I tried `TrieAutocompleteProvider<DictionaryTrie>` next and thought it would use less memory, but it turns out it actually takes up about 30% more. It's a little faster to train, but it provides autocompletions more slowly too.

I tried `TrieAutocompleteProvider<ArrayTrie>` and `TrieAutocompleteProvider<ListTrie>` next. They both use much less memory, with `ArrayTrie` using the least. It's also the fastest to train. I switched to using an iterative traversal hoping for additional speed gains when finding autocompletions.

I declare `TrieAutocompleteProvider<ArrayTrie>` the all-around winner.

I'm not comfortable with the time it takes to find autocompletions for any of these approaches. For a single word instead of a thousand like in the benchmark above we're still only talking tens of milliseconds, but for slower phones this will probably stretch out to be far too long. Even tens of milliseconds is a perceptible and annoying delay. 

Autocompleting a short word, such as "th" or even "t" will find many, many autocompletions. Most of these are not useful. A secondary data structure could be maintained for very short words that only keep the most likely autocompletions and do not contain the full set. As the word size increases, the algorithm could defer to the full set which will find them very quickly. I think this could ameliorate any concern about autocompletion retrieval time.

