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
using ShoppingWarehouse.Resources.Department;

namespace ShoppingWarehouse.Controllers
{
    [Authorize]
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            return View(
                await _context.Department
                    .Where(
                        p => query == null
                        || p.Name.Contains(query)
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

            var department = await _context.Department
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [Authorize(Roles = Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Id,IsActive")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(department);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Id,IsActive")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            return View(department);
        }

        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Department
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Department.FindAsync(id);
            _context.Department.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            string fileName = $"{ DepartmentResource.Departments }.csv";
            string filePath = FileHelper.GetFileExportPath(fileName);
            StreamWriter streamWriter = new(filePath, false, Encoding.UTF32);

            streamWriter.WriteLine("sep=,");
            streamWriter.WriteLine(
                $"{ nameof(Department.Id) }," +
                $"{ DepartmentResource.Name }," +
                $"{ BaseEntityResource.IsActive }," +
                $"{ BaseEntityResource.CreatedDate }," +
                $"{ BaseEntityResource.LastUpdatedDate }");

            var departments = await _context.Department.ToListAsync();

            foreach (var department in departments)
            {
                streamWriter.WriteLine(
                    $"{ department.Id }," +
                    $"{ department.Name }," +
                    $"{ BoolHelper.BoolToYesNoString(department.IsActive) }," +
                    $"{ department.CreatedDate }," +
                    $"{ department.LastUpdatedDate }");
            }

            streamWriter.Close();

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);

            var contentDisposition = new ContentDisposition { FileName = fileName, Inline = false };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }

        private bool DepartmentExists(int id)
        {
            return _context.Department.Any(e => e.Id == id);
        }
    }
}
