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

namespace Moodis.Ui
{
    class MenuViewModel
    {
        public static ImageInfo currentImage = new ImageInfo();
        private static Bitmap userImage;
        private static string jsonAsString;

        public Bitmap ShowImage(String fileToDisplay)
        {
            userImage?.Dispose();
            userImage = new Bitmap(fileToDisplay);
            return userImage;
        }

        public async Task GetFaceEmotionsAsync()
        {
            Face face = Face.Instance;
            jsonAsString = await face.SendImageForAnalysis(currentImage.ImagePath);
            Console.WriteLine(jsonAsString);
            if (ValidateJson())
            {
                currentImage.SetImageInfo(jsonAsString);
            }
        }

        public bool ValidateJson()
        {
            try
            {
                jsonAsString = jsonAsString.Replace("[", " ").Replace("]", "").Replace(" ", "");
            }
            catch (NullReferenceException e)
            {
                Console.WriteLine(e);
            }
            if (string.IsNullOrEmpty(jsonAsString))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
