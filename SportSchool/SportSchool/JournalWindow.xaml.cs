using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SportSchool
{
    public partial class JournalWindow : Window
    {
        string Email;
        List<SportSchool.Learner> learners;
        SportSchoolClassLibrary.Instructor instructor = null;
        Label lastTimeSelected = null;
        int selectedX = 0, selectedY = 0;
        public JournalWindow(string email)
        {
            InitializeComponent();
            using (var admSVC = new AdminServiceReference.AdminServiceClient())
            {
                instructor = admSVC.GetInstructorBy(email);
            }
            TimeTable timetable = new TimeTable(instructor.SectionID, instructor.ID);
            bool[] ttable = new bool[6 * 6];
            for (int i = 0; i < 6 * 6; i++)
                ttable[i] = false;

            int x = 0;
            foreach (var elem in timetable.LearnersOfSelectedInstructor)
            {
                int i = TimeTableCell.GetXbyHour(elem.TableTime.Hour);
                int j = elem.Weekday_ID;
                ttable[(i - 1) * 6 + (j - 1)] = true;
            }

            int height = 30;
            int width = 50;
            for (int i = 0; i < 7; i++)
            {
                fields.RowDefinitions.Add(new RowDefinition());
                fields.ColumnDefinitions.Add(new ColumnDefinition() { });
                for (int k = 0; k < 7; k++)
                {
                    if (i == 0)
                    {
                        Label lb = new Label()
                        {
                            Height = height,
                            Width = width,
                            Background = Brushes.LightGray,
                            Content = GetWeekDayForLabel(k),
                            BorderThickness = new Thickness(1),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                        };
                        Grid.SetRow(lb, 0);
                        Grid.SetColumn(lb, k);
                        fields.Children.Add(lb);
                    }
                    if (k == 0)
                    {
                        if (i > 0)
                        {
                            Label lb = new Label()
                            {
                                Height = height,
                                Width = width,//30,
                                BorderThickness = new Thickness(1),
                                Padding = new Thickness(1),
                                Background = Brushes.LightGray,
                                Content = GetTimeForLabel(i),
                                VerticalAlignment = VerticalAlignment.Center,
                            };
                            Grid.SetRow(lb, i);
                            Grid.SetColumn(lb, 0);
                            fields.Children.Add(lb);
                        }
                    }
                    else if (i != 0)
                    {
                        Brush brush = Brushes.Gray;
                        if (ttable[(i - 1) * 6 + (k - 1)])
                        {
                            brush = Brushes.Orange;
                        }
                        Label lb = new Label()
                        {
                            Height = height,
                            Width = width,
                            Background = brush,
                            BorderThickness = new Thickness(1, 1, 1, 1),
                            Name = String.Format("s{0}s{1}", i, k),
                        };
                        if (ttable[(i - 1) * 6 + (k - 1)])
                        {
                            lb.MouseDown += (s, e) =>
                            {
                                if (lastTimeSelected != null)
                                {
                                    lastTimeSelected.Background = Brushes.Orange;
                                }
                                lastTimeSelected = s as Label;
                                lastTimeSelected.Background = Brushes.Green;
                                Point pos = GetPositionOfLabel(lastTimeSelected);
                                AnimationSportSchool.AnimateOutLabelMargin(lastTimeSelected);
                                ShowAttendance((int)pos.X, (int)pos.Y);

                                //var thisWindowHeightAnimation = new DoubleAnimation()
                                //{
                                //    To = this.mainStackPanel.ActualHeight,
                                //    Duration = TimeSpan.FromSeconds(0.2)
                                //};
                                //this.BeginAnimation(HeightProperty, thisWindowHeightAnimation);
                            };
                        }
                        lb.MouseEnter += (s, e) =>
                        {
                            Label label = (s as Label);
                            Point pos = GetPositionOfLabel(lastTimeSelected);
                            Point pos2 = GetPositionOfLabel(label);
                            if ((int)pos.X == (int)pos2.X && (int)pos.Y == (int)pos2.Y/*lastTimeSelected == null || lastTimeSelected == label*/)
                            {
                                return;
                            }
                           
                            AnimationSportSchool.AnimateInLabelMargin(label);
                        };
                        lb.MouseLeave += (s, e) =>
                        {
                            Label label = (s as Label);
                            if ( lastTimeSelected == label)
                            {
                                return;
                            }
                            AnimationSportSchool.AnimateOutLabelMargin(label);
                        };
                        Grid.SetRow(lb, i);
                        Grid.SetColumn(lb, k);
                        fields.Children.Add(lb);
                    }
                }

            }
            Email = email;
        }

        private Point GetPositionOfLabel(Label label)
        {
            if (label == null) {
                return new Point(1, 1);
            }
            string[] mas = label.Name.Split('s');
            selectedX = Convert.ToInt32(mas[1]);
            selectedY = Convert.ToInt32(mas[2]);
            return new Point(selectedX, selectedY);
        }

        private void ShowAttendance(int hourID, int weekday)
        {
            if (hourID == 0 || weekday == 0) return;
            int hour = TimeTableCell.GetDateTimeByID(hourID).Hour;
            List<SportSchoolClassLibrary.Learner> list;
            using (var instructorSVC = new InstructorServiceReference.InstructorServiceClient())
            {
                list = instructorSVC.GetLearners(Email, weekday, hour).ToList();
            }
            if (learners != null)
            {
                learners.Clear();
            }
            else
            {
                learners = new List<Learner>();
            }
            SportSchoolClassLibrary.Learner min = null;
            if (list.Count != 0)
            {
                min = new Learner(list[0]);
            }
            foreach (var elem in list)
            {
                learners.Add(new SportSchool.Learner(elem));
                if (elem.RegistrationDate.Date.CompareTo(min.RegistrationDate.Date) < 0)
                {
                    min = elem;
                }
            }
            min.RegistrationDate = min.RegistrationDate.AddDays(weekday - (int)min.RegistrationDate.DayOfWeek);
            DateTime now = DateTime.Now.Date;
            if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday)
            {
                now = now.AddDays(weekday - (int)now.DayOfWeek);
            }
            else
            {
                now = now.AddDays(-1);
            }
            int cnt = 0;
            attendanceGrid.Children.Clear();
            attendanceGrid.ColumnDefinitions.Clear();
            attendanceGrid.RowDefinitions.Clear();
            attendanceTableHat.Children.Clear();

            int x = 1, y = 0;
            attendanceGrid.RowDefinitions.Add(new RowDefinition() { Height= new GridLength(0) ,});
            Label fio = new Label()
            {
                Content = "ФИО",
                Width = 175,
                Margin = new Thickness(0, 0, 45, 0),
                BorderThickness = new Thickness(1),
                Background = Brushes.Snow,
                BorderBrush = Brushes.Snow,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(fio, 0);
            Grid.SetColumn(fio, 0);
            attendanceTableHat.Children.Add(fio);
            attendanceGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width =new GridLength( 175) });
            
            foreach (var elem in learners)
            {
                attendanceGrid.RowDefinitions.Add(new RowDefinition() {  Height = new GridLength(20) });


                Label lbl = new Label()
                {
                    Content = elem.FIO,
                    Width = 175,
                    Height = 20,
                    Background = (x%2==0)?Brushes.Snow:Brushes.LightYellow,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    BorderThickness = new Thickness(1),
                    Padding = new Thickness(0),
                };
                y = 0;
                Grid.SetRow(lbl, x);
                Grid.SetColumn(lbl, y);
                attendanceGrid.Children.Add(lbl);

                List<SportSchoolClassLibrary.Attendance> attendanceList = null;
                using (var instructorSVC = new InstructorServiceReference.InstructorServiceClient())
                {
                    attendanceList = instructorSVC.GetAttendance(elem.ID).ToList();
                }


                y = 1;
                for (DateTime i = min.RegistrationDate.Date; i.CompareTo(now.Date) < 0; i = i.AddDays(7))
                {
                    if (x == 1)
                    {
                        attendanceGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                        Label dateLabel = new Label()
                        {
                            Content = i.ToString("yyyy-MM-dd"),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Bottom,
                            Margin = new Thickness(-45,0,0,0),
                            RenderTransform = new RotateTransform(-60),
                            Padding = new Thickness(0),
                            BorderThickness = new Thickness(0, 0, 0, 1),
                            BorderBrush = Brushes.Gray,
                        };
                        Grid.SetRow(dateLabel, 0);
                        Grid.SetColumn(dateLabel, y);
                        attendanceTableHat.Children.Add(dateLabel);
                    }
                    CheckBox attendanceCheckBox = new CheckBox()
                    {
                        IsEnabled = false,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = new Thickness(0),
                        Background = Brushes.Gray,
                    };
                    if (attendanceList.Find(s => s.AttendanceDate.Date.CompareTo(i.Date) == 0) == null)
                    {
                        if (elem.RegistrationDate.Date.CompareTo(i.Date) > 0)
                        {
                            attendanceCheckBox.Visibility = Visibility.Hidden;
                        }
                        attendanceCheckBox.IsChecked = false;
                    }
                    else
                    {
                        attendanceCheckBox.IsChecked = true;
                    }
                    Grid.SetRow(attendanceCheckBox, x);
                    Grid.SetColumn(attendanceCheckBox, y);
                    attendanceGrid.Children.Add(attendanceCheckBox);
                    y++;
                }
                cnt++;
                x++;
            }
            attendanceScrollViewer.DataContext = attendanceGrid;
        }

        public static string GetTimeForLabel(int i)
        {
            switch (i)
            {
                case 1: return "8:30";
                case 2: return "10:10";
                case 3: return "11:50";
                case 4: return "14:00";
                case 5: return "15:40";
                default: return "17:20";
            }
        }
        public static string GetWeekDayForLabel(int i)
        {
            switch (i)
            {
                case 0: return "";
                case 1: return "ПН";
                case 2: return "ВТ";
                case 3: return "СР";
                case 4: return "ЧТ";
                case 5: return "ПТ";
                default: return "СБ";
            }
        }

    }
    class Hour
    {
        public int hour { get; set; }
        public int minute { get; set; }
        public string Time
        {
            get
            {
                return string.Format("{0}:{1}", hour, (minute == 0) ? "00" : minute.ToString());
            }
        }
    }
    
}
