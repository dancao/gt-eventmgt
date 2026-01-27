using EventManagementAPI.Domain.Entities;

namespace EventManagementAPI.UnitTest
{
    [TestClass]
    public sealed class VenueRepositoryTest
    {
        [TestMethod]
        [DataRow("Venue1", 50)]
        public void VenueCreateUpdateDelete_Success(string venueName, int capacity)
        {
            //
            Venue newVenue = new Venue();
            newVenue.Name = venueName;
            newVenue.Capacity = capacity;
            newVenue.CreatedOn = DateTime.UtcNow;

            // 

        }
    }
}
