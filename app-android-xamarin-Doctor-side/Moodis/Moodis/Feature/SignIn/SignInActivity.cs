using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Moodis.Constants.Enums;
using Moodis.Extensions;
using Moodis.Feature.Register;
using Moodis.Network;
using System.Threading.Tasks;

namespace Moodis.Feature.SignIn
{
    [Activity(Label = "Sign In")]
    public class SignInActivity : AppCompatActivity
    {
        public static int REQUEST_CODE_REGISTER = 1;
        private readonly SignInViewModel SignInViewModel = new SignInViewModel();

        View progressBar;

        public static string EXTRA_SIGNED_IN = "EXTRA_SIGNED_IN";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_signin);
            InitButtonsAndInputs();
            //TODO when Android.Arch.Lifecycle lib gets updated use this provider, so various lifecycle and configuration changes won't affect data stored in viewmodel
            //SignInViewModel = ViewModelProviders.Of(this).Get(Java.Lang.Class.FromType(typeof(SignInViewModel))) as SignInViewModel;

            AnimationExtension.AnimateBackground(FindViewById(Resource.Id.constraintLayoutSingIn));
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();
            SetResult(Result.Canceled);
            Finish();
        }

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            //As we implement new Activities there will be more if statements
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.FirstUser && requestCode == REQUEST_CODE_REGISTER)
            {
                Toast.MakeText(this, Resource.String.user_created, ToastLength.Short);
                SetResult(Result.Ok);
                Finish();
            }
            else if (resultCode == Result.Canceled && requestCode == REQUEST_CODE_REGISTER)
            {
                if(SignInViewModel.currentUser != null)
                    await DeleteUser();
                SetResult(Result.Ok);
                Finish();
            }
        }

        private void InitButtonsAndInputs()
        {
            var usernameInput = FindViewById<EditText>(Resource.Id.usernameInput);
            var passwordInput = FindViewById<EditText>(Resource.Id.passwordInput);
            var signInButton = FindViewById(Resource.Id.signInButton);
            var registerButton = FindViewById(Resource.Id.registerButton);
            progressBar = FindViewById(Resource.Id.progressBarSignIn);

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

            passwordInput.KeyPress += (sender, e) =>
            {
                if (e.KeyCode == Android.Views.Keycode.Enter && e.Event.Action == Android.Views.KeyEventActions.Down)
                {
                    e.Handled = true;
                    if (string.IsNullOrWhiteSpace((sender as EditText).Text))
                    {
                        passwordInput.SetError(GetString(Resource.String.password_empty_error), null);
                    }
                    else
                    {
                        this.HideKeyboard(passwordInput);
                        SignIn(usernameInput.Text, passwordInput.Text);
                    }
                }
                else
                {
                    e.Handled = false;
                }
            };

            signInButton.Click += (sender, e) =>
            {
                if (string.IsNullOrEmpty(usernameInput.Text))
                {
                    usernameInput.SetError(GetString(Resource.String.username_empty_error), null);
                }
                else if (string.IsNullOrWhiteSpace(passwordInput.Text))
                {
                    passwordInput.SetError(GetString(Resource.String.password_empty_error), null);
                }
                else
                {
                    SignIn(usernameInput.Text, passwordInput.Text);
                }
            };
            registerButton.Click += (sender, e) =>
            {
                StartActivityForResult(new Intent(this, typeof(RegisterActivity)), REQUEST_CODE_REGISTER);
            };
        }

        private async void SignIn(string username, string password)
        {
            progressBar.Visibility = ViewStates.Visible;
            progressBar.BringToFront();

            var response = await SignInViewModel.Authenticate(username, password);

            if (response == Response.OK)
            {
                SetResult(Result.Ok);
                Finish();
            }
            else if (response == Response.BadCredentials)
            {
                progressBar.Visibility = ViewStates.Gone;
                Toast.MakeText(this, Resource.String.login_fail, ToastLength.Short).Show();
            }
            else if (response == Response.ApiError)
            {
                progressBar.Visibility = ViewStates.Gone;
                Toast.MakeText(this, Resource.String.api_error, ToastLength.Short).Show();
            }
        }
        private async Task DeleteUser()
        {
           await API.UserEndpoint.DeleteUser(SignInViewModel.currentUser.Id);
        }
    }
}