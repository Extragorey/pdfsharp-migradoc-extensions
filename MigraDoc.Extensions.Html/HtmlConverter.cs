using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;

namespace MigraDoc.Extensions.Html {
    /// <summary>
    /// Handles the conversion from HTML markup to MigraDoc objects.
    /// </summary>
    public class HtmlConverter : IConverter {
        private readonly bool _isPdf;

        /// <summary>
        /// Initialises a new HtmlConverter with default node handlers.
        /// </summary>
        /// <param name="isPdf">If false, images from &lt;img&gt; elements are first written
        /// to a temporary location before being added to the MigraDoc document model.</param>
        public HtmlConverter(bool isPdf = false) {
            _isPdf = isPdf;
            AddDefaultNodeHandlers();
        }

        /// <summary>
        /// A dictionary of handlers for all supported HTML nodes.
        /// To support additional node types, add an entry to this dictionary
        /// that returns a suitable MigraDoc object.
        /// See <see href="https://github.com/Extragorey/pdfsharp-migradoc-extensions#extending-the-html-converter">Extending the HTML converter</see>
        /// for more information.
        /// </summary>
        public IDictionary<string, Func<HtmlNode, DocumentObject, DocumentObject?>> NodeHandlers { get; }
            = new Dictionary<string, Func<HtmlNode, DocumentObject, DocumentObject?>>();

        /// <summary>
        /// Returns the action for converting the HTML
        /// <paramref name="contents"/> string into a MigraDoc Section.
        /// </summary>
        /// <param name="contents">The HTML-formatted string to convert.</param>
        /// <returns>The action for converting the HTML into a MigraDoc Section.</returns>
        public Action<Section> Convert(string contents) {
            TryDeleteAllTemporaryFiles();

            return section => ConvertHtml(contents, section);
        }

        /// <summary>
        /// Returns the action for converting the HTML
        /// <paramref name="contents"/> string into a MigraDoc HeaderFooter.
        /// </summary>
        /// <param name="contents">The HTML-formatted string to convert.</param>
        /// <returns>The action for converting the HTML into a MigraDoc HeaderFooter.</returns>
        public Action<HeaderFooter> ConvertHeaderFooter(string contents) {
            TryDeleteAllTemporaryFiles();

            return headerFooter => ConvertHtml(contents, headerFooter);
        }

        /// <summary>
        /// Returns the action for converting the HTML
        /// <paramref name="contents"/> string into a MigraDoc Cell.
        /// </summary>
        /// <param name="contents">The HTML-formatted string to convert.</param>
        /// <returns>The action for converting the HTML into a MigraDoc Cell.</returns>
        public Action<Cell> ConvertCell(string contents) {
            TryDeleteAllTemporaryFiles();

            return cell => ConvertHtml(contents, cell);
        }

        /// <summary>
        /// Returns the action for converting the HTML
        /// <paramref name="contents"/> string into a MigraDoc Paragraph.
        /// </summary>
        /// <param name="contents">The HTML-formatted string to convert.</param>
        /// <returns>The action for converting the HTML into a MigraDoc Paragraph.</returns>
        public Action<Paragraph> ConvertParagraph(string contents) {
            TryDeleteAllTemporaryFiles();

            return paragraph => ConvertHtml(contents, paragraph);
        }

