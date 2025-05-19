using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Windows.Compatibility;
using générationdétiquettes.Data;
using générationdétiquettes.Models;
using GenerationEtiquettes.Models;
using Microsoft.EntityFrameworkCore;

namespace GenerationEtiquettes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : ControllerBase
    {
        private readonly BarcodeDbContext _context;
        private const int QRCodeSize = 300;
        private const int BarcodeHeight = 100;
        private const int BarcodeWidth = 400;
        private const int Margin = 20;

        public BarcodeController(BarcodeDbContext context)
        {
            _context = context;
        }

        // ✅ POST pour générer l’étiquette
        [HttpPost("generate")]
        public IActionResult Generate([FromBody] BarcodeRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Code) && !string.IsNullOrWhiteSpace(request.Prefix))
            {
                var sequence = _context.CodeSequences.FirstOrDefault(s => s.Prefix == request.Prefix);
                if (sequence == null)
                {
                    sequence = new CodeSequence { Prefix = request.Prefix, LastNumber = 0 };
                    _context.CodeSequences.Add(sequence);
                }

                sequence.LastNumber++;
                _context.SaveChanges();
                request.Code = $"{request.Prefix}-{sequence.LastNumber:D6}";
            }

            var image = GenerateBarcodeImage(request);
            if (image == null)
                return BadRequest("Type de code-barres non pris en charge.");

            using var ms = new MemoryStream();
            
            image.Save(ms, ImageFormat.Png);
            var base64 = Convert.ToBase64String(ms.ToArray());

            var entity = new BarcodeEntity
            {
                Code = request.Code,
                Description = request.Description,
                Type = request.Type,
                Base64Image = base64,
                CodeFamille = request.CodeFamille,
                LibelleFamille = request.LibelleFamille,
                CodeLocalisation = request.CodeLocalisation,
                LibelleLocalisation = request.LibelleLocalisation,
                Texte = request.Texte,
                CreatedAt = DateTime.Now
            };

            _context.Barcodes.Add(entity);
            _context.SaveChanges();

            return Ok(new
            {
                id = entity.Id,
                base64Image = base64
            });
        }

        // ✅ GET pour lister toutes les étiquettes
        [HttpGet]
        public IActionResult GetAll()
        {
            var list = _context.Barcodes
                .Select(b => new
                {
                    b.Id,
                    b.Code,
                    b.Description,
                    b.Type,
                    b.CreatedAt,
                    base64Image = b.Base64Image
                }).ToList();

            return Ok(list);
        }

        // ✅ GET pour télécharger l’image
        [HttpGet("download/{id}")]
        public IActionResult DownloadImage(int id)
        {
            var barcode = _context.Barcodes.Find(id);
            if (barcode == null)
                return NotFound("Code-barres introuvable.");

            byte[] imageBytes = Convert.FromBase64String(barcode.Base64Image);
            return File(imageBytes, "image/png", $"barcode_{id}.png");
        }

        [HttpGet("by-code/{code}")]
        public IActionResult GetByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("Code requis");

            var cleanedCode = code.Trim().ToUpper();

            var barcode = _context.Barcodes
                .AsEnumerable() // ← ramène les données côté serveur pour comparer sans erreur de collation
                .FirstOrDefault(b => b.Code?.Trim().ToUpper() == cleanedCode);

            if (barcode == null)
                return NotFound();

            return Ok(new { base64Image = barcode.Base64Image });
        }




        [HttpPost("generate-from-article")]
        public IActionResult GenerateFromArticle([FromBody] Article article)
        {
            if (string.IsNullOrWhiteSpace(article.Nom))
                return BadRequest("Nom est requis.");

            // ✅ Générer automatiquement le CodeArticle s'il est vide
            if (string.IsNullOrWhiteSpace(article.CodeArticle))
            {
                var prefix = GeneratePrefix(article.Famille);
                var sequence = _context.CodeSequences.FirstOrDefault(s => s.Prefix == prefix);
                if (sequence == null)
                {
                    sequence = new CodeSequence { Prefix = prefix, LastNumber = 0 };
                    _context.CodeSequences.Add(sequence);
                }

                sequence.LastNumber++;
                _context.SaveChanges();

                article.CodeArticle = $"{prefix}-{sequence.LastNumber:D6}";
            }

            var request = new BarcodeRequest
            {
                Prefix = article.CodeArticle.Split('-').FirstOrDefault() ?? "DEF",
                Code = article.CodeArticle,
                Description = article.Nom,
                CodeFamille = article.Famille ?? "",
                LibelleFamille = article.Famille ?? "",
                CodeLocalisation = article.Localisation ?? "",
                LibelleLocalisation = article.Localisation ?? "",
                Texte = article.MarqueModele ?? "",
                Type = "qr",
                DateAcquisition = article.DateAcquisition,
                DateEnregistrement = article.DateEnregistrement,
                Layout = GetDefaultLayout()
            };

            var image = GenerateBarcodeImage(request);
            if (image == null) return BadRequest("Type non pris en charge");

            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            var base64 = Convert.ToBase64String(ms.ToArray());

            var entity = new BarcodeEntity
            {
                Code = request.Code,
                Description = request.Description,
                Type = request.Type,
                Base64Image = base64,
                CodeFamille = request.CodeFamille,
                LibelleFamille = request.LibelleFamille,
                CodeLocalisation = request.CodeLocalisation,
                LibelleLocalisation = request.LibelleLocalisation,
                Texte = request.Texte,
                CreatedAt = DateTime.Now
            };

            _context.Barcodes.Add(entity);
            _context.SaveChanges();

            // Lier le BarcodeId à l'article si existant
            var existingArticle = _context.Articles.FirstOrDefault(a => a.CodeArticle == article.CodeArticle);
            if (existingArticle != null)
            {
                existingArticle.BarcodeId = entity.Id;
                _context.SaveChanges();
            }

            return Ok(new { id = entity.Id, base64Image = base64 });
        }

        private List<LayoutElement> GetDefaultLayout()
        {
            return new List<LayoutElement>
    {
        new LayoutElement
        {
            Id = "code",
            Top = 10,
            Left = 10,
            Width = 300,
            Height = 30,
            FontSize = "14px",
            FontWeight = "bold",
            FontStyle = "normal",
            Color = "#000",
            TextAlign = "left",
            Visible = true
        },
        new LayoutElement
        {
            Id = "description",
            Top = 40,
            Left = 10,
            Width = 300,
            Height = 30,
            FontSize = "12px",
            FontWeight = "normal",
            FontStyle = "italic",
            Color = "#333",
            TextAlign = "left",
            Visible = true
        },
        new LayoutElement
        {
            Id = "dateAcquisition",
            Top = 70,
            Left = 10,
            Width = 300,
            Height = 20,
            FontSize = "12px",
            FontWeight = "normal",
            FontStyle = "normal",
            Color = "#000",
            TextAlign = "left",
            Visible = true
        },
        new LayoutElement
        {
            Id = "dateEnregistrement",
            Top = 90,
            Left = 10,
            Width = 300,
            Height = 20,
            FontSize = "12px",
            FontWeight = "normal",
            FontStyle = "normal",
            Color = "#000",
            TextAlign = "left",
            Visible = true
        },
        new LayoutElement
        {
            Id = "barcode",
            Top = 130,
            Left = 10,
            Width = 250,
            Height = 250,
            FontSize = "14px",
            FontWeight = "normal",
            FontStyle = "normal",
            Color = "#000",
            TextAlign = "left",
            Visible = true
        }
    };
        }



        // ⬇️ Génération de l’image finale avec bordure, saut de ligne, QR enrichi
        private Bitmap GenerateBarcodeImage(BarcodeRequest request)
        {
            Bitmap barcodeBitmap = request.Type.ToLower() switch
            {
                "qr" => GenerateQRCode(GenerateQrContent(request), QRCodeSize, QRCodeSize),
                "1d" => Generate1DBarcode(request.Code, BarcodeWidth, BarcodeHeight),
                _ => null
            };

            if (barcodeBitmap == null) return null;

            int canvasWidth = 600;
            int canvasHeight = 400;
            var finalImage = new Bitmap(canvasWidth, canvasHeight);

            using var g = Graphics.FromImage(finalImage);
            g.FillRectangle(Brushes.White, 0, 0, canvasWidth, canvasHeight);
            g.DrawRectangle(Pens.Black, 0, 0, canvasWidth - 1, canvasHeight - 1); // Bordure noire

            // Logo
            if (!string.IsNullOrEmpty(request.LogoBase64))
            {
                try
                {
                    var logoBytes = Convert.FromBase64String(request.LogoBase64
                        .Replace("data:image/png;base64,", "")
                        .Replace("data:image/jpeg;base64,", ""));
                    using var logoStream = new MemoryStream(logoBytes);
                    using var logo = new Bitmap(logoStream);
                    g.DrawImage(logo, Margin, Margin, 60, 60);
                }
                catch { }
            }

            // Layout des éléments
            foreach (var element in request.Layout)
            {
                if (!element.Visible) continue;

                float x = element.Left;
                float y = element.Top;
                float width = element.Width;
                float height = element.Height;

                var fontStyle = FontStyle.Regular;
                if (element.FontWeight == "bold") fontStyle |= FontStyle.Bold;
                if (element.FontStyle == "italic") fontStyle |= FontStyle.Italic;

                float fontSize = float.TryParse(element.FontSize?.Replace("px", ""), out var size) ? size : 10;
                var font = new Font("Arial", fontSize, fontStyle);
                Brush brush = new SolidBrush(ColorTranslator.FromHtml(element.Color));

                string content = element.Id switch
                {
                    "code" => request.Code,
                    "description" => request.Description,
                    "famille" => $"{request.CodeFamille} - {request.LibelleFamille}",
                    "localisation" => $"{request.CodeLocalisation} - {request.LibelleLocalisation}",
                    "texte" => request.Texte,
                    "dateAcquisition" => request.DateAcquisition?.ToString("dd/MM/yyyy") ?? "",
                    "dateEnregistrement" => request.DateEnregistrement?.ToString("dd/MM/yyyy") ?? "",
                    "barcode" => null,
                    _ => string.Empty
                };

                if (element.Id == "barcode")
                {
                    Bitmap barcodeImage = request.Type == "qr"
                        ? GenerateQRCode(GenerateQrContent(request), (int)width, (int)height)
                        : Generate1DBarcode(request.Code, (int)width, (int)height);
                    g.DrawImage(barcodeImage, x, y, width, height);
                }
                else
                {
                    var format = new StringFormat()
                    {
                        FormatFlags = StringFormatFlags.LineLimit,
                        Trimming = StringTrimming.EllipsisWord
                    };

                    format.Alignment = element.TextAlign switch
                    {
                        "center" => StringAlignment.Center,
                        "right" => StringAlignment.Far,
                        _ => StringAlignment.Near
                    };

                    g.DrawString(content, font, brush, new RectangleF(x, y, width, height), format);
                }
            }

            return finalImage;
        }

        private string GenerateQrContent(BarcodeRequest request)
        {
            var content = new StringBuilder();
            content.Append($"CODE:{request.Code}|");
            content.Append($"DESC:{request.Description}|");
            content.Append($"FAM:{request.CodeFamille}-{request.LibelleFamille}|");
            content.Append($"LOC:{request.CodeLocalisation}-{request.LibelleLocalisation}|");
            content.Append($"TXT:{request.Texte}");
            return content.ToString();
        }

        private Bitmap GenerateQRCode(string content, int width, int height)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 1,
                    CharacterSet = "UTF-8",
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H
                }
            };

            var pixelData = writer.Write(content);
            var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
            var bmpData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bmpData.Scan0, pixelData.Pixels.Length);
            bitmap.UnlockBits(bmpData);
            return bitmap;
        }

        private Bitmap Generate1DBarcode(string code, int width, int height)
        {
            var writer = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = width,
                    Height = height,
                    Margin = 2
                },
                Renderer = new BitmapRenderer()
            };

            return writer.Write(code);
        }

        private string GeneratePrefix(string? famille)
        {
            if (string.IsNullOrWhiteSpace(famille))
                return "DEF";

            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "Imprimantes", "IMP" },
        { "Ordinateurs", "ORD" },
        { "Scanners", "SCA" },
        { "Vidéoprojecteurs", "VID" },
        { "Téléphones", "TEL" },
        { "Serveurs", "SRV" },
        { "Écrans", "ECR" },
        { "Bureau", "BUR" },
        { "Chaises", "CHA" },
        { "Chaise", "CHA" },
        { "Armoires", "ARM" },
        { "Tables", "TAB" },
        { "Routeurs", "ROU" },
        { "Switchs", "SWT" },
        { "Accessoires", "ACC" },
        { "Câbles", "CAB" },
        { "Logiciels", "LOG" },
        { "Onduleurs", "OND" }
    };

            if (dict.TryGetValue(famille.Trim(), out var prefix))
                return prefix;

            return new string(famille
                .Where(char.IsLetter)
                .Take(3)
                .Select(char.ToUpper)
                .ToArray());
        }

    }
}
