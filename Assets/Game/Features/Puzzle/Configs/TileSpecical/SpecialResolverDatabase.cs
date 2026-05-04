using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Special Resolver Database",menuName = "Puzzle/Special/Special Resolver Database")]
public sealed class SpecialResolverDatabase : ScriptableObject
{
    [field: SerializeField]
    public List<SpecialResolverEntry> Entries {get; private set;}
}
