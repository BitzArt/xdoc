using System.Xml;

namespace BitzArt.XDoc.PlaintText.Tests;

public class XmlRendererTests
{
    // [Fact]
    // public void Render_SimpleXmlDoc_ShouldReturnFormattedText()
    // {
    //     // Arrange
    //     var xml = """
    //               <summary>
    //                   Class B Name of specific <see cref="T:TestAssembly.B.Dog" />.
    //                   Not all <see cref="T:TestAssembly.A.Animal" />s can have a name.
    //                   <example>
    //                       Dog: Dog.Name = "Rex"
    //                   </example>
    //                   <remarks></remarks>
    //                   Be carefully with this property.
    //               </summary>
    //               """;
    //     var xmlNode = GetXmlNode(xml);
    //     var xmlRenderer = new XmlRenderer();
    //
    //     // Act
    //     var str = xmlRenderer.Render(xmlNode);
    //
    //     // Assert
    //     Assert.Contains("Class B Name of specific TestAssembly.B.Dog", str);
    //     Assert.Contains("Dog: Dog.Name = \"Rex\"", str);
    // }
    //
    // [Fact]
    // public void Render_ComplexXmlDoc_ShouldReturnFormattedText()
    // {
    //     // Arrange
    //     string xml = """
    //                  <summary>
    //                      This method retrieves a specific <see cref="T:MyNamespace.MyClass" /> instance  based on the provided <paramref name="id" /> parameter.
    //                      
    //                      <para>
    //                          The method ensures that the returned instance is properly initialized 
    //                          and ready for use. If the <paramref name="id" /> is invalid, it will throw an 
    //                          <see cref="T:System.ArgumentException" />.
    //                      </para>
    //                  
    //                      <para>
    //                          Usage example:
    //                          <code>
    //                          var instance = MyClass.GetInstance(42);
    //                          Console.WriteLine(instance.Name);
    //                          </code>
    //                      </para>
    //                  
    //                      <list type="bullet">
    //                          <item><description>Ensures proper initialization.</description></item>
    //                          <item><description>Throws if <paramref name="id" /> is invalid.</description></item>
    //                          <item><description>Returns an instance of <typeparamref name="T" />.</description></item>
    //                      </list>
    //                  
    //                      <example>
    //                          The following example demonstrates how to use this method:
    //                          <code>
    //                          try
    //                          {
    //                              var obj = MyClass.GetInstance(10);
    //                              Console.WriteLine($"Object Name: {obj.Name}");
    //                          }
    //                          catch (ArgumentException ex)
    //                          {
    //                              Console.WriteLine($"Error: {ex.Message}");
    //                          }
    //                          </code>
    //                      </example>
    //                  
    //                      <remarks>
    //                          Be careful when using this method with untrusted input, as passing an invalid 
    //                          <paramref name="id" /> may result in an exception.
    //                      </remarks>
    //                  </summary>
    //                  """;
    //     var xmlNode = GetXmlNode(xml);
    //     var xmlRenderer = new XmlRenderer();
    //
    //     // Act
    //     var str = xmlRenderer.Render(xmlNode);
    //
    //     // Assert
    //     Assert.Contains("```", str);
    //     Assert.Contains("– Ensures proper initialization", str);
    //     Assert.Contains("This method retrieves a specific MyNamespace.MyClass instance based on the provided id", str);
    //     Assert.Contains("Console.WriteLine($\"Object Name: {obj.Name}\");", str);
    //     Assert.Contains("Be careful when using this method with untrusted input", str);
    // }

    private static XmlNode GetXmlNode(string xml)
    {
        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xml);

        XmlNode xmlNode = xmlDoc.DocumentElement!; // Get the node

        return xmlNode;
    }
}