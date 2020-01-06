using Android.App;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Support.Constraints;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Moodis.Constants.Enums;
using Moodis.Extensions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Moodis.Feature.Register
{
    [Activity(Label = "Register")]
    public class RegisterActivity : Activity
    {
        readonly RegisterViewModel registerViewModel = new RegisterViewModel();

        Button registerButton;
        View progressBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_register);
            InitButtonsAndInputs();

            /*AnimationDrawable animationDrawable;
            var constraintLayout = (ConstraintLayout)FindViewById(Resource.Id.constraintlayoutRegister);
            animationDrawable = (AnimationDrawable)constraintLayout.Background;
            animationDrawable.SetEnterFadeDuration(10);
            animationDrawable.SetExitFadeDuration(5000);
            animationDrawable.Start();*/
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            Finish();
        }

        private void InitButtonsAndInputs()
        {
            var usernameInput = FindViewById<EditText>(Resource.Id.usernameInput);
            var passwordInput = FindViewById<EditText>(Resource.Id.passwordInput);
            progressBar = FindViewById(Resource.Id.progressBarRegister);
            registerButton = FindViewById<Button>(Resource.Id.registerButton);

            usernameInput.TextChanged += (sender, e) =>
            {
                if (string.IsNullOrEmpty((sender as EditText).Text))
                {
                    usernameInput.SetError(GetString(Resource.String.username_empty_error), null);
                }
            };
            passwordInput.TextChanged += (sender, e) =>
            {
                if (string.IsNullOrEmpty((sender as EditText).Text))
                {
                    usernameInput.SetError(GetString(Resource.String.password_empty_error), null);
                }
            };

            usernameInput.EditorAction += (sender, e) =>
            {
                if (e.ActionId == ImeAction.Next)
                {
                    if (string.IsNullOrEmpty((sender as EditText).Text))
                    {
                        usernameInput.SetError(GetString(Resource.String.username_empty_error), null);
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
                else
                {
                    e.Handled = false;
                }
            };

            passwordInput.KeyPress += async (sender, e) =>
            {
                if (e.KeyCode == Keycode.Enter && e.Event.Action == KeyEventActions.Down)
                {
                    e.Handled = true;
                    if (string.IsNullOrEmpty((sender as EditText).Text))
                    {
                        passwordInput.SetError(GetString(Resource.String.password_empty_error), null);
                    }
                    else
                    {
                        this.HideKeyboard(passwordInput);
                        await Register(usernameInput.Text, passwordInput.Text);
                    }
                }
                else
                {
                    e.Handled = false;
                }
            };

            registerButton.Click += async (sender, e) =>
            {
                if (string.IsNullOrEmpty(usernameInput.Text))
                {
                    usernameInput.SetError(GetString(Resource.String.username_empty_error), null);
                }
                else if (string.IsNullOrEmpty(passwordInput.Text))
                {
                    passwordInput.SetError(GetString(Resource.String.username_empty_error), null);
                }
                else
                {
                    await Register(usernameInput.Text, passwordInput.Text);
                }
            };
        }

        private async Task Register(string username, string password)
        {
            progressBar.Visibility = ViewStates.Visible;
            progressBar.BringToFront();
            registerButton.Enabled = false;

            var regex = new Regex(@"^(?=.*\d)(?=.*[A-Z])(.+)$");
            if (regex.IsMatch(password))
            {
                var response = await registerViewModel.AddUser(username, password);

                if (response == Response.OK)
                {
                    SetResult(Result.FirstUser);
                    Finish();
                }
                else if (response == Response.UserExists)
                {
                    Toast.MakeText(this, Resource.String.user_exists, ToastLength.Short).Show();
                }
            }
            else
            {
                Toast.MakeText(this, Resource.String.stronger_password, ToastLength.Short).Show();
            }
            progressBar.Visibility = ViewStates.Gone;
            registerButton.Enabled = true;
        }
    }
}