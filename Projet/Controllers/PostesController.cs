using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projet.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Projet.Controllers
{
    public class PostesController : Controller
    {
        private readonly ProjetContext _context;

        public PostesController(ProjetContext context)
        {
            _context = context;
        }

        // GET: Postes
        public async Task<IActionResult> Index()
        {
            // 1. Récupérer la liste des postes
            var postes = await _context.Postes.ToListAsync();

            // 2. STATS POUR LE GRAPHIQUE : Top 5 des métiers avec le plus d'offres
            // On interroge la table Offre pour compter
            var stats = await _context.Offres
                .GroupBy(o => o.Poste.Intitule)
                .Select(g => new { Metier = g.Key, NbOffres = g.Count() })
                .OrderByDescending(x => x.NbOffres)
                .Take(5)
                .ToListAsync();

            ViewBag.Labels = stats.Select(x => x.Metier).ToList();
            ViewBag.Data = stats.Select(x => x.NbOffres).ToList();

            return View(postes);
        }

        // GET: Postes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var poste = await _context.Postes
                .FirstOrDefaultAsync(m => m.Id == id);

            // On récupère aussi le nombre d'offres liées pour l'afficher
            ViewBag.NbOffresLiees = await _context.Offres.CountAsync(o => o.PosteId == id);

            if (poste == null) return NotFound();

            return View(poste);
        }

        // GET: Postes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Postes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Intitule")] Poste poste)
        {
            if (ModelState.IsValid)
            {
                _context.Add(poste);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(poste);
        }

        // GET: Postes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var poste = await _context.Postes.FindAsync(id);
            if (poste == null) return NotFound();
            return View(poste);
        }

        // POST: Postes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Intitule")] Poste poste)
        {
            if (id != poste.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(poste);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PosteExists(poste.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(poste);
        }

        // GET: Postes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var poste = await _context.Postes.FirstOrDefaultAsync(m => m.Id == id);
            if (poste == null) return NotFound();
            return View(poste);
        }

        // POST: Postes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poste = await _context.Postes.FindAsync(id);
            if (poste != null) _context.Postes.Remove(poste);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PosteExists(int id)
        {
            return _context.Postes.Any(e => e.Id == id);
        }
    }
}