using MigraDoc.DocumentObjectModel;

namespace MigraDoc.Extensions {
    /// <summary>
    /// Defines extension methods for MigraDoc FormattedText objects.
    /// </summary>
    public static class FormattedTextExtensions {
        private static Dictionary<TextFormat, Action<FormattedText>> formats =
            new Dictionary<TextFormat, Action<FormattedText>> {
                { TextFormat.Bold, text => text.Bold = true },
                { TextFormat.NotBold, text => text.Bold = false },
                { TextFormat.Italic, text => text.Italic = true },
                { TextFormat.NotItalic, text => text.Italic = false },
                { TextFormat.Underline, text => text.Underline = Underline.Single },
                { TextFormat.NoUnderline, text => text.Underline = Underline.None }
            };

        /// <summary>
        /// Applies the given <paramref name="textFormat"/> to the current FormattedText.
        /// </summary>
        /// <param name="formattedText">The current FormattedText.</param>
        /// <param name="textFormat">The TextFormat to apply, e.g. Bold or Italic.</param>
        /// <returns>The same FormattedText with the TextFormat applied.</returns>
        public static FormattedText Format(this FormattedText formattedText, TextFormat textFormat) {
            if (formattedText == null) {
                throw new ArgumentNullException(nameof(formattedText));
            }

            if (formats.TryGetValue(textFormat, out Action<FormattedText>? formatter)) {
                formatter(formattedText);
            }

            return formattedText;
        }
    }
}
