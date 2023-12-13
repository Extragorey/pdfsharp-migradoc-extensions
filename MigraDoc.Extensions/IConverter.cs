using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.Extensions {
    public interface IConverter {
        Action<Section> Convert(string contents);

        Action<HeaderFooter> ConvertHeaderFooter(string contents);

        Action<Cell> ConvertCell(string contents);

        Action<Paragraph> ConvertParagraph(string contents);
    }
}
