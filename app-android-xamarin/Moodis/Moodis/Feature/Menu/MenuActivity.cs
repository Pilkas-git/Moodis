﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Moodis.Feature.CameraFeature;
using Moodis.Feature.Music;
using Moodis.Feature.SignIn;
using Moodis.History;
using Moodis.Ui;

namespace Moodis.Feature.Menu
{
    [Activity(Label = "Menu", Theme = "@style/AppTheme.NoActionBar")]
    public class MenuActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        private readonly string TAG = nameof(MenuActivity);
        private MenuViewModel MenuViewModel;
        private MusicPlayer MusicPlayer;
        private const string FormatDouble = "N3";
        private bool JustSignedIn = false;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            MenuViewModel = MenuViewModel.Instance;
            MusicPlayer = new MusicPlayer(this);
            SetContentView(Resource.Layout.activity_menu);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            if (MenuViewModel.currentImage.ImagePath == null)
            {
                StartActivity(new Intent(this, typeof(CameraActivity)));
                Finish();
            }
            else
            {
                MenuViewModel.image = BitmapFactory.DecodeFile(MenuViewModel.currentImage.ImagePath);

                JustSignedIn = Intent.GetBooleanExtra(SignInActivity.EXTRA_SIGNED_IN, false);

                InitViews();
                MenuViewModel.DeleteImage();
            }
        }

        public void InitViews()
        {
            var imageBox = FindViewById<ImageView>(Resource.Id.imageForView);
            var emotionLabel = FindViewById<TextView>(Resource.Id.highestEmotionLabel);

            imageBox.SetImageBitmap(MenuViewModel.image);

            if (MenuViewModel.currentImage.HighestEmotion != null)
            {
                emotionLabel.Text = GetString(Resource.String.menu_emotion_text, MenuViewModel.currentImage.HighestEmotion);
                MenuViewModel.AddImage();
            }
            else
            {
                emotionLabel.Text = GetString(Resource.String.warning_face_detection);
            }
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
            Android.Support.V7.App.AlertDialog.Builder builder = new Android.Support.V7.App.AlertDialog.Builder(this)
                .SetTitle(Resource.String.logout)
                .SetMessage(Resource.String.logout_confirmation_message)
                .SetNegativeButton(Resource.String.no, (senderAlert, args) => { })
                .SetPositiveButton(Resource.String.yes, (senderAlert, args) =>
                {
                    StartActivity(new Intent(this, typeof(SignInActivity)));
                    FinishAffinity();
                });
            builder.Create().Show();
            builder.Dispose();
        }

        private void MusicPlay()
        {
            if (MenuViewModel.currentImage.HighestEmotion != null && !MusicPlayer.IsPlaying())
            {
                int[] musicLabels = { Resource.Raw.Anger, Resource.Raw.Contempt, Resource.Raw.Disgust, Resource.Raw.Fear, Resource.Raw.Happiness, Resource.Raw.Neutral,
                                Resource.Raw.Sadness, Resource.Raw.Surprise };
                var index = MenuViewModel.GetHighestEmotionIndex();
                if (index != -1)
                {
                    MusicPlayer.Play(musicLabels[index]);
                }
            }
            else if (MusicPlayer.IsPlaying())
            {
                Snackbar.Make(FindViewById(Resource.Id.menuActivity), Resource.String.info_music_is_playing, Snackbar.LengthShort).Show();
            }
            else
            {
                Log.Debug(TAG, Resources.GetString(Resource.String.warning_playing_music));
                Snackbar.Make(FindViewById(Resource.Id.menuActivity), Resource.String.warning_playing_music, Snackbar.LengthShort).Show();
            }
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                if (JustSignedIn)
                {
                    StartActivity(new Intent(this, typeof(CameraActivity)));
                    Finish();
                }
                else
                {
                    OnBackPressed();
                }
            }
            else if (id == Resource.Id.nav_history)
            {
                StartActivity(new Intent(this, typeof(HistoryActivity)));
            }
            else if (id == Resource.Id.nav_music_play)
            {
                MusicPlay();
            }
            else if (id == Resource.Id.nav_music_stop)
            {
                if (MusicPlayer != null)
                    MusicPlayer.Stop();
                else
                    Snackbar.Make(FindViewById(Resource.Id.menuActivity), Resource.String.warning_no_music_playing, Snackbar.LengthShort).Show();
            }
            else if (id == Resource.Id.nav_music_settings)
            {

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
    }
}
