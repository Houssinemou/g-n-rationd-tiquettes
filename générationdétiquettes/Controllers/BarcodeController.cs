using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using ZXing.Rendering;
using ZXing.Windows.Compatibility;

namespace GenerationEtiquettes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarcodeController : ControllerBase
    {
        [HttpPost("generate")]
        public IActionResult Generate([FromBody] BarcodeRequest request)
        {
            // Vérifie que les champs obligatoires sont fournis
            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.Type))
                return BadRequest("Le code et le type sont obligatoires.");

            // Génère l'image du code-barres
            var image = GenerateBarcodeImage(request);

            if (image == null)
                return BadRequest("Type de code-barres non pris en charge.");

            // Convertit l'image en base64 pour l'envoyer au frontend
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            var base64 = Convert.ToBase64String(ms.ToArray());

            // Renvoie le code-barres au format base64
            return Ok(new { base64Image = base64 });
        }

        private Bitmap GenerateBarcodeImage(BarcodeRequest request)
        {
            string fullText = $"{request.Code} | {request.Description}";
            Bitmap bitmap;

            if (request.Type.ToLower() == "qr")
            {
                // Génération d’un QR Code
                var qrWriter = new BarcodeWriterPixelData
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new QrCodeEncodingOptions
                    {
                        Height = 200,
                        Width = 200,
                        Margin = 1
                    }
                };

                var pixelData = qrWriter.Write(fullText);
                bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppArgb);
                var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                                              ImageLockMode.WriteOnly, bitmap.PixelFormat);
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bmpData.Scan0, pixelData.Pixels.Length);
                bitmap.UnlockBits(bmpData);
            }
            else if (request.Type.ToLower() == "1d")
            {
                // Génération d’un code-barres 1D (Code 128)
                var writer = new BarcodeWriter<Bitmap>
                {
                    Format = BarcodeFormat.CODE_128,
                    Options = new EncodingOptions
                    {
                        Height = 100,
                        Width = 300,
                        Margin = 2
                    },
                    Renderer = new BitmapRenderer()
                };

                bitmap = writer.Write(fullText);
            }
            else
            {
                return null; // Type non supporté
            }

            return bitmap;
        }
    }

    // Modèle de données pour la requête
    public class BarcodeRequest
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Type { get; set; } // QR ou 1D
    }
}
