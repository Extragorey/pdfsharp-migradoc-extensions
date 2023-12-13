using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.Extensions.Markdown {
    public static class SectionExtensions {
        /// <summary>
        /// Adds the given <paramref name="markdown"/> formatted text to the section.
        /// </summary>
        public static Section AddMarkdown(this Section section, string markdown) {
            return section.Add(markdown, new MarkdownConverter());
        }

        /// <summary>
        /// Adds the given <paramref name="markdown"/> formatted text to the header or footer.
        /// </summary>
        public static HeaderFooter AddMarkdown(this HeaderFooter section, string markdown) {
            return section.Add(markdown, new MarkdownConverter());
        }

        /// <summary>
        /// Adds the given <paramref name="markdown"/> formatted text to the table cell.
        /// </summary>
        public static Cell AddMarkdown(this Cell cell, string markdown) {
            return cell.Add(markdown, new MarkdownConverter());
        }

        /// <summary>
        /// Adds the given <paramref name="markdown"/> formatted text to the paragraph.
        /// </summary>
        public static Paragraph AddMarkdown(this Paragraph paragraph, string markdown) {
            return paragraph.Add(markdown, new MarkdownConverter());
        }
    }
}
