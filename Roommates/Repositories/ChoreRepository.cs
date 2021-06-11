﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Roommates.Models;


namespace Roommates.Repositories
    ///
    ///This class is responsible for interacting with the Chore data
    ///It inherits from the BaseRepository class which allows it use of the BaseRepository Connection Property
{   /// <summary>
/// 
/// </summary>
    class ChoreRepository : BaseRepository
    {
        ///
        ///When new Chore Repository is instantiated, pass the connection string back to the Base Repository
        ///
        public ChoreRepository(string connectionString) : base(connectionString) { }
        ///
        ///Get a list of all Roommates
        ///
        public List<Chore> GetAll()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            using (SqlConnection conn = Connection)
            {
                ///The connection to SQL DB must be opened
                conn.Open();

                ///The ability to USE SQL COmmands must be established
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    ///Setup the command we want to execute
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    ///Execute the SQL command in DB and get a reader that will give us access to the data
                    SqlDataReader reader = cmd.ExecuteReader();

                    /// List to hold the chores we retrieve from the DB
                    List<Chore> chores = new List<Chore>();

                    //// Read() returns true if there is more data to read
                    while (reader.Read())
                    {
                        //The "ordinal" is the nummeric position of the column in the query results.
                        //For this Query Id has an ordinal value of 0 and Name is 1
                        int idColumnPosition = reader.GetOrdinal("Id");

                        /// use the reader's Getxxx methods to get the value for a particular ordinal
                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);


                        ///Now create a new Chore object using the data from the database
                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue,
                        };

                        ///Add Chore object to the list
                        chores.Add(chore);
                    }

                    //Need to Close() the reader. It Doesn't automatically do it for us
                    reader.Close();

                    //Return the list of chores to wherever it was called
                    return chores;
                }
            }
        }
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Chore chore = null;

                    //We just want one row so we don't use a while loop.
                    if (reader.Read())
                    {
                        chore = new Chore
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                    }
                    reader.Close();
                    return chore;
                }
            }
        }

        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
}
            }
        }
    }
}