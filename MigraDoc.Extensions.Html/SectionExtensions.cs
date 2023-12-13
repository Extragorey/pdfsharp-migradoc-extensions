using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.Extensions.Html {
    public static class SectionExtensions {
        /// <summary>
        /// Adds the given <paramref name="html"/> formatted text to the section.
        /// </summary>
        public static Section AddHtml(this Section section, string html, bool isPdf = false) {
            return section.Add(Sanitize(html), new HtmlConverter(isPdf));
        }

        /// <summary>
        /// Adds the given <paramref name="html"/> formatted text to the header or footer.
        /// </summary>
        public static HeaderFooter AddHtml(this HeaderFooter headerFooter, string html, bool isPdf = false) {
            return headerFooter.Add(Sanitize(html), new HtmlConverter(isPdf));
        }

        /// <summary>
        /// Adds the given <paramref name="html"/> formatted text to the table cell.
        /// </summary>
        public static Cell AddHtml(this Cell cell, string html, bool isPdf = false) {
            return cell.Add(Sanitize(html), new HtmlConverter(isPdf));
        }

        /// <summary>
        /// Adds the given <paramref name="html"/> formatted text to the paragraph.
        /// </summary>
        public static Paragraph AddHtml(this Paragraph paragraph, string html, bool isPdf = false) {
            return paragraph.Add(Sanitize(html), new HtmlConverter(isPdf));
        }



        private static string Sanitize(string html) {
            if (string.IsNullOrWhiteSpace(html)) {
                return html;
            }

            return html.Replace(@"\""", @"""");
        }
    }
}
