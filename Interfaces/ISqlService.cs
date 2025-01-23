namespace MVC_Studio.Interfaces
{
    public interface ISqlService
    {
        Task<bool> RegisterUserAsync(string name, string email, string password);
        Task<bool> UserExistsAsync(string email); //checkinf if user exists or not
        Task<bool> LoginAsync(string Email, string Password);
    }
}
