using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Constraints;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Moodis.Constants.Enums;
using Moodis.Extensions;

namespace Moodis.Feature.Group
{
    [Activity(Label = "Groups")]

    public class GroupActivity : AppCompatActivity
    {
        readonly GroupActivityModel groupActivityModel = new GroupActivityModel(); 

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_group);
            InitialiseInputs();

            //AnimationExtension.AnimateBackground(FindViewById(Resource.Id.groupActivity));
        }

        private void InitialiseInputs()
        {
            var GroupNameTextField = FindViewById<EditText>(Resource.Id.text_groupName);
            var CreateGroupButton = FindViewById(Resource.Id.button_create_group);
            var SeeGroupsButton = FindViewById(Resource.Id.button_see_groups);

            GroupNameTextField.TextChanged += (sender, e) =>
            {
                if (string.IsNullOrEmpty((sender as EditText).Text))
                {
                    GroupNameTextField.SetError(GetString(Resource.String.group_entry_empty), null);
                }
            };

            CreateGroupButton.Click += async (sender, e) =>
            {
                if(!string.IsNullOrWhiteSpace(GroupNameTextField.Text))
                {
                    var response = await groupActivityModel.CreateGroupAsync(GroupNameTextField.Text);
                    if(response == Response.OK)
                    {
                        Toast.MakeText(this, Resource.String.join_group_success, ToastLength.Short).Show();
                    }
                    else
                    {
                        Toast.MakeText(this, Resource.String.join_group_fail, ToastLength.Short).Show();
                    }
                }
                else
                {
                    Toast.MakeText(this, Resource.String.text_group_empty_error, ToastLength.Short).Show();
                }
            };

            SeeGroupsButton.Click += (sender, e) =>
            {
                StartActivity(new Intent(this, typeof(GroupListActivity)));
            };
        }
    }
}