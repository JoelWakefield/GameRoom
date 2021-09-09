
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DnDWebAppMVC.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace DnDWebAppMVC.Data {
    public class CosmosDbHelper {
        // The name of the database and container
        private string databaseId = "RPGData";
        private string[] containerId = { "Characters", "GameRooms" };

        private readonly IConfiguration _configuration;

        public CosmosDbHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Characters

        public async Task<IEnumerable<Character>> Characters(Guid ownerId)
        {
            List<Character> characters = new List<Character>();

            //  filter by owner, potentially by specific character
            var sqlQueryText = $"SELECT * FROM c WHERE c.ownerId = @ownerId";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            queryDefinition.WithParameter("@ownerId", ownerId);

            // Create a new instance of the Cosmos Client
            using (var client = new CosmosClient(
                _configuration["ConnectionStrings:EndpointUrl"],
                _configuration["ConnectionStrings:PrimaryKey"]))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[0]);

                FeedIterator<Character> queryResultSetIterator = container.GetItemQueryIterator<Character>(queryDefinition);

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Character> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Character character in currentResultSet)
                        characters.Add(character);
                }

                client.Dispose();
            }

            return characters;
        }

        public async Task<Character> Character(Guid id)
        {
            List<Character> characters = new List<Character>();

            //  filter by owner, potentially by specific character
            var sqlQueryText = $"SELECT * FROM c WHERE c.id = @id";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            queryDefinition.WithParameter("@id", id);

            // Create a new instance of the Cosmos Client
            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[0]);

                FeedIterator<Character> queryResultSetIterator = container.GetItemQueryIterator<Character>(queryDefinition);

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<Character> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (Character character in currentResultSet)
                        characters.Add(character);
                }

                client.Dispose();
            }

            return characters[0];
        }

        public async Task CreateCharacterAsync(Character character)
        {
            //  Assign character metadata
            character.Id = Guid.NewGuid();
            character.CreatedOn = DateTime.UtcNow;

            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[0]);

                try
                {
                    ItemResponse<Character> response = await container.CreateItemAsync(character, new PartitionKey(character.Id.ToString()));
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    throw;                
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        public async Task UpdateCharacterAsync(Character character)
        {
            //  Assign character metadata
            character.ModifiedOn = DateTime.UtcNow;

            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[0]);

                try
                {
                    ItemResponse<Character> response = await container.UpsertItemAsync(character, new PartitionKey(character.Id.ToString()));
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    throw;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        public async Task DeleteCharacterAsync(Character character)
        {
            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[0]);

                try
                {
                    ItemResponse<Character> response = await container.DeleteItemAsync<Character>(character.Id.ToString(), new PartitionKey(character.Id.ToString()));
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    throw;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        #endregion

        #region GameRooms

        public async Task<GameRoom> GetGameRoom(Guid Id)
        {
            List<GameRoom> rooms = new List<GameRoom>();

            //  filter by owner, potentially by specific character
            var sqlQueryText = $"SELECT * FROM c WHERE c.id = @id";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            queryDefinition.WithParameter("@id", Id);

            // Create a new instance of the Cosmos Client
            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[1]);

                FeedIterator<GameRoom> queryResultSetIterator = container.GetItemQueryIterator<GameRoom>(queryDefinition);

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<GameRoom> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (GameRoom gameRoom in currentResultSet)
                        rooms.Add(gameRoom);
                }

                client.Dispose();
            }

            return rooms[0];
        }

        public async Task<GameRoom> GetGameRoom(string key)
        {
            List<GameRoom> rooms = new List<GameRoom>();

            //  filter by owner, potentially by specific character
            var sqlQueryText = $"SELECT * FROM c WHERE c.password = @key";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            queryDefinition.WithParameter("@key", key);

            // Create a new instance of the Cosmos Client
            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[1]);

                FeedIterator<GameRoom> queryResultSetIterator = container.GetItemQueryIterator<GameRoom>(queryDefinition);

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<GameRoom> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (GameRoom gameRoom in currentResultSet)
                        rooms.Add(gameRoom);
                }

                client.Dispose();
            }

            return rooms[0];
        }

        public async Task<IEnumerable<GameRoom>> GetGameRooms(Guid ownerId)
        {
            List<GameRoom> rooms = new List<GameRoom>();

            //  filter by owner, potentially by specific character
            var sqlQueryText = $"SELECT * FROM c WHERE c.ownerId = @ownerId";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            queryDefinition.WithParameter("@ownerId", ownerId);

            // Create a new instance of the Cosmos Client
            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[1]);

                FeedIterator<GameRoom> queryResultSetIterator = container.GetItemQueryIterator<GameRoom>(queryDefinition);

                while (queryResultSetIterator.HasMoreResults)
                {
                    FeedResponse<GameRoom> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                    foreach (GameRoom gameRoom in currentResultSet)
                        rooms.Add(gameRoom);
                }

                client.Dispose();
            }

            return rooms;
        }

        public async Task CreateGameRoomAsync(GameRoom room)
        {
            //  Assign character metadata
            room.Id = Guid.NewGuid();
            room.CreatedOn = DateTime.UtcNow;

            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[1]);

                try
                {
                    ItemResponse<GameRoom> response = await container.CreateItemAsync(room, new PartitionKey(room.Id.ToString()));
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    throw;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        public async Task UpdateGameRoomAsync(GameRoom room)
        {
            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[1]);

                try
                {
                    ItemResponse<GameRoom> response = await container.UpsertItemAsync(room, new PartitionKey(room.Id.ToString()));
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    throw;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        public async Task DeleteGameRoomAsync(GameRoom room)
        {
            using (var client = new CosmosClient(
                _configuration.GetConnectionString("EndpointUrl"),
                _configuration.GetConnectionString("PrimaryKey")))
            {
                Database database = client.GetDatabase(databaseId);
                Container container = database.GetContainer(containerId[1]);

                try
                {
                    ItemResponse<GameRoom> response = await container.DeleteItemAsync<GameRoom>(room.Id.ToString(), new PartitionKey(room.Id.ToString()));
                }
                catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
                {
                    throw;
                }
                finally
                {
                    client.Dispose();
                }
            }
        }

        #endregion
    }
}