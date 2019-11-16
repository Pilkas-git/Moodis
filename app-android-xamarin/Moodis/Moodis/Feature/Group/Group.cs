﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Moodis.Feature.Login;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace Moodis.Feature.Group
{
    class Group
    {
        private const string SEPARATOR = "__,__";
        [PrimaryKey]
        public string Id { get; set; }

        [Ignore]
        public List<string> Members { get; set; }
        public string MembersInString { get; set; }

        public string Groupname { get; set; }

        public Group()
        {
            Id = Guid.NewGuid().ToString();
            Members = new List<string>();
        }

        public Group(string name, string firstUser) : this()
        {
            this.Groupname = name;
            Members.Add(firstUser);
        }

        public void AddMember(string user)
        {
            Members.Add(user);
        }

        public void RemoveMember(string user)
        {
            Members.Remove(user);
        }

        public bool IsMember(string username)
        {
            return Members.Contains(username) ? true : false;
        }

        public void ConvertToString()
        {
            using StringBuffer stringBuffer = new StringBuffer();
            Members.ForEach(entry => stringBuffer.Append(entry).Append(SEPARATOR));
            stringBuffer.SetLength(stringBuffer.Length() - SEPARATOR.Length);
            MembersInString = stringBuffer.ToString();
        }

        public void ConvertToList()
        {
            Members = MembersInString.Split(SEPARATOR).ToList();
        }
    }
}