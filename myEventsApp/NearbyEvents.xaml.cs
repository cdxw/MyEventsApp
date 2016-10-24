using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Geolocator;


namespace myEventsApp
{
    public partial class NearbyEvents : ContentPage
    {
        public NearbyEvents()
        {
            InitializeComponent();

            var goItem = new ToolbarItem
            {

                Text = "Home",
                Icon = "MyEventslogo-02.png"
            };

            this.ToolbarItems.Add(goItem);

            goItem.Clicked += (object sender, System.EventArgs e) =>
            {
                returnHome();
            };

            InitialiseNearbyEventData();
        }
        

        /// <summary>
        /// Returns to home page
        /// </summary>
        public async void returnHome()
        {
            await Navigation.PushAsync(new AllEvents());
        }

        /// <summary>
        /// Initialises the event data.
        /// </summary>
        /// <param name="myEvent">My event.</param>
        public async Task InitialiseNearbyEventData()
        {
            

            var tableData = await TodoItemManager.DefaultManager.GetNearbyItemsAsync();

            //// Load the data onto the page
            nearbyList.ItemsSource = tableData;

            //// Place the Pins on the map
            foreach (TodoItem value in tableData)
            {
                var position = new Position(value.EventLatitude, value.EventLongitude); // Latitude, Longitude
                string pinAddress = value.EventStreetNumber + " " + value.EventStreetName + " , " + value.EventSuburb + " , " + value.EventPostCode;
                string evName = value.EventName;

                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = position,
                    Label = evName,
                    Address = pinAddress
                };
                MyNearbyMap.Pins.Add(pin);

                // move to cuurent location of phone
                var locator = CrossGeolocator.Current;
                locator.DesiredAccuracy = 50;

                var currPosition = await locator.GetPositionAsync(10000);

                double currLat = currPosition.Latitude;
                double currLong = currPosition.Longitude;
                
                MyNearbyMap.MoveToRegion(
                    MapSpan.FromCenterAndRadius(new Position(currLat, currLong), Distance.FromKilometers(5)));

            }

        }


        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var todo = e.SelectedItem as TodoItem;


            await Navigation.PushAsync(new EventResultsPage(todo));


        }
    }
}
