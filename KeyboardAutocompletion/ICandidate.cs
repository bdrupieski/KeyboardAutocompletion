namespace KeyboardAutocompletion
{
    public interface ICandidate
    {
        string Word { get; }
        int Confidence { get; }
    }
}