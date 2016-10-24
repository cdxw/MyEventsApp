using System;
namespace myEventsApp
{
	public class Reviews
	{
		[Newtonsoft.Json.JsonProperty("Id")]
		public string Id { get; set; }

		[Microsoft.WindowsAzure.MobileServices.Version]
		public string AzureVersion { get; set; }

		[Newtonsoft.Json.JsonProperty("reviewDescription")]
		public string ReviewDescription{ get; set; }

		[Newtonsoft.Json.JsonProperty("reviewDate")]
		public string ReviewDate { get; set; }

		[Newtonsoft.Json.JsonProperty("reviewImage")]
		public string ReviewImage { get; set; }

		[Newtonsoft.Json.JsonProperty("eventID")]
		public string EventID { get; set; }

	}
}
