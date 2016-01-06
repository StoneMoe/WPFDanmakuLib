using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace WPFDanmakuLib
{
    public class WPFDanmaku
    {
        private Random ra;
        private Canvas _MainLayout;

        public WPFDanmaku(Canvas _targetCanvas)
        {
            ra = new Random();

            _MainLayout = _targetCanvas;
        }

        public void DrawDanmaku(BaseDanmaku _resource)
        {
            TextBlock _singleDanmaku = new TextBlock();
            _singleDanmaku.Name = "uni_" + getRandomString(ra.Next(5, 8)); //unique ID for destory itself

            //Style
            _singleDanmaku.Text = _resource.Content;
            _singleDanmaku.FontFamily = _resource.FontFamily;
            _singleDanmaku.FontSize = _resource.FontSize;
            _singleDanmaku.Foreground = new SolidColorBrush(Color.FromRgb(_resource.ColorR, _resource.ColorG, _resource.ColorB));
            _singleDanmaku.SetValue(Canvas.TopProperty, _resource.PositionX);
            _singleDanmaku.SetValue(Canvas.LeftProperty, _resource.PositionY);

            if (_resource.Shadow)
            {
                DropShadowEffect _ef = new DropShadowEffect();

                _ef.RenderingBias = RenderingBias.Performance;
                _ef.Opacity = (double)100;
                _ef.ShadowDepth = (double)0;
                _ef.BlurRadius = (double)11;

                if ((_resource.ColorR + _resource.ColorG + _resource.ColorB + 1) / 3 >= 255 / 2)
                {
                    _ef.Color = Color.FromRgb(0, 0, 0);
                }
                else
                {
                    _ef.Color = Color.FromRgb(255, 255, 255);
                }

                _singleDanmaku.Effect = _ef;
            }

            _singleDanmaku.Loaded += delegate(object o, RoutedEventArgs e) { addAnimation(_singleDanmaku.Name); };

            //Add to MainLayout Canvas
            _MainLayout.Children.Add(_singleDanmaku);
            _MainLayout.RegisterName(_singleDanmaku.Name, _singleDanmaku);
        }

        private void addAnimation(string _UniqueName)
        {
            TextBlock _targetDanmaku = _MainLayout.FindName(_UniqueName) as TextBlock;

            double _danmakuWidth = _targetDanmaku.ActualWidth;
            DoubleAnimation _doubleAnimation = new DoubleAnimation(_MainLayout.ActualWidth, -_danmakuWidth, new Duration(TimeSpan.FromMilliseconds(9000)), FillBehavior.Stop);

            Storyboard _sb = new Storyboard();
            Storyboard.SetTarget(_doubleAnimation, _targetDanmaku);
            Storyboard.SetTargetProperty(_doubleAnimation, new PropertyPath("(Canvas.Left)"));

            _sb.Completed += delegate(object o, EventArgs e) { removeOutdateDanmaku(_targetDanmaku.Name); }; //remove danmaku after animation end

            _sb.Children.Add(_doubleAnimation);
            _sb.Begin();
        }

        private void removeOutdateDanmaku(string _UniqueName)
        {
            TextBlock _targetDanmaku = _MainLayout.FindName(_UniqueName) as TextBlock;
            if (_targetDanmaku != null)
            {
                _MainLayout.Children.Remove(_targetDanmaku);
                _MainLayout.UnregisterName(_UniqueName);
                _targetDanmaku = null;
            }
        }

        #region Helper
        private string getRandomString(int _Length)
        {
            string _strList = "qwertyuioplkjhgfdsazxcvbnm1234567890";
            string _buffer = "";
            for (int i = 1; i <= _Length; i++)
            {
                _buffer += _strList[ra.Next(0, 35)];
            }
            return _buffer;
        }
        #endregion
    }
}
