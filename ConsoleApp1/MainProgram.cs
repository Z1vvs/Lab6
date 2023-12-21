using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FIS
{
    public class FlightInformationSystem
    {
        private List<Flight> flights;
        public FlightInformationSystem()
        {
            flights = new List<Flight>();
        }
        public void LoadFlightsFromJson(string filePath)
        {
            string jsonContent = File.ReadAllText(filePath);
            var flightData = JsonConvert.DeserializeObject<FlightData>(jsonContent);
            flights.AddRange(flightData.Flights);
        }
        public void SaveFlightsToJson(string filePath)
        {
            var flightData = new FlightData { Flights = flights };
            var jsonContent = JsonConvert.SerializeObject(flightData);
            File.WriteAllText(filePath, jsonContent);
        }
        public void AddFlight(Flight flight)
        {
            flights.Add(flight);
        }
        public void RemoveFlight(string flightNumber)
        {
            Flight flightToRemove = flights.FirstOrDefault(f => f.FlightNumber == flightNumber);
            if (flightToRemove != null)
                flights.Remove(flightToRemove);
        }
        public List<Flight> GetFlightsByAirline(string airline)
        {
            return flights.Where(f => f.Airline == airline)
                .OrderBy(f => f.DepartureTime)
                .ToList();
        }
        public List<Flight> GetDelayedFlights()
        {
            return flights.Where(f => f.Status == FlightStatus.Delayed)
                .OrderBy(f => f.DepartureTime)
                .ToList();
        }
        public List<Flight> GetFlightsByDepartureDate(string departureDateString)
        {
            return flights.Where(f => f.DepartureTime.ToString("yyyy-MM-dd") == departureDateString)
                .OrderBy(f => f.DepartureTime)
                .ToList();
        }
        public List<Flight> GetFlightsByTimeRangeAndDestination(string startTimeString, string endTimeString, string destination)
        {
            if (DateTime.TryParse(startTimeString, out DateTime startTime) && DateTime.TryParse(endTimeString, out DateTime endTime))
            {
                return flights.Where(f => f.DepartureTime >= startTime && f.ArrivalTime <= endTime && f.Destination == destination)
                    .OrderBy(f => f.DepartureTime)
                    .ToList();
            }
            else
            {
                Console.WriteLine("Invalid date format.");
                return new List<Flight>();
            }
        }
        public List<Flight> GetFlightsArrivedInLastHourOrByTimeRange(string endTimeString, double timeRangeInHours)
        {
            if (DateTime.TryParse(endTimeString, out DateTime endTime))
            {
                DateTime startTimeLastHour = endTime.AddHours(-timeRangeInHours);
                return flights.Where(f => f.ArrivalTime >= startTimeLastHour && f.ArrivalTime <= endTime)
                    .OrderBy(f => f.DepartureTime)
                    .ToList();
            }
            else
            {
                Console.WriteLine("Invalid time format.");
                return new List<Flight>();
            }
        }
        class Program
        {
            static void DisplayFlights(List<Flight> flights)
            {
                foreach (var flight in flights)
                {
                    Console.WriteLine($"Flight Number: {flight.FlightNumber}, Airline: {flight.Airline}, Departure Time: {flight.DepartureTime}, Status: {flight.Status}");
                }
            }
            static void Main(string[] args)
            {
                FlightInformationSystem flightSystem = new FlightInformationSystem();
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine("\nLoading Flights from JSON:");
                flightSystem.LoadFlightsFromJson("E:\\Visual Proggannent\\ConsoleApp1\\ConsoleApp1\\flights_data.json");
                Console.WriteLine("Loading Flights from JSON completed");
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                flightSystem.AddFlight(new Flight
                {
                    FlightNumber = "FL123",
                    Airline = "Airline1",
                    Destination = "Destination1",
                    DepartureTime = DateTime.Now.AddHours(1),
                    ArrivalTime = DateTime.Now.AddHours(3),
                    Status = FlightStatus.OnTime,
                    Duration = TimeSpan.FromHours(2),
                    AircraftType = "Boeing 747",
                    Terminal = "A"
                });
                flightSystem.SaveFlightsToJson("E:\\Visual Proggannent\\ConsoleApp1\\ConsoleApp1\\flights_data_modified.json");
                Console.WriteLine("\nAirline1 added");
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                flightSystem.AddFlight(new Flight
                {
                    FlightNumber = "FL456",
                    Airline = "Airline2",
                    Destination = "Destination2",
                    DepartureTime = DateTime.Now.AddHours(2),
                    ArrivalTime = DateTime.Now.AddHours(4),
                    Status = FlightStatus.Delayed,
                    Duration = TimeSpan.FromHours(2),
                    AircraftType = "Airbus A320",
                    Terminal = "B"
                });
                flightSystem.SaveFlightsToJson("E:\\Visual Proggannent\\ConsoleApp1\\ConsoleApp1\\flights_data_modified.json");
                Console.WriteLine("\nAirline2 added");
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                flightSystem.RemoveFlight("FL456");
                flightSystem.SaveFlightsToJson("E:\\Visual Proggannent\\ConsoleApp1\\ConsoleApp1\\flights_data_modified.json");
                Console.WriteLine("\nAirline2 deleted");
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine("\nFlights by Airline:");
                var airlineFlights = flightSystem.GetFlightsByAirline("WizAir");
                DisplayFlights(airlineFlights);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine("\nDelayed Flights:");
                var delayedFlights = flightSystem.GetDelayedFlights();
                DisplayFlights(delayedFlights);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine("\nFlights by Departure Date:");
                var departureDateFlights = flightSystem.GetFlightsByDepartureDate("2023-06-15");
                DisplayFlights(departureDateFlights);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine("\nFlights by Time Range and Destination:");
                var timeRangeFlights = flightSystem.GetFlightsByTimeRangeAndDestination("2023-05-1T23:59:59", "2023-05-31T23:59:59", "New York");
                DisplayFlights(timeRangeFlights);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                Console.WriteLine("\nFlights Arrived in Last Hour or Within Time Range:");
                var lastHourFlights = flightSystem.GetFlightsArrivedInLastHourOrByTimeRange("2023-05-31T23:59:59", 1);
                DisplayFlights(lastHourFlights);
                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }
        }
    }
}