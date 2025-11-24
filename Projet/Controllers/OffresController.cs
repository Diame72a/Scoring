using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Projet.Models;

namespace Projet.Controllers
{
    public class OffresController : Controller
    {
        private readonly ProjetContext _context;

        public OffresController(ProjetContext context)
        {
            _context = context;
        }

        // GET: Offres
        public async Task<IActionResult> Index(string searchString, int? posteId)
        {
            
            var query = _context.Offres.Include(o => o.Poste).AsQueryable();

            
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(o => o.Titre.Contains(searchString) || o.VilleCible.Contains(searchString));
            }

            
            if (posteId.HasValue)
            {
                query = query.Where(o => o.PosteId == posteId);
            }

            
            var offres = await query.OrderByDescending(o => o.DateCreation).ToListAsync();

            

            
            ViewBag.Postes = await _context.Postes.OrderBy(p => p.Intitule).ToListAsync();

            
            var statsVilles = offres
                .GroupBy(o => o.VilleCible)
                .Select(g => new { Ville = g.Key, Nombre = g.Count() })
                .ToList();

            ViewBag.VillesLabels = statsVilles.Select(s => s.Ville).ToList();
            ViewBag.VillesData = statsVilles.Select(s => s.Nombre).ToList();

            
            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPoste"] = posteId;

            return View(offres);
        }

        // GET: Offres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offres
                .Include(o => o.Poste)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offre == null)
            {
                return NotFound();
            }

            return View(offre);
        }

        // GET: Offres/Create
        public IActionResult Create()
        {
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Id");
            return View();
        }

        // POST: Offres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Description,DateCreation,VilleCible,CodePostalCible,PosteId")] Offre offre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(offre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Id", offre.PosteId);
            return View(offre);
        }

        // GET: Offres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offres.FindAsync(id);
            if (offre == null)
            {
                return NotFound();
            }
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Id", offre.PosteId);
            return View(offre);
        }

        // POST: Offres/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,DateCreation,VilleCible,CodePostalCible,PosteId")] Offre offre)
        {
            if (id != offre.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(offre);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OffreExists(offre.Id))
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
            ViewData["PosteId"] = new SelectList(_context.Postes, "Id", "Id", offre.PosteId);
            return View(offre);
        }

        // GET: Offres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offre = await _context.Offres
                .Include(o => o.Poste)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (offre == null)
            {
                return NotFound();
            }

            return View(offre);
        }

        // POST: Offres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var offre = await _context.Offres.FindAsync(id);
            if (offre != null)
            {
                _context.Offres.Remove(offre);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OffreExists(int id)
        {
            return _context.Offres.Any(e => e.Id == id);
        }
    }
}
