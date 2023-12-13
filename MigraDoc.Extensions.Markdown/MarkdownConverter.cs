using Markdig;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Extensions.Html;

namespace MigraDoc.Extensions.Markdown {
    /// <summary>
    /// Handles the conversion from Markdown text to HTML markup, which is then
    /// passed to the HtmlConverter for conversion to MigraDoc objects.
    /// </summary>
    public class MarkdownConverter : IConverter {
        private readonly MarkdownPipeline options;

        /// <summary>
        /// Initialises a new MarkdownConverter with default options.
        /// By default, the only option used is auto links
        /// (converts recognised hyperlinks into HTML anchors).
        /// </summary>
        public MarkdownConverter() {
            options = new MarkdownPipelineBuilder()
                //.UseSoftlineBreakAsHardlineBreak()
                .UseAutoLinks()
                .Build();
        }

        /// <summary>
        /// Initialises a new MarkdownConverter with the given
        /// <paramref name="options"/> (must not be null).
        /// </summary>
        public MarkdownConverter(MarkdownPipeline options) {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <summary>
        /// Converts the Markdown <paramref name="contents"/> string to HTML
        /// and returns the action for converting HTML into a MigraDoc Section.
        /// </summary>
        /// <param name="contents">The Markdown-formatted string to convert.</param>
        /// <returns>The action for converting the resulting HTML into a MigraDoc Section.</returns>
        public Action<Section> Convert(string contents) {
            var html = Markdig.Markdown.ToHtml(contents, options);
            var htmlConverter = new HtmlConverter();
            return htmlConverter.Convert(html);
        }

        /// <summary>
        /// Converts the Markdown <paramref name="contents"/> string to HTML
        /// and returns the action for converting HTML into a MigraDoc HeaderFooter.
        /// </summary>
        /// <param name="contents">The Markdown-formatted string to convert.</param>
        /// <returns>The action for converting the resulting HTML into a MigraDoc HeaderFooter.</returns>
        public Action<HeaderFooter> ConvertHeaderFooter(string contents) {
            var html = Markdig.Markdown.ToHtml(contents, options);
            var htmlConverter = new HtmlConverter();
            return htmlConverter.ConvertHeaderFooter(html);
        }

        /// <summary>
        /// Converts the Markdown <paramref name="contents"/> string to HTML
        /// and returns the action for converting HTML into a MigraDoc Cell.
        /// </summary>
        /// <param name="contents">The Markdown-formatted string to convert.</param>
        /// <returns>The action for converting the resulting HTML into a MigraDoc Cell.</returns>
        public Action<Cell> ConvertCell(string contents) {
            var html = Markdig.Markdown.ToHtml(contents, options);
            var htmlConverter = new HtmlConverter();
            return htmlConverter.ConvertCell(html);
        }

        /// <summary>
        /// Converts the Markdown <paramref name="contents"/> string to HTML
        /// and returns the action for converting HTML into a MigraDoc Paragraph.
        /// </summary>
        /// <param name="contents">The Markdown-formatted string to convert.</param>
        /// <returns>The action for converting the resulting HTML into a MigraDoc Paragraph.</returns>
        public Action<Paragraph> ConvertParagraph(string contents) {
            var html = Markdig.Markdown.ToHtml(contents, options);
            var htmlConverter = new HtmlConverter();
            return htmlConverter.ConvertParagraph(html);
        }
    }
}
