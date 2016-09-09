using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using WPFDanmakuLib.ExtraControl;

namespace WPFDanmakuLib {
    public class WPFDanmakuEngine {

        private Random mRandomObj;

        public enum DrawMode {
            Compatibility,
            Performance
        }
        public DrawMode CurrentDrawMode {
            get;
            private set;
        }

        /// <summary>
        /// Get current binding canvas (Compatibility mode only)
        /// </summary>
        private Canvas mBindingCanvas;
        public Canvas BindingCanvas {
            get {
                if (CurrentDrawMode == DrawMode.Compatibility) {
                    return mBindingCanvas;
                } else {
                    throw new InvalidOperationException("Only accesiable in compatibility mode");
                }
            }
            private set {
            }
        }

        /// <summary>
        /// Get or set default danmaku style
        /// </summary>
        private BaseDanmaku mDefaultStyle;
        public BaseDanmaku DefaultDanmakuStyle {
            get {
                return this.mDefaultStyle;
            }
            set {
                this.mDefaultStyle = value;
                if (CurrentDrawMode == DrawMode.Compatibility) GenerateWPFCache();
            }
        }

        // WPFCache - Core
        private DropShadowEffect mCache_ShadowEffect;
        private Duration mCache_Duration;
        private SolidColorBrush mCache_SolidColorBrush;

        // WPFCache - R2L
        private PropertyPath mCache_R2LPropertyPath;

        private void GenerateWPFCache() {
            mCache_ShadowEffect = new DropShadowEffect();
            mCache_ShadowEffect.RenderingBias = RenderingBias.Performance;
            mCache_ShadowEffect.Opacity = (double)100;
            mCache_ShadowEffect.ShadowDepth = (double)0;
            mCache_ShadowEffect.BlurRadius = (double)11;
            if ((mDefaultStyle.ColorR + mDefaultStyle.ColorG + mDefaultStyle.ColorB + 1) / 3 >= 255 / 2) {
                mCache_ShadowEffect.Color = Color.FromRgb(0, 0, 0);
            } else {
                mCache_ShadowEffect.Color = Color.FromRgb(255, 255, 255);
            }

            mCache_SolidColorBrush = new SolidColorBrush(Color.FromRgb(mDefaultStyle.ColorR, mDefaultStyle.ColorG, mDefaultStyle.ColorB));

            mCache_Duration = new Duration(TimeSpan.FromMilliseconds(mDefaultStyle.Duration));
            Console.WriteLine(string.Format("Duration: {0}", mDefaultStyle.Duration));

            mCache_R2LPropertyPath = new PropertyPath("(Canvas.Left)");
        }

        /// <summary>
        /// Create a WPFDanmakuEngine instance
        /// </summary>
        /// <param name="TargetCanvas">Danmaku container</param>
        /// <param name="DefaultStyle">Default danmaku style</param>
        public WPFDanmakuEngine(Canvas TargetCanvas, BaseDanmaku DefaultStyle, DrawMode Mode) {
            mRandomObj = new Random();
            if (Mode == DrawMode.Performance) {
                throw new NotImplementedException("Still working on this");
            } else {
                CurrentDrawMode = DrawMode.Compatibility;

                if (TargetCanvas.IsLoaded) {
                    this.mBindingCanvas = TargetCanvas;
                } else {
                    throw new InvalidOperationException("Canvas is not ready.");
                }

                this.mDefaultStyle = DefaultStyle;
                GenerateWPFCache();
            }
        }

        #region WPF
        /// <summary>
        /// Draw a right to left danmaku on binding canvas
        /// </summary>
        /// <param name="Content">Danmaku content</param>
        /// <param name="Style">Override default danmaku style if needed</param>
        /// <returns></returns>
        public string DrawDanmaku_R2L(string Content, BaseDanmaku Style = null) {
            SolidColorBrush _FillBrush;
            Duration _duration;
            DropShadowEffect _ShadowEffect;
            if (Style == null || Style == mDefaultStyle) {
                Style = mDefaultStyle;
                _FillBrush = mCache_SolidColorBrush;
                _ShadowEffect = mCache_ShadowEffect;
                _duration = mCache_Duration;
            } else {
                if (Style.Duration != mDefaultStyle.Duration) {
                    _duration = new Duration(TimeSpan.FromMilliseconds(Style.Duration));
                } else {
                    _duration = mCache_Duration;
                }

                if (Style.ColorR != mDefaultStyle.ColorR || Style.ColorG != mDefaultStyle.ColorG || Style.ColorB != mDefaultStyle.ColorB) {
                    _FillBrush = new SolidColorBrush(Color.FromRgb(Style.ColorR, Style.ColorG, Style.ColorB));
                    _ShadowEffect = mCache_ShadowEffect;
                    if ((Style.ColorR + Style.ColorG + Style.ColorB + 1) / 3 >= 255 / 2) {
                        _ShadowEffect.Color = Color.FromRgb(0, 0, 0);
                    } else {
                        _ShadowEffect.Color = Color.FromRgb(255, 255, 255);
                    }
                } else {
                    _ShadowEffect = mCache_ShadowEffect;
                    _FillBrush = mCache_SolidColorBrush;
                }
            }

            OutlinedTextBlock _thisDanmaku = new OutlinedTextBlock();
            _thisDanmaku.Name = "uni_" + Utils.GetRandomString(5);

            // Style
            _thisDanmaku.Text = Content;
            _thisDanmaku.FontFamily = Style.FontFamily;
            _thisDanmaku.FontSize = Style.FontSize;
            _thisDanmaku.Fill = _FillBrush;
            _thisDanmaku.SetValue(Canvas.TopProperty, Style.PositionX);
            _thisDanmaku.SetValue(Canvas.LeftProperty, Style.PositionY);
            _thisDanmaku.FontWeight = FontWeights.Bold;

            if (Style.Shadow) {
                _thisDanmaku.Effect = _ShadowEffect;
            }

            // Animation
            _thisDanmaku.Loaded += delegate (object o, RoutedEventArgs e) { AddR2LAnimation(_thisDanmaku.Name, _duration); };

            // Add to canvas
            BindingCanvas.Children.Add(_thisDanmaku);
            BindingCanvas.RegisterName(_thisDanmaku.Name, _thisDanmaku);

            return _thisDanmaku.Name;
        }

        private void AddR2LAnimation(string _UniqueName, Duration AppearDuration) {
            OutlinedTextBlock _targetDanmaku = BindingCanvas.FindName(_UniqueName) as OutlinedTextBlock;

            double _danmakuWidth = _targetDanmaku.ActualWidth;
            DoubleAnimation _doubleAnimation = new DoubleAnimation(BindingCanvas.ActualWidth, -_danmakuWidth, AppearDuration, FillBehavior.Stop);

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
            OutlinedTextBlock _targetDanmaku = BindingCanvas.FindName(_UniqueName) as OutlinedTextBlock;
            if (_targetDanmaku != null) {
                BindingCanvas.Children.Remove(_targetDanmaku);
                BindingCanvas.UnregisterName(_UniqueName);
                _targetDanmaku = null;
            }
        }
        #endregion

    }
}
