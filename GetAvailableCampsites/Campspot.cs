using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;

namespace GetAvailableCampsites
{
    public class Campspot
    {
        private static readonly int NEW_RESERVATION = 0;

        static void Main(string[] args)
        {
            if (args.Length == 1 && args[0].Contains(".json"))
            {
                var campsites = GetAvailableCampsites(LoadJson(args[0]));
                campsites.ForEach(x => Console.WriteLine(x.Name));
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine("This program only accepts 1 command line argument that must be the path of a valid .json file.");
                Console.ReadLine();
            }          
        }

        public static GetAvailableCampsitesRequest LoadJson(string filename)
        {
            using (var r = new StreamReader(filename))
            {
                var json = r.ReadToEnd();
                var item = JsonConvert.DeserializeObject<GetAvailableCampsitesRequest>(json);
                return item;                          
            }
        }

        public static void ValidateRequest(GetAvailableCampsitesRequest request)
        {
            if (request.Search == null)
            {
                throw new ArgumentException("You must submit search dates");
            }

            if (request.Search.EndDate < request.Search.StartDate)
            {
                throw new ArgumentException("The search end date must be AFTER the search start date");
            }

            if (request.Campsites.Count == 0 || request.Campsites == null)
            {
                throw new ArgumentException("You must select campsites in the request");
            }
        }

        public static List<Campsite> GetAvailableCampsites(GetAvailableCampsitesRequest request)
        {
            ValidateRequest(request);

            var possibleCampsites = RemoveUnavailableCampsites(request);

            var availableCampsites = new List<Campsite>();
            foreach (var campsite in possibleCampsites)
            {

                var reservations = request.Reservations.Where(x => x.CampsiteId == campsite.Id).ToList();

                reservations.Add(new Reservation
                {
                    CampsiteId = NEW_RESERVATION,
                    StartDate = request.Search.StartDate,
                    EndDate = request.Search.EndDate
                });

                if (HasValidReservations(reservations, request.GapRules))
                    availableCampsites.Add(campsite);
            }

            return availableCampsites;
        }

        public static bool HasValidReservations(List<Reservation> reservations, List<GapRule> gapRules)
        {

            if (reservations.Count == 1)
                return true;

            reservations = reservations.OrderBy(x => x.EndDate).ToList();

            for (var i = 0; i < reservations.Count - 1; i++)
            {
                if (reservations[i + 1].CampsiteId == NEW_RESERVATION || reservations[i].CampsiteId == NEW_RESERVATION)
                {
                    var gap = (reservations[i + 1].StartDate - reservations[i].EndDate).Days - 1;

                    if (gapRules.Any(x => x.GapSize == gap))
                        return false;
                }
            }

            return true;
        }

        public static List<Campsite> RemoveUnavailableCampsites(GetAvailableCampsitesRequest request)
        {
            var unavailableCampsites =
               request.Reservations.Where(
                   x => !(request.Search.StartDate > x.EndDate || x.StartDate > request.Search.EndDate)).Select(x => x.CampsiteId);

            return request.Campsites.Where(x => !unavailableCampsites.Contains(x.Id)).ToList();
        }
    }
}
