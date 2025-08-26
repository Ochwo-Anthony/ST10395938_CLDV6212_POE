using ABCRetailers.Models;
using ABCRetailers.Services;
using Microsoft.AspNetCore.Mvc;

namespace ABCRetailers.Controllers
{
    public class UploadController : Controller
    {
        private readonly IAzureStorageService _storageService;

        public UploadController(IAzureStorageService storageService)
        {
            _storageService = storageService;
        }

        public IActionResult Index()
        {
            return View(new FileUploadModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FileUploadModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (model.ProofOfPayment != null && model.ProofOfPayment.Length > 0)
                    {
                        //Upload to Blob Storage
                        var fileName = await _storageService.UploadFileAsync(model.ProofOfPayment, "payment-proofs");

                        //Also Upload to file share for contacts
                        await _storageService.UploadToFileShareAsync(model.ProofOfPayment, "contracts", "payments");
                        TempData["Success"] = $"File Uploaded successfully! File name: {fileName}";

                        //Clear the model for a fresh form
                        return View(new FileUploadModel());
                    }
                    else
                    {
                        ModelState.AddModelError("ProofOfPayment", "Please select a file to upload.");
                    }

                }

                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error Uploading file: {ex.Message}");
                }
            }

            return View(model);
        }
    }
}
