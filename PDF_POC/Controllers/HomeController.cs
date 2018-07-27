using iTextSharp.text;
using iTextSharp.text.pdf;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PDF_POC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult GetPdf()
        {
            var html = @"
            <html>
                <body>
                    <h1>Titulo</h1>
                </body>
            </html>
            ";
            var builder = new PdfBuilder(html);
            return builder.GetPdf();
        }
    }

    internal class PdfBuilder
    {
        string html = string.Empty;

        public PdfBuilder(string html)
        {
            this.html = html;
        }

        public FileContentResult GetPdf()
        {
            var html = GetHtml();
            Byte[] bytes;
            using (var ms = new MemoryStream())
            {
                using (var doc = new Document())
                {
                    using (var writer = PdfWriter.GetInstance(doc, ms))
                    {
                        doc.Open();
                        try
                        {
                            using (var msHtml = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(html)))
                            {
                                iTextSharp.tool.xml.XMLWorkerHelper.GetInstance()
                                    .ParseXHtml(writer, doc, msHtml, System.Text.Encoding.UTF8);
                            }
                        }
                        finally
                        {
                            doc.Close();
                        }
                    }
                }
                bytes = ms.ToArray();
            }
            return new FileContentResult(bytes, "application/pdf");
        }

        private string GetHtml()
        {            
            var model = new { };
            return Engine.Razor.RunCompile(html, new Guid().ToString(), null, model);
        }
    }
}