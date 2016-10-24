using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace myEventsApp
{
	public class TodoItem
	{
		//double eventName;
		//double eventLongitude;
		//string eventLatitude;
		//string address;
		//GeocodePage geo;


		//private string Address 
		//{			
		//	get { return getAddress(); }
		//}

		[Newtonsoft.Json.JsonProperty("Id")]
		public string Id { get; set; }

		[Microsoft.WindowsAzure.MobileServices.Version]
		public string AzureVersion { get; set; }

		[Newtonsoft.Json.JsonProperty("eventName")]
		public string EventName { get; set; }

		[Newtonsoft.Json.JsonProperty("eventStart")]
		public DateTime EventStart { get; set; }

		[Newtonsoft.Json.JsonProperty("eventFinish")]
		public DateTime EventFinish { get; set; }

		[Newtonsoft.Json.JsonProperty("eventStreetName")]
		public string EventStreetName { get; set; }

		[Newtonsoft.Json.JsonProperty("eventStreetNumber")]
		public  int EventStreetNumber { get; set; }

		[Newtonsoft.Json.JsonProperty("eventSuburb")]
		public string EventSuburb { get; set; }

		[Newtonsoft.Json.JsonProperty("eventPostCode")]
		public int EventPostCode { get; set; }

		[Newtonsoft.Json.JsonProperty("eventDescription")]
		public string EventDescription { get; set; }

		[Newtonsoft.Json.JsonProperty("eventLongitude")]
		public double EventLongitude { get; set; }

		[Newtonsoft.Json.JsonProperty("eventLatitude")]
		public double EventLatitude { get; set; }


		[Newtonsoft.Json.JsonProperty("isFinished")]
		public bool IsFinished { get; set; }



		public override string ToString()
		{

			return EventName + " " + EventStreetName + " , " + EventSuburb + " , " + EventPostCode;

		}

		public string EventInfo()
		{
			return EventName + ", " + EventStart.Date.ToString();
		}


	}
}

