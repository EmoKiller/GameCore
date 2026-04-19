public interface IFlowTracer
{
    void OnFlowStart(string flowName);
    void OnStepStart(string stepName);
    void OnStepEnd(string stepName, float ms);
}