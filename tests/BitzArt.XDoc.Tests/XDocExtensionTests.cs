using Microsoft.Extensions.DependencyInjection;
using TestAssembly.B;

namespace BitzArt.XDoc.Extensions.Tests;

public class XDocExtensionTests
{
    [Fact]
    public void AddXDoc_RegistersIXDocAsSingleton()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddXDoc();

        // Assert
        var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == typeof(XDoc));

        Assert.NotNull(serviceDescriptor);
        Assert.Equal(ServiceLifetime.Singleton, serviceDescriptor.Lifetime);
        Assert.Equal(typeof(XDoc), serviceDescriptor.ImplementationType);
    }

    [Fact]
    public void AddXDoc_CanResolveIXDocService()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXDoc();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var xdoc = serviceProvider.GetRequiredService<XDoc>();

        // Assert
        Assert.NotNull(xdoc);
        Assert.IsType<XDoc>(xdoc);
    }

    [Fact]
    public void Get_CanGetProperty()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddXDoc();
        var serviceProvider = services.BuildServiceProvider();

        // Act
        var xdoc = serviceProvider.GetRequiredService<XDoc>();

        var typeDocumentation = xdoc.Get(typeof(Dog));
        var propertyDocumentation = xdoc.Get(typeof(Dog).GetProperty(nameof(Dog.Property1)));

        // Assert
        Assert.NotNull(typeDocumentation);
        Assert.NotNull(propertyDocumentation);
    }
}