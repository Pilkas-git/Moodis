﻿using Moodis.Network.Face;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using moodis;
using System.Threading.Tasks;
using System.Windows.Forms;
using Moodis.Extensions;

namespace Moodis.Ui
{
    public class MenuViewModel
    {
        public ImageInfo currentImage;
        private Bitmap userImage;

        public MenuViewModel()
        {
            currentImage = new ImageInfo();
        }

        public Bitmap ShowImage(string fileToDisplay)
        {
            userImage?.Dispose();
            userImage = new Bitmap(fileToDisplay);
            return userImage;
        }

        public async Task GetFaceEmotionsAsync()
        {
            Face face = Face.Instance;
            var json = await face.SendImageForAnalysis(currentImage.ImagePath);
            var jsonAsString = json.FromJsonToString();

            if (!string.IsNullOrEmpty(jsonAsString))
            {
                currentImage.SetImageInfo(jsonAsString);
            }
        }
    }
}
