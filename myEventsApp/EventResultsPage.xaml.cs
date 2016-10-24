using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.Maps;
using Xamarin.Forms;
using System.Threading.Tasks;


namespace myEventsApp
{
    public partial class EventResultsPage : ContentPage
    {
        TodoItem eventItem;

        string eventName;

        public EventResultsPage(TodoItem eventItem)
        {
            InitializeComponent();

            this.eventItem = eventItem;

            var button = new Button
            {
                Text = "Back",
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            button.Clicked += OnBackButtonClicked;

            var homeItem2 = new ToolbarItem
            {

                Text = "Home",
                Icon = "MyEventslogo-02.png"
            };
            //homeItem.SetBinding(MenuItem.CommandProperty, "returnHome");
            this.ToolbarItems.Add(homeItem2);

            homeItem2.Clicked += (object sender, System.EventArgs e) =>
            {
                returnHome();
            };


            eventNameLbl.Text = eventItem.EventName;

            //nameLabel.SetBinding (Label.TextProperty, "Name");
            string eventID = eventItem.Id;
            double eventLatitude = eventItem.EventLatitude;
            double eventLongitude = eventItem.EventLongitude;
            int eventStreetNumber = eventItem.EventStreetNumber;
            string eventStreetName = eventItem.EventStreetName;
            string eventSuburb = eventItem.EventSuburb;
            int eventPostCode = eventItem.EventPostCode;
            eventName = eventItem.EventName;


            var position = new Position(eventLatitude, eventLongitude); // Latitude, Longitude
            string pinAddress = eventStreetNumber + " " + eventStreetName + " , " + eventSuburb + " , " + eventPostCode;

            var pin = new Pin
            {
                Type = PinType.Place,
                Position = position,
                Label = eventName,
                Address = pinAddress
            };
            MyResultMap.Pins.Add(pin);

            MyResultMap.MoveToRegion(
            MapSpan.FromCenterAndRadius(new Position(eventLatitude, eventLongitude), Distance.FromKilometers(1)));

            getReviews(eventID);

        }

        /// <summary>
        /// Returns to home page
        /// </summary>
        public async void returnHome()
        {
            await Navigation.PushAsync(new AllEvents());
        }

        /// <summary>
        /// Checks the reviews.
        /// </summary>
        /// <param name="eventID">Event identifier.</param>
        public async void getReviews(string eventID)
        {
            var reviewData = await ReviewsItemManager.DefaultManager.GetReviewItemsAsync(eventID);

            eventReviewList.ItemsSource = reviewData;
        }
        /// <summary>
        /// Ons the back button clicked.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        /// <summary>
        /// Adds the review.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        public async void AddReview(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new AddReview(eventItem));
        }
        /// <summary>
		/// Deletes the event.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public async void DeleteEvent(object sender, EventArgs e)
        {
            try
            {
                await TodoItemManager.DefaultManager.DeleteEvent(eventItem);
                await Navigation.PushAsync(new AllEvents());
                await DisplayAlert("Event Deleted", "Event Completed", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Sorry couldnt delete Event: " + ex, "OK");
            }

        }


    }
}

