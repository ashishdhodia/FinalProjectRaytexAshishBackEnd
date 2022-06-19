using Microsoft.AspNetCore.Mvc;
using EDIdataAPI.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EDIdataAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {
        private readonly SqlConnection _con;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable = new DataTable();
        public static UserDataMSSQL userDataMSSQL = new UserDataMSSQL();

        public UserDataController()
        {
            _con = new SqlConnection("Server=.; Database=eModal; Integrated Security=true; Trusted_Connection=True; MultipleActiveResultSets=True;");
            _con.Open();
        }

        // POST api/<UserDataController>
        [HttpPost]
        public void Post([Bind("Id,emailId,userId, userPass, userAddress")] UserData userdData)
        {
            dynamic hash = ComputeSha256Hash(userdData.userPass);


            userDataMSSQL.emailId = userdData.emailId;
            userDataMSSQL.userId = userdData.userId;
            userDataMSSQL.userPassHash = hash;
            userDataMSSQL.userAddress = userdData.userAddress;

            //if (ModelState.IsValid)
            //{
            //    dataAdapter = new SqlDataAdapter($"INSERT INTO userData (emailId,userId,userPass,userAddress) VALUES('{userdData.emailId}','{userdData.userId}', '{userdData.userPass}', '{userdData.userAddress}')", _con);
            //    dataAdapter.Fill(dataTable);
            //}
            if (ModelState.IsValid)
            {
                dataAdapter = new SqlDataAdapter($"INSERT INTO userData (emailId,userId,userPassHash,userAddress) VALUES('{userDataMSSQL.emailId}','{userDataMSSQL.userId}', '{userDataMSSQL.userPassHash}', '{userDataMSSQL.userAddress}')", _con);
                dataAdapter.Fill(dataTable);
            }
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
