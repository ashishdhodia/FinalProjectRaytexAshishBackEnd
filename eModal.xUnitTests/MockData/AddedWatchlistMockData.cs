using EDIdataAPI.Model;
using System.Collections.Generic;

namespace eModal.xUnitTests.MockData
{
    public class AddedWatchlistMockData
    {
        public static List<AddedWatchlistData> GetWatchlistData()
        {
            return new List<AddedWatchlistData>{
             new AddedWatchlistData{
                 Id = 1,
                 UserId = "UserId1",
                 ContainerId = "ContainerId1"
             },
             new AddedWatchlistData{
                 Id = 2,
                 UserId = "UserId2",
                 ContainerId = "ContainerId2"
             },
             new AddedWatchlistData{
                 Id = 3,
                 UserId = "UserId3",
                 ContainerId = "ContainerId3"
             },
         };
        }

        public static List<AddedWatchlistData> GetEmptyWatchlistData()
        {
            return new List<AddedWatchlistData>();
        }

        public static AddedWatchlistData NewWatchlistData()
        {
            return new AddedWatchlistData
            {
                Id = 4,
                UserId = "UserId4",
                ContainerId = "ContainerId4"
            };
        }
    }
}
