using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms;


namespace myEventsApp
{
    public partial class AllEvents : ContentPage
    {
        GeocodePage geocodePage;
        string myAddress;
      

        public AllEvents()
        {
            //nearbyEvents = new NearbyEvents();
            InitializeComponent();

            //Initialise all events data
            initialiseData();

            var homeItem = new ToolbarItem
            {
                
                Text = "Home",
                Icon = "MyEventslogo-02.png"
            };

            this.ToolbarItems.Add(homeItem);
            
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Set syncItems to true in order to synchronize the data on startup when running in offline mode
            //await RefreshItems(true, syncItems: false);
        }

        /// <summary>
        /// Initialises initial dummy data
        /// </summary>
        /// <returns>The data.</returns>
        private async Task initialiseData()
        {
            
            var myEvent = new TodoItem
            {

                EventName = "My New data",
                EventStart = DateTime.Now.ToLocalTime(),
                EventFinish = DateTime.Now.ToLocalTime(),
                EventSuburb = "Bathurst",
                EventPostCode = 2795,
                EventStreetName = "Howick St",
                EventDescription = "Slinging match at mine",
                EventStreetNumber = 296,
                IsFinished = false

            };
            
            myAddress = myEvent.EventStreetNumber + " " + myEvent.EventStreetName + " , " + myEvent.EventSuburb + " , " + myEvent.EventPostCode;

            geocodePage = new GeocodePage();

        
            await geocodePage.getGeoLocation(myAddress);

            myEvent.EventLatitude = geocodePage.EventLatitude;
            myEvent.EventLongitude = geocodePage.EventLongitude;

            InitialiseEventData(myEvent);


        }
        /// <summary>
        /// Initialises the event data.
        /// </summary>
        /// <param name="myEvent">My event.</param>

        private async void InitialiseEventData(TodoItem myEvent)
        {
            // release this await to add dummy data to the azure cloud if needed
            //await TodoItemManager.DefaultManager.SaveTaskAsync(myEvent);

            // retrieve all data back from database for list initalisation
            var tableData = await TodoItemManager.DefaultManager.GetTodoItemsAsync();

            // Load the data onto the page
            todoList.ItemsSource = tableData;

            // Place the Pins on the map
            foreach (TodoItem value in tableData)
            {
                var position = new Position(value.EventLatitude, value.EventLongitude); // Latitude, Longitude
                string pinAddress = value.EventStreetNumber + " " + value.EventStreetName + " , " + value.EventSuburb + " , " + value.EventPostCode;

                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = position,
                    Label = value.EventName,
                    Address = pinAddress
                };
                MyMap.Pins.Add(pin);

                MyMap.MoveToRegion(
                MapSpan.FromCenterAndRadius(new Position(value.EventLatitude, value.EventLongitude), Distance.FromKilometers(5)));
            }
            // add to map

        }


        /// <summary>
        /// send todo object to eventresultsPage
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {

            if (((ListView)sender).SelectedItem == null)
                return;

            var myEv = e.SelectedItem as TodoItem;

            await Navigation.PushAsync(new EventResultsPage(myEv));
            ((ListView)sender).SelectedItem = null;


        }

        /// <summary>
        /// Searchs the item.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void searchItem(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new SearchResults(searchEntry.Text));

            // reset search field
            searchEntry.Text = "";
        }

        

    }
}
