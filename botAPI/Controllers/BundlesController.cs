using botAPI.Models;
using botAPI.Repos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
/*using Newtonsoft.Json;*/

namespace botAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BundlesController : ControllerBase
    {
  
        private readonly IBundleRepo _bundleRepo;
        private readonly IPdfGenRepo _pdfGenRepo;

        public BundlesController(IBundleRepo bundleRepo, IPdfGenRepo pdfGenRepo)
        {
            _bundleRepo = bundleRepo;
            _pdfGenRepo = pdfGenRepo;
           
        }


        [Route("bundlelist")]
        [HttpPost]
        
        public async Task<IActionResult> GetBundlePackages([FromBody]BundlePackageRequest bundleId) {
            try
            {
                var result = await _bundleRepo.GetBundlePackages(bundleId.bundleId);
                if (result == null)
                {
                    return NotFound();
                }

                return new OkObjectResult(result);
            }
            catch(Exception ex)
            {
                return new BadRequestObjectResult(ex);
            }
        }

        [Route("purchase")]
        [HttpPost]
        public async Task<IActionResult> ProcessBundlePurchase([FromBody]PurchaseRequest request)
        {
            var result = await _bundleRepo.BuyDataPackage(request.userId, request.bundlePackageId, request.purchaseMode);
            if (result < 1) 
            {
                return new BadRequestObjectResult(result);
            }
            //pass on to method to acquire latest transaction
            var transaction = await _bundleRepo.GetPurchaseDetails(request.userId);

            //Obtain transaction details
            var items = transaction.ToList().First();

            //Transform to string
            /*String itemString = JsonSerializer.Serialize(items);*/

            //Generate pdf file
            var file = _pdfGenRepo.GenerateTransactionPdf(items);

            //To test pdf
         /*   return new FileStreamResult(file, "application/pdf");*/

            //Convert pdf file to Base64
            var base64string = Convert.ToBase64String(file.ToArray());

         /*   return base64string;*/
            return new OkObjectResult(base64string);
        }
    }
}
