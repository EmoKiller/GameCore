// builder.AddNode(new FlowTraceNode
// {
//     Id = stepId,
//     Name = stepName,
//     FlowName = flowName,
//     Timestamp = Now()
// });

// builder.Link(prevStepId, nextStepId, "await_continuation");

// eventGraph.Add(new EventTraceNode
// {
//     Id = eventId,
//     EventType = typeof(T).Name,
//     ProducerId = senderId,
//     Timestamp = Now()
// });

// eventGraph.Link(sourceEventId, targetEventId, "triggered");