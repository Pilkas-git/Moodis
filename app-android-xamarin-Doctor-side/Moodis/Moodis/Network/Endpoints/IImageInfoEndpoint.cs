using Moodis.Ui;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moodis.Network.Endpoints
{
    interface IImageInfoEndpoint
    {
        [Post("/imageinfo")]
        public Task AddImageInfo([Body] ImageInfo imageInfo);

        [Get("/imageinfo/user?userId={userId}")]
        public Task<IEnumerable<ImageInfo>> GetImageInfos(string userId, [Body] DateTime dateTime);
    }
}