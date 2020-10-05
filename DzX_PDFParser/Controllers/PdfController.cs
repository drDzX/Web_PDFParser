using DzX_PDFParser.PDFParser;
using iText.Kernel.Geom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace DzX_PDFParser.Controllers
{
    public class PdfController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return  PDFParser.PDFParser.ManipulatePdf();
        }

    }
}
