using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SportSchoolClassLibrary;
using MySql.Data.MySqlClient;

namespace SportSchoolWCF
{
    public class AdminService : IAdminService, IDisposable
    {
        private MySqlConnection connection;
        public bool isConnected { get; private set; }

        public AdminService()
        {
            isConnected = false;

            MySqlConnectionStringBuilder connectionString = new MySqlConnectionStringBuilder();
            connectionString.Server = "192.168.56.101";
            connectionString.Database = "SportSchool";
            connectionString.UserID = "root";
            connectionString.Password = "1111";
            connectionString.CharacterSet = "utf8";

            connection = new MySqlConnection(connectionString.ToString());
            try
            {
                connection.Open();
                isConnected = true;
            }
            catch (MySqlException ex)
            {
                isConnected = false;
            }
        }

        public int GetSectionID(string section)
        {
            string getSectionQuery = @"select ID from Sections where Name = '" + section + @"';";


            MySqlCommand command = new MySqlCommand(getSectionQuery, connection);
            MySqlDataReader dataReader = command.ExecuteReader();

            int sectionID = -1;
            while (dataReader.Read())
            {
                sectionID = dataReader.GetInt32(0);
            }
            return sectionID;
        }

        public List<Instructor> GetInstructorsBy(string section)
        {
            if (!isConnected) return null;

            int sectionID = GetSectionID(section);
            if (sectionID == -1) return null;

            #region GetInstructorsBySection
            string query = @"select 'ID','Name', 'Surname', 'MiddleName', 'TelephoneNumber', 'Email'" +
                           @"from Instructors" +
                           @"where Section_ID = " + sectionID + ";";

            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = command.ExecuteReader();

            List<Instructor> instructors = new List<Instructor>();
            while (dataReader.Read())
            {
                instructors.Add(new Instructor()
                {
                    ID = dataReader.GetInt32(0),
                    Name = dataReader.GetString(1),
                    Surname = dataReader.GetString(2),
                    MiddleName = dataReader.GetString(3),
                    TelephoneNumber = dataReader.GetString(4),
                    Email = dataReader.GetString(5)
                });
            }
            #endregion
            return instructors;
        }

        public List<Learner> GetLeranersBy(string section)
        {
            if (!isConnected) return null;

            int sectionID = GetSectionID(section);
            if (sectionID == -1) return null;

            #region GetLearnersBySection
            string query = @"select 'ID','Name', 'Surname', 'MiddleName', 'TelephoneNumber', 'Email', 'BirthDate'" +
                           @"from Learners" +
                           @"where Section_ID = " + sectionID + ";";

            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader dataReader = command.ExecuteReader();

            List<Learner> learners = new List<Learner>();
            while (dataReader.Read())
            {
                learners.Add(new Learner()
                {
                    ID = dataReader.GetInt32(0),
                    Name = dataReader.GetString(1),
                    Surname = dataReader.GetString(2),
                    MiddleName = dataReader.GetString(3),
                    TelephoneNumber = dataReader.GetString(4),
                    Email = dataReader.GetString(5),
                    BirthDate = dataReader.GetDateTime(6)
                });
            }
            #endregion

            return learners;
        }

        public void InsertNewInstructor(Instructor instructor)
        {
            /*
            string query = @"insert into SportSchool.Instructors" +
                           @"values(default" +
                           @" ,'" + instructor.SectionID +
                           @"','" + instructor.Name +
                           @"','" + instructor.Surname +
                           @"','" + instructor.MiddleName +
                           @"','" + instructor.TelephoneNumber +
                           @"','" + instructor.Email +
                           @"','" + instructor.Password + "');";
            */
            string query = @"insert into Instructors" +
                           @"values(default, ?SectionID, ?Name, ?Surname, ?MiddleName,?TelephoneNumber, ?Email, ?Password);";
            MySqlCommand command = new MySqlCommand(query, connection);

            command.CommandText = query;
            command.Parameters.Add("?SectionID", MySqlDbType.Int32).Value = instructor.SectionID;
            command.Parameters.Add("?Name", MySqlDbType.VarChar).Value = instructor.Name;
            command.Parameters.Add("?Surname", MySqlDbType.VarChar).Value = instructor.Surname;
            command.Parameters.Add("?MiddleName", MySqlDbType.VarChar).Value = instructor.MiddleName;
            command.Parameters.Add("?TelephoneNumber", MySqlDbType.VarChar).Value = instructor.TelephoneNumber;
            command.Parameters.Add("?Email", MySqlDbType.VarChar).Value = instructor.Email;
            command.Parameters.Add("?Password", MySqlDbType.VarChar).Value = instructor.Password;

            command.ExecuteNonQuery();
        }

        public void InsertNewLearner(Learner learner)
        {
            string query = @"insert into Learners" +
                           @"values(default, ?SectionID, ?Name, ?Surname, ?MiddleName, ?BirthDate, ?TelephoneNumber, ?Email, ?Password);";
            MySqlCommand command = new MySqlCommand(query, connection);

            command.CommandText = query;
            command.Parameters.Add("?SectionID", MySqlDbType.Int32).Value = learner.SectionID;
            command.Parameters.Add("?Name", MySqlDbType.VarChar).Value = learner.Name;
            command.Parameters.Add("?Surname", MySqlDbType.VarChar).Value = learner.Surname;
            command.Parameters.Add("?MiddleName", MySqlDbType.VarChar).Value = learner.MiddleName;
            command.Parameters.Add("?BirthDate", MySqlDbType.Date).Value = learner.BirthDate;
            command.Parameters.Add("?TelephoneNumber", MySqlDbType.VarChar).Value = learner.TelephoneNumber;
            command.Parameters.Add("?Email", MySqlDbType.VarChar).Value = learner.Email;
            command.Parameters.Add("?Password", MySqlDbType.VarChar).Value = learner.Password;

            command.ExecuteNonQuery();
        }

        public void Dispose()
        {
            connection.Close();
        }
    }
}

