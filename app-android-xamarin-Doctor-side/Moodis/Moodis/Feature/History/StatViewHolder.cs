using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Moodis.Extensions;
using Moodis.Feature.SignIn;
using Moodis.Ui;
using System.Linq;

namespace Moodis.History
{
    class StatViewHolder : RecyclerView.ViewHolder
    {
        public TextView TimeLabel { get; set; }
        public TextView EmotionLabel { get; set; }

        private const string TIME_FORMAT = "HH:mm";

        public StatViewHolder(View view) : base(view)
        {
            TimeLabel = view.FindViewById<TextView>(Resource.Id.timeLabel);
            EmotionLabel = view.FindViewById<TextView>(Resource.Id.emotionLabel);
        }

        public void OnBind(ImageInfo stat)
        {
            var context = EmotionLabel.Context;
            var emotionName = stat.HighestEmotion;

            TimeLabel.Text = stat.Date.ToLocalTime().ToString(TIME_FORMAT);
            EmotionLabel.Text = emotionName;
            EmotionLabel.SetTextColor(new Color(ContextCompat.GetColor(context, emotionName.EmotionColor())));
        }
    }
}