using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace TelegramBotweather
{
    internal class Weather
    {
        public async Task<WeatherMain> GetIWeatherAsync(string weather)
        {
            string line = "S";
           
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={weather}&appid=783fe1bd1ed18be1f425d6ed83cd971a");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    HttpContent responseContent = response.Content;
                    line = await responseContent.ReadAsStringAsync();
                }
            }
            WeatherMain weatherInfo = JsonConvert.DeserializeObject<WeatherMain>(line);
            return weatherInfo;
        }
    }
}
