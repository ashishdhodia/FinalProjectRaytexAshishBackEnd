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
using System.Security.Cryptography;
using EDIdataAPI.Controllers;

namespace EDIdataAPI.Repository
{
    public class JWTManagerRepository : IJWTManagerRepository
    {
        private readonly SqlConnection _con;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable = new DataTable();
        private string Hash;

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
            //string query = $"SELECT * FROM dbo.userData WHERE userid = '{user.Username}' AND userpass = '{user.Password}'";
            string query = $"SELECT * FROM userData WHERE userId = '{user.Username}'";
            SqlCommand cm = new SqlCommand(query, _con);
            Int32 count = Convert.ToInt32(cm.ExecuteScalar());
            if (count > 0) return true; else return false;
        }

        public Tokens Authenticate(Users users)
        {
            if (!GetExistence(users))
            {
                return null;
            }

            if (!VerifyPassword(users.Username, users.Password))
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

        public bool VerifyPassword(string Username, string Password)
        {
            UserDataController userDataController = new UserDataController();
            dataAdapter = new SqlDataAdapter($"SELECT userPassHash FROM userData WHERE userid = '{Username}'", _con);
            dataAdapter.Fill(dataTable);
            foreach (DataRow row in dataTable.Rows)
            {
                Hash = Convert.ToString(row["userPassHash"]);
            }

            var genHash = ComputeSha256Hash(Password);

            if (genHash == Hash)
            {
                return true;
            }
            return false;
        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
