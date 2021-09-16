using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DnDWebAppMVC.Data;
using DnDWebAppMVC.Models;
using DnDWebAppMVC.Models.Bases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace DnDWebAppMVC.Controllers
{
    public class GameRoomsController : Controller
    {
        private readonly CosmosDbHelper _cosmosDbHelper;
        private readonly AzureSQLDbContext _azureSQL;
        private readonly MessageHelper _messageHelper;
        private readonly CharacterHelper _characterHelper;

        public GameRoomsController(
            CosmosDbHelper cosmosDbHelper, 
            AzureSQLDbContext azureSQL, 
            MessageHelper messageHelper,
            CharacterHelper characterHelper)
        {
            _cosmosDbHelper = cosmosDbHelper;
            _azureSQL = azureSQL;
            _messageHelper = messageHelper;
            _characterHelper = characterHelper;
        }

        // GET: GameRooms
        public async Task<IActionResult> Index()
        {
            var profile = (await GetProfiles()).FirstOrDefault();
            if (profile == null)
                return NotFound("You must have a user profile - then make a character with which to enter the room.");

            ViewData["Characters"] = new SelectList(_cosmosDbHelper.Characters(profile.Id).Result, "Id", "Name");

            return View(await _cosmosDbHelper.GetGameRooms(profile.Id));
        }

        // GET: GameRooms/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            //  why check for this here?
            var userId = AuthHelper.GetOid(User);
            if (userId == null)
                return NotFound("You must register an account to create GameRooms.");

            var room = await _cosmosDbHelper.GetGameRoom(id);
            if (room == null)
                return NotFound();

            return View(room);
        }

        // GET: GameRooms/Create
        public async Task<IActionResult> Create()
        {
            //  If the user has no host profiles, then they cannot host a game
            var hostProfiles = await GetHostProfiles();
            if (hostProfiles == null)
                return NotFound("Only host profiles may create gamerooms.");

            ViewData["EditType"] = "Create";
            ViewData["SubmitLabel"] = "Create";
            ViewData["Profiles"] = new SelectList(hostProfiles, "Id", "NickName");

            return View();
        }

        // POST: GameRooms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GameRoom room) //[Bind("Id,Name,Class,Race,Level,Abilities")]
        {
            if (ModelState.IsValid)
            {
                room.Id = Guid.NewGuid();
                room.CreatedOn = DateTime.Now;
                room.IsActive = true;

                await _cosmosDbHelper.CreateGameRoomAsync(room);

                return RedirectToAction(nameof(Index));
            }

            var hostProfiles = await GetHostProfiles();
            if (hostProfiles == null)
                return NotFound("Only host profiles may create gamerooms.");

            ViewData["Profiles"] = new SelectList(hostProfiles, "Id", "NickName");
            return View(room);
        }

        // GET: GameRooms/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var room = await _cosmosDbHelper.GetGameRoom(id);
            if (room == null)
                return NotFound();

            var hostProfiles = await GetHostProfiles();
            if (!hostProfiles.Any(p => p.Id == room.OwnerId))
                return NotFound("You cannot edit this room since you are not it's owner.");

            ViewData["EditType"] = "Edit";
            ViewData["SubmitLabel"] = "Save";
            ViewData["Profiles"] = new SelectList(hostProfiles, "Id", "NickName", room.OwnerId);

            return View(room);
        }

        // POST: GameRooms/Edit/{id}
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, GameRoom room) //[Bind("Id,Name,Class,Race,Level,Abilities")]
        {
            if (id != room.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _cosmosDbHelper.UpdateGameRoomAsync(room);
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    if (!GameRoomExists(room.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            var hostProfiles = await GetHostProfiles();
            if (!hostProfiles.Any(p => p.Id == room.OwnerId))
                return NotFound("You cannot edit this room since you are not it's owner.");

            ViewData["Profiles"] = new SelectList(hostProfiles, "Id", "NickName", room.OwnerId);
            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> Open(Guid id, [Bind("CharacterId")] Guid characterId)
        {
            if (id == Guid.Empty)
                return NotFound();

            var userId = AuthHelper.GetOid(User);
            var profile = await _azureSQL.UserProfiles
                .FirstOrDefaultAsync(p => p.AccountId == userId);
            if (profile == null)
                return NotFound("You must have a user profile - then make a character with which to enter the room.");

            var character = await _cosmosDbHelper.Character(characterId);
            if (character == null)
                return NotFound("You must have a character enter a gameroom.");

            var room = await _cosmosDbHelper.GetGameRoom(id);
            if (room == null)
                return NotFound();

            room.Password = RandomString(6);
            room.IsOpen = true;

            _cosmosDbHelper.UpdateGameRoomAsync(room);

            var message = new Message
            {
                SenderId = character.OwnerId,
                SenderName = character.Name,
                RoomId = room.Id
            };

            var game = new Game()
            {
                Room = room,
                PlayerCharacter = character,
                CurrentMessage = message,
                //Messages = _messageHelper.Get(character.OwnerId, room.OwnerId),
                Characters = _characterHelper.Get()
            };

            _characterHelper.Set(character);

            ViewData["OwnerName"] = (await _azureSQL.UserProfiles
                .FirstOrDefaultAsync(p => p.Id == room.OwnerId))
                .NickName;

            return View("Room",game);
        }

        [HttpPost]
        public async Task<IActionResult> Join([Bind("RoomKey")] string roomKey, [Bind("CharacterId")] Guid characterId)
        {
            if (roomKey == null)
                return NotFound();

            var userId = AuthHelper.GetOid(User);
            var profile = await _azureSQL.UserProfiles
                .FirstOrDefaultAsync(p => p.AccountId == userId);
            if (profile == null)
                return NotFound("You must have a user profile - then make a character with which to enter the room.");

            var character = await _cosmosDbHelper.Character(characterId);
            if (character == null)
                return NotFound("You must have a character enter a gameroom.");

            var room = await _cosmosDbHelper.GetGameRoom(roomKey);
            if (room == null)
                return NotFound();

            if (!room.IsActive)
                return NotFound("This room is no longer active.");

            if (!room.IsOpen)
                return NotFound("This room is not open.");

            var message = new Message
            {
                SenderId = character.OwnerId,
                SenderName = character.Name,
                RoomId = room.Id
            };

            var game = new Game()
            {
                Room = room,
                PlayerCharacter = character,
                CurrentMessage = message,
                //Messages = _messageHelper.Get(character.OwnerId, room.OwnerId),
                Characters = _characterHelper.Get()
            };

            _characterHelper.Set(character);

            ViewData["OwnerName"] = (await _azureSQL.UserProfiles
                .FirstOrDefaultAsync(p => p.Id == room.OwnerId))
                .NickName;

            return View("Room",game);
        }

        //[HttpPost]
        //public async Task<ActionResult> Send(Game game)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var message = game.CurrentMessage;

        //        message.Id = Guid.NewGuid();
        //        message.SentOn = DateTime.Now;
        //        message.SenderId = game.PlayerCharacter.OwnerId;

        //        var roomId = game.Room.Id;
        //        message.RoomId = roomId;

        //        var senderProfile = await GetProfile(message.SenderId);
        //        message.SenderProfile = senderProfile.NickName;
        //        message.SenderName = game.PlayerCharacter.Name;

        //        if (message.IsPrivate)
        //        {
        //            UserProfile receiverProfile;

        //            if (message.SenderId == game.Room.OwnerId)
        //                receiverProfile = await GetProfile(message.ReceiverId);
        //            else
        //                receiverProfile = await GetProfile(game.Room.OwnerId);

        //            message.ReceiverProfile = receiverProfile.NickName;
        //            message.ReceiverName = game.PlayerCharacter.Name;
        //        }
        //        else
        //        {
        //            message.ReceiverId = Guid.Empty;
        //        }

        //        //_context.Add(message);
        //        //await _context.SaveChangesAsync();

        //        _messageHelper.Set(message);
        //        game.Messages = _messageHelper.Get(game.PlayerCharacter.Id, game.Room.OwnerId);
                
        //        game.CurrentMessage = null;

        //        return PartialView("GameMessages", game);
        //    }

        //    return NotFound();
        //}

        //[HttpPost]
        //public async Task<ActionResult> Refresh(Game game)
        //{
        //    game.Messages = _messageHelper.Get(game.PlayerCharacter.Id, game.Room.OwnerId);
        //    game.Characters = _characterHelper.Get();
        //    return PartialView("GameMessages", game);
        //}

        [HttpGet]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var room = await _cosmosDbHelper.GetGameRoom(id);
            room.IsOpen = false;
            room.IsActive = false;
            room.ClosedOn = DateTime.Now;
            await _cosmosDbHelper.UpdateGameRoomAsync(room);

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: GameRooms/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return NotFound();

            var room = (await _cosmosDbHelper.GetGameRoom(id));
            if (room == null)
                return NotFound();

            return View(room);
        }

        // POST: GameRooms/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var room = await _cosmosDbHelper.GetGameRoom(id);
            await _cosmosDbHelper.DeleteGameRoomAsync(room);

            return RedirectToAction(nameof(Index));
        }

        private bool GameRoomExists(Guid id)
        {
            return _cosmosDbHelper.GetGameRoom(id).Result != null;
        }

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private async Task<IEnumerable<UserProfile>> GetHostProfiles()
        {
            var userId = AuthHelper.GetOid(User);
            return await _azureSQL.UserProfiles
                .Where(p => p.AccountId == userId && p.IsHost)
                .ToListAsync();
        }

        private async Task<IEnumerable<UserProfile>> GetProfiles()
        {
            var userId = AuthHelper.GetOid(User);
            return await _azureSQL.UserProfiles
                .Where(p => p.AccountId == userId)
                .ToListAsync();
        }

        private async Task<UserProfile> GetProfile(Guid id)
        {
            var userId = AuthHelper.GetOid(User);
            return await _azureSQL.UserProfiles
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
