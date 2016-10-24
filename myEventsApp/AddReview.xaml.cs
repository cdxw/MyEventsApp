using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


namespace myEventsApp
{
    public partial class AddReview : ContentPage
    {
        TodoItem eventItem;

        Editor MyEditor = new Editor();

        public AddReview(TodoItem eventItem)
        {
            InitializeComponent();

            this.eventItem = eventItem;

            StackLayout stackLayout = new StackLayout();


            MyEditor = new Editor { };

            Label eventName = new Label
            {
                Text = eventItem.EventName
            };

            Label textName = new Label
            {
                Text = "Enter description of Event"
            };

            Button submit = new Button
            {
                Text = "Submit Review"
            };

            Padding = new Thickness(5, Device.OnPlatform(20, 5, 5), 5, 5);

            stackLayout.Children.Add(eventName);
            stackLayout.Children.Add(textName);
            stackLayout.Children.Add(MyEditor);

            stackLayout.Children.Add(submit);

            Content = stackLayout;


            submit.Clicked += SubmitReview;

            var homeItem1 = new ToolbarItem
            {

                Text = "Home",
                Icon = "MyEventslogo-02.png"
            };
            //homeItem.SetBinding(MenuItem.CommandProperty, "returnHome");
            this.ToolbarItems.Add(homeItem1);

            homeItem1.Clicked += (object sender, System.EventArgs e) =>
            {
                returnHome();
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
        /// Submit review item
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
        private async void SubmitReview(object sender, EventArgs e)
        {
            FormValidator fv = new FormValidator();
            // validate for empty values
            if (!fv.isValid(MyEditor.Text) || !fv.isValid(eventItem.Id))
            {
                
                DisplayAlert("Error", "Please enter a description", "OK");
                return;
            }
            else
            {
                Reviews currReview = new Reviews();
                currReview.EventID = eventItem.Id;
                currReview.ReviewDescription = MyEditor.Text;
                await ReviewsItemManager.DefaultManager.AddReviewItem(currReview);

                await Navigation.PushAsync(new EventResultsPage(eventItem));
            }
         


        }

       
    }
}
