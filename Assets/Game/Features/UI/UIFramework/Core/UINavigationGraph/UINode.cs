using System;
using System.Collections.Generic;

public sealed class UINode
{
    public Type ViewType;
    public List<UIEdge> Edges = new();
}