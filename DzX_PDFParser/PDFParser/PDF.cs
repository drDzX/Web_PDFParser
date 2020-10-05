using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Configuration;
using iText.Forms.Xfdf;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Data;
using iText.Kernel.Pdf.Canvas.Parser.Filter;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace DzX_PDFParser.PDFParser
{

    public class LicenseCertificate
    {
        public string CSN;
        public string CertDate;
        public string Customer;
        public string Software;
        public string Serial;
        public string ContactEmail;

    }
    public static class PDFExtract
    {

        private static FieldInfo locationalResultField = typeof(LocationTextExtractionStrategy).GetField("locationalResult", BindingFlags.NonPublic | BindingFlags.Instance);

        public static string[] ExtractText(this PdfPage page, params Rectangle[] rects)
        {
            var textEventListener = new LocationTextExtractionStrategy();
            PdfTextExtractor.GetTextFromPage(page, textEventListener);
            string[] result = new string[rects.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = textEventListener.GetResultantText(rects[i]);
            }
            return result;
        }

        public static String GetResultantText(this LocationTextExtractionStrategy strategy, Rectangle rect)
        {
            IList<TextChunk> locationalResult = (IList<TextChunk>)locationalResultField.GetValue(strategy);
            List<TextChunk> nonMatching = new List<TextChunk>();
            foreach (TextChunk chunk in locationalResult)
            {
                ITextChunkLocation location = chunk.GetLocation();
                Vector start = location.GetStartLocation();
                Vector end = location.GetEndLocation();
                if (!rect.IntersectsLine(start.Get(Vector.I1), start.Get(Vector.I2), end.Get(Vector.I1), end.Get(Vector.I2)))
                {
                    nonMatching.Add(chunk);
                }
            }
            nonMatching.ForEach(c => locationalResult.Remove(c));
            try
            {
                return strategy.GetResultantText();
            }
            finally
            {
                nonMatching.ForEach(c => locationalResult.Add(c));
            }
        }

    }
    public class PDFParser
    {
        string TempFile = WebConfigurationManager.AppSettings["TempFileLoc"];
        // public  readonly String SRC = TempFile;
        public string[] ManipulatePdf()
        {
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(TempFile));
            Rectangle rect_CSN = new Rectangle(135, 685, 150, 25);
            Rectangle rect_CertDate = new Rectangle(410, 685, 150, 25);
            Rectangle rect_Customer = new Rectangle(135, 657, 150, 15);
            Rectangle rect_Software = new Rectangle(130, 500, 150, 25);
            Rectangle rect_Serial = new Rectangle(130, 457, 150, 25);
            Rectangle rect_ContactEmail = new Rectangle(130, 313, 150, 25);
            Rectangle[] rectArray = new Rectangle[] { rect_CSN, rect_CertDate, rect_Customer, rect_Software, rect_Serial, rect_ContactEmail };
            return PDFExtract.ExtractText(pdfDoc.GetFirstPage(), rectArray);
        }
        public void Delete()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            File.Delete(TempFile);
        }
    }
}
