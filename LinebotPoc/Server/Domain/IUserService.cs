using LinebotPoc.Shared;

namespace LinebotPoc.Server.Domain
{
    public interface IUserService
    {
        Task InitAsync(string connectionString);
        Task Delete(DummyUserDto userDto);
        DummyUserDto GetUser(string userEmail);
        DummyUserDto GetUserByLine(string lineUserId);
        DummyUserDto GetUserByLineNonce(string lineNonce);
        List<DummyUserDto> GetUsers();
        Task Create(DummyUserDto userDto);
        Task Update(DummyUserDto userDto);
    }
}
