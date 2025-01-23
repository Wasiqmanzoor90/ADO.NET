namespace MVC_Studio.Interfaces
{
    public interface IJwtoken
    {
        string Createtoken(int id, string email);
        int VerifyToken(string id);

    }
}
