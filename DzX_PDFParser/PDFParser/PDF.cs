using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Configuration;

namespace DzX_PDFParser.PDFParser
{
    //Class to deserialize Json data
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
        //Local field info
        private static FieldInfo locationalResultField = typeof(LocationTextExtractionStrategy).GetField("locationalResult", BindingFlags.NonPublic | BindingFlags.Instance);

        /*Extract text from asked page inside marked rectangle
         * @page - page number to extratc from
         * @rects - array of rectangles/fields to extract text from page.
         */
        public static string[] ExtractText(this PdfPage page, params Rectangle[] rects)
        {
            //Make strategy
            var textEventListener = new LocationTextExtractionStrategy();
            //Get all text from page
            PdfTextExtractor.GetTextFromPage(page, textEventListener);

            //Make string container to handle all stored data
            string[] result = new string[rects.Length];
            //Loop all rectangles in the array
            for (int i = 0; i < result.Length; i++)
            {
                //Overrided GetResultantText
                //For each rectangle get text under the page and write it in the result arry
                result[i] = textEventListener.GetResultantText(rects[i]);
            }
            //returnt string array with results
            return result;
        }

        //Override for GetResultantText
        public static String GetResultantText(this LocationTextExtractionStrategy strategy, Rectangle rect)
        {
            //Get chunks of text from extraction strategy
            IList<TextChunk> locationalResult = (IList<TextChunk>)locationalResultField.GetValue(strategy);
            //Make container list to store chunks that do not match
            List<TextChunk> nonMatching = new List<TextChunk>();
            //For reach chunk in extraction strategy
            foreach (TextChunk chunk in locationalResult)
            {
                //Get chunk location
                ITextChunkLocation location = chunk.GetLocation();
                //Make start and end vectors
                Vector start = location.GetStartLocation();
                Vector end = location.GetEndLocation();
                //check if asked rectangle is NOT intersecting current chunk of text
                if (!rect.IntersectsLine(start.Get(Vector.I1), start.Get(Vector.I2), end.Get(Vector.I1), end.Get(Vector.I2)))
                {
                    //if rectangle is not containing this chunk add to nonMatching
                    nonMatching.Add(chunk);
                }
            }
            //For each element in nonMatching remove from locationalResult collection
            nonMatching.ForEach(c => locationalResult.Remove(c));
            try
            {
                //Try returning value if something remain
                return strategy.GetResultantText();
            }
            finally
            {
                //Return everything from non matching to localResult if there is no return value.
                nonMatching.ForEach(c => locationalResult.Add(c));
            }
        }

    }
    public class PDFParser
    {
        //Location of temp file
        string TempFile = WebConfigurationManager.AppSettings["TempFileLoc"];
        //Manipulate Autodesk License file PDF
        public string[] ManipulatePdf()
        {
            //Read temp PDF file of Autodesk License
            PdfDocument pdfDoc = new PdfDocument(new PdfReader(TempFile));
            //Create rectanglers around parameters
            Rectangle rect_CSN = new Rectangle(135, 685, 150, 25);
            Rectangle rect_CertDate = new Rectangle(410, 685, 150, 25);
            Rectangle rect_Customer = new Rectangle(135, 657, 150, 15);
            Rectangle rect_Software = new Rectangle(130, 500, 150, 25);
            Rectangle rect_Serial = new Rectangle(130, 457, 150, 25);
            Rectangle rect_ContactEmail = new Rectangle(130, 313, 150, 25);
            //Make array of rectangles
            Rectangle[] rectArray = new Rectangle[] { rect_CSN, rect_CertDate, rect_Customer, rect_Software, rect_Serial, rect_ContactEmail };
            //Read values from frist page
            return PDFExtract.ExtractText(pdfDoc.GetFirstPage(), rectArray);
        }
        //Delete temp file
        public void Delete()
        {
            //wait for actions to finish
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            //Dlete file
            File.Delete(TempFile);
        }
    }
}
