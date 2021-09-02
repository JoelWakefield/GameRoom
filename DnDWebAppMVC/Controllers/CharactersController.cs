using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DnDWebAppMVC.Data;
using DnDWebAppMVC.Models;
using DnDWebAppMVC.Models.Bases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace DnDWebAppMVC.Controllers
{
    public class CharactersController : Controller
    {
        private readonly CosmosDbHelper _cosmosDbHelper;
        private readonly AzureSQLDbContext _azureSQL;

        public CharactersController(CosmosDbHelper cosmosDbHelper, AzureSQLDbContext azureSQL)
        {
            _cosmosDbHelper = cosmosDbHelper;
            _azureSQL = azureSQL;
        }

        // GET: Characters
        public async Task<IActionResult> Index()
        {
            var profile = GetProfiles().Result.FirstOrDefault();
            if (profile == null)
                return NotFound("You must have a profile to look at characters.");

            return View(await _cosmosDbHelper.Characters(profile.Id));
        }


        // GET: Characters/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var profile = GetProfiles().Result.FirstOrDefault();
            var character = await _cosmosDbHelper.Character(id);
            if (character == null)
                return NotFound();

            return View(character);
        }

        // POST: Characters/AddStats
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddStat([Bind("Stats")] Character character)
        {
            character.Stats.Add(new Quantifiable());
            return PartialView("CharacterStats", character);
        }

        // POST: Characters/AddSkill
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddSkill([Bind("Skills")] Character character)
        {
            character.Skills.Add(new Quantifiable());
            return PartialView("CharacterSkills", character);
        }

        // POST: Characters/AddAbility
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAbility([Bind("Abilities")] Character character)
        {
            character.Abilities.Add(new Describable());
            return PartialView("CharacterAbilities", character);
        }

        // POST: Characters/AddAttribute
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddAttribute([Bind("Attributes")] Character character)
        {
            character.Attributes.Add(new Describable());
            return PartialView("CharacterAttributes", character);
        }

        // POST: Characters/AddItem
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddItem([Bind("Items")] Character character)
        {
            character.Items.Add(new Describable());
            return PartialView("CharacterItems", character);
        }

        // POST: Characters/AddWeapon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddWeapon([Bind("Weapons")] Character character)
        {
            character.Weapons.Add(new Describable());
            return PartialView("CharacterWeapons", character);
        }

        // POST: Characters/AddSpell
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddSpell([Bind("Spells")] Character character)
        {
            character.Spells.Add(new Describable());
            return PartialView("CharacterSpells", character);
        }

        // GET: Characters/Create
        public IActionResult Create()
        {
            ViewData["EditType"] = "Create";
            ViewData["SubmitLabel"] = "Create";

            return View();
        }

        // POST: Characters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Character character) //[Bind("Id,Name,Class,Race,Level,Abilities")]
        {
            if (ModelState.IsValid)
            {
                character.Id = Guid.NewGuid();
                var profile = GetProfiles().Result.FirstOrDefault();
                character.OwnerId = profile.Id;
                character.CreatedOn = DateTime.Now;
                await _cosmosDbHelper.CreateCharacterAsync(character);

                return RedirectToAction(nameof(Index));
            }

            return View(character);
        }

        // GET: Characters/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var profile = GetProfiles().Result.FirstOrDefault();
            if (profile == null)
                return NotFound("You can't edit characters until you have a profile.");

            var character = await _cosmosDbHelper.Character(id);
            if (character == null)
                return NotFound();

            ViewData["EditType"] = "Edit";
            ViewData["SubmitLabel"] = "Save";

            return View(character);
        }

        // POST: Characters/Edit/{id}
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Character character) //[Bind("Id,Name,Class,Race,Level,Abilities")]
        {
            if (id != character.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    character.ModifiedOn = DateTime.Now;
                    await _cosmosDbHelper.UpdateCharacterAsync(character);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    if (!CharacterExists(character.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(character);
        }

        // GET: Characters/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var profile = GetProfiles().Result.FirstOrDefault();
            if (profile == null)
                return NotFound("This room can't be deleted since there's no profile for it.");

            var character = await _cosmosDbHelper.Character(id);
            if (character == null)
                return NotFound();

            return View(character);
        }

        // POST: Characters/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var profile = GetProfiles().Result.FirstOrDefault();
            var character = await _cosmosDbHelper.Character(id);
            await _cosmosDbHelper.DeleteCharacterAsync(character);
            
            return RedirectToAction(nameof(Index));
        }

        private bool CharacterExists(Guid id)
        {
            return _cosmosDbHelper.Character(id).Result != null;
        }

        private async Task<IEnumerable<UserProfile>> GetProfiles()
        {
            var userId = AuthHelper.GetOid(User);
            return await _azureSQL.UserProfiles
                .Where(p => p.AccountId == userId)
                .ToListAsync();
        }
    }
}
