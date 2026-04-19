public interface IUIStateful
{
    object CaptureState();
    void RestoreState(object state);
}
