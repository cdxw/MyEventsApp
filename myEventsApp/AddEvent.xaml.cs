using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Plugin.Media;

using Xamarin.Forms;

namespace myEventsApp
{
    public partial class AddEvent : ContentPage
    {
        public AddEvent()
        {
            InitializeComponent();

            var homeBtnItem = new ToolbarItem
            {

                Text = "Home",
                Icon = "MyEventslogo-02.png"
            };
            //homeItem.SetBinding(MenuItem.CommandProperty, "returnHome");
            this.ToolbarItems.Add(homeBtnItem);

            homeBtnItem.Clicked += (object sender, System.EventArgs e) =>
            {
                returnHome();
            };


            takePhoto.Clicked += async (sender, args) =>
            {

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
                    return;
                }

                var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions

                {
                    
                    Name = "event.jpg",
                    SaveToAlbum = true,
                   
                    
                });

                if (file != null)
                {
                    image.Source = ImageSource.FromStream(() => file.GetStream());
                    
                }
                else
                {
                    return;
                };
                
                
            };


        }

        /// <summary>
        /// Returns to home page
        /// </summary>
        public async void returnHome()
        {
            await Navigation.PushAsync(new AllEvents());
        }

       

        /// <summary>
        /// Adds the new event object to the database
        /// On completion navigates to the all events page
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        async void AddNewEvent(object sender, EventArgs e)
        {
            string myAddress;
            FormValidator fv = new FormValidator();
            
            // validate for empty values
          

            if (!fv.isValid(evName.Text) || fv.isValid(evStreetNum.Text) || fv.isValid(evDescription.Text) || fv.isValid(evPostcode.Text) || fv.isValid(evSuburb.Text))
            {
                DisplayAlert("Error", "No blank fields allowed", "OK");
                return;             
            }

            var myEvent = new TodoItem
            {

                EventName = evName.Text,
                EventStart = evStartDate.Date,
                EventFinish = evFinishDate.Date,
                EventSuburb = evSuburb.Text,
                EventPostCode = Int32.Parse(evPostcode.Text),
                EventStreetName = evStreet.Text,
                EventDescription = evDescription.Text,
                EventStreetNumber = Int32.Parse(evStreetNum.Text),
                IsFinished = false

            };

            // get the geo locations from the street address
            myAddress = myEvent.EventStreetNumber + " " + myEvent.EventStreetName + " , " + myEvent.EventSuburb + " , " + myEvent.EventPostCode;

            GeocodePage geocodePage = new GeocodePage();

            //await initialiseAddress(myAddress);

            await geocodePage.getGeoLocation(myAddress);

            myEvent.EventLatitude = geocodePage.EventLatitude;
            myEvent.EventLongitude = geocodePage.EventLongitude;

            await TodoItemManager.DefaultManager.SaveTaskAsync(myEvent);

            // reset the values
            evName.Text = "";
            evStartDate = null;
            evFinishDate = null;
            evSuburb.Text = "";
            evPostcode.Text = "";
            evStreet.Text = "";
            evDescription.Text = "";
            evStreetNum.Text = "";

            // Navigate to all events page
            await Navigation.PushAsync(new AllEvents());

        }


    }
}
