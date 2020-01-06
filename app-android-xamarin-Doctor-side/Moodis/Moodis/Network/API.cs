using Moodis.Network.Endpoints;

namespace Moodis.Network
{
    class API
    {
        public static IUserEndpoint UserEndpoint { get; set; }
        public static IImageInfoEndpoint ImageInfoEndpoint { get; set; }
        public static IGroupEndpoint GroupEndpoint { get; set; }
    }
}