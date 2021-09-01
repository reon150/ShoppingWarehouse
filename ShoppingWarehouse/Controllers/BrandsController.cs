using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using ShoppingWarehouse.Constants;
using ShoppingWarehouse.Data;
using ShoppingWarehouse.Data.Entities;
using ShoppingWarehouse.Helpers;
using ShoppingWarehouse.Resources.BaseEntity;
using ShoppingWarehouse.Resources.Brand;

namespace ShoppingWarehouse.Controllers
{
    [Authorize]
    public class BrandsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BrandsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            return View(
                await _context.Brand
                    .Where(
                        p => query == null 
                        || p.Description.Contains(query)
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

            var brand = await _context.Brand
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [Authorize(Roles = Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Id,IsActive")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                _context.Add(brand);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }
            return View(brand);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Description,Id,IsActive")] Brand brand)
        {
            if (id != brand.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(brand);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BrandExists(brand.Id))
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
            return View(brand);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var brand = await _context.Brand
                .FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                return NotFound();
            }

            return View(brand);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var brand = await _context.Brand.FindAsync(id);
            _context.Brand.Remove(brand);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            string fileName = $"{ BrandResource.Brands }.csv";
            string filePath = FileHelper.GetFileExportPath(fileName);
            StreamWriter streamWriter = new(filePath, false, Encoding.UTF32); 

            streamWriter.WriteLine("sep=,");
            streamWriter.WriteLine(
                $"{ nameof(Brand.Id) }," +
                $"{ BrandResource.Description }," +
                $"{ BaseEntityResource.IsActive }," +
                $"{ BaseEntityResource.CreatedDate }," +
                $"{ BaseEntityResource.LastUpdatedDate }");

            var brands = await _context.Brand.ToListAsync();

            foreach (var brand in brands)
            {
                streamWriter.WriteLine(
                    $"{ brand.Id }," +
                    $"{ brand.Description }," +
                    $"{ BoolHelper.BoolToYesNoString(brand.IsActive) }," +
                    $"{ brand.CreatedDate }," +
                    $"{ brand.LastUpdatedDate }");
            }

            streamWriter.Close();

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);

            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);

            var contentDisposition = new ContentDisposition { FileName = fileName, Inline = false };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }

        private bool BrandExists(int id)
        {
            return _context.Brand.Any(e => e.Id == id);
        }
    }
}
