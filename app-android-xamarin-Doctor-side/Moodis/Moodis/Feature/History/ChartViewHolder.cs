using System.Collections.Generic;
using Android.Support.V7.Widget;
using Android.Views;
using Microcharts;
using Microcharts.Droid;
using Moodis.Extensions;
using Moodis.Ui;
using SkiaSharp;

namespace Moodis.Feature.History
{
    class ChartViewHolder : RecyclerView.ViewHolder
    {
        ChartView ChartView;
        private const string HEX_FORMAT = "#{0:X6}";

        public ChartViewHolder(View view) : base(view)
        {
            ChartView = view.FindViewById<ChartView>(Resource.Id.chartView);
        }

        public void OnBind(List<ImageInfo> stats, Chart chart)
        {
            if (stats.Count == 0) return;

            var entries = new List<ChartEntry>();
            var maxEmotions = new List<KeyValuePair<string, int>>(); //Pair of emotion name and how many times it was highest

            stats.ForEach(stat => {
                var highestEmotion = stat.HighestEmotion;

                var oldPairIndex = maxEmotions.FindIndex(pair => pair.Key == highestEmotion);
                if (oldPairIndex != -1)
                {
                    var oldPair = maxEmotions[oldPairIndex];

                    maxEmotions.RemoveAt(oldPairIndex);
                    maxEmotions.Insert(oldPairIndex, new KeyValuePair<string, int>(oldPair.Key, oldPair.Value + 1));
                }
                else
                {
                    maxEmotions.Add(new KeyValuePair<string, int>(highestEmotion, 1));
                }
            });

            maxEmotions.ForEach(emotionPair => {
                var color = SKColor.Parse(GetEmotionColorHex(emotionPair.Key));
                entries.Add(new ChartEntry(emotionPair.Value)
                {
                    Label = emotionPair.Key,
                    ValueLabel = emotionPair.Value.ToString(),
                    Color = color,
                    TextColor = color
                });
            });

            chart.LabelTextSize = ChartView.Context.Resources.GetDimension(Resource.Dimension.chart_label);
            chart.Entries = entries;
            ChartView.Chart = chart;
        }

        private string GetEmotionColorHex(string emotionName)
        {
            return string.Format(HEX_FORMAT, ChartView.Context.GetColor(emotionName.EmotionColor()));
        }
    }
}