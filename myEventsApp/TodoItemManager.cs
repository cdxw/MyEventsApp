// To add offline sync support: add the NuGet package Microsoft.Azure.Mobile.Client.SQLiteStore
// to all projects in the solution and uncomment the symbol definition OFFLINE_SYNC_ENABLED
// For Xamarin.iOS, also edit AppDelegate.cs and uncomment the call to SQLitePCL.CurrentPlatform.Init()
// For more information, see: http://go.microsoft.com/fwlink/?LinkId=620342 
//#define OFFLINE_SYNC_ENABLED

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Xamarin.Forms.Maps;

#if OFFLINE_SYNC_ENABLED
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
#endif

namespace myEventsApp
{
	

    public partial class TodoItemManager
    {
		//Geocoder geoCoder;

        static TodoItemManager defaultInstance = new TodoItemManager();
        MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceSyncTable<TodoItem> todoTable;
#else
        IMobileServiceTable<TodoItem> todoTable;
#endif
		/// <summary>
		/// Initializes a new instance of the <see cref="T:myEventsApp.TodoItemManager"/> class.
		/// </summary>
        private TodoItemManager()
        {
            this.client = new MobileServiceClient(
                Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore("localstore.db");
            store.DefineTable<TodoItem>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.todoTable = client.GetSyncTable<TodoItem>();
#else
            this.todoTable = client.GetTable<TodoItem>();
#endif
        }
		
		/// <summary>
		/// Gets the default manager.
		/// </summary>
		/// <value>The default manager.</value>
        public static TodoItemManager DefaultManager
        {
            get
            {
                return defaultInstance;
            }
            private set
            {
                defaultInstance = value;
            }
        }
		/// <summary>
		/// Gets the current client.
		/// </summary>
		/// <value>The current client.</value>
        public MobileServiceClient CurrentClient
        {
            get { return client; }
        }
		/// <summary>
		/// Gets a value indicating whether this <see cref="T:myEventsApp.TodoItemManager"/> is offline enabled.
		/// </summary>
		/// <value><c>true</c> if is offline enabled; otherwise, <c>false</c>.</value>
        public bool IsOfflineEnabled
        {
            get { return todoTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<TodoItem>; }
        }
		/// <summary>
		/// Gets the todo items async.
		/// </summary>
		/// <returns>The todo items async.</returns>
		/// <param name="syncItems">If set to <c>true</c> sync items.</param>
        public async Task<ObservableCollection<TodoItem>> GetTodoItemsAsync(bool syncItems = true)
        {
            try
            {
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
                IEnumerable<TodoItem> items = await todoTable
					.Where(todoItem => !todoItem.IsFinished)
                    .ToEnumerableAsync();

                return new ObservableCollection<TodoItem>(items);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }
            return null;
        }

		 public async Task<ObservableCollection<TodoItem>> GetSearchItemsAsync(string searchItem, bool syncItems = false)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<TodoItem> items = await todoTable
					.Where(todoItem => todoItem.EventName.Contains(searchItem))
					.ToEnumerableAsync();

				return new ObservableCollection<TodoItem>(items);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"Sync error: {0}", e.Message);
			}
			return null;
		}


		/// <summary>
		/// Gets the nearby items async.
		/// </summary>
		/// <returns>The nearby items async.</returns>
		/// <param name="syncItems">If set to <c>true</c> sync items.</param>
		public async Task<ObservableCollection<TodoItem>> GetNearbyItemsAsync(bool syncItems = true)
		{
			try
			{
#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<TodoItem> items = await todoTable
					.Where(todoItem => !todoItem.IsFinished)
					.ToEnumerableAsync();

				return new ObservableCollection<TodoItem>(items);
			}
			catch (MobileServiceInvalidOperationException msioe)
			{
				Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
			}
			catch (Exception e)
			{
				Debug.WriteLine(@"Sync error: {0}", e.Message);
			}
			return null;
		}

        /// <summary>
        /// Deletes the reviews.
        /// </summary>
        /// <returns>The reviews.</returns>
        /// <param name="evID">Ev identifier.</param>
        public async Task DeleteEvent(TodoItem delItem)
        {
            try
            {
                string evID = delItem.Id;

                await todoTable.DeleteAsync(delItem);
                // delete reviews associated with event
                await ReviewsItemManager.DefaultManager.DeleteReviews(evID);
            }
            catch (MobileServiceInvalidOperationException msioe)
            {
                Debug.WriteLine(@"Invalid sync operation: {0}", msioe.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(@"Sync error: {0}", e.Message);
            }

        }

        

        /// <summary>
        /// Saves the task async.
        /// </summary>
        /// <returns>The task async.</returns>
        /// <param name="item">Item.</param>
        public async Task SaveTaskAsync(TodoItem item)
        {
            if (item.Id == null)
            {
                await todoTable.InsertAsync(item);
            }
            else
            {
                await todoTable.UpdateAsync(item);
            }
        }

#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.todoTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allTodoItems",
                    this.todoTable.CreateQuery());
            }
            catch (MobileServicePushFailedException exc)
            {
                if (exc.PushResult != null)
                {
                    syncErrors = exc.PushResult.Errors;
                }
            }

            // Simple error/conflict handling. A real application would handle the various errors like network conditions,
            // server conflicts and others via the IMobileServiceSyncHandler.
            if (syncErrors != null)
            {
                foreach (var error in syncErrors)
                {
                    if (error.OperationKind == MobileServiceTableOperationKind.Update && error.Result != null)
                    {
                        //Update failed, reverting to server's copy.
                        await error.CancelAndUpdateItemAsync(error.Result);
                    }
                    else
                    {
                        // Discard local change.
                        await error.CancelAndDiscardItemAsync();
                    }

                    Debug.WriteLine(@"Error executing sync operation. Item: {0} ({1}). Operation discarded.", error.TableName, error.Item["id"]);
                }
            }
        }
#endif
    }
}
