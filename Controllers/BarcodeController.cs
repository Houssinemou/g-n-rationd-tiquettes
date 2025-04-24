using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using GenerationEtiquettes.Models;
using Microsoft.EntityFrameworkCore;
using générationdétiquettes.Data;
using générationdétiquettes.Models;
using ZXing.Windows.Compatibility;

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
        private const int TextAreaHeight = 150;
        private const int Margin = 20;

        public BarcodeController(BarcodeDbContext context)
        {
            _context = context;
        }

        [HttpPost("generate")]
        public IActionResult Generate([FromBody] BarcodeRequest request)
        {
            // Initialisation des champs optionnels
            request.Description ??= string.Empty;
            request.CodeFamille ??= string.Empty;
            request.LibelleFamille ??= string.Empty;
            request.CodeLocalisation ??= string.Empty;
            request.LibelleLocalisation ??= string.Empty;
            request.Texte ??= string.Empty;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
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
                    Texte = request.Texte
                };

                _context.Barcodes.Add(entity);
                _context.SaveChanges();

                return Ok(new
                {
                    id = entity.Id,
                    base64Image = base64,
                    details = new
                    {
                        entity.Code,
                        entity.Description,
                        entity.Type,
                        entity.CodeFamille,
                        entity.LibelleFamille,
                        entity.CodeLocalisation,
                        entity.LibelleLocalisation,
                        entity.Texte
                    },
                    qrContent = GenerateQrContent(request)
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Une erreur est survenue: {ex.Message}");
            }
        }

        private string GenerateQrContent(BarcodeRequest request)
        {
            var content = new StringBuilder();
            content.Append($"CODE:{request.Code}|");

            if (!string.IsNullOrEmpty(request.Description))
                content.Append($"DESC:{request.Description}|");

            if (!string.IsNullOrEmpty(request.CodeFamille))
                content.Append($"FAM:{request.CodeFamille}|");

            if (!string.IsNullOrEmpty(request.LibelleFamille))
                content.Append($"LIB_FAM:{request.LibelleFamille}|");

            if (!string.IsNullOrEmpty(request.CodeLocalisation))
                content.Append($"LOC:{request.CodeLocalisation}|");

            if (!string.IsNullOrEmpty(request.LibelleLocalisation))
                content.Append($"LIB_LOC:{request.LibelleLocalisation}|");

            if (!string.IsNullOrEmpty(request.Texte))
                content.Append($"TXT:{request.Texte}");

            return content.ToString();
        }

        private Bitmap GenerateBarcodeImage(BarcodeRequest request)
        {
            return request.Type.ToLower() switch
            {
                "qr" => GenerateQRCodeWithText(request),
                "1d" => Generate1DBarcodeWithText(request),
                _ => null
            };
        }

        private Bitmap GenerateQRCodeWithText(BarcodeRequest request)
        {
            var qrContent = GenerateQrContent(request);
            var qrBitmap = GenerateQRCode(qrContent, QRCodeSize, QRCodeSize);

            var finalImage = new Bitmap(QRCodeSize + Margin * 2, QRCodeSize + TextAreaHeight);

            using (var g = Graphics.FromImage(finalImage))
            {
                g.FillRectangle(Brushes.White, 0, 0, finalImage.Width, finalImage.Height);
                g.DrawImage(qrBitmap, Margin, 0);
                DrawTextInfo(g, request, QRCodeSize + 10);
            }

            return finalImage;
        }

        private Bitmap Generate1DBarcodeWithText(BarcodeRequest request)
        {
            var barcodeBitmap = Generate1DBarcode(request.Code, BarcodeWidth, BarcodeHeight);

            var finalImage = new Bitmap(BarcodeWidth + Margin * 2, BarcodeHeight + TextAreaHeight);

            using (var g = Graphics.FromImage(finalImage))
            {
                g.FillRectangle(Brushes.White, 0, 0, finalImage.Width, finalImage.Height);
                g.DrawImage(barcodeBitmap, Margin, 0);
                DrawTextInfo(g, request, BarcodeHeight + 10);
            }

            return finalImage;
        }

        private void DrawTextInfo(Graphics g, BarcodeRequest request, int startY)
        {
            var titleFont = new Font("Arial", 12, FontStyle.Bold);
            var normalFont = new Font("Arial", 10);
            var smallFont = new Font("Arial", 8);
            var format = new StringFormat { Alignment = StringAlignment.Center };
            int currentY = startY;

            // Code
            g.DrawString(request.Code, titleFont, Brushes.Black,
                        new RectangleF(Margin, currentY, BarcodeWidth, 20), format);
            currentY += 25;

            // Description
            if (!string.IsNullOrEmpty(request.Description))
            {
                g.DrawString(request.Description, normalFont, Brushes.Black,
                            new RectangleF(Margin, currentY, BarcodeWidth, 20), format);
                currentY += 20;
            }

            // Famille
            if (!string.IsNullOrEmpty(request.CodeFamille) || !string.IsNullOrEmpty(request.LibelleFamille))
            {
                g.DrawString($"{request.CodeFamille} {request.LibelleFamille}".Trim(),
                            normalFont, Brushes.Black,
                            new RectangleF(Margin, currentY, BarcodeWidth, 20), format);
                currentY += 20;
            }

            // Localisation
            if (!string.IsNullOrEmpty(request.CodeLocalisation) || !string.IsNullOrEmpty(request.LibelleLocalisation))
            {
                g.DrawString($"{request.CodeLocalisation} {request.LibelleLocalisation}".Trim(),
                            normalFont, Brushes.Black,
                            new RectangleF(Margin, currentY, BarcodeWidth, 20), format);
                currentY += 20;
            }

            // Texte
            if (!string.IsNullOrEmpty(request.Texte))
            {
                g.DrawString(request.Texte, smallFont, Brushes.Black,
                            new RectangleF(Margin, currentY, BarcodeWidth, 40), format);
            }
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
            var bmpData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                                      ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
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

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var list = _context.Barcodes.ToList();
            return Ok(list);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var barcode = _context.Barcodes.Find(id);
            if (barcode == null)
                return NotFound("Code-barres introuvable.");

            _context.Barcodes.Remove(barcode);
            _context.SaveChanges();

            return Ok("Code-barres supprimé avec succès.");
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadImage(int id)
        {
            var barcode = _context.Barcodes.Find(id);
            if (barcode == null)
                return NotFound("Code-barres introuvable.");

            byte[] imageBytes = Convert.FromBase64String(barcode.Base64Image);
            return File(imageBytes, "image/png", $"barcode_{id}.png");
        }
    }
}