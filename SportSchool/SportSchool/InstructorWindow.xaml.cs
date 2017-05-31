using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace SportSchool
{
    public partial class InstructorWindow : Window
    {
        string Email;

        List<SportSchool.Learner> learners;
        DateTime lastDateTime;
        public InstructorWindow(string login, string password)
        {
            InitializeComponent();
            List<string> list = new List<string>();
            list.Add("8:30");
            list.Add("10:10");
            list.Add("11:50");
            list.Add("14:00");
            list.Add("15:40");
            list.Add("17:20");
            timeCB.ItemsSource = list;
            Email = login;
            HideElements(attendanceField);
            HideElements(competitionSP);
        }
        private void timeCB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetData();
        }

        private void GetData()
        {
            if (!Selected(true))
            {
                return;
            }
            learnersLB.ItemsSource = null;
            string time = timeCB.SelectedItem as string;
            int hour = 8;
            DayOfWeek dayOfWeek = datePicker.SelectedDate.Value.DayOfWeek;
            lastDateTime = datePicker.SelectedDate.Value;
            if (dayOfWeek == DayOfWeek.Sunday)
            {
                MessageBox.Show("В воскресенье занятия не проводятся");
                return;
            }
            int day = (int)dayOfWeek;

            switch (time)
            {
                case "8:30": hour = 8; break;
                case "10:10": hour = 10; break;
                case "11:50": hour = 11; break;
                case "14:00": hour = 14; break;
                case "15:40": hour = 15; break;
                case "17:20": hour = 17; break;
            }
            List<SportSchoolClassLibrary.Learner> list;
            using (var instructorSVC = new InstructorServiceReference.InstructorServiceClient())
            {
                list = instructorSVC.GetLearners(Email, day, hour).ToList();
            }
            if (learners != null)
            {
                learners.Clear();
            }
            else
            {
                learners = new List<Learner>();
            }
            foreach (var elem in list)
            {
                if (elem.RegistrationDate.Date.CompareTo(datePicker.SelectedDate.Value.Date) < 0)
                {
                    learners.Add(new SportSchool.Learner(elem));
                }
            }
            learnersLB.ItemsSource = learners;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!Selected(true))
            {
                return;
            }
            var lrns = new List<SportSchoolClassLibrary.Learner>();
            foreach (var elem in (learnersLB.ItemsSource as List<Learner>).Where(learner => learner.IsChecked))
            {
                lrns.Add(new SportSchoolClassLibrary.Learner() { ID = elem.ID, BirthDate = lastDateTime });
            }
            using (var instructorSVC = new InstructorServiceReference.InstructorServiceClient())
            {
                instructorSVC.SaveAttendance(lrns.ToArray());
            }
            MessageBox.Show("Сохранено");
        }

        private bool Selected(bool show)
        {
            if (!datePicker.SelectedDate.HasValue)
            {
                if (show)
                {
                    MessageBox.Show("Выберите дату");
                }
                return false;
            }
            if (timeCB.SelectedIndex == -1)
            {
                if (show)
                {
                    MessageBox.Show("Выберите время проведения занятия");
                }
                return false;
            }
            return true;
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            GetData();
        }

        private void attendanceMI_Click(object sender, RoutedEventArgs e)
        {
            HideElements(competitionSP);
            ShowElements(attendanceField);
        }
        private void ShowElements(StackPanel panel)
        {
            DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(0.5)); 
            foreach(UIElement elem in panel.Children)
            {
                elem.Opacity = 0;
                elem.Visibility = Visibility.Visible;
                elem.BeginAnimation(OpacityProperty, anim);
            }   
        }
        private void HideElements(StackPanel panel)
        {
            //return;
            DoubleAnimation anim = new DoubleAnimation(0, TimeSpan.FromSeconds(0.5));
            foreach (UIElement elem in panel.Children)
            {
                elem.Opacity = 0;
                elem.BeginAnimation(OpacityProperty, anim);
                elem.Visibility = Visibility.Collapsed;
                //elem.Visibility = Visibility.Hidden;
            }
        }

        private void addCompetitionButton_Click(object sender, RoutedEventArgs e)
        {
            if (!competitionDP.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату проведения соревнования");
                return;
            }
            var learnersIDs = new List<int>();
            foreach (var elem in (competitorsLB.ItemsSource as List<Learner>).Where(learner => learner.IsChecked))
            {
                learnersIDs.Add(elem.ID);
            }
            if (learnersIDs.Count == 0)
            {
                MessageBox.Show("Участники соревнований не выбраны");
                return;
            }
            if(descriptionTB.Text.Length > 400)
            {
                MessageBox.Show("Число символов в описании не должно превышать 400");
                return;
            }
            if (descriptionTB.Text.Length == 0)
            {
                MessageBox.Show("Описание не должно быть пустым");
                return;
            }
            var competitn = new SportSchoolClassLibrary.Competition()
            {
                ID = 0,
                Date = competitionDP.SelectedDate.Value,
                Description = descriptionTB.Text
            };
            using (var instrSVC = new InstructorServiceReference.InstructorServiceClient())
            {
                instrSVC.AddCompetition(competitn, learnersIDs.ToArray());
            }
            MessageBox.Show("Соревнование добавлено");
        }

        private void addCompetitionMI_Click(object sender, RoutedEventArgs e)
        {
            HideElements(attendanceField);
            ShowElements(competitionSP);
            SportSchoolClassLibrary.Instructor instructor = null;
            using (var instructorSVC = new InstructorServiceReference.InstructorServiceClient())
            {
                instructor = instructorSVC.GetInstructor(Email);
            }
            List<SportSchoolClassLibrary.Learner> learners = null;
            using (var adminSVC = new AdminServiceReference.AdminServiceClient())
            {
                learners = adminSVC.GetLeranersBy(instructor.SectionID).ToList();
            }
            if (learners == null)
            {
                MessageBox.Show("В секции нет учащихся");
                return;
            }
            var learnersToBind = new List<Learner>();
            foreach(var learner in learners)
            {
                learnersToBind.Add(new Learner(learner));
            }
            competitorsLB.ItemsSource = learnersToBind;
        }

        private void showAllCompetitions_Click(object sender, RoutedEventArgs e)
        {
            CompetitionsWindow cw = new CompetitionsWindow();
            cw.ShowDialog();
        }

        private void ShowJournal_Click(object sender, RoutedEventArgs e)
        {
            JournalWindow jw = new JournalWindow(Email);
            jw.ShowDialog();
        }
    }
    class Learner:SportSchoolClassLibrary.Learner
    {
        public string FIO {
            get
            {
                return string.Format("{0} {1} {2}", Surname, Name, MiddleName);
            }
            set
            {

            }
        }
        public bool IsChecked { get; set; }
        public Learner(SportSchoolClassLibrary.Learner learner)
        {
            ID = learner.ID;
            SectionID = learner.SectionID;
            Name = learner.Name;
            Surname = learner.Surname;
            MiddleName = learner.MiddleName;
            TelephoneNumber = learner.TelephoneNumber;
            BirthDate = learner.BirthDate;
            Email = learner.Email;
            RegistrationDate = learner.RegistrationDate;
            IsChecked = false;
        }
    }
}
