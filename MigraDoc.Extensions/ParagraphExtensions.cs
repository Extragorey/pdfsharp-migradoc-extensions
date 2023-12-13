using MigraDoc.DocumentObjectModel;

namespace MigraDoc.Extensions {
    /// <summary>
    /// Defines extension methods for MigraDoc paragraph objects.
    /// </summary>
    public static class ParagraphExtensions {
        /// <summary>
        /// Assigns the given <paramref name="style"/> to the Paragraph.
        /// </summary>
        /// <param name="paragraph">The current Paragraph.</param>
        /// <param name="style">The style to assign.</param>
        /// <returns>The same Paragraph with the style assigned.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="style"/> is null.</exception>
        public static Paragraph SetStyle(this Paragraph paragraph, string style) {
            if (paragraph == null) {
                throw new ArgumentNullException(nameof(paragraph));
            }
            if (string.IsNullOrEmpty(style)) {
                throw new ArgumentNullException(nameof(style));
            }

            paragraph.Style = style;
            return paragraph;
        }
    }
}
