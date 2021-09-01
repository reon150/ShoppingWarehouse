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
using ShoppingWarehouse.Resources.UnitOfMeasurement;

namespace ShoppingWarehouse.Controllers
{
    [Authorize]
    public class UnitOfMeasurementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UnitOfMeasurementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            return View(
                await _context.UnitOfMeasurement
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

            var unitOfMeasurement = await _context.UnitOfMeasurement
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitOfMeasurement == null)
            {
                return NotFound();
            }

            return View(unitOfMeasurement);
        }

        [Authorize(Roles = Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Id,IsActive")] UnitOfMeasurement unitOfMeasurement)
        {
            if (ModelState.IsValid)
            {
                _context.Add(unitOfMeasurement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(unitOfMeasurement);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Edit(int? id) 
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasurement = await _context.UnitOfMeasurement.FindAsync(id);
            if (unitOfMeasurement == null)
            {
                return NotFound();
            }
            return View(unitOfMeasurement);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Description,Id,IsActive")] UnitOfMeasurement unitOfMeasurement)
        {
            if (id != unitOfMeasurement.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(unitOfMeasurement);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UnitOfMeasurementExists(unitOfMeasurement.Id))
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
            return View(unitOfMeasurement);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var unitOfMeasurement = await _context.UnitOfMeasurement
                .FirstOrDefaultAsync(m => m.Id == id);
            if (unitOfMeasurement == null)
            {
                return NotFound();
            }

            return View(unitOfMeasurement);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var unitOfMeasurement = await _context.UnitOfMeasurement.FindAsync(id);
            _context.UnitOfMeasurement.Remove(unitOfMeasurement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            string fileName = $"{ UnitOfMeasurementResource.UnitOfMeasurement }.csv";
            string filePath = FileHelper.GetFileExportPath(fileName);
            StreamWriter streamWriter = new(filePath, false, Encoding.UTF32);

            streamWriter.WriteLine("sep=,");
            streamWriter.WriteLine(
                $"{ nameof(UnitOfMeasurement.Id) }," +
                $"{ UnitOfMeasurementResource.Description }," +
                $"{ BaseEntityResource.IsActive }," +
                $"{ BaseEntityResource.CreatedDate }," +
                $"{ BaseEntityResource.LastUpdatedDate }");

            var unitOfMeasurements = await _context.UnitOfMeasurement.ToListAsync();

            foreach (var unitOfMeasurement in unitOfMeasurements)
            {
                streamWriter.WriteLine(
                    $"{ unitOfMeasurement.Id }," +
                    $"{ unitOfMeasurement.Description }," +
                    $"{ BoolHelper.BoolToYesNoString(unitOfMeasurement.IsActive) }," +
                    $"{ unitOfMeasurement.CreatedDate }," +
                    $"{ unitOfMeasurement.LastUpdatedDate }");
            }

            streamWriter.Close();

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);

            var contentDisposition = new ContentDisposition { FileName = fileName, Inline = false };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }

        private bool UnitOfMeasurementExists(int id)
        {
            return _context.UnitOfMeasurement.Any(e => e.Id == id);
        }
    }
}
