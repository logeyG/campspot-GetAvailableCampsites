using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GetAvailableCampsites.Test
{
    [TestClass]
    public class Tests
    {

        public GetAvailableCampsitesRequest MockRequestSimple(SearchDates searchDates, 
            List<GapRule> gapRules, 
            List<Reservation> reservations)
        {
            var campsites = new List<Campsite>
            {
                new Campsite
                {
                    Id = 1,
                    Name = "Campsite A"
                },
                new Campsite
                {
                    Id = 2,
                    Name = "Campsite B"
                },
                new Campsite
                {
                    Id = 3,
                    Name = "Campsite C"
                }
            };

            return CreateMockRequest(searchDates, campsites, gapRules, reservations);
        }

        public GetAvailableCampsitesRequest CreateMockRequest(SearchDates search,
            List<Campsite> campsites, List<GapRule> gapRules, List<Reservation> reservations)
        {
            return new GetAvailableCampsitesRequest
            {
                Search = search,
                Campsites = campsites,
                GapRules = gapRules,
                Reservations = reservations
            };
        }

        [TestMethod]
        public void Should_Accept_Request_With_No_Reservations()
        {
            var searchDates = new SearchDates
            {
                StartDate = new DateTime(2016, 1, 10),
                EndDate = new DateTime(2016, 1, 15)
            };

            var gapRules = new List<GapRule>
            {
                new GapRule
                {
                    GapSize = 1
                }
            };

            var request = MockRequestSimple(searchDates, gapRules, new List<Reservation>());
            var campsites = GetAvailableCampsitesProgram.GetAvailableCampsites(request);

            Assert.AreEqual(request.Campsites.Count, campsites.Count);
        }

        [TestMethod]
        public void Should_Accept_Request_With_No_GapRules()
        {
            var searchDates = new SearchDates
            {
                StartDate = new DateTime(2016, 1, 10),
                EndDate = new DateTime(2016, 1, 15)
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CampsiteId = 1,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 6)
                },
                new Reservation
                {
                    CampsiteId = 3,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 6)
                }
            };

            var request = MockRequestSimple(searchDates, new List<GapRule>(), reservations);
            var campsites = GetAvailableCampsitesProgram.GetAvailableCampsites(request);

            Assert.AreEqual(request.Campsites.Count, campsites.Count);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void Should_Error_Request_With_No_Campsites()
        {
            var searchDates = new SearchDates
            {
                StartDate = new DateTime(2016, 1, 10),
                EndDate = new DateTime(2016, 1, 15)
            };

            var gapRules = new List<GapRule>
            {
                new GapRule
                {
                    GapSize = 1
                }
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CampsiteId = 1,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 6)
                },
                new Reservation
                {
                    CampsiteId = 3,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 6)
                }
            };

            var request = CreateMockRequest(searchDates, new List<Campsite>(), gapRules, reservations);
            var campsites = GetAvailableCampsitesProgram.GetAvailableCampsites(request);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void Should_Error_Request_With_No_Search_Dates()
        {

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CampsiteId = 1,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 6)
                },
                new Reservation
                {
                    CampsiteId = 3,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 6)
                }
            };

            var request = MockRequestSimple(null, new List<GapRule>(), reservations);
            var campsites = GetAvailableCampsitesProgram.GetAvailableCampsites(request);
        }

        [TestMethod]
        public void Should_Remove_Unavailable_Campsites()
        {
            var searchDates = new SearchDates
            {
                StartDate = new DateTime(2016, 1, 10),
                EndDate = new DateTime(2016, 1, 15)
            };

            var gapRules = new List<GapRule>
            {
                new GapRule
                {
                    GapSize = 1
                }
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CampsiteId = 1,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 16)
                },
                new Reservation
                {
                    CampsiteId = 3,
                    StartDate = new DateTime(2016, 1, 14),
                    EndDate = new DateTime(2016, 1, 15)
                }
            };

            var request = MockRequestSimple(searchDates, gapRules, reservations);
            var campsites = GetAvailableCampsitesProgram.RemoveUnavailableCampsites(request);

            Assert.AreEqual("Campsite B", campsites.Select(x => x.Name).Single());
        }

        [TestMethod]
        public void Should_Return_False_For_Valid_Reservations()
        {
            var gapRules = new List<GapRule>
            {
                new GapRule
                {
                    GapSize = 1
                }
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CampsiteId = 1,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 16)
                },
                new Reservation
                {
                    CampsiteId = 0,
                    StartDate = new DateTime(2016, 1, 18),
                    EndDate = new DateTime(2016, 1, 19)
                }
            };

            var response = GetAvailableCampsitesProgram.HasValidReservations(reservations, gapRules);
            Assert.AreEqual(false, response);
        }

        [TestMethod]
        public void Should_Ignore_Already_Added_Reservations()
        {
            var gapRules = new List<GapRule>
            {
                new GapRule
                {
                    GapSize = 0
                },
                new GapRule
                {
                    GapSize = 1
                },
                new GapRule
                {
                    GapSize = 2
                }
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CampsiteId = 1,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 16)
                },
                new Reservation
                {
                    CampsiteId = 2,
                    StartDate = new DateTime(2016, 1, 17),
                    EndDate = new DateTime(2016, 1, 19)
                },
                 new Reservation
                {
                    CampsiteId = 2,
                    StartDate = new DateTime(2016, 1, 20),
                    EndDate = new DateTime(2016, 1, 21)
                }
            };

            var response = GetAvailableCampsitesProgram.HasValidReservations(reservations, gapRules);
            Assert.AreEqual(true, response);
        }

        [TestMethod]
        public void Should_Return_True_For_Valid_Reservations()
        {
            var gapRules = new List<GapRule>
            {
                new GapRule
                {
                    GapSize = 2
                }
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CampsiteId = 1,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 16)
                },
                new Reservation
                {
                    CampsiteId = 0,
                    StartDate = new DateTime(2016, 1, 18),
                    EndDate = new DateTime(2016, 1, 19)
                }
            };

            var response = GetAvailableCampsitesProgram.HasValidReservations(reservations, gapRules);
            Assert.AreEqual(true, response);
        }

        [TestMethod]
        public void Should_Return_True_For_One_Reservation()
        {
            var gapRules = new List<GapRule>
            {
                new GapRule
                {
                    GapSize = 2
                }
            };

            var reservations = new List<Reservation>
            {
                new Reservation
                {
                    CampsiteId = 1,
                    StartDate = new DateTime(2016, 1, 5),
                    EndDate = new DateTime(2016, 1, 16)
                }
            };

            var response = GetAvailableCampsitesProgram.HasValidReservations(reservations, gapRules);
            Assert.AreEqual(true, response);
        }
    }
}
