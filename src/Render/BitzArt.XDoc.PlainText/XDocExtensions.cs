namespace BitzArt.XDoc;

/// <summary>
/// Extension methods for <see cref="XDoc"/> objects which allow rendering documentation in plain text.
/// </summary>
public static class XDocExtensions
{
    /// <summary>
    /// Renders the documentation of a <see cref="Type"/> as plain text.
    /// </summary>
    /// <param name="documentation"></param>
    /// <returns></returns>
    public static string ToPlainText(this MemberDocumentation? documentation)
    {
        return documentation == null ? string.Empty : PlainTextRenderer.Render(documentation);
    }
}