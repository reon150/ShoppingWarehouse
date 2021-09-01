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
using ShoppingWarehouse.Resources.Employee;

namespace ShoppingWarehouse.Controllers
{
    [Authorize(Roles = Role.Admin)]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            var applicationDbContext = _context.Employee.Include(e => e.Department);

            return View(
                await applicationDbContext
                    .Where(
                        p => query == null
                        || p.IdentityCard.Contains(query)
                        || p.FirstName.Contains(query)
                        || p.LastName.Contains(query)
                        || p.DateOfBirth.ToString().Contains(query)
                        || p.Department.Name.Contains(query)
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

            var employee = await _context.Employee
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Department, nameof(Department.Id), nameof(Department.Name));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdentityCard,FirstName,LastName,DateOfBirth,DepartmentId,Id,IsActive")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, nameof(Department.Id), nameof(Department.Name));
            return View(employee);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Department, nameof(Department.Id), nameof(Department.Name));
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdentityCard,FirstName,LastName,DateOfBirth,DepartmentId,Id,IsActive")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["DepartmentId"] = new SelectList(_context.Department, nameof(Department.Id), nameof(Department.Name));
            return View(employee);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            _context.Employee.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            string fileName = $"{ EmployeeResource.Employees }.csv";
            string filePath = FileHelper.GetFileExportPath(fileName);
            StreamWriter streamWriter = new(filePath, false, Encoding.UTF32);

            streamWriter.WriteLine("sep=,");
            streamWriter.WriteLine(
                $"{ nameof(Employee.Id) }," +
                $"{ EmployeeResource.IdentityCard }," +
                $"{ EmployeeResource.FirstName }," +
                $"{ EmployeeResource.LastName }," +
                $"{ EmployeeResource.Department }," +
                $"{ BaseEntityResource.IsActive }," +
                $"{ BaseEntityResource.CreatedDate }," +
                $"{ BaseEntityResource.LastUpdatedDate }");

            var employees = await _context.Employee.Include(e => e.Department).ToListAsync();

            foreach (var employee in  employees)
            {
                streamWriter.WriteLine(
                    $"{ employee.Id }," +
                    $"{ employee.IdentityCard }," +
                    $"{ employee.FirstName }," +
                    $"{ employee.LastName }," +
                    $"{ employee.Department.Name }," +
                    $"{ BoolHelper.BoolToYesNoString(employee.IsActive) }," +
                    $"{ employee.CreatedDate }," +
                    $"{ employee.LastUpdatedDate }");
            }

            streamWriter.Close();

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);
            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);

            var contentDisposition = new ContentDisposition { FileName = fileName, Inline = false };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }
    }
}
