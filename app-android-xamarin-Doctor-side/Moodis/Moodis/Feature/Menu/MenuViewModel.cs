using Moodis.Network.Face;
using System;

namespace Moodis.Ui
{
    public class MenuViewModel
    {

        private static readonly Lazy<MenuViewModel> obj = new Lazy<MenuViewModel>(() => new MenuViewModel());
        private MenuViewModel()
        {
        }

        public static MenuViewModel Instance
        {
            get
            {
                return obj.Value;
            }
        }
        public async void MovePersonGroupAsync(string personGroupId,string personId,string username, string newGroupId)
        {
           await Face.Instance.MovePerson(personGroupId, personId, username, newGroupId);
        }
    }
}
