using Xunit;

namespace RhinoFoundry.UI.Tests;

public class MarkdownContentTests
{
    [Fact] public void RendersHeadingsEmphasisAndTables()
    {
        var html = FoundryMarkdownContent.ToHtml("### Review\n\n**Ready** and `code`\n\n| Sheet | Status |\n|---|---|\n| A01 | Reuse |");
        Assert.Contains("<h3>Review</h3>", html);
        Assert.Contains("<strong>Ready</strong>", html);
        Assert.Contains("<code>code</code>", html);
        Assert.Contains("<table>", html);
    }
    [Fact] public void RawHtmlCannotBecomeExecutableMarkup()
    {
        var html = FoundryMarkdownContent.ToHtml("<script>alert(1)</script>\n<iframe src='https://example.com'></iframe>");
        Assert.DoesNotContain("<script>", html);
        Assert.DoesNotContain("<iframe", html);
        Assert.Contains("&lt;script&gt;", html);
    }
}
