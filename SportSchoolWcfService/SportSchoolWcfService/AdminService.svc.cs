using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using SportSchoolClassLibrary;

namespace SportSchoolWcfService
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class AdminService : IAdminService
    {
        MySqlConnectionStringBuilder connectionString = new MySqlConnectionStringBuilder();
        public AdminService()
        {
            connectionString = new MySqlConnectionStringBuilder();
            connectionString.Server = "localhost";//192.168.56.101
            connectionString.Port = 3306;
            connectionString.Database = "SportSchool";
            connectionString.UserID = "root";
            connectionString.Password = "1111";
            connectionString.CharacterSet = "utf8";
        }

        public int GetSectionID(string section)
        {
            int sectionID = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT ID FROM Sections WHERE SName = 'NoSection'";
                using (MySqlCommand cmd = new MySqlCommand())//query, connection
                {
                    cmd.Connection = connection;
                    cmd.CommandText = query;
                    //cmd.Prepare();
                    cmd.Parameters.Add("@section", MySqlDbType.VarString).Value = section;
                    //cmd.Parameters.AddWithValue("@sectionID", section);
                    try
                    {
                        connection.Open();
                        MySqlDataReader dataReader = cmd.ExecuteReader();

                        if (dataReader.Read())
                        {
                            sectionID = dataReader.GetInt32(0);
                        }
                        dataReader.Close();
                    }
                    catch (MySqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return sectionID;
        }

        public List<Instructor> GetInstructorsBy(int sectionID)
        {
            if (sectionID == -1) return null;

            List<Instructor> instructors = new List<Instructor>();
            #region GetInstructorsBySection
            using (MySqlConnection connection = new MySqlConnection(connectionString.ToString()))
            {
                string query = "SELECT ID, Section_ID, IName, Surname, MiddleName, TelephoneNumber, Email " +
                            "FROM Instructors " +
                            "WHERE Section_ID = @sectionID";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    //cmd.Prepare();
                    cmd.Parameters.Add("@sectionID", MySqlDbType.UInt32);//.Value = sectionID;
                    cmd.Parameters["@sectionID"].Value = sectionID;
                    //cmd.Parameters.AddWithValue("@sectionID", sectionID);
                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            instructors.Add(new Instructor()
                            {
                                ID = rd.GetInt32(0),
                                SectionID = rd.GetInt32(1),
                                Name = rd.GetString(2),
                                Surname = rd.GetString(3),
                                MiddleName = rd.GetString(4),
                                TelephoneNumber = rd.GetString(5),
                                Email = rd.GetString(6)
                            });
                        }
                        rd.Close();
                    }
                    catch (MySqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            #endregion
            return instructors;
        }

        public List<Learner> GetLeranersBy(int sectionID)
        {
            if (sectionID == -1) return null;

            List<Learner> learners = new List<Learner>();
            #region GetLearnersBySection
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT ID, Section_ID, LName, Surname, MiddleName, TelephoneNumber, Email, BirthDate " +
                               "FROM Learners " +
                               "WHERE Section_ID = @sectionID";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@sectionID", sectionID);

                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            learners.Add(new Learner()
                            {
                                ID = rd.GetInt32(0),
                                SectionID = rd.GetInt32(1),
                                Name = rd.GetString(2),
                                Surname = rd.GetString(3),
                                MiddleName = rd.GetString(4),
                                TelephoneNumber = rd.GetString(5),
                                Email = rd.GetString(6),
                                BirthDate = rd.GetDateTime(7)
                            });
                        }
                        rd.Close();
                    }
                    catch (MySqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            #endregion

            return learners;
        }

        public void InsertNewInstructor(Instructor instructor)
        {
            string query = "INSERT INTO Instructors (ID, Section_ID, IName, Surname, MiddleName, TelephoneNumber, Email, Password)" +
                           "VALUES (default, @SectionID, @Name, @Surname, @MiddleName,@TelephoneNumber, @Email, @Password);";


            /* string query = String.Format("insert into Instructors (ID, Section_ID, Name, Surname, MiddleName, TelephoneNumber, Email, Password)" +
                            "values({0}, '{1}', '{2}', '{3}', '{4}','{5}', '{6}', '{7}');",
                            "default",
                            instructor.SectionID,
                            instructor.Name,
                            instructor.Surname,
                            instructor.MiddleName,
                            instructor.TelephoneNumber,
                            instructor.Email,
                            instructor.Password);
             */
            using (MySqlConnection connection = new MySqlConnection(connectionString.ToString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SectionID", instructor.SectionID);
                    cmd.Parameters.AddWithValue("@Name", instructor.Name);
                    cmd.Parameters.AddWithValue("@Surname", instructor.Surname);
                    cmd.Parameters.AddWithValue("@MiddleName", instructor.MiddleName);
                    cmd.Parameters.AddWithValue("@TelephoneNumber", instructor.TelephoneNumber);
                    cmd.Parameters.AddWithValue("@Email", instructor.Email);
                    cmd.Parameters.AddWithValue("@Password", instructor.Password);
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public void InsertNewLearner(Learner learner)
        {
            string query = "INSERT INTO Learners(ID, Section_ID, LName, Surname, MiddleName, BirthDate, TelephoneNumber, Email, Password, RegistrationDate) " +
                "VALUES (default, @SectionID, @Name, @Surname, @MiddleName,STR_TO_DATE(@BirthDate, '%Y-%m-%d'),@TelephoneNumber, @Email, @Password, STR_TO_DATE(@registrationDate, '%Y-%m-%d'));";
                //"VALUES (default, @SectionID, '@Name', '@Surname', '@MiddleName',STR_TO_DATE(@BirthDate, '%Y-%m-%d'),'@TelephoneNumber', '@Email', '@Password');";

            using (MySqlConnection connection = new MySqlConnection(connectionString.ToString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SectionID", learner.SectionID);
                    cmd.Parameters.AddWithValue("@Name", learner.Name);
                    cmd.Parameters.AddWithValue("@Surname", learner.Surname);
                    cmd.Parameters.AddWithValue("@MiddleName", learner.MiddleName);

                    //var dt = new MySql.Data.Types.MySqlDateTime(learner.BirthDate.Year,learner.BirthDate.Month,learner.BirthDate.Day, 0, 0, 0, 0);
                    //cmd.Parameters.Add("@BirthDate", MySqlDbType.DateTime);
                    //cmd.Parameters["@BirthDate"].Value = dt.Value.ToString("yyyy-mm-dd");
                    cmd.Parameters.AddWithValue("@BirthDate", learner.BirthDate.ToString("yyyy-MM-dd"));

                    cmd.Parameters.AddWithValue("@TelephoneNumber", learner.TelephoneNumber);
                    cmd.Parameters.AddWithValue("@Email", learner.Email);
                    cmd.Parameters.AddWithValue("@Password", learner.Password);
                    cmd.Parameters.AddWithValue("@registrationDate", learner.RegistrationDate.ToString("yyyy-MM-dd"));
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }




        }

        public List<Section> GetSections()
        {
            List<Section> sections = new List<Section>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT * FROM Sections";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    connection.Open();
                    MySqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        sections.Add(new Section()
                        {
                            ID = rd.GetInt32(0),
                            Name = rd.GetString(1)
                        });
                    }
                    rd.Close();
                    connection.Close();
                }
            }
            return sections;
        }

        public bool EmailExists(string email)
        {
            int learnersCount = -1, instractorsCount = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT Count(*) FROM Learners WHERE Email = @email";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    try
                    {
                        connection.Open();
                        learnersCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (MySqlException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }

                    /*
                    cmd.CommandText = "SELECT Count(*) FROM Instructors WHERE Email = '@email'";
                    connection.Open();
                    instractorsCount = Convert.ToInt32(cmd.ExecuteScalar());
                    connection.Close();
                    */
                }
                query = "SELECT Count(*) FROM Instructors WHERE Email = @email";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    instractorsCount = Convert.ToInt32(cmd.ExecuteScalar());
                    connection.Close();
                }
            }

            if (learnersCount != 0 || instractorsCount != 0) return true;
            return false;
        }

        public void InsertSection(Section section)
        {
            string query = "INSERT INTO Sections(ID, SName) " +
                           "VALUES (default, @SName);";
            
            using (MySqlConnection connection = new MySqlConnection(connectionString.ToString()))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@SName", section.Name);
                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
        }

        public bool SectionExists(string section)
        {
            int count = -1;
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT Count(*) FROM Sections WHERE SName = @section";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@section", section);
                    try
                    {
                        connection.Open();
                        count = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    catch (MySqlException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            if (count != 0) return true;
            return false;
        }

        public List<TimeTableRow> GetLearnesTimeTable(int sectionID, int instructorID)
        {
            List<TimeTableRow> listLTT = GetLearnersTimeTableRows(sectionID);
            List<TimeTableRow> listITT = GetInstructorsTimeTableRows(sectionID, instructorID);
            List<TimeTableRow> result = new List<TimeTableRow>();
            
                
            
            foreach(TimeTableRow learner in listLTT)
            {
                foreach(TimeTableRow instructor in listITT)
                {
                    if(learner.CompareTo(instructor) == 0)
                    {
                        result.Add(learner.Clone() as TimeTableRow);
                        break;
                    }
                }
            }
            return result;
        }

        private List<TimeTableRow> GetLearnersTimeTableRows(int sectionID)
        {
            List<TimeTableRow> listLTT = new List<TimeTableRow>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT LearnersTimeTable.* FROM LearnersTimeTable, Learners WHERE Learners.Section_ID = @sectionID AND LearnersTimeTable.Learner_ID = Learners.ID;";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@sectionID", sectionID);
                    connection.Open();
                    MySqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        listLTT.Add(new TimeTableRow()
                        {
                            Holder_ID = rd.GetInt32(0),
                            Weekday_ID = rd.GetInt32(1),
                            TableTime = rd.GetDateTime(2)
                        });
                    }
                    rd.Close();
                    connection.Close();
                }
            }
            return listLTT;
        }
        private List<TimeTableRow> GetInstructorsTimeTableRows(int sectionID, int instructorID)
        {
            List<TimeTableRow> listITT = new List<TimeTableRow>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                //string query = "SELECT * FROM InstructorsTimeTable WHERE Instructor_ID = @instructorID";
                string query = "SELECT A.* FROM InstructorsTimeTable A, Instructors B WHERE B.Section_ID = @sectionID AND A.Instructor_ID = B.ID AND A.Instructor_ID = @instructorID";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@sectionID", sectionID);
                    cmd.Parameters.AddWithValue("@instructorID", instructorID);
                    connection.Open();
                    MySqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        listITT.Add(new TimeTableRow()
                        {
                            Holder_ID = rd.GetInt32(0),
                            Weekday_ID = rd.GetInt32(1),
                            TableTime = rd.GetDateTime(2)
                        });
                    }
                    rd.Close();
                    connection.Close();
                }
            }
            return listITT;
        }
        
        public List<TimeTableRow> GetOtherLearnesTimeTable(int sectionID, int instructorID)
        {
            List<TimeTableRow> listLTT = GetLearnersTimeTableRows(sectionID);
            List<TimeTableRow> listITT = new List<TimeTableRow>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT A.* FROM InstructorsTimeTable A, Instructors B WHERE B.Section_ID = @sectionID AND A.Instructor_ID = B.ID AND A.Instructor_ID != @instructorID";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@sectionID", sectionID);
                    cmd.Parameters.AddWithValue("@instructorID", instructorID);
                    connection.Open();
                    MySqlDataReader rd = cmd.ExecuteReader();

                    while (rd.Read())
                    {
                        listITT.Add(new TimeTableRow()
                        {
                            Holder_ID = rd.GetInt32(0),
                            Weekday_ID = rd.GetInt32(1),
                            TableTime = rd.GetDateTime(2)
                        });
                    }
                    rd.Close();
                    connection.Close();
                }
            }
            List<TimeTableRow> result = new List<TimeTableRow>();
            foreach (TimeTableRow learner in listLTT)
            {
                foreach (TimeTableRow instructor in listITT)
                {
                    if (learner.CompareTo(instructor) == 0)
                    {
                        result.Add(learner.Clone() as TimeTableRow);
                        break;
                    }
                }
            }
            return result;

        }

        public Learner GetLearner(int learnerID)
        {
            Learner learner = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT ID, Section_ID, LName, Surname, MiddleName, TelephoneNumber, Email, BirthDate, RegistrationDate " +
                               "FROM Learners " +
                               "WHERE ID = @learnerID";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@learnerID", learnerID);

                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            learner = new Learner()
                            {
                                ID = rd.GetInt32(0),
                                SectionID = rd.GetInt32(1),
                                Name = rd.GetString(2),
                                Surname = rd.GetString(3),
                                MiddleName = rd.GetString(4),
                                TelephoneNumber = rd.GetString(5),
                                Email = rd.GetString(6),
                                BirthDate = rd.GetDateTime(7),
                                RegistrationDate = rd.GetDateTime(8),
                            };
                        }
                        rd.Close();
                    }
                    catch (MySqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return learner;
        }

        public void ChangeLearnersTimeTable(List<TimeTableRow> rows, int sectionID)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                foreach (TimeTableRow row in rows)
                {
                    bool exists = false;
                    string existsQuery = "SELECT Count(*) FROM LearnersTimeTable WHERE Weekday_ID=@weekdayID AND Learner_ID=@learnerID AND LTTableTime = STR_TO_DATE(@dateTime, '%Y-%m-%d %k:%i');";
                    using (MySqlCommand cmd = new MySqlCommand(existsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@weekdayID", row.Weekday_ID);
                        cmd.Parameters.AddWithValue("@dateTime", row.TableTime.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@learnerID", row.Holder_ID);
                        try
                        {
                            connection.Open();
                            int count = Convert.ToInt32(cmd.ExecuteScalar());
                            if (count > 0)
                            {
                                exists = true;
                            }
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    if (!exists)
                    {
                        if (row.prevValue != null)
                        {
                            string query = "UPDATE LearnersTimeTable SET Weekday_ID=@newWeekdayID,LTTableTime=STR_TO_DATE(@newDateTime,'%Y-%m-%d %k:%i')  WHERE Learner_ID=@learnerID AND Weekday_ID=@weekdayID AND LTTableTime=STR_TO_DATE(@dateTime,'%Y-%m-%d %k:%i');";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("@newWeekdayID", row.Weekday_ID);
                                cmd.Parameters.AddWithValue("@newDateTime", row.TableTime.ToString("yyyy-MM-dd HH:mm"));
                                cmd.Parameters.AddWithValue("@weekdayID", row.prevValue.Weekday_ID);
                                cmd.Parameters.AddWithValue("@learnerID", row.prevValue.Holder_ID);
                                cmd.Parameters.AddWithValue("@dateTime", row.prevValue.TableTime.ToString("yyyy-MM-dd HH:mm"));
                                try
                                {
                                    connection.Open();
                                    cmd.ExecuteNonQuery();
                                }
                                finally
                                {
                                    connection.Close();
                                }
                            }
                        }
                        else
                        {

                            string query = "INSERT INTO LearnersTimeTable (Learner_ID, Weekday_ID, LTTableTime) VALUES (@learnerID, @weekdayID, STR_TO_DATE(@dateTime,'%Y-%m-%d %k:%i'));";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("@weekdayID", row.Weekday_ID);
                                cmd.Parameters.AddWithValue("@learnerID", row.Holder_ID);
                                cmd.Parameters.AddWithValue("@dateTime", row.TableTime.ToString("yyyy-MM-dd HH:mm"));
                                try
                                {
                                    connection.Open();
                                    cmd.ExecuteNonQuery();
                                }
                                catch (MySqlException ex)
                                {
                                    throw ex;
                                }
                                finally
                                {
                                    connection.Close();
                                }
                            }
                            query = "UPDATE Learners SET Section_ID=@sectionID WHERE ID=@learnerID;";
                            using (MySqlCommand cmd = new MySqlCommand(query, connection))
                            {
                                cmd.Parameters.AddWithValue("@sectionID", sectionID);
                                cmd.Parameters.AddWithValue("@learnerID", row.Holder_ID);
                                try
                                {
                                    connection.Open();
                                    cmd.ExecuteNonQuery();
                                }
                                catch (MySqlException ex)
                                {
                                    throw ex;
                                }
                                finally
                                {
                                    connection.Close();
                                }
                            }
                        }
                    }
                }
            }
        }

        public void FreeLearners(List<int> list)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "UPDATE Learners SET Section_ID=@sectionID WHERE ID=@learnerID;";
                foreach (int row in list)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@sectionID", 1);
                        cmd.Parameters.AddWithValue("@learnerID", row);
                        try
                        {
                            connection.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch(MySqlException ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    query = "DELETE FROM LearnersTimeTable WHERE Learner_ID=@learnerID;";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@learnerID", row);
                        try
                        {
                            connection.Open();
                            cmd.ExecuteNonQuery();
                        }
                        catch (MySqlException ex)
                        {
                            throw ex;
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }

        public void InsertIntoInstructorsTimeTable(List<TimeTableRow> rows)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "INSERT INTO InstructorsTimeTable (Instructor_ID, Weekday_ID, ITTableTime) VALUES (@instructorID, @weekdayID, STR_TO_DATE(@dateTime, '%Y-%m-%d %k:%i'));";
                string existsQuery = "SELECT Count(*) FROM InstructorsTimeTable WHERE Weekday_ID=@weekdayID AND Instructor_ID=@instructorID AND ITTableTime = STR_TO_DATE(@dateTime, '%Y-%m-%d %k:%i');";
                foreach (var row in rows)
                {
                    bool exists = false;
                    using (MySqlCommand cmd = new MySqlCommand(existsQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@weekdayID", row.Weekday_ID);
                        cmd.Parameters.AddWithValue("@dateTime", row.TableTime.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@instructorID", row.Holder_ID);
                        try
                        {
                            connection.Open();
                            int count = Convert.ToInt32(cmd.ExecuteScalar());
                            if(count > 0)
                            {
                                exists = true;
                            }
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                    if (!exists)
                    {
                        using (MySqlCommand cmd = new MySqlCommand(query, connection))
                        {
                            cmd.Parameters.AddWithValue("@weekdayID", row.Weekday_ID);
                            cmd.Parameters.AddWithValue("@dateTime", row.TableTime.ToString("yyyy-MM-dd HH:mm"));
                            cmd.Parameters.AddWithValue("@instructorID", row.Holder_ID);
                            try
                            {
                                connection.Open();
                                cmd.ExecuteNonQuery();
                            }
                            finally
                            {
                                connection.Close();
                            }
                        }
                    }
                }
            }
        }

        public void DeleteFromInstructorsTimeTable(List<TimeTableRow> rows)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "DELETE FROM InstructorsTimeTable WHERE Weekday_ID=@weekdayID AND Instructor_ID=@instructorID AND ITTableTime = @dateTime;";

                foreach (var row in rows)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@weekdayID", row.Weekday_ID);
                        cmd.Parameters.AddWithValue("@dateTime", row.TableTime.ToString("yyyy-MM-dd HH:mm"));
                        cmd.Parameters.AddWithValue("@instructorID", row.Holder_ID);
                        try
                        {
                            connection.Open();
                            cmd.ExecuteNonQuery();
                        }
                        finally
                        {
                            connection.Close();
                        }
                    }
                }
            }
        }
        private string FormQuery(Person person, string Person)
        {
            string query = "";
            switch (Person)
            {
                case "learner":
                    query = "SELECT * FROM Learners WHERE";
                    if (person.Name.Length != 0)
                        query += " LName=@name";
                    break;
                case "instructor":
                    query = "SELECT * FROM Instructors WHERE";
                    if (person.Name.Length != 0)
                        query += " IName=@name";
                    break;
            }
            if (person.Surname.Length != 0)
                query += " AND Surname=@surname";
            if (person.MiddleName.Length != 0)
                query += " AND MiddleName=@middleName";
            if (person.Name.Length == 0 && person.Surname.Length == 0 && person.MiddleName.Length == 0)
                query = query.Replace(" WHERE", "");
            query = query.Replace(" WHERE AND", " WHERE");
            query += ";";
            return query;
        }
        public List<Learner> FindLearners(Person person, string Person)
        {
            string query = FormQuery(person, Person);
            List<Learner> learners = new List<Learner>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", person.Name);
                    cmd.Parameters.AddWithValue("@surname", person.Surname);
                    cmd.Parameters.AddWithValue("@middleName", person.MiddleName);
                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            learners.Add(new Learner()
                            {
                                ID = rd.GetInt32(0),
                                SectionID = rd.GetInt32(1),
                                Name = rd.GetString(2),
                                Surname = rd.GetString(3),
                                MiddleName = rd.GetString(4),
                                BirthDate = rd.GetDateTime(5),
                                TelephoneNumber = rd.GetString(6),
                                Email = rd.GetString(7),
                            });
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return learners;
        }
        public List<Instructor> FindInstructors(Person person, string Person)
        {
            string query = FormQuery(person, Person);
            List<Instructor> instructors = new List<Instructor>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", person.Name);
                    cmd.Parameters.AddWithValue("@surname", person.Surname);
                    cmd.Parameters.AddWithValue("@middleName", person.MiddleName);
                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            instructors.Add(new Instructor()
                            {
                                ID = rd.GetInt32(0),
                                SectionID = rd.GetInt32(1),
                                Name = rd.GetString(2),
                                Surname = rd.GetString(3),
                                MiddleName = rd.GetString(4),
                                TelephoneNumber = rd.GetString(5),
                                Email = rd.GetString(6)
                            });       
                        }
                    }
                    catch (MySqlException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return instructors;
        }

        public void ModifyLearner(Learner learner)
        {
            string query = "UPDATE Learners SET LName=@name, Surname=@surname, MiddleName=@midname, BirthDate=STR_TO_DATE(@birthDate, '%Y-%m-%d'), TelephoneNumber=@telephone, Email=@email WHERE ID=@learnerID;";
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", learner.Name);
                    cmd.Parameters.AddWithValue("@surname", learner.Surname);
                    cmd.Parameters.AddWithValue("@midname", learner.MiddleName);
                    cmd.Parameters.AddWithValue("@birthDate", learner.BirthDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@telephone", learner.TelephoneNumber);
                    cmd.Parameters.AddWithValue("@email", learner.Email);
                    cmd.Parameters.AddWithValue("@learnerID", learner.ID);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch(MySqlException ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
        }

        public void ModifyInstructor(Instructor instructor)
        {
            string query = "UPDATE Instructors SET IName=@name, Surname=@surname, MiddleName=@midname, TelephoneNumber=@telephone, Email=@email WHERE ID=@instructorID;";
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@name", instructor.Name);
                    cmd.Parameters.AddWithValue("@surname", instructor.Surname);
                    cmd.Parameters.AddWithValue("@midname", instructor.MiddleName);
                    cmd.Parameters.AddWithValue("@telephone", instructor.TelephoneNumber);
                    cmd.Parameters.AddWithValue("@email", instructor.Email);
                    cmd.Parameters.AddWithValue("@instructorID", instructor.ID);

                    try
                    {
                        connection.Open();
                        cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException ex)
                    {

                    }
                    finally
                    {
                        connection.Close();
                    }

                }
            }
        }

        public Instructor GetInstructorBy(string email)
        {
            Instructor instructor = null;
            #region GetInstructorByEmail
            using (MySqlConnection connection = new MySqlConnection(connectionString.ToString()))
            {
                string query = "SELECT ID, Section_ID, IName, Surname, MiddleName, TelephoneNumber, Email " +
                            "FROM Instructors " +
                            "WHERE Email = @email";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            instructor = new Instructor()
                            {
                                ID = rd.GetInt32(0),
                                SectionID = rd.GetInt32(1),
                                Name = rd.GetString(2),
                                Surname = rd.GetString(3),
                                MiddleName = rd.GetString(4),
                                TelephoneNumber = rd.GetString(5),
                                Email = rd.GetString(6)
                            };
                        }
                        rd.Close();
                    }
                    catch (MySqlException ex)
                    {
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            #endregion
            return instructor;
        }
    }
}
