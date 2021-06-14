using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Roommates.Models;


namespace Roommates.Repositories

/// This class is responsible for interacting with the Roommate data
/// It inherits from the BaseRepository class which allows it use of the BaseRepository Connection Property
{   

    class RoommateRepository : BaseRepository
{
        ///When new Roommate Repository is instantiated, pass the connection string back to the Base Repository
        public RoommateRepository(string connectionString) : base(connectionString) { }
       
       // /Get a list of all Roommates
        public List<Roommate> GetAll()
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
                    cmd.CommandText = "SELECT Id, FirstName, LastName, RentPortion, MoveInDate, RoomId FROM Roommate";

                    ///Execute the SQL command in DB and get a reader that will give us access to the data
                    SqlDataReader reader = cmd.ExecuteReader();

                    /// List to hold the roommates we retrieve from the DB
                    List<Roommate> roommates = new List<Roommate>();

                    //// Read() returns true if there is more data to read
                    while (reader.Read())
                    {
                        //For each column in the Roommate table
                        // read the ordinal position(index left to right starting at 0) and assign to variable
                        //read the value at that position and assign it to a variable of the same type

                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        int firstNameColumnPosition = reader.GetOrdinal("FirstName");
                        string firstNameValue = reader.GetString(firstNameColumnPosition);

                        int lastNameColumnPosition = reader.GetOrdinal("LastName");
                        string lastNameValue = reader.GetString(lastNameColumnPosition);

                        int rentPortionColPos = reader.GetOrdinal("RentPortion");
                        int rentPortionValue = reader.GetInt32(rentPortionColPos);

                        int moveInDateColPos = reader.GetOrdinal("MoveInDate");
                        DateTime moveInDateValue = reader.GetDateTime(moveInDateColPos);

                        int roomIdColPos = reader.GetOrdinal("RoomId");
                        int roomIdValue = reader.GetInt32(roomIdColPos);


                        ///Now create a new Roommate object using the data from the database
                        Roommate roommate = new Roommate
                        {
                            Id = idValue,
                            FirstName = firstNameValue,
                            LastName = lastNameValue,
                            RentPortion = rentPortionValue,
                            MoveInDate = moveInDateValue,
                            RoomId = roomIdValue,


                        };

                        ///Add roommate object to the list
                        roommates.Add(roommate);
                    }

                    
                    reader.Close();

                    //Return the list of roommates to wherever it was called
                    return roommates;
                }
            }
        }
        public Roommate GetById(int id)
    {
        using (SqlConnection conn = Connection)
        {
            conn.Open();
            using (SqlCommand cmd = conn.CreateCommand())
            {
                    // Set up command text and any parameters before executing
                cmd.CommandText = @"
                        SELECT rm.Id, rm.FirstName, rm.LastName, rm.RentPortion ,rm.MoveInDate,  r.Name as RoomName  
                        FROM Roommate rm 
                        JOIN Room r On r.Id = rm.RoomId 
                        WHERE rm.Id = @id";
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                Roommate roommate = null;


                //Since we are getting one row of a record no while loop is needed
                if (reader.Read())
                {
                    roommate = new Roommate
                    {
                        Id = id,
                        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                        RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                        Name = reader.GetString(reader.GetOrdinal("r.Name")),
                    };

                }
                reader.Close();
                return roommate;
            }
        }
    }

   
}
}
