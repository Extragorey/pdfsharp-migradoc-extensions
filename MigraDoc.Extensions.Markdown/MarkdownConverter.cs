using Markdig;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Extensions.Html;

namespace MigraDoc.Extensions.Markdown {
    public class MarkdownConverter : IConverter {
        private readonly MarkdownPipeline options;

        public MarkdownConverter() {
            options = new MarkdownPipelineBuilder()
                //.UseSoftlineBreakAsHardlineBreak()
                .UseAutoLinks()
                .Build();
        }

        public MarkdownConverter(MarkdownPipeline options) {
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public Action<Section> Convert(string contents) {
            var html = Markdig.Markdown.ToHtml(contents, options);
            var htmlConverter = new HtmlConverter();
            return htmlConverter.Convert(html);
        }

        public Action<HeaderFooter> ConvertHeaderFooter(string contents) {
            var html = Markdig.Markdown.ToHtml(contents, options);
            var htmlConverter = new HtmlConverter();
            return htmlConverter.ConvertHeaderFooter(html);
        }

        public Action<Cell> ConvertCell(string contents) {
            var html = Markdig.Markdown.ToHtml(contents, options);
            var htmlConverter = new HtmlConverter();
            return htmlConverter.ConvertCell(html);
        }

        public Action<Paragraph> ConvertParagraph(string contents) {
            var html = Markdig.Markdown.ToHtml(contents, options);
            var htmlConverter = new HtmlConverter();
            return htmlConverter.ConvertParagraph(html);
        }
    }
}
