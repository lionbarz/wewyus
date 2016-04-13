using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Wewy.Common;
using Wewy.Models;

namespace Wewy.Services
{
    public class LocationService
    {
        private class GoogleReverseGeocodeResult
        {
            public List<GoogleReverseGeocodeResultItem> Results { get; set; }
            public string Status { get; set; }
        }

        private class GoogleReverseGeocodeResultItem
        {
            public List<GoogleReverseGeocodeAddressComponent> Address_Components { get; set; }
        }

        private class GoogleReverseGeocodeAddressComponent
        {
            public string Long_Name { get; set; }
            public string Short_Name { get; set; }
            public List<string> Types { get; set; }
        }

        public static Location ReverseGeocode(Position position)
        {
            UriBuilder uri = new UriBuilder("https://maps.googleapis.com/maps/api/geocode/json");
            uri.Query = string.Format(
                "latlng={0},{1}&key={2}",
                position.Latitude,
                position.Longitude,
                Constants.GOOGLE_MAPS_API_KEY);
            WebRequest request = WebRequest.Create(uri.Uri);
            // Get the response.
            WebResponse response = request.GetResponse();
            // Display the status.
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.
            Stream dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.
            string responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
            // Clean up the streams and the response.
            reader.Close();
            response.Close();

            JavaScriptSerializer ser = new JavaScriptSerializer();
            GoogleReverseGeocodeResult result = ser.Deserialize<GoogleReverseGeocodeResult>(responseFromServer);

            if (result.Status == null || !result.Status.Equals("OK"))
            {
                // Something went wrong. TODO: Log it somewhere!
                // Just return null and we won't store the city name but everything else will work.
                return null;
            }

            if (result.Results.Count == 0 || 
                result.Results[0].Address_Components == null)
            {
                // TODO: Also log somewhere.
                return null;
            }

            GoogleReverseGeocodeAddressComponent cityComponent = TryGetComponent(result, "neighborhood");

            if (cityComponent == null)
            {
                cityComponent = TryGetComponent(result, "locality");
            }

            if (cityComponent == null)
            {
                cityComponent = TryGetComponent(result, "administrative_area_level_3");
            }

            if (cityComponent == null)
            {
                cityComponent = TryGetComponent(result, "administrative_area_level_2");
            }

            if (cityComponent == null)
            {
                cityComponent = TryGetComponent(result, "administrative_area_level_1");
            }

            string city = null;

            if (cityComponent != null)
            {
                city = cityComponent.Long_Name;
            }

            GoogleReverseGeocodeAddressComponent countryComponent = TryGetComponent(result, "country");

            string country = null;

            if (countryComponent != null)
            {
                country = countryComponent.Long_Name;
            }

            if (string.IsNullOrEmpty(country) && string.IsNullOrEmpty(city))
            {
                // Don't bother with constructing location object.
                return null;
            }

            return new Location()
            {
                City = city,
                Country = country
            };
        }

        private static GoogleReverseGeocodeAddressComponent TryGetComponent(GoogleReverseGeocodeResult result, string type)
        {
            return result
                .Results[0]
                .Address_Components
                .Where(x => x.Types.Any(t => t.Equals(type)))
                .FirstOrDefault();
        }

        public static void UpdateUserLocation(
            ApplicationDbContext context,
            ApplicationUser user,
            Position position,
            int timezoneOffsetMinutes)
        {
            if (position == null)
            {
                // We don't know where the dude is.
                // If it's the same offset as the last time, just keep the last position.
                if (user.TimezoneOffsetMinutes == timezoneOffsetMinutes)
                {
                    // Nothing to do here.
                    return;
                }
                else
                {
                    // Dude is in new time zone but we don't know his position.
                    // Overwrite the stale position data.
                    user.TimezoneOffsetMinutes = timezoneOffsetMinutes;
                    user.Latitude = 0.0;
                    user.Longitude = 0.0;
                    user.City = null;
                    user.Country = null;
                }
            }
            else
            {
                user.Latitude = position.Latitude;
                user.Longitude = position.Longitude;       

                Location location = ReverseGeocode(position);

                if (location != null)
                {
                    user.City = location.City;
                    user.Country = location.Country;
                }
                else if (user.TimezoneOffsetMinutes != timezoneOffsetMinutes)
                {
                    // The user is somewhere different than last time but we don't know where.
                    user.City = null;
                    user.Country = null;
                }

                user.TimezoneOffsetMinutes = timezoneOffsetMinutes;
            }
        }
    }
}