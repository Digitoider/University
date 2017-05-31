using SportSchoolClassLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace SportSchool
{
    /// <summary>
    /// Логика взаимодействия для FindLearnerWindow.xaml
    /// </summary>
    public partial class FindLearnerWindow : Window
    {
        string Person;
        BackgroundWorker searchBGWorker, changeBGWorker;
        List<MyPerson> persons = new List<MyPerson>();
        LoadingWindow searchLW = new LoadingWindow("Выполняется поиск...");
        LoadingWindow changeLW = new LoadingWindow("Выполняется сохранение измененний...");
        public FindLearnerWindow(string person)
        {
            InitializeComponent();
            Person = person.ToLower();
            
            switch (Person)
            {
                case "learner":
                    peopleLabel.Content = "Учащиеся";
                    break;
                case "instructor":
                    this.Title = "Найти тренера";
                    peopleLabel.Content = "Тренеры";
                    birthdayDP.Visibility = Visibility.Collapsed;
                    birthdayLabel.Visibility = Visibility.Collapsed;
                    break;
            }

            searchBGWorker = new BackgroundWorker();
            searchBGWorker.DoWork += Search;
            searchBGWorker.RunWorkerCompleted += SearchCompleted;
            searchBGWorker.ProgressChanged += SearchProgressChanged;
            searchBGWorker.WorkerReportsProgress = true;
            searchBGWorker.WorkerSupportsCancellation = true;

            changeBGWorker = new BackgroundWorker();
            changeBGWorker.DoWork += Change;
            changeBGWorker.RunWorkerCompleted += ChangeCompleted;
            changeBGWorker.WorkerReportsProgress = true;
            changeBGWorker.WorkerSupportsCancellation = true;
        }

        private void ChangeCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            changeLW.Hide();

        }

        private void Change(object sender, DoWorkEventArgs e)
        {
            MyPerson person = e.Argument as MyPerson;
            System.Threading.Thread.Sleep(1000);
            switch (person.type)
            {
                case "learner":
                    using(var adminSVC = new AdminServiceReference.AdminServiceClient())
                    {
                        adminSVC.ModifyLearner(new SportSchoolClassLibrary.Learner()
                        {
                            ID = person.ID,
                            Name = person.Name,
                            Surname = person.Surname,
                            MiddleName = person.MiddleName,
                            BirthDate  = person.date,
                            TelephoneNumber = person.TelephoneNumber,
                            Email = person.Email
                        });
                    }
                    break;
                case "instructor":
                    using (var adminSVC = new AdminServiceReference.AdminServiceClient())
                    {
                        adminSVC.ModifyInstructor(new SportSchoolClassLibrary.Instructor()
                        {
                            ID = person.ID,
                            Name = person.Name,
                            Surname = person.Surname,
                            MiddleName = person.MiddleName,
                            TelephoneNumber = person.TelephoneNumber,
                            Email = person.Email
                        });
                    }
                    break;
            }
            changeBGWorker.ReportProgress(100);
        }
        private void SearchProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            persons.Clear();
            List<SportSchoolClassLibrary.Learner> learners = null;
            List<Instructor> instructors = null;
            switch (Person)
            {
                case "learner":
                    learners = e.UserState as List<SportSchoolClassLibrary.Learner>;
                    foreach(SportSchoolClassLibrary.Learner elem in learners)
                    {
                        persons.Add(new MyPerson(elem));
                    }
                    break;
                case "instructor":
                    instructors = e.UserState as List<Instructor>;
                    foreach (Instructor elem in instructors)
                    {
                        persons.Add(new MyPerson(elem));
                    }
                    break;
            }
        }

        private void SearchCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            searchLW.Hide();
            pepopleLB.ItemsSource = null;
            pepopleLB.ItemsSource = persons;
        }

        private void Search(object sender, DoWorkEventArgs e)
        {
            Person person = e.Argument as Person;
            List<SportSchoolClassLibrary.Learner> learners = null;
            List<SportSchoolClassLibrary.Instructor> instructors = null;
            System.Threading.Thread.Sleep(1000);
            using (var adminSVC = new AdminServiceReference.AdminServiceClient())
            {
                //persons = adminSVC.FindPerson(person, Person).ToList();
                switch (Person)
                {
                    case "learner":
                        learners = adminSVC.FindLearners(person, Person).ToList();
                        searchBGWorker.ReportProgress(100, learners);
                        break;
                    case "instructor":
                        instructors = adminSVC.FindInstructors(person, Person).ToList();
                        searchBGWorker.ReportProgress(100, instructors);
                        break;
                }
            }
        }

        private void ChangePersonInDB_Click(object sender, RoutedEventArgs e)
        {
            if(pepopleLB.SelectedIndex == -1)
            {
                MessageBox.Show("Выделите персону из списка");
                return;
            }
            if (!CheckAndModify()) return;
            changeLW.Show();
            switch (Person)
            {
                case "learner":
                    changeBGWorker.RunWorkerAsync(new MyPerson()
                    {
                        ID = (pepopleLB.SelectedItem as MyPerson).ID,
                        Name = nameInformationTB.Text,
                        Surname = surnameInformationTB.Text,
                        MiddleName = middleNameInformationTB.Text,
                        date = birthdayDP.SelectedDate.Value,
                        TelephoneNumber = telephoneNumberInformationTB.Text,
                        Email = emailInformationTB.Text,
                        type = "learner"
                    });
                    break;
                case "instructor":
                    changeBGWorker.RunWorkerAsync(new MyPerson()
                    {
                        ID = (pepopleLB.SelectedItem as MyPerson).ID,
                        Name = nameInformationTB.Text,
                        Surname = surnameInformationTB.Text,
                        MiddleName = middleNameInformationTB.Text,
                        TelephoneNumber = telephoneNumberInformationTB.Text,
                        Email = emailInformationTB.Text,
                        type = "instructor"
                    });
                    break;
            }
            int i = persons.IndexOf(pepopleLB.SelectedValue as MyPerson);
            persons[i].Name = nameInformationTB.Text;
            persons[i].Surname = surnameInformationTB.Text;
            persons[i].MiddleName = middleNameInformationTB.Text;
            if(Person == "learner")
            {
                persons[i].date = birthdayDP.SelectedDate.Value;
            }
            persons[i].TelephoneNumber = telephoneNumberInformationTB.Text;
            persons[i].Email = emailInformationTB.Text;
            pepopleLB.ItemsSource = null;
            pepopleLB.ItemsSource = persons;
            AnimationSportSchool.HideElements(informationSP, 0.2);
        }

        private bool CheckAndModify()
        {
            string value = CheckText(nameInformationTB.Text, "'Имя'");
            if (value == "false") return false;

            value = CheckText(surnameInformationTB.Text, "'Фамилия'");
            if (value == "false") return false;

            value = CheckText(middleNameInformationTB.Text, "'Отчество'");
            if (value == "false") return false;

            if (!CheckNumber(telephoneNumberInformationTB.Text, "'Телефонный номер'")) return false;

            if (!CheckEmail(emailInformationTB.Text, "'Email'")) return false;

            bool exists = true;
            using (var AdminSVC = new AdminServiceReference.AdminServiceClient())
            {
                exists = AdminSVC.EmailExists(emailInformationTB.Text);
            }
            if (emailInformationTB.Text != currentEmail && exists )
            {
                MessageBox.Show("Данный Email используется другим пользователем");
                return false;
            }

            if (Person == "learner" && !CheckDate(birthdayDP, "'Дата рождения'")) return false;

            return true;
        }

        private bool CheckDate(DatePicker datePicker, string fieldName)
        {
            if (!datePicker.SelectedDate.HasValue)
            {
                MessageBox.Show(String.Format("{0} должна быть выбрана", fieldName));
                return false;
            }
            if(datePicker.SelectedDate.Value.Year > (DateTime.Now.Year - 3))
            {
                MessageBox.Show("Слишком малый возраст");
                return false;
            }
            return true;
        }
        private bool CheckNumber(string telephoneNumber, string fieldName)
        {
            Regex numberRegex = new Regex("^[0-9]+$");
            if (!numberRegex.IsMatch(telephoneNumber))
            {
                MessageBox.Show(String.Format("Для поля {0} допускается ввод только арабских цифр", fieldName));
                return false;
            }
            return true;
        }

        private bool CheckEmail(string email, string fieldName)
        {
            email.Replace(" ", "");
            if (email.Length == 0)
            {
                MessageBox.Show(String.Format("Поле {0} не должно быть пустым", fieldName));
                return false;
            }
            Regex emailRegex = new Regex(@"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$");
            if (!emailRegex.IsMatch(email))
            {
                MessageBox.Show("Неверный формат ввода электронной почты");
                return false;
            }
            return true;
        }

        private string CheckText(string text, string fieldName)
        {
            text.Replace(" ", "");
            if (text.Length == 0)
            {
                MessageBox.Show(String.Format("Поле {0} не должно быть пустым",fieldName));
                return "false";
            }
            Regex regex = new Regex("^[а-яА-ЯёЁa-zA-Z]+$");
            if (!regex.IsMatch(text))
            {
                MessageBox.Show(String.Format("Поле {0} должно быть введено корректно", fieldName));
                return "false";
            }
            return AddLearnerWindow.ModifyToCorrectForm(text);
        }

        private void LoadInformation_Selected(object sender, RoutedEventArgs e)
        {
            AnimationSportSchool.ShowElements(informationSP, 1);
            MyPerson p = pepopleLB.SelectedItem as MyPerson;
            if (p == null)
            {
                ClearAllFields();
                return;
            }
            nameInformationTB.Text = p.Name;
            surnameInformationTB.Text = p.Surname;
            middleNameInformationTB.Text = p.MiddleName;

            if (p.type == "learner")
            {
                birthdayDP.SelectedDate = p.date;
            }
            telephoneNumberInformationTB.Text = p.TelephoneNumber;
            currentEmail = emailInformationTB.Text = p.Email;
        }
        string currentEmail;
        private void ClearAllFields()
        {
            nameInformationTB.Text = "";
            surnameInformationTB.Text = "";
            middleNameInformationTB.Text = "";
            telephoneNumberInformationTB.Text = "";
            emailInformationTB.Text = "";
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            searchLW.Show();
            searchBGWorker.RunWorkerAsync(new Person()
            {
                Name = AddLearnerWindow.ModifyToCorrectForm(nameTB.Text),
                Surname = AddLearnerWindow.ModifyToCorrectForm(surnameTB.Text),
                MiddleName = AddLearnerWindow.ModifyToCorrectForm(middleNameTB.Text)
            });
        }
    }
    class MyPerson:Person
    {
        public string type { get; set; }
        public DateTime date { get; set; } 
        public string FIO
        {
            get
            {
                return string.Format("{0} {1} {2}", Surname, Name, MiddleName);
            }
            set
            {

            }
        }
        public MyPerson()
        {
        }

        public MyPerson(SportSchoolClassLibrary.Learner learner)
        {
            ID = learner.ID;
            Name = learner.Name;
            Surname = learner.Surname;
            MiddleName = learner.MiddleName;
            date = learner.BirthDate;
            TelephoneNumber = learner.TelephoneNumber;
            Email = learner.Email;
            type = "learner";
        }
        public MyPerson(Instructor instructor)
        {
            ID = instructor.ID;
            Name = instructor.Name;
            Surname = instructor.Surname;
            MiddleName = instructor.MiddleName;
            TelephoneNumber = instructor.TelephoneNumber;
            Email = instructor.Email;
            type = "instructor";
        }
    }

}
