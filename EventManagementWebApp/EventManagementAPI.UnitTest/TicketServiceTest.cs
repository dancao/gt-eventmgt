using EventManagementAPI.Data;
using EventManagementAPI.Domain.Entities;
using EventManagementAPI.Repositories.Interfaces;
using EventManagementAPI.Services;
using EventManagementAPI.ViewModels;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace EventManagementAPI.UnitTest
{
    [TestClass]
    public class TicketServiceTest
    {
        private DbContextOptions<AppDbContext> _options = null!;
        private Mock<ITicketRepository> _mockTicketRepo = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockTicketRepo = new Mock<ITicketRepository>();

            var dbName = $"test_{Guid.NewGuid():N}";
            _options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlite($"DataSource=file::memory:?cache=shared;Filename={dbName}.db")
                .Options;
            using var ctx = new AppDbContext(_options);
            ctx.Database.EnsureCreated();
        }

        [TestMethod]
        [DataRow(2, "James Bond", 2, "99.99")]
        public async Task PurchaseTicket_Success(long ticketTypeId, string buyerName, int quantity, string totalCost)
        {
            // Arrange
            var ticketDto = new PurchaseTicketDto { TicketTypeId = ticketTypeId, BuyerName = buyerName, Quantity = quantity, 
                TotalCost = decimal.Parse(totalCost)
            };

            await using var context = new AppDbContext(_options);

            _mockTicketRepo.Setup(x => x.UpdateTicketTypeAsync(ticketTypeId, quantity)).ReturnsAsync(1);
            _mockTicketRepo.Setup(x => x.AddTicketAsync(It.IsAny<Ticket>()));

            var service = new TicketService(context, _mockTicketRepo.Object);

            // Act
            var resutl = await service.PurchaseTicketAsync(ticketDto);

            // Assert
            Assert.IsTrue(resutl);
        }

        [TestMethod]
        [DataRow(2, "James Bond", 2, "99.99")]
        public async Task PurchaseTicket_Failed(long ticketTypeId, string buyerName, int quantity, string totalCost)
        {
            // Arrange
            var ticketDto = new PurchaseTicketDto
            {
                TicketTypeId = ticketTypeId,
                BuyerName = buyerName,
                Quantity = quantity,
                TotalCost = decimal.Parse(totalCost)
            };

            await using var context = new AppDbContext(_options);

            _mockTicketRepo.Setup(x => x.UpdateTicketTypeAsync(ticketTypeId, quantity)).ReturnsAsync(0);
            _mockTicketRepo.Setup(x => x.AddTicketAsync(It.IsAny<Ticket>()));

            var service = new TicketService(context, _mockTicketRepo.Object);

            // Act
            var resutl = await service.PurchaseTicketAsync(ticketDto);

            // Assert
            Assert.IsFalse(resutl);
        }
    }
}
