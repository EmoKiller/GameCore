public interface IEventBusTracer
{
    void OnPublish(string eventName);
    void OnHandlerStart(string eventName);
    void OnHandlerEnd(string eventName, float ms);
}