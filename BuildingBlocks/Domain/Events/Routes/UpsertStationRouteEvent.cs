using BuildingBlocks.Domain.Common;
using static System.Collections.Specialized.BitVector32;

namespace BuildingBlocks.Domain.Events.Routes
{
    public class UpsertStationRouteEvent : DomainBaseEvent
    {
        public Guid Id { get; set; }
       
        public double LengthInKm { get; set; }

        public ICollection<StationRouteEvent> StationRoutes { get; set; } = new List<StationRouteEvent>();

    }

    public class StationRouteEvent : DomainBaseEvent
    {
        public Guid StationId { get; set; }

        public Guid RouteId { get; set; }

        public int Order { get; set; }

        public double DistanceToNext { get; set; }
    }

}
