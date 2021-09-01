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
using ShoppingWarehouse.Resources.Supplier;

namespace ShoppingWarehouse.Controllers
{
    [Authorize]
    public class SuppliersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SuppliersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            return View(
                await _context.Supplier
                    .Where(
                        p => query == null
                        || p.NationalTaxPayerRegistry.Contains(query)
                        || p.CommercialName.Contains(query)
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

            var supplier = await _context.Supplier
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [Authorize(Roles = Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NationalTaxPayerRegistry,CommercialName,Id,IsActive")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplier);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier.FindAsync(id);
            if (supplier == null)
            {
                return NotFound();
            }
            return View(supplier);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NationalTaxPayerRegistry,CommercialName,Id,IsActive")] Supplier supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplierExists(supplier.Id))
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
            return View(supplier);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplier = await _context.Supplier
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supplier == null)
            {
                return NotFound();
            }

            return View(supplier);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _context.Supplier.FindAsync(id);
            _context.Supplier.Remove(supplier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            string fileName = $"{ SupplierResource.Suppliers }.csv";
            string filePath = FileHelper.GetFileExportPath(fileName);
            StreamWriter streamWriter = new(filePath, false, Encoding.UTF32);

            streamWriter.WriteLine("sep=,");
            streamWriter.WriteLine(
                $"{ nameof(Supplier.Id) }," +
                $"{ SupplierResource.NationalTaxPayerRegistry }," +
                $"{ SupplierResource.CommercialName }," +
                $"{ BaseEntityResource.IsActive }," +
                $"{ BaseEntityResource.CreatedDate }," +
                $"{ BaseEntityResource.LastUpdatedDate }");

            var suppliers = await _context.Supplier.ToListAsync();

            foreach (var supplier in suppliers) 
            {
                streamWriter.WriteLine(
                    $"{ supplier.Id }," +
                    $"{ supplier.NationalTaxPayerRegistry }," +
                    $"{ supplier.CommercialName }," +
                    $"{ BoolHelper.BoolToYesNoString(supplier.IsActive) }," +
                    $"{ supplier.CreatedDate }," +
                    $"{ supplier.LastUpdatedDate }");
            }

            streamWriter.Close();

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);

            var contentDisposition = new ContentDisposition { FileName = fileName, Inline = false };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }

        private bool SupplierExists(int id)
        {
            return _context.Supplier.Any(e => e.Id == id);
        }
    }
}
