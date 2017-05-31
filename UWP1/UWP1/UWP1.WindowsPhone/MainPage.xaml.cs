using System;
using Windows.Devices.Sensors;
using Windows.UI.Core;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// Шаблон элемента пустой страницы задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP1
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Accelerometer acc;
        public MainPage()
        {
            this.InitializeComponent();
            acc = Accelerometer.GetDefault();   
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Вызывается перед отображением этой страницы во фрейме.
        /// </summary>
        /// <param name="e">Данные события, описывающие, каким образом была достигнута эта страница.
        /// Этот параметр обычно используется для настройки страницы.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Подготовьте здесь страницу для отображения.

            // TODO: Если приложение содержит несколько страниц, обеспечьте
            // обработку нажатия аппаратной кнопки "Назад", выполнив регистрацию на
            // событие Windows.Phone.UI.Input.HardwareButtons.BackPressed.
            // Если вы используете NavigationHelper, предоставляемый некоторыми шаблонами,
            // данное событие обрабатывается для вас.
        }

        private void OnStartButtinClicked(object sender, RoutedEventArgs e)
        {
            if (acc != null)
            {
                // Establish the report interval
                acc.ReportInterval = 100;

                //Window.Current.VisibilityChanged += new WindowVisibilityChangedEventHandler(VisibilityChanged);
                acc.ReadingChanged += new TypedEventHandler<Accelerometer, AccelerometerReadingChangedEventArgs>(ReadingChanged);
            }
        }
        async private void ReadingChanged(object sender, AccelerometerReadingChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                AccelerometerReading reading = e.Reading;
                xAxis.Text = String.Format("{0,5:0.00}", reading.AccelerationX);
                yAxis.Text = String.Format("{0,5:0.00}", reading.AccelerationY);
                zAxis.Text = String.Format("{0,5:0.00}", reading.AccelerationZ);
                xAxisSlider.Value = reading.AccelerationX;
                yAxisSlider.Value = reading.AccelerationY;
                zAxisSlider.Value = reading.AccelerationZ;
            });
        }
    }
}
