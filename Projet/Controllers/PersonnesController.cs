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
    public class PersonnesController : Controller
    {
        private readonly ProjetContext _context;

        public PersonnesController(ProjetContext context)
        {
            _context = context;
        }

        // GET: Personnes
        public async Task<IActionResult> Index()
        {
            var personnes = await _context.Personnes.ToListAsync();

            
            int junior = personnes.Count(p => p.AnneesExperienceTotal < 2);
            int confirme = personnes.Count(p => p.AnneesExperienceTotal >= 2 && p.AnneesExperienceTotal < 5);
            int senior = personnes.Count(p => p.AnneesExperienceTotal >= 5);

            ViewBag.ExpLabels = new List<string> { "Junior (<2 ans)", "Confirmé (2-5 ans)", "Senior (5+ ans)" };
            ViewBag.ExpData = new List<int> { junior, confirme, senior };

            return View(personnes);
        }

        
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            
            var personne = await _context.Personnes
                .Include(p => p.CompetenceAcquises)
                    .ThenInclude(ca => ca.Competence) 
                .FirstOrDefaultAsync(m => m.Id == id);

            if (personne == null) return NotFound();

            
            ViewBag.CompetencesDispos = _context.Competences.OrderBy(c => c.Nom).ToList();

            return View(personne);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterCompetence(int personneId, int competenceId, int niveau)
        {
            
            var existeDeja = await _context.CompetenceAcquises
                .AnyAsync(ca => ca.PersonneId == personneId && ca.CompetenceId == competenceId);

            if (!existeDeja)
            {
                var nouvelleComp = new CompetenceAcquise
                {
                    PersonneId = personneId,
                    CompetenceId = competenceId,
                    Niveau = niveau
                };
                _context.Add(nouvelleComp);
                await _context.SaveChangesAsync();
            }

            
            return RedirectToAction(nameof(Details), new { id = personneId });
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerCompetence(int id)
        {
            var compAcquise = await _context.CompetenceAcquises.FindAsync(id);
            if (compAcquise != null)
            {
                int personneId = compAcquise.PersonneId; // On garde l'ID pour la redirection
                _context.CompetenceAcquises.Remove(compAcquise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = personneId });
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Personnes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Personnes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nom,Prenom,Email,DateNaissance,Ville,CodePostal,AnneesExperienceTotal")] Personne personne)
        {
            if (ModelState.IsValid)
            {
                _context.Add(personne);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(personne);
        }

        // GET: Personnes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personne = await _context.Personnes.FindAsync(id);
            if (personne == null)
            {
                return NotFound();
            }
            return View(personne);
        }

        // POST: Personnes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nom,Prenom,Email,DateNaissance,Ville,CodePostal,AnneesExperienceTotal")] Personne personne)
        {
            if (id != personne.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(personne);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonneExists(personne.Id))
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
            return View(personne);
        }

        // GET: Personnes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var personne = await _context.Personnes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (personne == null)
            {
                return NotFound();
            }

            return View(personne);
        }

        // POST: Personnes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var personne = await _context.Personnes.FindAsync(id);
            if (personne != null)
            {
                _context.Personnes.Remove(personne);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonneExists(int id)
        {
            return _context.Personnes.Any(e => e.Id == id);
        }
    }
}
