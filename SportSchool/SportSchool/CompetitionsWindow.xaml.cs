using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SportSchool
{
    /// <summary>
    /// Логика взаимодействия для CompetitionsWindow.xaml
    /// </summary>
    public partial class CompetitionsWindow : Window
    {
        List<CompetitionSS> list;
        public CompetitionsWindow()
        {
            InitializeComponent();
            List<SportSchoolClassLibrary.Competition> competitions = null;
            using (var instrSVC = new InstructorServiceReference.InstructorServiceClient())
            {
                competitions = instrSVC.GetCompetitions().ToList();
            }
            list = new List<CompetitionSS>();
            foreach(var elem in competitions)
            {
                list.Add(new CompetitionSS(elem));
            }
            competitionsLB.ItemsSource = list;
        }

        private void ShowCompetitors_MouseEnter(object sender, MouseEventArgs e)
        {
            //CompetitionSS compet = sender as CompetitionSS;
            CompetitionSS compet = (sender as ListBoxItem).DataContext as CompetitionSS;
            if (compet == null)
            {
                MessageBox.Show("Не получается получить данные о соревновании");
                return;
            }
            description.Content = "Описание:\n" + compet.Description;
            List<SportSchoolClassLibrary.Learner> learners = null;
            using (var instrSVC = new InstructorServiceReference.InstructorServiceClient())
            {
                learners = instrSVC.GetCompetitiors(new SportSchoolClassLibrary.Competition()
                {
                    ID = compet.ID,
                    Date = compet.Date,
                    Description = compet.Description
                }).ToList();
            }
            if(learners == null)
            {
                MessageBox.Show("Не удается получить участников соревнований");
                return;
            }
            var lrns = new List<SportSchool.Learner>();
            foreach(var elem in learners)
            {
                lrns.Add(new Learner(elem));
            }
            competitorsLB.ItemsSource = lrns;
        }

    }
    public class CompetitionSS : SportSchoolClassLibrary.Competition
    {
        public string Data
        {
            get
            {
                return "Дата проведения: " + Date.ToString("yyyy-MM-dd"); 
            }
            set
            {
                Data = "Дата проведения: " + Date.ToString("yyyy-MM-dd");
            }
        }
        public CompetitionSS(SportSchoolClassLibrary.Competition competition)
        {
            ID = competition.ID;
            Date = competition.Date;
            Description = competition.Description;
            
        }
    }
}
