using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Animation;

namespace SportSchool
{
    public partial class AddSectionWindow : Window
    {
        public AddSectionWindow()
        {
            InitializeComponent();
        }

        private void AddSection_Click(object sender, RoutedEventArgs e)
        {
            AddSection();
        }

        private void AddSection()
        {
            Regex regex = new Regex("^[a-zA-Zа-яА-я]+$");
            SectionNameTB.Text = SectionNameTB.Text.Trim(' ');
            SectionNameTB.Text = ModifyToCorrectForm(SectionNameTB.Text);

            if (!regex.IsMatch(SectionNameTB.Text))
            {
                SectionWarningLbl.Content = "← Введено недопустимое название секции";
                AnimateSectionWarningLabel();
                return;
            }
            if (SectionNameTB.Text.Length == 0)
            {
                SectionWarningLbl.Content = "← Поле для ввода названия секции не должно быть пустым";
                AnimateSectionWarningLabel();
                return;
            }
            var adminClient = new AdminServiceReference.AdminServiceClient();
            if (!adminClient.SectionExists(SectionNameTB.Text))
            {
                adminClient.InsertSection(new SportSchoolClassLibrary.Section()
                {
                    ID = 0,
                    Name = SectionNameTB.Text
                });

                SectionAddedLbl.Content = "✔ Секция добавлена";
                DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(2));
                anim.To = 0;
                anim.Duration = TimeSpan.FromSeconds(2);
                SectionAddedLbl.BeginAnimation(UIElement.OpacityProperty, anim);
                SectionNameTB.Text = "";
            }
            else
            {
                SectionWarningLbl.Content = "← Секция с таким названием уже существует";
                AnimateSectionWarningLabel();
            }
            adminClient.Close();
        }
        private string ModifyToCorrectForm(string text)
        {
            if (text.Length == 0) return "";
            text = text.ToLower();
            return text.Replace(text, char.ToUpper(text[0]) + text.Remove(0, 1));
        }
        private void AnimateSectionWarningLabel()
        {
            DoubleAnimation anim = new DoubleAnimation(1, TimeSpan.FromSeconds(2));
            SectionWarningLbl.BeginAnimation(UIElement.OpacityProperty, anim);
            anim.From = 1;
            anim.To = 0;
            SectionWarningLbl.BeginAnimation(UIElement.OpacityProperty, anim);
        }
    }
}
