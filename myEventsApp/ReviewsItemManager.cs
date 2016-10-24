using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;


namespace myEventsApp
{
	
	public class ReviewsItemManager
	{
		TodoItem todo = new TodoItem();
		string todoID = "";

		public void Reviews(string todoID)
		{
			this.todoID = todoID;
		}

		static ReviewsItemManager defaultInstance = new ReviewsItemManager();
		MobileServiceClient client;

#if OFFLINE_SYNC_ENABLED
        IMobileServiceTable<Reviews> reviewTable;
#else

		IMobileServiceTable<Reviews> reviewTable;
#endif
		/// <summary>
		/// Initializes a new instance of the <see cref="T:myEventsApp.ReviewsItemManager"/> class.
		/// </summary>
		private ReviewsItemManager()
		{
			this.client = new MobileServiceClient(
				Constants.ApplicationURL);

#if OFFLINE_SYNC_ENABLED
            var store = new MobileServiceSQLiteStore("localstore.db");
            store.DefineTable<Reviews>();

            //Initializes the SyncContext using the default IMobileServiceSyncHandler.
            this.client.SyncContext.InitializeAsync(store);

            this.reviewTable = client.GetSyncTable<Reviews>();
#else
			this.reviewTable = client.GetTable<Reviews>();
#endif
		}
		/// <summary>
		/// Adds the item.
		/// </summary>
		/// <returns>The item.</returns>
		/// <param name="item">Item.</param>
		// Data methods
		public async Task AddReviewItem(Reviews item)
		{
			await SaveReviewAsync(item);
		}
		/// <summary>
		/// Gets the default manager.
		/// </summary>
		/// <value>The default manager.</value>
		public static ReviewsItemManager DefaultManager
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
			get { return reviewTable is Microsoft.WindowsAzure.MobileServices.Sync.IMobileServiceSyncTable<Reviews>; }
		}

		/// <summary>
		/// Gets the reviews items async.
		/// </summary>
		/// <returns>The todo items async.</returns>
		/// <param name="syncItems">If set to <c>true</c> sync items.</param>
		public async Task<ObservableCollection<Reviews>> GetReviewItemsAsync(string eventID, bool syncItems = true)
		{
			try
			{
				reviewTable = client.GetTable<Reviews>();

#if OFFLINE_SYNC_ENABLED
                if (syncItems)
                {
                    await this.SyncAsync();
                }
#endif
				IEnumerable<Reviews> items = await reviewTable
					.Where(reviewItem => reviewItem.EventID == eventID)
					.ToEnumerableAsync();

				return new ObservableCollection<Reviews>(items);
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
		/// <param name="eventID">Event identifier.</param>
		public async Task DeleteReviews(string evID)
        {
            try
            {
                //reviewTable = client.GetTable<Reviews>();

                //var context = new FooContext(reviewTable);

                //var query = (from e in context.TableBar
                //			 where e.RowKey == rowKey
                //			 select e).AsTableServiceQuery();


                IEnumerable<Reviews> items = await reviewTable
                    .Where(reviewItem => reviewItem.EventID == evID)
                    .ToEnumerableAsync();

                foreach (Reviews item in items)
                {
                    await reviewTable.DeleteAsync(item);

                }
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
        /// Saves the review async.
        /// </summary>
        /// <returns>The review async.</returns>
        /// <param name="item">Item.</param>
        public async Task SaveReviewAsync(Reviews item)
		{
			if (item.Id == null)
			{
				await reviewTable.InsertAsync(item);
			}
			else
			{
				await reviewTable.UpdateAsync(item);
			}
		}

#if OFFLINE_SYNC_ENABLED
        public async Task SyncAsync()
        {
            ReadOnlyCollection<MobileServiceTableOperationError> syncErrors = null;

            try
            {
                await this.client.SyncContext.PushAsync();

                await this.reviewTable.PullAsync(
                    //The first parameter is a query name that is used internally by the client SDK to implement incremental sync.
                    //Use a different query name for each unique query in your program
                    "allReviewItems",
                    this.reviewTable.CreateQuery());
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
