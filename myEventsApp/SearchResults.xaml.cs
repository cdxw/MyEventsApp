using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace myEventsApp
{
    public partial class SearchResults : ContentPage
    {
        public SearchResults(string searchItem)
        {
            InitializeComponent();

            getSearchResults(searchItem);

            var homeItem3 = new ToolbarItem
            {

                Text = "Home",
                Icon = "MyEventslogo-02.png"
            };
 
            this.ToolbarItems.Add(homeItem3);

            homeItem3.Clicked += (object sender, System.EventArgs e) =>
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
        /// 
        /// </summary>
        /// <param name="searchItem"></param>
        public async void getSearchResults(string searchItem)
        {
            var eventData = await TodoItemManager.DefaultManager.GetSearchItemsAsync(searchItem);

            searchList.ItemsSource = eventData;
        }

        // Event handlers
        public async void OnSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var todo = e.SelectedItem as TodoItem;
            await Navigation.PushAsync(new EventResultsPage(todo));


        }


    }
}

