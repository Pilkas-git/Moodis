using AndroidX.Lifecycle;
using Microcharts;
using Moodis.Network;
using Moodis.Ui;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moodis.History
{
    class HistoryViewModel : ViewModel
    {
        public async Task<IList<object>> FetchItemList(List<string> userIds, DateTime dateTime)
        {
            var listToReturn = new List<object>();
            IEnumerable<ImageInfo> imageInfos;
            try
            {
                foreach (var userId in userIds)
                {
                    imageInfos = await API.ImageInfoEndpoint.GetImageInfos(userId, dateTime);
                    listToReturn.AddRange(imageInfos);
                }
            }
            catch
            {
                return null;
            }


            if (listToReturn.Count != 0)
            {
                listToReturn.Insert(0, new DonutChart());
            }

            return listToReturn;
        }
    }
}