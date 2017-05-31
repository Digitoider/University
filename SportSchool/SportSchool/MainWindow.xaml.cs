using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.ComponentModel;

namespace SportSchool
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        BackgroundWorker checkingBGworker;
        LoadingWindow loadingWindow = new LoadingWindow("Выполняется проверка...");
        public MainWindow()
        {
            InitializeComponent();
            loginTextBox.Text = "user154@mail.ru"/*"Admin"*/;// "user154@mail.ru";
            passwordBox.Password = "1111";
            InitTimer();
            checkingBGworker = new BackgroundWorker();
            checkingBGworker.DoWork += Check;
            checkingBGworker.RunWorkerCompleted += WorkerCompleted;
            checkingBGworker.ProgressChanged += BGWorkerProgressChanged;
            checkingBGworker.WorkerReportsProgress = true;
            //JournalWindow jw = new JournalWindow("user154@mail.ru");
            //jw.ShowDialog();
            //AdminWindow aw = new AdminWindow();
            //aw.ShowDialog();
            //this.Close();
        }

        private void BGWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            User = e.UserState as string;
        }

        string User = "none";
        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadingWindow.Hide();
            AnimationSportSchool.ShowElements(fieldsSP, 1);
            AnimationSportSchool.BlurOutImage(RunningWomanImage, 0);
            switch (User)
            {
                case "Admin":
                    this.Hide();
                    AdminWindow aw = new AdminWindow();
                    try
                    {
                        aw.ShowDialog();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Связь с БД разорвалась. Войдите в аккаунт");
                    }
                        this.Show();
                    break;
                case "Instructor":
                    this.Hide();
                    InstructorWindow iw = new InstructorWindow(loginTextBox.Text, passwordBox.Password);
                    try
                    {
                        iw.ShowDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Связь с БД разорвалась. Войдите в аккаунт");
                    }
                    this.Show();
                    break;
                default:
                    MessageBox.Show("Неверный логин или пароль");
                    break;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            AnimationSportSchool.HideElements(fieldsSP, 0.2);
            AnimationSportSchool.BlurInImage(RunningWomanImage, 7);


            loadingWindow.Show();
            LoginPassword lp = new LoginPassword()
            {
                Login = loginTextBox.Text,
                Password = passwordBox.Password
            };
            checkingBGworker.RunWorkerAsync(lp);
        }
        private void Check(object sender, DoWorkEventArgs e)
        {
            LoginPassword lp = e.Argument as LoginPassword;
            string user = "default";
            
            using (var authorizationClient = new AuthorizationServiceReference.AuthorizationServiceClient())
            {
                user = authorizationClient.CheckUser(lp.Login, lp.Password);
            }
            checkingBGworker.ReportProgress(100, user);
        }



        Point oldMousePosition = new Point(0, 0);
        System.Timers.Timer timer = new System.Timers.Timer();
        private void InitTimer()
        {
            timer.Interval = 300;
            timer.Elapsed += Animate;
            timer.AutoReset = true;
            timer.Enabled = true;
            timer.Start();
        }
        private void Animate(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                Point currentMousePosition = Mouse.GetPosition(loginButton);
                Point diff = new Point();
                diff.X = oldMousePosition.X - currentMousePosition.X;
                diff.Y = oldMousePosition.Y - currentMousePosition.Y;

                Thickness imgMargin = RunningWomanImage.Margin;
                int maxLeft, maxTop, maxRight, maxBottom;
                maxLeft = maxTop = maxRight = maxBottom = -25;

                double coefficient = 0.09;
                double left = imgMargin.Left + diff.X * coefficient;
                double top =  imgMargin.Top + diff.Y * coefficient;
                double right = imgMargin.Right - diff.X * coefficient;
                double bottom =  imgMargin.Bottom - diff.Y * coefficient;
                if (left > 0 || top > 0 || right > 0 || bottom > 0)
                {
                    left = maxLeft;
                    top = maxTop;
                    right = maxRight;
                    bottom = maxBottom;
                }
                ThicknessAnimation imgAnimation = new ThicknessAnimation()
                {
                    From = new Thickness(imgMargin.Left, imgMargin.Top, imgMargin.Right, imgMargin.Bottom),
                    To = new Thickness(left, top, right, bottom),//left<maxLeft? maxLeft: left, top < maxTop ? maxTop : top, right < maxRight ? maxRight : right, bottom < maxBottom ? maxBottom : bottom
                    //To = new Thickness(left<0?0:left, top<0?0:top, right, bottom),//right>this.Width?this.Width:right, bottom>this.Height?this.Height:bottom),
                    Duration = TimeSpan.FromMilliseconds(1000),

                    EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut }
                    //FillBehavior= FillBehavior.HoldEnd
                };
                RunningWomanImage.BeginAnimation(MarginProperty, imgAnimation);
                oldMousePosition = currentMousePosition;
            }));
        }

        private void OnAppClosing(object sender, CancelEventArgs e)
        {
            loadingWindow.Hide();
            System.Environment.Exit(1);
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            return;
            ThicknessAnimation imgAnimation = new ThicknessAnimation()
            {
                To = new Thickness(-195, -195, -195, -195),
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut }
            };
            RunningWomanImage.BeginAnimation(MarginProperty, imgAnimation);
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            return;
            ThicknessAnimation imgAnimation = new ThicknessAnimation()
            {
                To = new Thickness(-25, -25, -25, -25),
                Duration = TimeSpan.FromMilliseconds(1000),
                EasingFunction = new PowerEase() { EasingMode = EasingMode.EaseInOut }
            };
            RunningWomanImage.BeginAnimation(MarginProperty, imgAnimation);
        }
    }
    class LoginPassword
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
