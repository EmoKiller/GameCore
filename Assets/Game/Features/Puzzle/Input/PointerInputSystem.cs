public interface IPointerInputSystem
{
    PointerState Current { get; }
}
public sealed class PointerInputSystem : IPointerInputSystem
{
    private readonly IInputPuzzleService _input;

    public PointerState Current { get; private set; }

    public PointerInputSystem(IInputPuzzleService input)
    {
        _input = input;
    }

    public void Tick()
    {
        Current = _input.GetPointer();
    }
}