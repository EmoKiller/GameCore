public readonly struct SwapContext
{
    public TilePosition From { get; }

    public TilePosition To { get; }

    public SwapContext(TilePosition from, TilePosition to)
    {
        From = from;

        To = to;
    }
}
