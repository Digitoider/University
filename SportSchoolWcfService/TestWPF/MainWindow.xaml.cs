using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestWPF.AdminServiceReference;
using MySql.Data.MySqlClient;
using System.Data;

namespace TestWPF
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        AdminServiceClient adminClient;
        public MainWindow()
        {
            InitializeComponent();
            label.Content = "Тестирование AdminService\n";
            adminClient = new AdminServiceClient();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //Array instructors = adminClient.GetInstructorsBy("NoSection");
            List<Instructor> instructors = adminClient.GetInstructorsBy("NoSection").ToList();
            //Instructor[] instructors = adminClient.GetInstructorsBy("NoSection");
            foreach (Instructor i in instructors)
            {
                MessageBox.Show("|" + i.ID + i.Name + i.Surname + i.MiddleName + i.TelephoneNumber + i.Email);
                //label.Content += "|"+i.ID + i.Name + i.Surname + i.MiddleName + i.TelephoneNumber + i.Email + "\n";
            }
            
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < 4; i++)
            {
                adminClient.InsertNewInstructor(new Instructor()
                {
                    ID = 0,
                    SectionID = 1,
                    Name = "Имя" + i,
                    Surname = "Фамилия" + i,
                    MiddleName = "Отчество" + i,
                    TelephoneNumber = "12345",
                    Email = "Mad" + i + "@m.ru",
                    Password = "1111"
                });
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            dataGrid.DataContext = adminClient.GetInstructorsBy("NoSection");
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=SportSchool;UID=root;PASSWORD=1111;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            try
            {
                connection.Open();

                string query = "SELECT ID, IName, Surname, MiddleName, TelephoneNumber, Email " +
                           "FROM Instructors " +
                           "WHERE Section_ID = @SECTIONID";

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;

                command.Parameters.AddWithValue("@SECTIONID", 1);
                //command.Parameters.Add("@SECTIONID", MySqlDbType.Int32).Value = 1;

                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());
                connection.Close();
                dataGrid.DataContext = dt;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Не удается подключиться к БД");
            }
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=SportSchool;UID=root;PASSWORD=1111;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            string query = "SELECT ID, IName, Surname, MiddleName, TelephoneNumber, Email " +
                           "FROM Instructors " +
                           "WHERE Section_ID = @SECTIONID";

            MySqlCommand command = connection.CreateCommand();
            command.CommandText = query;

            command.Parameters.Add("@SECTIONID", MySqlDbType.Int32).Value = 1;
            //DataTable dt = new DataTable();
            //dt.Load(command.ExecuteReader());

            //command.Prepare();
            //command.Parameters.AddWithValue("@sectionID", sectionID);
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

            dataReader.Close();
            connection.Close();
        }
    }
}
