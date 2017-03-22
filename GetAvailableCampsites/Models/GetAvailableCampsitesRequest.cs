using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetAvailableCampsites
{
    public class GetAvailableCampsitesRequest
    {
        public SearchDates Search { get; set; }
        public List<Campsite> Campsites { get; set; }
        public List<GapRule> GapRules { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}
