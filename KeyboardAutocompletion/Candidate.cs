namespace KeyboardAutocompletion
{
    public class Candidate : ICandidate
    {
        public Candidate(string word, int confidence)
        {
            Word = word;
            Confidence = confidence;
        }

        public string Word { get; }
        public int Confidence { get; }

        protected bool Equals(Candidate other)
        {
            return string.Equals(Word, other.Word) && Confidence == other.Confidence;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Candidate)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Word?.GetHashCode() ?? 0) * 397) ^ Confidence;
            }
        }

        public override string ToString()
        {
            return $"Word: {Word}, Confidence: {Confidence}";
        }
    }
}