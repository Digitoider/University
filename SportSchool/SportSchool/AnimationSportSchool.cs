using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace SportSchool
{
    class AnimationSportSchool
    {
        public static void AnimateInLabelMargin(Label label, int marginValue = 3)
        {

            ThicknessAnimation anim = new ThicknessAnimation()
            {
                To = new Thickness(marginValue),
                Duration = TimeSpan.FromSeconds(0.3),
            };
            label.BeginAnimation(Label.MarginProperty, anim);
        }
        public static void AnimateOutLabelMargin(Label label, int marginValue=0)
        {
            ThicknessAnimation anim = new ThicknessAnimation()
            {
                To = new Thickness(marginValue),
                Duration = TimeSpan.FromSeconds(0.3),
            };
            label.BeginAnimation(Label.MarginProperty, anim);
        }
        public static void  ShowElements(StackPanel panel, double ToValue)
        {
            DoubleAnimation anim = new DoubleAnimation()
            {
                //From = 0,
                To = ToValue,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            
            foreach (UIElement elem in panel.Children)
            {
                elem.Opacity = 0;
                elem.Visibility = Visibility.Visible;
                elem.BeginAnimation(UIElement.OpacityProperty, anim);
            }
        }
        public static void HideElements(StackPanel panel, double ToValue)
        {
            DoubleAnimation anim = new DoubleAnimation()
            {
                //From = 1,
                To = ToValue,
                Duration = TimeSpan.FromSeconds(0.5)
            };

            foreach (UIElement elem in panel.Children)
            {
                elem.BeginAnimation(UIElement.OpacityProperty, anim);
                //elem.Visibility = Visibility.Collapsed;
            }
        }
        public static void BlurInImage(Image img, double ToValue)
        {
            DoubleAnimation anim = new DoubleAnimation()
            {
                To = ToValue,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            img.BeginAnimation(BlurBitmapEffect.RadiusProperty, anim);
        }
        public static void BlurOutImage(Image img, double ToValue)
        {
            BlurEffect bl = new BlurEffect();
            
            DoubleAnimation anim = new DoubleAnimation()
            {
                To = ToValue,
                Duration = TimeSpan.FromSeconds(0.5)
            };
            img.BeginAnimation(BlurBitmapEffect.RadiusProperty, anim);
        }
    }
}
