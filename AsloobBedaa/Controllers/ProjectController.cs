using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AsloobBedaa.DataContext;
using AsloobBedaa.Models;

namespace AsloobBedaa.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Project
        public async Task<IActionResult> Index()
        {
            var projects = await _context.Projects
                                         .AsNoTracking()
                                         .Where(p => !p.IsDeleted)
                                         .ToListAsync();
            return View(projects);
        }

        // GET: Project/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Project project)
        {
            if (!ModelState.IsValid)
                return View(project);

            project.CreatedAt = DateTime.Now;
            project.IsDeleted = false;

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Project/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _context.Projects
                                        .FirstOrDefaultAsync(p => p.ProjectID == id && !p.IsDeleted);

            if (project == null)
                return NotFound();

            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Project project)
        {
            if (id != project.ProjectID)
                return NotFound();

            if (!ModelState.IsValid)
                return View(project);

            try
            {
                project.UpdatedAt = DateTime.Now;
                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Project/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _context.Projects
                                        .FirstOrDefaultAsync(p => p.ProjectID == id && !p.IsDeleted);

            if (project == null)
                return NotFound();

            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project != null)
            {
                project.IsDeleted = true;   // Soft delete
                project.UpdatedAt = DateTime.Now;

                _context.Projects.Update(project);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(p => p.ProjectID == id && !p.IsDeleted);
        }
    }
}
