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
using ShoppingWarehouse.Resources.Article;
using ShoppingWarehouse.Resources.BaseEntity;

namespace ShoppingWarehouse.Controllers
{
    [Authorize]
    public class ArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            var applicationDbContext = _context.Article.Include(a => a.Brand).Include(a => a.Supplier).Include(a => a.UnitOfMeasurement);

            return View(
                await applicationDbContext
                    .Where(
                        p => query == null
                        || p.Description.Contains(query)
                        || p.Stock.ToString().Contains(query)
                        || p.Brand.Description.Contains(query)
                        || p.UnitOfMeasurement.Description.Contains(query)
                        || p.Supplier.CommercialName.Contains(query)
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

            var article = await _context.Article
                .Include(a => a.Brand)
                .Include(a => a.Supplier)
                .Include(a => a.UnitOfMeasurement)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [Authorize(Roles = Role.Admin)]
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brand, nameof(Brand.Id), nameof(Brand.Description));
            ViewData["SupplierId"] = new SelectList(_context.Supplier, nameof(Supplier.Id), nameof(Supplier.CommercialName));
            ViewData["UnitOfMeasurementId"] = new SelectList(_context.UnitOfMeasurement, nameof(UnitOfMeasurement.Id), nameof(UnitOfMeasurement.Description));
            return View();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Stock,BrandId,UnitOfMeasurementId,SupplierId,Id,IsActive")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brand, nameof(Brand.Id), nameof(Brand.Description));
            ViewData["SupplierId"] = new SelectList(_context.Supplier, nameof(Supplier.Id), nameof(Supplier.CommercialName));
            ViewData["UnitOfMeasurementId"] = new SelectList(_context.UnitOfMeasurement, nameof(UnitOfMeasurement.Id), nameof(UnitOfMeasurement.Description));
            return View(article);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brand, nameof(Brand.Id), nameof(Brand.Description));
            ViewData["SupplierId"] = new SelectList(_context.Supplier, nameof(Supplier.Id), nameof(Supplier.CommercialName));
            ViewData["UnitOfMeasurementId"] = new SelectList(_context.UnitOfMeasurement, nameof(UnitOfMeasurement.Id), nameof(UnitOfMeasurement.Description));
            return View(article);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Description,Stock,BrandId,UnitOfMeasurementId,SupplierId,Id,IsActive")] Article article)
        {
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)

                {
                    if (!ArticleExists(article.Id))
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
            ViewData["BrandId"] = new SelectList(_context.Brand, nameof(Brand.Id), nameof(Brand.Description));
            ViewData["SupplierId"] = new SelectList(_context.Supplier, nameof(Supplier.Id), nameof(Supplier.CommercialName));
            ViewData["UnitOfMeasurementId"] = new SelectList(_context.UnitOfMeasurement, nameof(UnitOfMeasurement.Id), nameof(UnitOfMeasurement.Description));
            return View(article);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .Include(a => a.Brand)
                .Include(a => a.Supplier)
                .Include(a => a.UnitOfMeasurement)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.FindAsync(id);
            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            string fileName = $"{ ArticleResource.Articles }.csv";
            string filePath = FileHelper.GetFileExportPath(fileName);
            StreamWriter streamWriter = new(filePath, false, Encoding.UTF32);

            streamWriter.WriteLine("sep=,");
            streamWriter.WriteLine(
                $"{ nameof(Article.Id) }," +
                $"{ ArticleResource.Description }," +
                $"{ ArticleResource.Brand }," +
                $"{ ArticleResource.UnitOfMeasurement }," +
                $"{ ArticleResource.Stock }," +
                $"{ ArticleResource.Supplier }," +
                $"{ BaseEntityResource.IsActive }," +
                $"{ BaseEntityResource.CreatedDate }," +
                $"{ BaseEntityResource.LastUpdatedDate }");

            var articles = await _context.Article.Include(a => a.Brand).Include(a => a.Supplier).Include(a => a.UnitOfMeasurement).ToListAsync();

            foreach (var article in articles)
            {
                streamWriter.WriteLine(
                    $"{ article.Id }," +
                    $"{ article.Description }," +
                    $"{ article.Brand.Description }," +
                    $"{ article.UnitOfMeasurement.Description }," +
                    $"{ article.Stock }," +
                    $"{ article.Supplier.CommercialName }," +
                    $"{ BoolHelper.BoolToYesNoString(article.IsActive) }," +
                    $"{ article.CreatedDate }," +
                    $"{ article.LastUpdatedDate }");
            }

            streamWriter.Close();

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);

            var contentDisposition = new ContentDisposition { FileName = fileName, Inline = false };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
    }
}
