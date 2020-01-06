using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Moodis.Feature.Group
{  
    class GroupListAdapter : RecyclerView.Adapter
    {
        readonly List<Group> groups;
        readonly Context context;
        TextView noGroupLabel;

        public GroupListAdapter(Context context,List<Group> groups, TextView noGroupLabel)
        {
            this.groups = groups;
            this.context = context;
            this.noGroupLabel = noGroupLabel;
        }

        public override int ItemCount
        {
            get { return groups.Count(); }
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var viewHolder = holder as GroupViewHolder;
            viewHolder.GroupNameLabel.Text = groups[position].Groupname;

            viewHolder.GroupLeaveButton.Click += async (sender, e) =>
            {
                await GroupActivityModel.DeleteGroupAsync(groups[position].Groupname);
                groups.RemoveAt(position);
                NotifyItemRemoved(position);
                if(groups.Count == 0)
                {
                    noGroupLabel.Visibility = ViewStates.Visible;
                }
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).
                Inflate(Resource.Layout.holder_group_entry, parent, false);
            return new GroupViewHolder(itemView);
        }
    }
}