using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.Extensions {
    /// <summary>
    /// Defines extension methods for various MigraDoc objects.
    /// </summary>
    public static class SectionExtensions {
        /// <summary>
        /// Adds the given contents with the given converter to the current Section.
        /// </summary>
        /// <param name="section">The current Section.</param>
        /// <param name="contents">The HTML or Markdown string to add.</param>
        /// <param name="converter">The HTML or Markdown converter.</param>
        /// <returns>The same Section with the contents added.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contents"/>
        /// is null or empty, or <paramref name="converter"/> is null.</exception>
        public static Section Add(this Section section, string contents, IConverter converter) {
            if (string.IsNullOrEmpty(contents)) {
                throw new ArgumentNullException(nameof(contents));
            }
            if (converter == null) {
                throw new ArgumentNullException(nameof(converter));
            }

            var addAction = converter.Convert(contents);
            addAction(section);
            return section;
        }

        /// <summary>
        /// Adds the given contents with the given converter to the current HeaderFooter.
        /// </summary>
        /// <param name="headerFooter">The current HeaderFooter.</param>
        /// <param name="contents">The HTML or Markdown string to add.</param>
        /// <param name="converter">The HTML or Markdown converter.</param>
        /// <returns>The same HeaderFooter with the contents added.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contents"/>
        /// is null or empty, or <paramref name="converter"/> is null.</exception>
        public static HeaderFooter Add(this HeaderFooter headerFooter, string contents, IConverter converter) {
            if (string.IsNullOrEmpty(contents)) {
                throw new ArgumentNullException(nameof(contents));
            }
            if (converter == null) {
                throw new ArgumentNullException(nameof(converter));
            }

            var addAction = converter.ConvertHeaderFooter(contents);
            addAction(headerFooter);
            return headerFooter;
        }

        /// <summary>
        /// Adds the given contents with the given converter to the current Cell.
        /// </summary>
        /// <param name="cell">The current Cell.</param>
        /// <param name="contents">The HTML or Markdown string to add.</param>
        /// <param name="converter">The HTML or Markdown converter.</param>
        /// <returns>The same Cell with the contents added.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contents"/>
        /// is null or empty, or <paramref name="converter"/> is null.</exception>
        public static Cell Add(this Cell cell, string contents, IConverter converter) {
            if (string.IsNullOrEmpty(contents)) {
                throw new ArgumentNullException(nameof(contents));
            }
            if (converter == null) {
                throw new ArgumentNullException(nameof(converter));
            }

            var addAction = converter.ConvertCell(contents);
            addAction(cell);
            return cell;
        }

        /// <summary>
        /// Adds the given contents with the given converter to the current Paragraph.
        /// </summary>
        /// <param name="paragraph">The current Paragraph.</param>
        /// <param name="contents">The HTML or Markdown string to add.</param>
        /// <param name="converter">The HTML or Markdown converter.</param>
        /// <returns>The same Paragraph with the contents added.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="contents"/>
        /// is null or empty, or <paramref name="converter"/> is null.</exception>
        public static Paragraph Add(this Paragraph paragraph, string contents, IConverter converter) {
            if (string.IsNullOrEmpty(contents)) {
                throw new ArgumentNullException(nameof(contents));
            }
            if (converter == null) {
                throw new ArgumentNullException(nameof(converter));
            }

            var addAction = converter.ConvertParagraph(contents);
            addAction(paragraph);
            return paragraph;
        }
    }
}
