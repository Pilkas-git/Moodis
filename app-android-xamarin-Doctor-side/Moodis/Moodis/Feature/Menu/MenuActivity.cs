using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Moodis.Extensions;
using Moodis.Feature.Group;
using Moodis.Feature.Login;
using Moodis.Feature.SignIn;
using Moodis.History;
using Moodis.Network;
using Moodis.Ui;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Moodis.Feature.Menu
{
    [Activity(Label = "Menu", Theme = "@style/AppTheme.NoActionBar")]
    public class MenuActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private readonly string TAG = nameof(MenuActivity);
        RecyclerView recyclerView;
        private MenuViewModel MenuViewModel;
        private int spinnerPosition;
        private const string FormatDouble = "N3";
        private UserListAdapter adapterUserList;
        private const string USERS_WITHOUT_GROUP = "USERS_WITHOUT_GROUP";

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MenuViewModel = MenuViewModel.Instance;
            SetContentView(Resource.Layout.activity_menu);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);


            recyclerView = FindViewById<RecyclerView>(Resource.Id.userRecyclerView);
            recyclerView.SetLayoutManager(new LinearLayoutManager(this));

            /* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            TODO THIS SHOULD LATER CHECK IF USER ALREADY HAS A GROUP AS A USER CAN ONLY HAVE 1 GROUP BECAUSE OF THE FACE API STUFF
            !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!*/
            SignInViewModel.userList = await API.UserEndpoint.GetALLUsers();
            var userList = SignInViewModel.userList.Where(user => !user.IsDoctor).Where(user => user.GroupId == null).ToList();
            adapterUserList = new UserListAdapter(userList);
            recyclerView.SetAdapter(adapterUserList);

            InitialiseInputs();

            //AnimationExtension.AnimateBackground(FindViewById(Resource.Id.menuActivity));
        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if (drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
                Finish();
            }
        }

        private void LogoutWindowShow()
        {
            var dialog = this.ConfirmationAlert(
                    titleRes: Resource.String.logout,
                    messageRes: Resource.String.logout_confirmation_message,
                    positiveButtonRes: Resource.String.yes,
                    negativeButtonRes: Resource.String.no,
                    positiveCallback: delegate { HandleLogout(); });
            dialog.Show();
            dialog.Dispose();
        }

        private void HandleLogout()
        {
            StartActivity(new Intent(this, typeof(SignInActivity)));
            FinishAffinity();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_groups)
            {
                StartActivity(new Intent(this, typeof(GroupActivity)));
            }
            else if (id == Resource.Id.nav_history)
            {
                StartActivity(new Intent(this, typeof(HistoryActivity)));
            }
            else if (id == Resource.Id.nav_menu_logout)
            {
                LogoutWindowShow();
            }

            Log.Info(TAG, "Interraction with navigation window");
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }

        private void InitialiseInputs()
        {
            var AddUserToGroup = FindViewById(Resource.Id.AddToGroup);
            var CheckGroupHistory = FindViewById(Resource.Id.CheckGroupHistory);
            var CheckUserHistory = FindViewById(Resource.Id.CheckPersonHistory);
            var DisplayUsers = FindViewById(Resource.Id.DisplayYourGroupMembersButton);

            LayoutInflater layoutInflater = LayoutInflater.From(this);

            AddUserToGroup.Click += async delegate {
                View view = layoutInflater.Inflate(Resource.Layout.user_input_dialog_box, null);
                var spinner = view.FindViewById<Spinner>(Resource.Id.spinnerGroupName);
                List<Group.Group> groups;
                try
                {
                    groups = await API.GroupEndpoint.GetDoctorGroups(SignInViewModel.currentUser.Id);
                }
                catch (ApiException ex)
                {
                    groups = new List<Group.Group>();
                    Log.Error(TAG, "Error reading doctors groups " + ex.StackTrace);
                }
                var groupsList = groups.Select(group => group.Groupname).ToList();

                if (groupsList.Count <= 0)
                {
                    Toast.MakeText(this, Resource.String.you_dont_have_group, ToastLength.Short).Show();
                }
                else
                {
                    spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
                    var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, groupsList);
                    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinner.Adapter = adapter;

                    var selectedUsers = SignInViewModel.userList.Where(user => user.IsSelected && !user.IsDoctor).ToList();
                    string choice = spinner.GetItemAtPosition(spinnerPosition).ToString();

                    Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alertbuilder.SetView(view);
                    alertbuilder.SetCancelable(false)
                                    .SetPositiveButton("Confirm", delegate
                                    {
                                        foreach (var user in selectedUsers)
                                        {
                                            foreach (var group in GroupActivityModel.groups)
                                            {
                                                if (group.Groupname == choice)
                                                {
                                                    user.GroupId = group.Id;
                                                    API.UserEndpoint.UpdateUser(user);
                                                }
                                            }
                                        }
                                        adapterUserList.userList = SignInViewModel.userList.Where(user => !user.IsSelected).Where(user => !user.IsDoctor).ToList();
                                        selectedUsers.ForEach(user => user.IsSelected = false);
                                        adapterUserList.NotifyDataSetChanged();
                                        alertbuilder.Dispose();
                                    })
                                    .SetNegativeButton("Cancel", delegate
                                    {
                                        alertbuilder.Dispose();
                                    });
                    alertbuilder.Create();
                    alertbuilder.Show();
                }
            };

            CheckGroupHistory.Click += async delegate {
                View view = layoutInflater.Inflate(Resource.Layout.user_input_dialog_box, null);
                var spinner = view.FindViewById<Spinner>(Resource.Id.spinnerGroupName);

                List<Group.Group> groups;
                try
                {
                    groups = await API.GroupEndpoint.GetDoctorGroups(SignInViewModel.currentUser.Id);
                }
                catch (ApiException ex)
                {
                    groups = new List<Group.Group>();
                    Log.Error(TAG, "Error reading doctors groups " + ex.StackTrace);
                }
                var groupsList = groups.Select(group => group.Groupname).ToList();
                if (groupsList.Count <= 0)
                {
                    Toast.MakeText(this, Resource.String.you_dont_have_group, ToastLength.Short).Show();
                }
                else
                {
                    spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
                    var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, groupsList);
                    adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                    spinner.Adapter = adapter;

                    Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
                    alertbuilder.SetView(view);
                    alertbuilder.SetCancelable(false)
                                    .SetPositiveButton("Confirm", delegate
                                    {
                                        string choice = spinner.GetItemAtPosition(spinnerPosition).ToString();
                                        StartActivity(new Intent(this, typeof(HistoryActivity)).PutExtra("EXTRA_NAME", choice).PutExtra("EXTRA_REASON", 1));
                                        alertbuilder.Dispose();
                                    })
                                    .SetNegativeButton("Cancel", delegate
                                    {
                                        alertbuilder.Dispose();
                                    });
                    alertbuilder.Create();
                    alertbuilder.Show();
                }
            };

            CheckUserHistory.Click += delegate {
                var selectedUsers = adapterUserList.userList.Where(user => user.IsSelected).ToList();
                if (selectedUsers.Count == 1)
                {
                    adapterUserList.userList.ForEach(user => user.IsSelected = false);
                    StartActivity(new Intent(this, typeof(HistoryActivity)).PutExtra("EXTRA_REASON", 2).PutExtra("EXTRA_NAME",selectedUsers[0].Username));
                }
                else
                {
                    Toast.MakeText(this, Resource.String.select_only_one, ToastLength.Short).Show();
                }
            };

            DisplayUsers.Click += async delegate {
                View view = layoutInflater.Inflate(Resource.Layout.user_input_dialog_box, null);
                var spinner = view.FindViewById<Spinner>(Resource.Id.spinnerGroupName);

                List<Group.Group> groups;
                try
                {
                    groups = await API.GroupEndpoint.GetDoctorGroups(SignInViewModel.currentUser.Id);
                }
                catch (ApiException ex)
                {
                    groups = new List<Group.Group>();
                    Log.Error(TAG, "Error reading doctors groups " + ex.StackTrace);
                }
                var groupsList = groups.Select(group => group.Groupname).ToList();



                groupsList.Add("USERS_WITHOUT_GROUP");

                spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
                var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, groupsList);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinner.Adapter = adapter;

                Android.Support.V7.App.AlertDialog.Builder alertbuilder = new Android.Support.V7.App.AlertDialog.Builder(this);
                alertbuilder.SetView(view);
                alertbuilder.SetCancelable(false)
                                .SetPositiveButton("Confirm", async delegate
                                {
                                    string choice = spinner.GetItemAtPosition(spinnerPosition).ToString();
                                    List<User> userList = null;
                                    if (choice == USERS_WITHOUT_GROUP) {
                                        SignInViewModel.userList = await API.UserEndpoint.GetALLUsers();
                                        userList = SignInViewModel.userList.Where(user => !user.IsDoctor).Where(user => user.GroupId == null).ToList();
                                    }
                                    else 
                                    {
                                        var selection = groups.Where(group => group.Groupname == choice).Select(group => group.Id).First();
                                        userList = await API.UserEndpoint.GetAllUsersByGroup(selection);
                                    }
                                    adapterUserList.userList = userList;
                                    userList.ForEach(user => user.IsSelected = false);
                                    adapterUserList.NotifyDataSetChanged();
                                    alertbuilder.Dispose();
                                })
                                .SetNegativeButton("Cancel", delegate
                                {
                                    alertbuilder.Dispose();
                                });
                alertbuilder.Create();
                alertbuilder.Show();
            };
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            spinnerPosition = e.Position;
        }
    }
}