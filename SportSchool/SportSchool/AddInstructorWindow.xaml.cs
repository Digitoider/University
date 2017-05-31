using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SportSchool
{
    /// <summary>
    /// Логика взаимодействия для AddInstructorWindow.xaml
    /// </summary>
    public partial class AddInstructorWindow : Window
    {
        private List<SportSchoolClassLibrary.Section> sections;
        public AddInstructorWindow()
        {
            InitializeComponent();
            AdminServiceReference.AdminServiceClient adminClient = new AdminServiceReference.AdminServiceClient();
            sections = adminClient.GetSections().ToList();
            adminClient.Close();
            SectionCB.DataContext = GetSectionNames();

            NameTB.Text = "f";
            SurnameTB.Text = "h";
            MiddleNameTB.Text = "h";
            TelephoneNumberTB.Text = "11";
            EmailTB.Text = "h1@mail.ru";
            PasswordTB.Text = "1111";

        }

        private List<string> GetSectionNames()
        {
            List<string> sectionNames = new List<string>();
            foreach (SportSchoolClassLibrary.Section section in sections)
            {
                sectionNames.Add(section.Name);
            }
            return sectionNames;
        }

        private void AddInstructor_Click(object sender, RoutedEventArgs e)
        {
            AddInstructor();
        }

        private void AddInstructor()
        {
            bool validated = Check();
            if (!validated) return;
            var Instructor = new SportSchoolClassLibrary.Instructor()
            {
                ID = 0,
                SectionID = GetSectionID(SectionCB.SelectedItem.ToString()),
                Name = NameTB.Text,
                Surname = SurnameTB.Text,
                MiddleName = MiddleNameTB.Text,
                TelephoneNumber = TelephoneNumberTB.Text,
                Email = EmailTB.Text,
                Password = PasswordTB.Text
            };
            var adminClient = new AdminServiceReference.AdminServiceClient();
            if (!adminClient.EmailExists(Instructor.Email))
            {
                adminClient.InsertNewInstructor(Instructor);
                WarningLbl.Margin = new Thickness(130, 20 * 17 - 5, 0, 0);

                ValueAdded.Content = "✔ Запись добавлена";
                DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(2));
                ValueAdded.BeginAnimation(UIElement.OpacityProperty, anim);
                anim.From = 1;
                anim.To = 0;
                ValueAdded.BeginAnimation(UIElement.OpacityProperty, anim);
                AnimateUIElement("AddBtn", "Green");
                ClearAllTextBoxes();
            }
            else
            {
                WarningLbl.Content = "← Данный Email уже используется";
                WarningLbl.Margin = new Thickness(130, 20 * 12 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("EmailTB");
            }
            adminClient.Close();
        }

        private void ClearAllTextBoxes()
        {
            NameTB.Text = "";
            SurnameTB.Text = "";
            MiddleNameTB.Text = "";
            TelephoneNumberTB.Text = "";
            EmailTB.Text = "";
            PasswordTB.Text = "";
        }

        private int GetSectionID(string sectionName)
        {
            foreach (var section in sections)
            {
                if (section.Name == sectionName) return section.ID;
            }
            return 1;
        }

        private bool Check()
        {
            int width = 130, height = 20;
            WarningLbl.Opacity = 0;
            WarningLbl.Foreground = Brushes.Red;

            if (SectionCB.SelectedIndex == -1)
            {
                WarningLbl.Content = "← Секция должна быть выбрана";
                WarningLbl.Margin = new Thickness(width, height * 2 - 10, 0, 0);
                AnimateWarningLabel();
                return false;
            }
            Regex regex = new Regex("^[а-яА-ЯёЁa-zA-Z]+$");

            TrimAllTextBoxes();

            if (NameTB.Text.Length == 0)
            {
                WarningLbl.Content = "← Поле не должно быть пустым";
                WarningLbl.Margin = new Thickness(width, height * 4 - 10, 0, 0);
                AnimateUIElement("NameTB");
                AnimateWarningLabel();
                AnimateUIElement("NameTB");
                return false;
            }
            if (!regex.IsMatch(NameTB.Text))
            {
                WarningLbl.Content = "← Имя должно быть введено корректно";
                WarningLbl.Margin = new Thickness(width, height * 4 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("NameTB");
                return false;
            }
            NameTB.Text = ModifyToCorrectForm(NameTB.Text);

            if (SurnameTB.Text.Length == 0)
            {
                WarningLbl.Content = "← Поле не должно быть пустым";
                WarningLbl.Margin = new Thickness(width, height * 6 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("SurnameTB");
                return false;
            }
            if (!regex.IsMatch(SurnameTB.Text))
            {
                WarningLbl.Content = "← Фамилия должна быть введена корректно";
                WarningLbl.Margin = new Thickness(width, height * 6 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("SurnameTB");
                return false;
            }
            SurnameTB.Text = ModifyToCorrectForm(SurnameTB.Text);

            if (MiddleNameTB.Text.Length == 0)
            {
                WarningLbl.Content = "← Поле не должно быть пустым";
                WarningLbl.Margin = new Thickness(width, height * 8 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("MiddleNameTB");
                return false;
            }
            if (!regex.IsMatch(MiddleNameTB.Text))
            {
                WarningLbl.Content = "← Отчество должно быть введено корректно";
                WarningLbl.Margin = new Thickness(width, height * 8 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("MiddleNameTB");
                return false;
            }
            MiddleNameTB.Text = ModifyToCorrectForm(MiddleNameTB.Text);

            if (TelephoneNumberTB.Text.Length == 0)
            {
                WarningLbl.Content = "← Поле не должно быть пустым";
                WarningLbl.Margin = new Thickness(width, height * 10 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("TelephoneNumberTB");
                return false;
            }

            Regex numberRegex = new Regex("^[0-9]+$");
            if (!numberRegex.IsMatch(TelephoneNumberTB.Text))
            {
                WarningLbl.Content = "← Допускается ввод только арабских цифр";
                WarningLbl.Margin = new Thickness(width, height * 10 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("TelephoneNumberTB");
                return false;
            }

            if (EmailTB.Text.Length == 0)
            {
                WarningLbl.Content = "← Поле не должно быть пустым";
                WarningLbl.Margin = new Thickness(width, height * 12 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("EmailTB");
                return false;
            }
            Regex emailRegex = new Regex(@"^[-\w.]+@([A-z0-9][-A-z0-9]+\.)+[A-z]{2,4}$");
            if (!emailRegex.IsMatch(EmailTB.Text))
            {
                WarningLbl.Content = "← Неверный формат ввода электронной почты";
                WarningLbl.Margin = new Thickness(width, height * 12 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("EmailTB");
                return false;
            }

            if (PasswordTB.Text.Length == 0)
            {
                WarningLbl.Content = "← Поле не должно быть пустым";
                WarningLbl.Margin = new Thickness(width, height * 14 - 10, 0, 0);
                AnimateWarningLabel();
                AnimateUIElement("PasswordTB");
                return false;
            }
            return true;
        }

        private void AnimateUIElement(string UIElementName, string from = "OrangeRed")
        {
            SolidColorBrush brush = new SolidColorBrush();
            switch (UIElementName)
            {
                case "NameTB": NameTB.BorderBrush = brush; break;
                case "SurnameTB": SurnameTB.BorderBrush = brush; break;
                case "MiddleNameTB": MiddleNameTB.BorderBrush = brush; break;
                case "TelephoneNumberTB": TelephoneNumberTB.BorderBrush = brush; break;
                case "EmailTB": EmailTB.BorderBrush = brush; break;
                case "PasswordTB": PasswordTB.BorderBrush = brush; break;
                case "AddBtn": AddBtn.BorderBrush = brush; break;
                default: return;
            }


            ColorAnimation anim = new ColorAnimation();
            switch (from)
            {
                case "Green": anim.From = Colors.Green; break;
                default: anim.From = Colors.OrangeRed; break;
            }
            anim.To = Colors.LightSlateGray;
            anim.Duration = new Duration(TimeSpan.FromSeconds(2));
            brush.BeginAnimation(SolidColorBrush.ColorProperty, anim);
        }

        private void TrimAllTextBoxes()
        {
            NameTB.Text = NameTB.Text.Trim(' ');
            SurnameTB.Text = SurnameTB.Text.Trim(' ');
            MiddleNameTB.Text = MiddleNameTB.Text.Trim(' ');
            TelephoneNumberTB.Text = TelephoneNumberTB.Text.Trim(' ');
            EmailTB.Text = EmailTB.Text.Trim(' ');
            PasswordTB.Text = PasswordTB.Text.Trim(' ');
        }

        private string ModifyToCorrectForm(string text)
        {
            if (text.Length == 0) return "";
            text = text.ToLower();
            return text.Replace(text, char.ToUpper(text[0]) + text.Remove(0, 1));
        }

        private void AnimateWarningLabel()
        {
            DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(2));
            WarningLbl.BeginAnimation(UIElement.OpacityProperty, anim);
            anim.From = 1;
            anim.To = 0;
            WarningLbl.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}

