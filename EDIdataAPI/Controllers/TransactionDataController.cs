using Microsoft.AspNetCore.Mvc;
using EDIdataAPI.Model;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EDIdataAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionDataController : ControllerBase
    {
        private readonly SqlConnection _con;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable = new DataTable();

        public TransactionDataController()
        {
            _con = new SqlConnection("Server=.; Database=eModal; Integrated Security=true; Trusted_Connection=True; MultipleActiveResultSets=True;");
            _con.Open();
        }

        // GET: api/<TransactionDataController>
        [HttpGet]
        public List<TransactionData> Get()
        {
            List<TransactionData> transactionData = new List<TransactionData>();
            dataAdapter = new SqlDataAdapter("SELECT * FROM transactionData", _con);
            dataAdapter.Fill(dataTable);
            _con.Close();
            foreach (DataRow row in dataTable.Rows)
            {
                transactionData.Add(new TransactionData
                {
                    Id = Convert.ToInt32(row["id"]),
                    containerid = Convert.ToString(row["containerid"]),
                    containerFees = Convert.ToInt32(row["containerFees"]),
                    userid = Convert.ToString(row["userid"]),
                    cardOwnerName = Convert.ToString(row["cardOwnerName"]),
                    cardType = Convert.ToString(row["cardType"]),
                    cardNumber = Convert.ToInt32(row["cardNumber"]),
                    txnTime = Convert.ToString(row["txnTime"])
                });
            }
            return transactionData;
        }

        // POST api/<TransactionDataController>
        [HttpPost]
        public void Post([Bind("Id,containerid,containerFees, userid, cardOwnerName, cardType, cardNumber, txnTime")] TransactionData transactionData)
        {
            transactionData.txnTime = DateTime.Now.ToString();

            if (ModelState.IsValid)
            {
                dataAdapter = new SqlDataAdapter($"INSERT INTO transactionData(containerid,containerFees, userid, cardOwnerName, cardType, cardNumber, txnTime) VALUES('{transactionData.containerid}','{transactionData.containerFees}', '{transactionData.userid}', '{transactionData.cardOwnerName}', '{transactionData.cardType}', '{transactionData.cardNumber}', '{transactionData.txnTime}')", _con);
                dataAdapter.Fill(dataTable);
            }
        }
    }
}
