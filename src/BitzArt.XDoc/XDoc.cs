using System.Reflection;

namespace BitzArt.XDoc;

/// <summary>
/// Provides functionality for fetching and parsing documentation
/// of code members.
/// <para>
/// Currently supported code members: <br />
/// � <see cref="Assembly"/> <br />
/// � <see cref="Type"/> <br />
/// � <see cref="PropertyInfo"/>
/// </para>
/// <para>
/// Fetching documentation for an <see cref="Assembly"/> or any of its members
/// will result in parsing and indexing of the XML documentation file provided for said <see cref="Assembly"/>.
/// </para>
/// <para>
/// As long as you retain an instance of an <see cref="XDoc"/> object,
/// all data fetched by it will be parsed, cached, indexed and reused
/// for subsequent requests. <br />
/// Disposing of an <see cref="XDoc"/> object will lead to a loss of all said data.
/// </para>
/// </summary>
public class XDoc
{
    private readonly Dictionary<Assembly, AssemblyDocumentation> _fetchedAssemblies = [];

    /// <summary>
    /// Fetches documentation for the specified <see cref="Assembly"/>.
    /// </summary>
    /// <param name="assembly">The <see cref="Assembly"/> to retrieve documentation for.</param>"/>
    /// <returns><see cref="AssemblyDocumentation"/> for the specified <see cref="Assembly"/>.</returns>
    public AssemblyDocumentation GetDocumentation(Assembly assembly)
        => _fetchedAssemblies.TryGetValue(assembly, out var result)
            ? result
            : Fetch(assembly);

    private AssemblyDocumentation Fetch(Assembly assembly)
    {
        var result = new AssemblyDocumentation(this, assembly);
        _fetchedAssemblies.Add(assembly, result);

        return result;
    }

    /// <summary>
    /// Fetches documentation for the specified <see cref="Type"/>.
    /// </summary>
    /// <param name="type">The <see cref="Type"/> to retrieve documentation for.</param>"/>
    /// <returns>
    /// <see cref="TypeDocumentation"/> for the specified <see cref="Type"/> if available;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    public TypeDocumentation? GetDocumentation(Type type)
        => GetDocumentation(type.Assembly).GetDocumentation(type);

    /// <summary>
    /// Fetches documentation for the specified <see cref="PropertyInfo"/>.
    /// </summary>
    /// <param name="property">The <see cref="PropertyInfo"/> to retrieve documentation for.</param>
    /// <returns>
    /// <see cref="PropertyDocumentation"/> for the specified <see cref="PropertyInfo"/> if available;
    /// otherwise, <see langword="null"/>.
    /// </returns>
    public PropertyDocumentation? GetDocumentation(PropertyInfo property)
        => GetDocumentation(property.DeclaringType!)?.GetDocumentation(property);
}