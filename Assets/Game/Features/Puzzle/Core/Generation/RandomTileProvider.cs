
public interface IRandomTileProvider
{
    Tile GetRandom();
}
public sealed class RandomTileProvider : IRandomTileProvider
{
    private readonly ETileType[] _types;
    private readonly IRandomProvider _random;

    public RandomTileProvider(ETileType[] types, IRandomProvider random)
    {
        _types = types;
        _random = random;
    }

    public Tile GetRandom()
    {
        var index = _random.Next(0, _types.Length);
        return new Tile(_types[index]);
    }
}
