using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;


public struct Weather
{
    public string Country { get; set; }
    public string Name { get; set; }
    public double Temp { get; set; }
    public string Description { get; set; }
}

public class OpenWeatherMapApi
{
    private readonly string apiKey;
    private readonly HttpClient httpClient;

    public OpenWeatherMapApi(string apiKey)
    {
        this.apiKey = apiKey;
        httpClient = new HttpClient();
    }


    public async Task<Weather?> GetWeatherAsync(double lat, double lon)
    {
        string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric";

            HttpResponseMessage response = await httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();
          
            var weatherData = await response.Content.ReadFromJsonAsync<WeatherResponse>();


            return new Weather
            {
                Country = weatherData.sys.country,
                Name = weatherData.name,
                Temp = weatherData.main.temp,
                Description = weatherData.weather[0].description
            };
        
        }
    


    public (double lat, double lon) GenerateRandomCoordinates()
    {
        Random random = new Random();
        return (random.NextDouble() * 180 - 90, random.NextDouble() * 360 - 180);
    }
}

public class WeatherResponse
{
    public Main main { get; set; }
    public Sys sys { get; set; }
    public string name { get; set; }
    public List<WeatherDescription> weather { get; set; }
}

public class Main
{
    public double temp { get; set; }
}

public class Sys
{
    public string country { get; set; }
}

public class WeatherDescription
{
    public string description { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        // Замените "YOUR_API_KEY" на ваш API ключ от OpenWeatherMap
        string apiKey = "";
        OpenWeatherMapApi api = new OpenWeatherMapApi(apiKey);
        List<Weather> weatherData = new List<Weather>();
        // Получение данных о погоде

        while (weatherData.Count < 50)
        {
            (double lat, double lon) = api.GenerateRandomCoordinates();
            Weather? weather = await api.GetWeatherAsync(lat, lon);
            
            weatherData.Add(weather.Value);
            Console.WriteLine(weather.Value.Temp);
            
        }


        // LINQ запросы
        var maxTempCountry = weatherData.OrderByDescending(w => w.Temp).First();
        var minTempCountry = weatherData.OrderBy(w => w.Temp).First();
        var avgTemp = weatherData.Average(w => w.Temp);
        var uniqueCountriesCount = weatherData.Select(w => w.Country).Distinct().Count();
        var specificDescriptions = weatherData.FirstOrDefault(w => w.Description == "clear sky" || w.Description == "rain" || w.Description == "few clouds");


        Console.WriteLine($"Страна с максимальной температурой: {maxTempCountry.Country}, {maxTempCountry.Name}, {maxTempCountry.Temp}°C");
        Console.WriteLine($"Страна с минимальной температурой: {minTempCountry.Country}, {minTempCountry.Name}, {minTempCountry.Temp}°C");
        Console.WriteLine($"Средняя температура: {avgTemp:F2}°C");
        Console.WriteLine($"Количество стран: {uniqueCountriesCount}");
    }
}
