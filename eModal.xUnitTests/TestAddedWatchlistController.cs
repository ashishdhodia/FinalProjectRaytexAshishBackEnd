using EDIdataAPI.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using EDIdataAPI.Controllers;
using FluentAssertions;
using EDIdataAPI.Repository;
using eModal.xUnitTests.MockData;

namespace eModal.xUnitTests
{
    public class TestAddedWatchlistController
    {
        [Fact]
        public async Task GetAllAsync_ShouldReturn200Status()
        {
            /// Arrange
            var addedWatchlistData = new Mock<IAddedWatchlistDataRepository>();
            addedWatchlistData.Setup(_ => _.GetAllAddedWatchlistData()).ReturnsAsync(AddedWatchlistMockData.GetWatchlistData());
            var sut = new AddedWatchlistDataController(addedWatchlistData.Object);

            /// Act
            var result = (OkObjectResult)await sut.GetAllAddedWatchlistData();

            // /// Assert
            result.StatusCode.Should().Be(200);
        }
    }
}