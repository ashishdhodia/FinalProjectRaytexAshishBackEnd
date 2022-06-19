using EDIdataAPI.Model;
using System.Data;
using System.Data.SqlClient;

namespace EDIdataAPI.Repository
{
    public class AddedWatchlistDataRepository: IAddedWatchlistDataRepository
    {
        private readonly SqlConnection _con;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable = new DataTable();

        public AddedWatchlistDataRepository()
        {
            _con = new SqlConnection("Server=.; Database=eModal; Integrated Security=true; Trusted_Connection=True; MultipleActiveResultSets=True;");
            _con.Open();
        }

        public async Task<List<AddedWatchlistData>> GetAllAddedWatchlistData()
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
            return addedWatchlistData.ToList(); 
        }
    }
}
