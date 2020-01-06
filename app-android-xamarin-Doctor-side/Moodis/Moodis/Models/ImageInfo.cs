using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using Moodis.Feature.SignIn;
using SQLite;
using System;
using System.Linq;

namespace Moodis.Ui
{
    public class ImageInfo
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string HighestEmotion { get; set; }
        public DateTime Date { get; set; }

        [Ignore]
        public string ImagePath { get; set; }

        public void SetImageInfo(DetectedFace face)
        {
            UserId = SignInViewModel.currentUser.Id;
            Date = DateTime.UtcNow;
            FindHighestEmotion(face.FaceAttributes.Emotion);
        }

        private void FindHighestEmotion(Emotion detectedEmotions)
        {
            var properties = detectedEmotions.GetType().GetProperties().ToList();
            double highestEmotionConfidence = 0;
            double confidence;

            properties.ForEach(property =>
            {
                confidence = (double)property.GetValue(detectedEmotions, null);
                if (confidence > highestEmotionConfidence)
                {
                    highestEmotionConfidence = confidence;
                    HighestEmotion = property.Name;
                }
            });
        }
    }
}