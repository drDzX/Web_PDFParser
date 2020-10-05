using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DzX_PDFParser.PDFParser;
using Newtonsoft.Json;
using System.Text;
using System.Configuration;
using System.Web.Configuration;

namespace DzX_PDFParser.Controllers
{
    public class PDFLoc
    {
        public byte[] PDFContent;
    }
    public class PdfSetController : ApiController
    {

        // POST api/pdfset
        public LicenseCertificate Post([FromBody] PDFLoc content)
        {
            PDFLoc deser = content;
            string TempFile= WebConfigurationManager.AppSettings["TempFileLoc"];
            File.WriteAllBytes(TempFile, deser.PDFContent);

            //Response
            LicenseCertificate Lic = new LicenseCertificate();
            PDFParser.PDFParser Parser = new PDFParser.PDFParser();
            string[] pdfInfo = Parser.ManipulatePdf();
            Lic.CSN = pdfInfo[0];
            Lic.CertDate = pdfInfo[1];
            Lic.Customer = pdfInfo[2];
            Lic.Software = pdfInfo[3];
            Lic.Serial = pdfInfo[4];
            Lic.ContactEmail = pdfInfo[5];

            Parser.Delete();
            return Lic;
        }
    }
}
