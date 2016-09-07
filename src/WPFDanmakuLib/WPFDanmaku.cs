using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using WPFDanmakuLib.ExtraControl;

namespace WPFDanmakuLib {
    public class WPFDanmakuEngine {

        /// <summary>
        /// Get BindedCanvas from engine instance (Read only)
        /// </summary>
        public Canvas BindedCanvas {
            get;
            private set;
        }
        /// <summary>
        /// Get or set danmaku style for this engine instance
        /// </summary>
        public BaseDanmaku DefaultDanmakuStyle {
            get {
                return this.mDefaultDanmakuStyle;
            }
            set {
                this.mDefaultDanmakuStyle = value;
                GenerateCacheFromStyle();
            }
        }

        private BaseDanmaku mDefaultDanmakuStyle;

        private Random mRandomObj;
        private DanmakuManager mDanmakuMgr;

        // Cache - general
        private DropShadowEffect mCache_ShadowEffect;
        private Duration mCache_Duration;
        private SolidColorBrush mCache_SolidColorBrush;

        //Cache - R2L
        private PropertyPath mCache_R2LPropertyPath;

        /// <summary>
        /// Create a WPF danmaku engine instance
        /// </summary>
        /// <param name="TargetCanvas">Set Canvas element for drawing danmaku for this engine instance (Cannot change later)</param>
        /// <param name="DefaultStyle">Set default danmaku style for this engine instance</param>
        public WPFDanmakuEngine(Canvas TargetCanvas, BaseDanmaku DefaultStyle) {
            mRandomObj = new Random();
            if (TargetCanvas.IsLoaded) {
                this.BindedCanvas = TargetCanvas;
            } else {
                throw new InvalidOperationException("Canvas is not ready.");
            }

            this.mDefaultDanmakuStyle = DefaultStyle;
            GenerateCacheFromStyle();
        }

        private void GenerateCacheFromStyle() {
            mCache_ShadowEffect = new DropShadowEffect();
            mCache_ShadowEffect.RenderingBias = RenderingBias.Performance;
            mCache_ShadowEffect.Opacity = (double)100;
            mCache_ShadowEffect.ShadowDepth = (double)0;
            mCache_ShadowEffect.BlurRadius = (double)11;
            if ((mDefaultDanmakuStyle.ColorR + mDefaultDanmakuStyle.ColorG + mDefaultDanmakuStyle.ColorB + 1) / 3 >= 255 / 2) {
                mCache_ShadowEffect.Color = Color.FromRgb(0, 0, 0);
            } else {
                mCache_ShadowEffect.Color = Color.FromRgb(255, 255, 255);
            }
            
            mCache_SolidColorBrush = new SolidColorBrush(Color.FromRgb(mDefaultDanmakuStyle.ColorR, mDefaultDanmakuStyle.ColorG, mDefaultDanmakuStyle.ColorB));
            
            mCache_Duration = new Duration(TimeSpan.FromMilliseconds(mDefaultDanmakuStyle.Duration));
            Console.WriteLine(string.Format("Duration: {0}", mDefaultDanmakuStyle.Duration));

            mCache_R2LPropertyPath = new PropertyPath("(Canvas.Left)");
        }

        public string DrawDanmaku_R2L(string Content, BaseDanmaku Style = null) {
            SolidColorBrush _FillBrush;
            Duration _duration;
            if (Style == null || Style == mDefaultDanmakuStyle) {
                Style = mDefaultDanmakuStyle;
                _FillBrush = mCache_SolidColorBrush;
                _duration = mCache_Duration;
            } else {
                // replace cache
                _FillBrush = new SolidColorBrush(Color.FromRgb(Style.ColorR, Style.ColorG, Style.ColorB));
                _duration = new Duration(TimeSpan.FromMilliseconds(Style.Duration));
            }
            OutlinedTextBlock _thisDanmaku = new OutlinedTextBlock();
            _thisDanmaku.Name = "uni_" + Utils.getRandomString(5);

            // Style
            _thisDanmaku.Text = Content;
            _thisDanmaku.FontFamily = Style.FontFamily;
            _thisDanmaku.FontSize = Style.FontSize;
            _thisDanmaku.Fill = _FillBrush;
            _thisDanmaku.SetValue(Canvas.TopProperty, Style.PositionX);
            _thisDanmaku.SetValue(Canvas.LeftProperty, Style.PositionY);
            _thisDanmaku.FontWeight = FontWeights.Bold;

            if (Style.Shadow) {
                _thisDanmaku.Effect = mCache_ShadowEffect;
            }

            // Animation
            _thisDanmaku.Loaded += delegate (object o, RoutedEventArgs e) { AddR2LAnimation(_thisDanmaku.Name, _duration); };

            // Add to canvas
            BindedCanvas.Children.Add(_thisDanmaku);
            BindedCanvas.RegisterName(_thisDanmaku.Name, _thisDanmaku);

            return _thisDanmaku.Name;
        }

        private void AddR2LAnimation(string _UniqueName, Duration AppearDuration) {
            OutlinedTextBlock _targetDanmaku = BindedCanvas.FindName(_UniqueName) as OutlinedTextBlock;

            double _danmakuWidth = _targetDanmaku.ActualWidth;
            DoubleAnimation _doubleAnimation = new DoubleAnimation(BindedCanvas.ActualWidth, -_danmakuWidth, AppearDuration, FillBehavior.Stop);

            //Storyboard _sb = new Storyboard();
            //Storyboard.SetTarget(_doubleAnimation, _targetDanmaku);
            //Storyboard.SetTargetProperty(_doubleAnimation, mCache_R2LPropertyPath);
            _doubleAnimation.Completed += delegate (object o, EventArgs e) { removeOutdateDanmaku(_targetDanmaku.Name); };
            _targetDanmaku.BeginAnimation(Canvas.LeftProperty, _doubleAnimation);
            

            //_sb.Completed += delegate (object o, EventArgs e) { removeOutdateDanmaku(_targetDanmaku.Name); };

            //_sb.Children.Add(_doubleAnimation);
            //_sb.Begin();
        }

        private void removeOutdateDanmaku(string _UniqueName) {
            OutlinedTextBlock _targetDanmaku = BindedCanvas.FindName(_UniqueName) as OutlinedTextBlock;
            if (_targetDanmaku != null) {
                BindedCanvas.Children.Remove(_targetDanmaku);
                BindedCanvas.UnregisterName(_UniqueName);
                _targetDanmaku = null;
            }
        }
    }
}
