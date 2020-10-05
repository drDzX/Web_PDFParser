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
    public class PdfController : ApiController
    {
        // GET api/pdf
        public LicenseCertificate Get()
        {
            LicenseCertificate Lic = new LicenseCertificate();
            PDFParser.PDFParser Parser = new PDFParser.PDFParser();
            Lic.CSN = Parser.ManipulatePdf()[0];
            Lic.CertDate = Parser.ManipulatePdf()[1];
            Lic.Customer = Parser.ManipulatePdf()[2]; 
            Lic.Software = Parser.ManipulatePdf()[3]; 
            Lic.Serial = Parser.ManipulatePdf()[4]; 
            Lic.ContactEmail = Parser.ManipulatePdf()[5]; 

            Parser.Delete();
            return Lic;
        }

    }
}
