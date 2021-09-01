using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using ShoppingWarehouse.Constants;
using ShoppingWarehouse.Data;
using ShoppingWarehouse.Data.Entities;
using ShoppingWarehouse.Helpers;
using ShoppingWarehouse.Resources.BaseEntity;
using ShoppingWarehouse.Resources.PurchaseOrder;

namespace ShoppingWarehouse.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class PurchaseOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PurchaseOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            var applicationDbContext = _context.PurchaseOrder.Include(p => p.Article);

            return View(
                await applicationDbContext
                    .Where(
                        p => query == null
                        || p.Quantity.ToString().Contains(query)
                        || p.UnitCost.ToString().Contains(query)
                        || p.Article.Description.Contains(query)
                        || p.CreatedDate.ToString().Contains(query)
                        || p.LastUpdatedDate.ToString().Contains(query)
                        || p.Id.ToString().Contains(query))
                    .ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder
                .Include(p => p.Article)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        [Authorize(Roles = Role.Admin)]
        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.Article, nameof(Article.Id), nameof(Article.Description));
            return View();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Quantity,UnitCost,ArticleId,Id,IsActive")] PurchaseOrder purchaseOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchaseOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, nameof(Article.Id), nameof(Article.Description));
            return View(purchaseOrder);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, nameof(Article.Id), nameof(Article.Description));
            return View(purchaseOrder);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Quantity,UnitCost,ArticleId,Id,IsActive")] PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(purchaseOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PurchaseOrderExists(purchaseOrder.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, nameof(Article.Id), nameof(Article.Description));
            return View(purchaseOrder);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var purchaseOrder = await _context.PurchaseOrder
                .Include(p => p.Article)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null)
            {
                return NotFound();
            }

            return View(purchaseOrder);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var purchaseOrder = await _context.PurchaseOrder.FindAsync(id);
            _context.PurchaseOrder.Remove(purchaseOrder);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            string fileName = $"{ PurchaseOrderResource.PurchaseOrders }.csv";
            string filePath = FileHelper.GetFileExportPath(fileName);
            StreamWriter streamWriter = new(filePath, false, Encoding.UTF32);

            streamWriter.WriteLine("sep=,");
            streamWriter.WriteLine(
                $"{ PurchaseOrderResource.OrderNumber }," +
                $"{ PurchaseOrderResource.OrderDate }," +
                $"{ PurchaseOrderResource.Article }," +
                $"{ PurchaseOrderResource.Quantity }," +
                $"{ PurchaseOrderResource.UnitOfMeasurement }," +
                $"{ PurchaseOrderResource.Brand }," +
                $"{ PurchaseOrderResource.UnitCost }," +
                $"{ BaseEntityResource.IsActive }," +
                $"{ BaseEntityResource.LastUpdatedDate }");

            var purchaseOrders = 
                await _context.PurchaseOrder
                    .Include(p => p.Article)
                        .ThenInclude(a => a.Brand)
                    .Include(p => p.Article)
                        .ThenInclude(a => a.UnitOfMeasurement)
                    .ToListAsync();

            foreach (var purchaseOrder in purchaseOrders)
            {
                streamWriter.WriteLine(
                    $"{ purchaseOrder.Id }," +
                    $"{ purchaseOrder.CreatedDate }," +
                    $"{ purchaseOrder.Article.Description }," +
                    $"{ purchaseOrder.Quantity }," +
                    $"{ purchaseOrder.Article.UnitOfMeasurement.Description }," +
                    $"{ purchaseOrder.Article.Brand.Description }," +
                    $"{ purchaseOrder.UnitCost }," +
                    $"{ BoolHelper.BoolToYesNoString(purchaseOrder.IsActive) }," +
                    $"{ purchaseOrder.LastUpdatedDate }");
            }

            streamWriter.Close();

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);

            var contentDisposition = new ContentDisposition { FileName = fileName, Inline = false };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }

        private bool PurchaseOrderExists(int id)
        {
            return _context.PurchaseOrder.Any(e => e.Id == id);
        }
    }
}
