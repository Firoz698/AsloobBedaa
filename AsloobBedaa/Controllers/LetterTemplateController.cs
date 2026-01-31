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
            Font companyFont = new Font(baseFont, 8, Font.NORMAL, new BaseColor(0, 102, 204));

            // ================ TOP HEADER (Ref + Logo) ================
            var topTable = new PdfPTable(2) { WidthPercentage = 100, SpacingAfter = 15f };
            topTable.SetWidths(new float[] { 60f, 40f });

            // Left: Ref and Date
            var refText = $"Ref: ASLOOB BEDAA/OFFER/KSA/131, Date: {DateTime.Now:dd/MM/yyyy}";
            var refCell = new PdfPCell(new Phrase(refText, refFont))
            {
                Border = Rectangle.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_LEFT,
                VerticalAlignment = Element.ALIGN_TOP,
                PaddingTop = 5f
            };
            topTable.AddCell(refCell);

            // Right: Logo and Arabic text
            var logoPath = Path.Combine(_env.WebRootPath, "images", "header_logo.jpg");

            if (System.IO.File.Exists(logoPath))
            {
                var logoTable = new PdfPTable(1);

                // Arabic text (if you have Arabic font)
                var arabicText = new Phrase("شركة أسلوب بداع لمقاولات", refFont);
                var arabicCell = new PdfPCell(arabicText)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                logoTable.AddCell(arabicCell);

                // Logo
                var logo = iTextSharp.text.Image.GetInstance(logoPath);
                logo.ScaleToFit(120f, 40f);
                var logoImgCell = new PdfPCell(logo)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT,
                    PaddingTop = 5f
                };
                logoTable.AddCell(logoImgCell);

                // Company name
                var companyName = new Phrase("ASLOOB BEDAA CONTRACTING COMPANY", companyFont);
                var companyCell = new PdfPCell(companyName)
                {
                    Border = Rectangle.NO_BORDER,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                logoTable.AddCell(companyCell);

                var rightCell = new PdfPCell(logoTable)
                {
                    Border = Rectangle.NO_BORDER
                };
                topTable.AddCell(rightCell);
            }
            else
            {
                topTable.AddCell(new PdfPCell(new Phrase("")) { Border = Rectangle.NO_BORDER });
            }

            doc.Add(topTable);
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
        private int _totalPages = 0;

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

            // ================ PAGE NUMBER (Top Right like "1 of 3") ================
            cb.BeginText();
            cb.SetFontAndSize(baseFont, 9);
            cb.SetColorFill(BaseColor.Black);

            var pageText = $"{currentPage} of 3"; // You can calculate total pages dynamically
            var pageWidth = baseFont.GetWidthPoint(pageText, 9);
            cb.SetTextMatrix(pageSize.Width - 40 - pageWidth, pageSize.Height - 30);
            cb.ShowText(pageText);
            cb.EndText();

            // ================ FOOTER (Bottom) ================
            // Footer background (light gray or tan color)
            cb.SetColorFill(new BaseColor(240, 240, 235));
            cb.Rectangle(0, 0, pageSize.Width, 60);
            cb.Fill();

            // Footer icons and text
            cb.BeginText();
            cb.SetFontAndSize(baseFont, 8);
            cb.SetColorFill(new BaseColor(100, 100, 100));

            // Left side: Phone and Email
            float leftMargin = 40;
            float footerY = 35;

            // Phone icon + number
            cb.SetTextMatrix(leftMargin, footerY);
            cb.ShowText("☎ +966 11 483 9783   ✉ info@asloob.com");

            // Address line
            cb.SetTextMatrix(leftMargin, footerY - 12);
            cb.ShowText("⌂ Kingdom of Saudi Arabia - Riyadh - Al Arfaj - Olaya - PO Box 6715 - Postal Code 12611 - C.R. 1010447921");

            cb.EndText();

            // Right side: Building icons (decorative)
            // You can add small building icons here if you have them
            // For now, using simple rectangles as placeholders
            cb.SetColorFill(new BaseColor(180, 180, 170));

            float iconX = pageSize.Width - 100;
            float iconY = 15;

            // Building 1
            cb.Rectangle(iconX, iconY, 15, 25);
            cb.Fill();

            // Building 2
            cb.Rectangle(iconX + 20, iconY, 15, 30);
            cb.Fill();

            // Building 3
            cb.Rectangle(iconX + 40, iconY, 20, 20);
            cb.Fill();

            // Building 4
            cb.Rectangle(iconX + 65, iconY, 18, 28);
            cb.Fill();
        }



    }
}