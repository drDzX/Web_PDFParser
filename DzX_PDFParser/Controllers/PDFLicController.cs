using DzX_PDFParser.PDFParser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Configuration;
using System.Web.Http;

namespace DzX_PDFParser.Controllers
{
    public class PDFLicController : ApiController
    {
        //Local class to contain PDF byte data
        public class PDFLoc
        {
            public byte[] PDFContent;
        }
        /* POST api/pdflic
         *
         *In value:
         *@content - json with format that contain PDFContent value with binary entry for PDF file.
         * 
         * Return
         * Json with asked parameters for this class;
         */
        public LicenseCertificate Post([FromBody] PDFLoc content)
        {
            //Create class from json data #Optimize - maybe not needed
            PDFLoc deser = content;
            //Read temp location from web.config
            string TempFile = WebConfigurationManager.AppSettings["TempFileLoc"];
            //Make temp PDF file with data from the inpout value
            File.WriteAllBytes(TempFile, deser.PDFContent);

            //Response

            //Create instanced class that contain Autodesk License certificate values
            LicenseCertificate Lic = new LicenseCertificate();
            //Create instanced class of PDF parser
            PDFParser.PDFParser Parser = new PDFParser.PDFParser();
            //Get values from PDFParser for Autodesk Lic file
            string[] pdfInfo = Parser.ManipulatePdf();
            //Write down returned strings into parameters of asked class
            Lic.CSN = pdfInfo[0];
            Lic.CertDate = pdfInfo[1];
            Lic.Customer = pdfInfo[2];
            Lic.Software = pdfInfo[3];
            Lic.Serial = pdfInfo[4];
            Lic.ContactEmail = pdfInfo[5];
            //Delete temp PDF file
            Parser.Delete();
            //Return class with inputed data
            return Lic;
        }
    }
}
