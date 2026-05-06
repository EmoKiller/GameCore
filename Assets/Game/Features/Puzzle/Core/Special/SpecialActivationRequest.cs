public readonly struct SpecialActivationRequest
{
    public TilePosition Position { get; }

    public TileData Tile { get; }

    public SpecialActivationRequest(
        TilePosition position,
        TileData tile)
    {
        Position = position;

        Tile = tile;
    }
}
