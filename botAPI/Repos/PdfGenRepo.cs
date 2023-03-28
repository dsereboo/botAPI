using botAPI.Models;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace botAPI.Repos
{
    public class PdfGenRepo : IPdfGenRepo
    {

        public MemoryStream GenerateTransactionPdf(PurchaseResponse items)
        {
            PdfDocument document = new PdfDocument();
            document.Info.Title = "TransactionID";
            document.Info.Author = "PCESBot";

            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page);

            //Fonts
            XFont header = new XFont("Arial", 18, XFontStyle.Bold);
            XFont body = new XFont("Verdana", 14, XFontStyle.Regular);

            //Purchase details
            string? trans = items?.transactionReference?.ToString();
            string? bundle = items?.size?.ToString();
            string? unit = items?.unit?.ToString();
            string? price = items?.amount?.ToString();
            string? date = items?.purchaseDate?.ToString();

            //PDF layout
            gfx.DrawString("Data Bundle Purchase", header, XBrushes.Black, 100, 120);
            gfx.DrawString($"Transaction Reference:{trans}", body, XBrushes.Black, 100, 160);
            gfx.DrawString($"Bundle:{bundle} {unit}", body, XBrushes.Black, 100, 200);
            gfx.DrawString($"Price: GHS {price}", body, XBrushes.Black, 100, 240);
            gfx.DrawString($"Date:{date}", body, XBrushes.Black, 100, 280);

          /*  gfx.DrawString($"Hello", body, XBrushes.Black,
            new XRect(0, 10, page.Width, page.Height), XStringFormats.TopLeft);*/


            //Store pdf in memory stream
            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);

            return stream;
        }
    }
}
