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
using ShoppingWarehouse.Resources.ArticleRequest;
using ShoppingWarehouse.Resources.BaseEntity;
using ShoppingWarehouse.Resources.Generic;

namespace ShoppingWarehouse.Controllers
{
    [Authorize(Roles = Role.Admin)] 
    public class ArticleRequestsController : Controller 
    {
        private readonly ApplicationDbContext _context;

        public ArticleRequestsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string query = null)
        {
            var applicationDbContext = _context.ArticleRequest.Include(a => a.Article).Include(a => a.Employee);

            return View(
                await applicationDbContext
                    .Where(
                        p => query == null
                        || p.Quantity.ToString().Contains(query)
                        || p.Employee.FirstName.Contains(query)
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

            var articleRequest = await _context.ArticleRequest
                .Include(a => a.Article)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (articleRequest == null)
            {
                return NotFound();
            }

            return View(articleRequest);
        }

        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.Article, nameof(Article.Id), nameof(Article.Description));
            ViewData["EmployeeId"] = new SelectList(_context.Employee, nameof(Employee.Id), nameof(Employee.FirstName));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Quantity,EmployeeId,ArticleId,Id,IsActive")] ArticleRequest articleRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(articleRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, nameof(Article.Id), nameof(Article.Description), articleRequest.ArticleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employee, nameof(Employee.Id), nameof(Employee.FirstName), articleRequest.EmployeeId);
            return View(articleRequest);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleRequest = await _context.ArticleRequest.FindAsync(id);
            if (articleRequest == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.Article, nameof(Article.Id), nameof(Article.Description), articleRequest.ArticleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employee, nameof(Employee.Id), nameof(Employee.FirstName), articleRequest.EmployeeId);
            return View(articleRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Quantity,EmployeeId,ArticleId,Id,IsActive")] ArticleRequest articleRequest)
        {
            if (id != articleRequest.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(articleRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleRequestExists(articleRequest.Id))
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
            ViewData["ArticleId"] = new SelectList(_context.Article, nameof(Article.Id), nameof(Article.Description), articleRequest.ArticleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employee, nameof(Employee.Id), nameof(Employee.FirstName), articleRequest.EmployeeId);
            return View(articleRequest);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var articleRequest = await _context.ArticleRequest
                .Include(a => a.Article)
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (articleRequest == null)
            {
                return NotFound();
            }

            return View(articleRequest);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var articleRequest = await _context.ArticleRequest.FindAsync(id);
            _context.ArticleRequest.Remove(articleRequest);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ExportToExcel()
        {
            string fileName = $"{ ArticleRequestResource.ArticleRequests }.csv";
            string filePath = FileHelper.GetFileExportPath(fileName);
            StreamWriter streamWriter = new(filePath, false, Encoding.UTF32);

            streamWriter.WriteLine("sep=,");
            streamWriter.WriteLine(
                $"{ nameof(ArticleRequest.Id) }," +
                $"{ ArticleRequestResource.Employee }," +
                $"{ ArticleRequestResource.SolicitationDate }," +
                $"{ ArticleRequestResource.Article }," +
                $"{ ArticleRequestResource.Quantity }," +
                $"{ ArticleRequestResource.UnitOfMeasurement }," +
                $"{ BaseEntityResource.IsActive }," +
                $"{ BaseEntityResource.LastUpdatedDate }");

            var articleRequests = await _context.ArticleRequest.Include(a => a.Article).Include(a => a.Employee).ToListAsync();

            foreach (var articleRequest in articleRequests)
            {
                streamWriter.WriteLine(
                    $"{ articleRequest.Id }," +
                    $"{ articleRequest.Employee.FirstName } { articleRequest.Employee.LastName }," +
                    $"{ articleRequest.CreatedDate }," +
                    $"{ articleRequest.Article.Description }," +
                    $"{ articleRequest.Quantity }," +
                    $"{ articleRequest.Article.UnitOfMeasurement }," +
                    $"{ BoolHelper.BoolToYesNoString(articleRequest.IsActive) }," +
                    $"{ articleRequest.LastUpdatedDate }");
            }

            streamWriter.Close();

            byte[] filedata = System.IO.File.ReadAllBytes(filePath);

            new FileExtensionContentTypeProvider().TryGetContentType(filePath, out string contentType);

            var contentDisposition = new ContentDisposition { FileName = fileName, Inline = false };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(filedata, contentType);
        }

        private bool ArticleRequestExists(int id)
        {
            return _context.ArticleRequest.Any(e => e.Id == id);
        }
    }
}
