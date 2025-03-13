using TestAssembly.A;
using TestAssembly.B;

namespace BitzArt.XDoc.Tests;

public class XDocExtensionsTests
{
    [Fact]
    public void ToPlainText_DogClass_ShouldReturnDescription()
    {
        // Arrange
        var xDoc = new XDoc();

        // Act
        var str = xDoc.Get(typeof(Dog)).ToPlainText();

        // Assert
        Assert.NotEmpty(str);
    }

    [Fact]
    public void ToPlainText_DogNameProperty_ShouldReturnDescription()
    {
        // Arrange
        var xDoc = new XDoc();

        // Act
        var str = xDoc.Get(typeof(Dog).GetProperty(nameof(Dog.Name))!).ToPlainText();

        // Assert
        Assert.NotEmpty(str);
        Assert.Contains("Dog: Dog.Name = \"Rex\"", str);
    }

    [Fact]
    public void ToPlainText_DogProperties_ShouldReturnDescriptions()
    {
        // Arrange
        var xDoc = new XDoc();
        var type = typeof(Dog);
        // var type = typeof(Animal);

        var methodInfo = typeof(IAnimal).GetMethod(nameof(IAnimal.GetProperty1));
        // var methodInfo = typeof(Dog).GetMethod(nameof(IAnimal.GetProperty1));
        // new Dog().GetInfo()

        //type.GetMethod("Get")
        var propertyInfos = type.GetProperties();

        // var field1Info = type.GetProperty(nameof(Dog.Field1));
        // var field2Info = type.GetProperty(nameof(Dog.Field2));
        // var nameInfo = type.GetProperty(nameof(Dog.Name));
        var idInfo = type.GetProperty(nameof(Dog.Id));
        
        // Act
        // var filed1Comment = xDoc.Get(field1Info!).ToPlainText();
        // var filed2Comment = xDoc.Get(field2Info!).ToPlainText();
        // var nameComment = xDoc.Get(nameInfo!).ToPlainText();
        var idComment = xDoc.Get(idInfo!).ToPlainText();

        // Assert
        // Assert.NotEmpty(filed1Comment);
        // Assert.NotEmpty(filed2Comment);
        // Assert.NotEmpty(nameComment);
        Assert.NotEmpty(idComment);
    }
}