using System.Diagnostics;
using MigraDoc.DocumentObjectModel;
using MigraDoc.Extensions.Html;
using MigraDoc.Extensions.Markdown;
using MigraDoc.Rendering;
using PdfSharp.Fonts;
using PdfSharp.Snippets.Font;

namespace MigraDoc.Extensions.Examples {
    internal class Program {
        static void Main(string[] args) {
            new Program().Run();
        }

        private string outputName = "output.pdf";

        internal void Run() {
            if (File.Exists(outputName)) {
                File.Delete(outputName);
            }

            var doc = new Document();
            StyleDocument(doc);
            var section = doc.AddSection();

            var pageSetup = doc.DefaultPageSetup.Clone();
            pageSetup.TopMargin = "3cm";
            pageSetup.BottomMargin = "4cm";
            section.PageSetup = pageSetup;

            section.Headers.Primary.AddMarkdown("**This** is a *header* rendered by Markdown.");
            section.Headers.Primary.AddMarkdown("*Another* header line.");

            var html = File.ReadAllText("example.html");
            section.AddHtml(html);

            section.AddPageBreak();

            var markdown = File.ReadAllText("example.md");
            section.AddMarkdown(markdown);

            section.AddPageBreak();

            section.AddParagraph().AddMarkdown("A **Markdown** formatted *paragraph*.");

            var table = section.AddTable();
            table.AddColumn("10cm");
            table.AddColumn("8cm");

            var row = table.AddRow();
            row.Cells[0].AddMarkdown(markdown);
            row.Cells[1].AddParagraph("Second cell");

            section.Footers.Primary.AddMarkdown("""
                Footer lorem ipsum dolor sit amet, consectetur adipiscing elit. Pellentesque laoreet nibh sit amet lacus ultrices in mollis sapien eleifend.
                Mauris congue luctus elit sit amet elementum. Integer nisl sapien, tristique et venenatis nec, auctor ac risus. Pellentesque et nisl eu risus pretium elementum.

                This&nbsp;
                *should*&nbsp;
                collapse&nbsp;
                to&nbsp;
                **one**&nbsp;
                line
                """);

            var renderer = new PdfDocumentRenderer();
            renderer.Document = doc;
            renderer.RenderDocument();

            renderer.Save(outputName);

            var psi = new ProcessStartInfo {
                FileName = outputName,
                UseShellExecute = true,
            };
            Process.Start(psi);
        }

        private static void StyleDocument(Document doc) {
            Color green = new(108, 179, 63),
                  brown = new(88, 71, 76),
                  lightbrown = new(150, 132, 126);

            var body = doc.Styles.Normal;
            body.Font.Size = Unit.FromPoint(10);
            body.Font.Color = new Color(51, 51, 51);

            body.ParagraphFormat.LineSpacingRule = LineSpacingRule.Multiple;
            body.ParagraphFormat.LineSpacing = 1.25;
            body.ParagraphFormat.SpaceAfter = 10;

            var footer = doc.Styles[StyleNames.Footer]!;
            footer.Font.Size = Unit.FromPoint(9);
            footer.Font.Color = lightbrown;

            var h1 = doc.Styles[StyleNames.Heading1]!;
            h1.Font.Color = brown;
            h1.Font.Bold = true;
            h1.Font.Size = Unit.FromPoint(15);

            var h2 = doc.Styles[StyleNames.Heading2]!;
            h2.Font.Color = green;
            h2.Font.Bold = true;
            h2.Font.Size = Unit.FromPoint(13);

            var h3 = doc.Styles[StyleNames.Heading3]!;
            h3.Font.Bold = true;
            h3.Font.Color = Colors.Black;
            h3.Font.Size = Unit.FromPoint(11);

            var links = doc.Styles[StyleNames.Hyperlink]!;
            links.Font.Color = green;

            var unorderedlist = doc.AddStyle("UnorderedList", StyleNames.Normal);
            var listInfo = new ListInfo();
            listInfo.ListType = ListType.BulletList1;
            unorderedlist.ParagraphFormat.ListInfo = listInfo;
            unorderedlist.ParagraphFormat.LeftIndent = "1cm";
            unorderedlist.ParagraphFormat.FirstLineIndent = "-0.5cm";
            unorderedlist.ParagraphFormat.SpaceAfter = 0;

            var orderedlist = doc.AddStyle("OrderedList", "UnorderedList");
            orderedlist.ParagraphFormat.ListInfo.ListType = ListType.NumberList1;

            // for list spacing (since MigraDoc doesn't provide a list object that we can target)
            var listStart = doc.AddStyle("ListStart", StyleNames.Normal);
            listStart.ParagraphFormat.SpaceAfter = 0;
            listStart.ParagraphFormat.LineSpacing = 0.5;
            var listEnd = doc.AddStyle("ListEnd", "ListStart");
            listEnd.ParagraphFormat.LineSpacing = 1;

            var hr = doc.AddStyle("HorizontalRule", StyleNames.Normal);
            var hrBorder = new Border();
            hrBorder.Width = "1pt";
            hrBorder.Color = Colors.DarkGray;
            hr.ParagraphFormat.Borders.Bottom = hrBorder;
            hr.ParagraphFormat.LineSpacing = 0;
            hr.ParagraphFormat.SpaceBefore = 15;
        }
    }
}
