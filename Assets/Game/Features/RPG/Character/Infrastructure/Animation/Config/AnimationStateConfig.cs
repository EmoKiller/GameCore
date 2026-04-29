using System;
using UnityEngine;

[Serializable]
public class AnimationStateConfig
{
    public ECharacterAnimationState State;
    public int LayerIndex;

    [Header("Default")]
    [SerializeField] private string defaultStateName;

    [Header("Variants")]
    [SerializeField] private string[] variantStateNames;

    [Header("Settings")]
    public float CrossFadeDuration = 0.1f;

    // ========================
    // RUNTIME CACHE (HASH)
    // ========================
    private int _defaultHash;
    private int[] _variantHashes;

    public void Initialize()
    {
        _defaultHash = Animator.StringToHash(defaultStateName);

        if (variantStateNames != null && variantStateNames.Length > 0)
        {
            _variantHashes = new int[variantStateNames.Length];
            for (int i = 0; i < variantStateNames.Length; i++)
            {
                _variantHashes[i] = Animator.StringToHash(variantStateNames[i]);
            }
        }
        else
        {
            _variantHashes = null;
        }
    }

    public int ResolveHash(int variant)
    {
        if (_variantHashes != null && _variantHashes.Length > 0)
        {
            int index = Mathf.Clamp(variant, 0, _variantHashes.Length - 1);
            return _variantHashes[index];
        }

        return _defaultHash;
    }
}