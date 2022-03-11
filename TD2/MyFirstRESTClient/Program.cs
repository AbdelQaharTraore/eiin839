using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Device.Location;
using MyFirstRESTClient.CalculatorServiceReference;

namespace MyFirstRESTClient // JCDecaux
{
    public class Position
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Station
    {
        public int number { get; set; }
        public string contract_name { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public Position position { get; set; }
        public bool banking { get; set; }
        public bool bonus { get; set; }
        public int bike_stands { get; set; }
        public int available_bike_stands { get; set; }
        public int available_bikes { get; set; }
        public string status { get; set; }
        public Int64 last_update { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }
    }

    internal class RestClient
    {
        static readonly HttpClient client = new HttpClient();
        static readonly string rootUrl = "https://api.jcdecaux.com/vls/v1/";
        static readonly string apiKey = "f5f2574c3604270c64f878e398b181acdaee4a18";

        public async void GetAllContracts()
        {
            try
            {
                string url = rootUrl + "contracts" + "?apiKey=" + apiKey;
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public async void GetAllStationsFromContract(string contractName)
        {
            try
            {
                string url = rootUrl + "stations?contract=" + contractName + "&apiKey=" + apiKey;
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);

            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public async Task<List<Station>> ReturnAllStationsFromContract(string contractName)
        {
            try
            {
                string url = rootUrl + "stations?contract=" + contractName + "&apiKey=" + apiKey;
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseBody);
                List<Station> stations = JsonSerializer.Deserialize<List<Station>>(responseBody);
                //Console.WriteLine(stations);
                return stations;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }

        public async void GetAllInformationFromStation(string contractName, int stationNumber)
        {
            try
            {
                string url = rootUrl + "stations/" + stationNumber + "?contract=" + contractName + "&apiKey=" + apiKey;
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                //Console.WriteLine(responseBody);
                Station station = JsonSerializer.Deserialize<Station>(responseBody);
                Console.WriteLine(station.name);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        public async Task<Station> GetNearestStation(string contractName, GeoCoordinate coordinate)
        {
            Station nearestStation = new Station();
            try
            {
                List<Station> stations = await ReturnAllStationsFromContract(contractName);
                double distance = -1;
                for (int i = 0; i < stations.Count; i++)
                {
                    Station station = stations[i];
                    double dst = coordinate.GetDistanceTo(new GeoCoordinate(station.position.lat, station.position.lng));
                    if ((dst < distance) || (distance < 0))
                    {
                        nearestStation = station;
                        distance = dst;
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return nearestStation;
        }

        public async void CalculateOnline(int a, int b)
        {
            Console.WriteLine("Nombres : " + a + " et " + b);
            Console.WriteLine("Calcul...");
            CalculatorSoapClient calculatorSoapClient = new CalculatorSoapClient();
            int addition = await calculatorSoapClient.AddAsync(a, b);
            int soustraction = await calculatorSoapClient.SubtractAsync(a, b);
            int multiplication = await calculatorSoapClient.MultiplyAsync(a, b);
            int division = await calculatorSoapClient.DivideAsync(a, b);
            Console.WriteLine("Addition: " + addition);
            Console.WriteLine("Soustraction: " + soustraction);
            Console.WriteLine("Multiplication: " + multiplication);
            Console.WriteLine("Division: " + division);
        }

        static async Task Main()
        {
            RestClient rc = new RestClient();
            //rc.GetAllContracts();
            string contractName = "bruxelles";
            //rc.GetAllStationsFromContract(contractName);
            int stationNumber = 333;
            //rc.GetAllInformationFromStation(contractName, stationNumber);
            //GeoCoordinate geo = new GeoCoordinate(50.856582, 4.430980);   // proche de 333 - TRACTEBEL (50.856582, 4.430982)
            GeoCoordinate geo = new GeoCoordinate(20.856582, 8.430988);
            Station s = await rc.GetNearestStation(contractName, geo);
            Console.WriteLine(s.name);
            rc.CalculateOnline(50, 10);
            Console.WriteLine("End");
            Console.ReadLine();
        }
    }
}
