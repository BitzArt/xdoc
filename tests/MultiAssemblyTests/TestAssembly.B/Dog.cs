using TestAssembly.A;

namespace TestAssembly.B;

/// <summary>
/// Test class B.
/// </summary>
public class Dog : Animal, IDog
{
    /// <summary>
    /// Dog's Age
    /// </summary>
    public int Age;
    
    /// <summary>
    /// Class B Name
    /// </summary>
    public string Name { get; set; } = "";

    /// <inheritdoc/>
    public string Field1 { get; set; } = "";

    /// <inheritdoc/>
    public string Field2 { get; set; } = "";

    /// <summary>
    /// Get some info
    /// </summary>
    /// <returns></returns>
    public string GetInfo()
    {
        return $"Field1: {Field1}, Field2: {Field2}";
    }
}