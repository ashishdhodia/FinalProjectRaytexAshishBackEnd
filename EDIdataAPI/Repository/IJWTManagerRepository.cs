using EDIdataAPI.Model;

namespace EDIdataAPI.Repository
{
    public interface IJWTManagerRepository
    {
        Tokens Authenticate(Users users);
    }
}
