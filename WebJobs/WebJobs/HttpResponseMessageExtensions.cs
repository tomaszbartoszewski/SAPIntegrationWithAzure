using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OutboundStudent
{
    public static class HttpResponseMessageExtensions
    {
        public static object MessageConfiguration { get; private set; }

        public static async Task<T> GetBodyFromJsonAsync<T>(this HttpResponseMessage response)
        {
            string messageBody;
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                var reader = new StreamReader(stream);
                messageBody = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<T>(messageBody);
        }

        public static async Task<string> GetErrorMessage(this HttpResponseMessage message, string apiRoute)
        {
            var contentErrors = await GetContentErrorMessage(message);

            return string.Join(Environment.NewLine,
                $"Request to {apiRoute} failed.",
                contentErrors);
        }

        private static async Task<string> GetContentErrorMessage(this HttpResponseMessage message)
        {
            var contentString = await message.Content.ReadAsStringAsync();
            Dictionary<string, object> properties = null;
            try
            {
                properties = JsonConvert.DeserializeObject<Dictionary<string, object>>(contentString);
            }
            catch (JsonReaderException)
            {
                // swallow json reading exception, content may not be json formatted.
            }


            return properties == null
                ? string.Join(Environment.NewLine, "Could not parse message. Raw Content:", contentString)
                : GetContentErrorMessage(properties);
        }

        private static string GetContentErrorMessage(IDictionary<string, object> propertyDictionary)
        {
            return propertyDictionary.ContainsKey("Message")
                ? GetContentErrorMessage("Error Message", propertyDictionary["Message"])
                : string.Join(Environment.NewLine, propertyDictionary.Select(GetContentErrorMessage));
        }

        private static string GetContentErrorMessage(KeyValuePair<string, object> property)
            => GetContentErrorMessage(property.Key, property.Value);

        private static string GetContentErrorMessage(string key, object value) => $"{key}: {value}";
    }
}
