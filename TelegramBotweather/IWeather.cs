using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotweather
{
    internal interface IWeather
    {
        string[] GetIWeatherAsync(string weather);
    }
}
