using Microsoft.AspNetCore.Mvc;
using EDIdataAPI.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EDIdataAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AddedWatchlistDataController : ControllerBase
    {
        private readonly SqlConnection _con;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable = new DataTable();

        public AddedWatchlistDataController()
        {
            _con = new SqlConnection("Server=.; Database=eModal; Integrated Security=true; Trusted_Connection=True; MultipleActiveResultSets=True;");
            _con.Open();
        }

        // GET: api/<AddedWatchlistDataController>
        [HttpGet]
        public List<AddedWatchlistData> Get()
        {
            List<AddedWatchlistData> addedWatchlistData = new List<AddedWatchlistData>();
            dataAdapter = new SqlDataAdapter("SELECT * FROM addedWatchlistData", _con);
            dataAdapter.Fill(dataTable);
            _con.Close();
            foreach (DataRow row in dataTable.Rows)
            {
                addedWatchlistData.Add(new AddedWatchlistData
                {
                    Id = Convert.ToInt32(row["id"]),
                    UserId = Convert.ToString(row["userId"]),
                    ContainerId = Convert.ToString(row["containerId"])
                });
            }
            return addedWatchlistData;
        }

        [HttpPost]
        public void Post([Bind("Id,UserId,ContainerId")] AddedWatchlistData addedWatchlistData)
        {
            if (ModelState.IsValid)
            {
                dataAdapter = new SqlDataAdapter($"INSERT INTO addedWatchlistData(userId,containerId) VALUES('{addedWatchlistData.UserId}','{addedWatchlistData.ContainerId}')", _con);
                dataAdapter.Fill(dataTable);
            }
        }

        // DELETE api/<AddedWatchlistDataController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            dataAdapter = new SqlDataAdapter($"DELETE FROM addedWatchlistData WHERE id={id}", _con);
            dataAdapter.Fill(dataTable);
        }
    }
}
