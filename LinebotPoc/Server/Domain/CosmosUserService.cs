using LinebotPoc.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace LinebotPoc.Server.Domain
{
    public class CosmosUserService : IUserService
    {
        CosmosClient CosmosClient;
        Container UserContainer;
        public CosmosUserService()
        {

        }

        public Task Create(DummyUserDto userDto)
        {
            userDto.id = Guid.NewGuid().ToString();
            return UserContainer.CreateItemAsync(userDto);

        }

        public Task Update(DummyUserDto userDto)
        {
            return UserContainer.UpsertItemAsync(userDto);
        }

        public List<DummyUserDto> GetUsers()
        {
            return UserContainer.GetItemLinqQueryable<DummyUserDto>(true).ToList();
        }

        public DummyUserDto GetUser(string userEmail)
        {
            var query = UserContainer.GetItemLinqQueryable<DummyUserDto>(true);
            //只支援先Where,再AsEnumerable後的查詢
            var find = query.Where(o => o.UserEmail == userEmail).AsEnumerable().FirstOrDefault();
            return find;
        }

        public DummyUserDto GetUserByLine(string lineUserId)
        {
            var query = UserContainer.GetItemLinqQueryable<DummyUserDto>(true);
            var find = query.Where(o => o.LineUserId == lineUserId).AsEnumerable().FirstOrDefault();
            return find;
        }

        public DummyUserDto GetUserByLineNonce(string lineNonce)
        {
            var query = UserContainer.GetItemLinqQueryable<DummyUserDto>(true);
            var find = query.Where(o => o.LineNonce == lineNonce).AsEnumerable().FirstOrDefault();
            return find;
        }

        public async Task Delete(DummyUserDto userDto)
        {
            var result = await UserContainer.DeleteItemAsync<DummyUserDto>(userDto.id, PartitionKey.None);
            await Task.CompletedTask;
        }

        public async Task InitAsync(string connectionString)
        {
            var split = connectionString.Replace("AccountEndpoint=", "").Replace("AccountKey=", "").Split(';');
            string endpoingtUri = split[0];
            string primaryKey =  split[1];
            this.CosmosClient = new CosmosClient(endpoingtUri, primaryKey, new CosmosClientOptions() { ApplicationName = "LineBotPocApp" });
            Database database = await CosmosClient.CreateDatabaseIfNotExistsAsync("LineBotPoc");
            this.UserContainer = await database.CreateContainerIfNotExistsAsync("User", "/Dev");
        }
    }
}
