using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Util;
using Moodis.Constants.Enums;
using Moodis.Feature.Login;
using Moodis.Feature.Register;
using Moodis.Feature.SignIn;
using Moodis.Network;
using Refit;

namespace Moodis.Feature.Group
{
    class GroupActivityModel
    {
        public static List<Group> groups;

        public GroupActivityModel()
        {
            oncreate();
        }

        private async void oncreate()
        {
            try
            {
                groups = await API.GroupEndpoint.GetAllGroups();
            }
            catch(ApiException ex)
            {
                groups = new List<Group>();
                Log.Error("group",  "No groups found" + ex.StackTrace);
            }
        }

        public async Task<Response> CreateGroupAsync(string groupName)
        {
            if (groups.Count == 0 || !groups.Exists(group => group.Groupname == groupName))
            {
                var newGroup = new Group(groupName, SignInViewModel.currentUser.Id);
                groups.Add(newGroup);
                try
                {
                    await API.GroupEndpoint.CreateGroup(newGroup);
                }catch(ApiException ex)
                {
                    Log.Error("group", "Error creating group" + ex.StackTrace);
                }
                return Response.OK;
            }
            else
            {
                return Response.GroupExists;
            }
        }

        public static async Task<List<string>> GetGroupUserIdsAsync(string groupId)
        {
            List<string> UserIds = new List<string>();
            var users = await API.UserEndpoint.GetAllUsersByGroup(groupId);
            users.ForEach(user => UserIds.Add(user.Id));
            return UserIds;
        }

        public static async Task DeleteGroupAsync(string groupId)
        {
            var group = await API.GroupEndpoint.GetByIdGroup(groupId);
            var users = await API.UserEndpoint.GetAllUsersByGroup(groupId);
            users.ForEach(user => user.GroupId = null);
            users.ForEach(async user => await API.UserEndpoint.UpdateUser(user));
            await API.GroupEndpoint.DeleteGroup(groupId);
        }
    }
}