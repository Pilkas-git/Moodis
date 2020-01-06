using System;

using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Widget;
using Moodis.Extensions;
using Moodis.Feature.Group;
using Moodis.Feature.SignIn;
using Moodis.Widget;
using System.Collections.Generic;
using Moodis.Network;

namespace Moodis.History
{
    [Activity(Label = "History")]
    public class HistoryActivity : AppCompatActivity
    {
        private readonly HistoryViewModel historyViewModel = new HistoryViewModel();
        private RecyclerView RecyclerView;

        private const string EXTRA_REASON = "EXTRA_REASON";
        private const string EXTRA_NAME = "EXTRA_NAME";

        public static int extraReason; // 0 your own, 1 - group, 2 - person;
        public static string extraName;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetSupportActionBar();
            SetContentView(Resource.Layout.activity_history);

            extraName = Intent.GetStringExtra(EXTRA_NAME);
            extraReason = Intent.GetIntExtra(EXTRA_REASON, 0);

            AnimationExtension.AnimateBackground(FindViewById(Resource.Id.constraintLayoutHistory));
            InitView();
            InitAdapterAsync();

        }

        public override bool OnSupportNavigateUp()
        {
            OnBackPressed();
            return true;
        }

        private void InitView()
        {
            var dateInput = FindViewById<EditText>(Resource.Id.datePicker);
            dateInput.Click += (sender, e) =>
            {
                DatePickerFragment frag = DatePickerFragment.NewInstance(async delegate (DateTime time)
                {
                    dateInput.Text = time.ToLongDateString();

                    var ids = new List<string>();
                    switch (extraReason)
                    {
                        case 0:
                            ids.Add(SignInViewModel.currentUser.Id);
                            var itemList = await historyViewModel.FetchItemList(ids, time);
                            (RecyclerView.GetAdapter() as HistoryStatsAdapter).UpdateList(itemList);
                            break;
                        case 1:
                            ids = await GroupActivityModel.GetGroupUserIdsAsync(extraName);
                            itemList = await historyViewModel.FetchItemList(ids, time);
                            (RecyclerView.GetAdapter() as HistoryStatsAdapter).UpdateList(itemList);
                            break;
                        case 2:
                            var user = await API.UserEndpoint.GetUserByUsername(extraName);
                            ids.Add(user.Id);
                            itemList = await historyViewModel.FetchItemList(ids, time);
                            (RecyclerView.GetAdapter() as HistoryStatsAdapter).UpdateList(itemList);
                            break;
                    default:
                            ids.Add(SignInViewModel.currentUser.Id);
                            itemList = await historyViewModel.FetchItemList(ids, time);
                            (RecyclerView.GetAdapter() as HistoryStatsAdapter).UpdateList(itemList);
                            break;
                    }
                });
                frag.Show(SupportFragmentManager, DatePickerFragment.TAG);
            };
        }

        private async void InitAdapterAsync()
        {
            RecyclerView = FindViewById<RecyclerView>(Resource.Id.statsList);

            var layoutManager = new LinearLayoutManager(this);
            RecyclerView.SetLayoutManager(layoutManager);
            HistoryStatsAdapter adapter;
            var ids = new List<string>();

            switch (extraReason)
            {
                case 0:
                    var itemList = await historyViewModel.FetchItemList(ids, DateTime.UtcNow);
                    ids.Add(SignInViewModel.currentUser.Id);
                    adapter = new HistoryStatsAdapter(itemList);
                    break;
                case 1:
                    ids = await GroupActivityModel.GetGroupUserIdsAsync(extraName);
                    itemList = await historyViewModel.FetchItemList(ids, DateTime.UtcNow);
                    adapter = new HistoryStatsAdapter(itemList);
                    break;
                case 2:
                    var user = await API.UserEndpoint.GetUserByUsername(extraName);
                    ids.Add(user.Id);
                    itemList = await historyViewModel.FetchItemList(ids, DateTime.UtcNow);
                    adapter = new HistoryStatsAdapter(itemList);
                    break;
                default:
                    itemList = await historyViewModel.FetchItemList(ids, DateTime.UtcNow);
                    ids.Add(SignInViewModel.currentUser.Id);
                    adapter = new HistoryStatsAdapter(itemList);
                    break;
            }
            RecyclerView.SetAdapter(adapter);
        }
    }
}