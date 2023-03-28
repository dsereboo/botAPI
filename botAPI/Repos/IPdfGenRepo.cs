using botAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace botAPI.Repos
{
    public interface IPdfGenRepo
    {
        MemoryStream GenerateTransactionPdf(PurchaseResponse items);
    }
}