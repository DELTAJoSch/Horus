using HorusAPI.Entities;
using HorusAPI.Exceptions;
using HorusAPI.Helpers;
using HorusAPI.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog.Filters;
using System.Runtime.CompilerServices;

namespace HorusAPI.Database
{
    /// <summary>
    /// Repository for User entries
    /// </summary>
    public class UserDatabase: ICrud<User>
    {
        private readonly IMongoCollection<User> _userCollection;

        public UserDatabase(IOptions<DatabaseSettings> settings)
        {
            var mongoClient = new MongoClient(
                settings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                settings.Value.DatabaseName);

            var options = new CreateIndexOptions() { Unique = true };
            var field = new StringFieldDefinition<User>("Name");
            var indexDefinition = new IndexKeysDefinitionBuilder<User>().Ascending(field);
            var model = new CreateIndexModel<User>(indexDefinition, options);

            _userCollection = mongoDatabase.GetCollection<User>(name: settings.Value.Collections["Users"]);

            _userCollection.Indexes.CreateOne(model);
        }

        public long Count()
        {
            var res = _userCollection.CountDocuments(FilterDefinition<User>.Empty);
            return res;
        }

        public long Count(User hints)
        {
            var filter = new FilterDefinitionBuilder<User>()
                .Where(user =>
                    (hints.Email == null || (user.Email != null && user.Email.ToLower().Contains(hints.Email.ToLower()))) &&
                    (hints.Name == null || (user.Name != null && user.Name.ToLower().Contains(hints.Name.ToLower()))) &&
                    (hints.Role == null || user.Role == hints.Role)
                );

            var res = _userCollection.CountDocuments(filter);
            return res;
        }

        public async Task<string> Create(User create)
        {
            await _userCollection.InsertOneAsync(create);

            if(create.Id == null)
            {
                throw new ArgumentNullException("Id was null after insert!");
            }

            return create.Id;
        }

        public async Task Delete(string id)
        {
            var res = await _userCollection.DeleteOneAsync(id);
            if(!res.IsAcknowledged || res.DeletedCount == 0)
            {
                throw new NotFoundException("Could not find element");
            }
        }

        public async Task Delete(User entity)
        {
            var filter = GetFilterDefinition(entity);
            var res = await _userCollection.DeleteOneAsync(filter);
            if (!res.IsAcknowledged || res.DeletedCount == 0)
            {
                throw new NotFoundException("Could not find element");
            }
        }

        public async Task<IEnumerable<User>> Get(int batchSize, int pageOffset)
        {
            var result = await _userCollection.FindAsync(FilterDefinition<User>.Empty);

            return result.Current.Skip(batchSize * pageOffset).Take(batchSize);
        }

        public async Task<IEnumerable<User>> Get(int batchSize, int pageOffset, User hints)
        {
            var filter = new FilterDefinitionBuilder<User>()
                .Where(user =>
                    (hints.Email == null || (user.Email != null && user.Email.ToLower().Contains(hints.Email.ToLower()))) &&
                    (hints.Name == null || (user.Name != null && user.Name.ToLower().Contains(hints.Name.ToLower()))) &&
                    (hints.Role == null || user.Role == hints.Role)
                );

            var result = _userCollection.Find(filter)
                .Skip(batchSize * pageOffset)
                .Limit(batchSize);

            return await result.ToListAsync();
        }

        public async Task<User> Get(string id)
        {
            var result = await _userCollection.FindAsync(x => x.Id == id);

            var user = await result.FirstOrDefaultAsync();

            if(user == null)
            {
                throw new NotFoundException("Could not find element");
            }

            return user;
        }

        public async Task<User> Get(User hints)
        {
            var filter = GetFilterDefinition(hints);

            var result = await _userCollection.FindAsync(filter);

            var user = await result.FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NotFoundException("Could not find element");
            }

            return user;
        }

        public async Task Update(string id, User updated)
        {
            var filter = new FilterDefinitionBuilder<User>()
                .Where(user => user.Id == id);

            var update = new UpdateDefinitionBuilder<User>()
                .Set(x => x.Name, updated.Name)
                .Set(x => x.Email, updated.Email)
                .Set(x => x.Salt, updated.Salt)
                .Set(x => x.Password, updated.Password)
                .Set(x => x.Role, updated.Role);

            await _userCollection.UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Returns a filter defintion for user search
        /// </summary>
        /// <param name="hints">The hints to look for</param>
        /// <returns>Returns a new filter definition</returns>
        private FilterDefinition<User> GetFilterDefinition(User hints)
        {
            return new FilterDefinitionBuilder<User>()
                .Where(user =>
                    (user.Email == hints.Email || hints.Email == null) &&
                    (user.Name == hints.Name || hints.Name == null) &&
                    (user.Password == hints.Password || hints.Password == null) &&
                    (user.Role == hints.Role || hints.Role == null)
                );
        }
    }
}
