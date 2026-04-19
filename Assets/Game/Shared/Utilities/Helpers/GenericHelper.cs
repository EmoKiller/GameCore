
public static class GenericHelper 
{
    /// <summary>
    /// Tạo một instance mới của một class bất kỳ có constructor mặc định (parameterless constructor).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T CreateInstance<T>() where T : new()
    {
        return new T();
    }
}
