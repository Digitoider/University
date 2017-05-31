using MySql.Data.MySqlClient;
using SportSchoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InstructorSVC
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class InstructorService : IInstructorService
    {
        MySqlConnectionStringBuilder connectionString;
        public InstructorService()
        {
            connectionString = new MySqlConnectionStringBuilder();
            connectionString.Server = "localhost";//192.168.56.101
            connectionString.Port = 3306;
            connectionString.Database = "SportSchool";
            connectionString.UserID = "root";
            connectionString.Password = "1111";
            connectionString.CharacterSet = "utf8";
        }

        public void AddCompetition(Competition competition, List<int> learnersIDs)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "INSERT INTO Competition(ID, CompetitionDate, Description) VALUES(default, STR_TO_DATE(@date, '%Y-%m-%d'), @description);";
                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@date", competition.Date.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("@description", competition.Description);

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
                Competition cmptn = null;
                foreach(var elem in GetCompetitions())
                {
                    if(elem.Date.CompareTo(competition.Date)==0 && elem.Description == competition.Description)
                    {
                        cmptn = elem;
                    }
                }
                if (cmptn == null) return;
                query = "INSERT INTO Competitors(Competition_ID, Learner_ID) VALUES(@competitionID, @learnerID);";
                foreach (int elem in learnersIDs)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@competitionID", cmptn.ID);
                        cmd.Parameters.AddWithValue("@learnerID", elem);

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

        public List<Attendance> GetAttendance(int learnerID)
        {
            List<Attendance> attendanceList = new List<Attendance>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT * FROM Attendance WHERE Learner_ID=@learnerID;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@learnerID", learnerID);
                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                attendanceList.Add(new Attendance()
                                {
                                    Learner_ID = rd.GetInt32(0),
                                    AttendanceDate = rd.GetDateTime(1),
                                });
                            }
                        }
                        rd.Close();
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
            return attendanceList;
        }

        public List<Competition> GetCompetitions()
        {
            List<Competition> competitionList = new List<Competition>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT * FROM Competition;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        if (rd.HasRows)
                        {
                            while (rd.Read())
                            {
                                competitionList.Add(new Competition()
                                {
                                    ID = rd.GetInt32(0),
                                    Date = rd.GetDateTime(1),
                                    Description = rd.GetString(2)
                                });
                            }
                        }
                        rd.Close();
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
            return competitionList;
        }


        public List<Learner> GetCompetitiors(Competition competition)
        {
            List<Learner> competitors = new List<Learner>();
            List<int> competitorsIDs = new List<int>();
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT Learner_ID FROM Competitors WHERE Competition_ID=@competitionID;";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@competitionID", competition.ID);
                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        while (rd.Read())
                        {
                            competitorsIDs.Add(rd.GetInt32(0));
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
            using(var admSVC = new AdminServiceReference.AdminServiceClient())
            {
                foreach(int ID in competitorsIDs)
                {
                    competitors.Add(admSVC.GetLearner(ID));
                }
            }
            return competitors;
        }

        public Instructor GetInstructor(string email)
        {
            Instructor instructor = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT ID, Section_ID, IName, Surname, MiddleName, TelephoneNumber, Email  FROM Instructors WHERE Email=@email;";

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
                return instructor;
            }
        }

        public List<Learner> GetLearners(string instructorsEmail, int dayOfWeek, int hour)
        {
            Instructor instr = GetInstructor(instructorsEmail);
            if (instr == null)
            {
                return null;
            }
            var adm = new AdminServiceReference.AdminServiceClient();
            List<TimeTableRow> dataFromLearnersTimeTable = adm.GetLearnesTimeTable(instr.SectionID, instr.ID).ToList();
            adm.Close();
            if (dataFromLearnersTimeTable == null)
            {
                return null;
            }

            List<int> learnersIDs = new List<int>();
            foreach (TimeTableRow row in dataFromLearnersTimeTable)
            {
                if ((row.Weekday_ID == dayOfWeek) && (hour == row.TableTime.Hour))
                {
                    learnersIDs.Add(row.Holder_ID);
                }
            }
            List<Learner> learners = new List<Learner>();

            using (var adminSVC = new AdminServiceReference.AdminServiceClient())
            {
                foreach (int elem in learnersIDs)
                {
                    learners.Add(adminSVC.GetLearner(elem));
                }
            }
            return learners;

        }

        public void SaveAttendance(List<Learner> learners)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "INSERT INTO Attendance(Learner_ID, AttendanceDate) VALUES(@learnerID, STR_TO_DATE(@attendanceDate, '%Y-%m-%d'));";
                foreach (var elem in learners)
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        bool exists = false;
                        cmd.CommandText = "SELECT Count(*) FROM Attendance WHERE learner_ID=@learnerID AND AttendanceDate=@attendanceDate;";

                        cmd.Parameters.AddWithValue("@learnerID", elem.ID);
                        cmd.Parameters.AddWithValue("@attendanceDate", elem.BirthDate.ToString("yyyy-MM-dd"));

                        try
                        {
                            connection.Open();
                            int count = Convert.ToInt32(cmd.ExecuteScalar());
                            if (count > 0) exists = true;
                        }
                        catch (MySqlException ex)
                        {

                        }
                        finally
                        {
                            connection.Close();
                        }
                        if (!exists)
                        {
                            cmd.CommandText = query;
                            try
                            {
                                connection.Open();
                                cmd.ExecuteNonQuery();
                            }
                            catch (MySqlException ex)
                            {
                                /*{"You have an error in your SQL syntax; 
                                check the manual that corresponds to your MySQL server version for the right syntax to use near '' at line 1"}*/
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
}

