public interface ILoadingTracer
{
    void OnTaskStart(string task);
    void OnTaskProgress(string task, float progress);
    void OnTaskEnd(string task, float ms);
}