using EDIdataAPI.Model;
using EDIdataAPI.Repository;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace EDIdataAPI.Repository
{
    public class JWTManagerRepository : IJWTManagerRepository
    {
        private readonly SqlConnection _con;
        //private SqlDataAdapter dataAdapter;
        //private DataTable dataTable = new DataTable();

        private readonly IConfiguration iconfiguration;
        public JWTManagerRepository(IConfiguration iconfiguration)
        {
            this.iconfiguration = iconfiguration;
            _con = new SqlConnection("Server=localhost; Database=eModal; Integrated Security=true; Trusted_Connection=True; MultipleActiveResultSets=True;");
            _con.Open();
        }

        [HttpGet]
        public bool GetExistence(Users user)
        {
            string query = $"SELECT * FROM dbo.userData WHERE userid = '{user.Username}' AND userpass = '{user.Password}'";
            SqlCommand cm = new SqlCommand(query, _con);
            Int32 count = Convert.ToInt32(cm.ExecuteScalar());
            if (count > 0) return true; else return false;
        }

        public Tokens Authenticate(Users users)
        {
            bool exist = GetExistence(users);
            Console.WriteLine(exist);

            if (!exist)
            {
                return null;
            }

            // Else we generate JSON Web Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.UTF8.GetBytes(iconfiguration["JWT:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
              {
             new Claim(ClaimTypes.Name, users.Username),
             new Claim(ClaimTypes.Role, "User")

              }),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new Tokens { Token = tokenHandler.WriteToken(token) };

        }
    }
}
