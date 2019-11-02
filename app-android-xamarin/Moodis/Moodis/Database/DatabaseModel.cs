﻿using System.Collections.Generic;
using System.IO;
using Moodis.Feature.Login;

namespace Moodis.Database
{
    class DatabaseModel
    {
        private static SQLite.SQLiteConnection databaseConnection;
        static DatabaseModel()
        {
            string filename = "users_db.sqlite";
            string fileLocation = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            databaseConnection = new SQLite.SQLiteConnection(Path.Combine(fileLocation, filename));
            //Use this for testing compatability with new users
            //databaseConnection.DeleteAll<User>();
            databaseConnection.CreateTable<User>();
            
        }

        public static List<User> FetchData()
        {
            return databaseConnection.Table<User>().ToList();
        }
        public static void AddUserToDatabase(User user)
        {
            databaseConnection.Insert(user);
        }

        public static void CloseConnectionToDatabase()
        {
            databaseConnection.Dispose();
        }
    }
}