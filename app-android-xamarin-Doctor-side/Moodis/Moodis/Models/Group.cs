using System;
using System.Collections.Generic;
using System.Linq;
using Java.Lang;
using SQLite;

namespace Moodis.Feature.Group
{
    class Group
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string DoctorId { get; set; }

        public string Groupname { get; set; }

        public Group(string name, string doctorId)
        {
            Groupname = name;
            DoctorId = doctorId;
            Id = Guid.NewGuid().ToString();
        }
    }
}