        private void ConvertHtml(string html, Section section) {
            if (html is null) {
                throw new ArgumentNullException(nameof(html));
            }

            if (section == null) {
                throw new ArgumentNullException(nameof(section));
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            ConvertHtmlNodes(doc.DocumentNode.ChildNodes, section);
        }

        private void ConvertHtml(string html, HeaderFooter headerFooter) {
            if (html is null) {
                throw new ArgumentNullException(nameof(html));
            }

            if (headerFooter == null) {
                throw new ArgumentNullException(nameof(headerFooter));
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            ConvertHtmlNodes(doc.DocumentNode.ChildNodes, headerFooter);
        }

        private void ConvertHtml(string html, Cell cell) {
            if (string.IsNullOrEmpty(html)) {
                throw new ArgumentNullException(nameof(html));
            }

            if (cell == null) {
                throw new ArgumentNullException(nameof(cell));
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            ConvertHtmlNodes(doc.DocumentNode.ChildNodes, cell);
        }

        private void ConvertHtml(string html, Paragraph paragraph) {
            if (string.IsNullOrEmpty(html)) {
                throw new ArgumentNullException(nameof(html));
            }

            if (paragraph == null) {
                throw new ArgumentNullException(nameof(paragraph));
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            ConvertHtmlNodes(doc.DocumentNode.ChildNodes, paragraph);
        }

        private void ConvertHtmlNodes(HtmlNodeCollection nodes, DocumentObject section, DocumentObject? current = null) {
            foreach (var node in nodes) {
                if (NodeHandlers.TryGetValue(node.Name, out var nodeHandler)) {
                    // pass the current container or section
                    var result = nodeHandler(node, current ?? section);

                    if (node.HasChildNodes) {
                        ConvertHtmlNodes(node.ChildNodes, section, result);
                    }
                } else {
                    if (node.HasChildNodes) {
                        ConvertHtmlNodes(node.ChildNodes, section, current);
                    }
                }
            }
        }

        private void AddDefaultNodeHandlers() {
            // Block Elements

            // could do with a predicate/regex matcher so we could just use one handler for all headings
            NodeHandlers.Add("h1", AddHeading);
            NodeHandlers.Add("h2", AddHeading);
            NodeHandlers.Add("h3", AddHeading);
            NodeHandlers.Add("h4", AddHeading);
            NodeHandlers.Add("h5", AddHeading);
            NodeHandlers.Add("h6", AddHeading);

            NodeHandlers.Add("p", (node, parent) => {
                Paragraph? p = null;

                if (parent is Paragraph paragraph) {
                    p = paragraph;
                }

                if (parent is Cell cell) {
                    p = cell.AddParagraph();
                }

                if (parent is Row) {
                    return parent;
                }

                if (parent is HeaderFooter headerFooter) {
                    p = headerFooter.AddParagraph();
                }

                if (parent is Section section) {
                    p = section.AddParagraph();
                }

                if (p == null) {
                    return p;
                }

                var format = new ParagraphFormat();

                var attr = node.Attributes["align"];
                if (attr != null) {
                    var input = ToPascalCase(attr.Value.ToLower());
                    Enum.TryParse(input, out ParagraphAlignment result);

                    format.Alignment = result;
                }

                var attrStyle = node.Attributes["style"];
                if (attrStyle != null) {
                    var values = attrStyle.Value.Split(';');

                    foreach (var item in values) {
                        var val = item.Split(':');

                        if (val[0].Contains("margin-left")) {
                            format.LeftIndent = val[1].Replace("px", "");
                        }

                        if (val[0].Contains("margin-right")) {
                            format.RightIndent = val[1].Replace("px", "");
                        }

                        if (val[0].ToLower().Contains("text-align")) {
                            var align = val[1].ToLower();
                            if (align.Contains("right")) {
                                format.Alignment = ParagraphAlignment.Right;
                            } else if (align.Contains("justify")) {
                                format.Alignment = ParagraphAlignment.Justify;
                            } else if (align.Contains("center")) {
                                format.Alignment = ParagraphAlignment.Center;
                            } else if (align.Contains("left")) {
                                format.Alignment = ParagraphAlignment.Left;
                            }
                        }
                    }
                }

                p.Format = format;

                return p;
            });

            //NodeHandlers.Add("div", (node, parent) => {
            //    Paragraph p = null;

            //    if (parent is Paragraph paragraph) {
            //        p = paragraph;
            //    }

            //    if (parent is Cell cell) {
            //        p = cell.AddParagraph();
            //    }

            //    if (parent is Row) {
            //        return parent;
            //    }

            //    if (parent is HeaderFooter headerFooter) {
            //        p = headerFooter.AddParagraph();
            //    }

            //    if (parent is Section section) {
            //        p = section.AddParagraph();
            //    }

            //    var format = new ParagraphFormat();

            //    var attr = node.Attributes["align"];
            //    if (attr != null) {
            //        var input = ToPascalCase(attr.Value.ToLower());
            //        Enum.TryParse(input, out ParagraphAlignment result);

            //        format.Alignment = result;
            //    }

            //    var attrStyle = node.Attributes["style"];
            //    if (attrStyle != null) {
            //        var values = attrStyle.Value.Split(';');

            //        foreach (var item in values) {
            //            var val = item.Split(':');

            //            if (val[0].Contains("margin-left")) {
            //                format.LeftIndent = val[1].Replace("px", "");
            //            }

            //            if (val[0].Contains("margin-right")) {
            //                format.RightIndent = val[1].Replace("px", "");
            //            }

            //            if (val[0].ToLower().Contains("text-align")) {
            //                var align = val[1].ToLower();
            //                if (align.Contains("right")) {
            //                    format.Alignment = ParagraphAlignment.Right;
            //                } else if (align.Contains("justify")) {
            //                    format.Alignment = ParagraphAlignment.Justify;
            //                } else if (align.Contains("center")) {
            //                    format.Alignment = ParagraphAlignment.Center;
            //                } else if (align.Contains("left")) {
            //                    format.Alignment = ParagraphAlignment.Left;
            //                }
            //            }
            //        }
            //    }

            //    p.Format = format;

            //    return p;
            //});

            string ToPascalCase(string text) {
                return CultureInfo.InvariantCulture.TextInfo
                    .ToTitleCase(text.ToLowerInvariant())
                    .Replace("-", "")
                    .Replace("_", "");
            }


            // Inline Elements

            NodeHandlers.Add("strong", (node, parent) => AddFormattedText(node, parent, TextFormat.Bold));
            NodeHandlers.Add("b", (node, parent) => AddFormattedText(node, parent, TextFormat.Bold));
            NodeHandlers.Add("i", (node, parent) => AddFormattedText(node, parent, TextFormat.Italic));
            NodeHandlers.Add("em", (node, parent) => AddFormattedText(node, parent, TextFormat.Italic));
            NodeHandlers.Add("u", (node, parent) => AddFormattedText(node, parent, TextFormat.Underline));
            NodeHandlers.Add("a", (node, parent) => {
                if (parent is FormattedText formattedText) {
                    return formattedText.AddHyperlink(node.GetAttributeValue("href", ""), HyperlinkType.Web);
                }

                return GetParagraph(parent).AddHyperlink(node.GetAttributeValue("href", ""), HyperlinkType.Web);
            });
            NodeHandlers.Add("hr", (node, parent) => GetParagraph(parent).SetStyle("HorizontalRule"));
            NodeHandlers.Add("br", (node, parent) => {
                if (parent is FormattedText formattedText) {
                    // inline elements can contain line breaks
                    formattedText.AddLineBreak();
                    return parent;
                }

                var paragraph = GetParagraph(parent);
                paragraph.AddLineBreak();
                return paragraph;
            });

            NodeHandlers.Add("li", (node, parent) => {
                var listStyle = node.ParentNode.Name == "ul"
                    ? "UnorderedList"
                    : "OrderedList";

                //var section = (Section)parent;                
                var isFirst = node.ParentNode.Elements("li").First() == node;
                var isLast = node.ParentNode.Elements("li").Last() == node;

                // if this is the first item add the ListStart paragraph
                if (isFirst) {
                    if (parent is Cell cell) {
                        cell.AddParagraph().SetStyle("ListStart");
                    } else if (parent is Paragraph paragraph) {
                        paragraph.SetStyle("ListStart");
                    } else if (parent is HeaderFooter headerFooter) {
                        headerFooter.AddParagraph().SetStyle("ListStart");
                    } else {
                        ((Section)parent).AddParagraph().SetStyle("ListStart");
                    }
                }

                var listItem = parent is Cell ? ((Cell)parent).AddParagraph().SetStyle(listStyle) :
                    parent is Paragraph ? ((Paragraph)parent).SetStyle(listStyle) :
                    parent is HeaderFooter ? ((HeaderFooter)parent).AddParagraph().SetStyle(listStyle) :
                    ((Section)parent).AddParagraph().SetStyle(listStyle);

                // ToDo (RD 12/26/2019): Temporary default color 
                if (node.ParentNode.Name == "ol") {
                    listItem.Format.ListInfo.ListType = ListType.NumberList1;
                }
                listItem.Format.Font.Color = Colors.Black;

                // disable continuation if this is the first list item
                listItem.Format.ListInfo.ContinuePreviousList = !isFirst;

                // if this is the last item, add the ListEnd paragraph
                if (isLast) {
                    if (parent is Cell cell) {
                        cell.AddParagraph().SetStyle("ListEnd");
                    } else if (parent is Paragraph paragraph) {
                        paragraph.SetStyle("ListEnd");
                    } else if (parent is HeaderFooter headerFooter) {
                        headerFooter.AddParagraph().SetStyle("ListEnd");
                    } else {
                        ((Section)parent).AddParagraph().SetStyle("ListEnd");
                    }
                }

                return listItem;
            });

            NodeHandlers.Add("#text", (node, parent) => {
                if (parent is Table || parent is Row) {
                    node.InnerHtml = string.Empty;
                    return parent;
                }

                if (parent is Paragraph p && node.ParentNode.Name == "h2") {
                    p.Format.Font.Color = Colors.Black;
                }

                // remove line breaks
                var innerText = node.InnerText.Replace("\r", "").Replace("\n", "");

                //if (string.IsNullOrWhiteSpace(innerText))
                if (innerText is null) {
                    return parent;
                }

                // decode escaped HTML
                innerText = WebUtility.HtmlDecode(innerText);

                // text elements must be wrapped in a paragraph but this could also be FormattedText or a Hyperlink!!
                // this needs some work
                if (parent is FormattedText || parent is Paragraph) {
                    return TryAddFormattedText(innerText, node, parent);
                }

                if (parent is Cell cell) {
                    return TryAddFormattedText(innerText, node, GetCellParagraph(cell));
                }

                if (parent is HeaderFooter headerFooter) {
                    return TryAddFormattedText(innerText, node, GetHeaderFooterParagraph(headerFooter));
                }

                if (parent is Section section) {
                    return TryAddFormattedText(innerText, node, GetSectionParagraph(section));
                }

                if (parent is Hyperlink hyperlink) {
                    return hyperlink.AddText(innerText);
                }

                // otherwise a section or paragraph
                return GetParagraph(parent).AddText(innerText);
            });

            // Note that the current implementation only supports base-64 encoded images
            NodeHandlers.Add("img", (node, parent) => {
                //const double point = 0.75;

                var source = "base64:" + Regex.Replace(node.Attributes["src"].Value, "^.+?(;base64),", "");
                var base64 = Regex.Replace(node.Attributes["src"].Value, "^.+?(;base64),", "");

                base64 = base64.Replace(" ", "");

                if (!_isPdf) {
                    var filename = node.Attributes["data-filename"]?.Value;
                    if (string.IsNullOrWhiteSpace(filename)) {
                        source = Path.GetTempPath() + "temp_" + DateTime.UtcNow.ToString("MMddyyyy_HHmmss") + ".jpg";
                    } else {
                        source = Path.GetTempPath() + filename.Split('.')[0] + "_" +
                            DateTime.UtcNow.ToString("MMddyyyy_HHmmss") + "." + filename.Split('.')[1];
                    }

                    if (!File.Exists(source)) {
                        //File.Delete(source);
                        File.WriteAllBytes(source, System.Convert.FromBase64String(base64));
                    }
                }

                Image? img = null;

                if (string.IsNullOrWhiteSpace(source)) {
                    return null;
                }

                if (parent is Paragraph paragraph) {
                    img = paragraph.AddImage(source);
                } else if (parent is Cell cell) {
                    img = cell.AddImage(source);
                } else {
                    img = ((Section)parent).AddImage(source);
                }

                if (node.Attributes.Contains("style") &&
                    (node.Attributes["style"].Value.Contains("width") ||
                    node.Attributes["style"].Value.Contains("height"))
                ) {
                    var styles = node.Attributes["style"].Value.Split(';');

                    foreach (var style in styles) {
                        if (style.Contains("width:")) {
                            //var width = style.Split(':');

                            //img.Width = point * double.Parse(width[1].Trim().Replace("px", ""));
                            img.Width = SetUnit(style);
                        }

                        if (style.Contains("height:")) {
                            //var height = style.Split(':');

                            //img.Height = point * double.Parse(height[1].Trim().Replace("px", ""));
                            img.Height = SetUnit(style);
                        }

                        if (style.Contains("float:")) {
                            var floating = style.Split(':');

                            if (floating[1].Contains("left")) {
                                img.Left = LeftPosition.Parse("Left");
                                img.WrapFormat.Style = WrapStyle.Through;
                            } else if (floating[1].Contains("right")) {
                                img.Left = LeftPosition.Parse("Right");
                                img.WrapFormat.Style = WrapStyle.Through;
                            }
                        }

                    }
                } else {
                    if (node.Attributes["width"] != null) {
                        img.Width = node.Attributes["width"].Value;
                    }

                    if (node.Attributes["height"] != null) {
                        img.Height = node.Attributes["height"].Value;
                    }
                }

                return img;
            });

            NodeHandlers.Add("table", (node, parent) => {
                var columns = node.ChildNodes["tbody"].ChildNodes[0].ChildNodes.Count;
                Table table;
                double unit;
                const double DefaultTableWidth = 16;
                if (parent is Cell cell) {
                    table = cell.Elements.AddTable();
                    unit = (cell.Column?.Width.Value ?? DefaultTableWidth) / columns;
                } else {
                    table = ((Section)parent).AddTable();
                    unit = DefaultTableWidth / columns;
                }

                for (var i = 0; i < columns; i++) {
                    table.AddColumn(Unit.FromCentimeter(unit));
                }

                table.Borders.Width = 0.5;

                return table;
            });

            NodeHandlers.Add("tr", (node, parent) => {
                return ((Table)parent).AddRow();
            });

            NodeHandlers.Add("td", (node, parent) => {
                var i = 0;

                for (var x = 0; x < node.ParentNode.ChildNodes.Count; x++) {
                    if (node == node.ParentNode.ChildNodes[x]) {
                        i = x;
                        break;
                    }
                }

                return ((Row)parent).Cells[i];
            });
        }

        private static DocumentObject AddFormattedText(HtmlNode node, DocumentObject parent, TextFormat format) {
            if (parent is FormattedText formattedText) {
                return formattedText.Format(format);
            }

            if (parent is Hyperlink hyperlink) {
                return hyperlink.AddFormattedText(format);
            }

            if (parent is Cell cell) {
                return GetCellParagraph(cell);
            }

            if (parent is HeaderFooter headerFooter) {
                return GetHeaderFooterParagraph(headerFooter);
            }

            if (parent is Section section) {
                return GetSectionParagraph(section);
            }

            // otherwise parent is paragraph or section
            return GetParagraph(parent).AddFormattedText(format);
        }

        private static DocumentObject AddHeading(HtmlNode node, DocumentObject parent) {
            if (parent is Cell cell) {
                return cell.AddParagraph().SetStyle("Heading" + node.Name[1]);
            }

            if (parent is HeaderFooter headerFooter) {
                return headerFooter.AddParagraph().SetStyle("Heading" + node.Name[1]);
            }

            if (parent is Paragraph paragraph) {
                return paragraph.SetStyle("Heading" + node.Name[1]);
            }

            return ((Section)parent).AddParagraph().SetStyle("Heading" + node.Name[1]);
        }

        private static Paragraph GetParagraph(DocumentObject parent) {
            if (parent is Paragraph paragraph) {
                return paragraph;
            }

            if (parent is Cell cell) {
                return cell.AddParagraph();
            }

            if (parent is HeaderFooter headerFooter) {
                return headerFooter.AddParagraph();
            }

            //if (parent is FormattedText) {
            //    return ((FormattedText)parent);
            //}

            return ((Section)parent).AddParagraph();
        }

        private static Paragraph AddParagraphWithStyle(DocumentObject parent, string style) {
            if (parent is Cell cell) {
                return cell.AddParagraph().SetStyle(style);
            }

            if (parent is HeaderFooter headerFooter) {
                return headerFooter.AddParagraph().SetStyle(style);
            }

            return ((Section)parent).AddParagraph().SetStyle(style);
        }

        private void TryDeleteAllTemporaryFiles() {
            var supportedExtensions = "*.jpg,*.gif,*.png,*.bmp,*.jpe,*.jpeg,*.wmf,*.emf,*.xbm,*.ico,*.eps,*.tif,*.tiff,*.g01,*.g02,*.g03,*.g04,*.g05,*.g06,*.g07,*.g08";

            foreach (var path in Directory.GetFiles(Path.GetTempPath(), "*.*", SearchOption.TopDirectoryOnly).Where(s => supportedExtensions.Contains(Path.GetExtension(s).ToLower()))) {
                try {
                    var file = new FileInfo(path);

                    if (file.CreationTimeUtc < DateTime.UtcNow.AddMinutes(-15)) {
                        file.Delete();
                    }
                } catch (Exception) {
                    continue;
                }
            }
        }

        private FormattedText TryAddFormattedText(string innerText, HtmlNode node, DocumentObject parent) {
            var font = parent is Paragraph ? ((Paragraph)parent).Format.Font : ((FormattedText)parent).Font;

            font.Bold = false;
            font.Italic = false;
            font.Underline = Underline.None;

            var xmlNode = node.ParentNode;

            while (xmlNode.Name != "#document") {
                switch (xmlNode.Name) {
                    case "u":
                        font.Underline = Underline.Single;
                        break;
                    case "b":
                    case "strong":
                        font.Bold = true;
                        break;
                    case "i":
                    case "em":
                        font.Italic = true;
                        break;
                }

                xmlNode = xmlNode.ParentNode;
            }

            return parent is Paragraph ?
                ((Paragraph)parent).AddFormattedText(innerText, font) :
                ((FormattedText)parent).AddFormattedText(innerText, font);
        }


        /// <summary>
        /// Gets or adds the last paragraph in the given table cell.
        /// </summary>
        /// <param name="source">The table cell.</param>
        /// <returns>A Paragraph object in the given cell.</returns>
        private static Paragraph GetCellParagraph(Cell source) {
            for (var i = source.Elements.Count; i > 0; i--) {
                if (source.Elements[i - 1] is Paragraph paragraph) {
                    return paragraph;
                }
            }

            return source.AddParagraph();
        }

        /// <summary>
        /// Gets or adds the last paragraph in the given HeaderFooter section.
        /// </summary>
        /// <param name="source">The HeaderFooter section.</param>
        /// <returns>A Paragraph object in the given section.</returns>
        private static Paragraph GetHeaderFooterParagraph(HeaderFooter source) {
            for (var i = source.Elements.Count; i > 0; i--) {
                if (source.Elements[i - 1] is Paragraph paragraph) {
                    return paragraph;
                }
            }

            return source.AddParagraph();
        }

        /// <summary>
        /// Gets or adds the last paragraph in the given Section.
        /// </summary>
        /// <param name="source">The section.</param>
        /// <returns>A Paragraph object in the given section.</returns>
        private static Paragraph GetSectionParagraph(Section source) {
            for (var i = source.Elements.Count; i > 0; i--) {
                if (source.Elements[i - 1] is Paragraph paragraph) {
                    return paragraph;
                }
            }

            return source.AddParagraph();
        }

        private static double SetUnit(string styleValue) {
            const double point = 0.75;
            const double maxPx = 600.0;
            var style = styleValue.Split(':');
            var px = double.Parse(style[1].Trim().Replace("px", ""));

            if (px > maxPx && style[0].Contains("width")) {
                return point * maxPx;
            }

            return point * px;
        }
    }
}
