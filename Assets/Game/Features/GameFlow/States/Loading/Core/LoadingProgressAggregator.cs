using System;
using UnityEngine;

public sealed class LoadingProgressAggregator
{
    private float[] _weights;
    private float[] _values;

    public int TaskCount => _values?.Length ?? 0;

    #region Initialize

    // Default: equal weight
    public void Initialize(int taskCount)
    {
        if (taskCount <= 0)
            throw new ArgumentException("Task count must be > 0");

        _weights = new float[taskCount];
        _values = new float[taskCount];

        float weight = 1f / taskCount;
        for (int i = 0; i < taskCount; i++)
            _weights[i] = weight;
    }

    // ✅ NEW: weighted init
    public void Initialize(float[] weights)
    {
        if (weights == null || weights.Length == 0)
            throw new ArgumentException("Weights cannot be null or empty");

        _weights = new float[weights.Length];
        _values = new float[weights.Length];

        float sum = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] < 0f)
                throw new ArgumentException("Weight cannot be negative");

            sum += weights[i];
        }

        if (sum <= 0f)
            throw new ArgumentException("Total weight must be > 0");

        // Normalize về 1.0
        for (int i = 0; i < weights.Length; i++)
        {
            _weights[i] = weights[i] / sum;
        }
    }

    #endregion

    #region Report

    public void Report(int index, float progress)
    {
        if (_values == null)
            throw new InvalidOperationException("Aggregator not initialized");

        if (index < 0 || index >= _values.Length)
            throw new ArgumentOutOfRangeException(nameof(index));

        _values[index] = Mathf.Clamp01(progress);
    }

    #endregion

    #region Result

    public float GetTotalProgress()
    {
        if (_values == null)
            return 0f;

        float total = 0f;
        for (int i = 0; i < _values.Length; i++)
        {
            total += _values[i] * _weights[i];
        }

        return total;
    }

    #endregion
}