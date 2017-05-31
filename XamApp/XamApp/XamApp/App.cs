using CoreMotion;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.UI.Core;

namespace XamApp
{
    class TabbedControlPage : CarouselPage
    {
        public TabbedControlPage()
        {
            this.Title = "Мобильные ИТ";
            this.Children.Add(new FirstLab());
            this.Children.Add(new SecondLab());
            this.Children.Add(new ThirdLab());
            this.Children.Add(new FourthLab());
        }
    }
    class FirstLab: ContentPage
    {
        int clickAmount = 0;
        public FirstLab()
        {
            Button clickBtn = new Button()
            {
                Text = "Нажми на меня",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
            };
            StackLayout sl = new StackLayout();
            Label lb = new Label
            {
                Text = "Первая лаба",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
            };
            sl.Children.Add(lb);

            sl.Children.Add(clickBtn);
            clickBtn.Clicked += onButtonClicked;

            
            Content = sl;
        }
        public void onButtonClicked(object s, EventArgs e)
        {
            clickAmount++;
            ((Button)s).Text = "Ты нажал на кнопку " + clickAmount + " раз";
        }
    }
    class SecondLab : ContentPage
    {
        Entry entry;
        public SecondLab()
        {
            StackLayout sl = new StackLayout()
            {
                Padding = new Thickness(20),
            };

            sl.Children.Add(
                new Label() {
                    Text = "Вторая лаба",
                    Margin = new Thickness(20, 30, 20, 10),
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
               });
            entry = new Entry()
            {
                Keyboard = Keyboard.Numeric,
                Placeholder = "In degrees°"
            };
            sl.Children.Add(entry);
            Button tanButton = new Button()
            {
                Text = "Calculate tan()",
            };
            tanButton.Clicked += onTanButtonClicked;
            sl.Children.Add(tanButton);
            Content = sl;
        }

        private void onTanButtonClicked(object sender, EventArgs e)
        {
            int result;

            if(!Int32.TryParse(entry.Text.Replace(',', '.'), out result))
            {
                DisplayAlert("Warning", "Can't calculate", "OK");
                return;
            }
            DisplayAlert("Result", "tan(" + result + ") = " + Math.Tan(result*Math.PI/180), "OK");
        }
    }
    class ThirdLab : ContentPage
    {
        Accelerometer acc;
        public ThirdLab()
        {
            StackLayout sl = new StackLayout()
            {
                Padding = new Thickness(20),
            };

            sl.Children.Add(
                new Label()
                {
                    Text = "Третья лаба",
                    Margin = new Thickness(20, 30, 20, 10),
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
                });
            labelX = new Label() {
                Margin = new Thickness(20, 30, 20, 10),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            labelY = new Label()
            {
                Margin = new Thickness(20, 30, 20, 10),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            labelZ = new Label()
            {
                Margin = new Thickness(20, 30, 20, 10),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            sl.Children.Add(labelX);
            sl.Children.Add(labelY);
            sl.Children.Add(labelZ);
            //Device.StartTimer(TimeSpan.FromMilliseconds(100), accelerometer);
            Button btn = new Button()
            {
                Text = "Включить акселерометр",
                Margin = new Thickness(20, 30, 20, 10),
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            };
            if (acc != null)
            {
                acc.ReportInterval = 100;
                //acc.Shaken += OnPhoneShaked;
                acc.ReadingChanged += OnAccelerometerReadingsChanged;
                DisplayAlert("Warning", "Акселерометр включен", "Окей");
            }
            else
            {
                DisplayAlert("Warning", "Акселерометр не ранится", "Окей");
            }
            btn.Clicked += Btn_Clicked;
            sl.Children.Add(btn);
            Content = sl;

            acc = Accelerometer.GetDefault();
        }

        private void Btn_Clicked(object sender, EventArgs e)
        {
            
        }

        //private void OnPhoneShaked(Accelerometer sender, AccelerometerShakenEventArgs args)
        //{
        //    DisplayAlert("Внимание", "Мистер Трясун, вы дерныли телефон!", "Да знаю я!");
        //}

        private void OnAccelerometerReadingsChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs e)
        {
                AccelerometerReading reading = e.Reading;
                this.labelX.Text = "X: " + reading.AccelerationX;
                this.labelY.Text = "Y: " + reading.AccelerationY;
                this.labelZ.Text = "Z: " + reading.AccelerationZ;
        }

        //public UIAccelerometer Accelerometer { get; set; }
        Label labelX;
        Label labelY;
        Label labelZ;
        //CMMotionManager motionManager;
        //private bool accelerometer()
        //{
        //    motionManager = new CMMotionManager();
        //    motionManager.StartAccelerometerUpdates(NSOperationQueue.CurrentQueue, (data, error) =>
        //    {
        //        this.labelX.Text = "X: " + data.Acceleration.X.ToString("0.00000000");
        //        this.labelY.Text = "X: " + data.Acceleration.Y.ToString("0.00000000");
        //        this.labelZ.Text = "X: " + data.Acceleration.Z.ToString("0.00000000");
        //    });
        //    //labelX.Text = Accelerometer.ToString();
        //    return true;
        //}
    }
    class FourthLab : ContentPage
    {
        public FourthLab()
        {
            Content = new StackLayout
            {
                Children = {
                    new Label { Text = "Четвертая лаба",
                        FontSize = Device.GetNamedSize (NamedSize.Large, typeof(Label)) }
                }
            };
        }
    }
    public class App : Application
    {
        public App()
        {
            // The root page of your application
            

            MainPage = new NavigationPage(new TabbedControlPage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
