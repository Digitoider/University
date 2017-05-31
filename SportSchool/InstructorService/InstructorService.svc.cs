using MySql.Data.MySqlClient;
using SportSchoolClassLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace InstructorService
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

        public List<Learner> GetLearners(string instructorsEmail, int dayOfWeek, int hour)
        {
            Instructor instr = null;
            #region Getting Instructor from DB
            using (MySqlConnection connection = new MySqlConnection(connectionString.ConnectionString))
            {
                string query = "SELECT ID, Section_ID FROM Instructors WHERE Email=@email";

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@email", instructorsEmail);
                    try
                    {
                        connection.Open();
                        MySqlDataReader rd = cmd.ExecuteReader();
                        if (rd.Read())
                        {
                            instr = new Instructor();
                            instr.ID = rd.GetInt32(0);
                            instr.SectionID = rd.GetInt32(1);
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
                if (row.Weekday_ID == dayOfWeek && hour == row.TableTime.Hour)
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
            /*var adminSVC = new AdminServiceReference.AdminServiceClient();
                foreach (int elem in learnersIDs)
                {
                    learners.Add(adminSVC.GetLearner(elem));
                }
            adminSVC.Close();*/
            return learners;

        }
    }
}
