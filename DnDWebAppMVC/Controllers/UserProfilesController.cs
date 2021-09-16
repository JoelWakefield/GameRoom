using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DnDWebAppMVC.Data;
using DnDWebAppMVC.Models;

namespace DnDWebAppMVC.Controllers
{
    public class UserProfilesController : Controller
    {
        private readonly AzureSQLDbContext _context;

        public UserProfilesController(AzureSQLDbContext context)
        {
            _context = context;
        }

        // GET: UserProfiles
        public async Task<IActionResult> Index()
        {
            return View(await GetProfiles());
        }

        // GET: UserProfiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
                return NotFound();

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProfile == null)
                return NotFound();

            return View(userProfile);
        }

        // GET: UserProfiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserProfiles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfile userProfile)
        {
            if (ModelState.IsValid)
            {
                userProfile.Id = Guid.NewGuid();
                userProfile.AccountId = AuthHelper.GetOid(User);
                _context.Add(userProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(userProfile);
        }

        // GET: UserProfiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
                return NotFound();

            var userProfile = await _context.UserProfiles.FindAsync(id);
            if (userProfile == null)
                return NotFound();
            
            return View(userProfile);
        }

        // POST: UserProfiles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, UserProfile userProfile)
        {
            if (id != userProfile.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    userProfile.ModifiedOn = DateTime.Now;
                    _context.Update(userProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserProfileExists(userProfile.Id))
                        return NotFound();
                    else
                        throw;
                }
            
                return RedirectToAction(nameof(Index));
            }
            
            return View(userProfile);
        }

        // GET: UserProfiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
                return NotFound();

            var userProfile = await _context.UserProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userProfile == null)
                return NotFound();

            return View(userProfile);
        }

        // POST: UserProfiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userProfile = await _context.UserProfiles.FindAsync(id);
            _context.UserProfiles.Remove(userProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserProfileExists(Guid id)
        {
            return _context.UserProfiles.Any(e => e.Id == id);
        }

        private async Task<IEnumerable<UserProfile>> GetProfiles()
        {
            var userId = AuthHelper.GetOid(User);
            return await _context.UserProfiles
                .Where(p => p.AccountId == userId)
                .ToListAsync();
        }
    }
}
