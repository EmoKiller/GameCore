public interface IPointerInputSystem
{
    PointerState Current { get; }
    void Update();
}
public sealed class PointerInputSystem : IPointerInputSystem
{
    private readonly IInputPuzzleService _input;

    public PointerState Current { get; private set; }

    public PointerInputSystem(IInputPuzzleService input)
    {
        _input = input;
    }

    public void Update()
    {
        Current = _input.GetPointer();
    }
}