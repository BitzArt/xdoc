namespace BitzArt.XDoc.PlainText;

public interface IXmlToPlainTextRenderer
{
    /// <summary>
    /// Renders the documentation of a <see cref="MemberDocumentation"/> as plain text.
    /// </summary>
    /// <param name="documentation"></param>
    /// <returns></returns>
    string Render(MemberDocumentation documentation);
}