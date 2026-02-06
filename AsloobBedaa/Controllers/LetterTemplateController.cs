using AsloobBedaa.DataContext;
using AsloobBedaa.Models.Letter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace AsloobBedaa.Controllers
{
    public class LetterTemplateController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LetterTemplateController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Index()
        {
            var data = _context.LetterTemplates
                .Include(x => x.LetterType)
                .OrderByDescending(x => x.CreatedDate)
                .ToList();
            return View(data);
        }

        // GET
        public IActionResult Create()
        {
            ViewBag.LetterTypes = _context.LetterTypes.ToList();
            return View();
        }

        // POST
        [HttpPost]
        public IActionResult Create(LetterTemplateVM model)
        {
            var template = new LetterTemplate
            {
                LetterTypeId = model.LetterTypeId,
                Title = model.Title,
                CreatedBy = User.Identity.Name ?? "Admin",
                CreatedDate = DateTime.Now
            };
            _context.LetterTemplates.Add(template);
            _context.SaveChanges();

            foreach (var sec in model.Sections)
            {
                sec.LetterTemplateId = template.Id;
                _context.LetterTemplateSections.Add(sec);
            }
            _context.SaveChanges();

            return RedirectToAction("Preview", new { id = template.Id });
        }

        // PREVIEW
        public IActionResult Preview(int id)
        {
            var data = _context.LetterTemplates
                .Include(x => x.Sections)
                .Include(x => x.LetterType)
                .FirstOrDefault(x => x.Id == id);

            if (data == null)
                return NotFound();

            return View(data);
        }

        // ===================== PDF GENERATION (EXACT MATCH) =====================

        /// <summary>
        /// Generate PDF matching exact offer letter design
        /// </summary>
        [HttpGet]
        public IActionResult PrintPDF(int id)
        {
            try
            {
                var data = _context.LetterTemplates
                    .Include(x => x.Sections)
                    .Include(x => x.LetterType)
                    .FirstOrDefault(x => x.Id == id);

                if (data == null)
                    return NotFound();

                var pdfBytes = GenerateOfferLetterPDF(data);

                var fileName = $"{data.Title.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf";
                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to generate PDF. Please try again.";
                return RedirectToAction("Preview", new { id });
            }
        }

        /// <summary>
        /// Generate PDF exactly matching the offer letter design
        /// </summary>  
        private byte[] GenerateOfferLetterPDF(LetterTemplate template)
        {
            using var ms = new MemoryStream();

            // A4 Portrait with margins matching the image
            var pageSize = PageSize.A4;
            var doc = new Document(pageSize, 40f, 40f, 30f, 70f);

            var writer = PdfWriter.GetInstance(doc, ms);
            writer.PageEvent = new OfferLetterFooterHelper(template.Id, template.CreatedBy);

            doc.Open();

            // ================ FONT SETUP ================
            var fontPath = GetFontPath();
            BaseFont baseFont;

            if (fontPath == BaseFont.HELVETICA)
            {
                baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
            }
            else
            {
                baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            }

            // Fonts matching the image
            Font refFont = new Font(baseFont, 9, Font.NORMAL, BaseColor.Black);
            Font titleFont = new Font(baseFont, 14, Font.BOLD, BaseColor.Black);
            Font boldFont = new Font(baseFont, 10, Font.BOLD, BaseColor.Black);
            Font normalFont = new Font(baseFont, 10, Font.NORMAL, BaseColor.Black);
            Font underlineFont = new Font(baseFont, 10, Font.BOLD | Font.UNDERLINE, BaseColor.Black);

            // ================ TOP HEADER (Logo + Date) ================
            var logoPath = Path.Combine(_env.WebRootPath, "images", "header_logo.jpg");

            if (System.IO.File.Exists(logoPath))
            {
                // Logo
                var logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleToFit(120f, 40f);
                logo.Alignment = Element.ALIGN_RIGHT;
                doc.Add(logo);

                // Add spacing between logo and date
                doc.Add(new Paragraph("\n"));

                // Date below logo (centered)
                var dateText = $"Date: {DateTime.Now:dd/MM/yyyy}";
                var datePara = new Paragraph(dateText, refFont)
                {
                    Alignment = Element.ALIGN_RIGHT,
                    SpacingAfter = 10f
                };
                doc.Add(datePara);

                // Orange line below date
                var orangeLine = new iTextSharp.text.pdf.draw.LineSeparator(2f, 100f, new BaseColor(255, 140, 0), Element.ALIGN_CENTER, -5);
                doc.Add(new Chunk(orangeLine));
                doc.Add(new Paragraph(" ") { SpacingAfter = 15f });
            }
            else
            {
                // If logo not found, just add date
                var dateText = $"Date: {DateTime.Now:dd/MM/yyyy}";
                var datePara = new Paragraph(dateText, refFont)
                {
                    Alignment = Element.ALIGN_CENTER,
                    SpacingAfter = 10f
                };
                doc.Add(datePara);

                // Orange line below date
                var orangeLine = new iTextSharp.text.pdf.draw.LineSeparator(2f, 100f, new BaseColor(255, 140, 0), Element.ALIGN_CENTER, -5);
                doc.Add(new Chunk(orangeLine));
                doc.Add(new Paragraph(" ") { SpacingAfter = 15f });
            }

            doc.Add(new Paragraph("\n"));

            // ================ TITLE (Offer Letter / Letter Title) ================
            var titlePara = new Paragraph(template.Title, titleFont)
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 20f
            };

            // Add underline to title
            titlePara.Font.SetStyle(Font.BOLD | Font.UNDERLINE);
            doc.Add(titlePara);

            // ================ SECTIONS ================
            foreach (var section in template.Sections.OrderBy(x => x.SerialNo))
            {
                // Section header (numbered and underlined like "1. Monthly Gross Salary")
                var sectionHeaderText = $"{section.SerialNo}. {section.Header}";
                var sectionHeader = new Paragraph(sectionHeaderText, underlineFont)
                {
                    SpacingBefore = 12f,
                    SpacingAfter = 8f
                };
                doc.Add(sectionHeader);

                // Section content (description)
                if (!string.IsNullOrEmpty(section.Description))
                {
                    // Split by lines to maintain formatting
                    var lines = section.Description.Split('\n');
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var contentPara = new Paragraph(line.Trim(), normalFont)
                            {
                                Alignment = Element.ALIGN_JUSTIFIED,
                                SpacingAfter = 3f
                            };
                            doc.Add(contentPara);
                        }
                    }
                }

                doc.Add(new Paragraph(" ", normalFont) { SpacingAfter = 5f });
            }

            doc.Close();

            return ms.ToArray();
        }

        /// <summary>
        /// Get font path with fallback
        /// </summary>
        private string GetFontPath()
        {
            var fontPath = Path.Combine(_env.WebRootPath, "fonts", "arial.ttf");

            if (!System.IO.File.Exists(fontPath))
            {
                fontPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                    "arial.ttf"
                );
            }

            if (!System.IO.File.Exists(fontPath))
            {
                return BaseFont.HELVETICA;
            }

            return fontPath;
        }
    }

    /// <summary>
    /// Footer matching the exact offer letter design
    /// </summary>
    public class OfferLetterFooterHelper : PdfPageEventHelper
    {
        private readonly int _letterId;
        private readonly string _createdBy;

        public OfferLetterFooterHelper(int letterId, string createdBy)
        {
            _letterId = letterId;
            _createdBy = createdBy;
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            base.OnEndPage(writer, document);

            var cb = writer.DirectContent;
            var pageSize = document.PageSize;

            // Calculate page count for "X of Y" format
            int currentPage = writer.PageNumber;

            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

            // ================ PAGE NUMBER (Bottom Center in Footer) ================
            cb.BeginText();
            cb.SetFontAndSize(baseFont, 9);
            cb.SetColorFill(BaseColor.Black);

            var pageText = $"Page {currentPage}";
            var pageWidth = baseFont.GetWidthPoint(pageText, 9);
            cb.SetTextMatrix((pageSize.Width - pageWidth) / 2, 45); // Center position in footer
            cb.ShowText(pageText);
            cb.EndText();

            // ================ FOOTER (Bottom) ================
            // Footer background (light gray or tan color)
            cb.SetColorFill(new BaseColor(240, 240, 235));
            cb.Rectangle(0, 0, pageSize.Width, 60);
            cb.Fill();

            // Footer disclaimer text
            cb.BeginText();
            cb.SetFontAndSize(baseFont, 7);
            cb.SetColorFill(new BaseColor(100, 100, 100));

            // Disclaimer text (centered)
            float leftMargin = 40;
            float rightMargin = 40;
            float footerY = 25;
            float maxWidth = pageSize.Width - leftMargin - rightMargin;

            string disclaimer = "DISCLAIMER: This statement is for information purposes only. Payment amounts, services, and dates may change based on project needs and contractor agreements. Please verify all details with Accounts Department (Asloob Bedaa Contracting Co.) For any other inquiries or discrepancies, please contact our Accounts Department at +96654181845, +96656024445 or email us at accounts@asloob.com";

            // Split disclaimer into multiple lines
            var words = disclaimer.Split(' ');
            var currentLine = "";
            var yPosition = footerY;

            foreach (var word in words)
            {
                var testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                var testWidth = baseFont.GetWidthPoint(testLine, 7);

                if (testWidth > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    cb.SetTextMatrix(leftMargin, yPosition);
                    cb.ShowText(currentLine);
                    currentLine = word;
                    yPosition -= 8;
                }
                else
                {
                    currentLine = testLine;
                }
            }

            // Print last line
            if (!string.IsNullOrEmpty(currentLine))
            {
                cb.SetTextMatrix(leftMargin, yPosition);
                cb.ShowText(currentLine);
            }

            cb.EndText();
        }
    }
}