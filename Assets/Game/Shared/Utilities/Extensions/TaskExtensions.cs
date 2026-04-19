using System;
using System.Threading.Tasks;
using UnityEngine;

public static class TaskExtensions
{
    public static async void ForgetSafe(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
}