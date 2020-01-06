using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Moodis.Feature.Menu;
using Moodis.Feature.SignIn;
using Moodis.Helpers;
using Moodis.Network;
using Moodis.Network.Endpoints;
using Refit;

namespace Moodis
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static int REQUEST_CODE_LOGIN = 1;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            AppCenter.Start("4493172f-d0e0-49d4-bb48-bc5a529ac6ee", typeof(Analytics), typeof(Crashes));
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            InitWebServices();
            StartActivityForResult(new Intent(this, typeof(SignInActivity)), REQUEST_CODE_LOGIN);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok && requestCode == REQUEST_CODE_LOGIN)
            {
                var intent = new Intent(this, typeof(MenuActivity));
                StartActivity(intent);
            }
            else if (resultCode == Result.Canceled && requestCode == REQUEST_CODE_LOGIN)
            {
                Finish();
            }
        }

        private void InitWebServices()
        {
            API.UserEndpoint = RestService.For<IUserEndpoint>(Secrets.WebServiceAPI);
            API.ImageInfoEndpoint = RestService.For<IImageInfoEndpoint>(Secrets.WebServiceAPI);
            API.GroupEndpoint = RestService.For<IGroupEndpoint>(Secrets.WebServiceAPI);
        }
    }
}