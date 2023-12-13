using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.Extensions {
    /// <summary>
    /// The interface for all converters.
    /// </summary>
    public interface IConverter {
        /// <summary>
        /// Returns the action for converting <paramref name="contents"/> into a MigraDoc Section.
        /// </summary>
        /// <param name="contents">The formatted string to convert.</param>
        /// <returns>The action for converting <paramref name="contents"/> into a MigraDoc Section.</returns>
        Action<Section> Convert(string contents);

        /// <summary>
        /// Returns the action for converting <paramref name="contents"/> into a MigraDoc HeaderFooter.
        /// </summary>
        /// <param name="contents">The formatted string to convert.</param>
        /// <returns>The action for converting <paramref name="contents"/> into a MigraDoc HeaderFooter.</returns>
        Action<HeaderFooter> ConvertHeaderFooter(string contents);

        /// <summary>
        /// Returns the action for converting <paramref name="contents"/> into a MigraDoc Cell.
        /// </summary>
        /// <param name="contents">The formatted string to convert.</param>
        /// <returns>The action for converting <paramref name="contents"/> into a MigraDoc Cell.</returns>
        Action<Cell> ConvertCell(string contents);

        /// <summary>
        /// Returns the action for converting <paramref name="contents"/> into a MigraDoc Paragraph.
        /// </summary>
        /// <param name="contents">The formatted string to convert.</param>
        /// <returns>The action for converting <paramref name="contents"/> into a MigraDoc Paragraph.</returns>
        Action<Paragraph> ConvertParagraph(string contents);
    }
}
