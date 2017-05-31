using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace XamApp
{
    public partial class Page1 : ContentPage
    {
        int clicksAmount = 0;
        public Page1()
        {
            InitializeComponent();
            clickButton.Clicked += onButtonClicked;
        }

        private void onButtonClicked(object sender, System.EventArgs e)
        {
            clicksAmount++;
            label1.Text = "Ну ты и кликун! Накликал " + clicksAmount + " раз"; 
        }
    }
}
