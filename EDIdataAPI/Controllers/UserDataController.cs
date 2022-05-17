using Microsoft.AspNetCore.Mvc;
using EDIdataAPI.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;

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

        public UserDataController()
        {
            _con = new SqlConnection("Server=.; Database=eModal; Integrated Security=true; Trusted_Connection=True; MultipleActiveResultSets=True;");
            _con.Open();
        }

        // POST api/<UserDataController>
        [HttpPost]
        public void Post([Bind("Id,emailid,userid, userpass, useraddress")] UserData userdData)
        {
            if (ModelState.IsValid)
            {
                dataAdapter = new SqlDataAdapter($"INSERT INTO userData (emailid,userid, userpass, useraddress) VALUES('{userdData.emailid}','{userdData.userid}', '{userdData.userpass}', '{userdData.useraddress}')", _con);
                dataAdapter.Fill(dataTable);
            }
        }
    }
}
