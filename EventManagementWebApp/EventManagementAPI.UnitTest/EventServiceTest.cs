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
    public sealed class EventServiceTest
    {
        private Mock<AppDbContext> _mockContext = null!;
        private Mock<IVenueRepository> _mockVenueRepo = null!;
        private Mock<IEventRepository> _mockEventRepo = null!;
        private Mock<IPricingRepository> _mockPricingRepo = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockContext = new Mock<AppDbContext>();
            _mockVenueRepo = new Mock<IVenueRepository>();
            _mockEventRepo = new Mock<IEventRepository>();
            _mockPricingRepo = new Mock<IPricingRepository>();
        }

        [TestMethod]
        [DataRow("Venue1", 50)]
        [DataRow("Venue2", 60)]
        public async Task VenueCreate_Success(string venueName, int capacity)
        {
            // Arrange
            var venueDto = new VenueDto { Name = venueName, IsActive = true, Capacity = capacity };

            _mockVenueRepo.Setup(x => x.AddAsync(It.IsAny<Venue>()));
            int saveChangesCallCount = 0;
            _mockContext
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => saveChangesCallCount++)
                .ReturnsAsync(1);

            var service = new EventService(_mockVenueRepo.Object, _mockEventRepo.Object, _mockContext.Object, _mockPricingRepo.Object);

            // Act
            await service.AddVenueAsync(venueDto);

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        [DataRow(1, "Venue1", 50)]
        [DataRow(2, "Venue2", 60)]
        public async Task VenueUpdate_Success(int id, string venueName, int capacity)
        {
            // Arrange
            var venueDto = new VenueDto { Id = id, Name = venueName, IsActive = true, Capacity = capacity };
            var expectedVenue = new Venue { Id = id, Name = venueName, IsActive = true, Capacity = capacity };

            _mockVenueRepo.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(expectedVenue);
            _mockVenueRepo.Setup(x => x.UpdateAsync(It.IsAny<Venue>()));
            int saveChangesCallCount = 0;
            _mockContext
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => saveChangesCallCount++)
                .ReturnsAsync(1);

            var service = new EventService(_mockVenueRepo.Object, _mockEventRepo.Object, _mockContext.Object, _mockPricingRepo.Object);

            // Act
            await service.UpdateVenueAsync(venueDto);

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [TestMethod]
        [DataRow("Event 1", 1)]
        public async Task EventCreate_Success(string name, long venueId)
        {
            // Arrange
            var ticketTypes = new List<TicketTypeDto>() {
                new TicketTypeDto(){ Name = "VIP", TotalAvailable=5, Remaining = 5, PricingTierId=1 },
                new TicketTypeDto(){ Name = "General", TotalAvailable=5, Remaining = 5, PricingTierId=1 }};
            var eventDto = new EventDto { Name = name, IsActive = true, VenueId= venueId, EventDate = DateTime.UtcNow, Duration=2,
            TicketTypes = ticketTypes };
            var expectedVenue = new Venue { Id = 1, Name = "Venue 1", IsActive = true, Capacity = 10 };
            var expectedPricingTier = new PricingTier { Id = 1, Name = "PC 1", IsActive = true, Price = 12 };

            _mockPricingRepo.Setup(x => x.GetPricingTierByIdAsync(It.IsAny<long>())).ReturnsAsync(expectedPricingTier);

            _mockVenueRepo.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(expectedVenue);

            _mockEventRepo.Setup(x => x.IsVenueAvailable(It.IsAny<Event>())).ReturnsAsync(true);
            _mockEventRepo.Setup(x => x.AddAsync(It.IsAny<Event>()));

            int saveChangesCallCount = 0;
            _mockContext
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Callback(() => saveChangesCallCount++)
                .ReturnsAsync(1);

            var service = new EventService(_mockVenueRepo.Object, _mockEventRepo.Object, _mockContext.Object, _mockPricingRepo.Object);

            // Act
            await service.AddEventAsync(eventDto);

            // Assert
            _mockContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }
    }
}
