using TestAssembly.A;

namespace TestAssembly.B;

/// <summary>
/// This is a class which represents a dog as defined by the interface <see cref="IDog"/>.
/// </summary>
public class Dog : Animal, IDog
{
    /// <summary>
    /// Dog's Age
    /// </summary>
    public int Age;
    
    /// <summary>
    /// Class B Name of specific <see cref="Dog"/>.
    /// Not all <see cref="Animal"/>s can have a name.
    /// <example>
    ///     Dog: Dog.Name = "Rex"
    /// </example>
    /// <remarks></remarks>
    /// Be carefully with this property.
    /// </summary>
    public string Name { get; set; } = "";

    /// <inheritdoc/>
    public string Field1 { get; set; } = "";

    /// <inheritdoc/>
    public string Field2 { get; set; } = "";

    /// <summary>
    /// Get some about <see cref="Dog"/>
    /// </summary>
    /// <returns></returns>
    public string GetInfo()
    {
        return $"Field1: {Field1}, Field2: {Field2}";
    }
}