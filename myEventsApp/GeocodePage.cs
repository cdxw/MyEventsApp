using System;
using Xamarin.Forms.Maps;
using System.Threading.Tasks;


namespace myEventsApp
{
	public class GeocodePage
	{
		//string address;
		private double eventLongitude;
		private double eventLatitude;

		Geocoder geoCoder;
		
		public GeocodePage(){
			
		}

		public double EventLongitude
		{
			get { return eventLongitude; }
			set { eventLongitude = value; }
		}
		public double EventLatitude
		{
			get { return eventLatitude; }
			set { eventLatitude = value;}

		}

		/// <summary>
		/// Gets the geo location from the address
		/// </summary>
		/// <returns>The geo location.</returns>
		/// <param name="address">Address.</param>
		public async Task getGeoLocation(string strAddress)
		{
			geoCoder = new Geocoder();

			var approximateLocations = await geoCoder.GetPositionsForAddressAsync(strAddress);

			foreach (var position in approximateLocations)
			{
				EventLatitude = position.Latitude;
				EventLongitude = position.Longitude;

			}


		}

	}
}
