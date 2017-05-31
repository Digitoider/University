using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SportSchoolClassLibrary;
using System.Text.RegularExpressions;
using System.Windows.Media.Animation;

namespace SportSchool
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private List<TimeTableCell> listBoxes = new List<TimeTableCell>();
        private List<TimeTableCell> prevListBoxes;
        ListBox selectedListBox;
        ListBox oldLearnersLB;

        /// <summary>
        /// Содержит список секций для их выбора в меню
        /// </summary>
        Sections sections;

        /// <summary>
        /// Содержит список тренеров для их выбора в меню (зависит от секции)
        /// </summary>
        Instructors instructors;

        Learners learners;
        Learners prevLearners;
        TimeTable timeTable;
        TimeTable prevTimeTable;

        Section selectedSection;
        Instructor selectedInstructor;
        bool saved = true;
        private Label lastTimeSelected;
        private int selectedX;
        private int selectedY;

        public AdminWindow()
        {
            InitializeComponent();
            InitializeListBoxes();
            foreach(var elem in listBoxes)
            {
                elem.Item.MouseEnter += (s, e) => {
                    if (selectedListBox == elem.Item) return;
                    elem.Item.BorderBrush = Brushes.Blue;
                    elem.Item.BorderThickness = new Thickness(3, 3, 3, 3);
                    /*DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(2));
                    elem.BeginAnimation(BorderBrushProperty, anim);
                    elem.BeginAnimation(UIElement.OpacityProperty, anim);
                    anim.From = 1;
                    anim.To = 0;
                    elem.BeginAnimation(UIElement.OpacityProperty, anim);*/
                };
                elem.Item.MouseLeave += (s, e) => {
                    if (selectedListBox == elem.Item) return;
                    elem.Item.BorderBrush = Brushes.LightSlateGray;
                    elem.Item.BorderThickness = new Thickness(1, 1, 1, 1);
                };
                elem.Item.MouseRightButtonDown += (s, e) =>
                {
                    foreach(TimeTableCell cell in listBoxes)
                    {
                        if(cell.Item == elem.Item && cell.Locked)
                        {
                            return;
                        }
                    }
                    if (selectedListBox == elem.Item)
                    {
                        selectedListBox = null;
                        elem.Item.BorderBrush = Brushes.LightSlateGray;
                        elem.Item.Background = Brushes.White;
                        elem.Item.BorderThickness = new Thickness(1, 1, 1, 1);
                        return;
                    }
                    elem.Item.BorderBrush = Brushes.Green;
                    elem.Item.Background = Brushes.LightGreen;
                    elem.Item.BorderThickness = new Thickness(3, 3, 3, 3);
                    if(selectedListBox != null)
                    {
                        selectedListBox.BorderBrush = Brushes.LightSlateGray;
                        selectedListBox.Background = Brushes.White;
                        selectedListBox.BorderThickness = new Thickness(1, 1, 1, 1);
                    }
                    selectedListBox = elem.Item;
                };
            }
            sections = new Sections();
            GetSectionsInit();
        }

        private void GetInstructorsInit()
        {
            if(instructors != null)
            {
                instructors.Clear();
                instructors = null;
                instructorsMenuItem.Items.Clear();
            }
            instructors = new Instructors(selectedSection.ID);
            foreach(var iName in instructors.GetInstructorsNamePlusID())
            {
                Regex regex = new Regex(@"\d");
                string name = regex.Replace(iName, "");
                instructorsMenuItem.Items.Add(new MenuItem() {  Header = name, Name = iName.Replace(" ", "") , IsCheckable=true});//Name = iName
            }
            foreach (var elem in instructorsMenuItem.Items)
            {
                MenuItem item = elem as MenuItem;
                if (item == null)
                {
                    MessageBox.Show("Не преобразуется 'elem as MenuItem' in GetInstructorsInit()");
                    return;
                }
                item.Click += (s, e) =>
                {
                    MenuItem mi = s as MenuItem;
                    if (selectedInstructor!=null && selectedInstructor.Surname + " " + selectedInstructor.Name + " " + selectedInstructor.MiddleName == item.Header.ToString())
                    {
                        return;
                    }
                    CheckIfSaved("DoNotErase");
                    CurrentSelectedItem.Items.Clear();
                    foreach (TimeTableCell cell in listBoxes)
                    {

                        cell.Item.Items.Clear();
                        cell.Locked = false;
                        cell.Changed = false;
                        cell.Item.Background = Brushes.White;
                    }
                    selectedListBox = null;
                    selectedInstructor = instructors.GetInstructor(mi.Header.ToString());
                    foreach(MenuItem i in instructorsMenuItem.Items)
                    {
                        i.IsChecked = false;
                    }
                    mi.IsChecked = true;
                    if(selectedInstructor != null)
                    {
                        LoadDataFromDB();
                        FormTimeTable( selectedInstructor.Email);
                    }
                };
            }
        }
       
        private void FormTimeTable( string еmail)
        {
            fields.Children.Clear();
            fields.ColumnDefinitions.Clear();
            fields.RowDefinitions.Clear();
            //fields = new Grid()
            //{
            //    Margin = new Thickness(10, 10, 0, 0),
            //    HorizontalAlignment = HorizontalAlignment.Left,
            //    VerticalAlignment = VerticalAlignment.Top,
            //    Name = "fields",
            //    Width = 330,
            //    Height = 140,
            //};
            Instructor instructor = selectedInstructor;
            //using (var admSVC = new AdminServiceReference.AdminServiceClient())
            //{
            //    instructor = admSVC.GetInstructorBy(email);
            //}
            if (instructor == null) return;
            TimeTable timetable = new TimeTable(instructor.SectionID, instructor.ID);
            bool[] ttable = new bool[6 * 6];
            for (int i = 0; i < 6 * 6; i++)
                ttable[i] = false;

            foreach (var elem in listBoxes)
            {
                int i = elem.X;
                int j = elem.Y;
                if (!elem.Locked)
                {
                    ttable[(i - 1) * 6 + (j - 1)] = true;
                }
            }

            int height = 50;
            int width = 70;
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
                            Content = JournalWindow.GetWeekDayForLabel(k),
                            BorderThickness = new Thickness(1),
                            Padding = new Thickness(1),
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
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
                                Content = JournalWindow.GetTimeForLabel(i),
                                VerticalAlignment = VerticalAlignment.Center,
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center,
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
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Content = listBoxes[(i - 1) * 6 + (k - 1)].Item.Items.Count,
                        };
                        if (ttable[(i - 1) * 6 + (k - 1)])
                        {
                            lb.MouseDown += (s, e) =>
                            {
                                if (lastTimeSelected != null && lastTimeSelected == (s as Label))
                                    return;
                                if(CurrentSelectedItem.Items.Count != 0 && lastTimeSelected != null)
                                {
                                    /*сохранить их в listBoxes*/
                                    SaveToListBox();
                                    //Point lastpos = GetPositionOfLabel(lastTimeSelected);
                                    //foreach(var elem in listBoxes)
                                    //{
                                    //    if(elem.X == (int)lastpos.X && elem.Y == (int)lastpos.Y)
                                    //    {
                                    //        elem.Item.Items.Clear();
                                    //        foreach(ListBoxItem v in CurrentSelectedItem.Items)
                                    //        {
                                    //            elem.Item.Items.Add(new ListBoxItem()
                                    //            {
                                    //                Name = v.Name,
                                    //                Content = v.Content,
                                    //            });
                                    //        }
                                    //        break;
                                    //    }
                                    //}
                                }
                                Point pos = GetPositionOfLabel(s as Label);
                                if (pos.X == -1) return;
                                TimeTableCell ttc = null;
                                foreach (var elem in listBoxes)
                                {
                                    if (elem.X == (int)pos.X && elem.Y == (int)pos.Y)
                                    {
                                        ttc = elem;
                                        break;
                                    }
                                }
                                if (ttc.Locked) return;
                                CurrentSelectedItem.Items.Clear();
                                foreach (ListBoxItem elem in ttc.Item.Items)
                                {
                                    CurrentSelectedItem.Items.Add(new ListBoxItem()
                                    {
                                        Content = elem.Content,
                                        Name = elem.Name,
                                    });
                                }
                                GroupNameLabel.Content = String.Format("{0} {1}", JournalWindow.GetWeekDayForLabel((int)pos.Y), JournalWindow.GetTimeForLabel((int)pos.X));
                                #region Animation and visualization
                                if (lastTimeSelected != null)
                                {
                                    lastTimeSelected.Background = Brushes.Orange;
                                }
                                lastTimeSelected = s as Label;
                                lastTimeSelected.Background = Brushes.Green;
                                AnimationSportSchool.AnimateOutLabelMargin(lastTimeSelected);
                                #endregion
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

                            AnimationSportSchool.AnimateInLabelMargin(label, 7);
                        };
                        lb.MouseLeave += (s, e) =>
                        {
                            Label label = (s as Label);
                            if (lastTimeSelected == label)
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
        }

        private void SaveToListBox()
        {
            Point lastpos = GetPositionOfLabel(lastTimeSelected);
            foreach (var elem in listBoxes)
            {
                if (elem.X == (int)lastpos.X && elem.Y == (int)lastpos.Y)
                {
                    elem.Item.Items.Clear();
                    foreach (ListBoxItem v in CurrentSelectedItem.Items)
                    {
                        elem.Item.Items.Add(new ListBoxItem()
                        {
                            Name = v.Name,
                            Content = v.Content,
                        });
                    }
                    break;
                }
            }
        }

        private Point GetPositionOfLabel(Label label)
        {
            if (label == null)
            {
                return new Point(-1, -1);
            }
            string[] mas = label.Name.Split('s');
            selectedX = Convert.ToInt32(mas[1]);
            selectedY = Convert.ToInt32(mas[2]);
            return new Point(selectedX, selectedY);
        }

        private void LoadDataFromDB()
        {
            learnersLoadPB.Opacity = 1;
            learners = new Learners(sections.GetSectionID("Нет секции"));
            learnersLoadPB.Opacity = 0;

            oldLearnersLB = new ListBox();
            prevLearners = new Learners();
            if(LearnersLB != null && LearnersLB.Items != null)
            {
                LearnersLB.Items.Clear();
            }
            foreach (SportSchoolClassLibrary.Learner elem in learners.Items)
            {
                LearnersLB.Items.Add(new ListBoxItem() { Content = elem.Surname + " " + elem.Name + " " + elem.MiddleName, Name = "ListBoxItem" + elem.ID });
                prevLearners.Items.Add(elem.Clone() as SportSchoolClassLibrary.Learner);
                oldLearnersLB.Items.Add(new ListBoxItem() { Content = elem.Surname + " " + elem.Name + " " + elem.MiddleName, Name = "ListBoxItem" + elem.ID });
            }
            try
            {
                timeTable = new TimeTable(selectedSection.ID, selectedInstructor.ID);
                prevTimeTable = new TimeTable(timeTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            
            int x = 0, y = 0;

            foreach (TimeTableRow row in timeTable.LearnersOfOtherInstructors)
            {
                InsertLearnerIntoTimeTable(row, out x, out y);
                listBoxes[x * 6 + y].Locked = true;
                listBoxes[x * 6 + y].Item.Background = Brushes.Red;
            }

            foreach(TimeTableRow row in timeTable.LearnersOfSelectedInstructor)
            {
                InsertLearnerIntoTimeTable(row, out x, out y);
                listBoxes[x * 6 + y].Locked = false;
                listBoxes[x * 6 + y].Item.Background = Brushes.White;
            }
            LoadPrevListBoxes();
            //show(listBoxes, "loaded listBoxes:\n");
            //show(prevListBoxes, "loaded prevListBoxes:\n");
        }

        private void LoadPrevListBoxes()
        {
            if(prevListBoxes == null)
            {
                prevListBoxes = new List<TimeTableCell>();
            }
            foreach (TimeTableCell l in listBoxes)
            {
                ListBox lb = new ListBox();
                foreach (ListBoxItem i in l.Item.Items)
                {
                    lb.Items.Add(new ListBoxItem()
                    {
                        Name = i.Name
                    });
                }
                prevListBoxes.Add(new TimeTableCell()
                {
                    Changed = l.Changed,
                    Locked = l.Locked,
                    Item = lb,
                    X = l.X,
                    Y = l.Y
                });
            }

        }

        private void InsertLearnerIntoTimeTable(TimeTableRow row, out int x, out int y)
        {
            x = TimeTableCell.GetXbyHour(row.TableTime.Hour) - 1;
            y = row.Weekday_ID - 1;
            var adm = new AdminServiceReference.AdminServiceClient();
            SportSchoolClassLibrary.Learner lr = adm.GetLearner(row.Holder_ID);
            adm.Close();
            string FIO = lr.Surname + " " + lr.Name + " " + lr.MiddleName;
            listBoxes[x * 6 + y].Item.Items.Add(new ListBoxItem() { Content = FIO, Name = "ListBoxItem" + lr.ID });
            prevLearners.Items.Add(lr);
        }

        private void GetSectionsInit()
        {
            foreach (var sectionName in sections.GetSectionNames())
            {
                if (sectionName != "Нет секции")
                    sectionsMenuItem.Items.Add(new MenuItem() { Header = sectionName, IsCheckable = true });
            }
            foreach (var elem in sectionsMenuItem.Items)
            {
                MenuItem item = elem as MenuItem;
                if (item == null)
                {
                    MessageBox.Show("Не преобразуется 'elem as MenuItem' GetSectionsInit()");
                    return;
                }
                item.Click += (s, e) =>
                {
                    CheckIfSaved();
                    CurrentSelectedItem.Items.Clear();
                    selectedSection = sections.GetSection(item.Header.ToString());
                    if(selectedSection != null)
                    {
                        GetInstructorsInit();
                    }
                    foreach (MenuItem i in sectionsMenuItem.Items)
                    {
                        i.IsChecked = false;
                    }
                    (s as MenuItem).IsChecked = true;
                };
            }
        }
        public void CheckIfSaved(string parameter="erase")
        {
            if (!saved)
            {
                MessageBoxResult res = MessageBox.Show("Данные не сохранены, Сохранить?", "Внимание", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    SaveData();
                }
            }
            if(parameter == "erase")
            {
                EraseAllData();
                saved = true;
            }
        }

        private void EraseAllData()
        {
            if (prevListBoxes != null)
                prevListBoxes.Clear();

            selectedListBox = null;

            if (oldLearnersLB != null && oldLearnersLB.Items != null)
                oldLearnersLB.Items.Clear();
            if (instructors != null)
                instructors.Clear();
            if (learners != null && learners.Items != null)
                learners.Items.Clear();
            if (prevLearners != null && prevLearners.Items != null)
                prevLearners.Items.Clear();
            if (timeTable != null)
            {
                timeTable.LearnersOfOtherInstructors.Clear();
                timeTable.LearnersOfSelectedInstructor.Clear();
            }
            if (prevTimeTable != null)
            {
                prevTimeTable.LearnersOfOtherInstructors.Clear();
                prevTimeTable.LearnersOfSelectedInstructor.Clear();
            }
            selectedSection = null;
            selectedInstructor = null;


            foreach (TimeTableCell cell in listBoxes)
            {
                cell.Item.Items.Clear();
                cell.Locked = false;
                cell.Changed = false;
                cell.Item.Background = Brushes.White;
            }
            //show(listBoxes, "listBoxes after erasing:\n");
        }


        private void show(List<TimeTableCell> somelist, string str1)
        {
            foreach (TimeTableCell elem in somelist)
            {
                if (elem.Item.Items.Count != 0)
                {
                    str1 += elem.X + "," + elem.Y + ":\n";
                    foreach (ListBoxItem lb in elem.Item.Items)
                    {
                        str1 += "  " + lb.Name + "\n";
                    }
                }
            }
            MessageBox.Show(str1);
        }

        private void SaveData()
        {
            if (saved) return;
            SaveToListBox();
            
            /*DoubleAnimation anim = new DoubleAnimation()
            {
                From = 1,
                To=0.2,
                Duration = TimeSpan.FromSeconds(2),
                FillBehavior = FillBehavior.HoldEnd
            };*/
            TimeTableGrid.Opacity = 0.2;
            LearnersLB.Opacity = 0.2;
            /*
            TimeTableGrid.BeginAnimation(OpacityProperty, anim);
            LearnersLB.BeginAnimation(OpacityProperty, anim);
            */
            savingLbl.Content = "Сохранение";
            //savingLbl.Background = Brushes.White;
            savingLbl.Opacity = 1;
            UploadPB.Opacity = 1;

            List<int> differentFree = new List<int>();
            List<int> differentBusy = new List<int>();

            #region Getting differs from all cells in Time Table v2
            List<TimeTableCell> difference = new List<TimeTableCell>();
            foreach (TimeTableCell pcell in prevListBoxes)
            {

                foreach (TimeTableCell ncell in listBoxes)
                {
                    if(pcell.X == ncell.X && pcell.Y == ncell.Y)
                    {
                        difference.Add(ncell.Minus(pcell.Intersect(ncell)));
                    }
                }
            }
            #endregion

            //show(difference,"Difference:\n");
            //show(prevListBoxes,"prevListBoxes:\n");
            //show(listBoxes,"listBoxes:\n");

            #region Getting differs from learners who is not in any section 
            foreach (ListBoxItem lb in LearnersLB.Items)
            {
                bool exists = false;
                foreach(SportSchoolClassLibrary.Learner oldL in prevLearners.Items)
                {
                    if("ListBoxItem"+ oldL.ID == lb.Name)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    string v = lb.Name.Replace("ListBoxItem", "");
                    differentFree.Add(Convert.ToInt32(v));
                }
            }
            List<int> learnersIDsToFreeFromSection = new List<int>();
            //List<TimeTableRow> learnersToFreeFromSection = new List<TimeTableRow>();
            foreach (ListBoxItem lb in LearnersLB.Items)
            {
                bool exists = false;
                foreach(int i in differentFree)
                {
                    if(lb.Name == "ListBoxItem" + i)
                    {
                        exists = true;
                        break;
                    }
                }
                if (!exists)
                {
                    string v = lb.Name.Replace("ListBoxItem", "");
                    learnersIDsToFreeFromSection.Add(Convert.ToInt32(v));
                }
            }

            #endregion


            #region Geting data to change InstructorsTimeTable in DB
            //foreach(TimeTableCell elem in listBoxes)
            List<TimeTableRow> valuesToInsertIntoInstructorsTimeTable = new List<TimeTableRow>();
            List<TimeTableRow> valuesToDeleteFromInstructorsTimeTable = new List<TimeTableRow>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    int amount = listBoxes[6 * i + j].Item.Items.Count;
                    if (listBoxes[6 * i + j].Changed && amount != 0)
                    {
                        valuesToInsertIntoInstructorsTimeTable.Add(new TimeTableRow()
                        {
                            Holder_ID = selectedInstructor.ID,
                            Weekday_ID = j + 1,
                            TableTime = TimeTableCell.GetDateTimeByID(i + 1)
                        });
                    }
                    if(amount == 0)
                    {
                        valuesToDeleteFromInstructorsTimeTable.Add(new TimeTableRow()
                        {
                            Holder_ID = selectedInstructor.ID,
                            Weekday_ID = j + 1,
                            TableTime = TimeTableCell.GetDateTimeByID(i + 1)
                        });
                    }
                }
            }
            #endregion


            #region Getting Time table rows to change learners in LearnersTimeTable v1
            List<TimeTableRow> valuesToChangeInDB = new List<TimeTableRow>();
            
            foreach(TimeTableCell cell in difference)
            {
                if(cell.X == 0 && cell.Y == 0)
                {
                    continue;
                }
                foreach (ListBoxItem item in cell.Item.Items)
                {
                    bool exists = false;
                    foreach (TimeTableCell pcell in prevListBoxes)
                    {
                        if(pcell.Item.Items.Count == 0)
                        {
                            break;
                        }
                        foreach (ListBoxItem i in pcell.Item.Items)
                        {
                            if (item.Name == i.Name)
                            {
                                string s = i.Name.Replace("ListBoxItem", "");
                                int id = Convert.ToInt32(s);
                                valuesToChangeInDB.Add(new TimeTableRow()
                                {
                                    Holder_ID = selectedInstructor.ID,
                                    Weekday_ID = cell.Y,
                                    TableTime = TimeTableCell.GetDateTimeByID(cell.X),
                                    prevValue = new TimeTableRow()
                                    {
                                        Holder_ID = id,
                                        Weekday_ID = pcell.Y,
                                        TableTime = TimeTableCell.GetDateTimeByID(pcell.X),
                                        prevValue = null
                                    }
                                });
                                exists = true;
                                break;
                            }
                        }
                        if (exists) break;
                    }
                    if (!exists)
                    {
                        string s = item.Name.Replace("ListBoxItem", "");
                        int id = Convert.ToInt32(s);
                        valuesToChangeInDB.Add(new TimeTableRow()
                        {
                            Holder_ID = id,
                            Weekday_ID = cell.Y,
                            TableTime = TimeTableCell.GetDateTimeByID(cell.X),
                            prevValue = null
                        });
                    }
                }
                
            }
            #endregion

            /*
            string str = "holderID|WeekdayID|time\n";
            foreach (TimeTableRow elem in valuesToChangeInDB)
            {
                str += elem.Holder_ID + "      |" + elem.Weekday_ID + "        |" + elem.TableTime+"\n";
                if (elem.prevValue != null)
                {
                    str += "  "+elem.prevValue.Holder_ID + "      |" + elem.prevValue.Weekday_ID + "        |" + elem.prevValue.TableTime;
                }
            }
            MessageBox.Show(str);
            */

            #region Change data in DB
            var adm = new AdminServiceReference.AdminServiceClient();
            adm.FreeLearners(differentFree.ToArray());//learnersIDsToFreeFromSection
            adm.ChangeLearnersTimeTable(valuesToChangeInDB.ToArray(), selectedSection.ID);
            adm.InsertIntoInstructorsTimeTable(valuesToInsertIntoInstructorsTimeTable.ToArray());
            adm.DeleteFromInstructorsTimeTable(valuesToDeleteFromInstructorsTimeTable.ToArray());
            adm.Close();
            #endregion

            /*TODO всем prevValue установить новые значения*/
            prevListBoxes.Clear();
            LoadPrevListBoxes();
            prevLearners.Items.Clear();
            var adm1 = new AdminServiceReference.AdminServiceClient();
            foreach(ListBoxItem lb in LearnersLB.Items)
            {
                string v = lb.Name.Replace("ListBoxItem", "");
                prevLearners.Items.Add(adm1.GetLearner(Convert.ToInt32(v)));
            }
            adm1.Close();
            saved = true;
            #region Animating progress bar
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(2),
                FillBehavior = FillBehavior.HoldEnd
            };
            savingLbl.Content = "Сохранено ✔";
            savingLbl.Foreground = Brushes.Green;
            savingLbl.BeginAnimation(OpacityProperty, animation);

            UploadPB.Opacity = 0;
            animation.To = 1;
            animation.From = 0.2;
            animation.BeginTime = TimeSpan.FromSeconds(1);
            //animation.FillBehavior = FillBehavior.HoldEnd;
            TimeTableGrid.BeginAnimation(OpacityProperty, animation);
            LearnersLB.BeginAnimation(OpacityProperty, animation);
            #endregion
            //MessageBox.Show("Сохранено");
        }
        private void InitializeListBoxes()
        {
            listBoxes.Clear();
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB11, X = 1, Y = 1 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB12, X = 1, Y = 2 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB13, X = 1, Y = 3 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB14, X = 1, Y = 4 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB15, X = 1, Y = 5 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB16, X = 1, Y = 6 });

            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB21, X = 2, Y = 1 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB22, X = 2, Y = 2 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB23, X = 2, Y = 3 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB24, X = 2, Y = 4 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB25, X = 2, Y = 5 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB26, X = 2, Y = 6 });

            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB31, X = 3, Y = 1 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB32, X = 3, Y = 2 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB33, X = 3, Y = 3 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB34, X = 3, Y = 4 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB35, X = 3, Y = 5 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB36, X = 3, Y = 6 });

            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB41, X = 4, Y = 1 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB42, X = 4, Y = 2 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB43, X = 4, Y = 3 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB44, X = 4, Y = 4 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB45, X = 4, Y = 5 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB46, X = 4, Y = 6 });

            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB51, X = 5, Y = 1 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB52, X = 5, Y = 2 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB53, X = 5, Y = 3 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB54, X = 5, Y = 4 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB55, X = 5, Y = 5 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB56, X = 5, Y = 6 });

            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB61, X = 6, Y = 1 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB62, X = 6, Y = 2 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB63, X = 6, Y = 3 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB64, X = 6, Y = 4 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB65, X = 6, Y = 5 });
            listBoxes.Add(new TimeTableCell() { Locked = false, Item = TimeTableLB66, X = 6, Y = 6 });

            foreach(TimeTableCell cell in listBoxes)
            {
                cell.Changed = false;
            }
        }

        private void AddLearner_Click(object sender, RoutedEventArgs e)
        {
            AddLearnerWindow LW = new AddLearnerWindow();
            LW.ShowDialog();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AddInstructorWindow IW = new AddInstructorWindow();
            IW.ShowDialog();
        }

        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
            AddSectionWindow SW = new AddSectionWindow();
            SW.ShowDialog();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            return;
            string[] maleNames = { "Аарон", "Абрам", "Аваз", "Тимур", "Аввакум", "Василий", "Август", "Авдей", "Игорь", "Авраам" };
            string[] maleSurnames = { "Иванов", "Васильев", "Петров", "Смирнов", "Михайлов", "Фёдоров", "Соколов", "Яковлев", "Попов", "Андреев", "Алексеев"};
            string[] maleMiddleNames = { "Ильич", "Тарасович", "Львович", "Валентинович", "Викторович", "Игнатьевич", "Филиппович", "Ярославович" };
            Random rnd = new Random(9341234);
            AdminServiceReference.AdminServiceClient adm = new AdminServiceReference.AdminServiceClient();
            for (int i = 0; i < 150; i++)
            {
                adm.InsertNewLearner(new SportSchoolClassLibrary.Learner()
                {
                    ID = 0,
                    SectionID = 1,
                    Name = maleNames[rnd.Next() % maleNames.Length],
                    Surname = maleSurnames[rnd.Next() % maleSurnames.Length],
                    MiddleName = maleMiddleNames[rnd.Next() % maleMiddleNames.Length],
                    BirthDate = new DateTime(1990 + rnd.Next() % 22, rnd.Next() % 12 + 1, rnd.Next() % 28 + 1),
                    TelephoneNumber = rnd.Next().ToString(),
                    Email = "user" + i + "@mail.ru",
                    Password = "1111",
                    RegistrationDate = new DateTime(2016, 11, 1),
                });
            }
            for (int i = 150; i < 160; i++)
            {
                adm.InsertNewInstructor(new Instructor()
                {
                    ID = 0,
                    SectionID = rnd.Next() % 3 + 1,
                    Name = maleNames[rnd.Next() % maleNames.Length],
                    Surname = maleSurnames[rnd.Next() % maleSurnames.Length],
                    MiddleName = maleMiddleNames[rnd.Next() % maleMiddleNames.Length],
                    TelephoneNumber = rnd.Next().ToString(),
                    Email = "user" + i + "@mail.ru",
                    Password = "1111"
                });
            }
            adm.Close();
            MessageBox.Show("Added");
        }

        private void GetToTimeTable_Click(object sender, RoutedEventArgs e)
        {
            if(lastTimeSelected == null)
            {
                MessageBox.Show("Выделите поле в таблице расписаний");
                return;
            }
            if (LearnersLB.SelectedIndex == -1)
            {
                MessageBox.Show("Выделите обучающегося в списке 'Учащиеся'");
                return;
            }
            Point lastpos = GetPositionOfLabel(lastTimeSelected);
            ListBox listBox = null;
            foreach (var elem in listBoxes)
            {
                if (elem.X == (int)lastpos.X && elem.Y == (int)lastpos.Y)
                {
                    listBox = elem.Item;
                    break;
                }
            }
            if (CurrentSelectedItem.Items.Count == 5)
            {
                MessageBox.Show("Выделенное поле содержит максимальное количество учащихся, выберите другое поле");
                return;
            }
            saved = false;
            ListBoxItem old = LearnersLB.Items.GetItemAt(LearnersLB.SelectedIndex) as ListBoxItem;
            ListBoxItem item = new ListBoxItem()
            {
                Name = old.Name,
                Content = old.Content
            };
            //listBox.Items.Add(item);
            CurrentSelectedItem.Items.Add(item);
            LearnersLB.Items.RemoveAt(LearnersLB.SelectedIndex);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (listBoxes[6*i + j].Item == listBox)
                    {
                        listBoxes[6 * i + j].Changed = true;
                    }
                }
            }
            lastTimeSelected.Content = CurrentSelectedItem.Items.Count.ToString();
            //int selectedIndex = LearnersLB.SelectedIndex;
            //if (selectedIndex == -1)
            //{
            //    MessageBox.Show("Выделите обучающегося в таблице");
            //    return;
            //}
            //if(selectedListBox == null)
            //{
            //    MessageBox.Show("Выделите поле в таблице расписаний (Клик правой кнопкой мыши по полю)");
            //    return;
            //}
            //if(selectedListBox.Items.Count == 30)
            //{
            //    MessageBox.Show("Выделенное поле содержит максимальное количество учащихся, выберите другое поле");
            //    return;
            //}
            //saved = false;
            //ListBoxItem old = LearnersLB.Items.GetItemAt(selectedIndex) as ListBoxItem;
            //ListBoxItem item = new ListBoxItem()
            //{
            //    Name = old.Name,
            //    Content = old.Content
            //};
            //selectedListBox.Items.Add(item);
            //LearnersLB.Items.RemoveAt(selectedIndex);
            //for (int i = 0; i < 6; i++)
            //{
            //    for (int j = 0; j < 6; j++)
            //    {
            //        if (listBoxes[6*i + j].Item == selectedListBox)
            //        {
            //            listBoxes[6 * i + j].Changed = true;
            //        }
            //    }
            //}
        }

        private void GetFromTimeTable_Click(object sender, RoutedEventArgs e)
        {
            if(lastTimeSelected == null)
            {
                MessageBox.Show("Выделите поле в таблице расписаний");
                return;
            }
            if (CurrentSelectedItem.SelectedIndex == -1)
            {
                MessageBox.Show("Выделите обучающегося в списке");
                return;
            }
            saved = false;
            //ListBox listBox = null;
            Point lastpos = GetPositionOfLabel(lastTimeSelected);
            foreach (var elem in listBoxes)
            {
                if (elem.X == (int)lastpos.X && elem.Y == (int)lastpos.Y)
                {
                    selectedListBox = elem.Item;
                    break;
                }
            }
            ListBoxItem old = CurrentSelectedItem.SelectedItem/*Items.GetItemAt(CurrentSelectedItem.SelectedIndex)*/ as ListBoxItem;
            ListBoxItem item = new ListBoxItem()
            {
                Name = old.Name,
                Content = old.Content
            };
            LearnersLB.Items.Add(item);
            try
            {
                selectedListBox.Items.RemoveAt(CurrentSelectedItem.SelectedIndex);
            }
            catch(Exception ex)
            {

            }
            CurrentSelectedItem.Items.Remove(old);
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    if (listBoxes[6 * i + j].Item == selectedListBox)
                    {
                        listBoxes[6 * i + j].Changed = true;
                    }
                }
            }
            lastTimeSelected.Content = CurrentSelectedItem.Items.Count.ToString();
            //if (selectedListBox == null)
            //{
            //    MessageBox.Show("Выделите поле в таблице расписаний (Клик правой кнопкой мыши по полю)");
            //    return;
            //}
            //int selectedIndex = selectedListBox.SelectedIndex;
            //if (selectedIndex == -1)
            //{
            //    MessageBox.Show("Выделите обучающегося в таблице");
            //    return;
            //}
            //saved = false;
            //ListBoxItem old = selectedListBox.Items.GetItemAt(selectedIndex) as ListBoxItem;
            //ListBoxItem item = new ListBoxItem()
            //{
            //    Name = old.Name,
            //    Content = old.Content
            //};
            //LearnersLB.Items.Add(item);
            //selectedListBox.Items.RemoveAt(selectedIndex);
            //for (int i = 0; i < 6; i++)
            //{
            //    for (int j = 0; j < 6; j++)
            //    {
            //        if (listBoxes[6 * i + j].Item == selectedListBox)
            //        {
            //            listBoxes[6 * i + j].Changed = true;
            //        }
            //    }
            //}
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
        }

        private void HelpMI_Click(object sender, RoutedEventArgs e)
        {
            string helpFileName = @"C:\Users\Alexander\Documents\Visual Studio 2015\Projects\SportSchool\SportSchool\HelpFiles\AdminHelp.chm";
            if (System.IO.File.Exists(helpFileName))
            {
                System.Diagnostics.Process.Start(helpFileName);
            }
            else
            {
                MessageBox.Show("File doesn't exist; Current directory:" + System.IO.Directory.GetCurrentDirectory());
            }
        }

        private void FindLearner_Click(object sender, RoutedEventArgs e)
        {
            FindLearnerWindow flw = new FindLearnerWindow("learner");
            flw.ShowDialog();
        }

        private void FindInstructor_Click(object sender, RoutedEventArgs e)
        {
            FindLearnerWindow flw = new FindLearnerWindow("instructor");
            flw.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CheckIfSaved("dontErase");
        }
    }
}
