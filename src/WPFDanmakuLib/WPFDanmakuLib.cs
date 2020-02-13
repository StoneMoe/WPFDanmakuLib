using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;

namespace WPFDanmakuLib {
    public class WPFDanmakuEngine {

        /// <summary>
        /// Create a WPFDanmakuEngine instance
        /// </summary
        /// <param name="engineBehavior">Danmaku engine behavior configuration</param>
        /// <param name="defaultStyle">Default danmaku style</param>
        /// <param name="targetCanvas">Danmaku container for WPF mode</param>
        public WPFDanmakuEngine(EngineBehavior engineBehavior, DanmakuStyle defaultStyle, Canvas targetCanvas = null) {
            this.EngineBehavior = engineBehavior;
            this.DefaultDanmakuStyle = defaultStyle;

            if (this.EngineBehavior.DrawMode == DrawMode.DX) {
                throw new NotImplementedException("Not Implemented");
            } else if (this.EngineBehavior.DrawMode == DrawMode.WPF) {
                if (targetCanvas == null) {
                    throw new InvalidOperationException("MUST specify a Canvas in WPF Mode");
                }
                if (targetCanvas.IsLoaded) {
                    this.mBindingCanvas = targetCanvas;
                } else {
                    throw new InvalidOperationException("Canvas is not ready");
                }
            } else {
                throw new NotImplementedException("Not Implemented");
            }

            if (this.EngineBehavior.CollisionPrevention == CollisionPrevention.Enabled) {
                // test actual height of textblock
                var formattedText = new FormattedText(
                    "测试_test",
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(this.DefaultDanmakuStyle.FontFamily.Source),
                    this.DefaultDanmakuStyle.FontSize,
                    Brushes.Black,
                    new NumberSubstitution(),
                    TextFormattingMode.Display);
                this.mSlotManager = new SlotManager(this.mBindingCanvas.ActualHeight, formattedText.Height);
            }
        }

        // Settings
        public EngineBehavior EngineBehavior {
            get;
            private set;
        }
        private DanmakuStyle mDefaultDanmakuStyle;
        public DanmakuStyle DefaultDanmakuStyle {
            get {
                return this.mDefaultDanmakuStyle;
            }
            set {
                this.mDefaultDanmakuStyle = value;
                if (this.EngineBehavior.DrawMode == DrawMode.WPF) GenerateWPFCache();
            }
        }
        private Canvas mBindingCanvas;
        public Canvas BindingCanvas {
            get {
                if (this.EngineBehavior.DrawMode == DrawMode.WPF) {
                    return mBindingCanvas;
                } else {
                    throw new InvalidOperationException("Only accesiable in compatibility mode");
                }
            }
            private set { }
        }

        // Slot manager
        private SlotManager mSlotManager;

        // Cache
        private DropShadowEffect mCache_ShadowEffect;
        private Duration mCache_Duration;
        private SolidColorBrush mCache_SolidColorBrush;
        private Typeface mCache_Typeface;

        private void GenerateWPFCache() {
            mCache_ShadowEffect = new DropShadowEffect();
            mCache_ShadowEffect.RenderingBias = RenderingBias.Performance;
            mCache_ShadowEffect.Opacity = (double)100;
            mCache_ShadowEffect.ShadowDepth = (double)0;
            mCache_ShadowEffect.BlurRadius = (double)11;
            if ((DefaultDanmakuStyle.ColorR + DefaultDanmakuStyle.ColorG + DefaultDanmakuStyle.ColorB + 1) / 3 >= 255 / 2) {
                mCache_ShadowEffect.Color = Color.FromRgb(0, 0, 0);
            } else {
                mCache_ShadowEffect.Color = Color.FromRgb(255, 255, 255);
            }
            mCache_SolidColorBrush = new SolidColorBrush(Color.FromRgb(DefaultDanmakuStyle.ColorR, DefaultDanmakuStyle.ColorG, DefaultDanmakuStyle.ColorB));
            mCache_Duration = new Duration(TimeSpan.FromMilliseconds(DefaultDanmakuStyle.Duration));
            mCache_Typeface = new Typeface(DefaultDanmakuStyle.FontFamily.Source);
        }

        /// <summary>
        /// Draw a danmaku
        /// </summary>
        /// <param name="Content">Danmaku content</param>
        /// <param name="Style">Override default danmaku style if needed</param>
        /// <returns></returns>
        public string DrawDanmaku(string Content, DanmakuStyle Style = null) {
            if (this.EngineBehavior.DrawMode == DrawMode.WPF) {
                return DrawDanmaku_WPF(Content, Style);
            } else if (this.EngineBehavior.DrawMode == DrawMode.DX) {
                return DrawDanmaku_DX(Content, Style);
            } else {
                throw new InvalidOperationException();
            }
        }
        private string DrawDanmaku_WPF(string Content, DanmakuStyle Style = null) {
            Typeface _Typeface;
            SolidColorBrush _FillBrush;
            DropShadowEffect _ShadowEffect;
            Duration _duration;

            // Validating cache
            if (Style == null) {
                Style = DefaultDanmakuStyle;
            }
            if (Style == DefaultDanmakuStyle) {
                _Typeface = mCache_Typeface;
                _FillBrush = mCache_SolidColorBrush;
                _ShadowEffect = mCache_ShadowEffect;
                _duration = mCache_Duration;
            } else {
                _Typeface = new Typeface(Style.FontFamily.Source);
                if (Style.Duration != DefaultDanmakuStyle.Duration) {
                    _duration = new Duration(TimeSpan.FromMilliseconds(Style.Duration));
                } else {
                    _duration = mCache_Duration;
                }

                if (Style.ColorR != DefaultDanmakuStyle.ColorR || Style.ColorG != DefaultDanmakuStyle.ColorG || Style.ColorB != DefaultDanmakuStyle.ColorB) {
                    _FillBrush = new SolidColorBrush(Color.FromRgb(Style.ColorR, Style.ColorG, Style.ColorB));
                    _ShadowEffect = mCache_ShadowEffect;
                    if ((Style.ColorR + Style.ColorG + Style.ColorB + 1) / 3 >= 255 / 2) {
                        _ShadowEffect.Color = Color.FromRgb(0, 0, 0);
                    } else {
                        _ShadowEffect.Color = Color.FromRgb(255, 255, 255);
                    }
                } else {
                    _FillBrush = mCache_SolidColorBrush;
                    _ShadowEffect = mCache_ShadowEffect;
                }
            }

            // Apply style
            OutlinedTextBlock _thisDanmaku = new OutlinedTextBlock(Style.OutlineEnabled);
            _thisDanmaku.Name = "uni_" + Utils.GetRandomString(5);

            _thisDanmaku.Text = Content;
            _thisDanmaku.FontFamily = Style.FontFamily;
            _thisDanmaku.FontSize = Style.FontSize;
            _thisDanmaku.Fill = _FillBrush;
            _thisDanmaku.SetValue(Canvas.LeftProperty, Style.PositionX);
            if (this.EngineBehavior.CollisionPrevention == CollisionPrevention.Enabled) {
                int slot = mSlotManager.getIdleSlot();
                _thisDanmaku.SetValue(Canvas.TopProperty, slot * mSlotManager.TextHeight);
                mSlotManager.LockSlot(slot);
                _thisDanmaku.Unloaded += delegate (object sender, RoutedEventArgs e) { mSlotManager.UnlockSlot(slot); };
            } else {
                _thisDanmaku.SetValue(Canvas.TopProperty, Style.PositionY);
            }
            _thisDanmaku.FontWeight = FontWeights.Bold;
            if (Style.ShadowEnabled) {
                _thisDanmaku.Effect = _ShadowEffect;
            }

            // Apply animation
            if (Style.Direction == DanmakuDirection.R2L) {
                _thisDanmaku.Loaded += delegate (object o, RoutedEventArgs e) { AddR2LAnimation(_thisDanmaku.Name, _duration, Style.PositionX); };
            } else if (Style.Direction == DanmakuDirection.L2R) {
                _thisDanmaku.Loaded += delegate (object o, RoutedEventArgs e) { AddL2RAnimation(_thisDanmaku.Name, _duration, Style.PositionX); };
            } else {
                throw new InvalidOperationException();

            }

            // Add to canvas
            BindingCanvas.Children.Add(_thisDanmaku);
            BindingCanvas.RegisterName(_thisDanmaku.Name, _thisDanmaku);

            return _thisDanmaku.Name;

        }

        private string DrawDanmaku_DX(string Content, DanmakuStyle Style = null) {
            throw new NotImplementedException();
        }


        private void AddR2LAnimation(string _UniqueName, Duration AppearDuration, double InitPositionX) {
            OutlinedTextBlock _targetDanmaku = BindingCanvas.FindName(_UniqueName) as OutlinedTextBlock;
            if (_targetDanmaku == null || _targetDanmaku.IsLoaded == false) {
                throw new InvalidOperationException("Make sure OutlinedTextBlock is ready.");
            }

            double _danmakuWidth = _targetDanmaku.ActualWidth;
            DoubleAnimation _doubleAnimation = new DoubleAnimation(InitPositionX, -_danmakuWidth, AppearDuration, FillBehavior.Stop);
            _doubleAnimation.Completed += delegate (object o, EventArgs e) { removeOutdateDanmaku(_targetDanmaku.Name); };
            _targetDanmaku.BeginAnimation(Canvas.LeftProperty, _doubleAnimation);
        }
        private void AddL2RAnimation(string _UniqueName, Duration AppearDuration, double InitPositionX) {
            throw new NotImplementedException();
        }

        private void removeOutdateDanmaku(string _UniqueName) {
            OutlinedTextBlock _targetDanmaku = BindingCanvas.FindName(_UniqueName) as OutlinedTextBlock;
            if (_targetDanmaku != null) {
                BindingCanvas.Children.Remove(_targetDanmaku);
                BindingCanvas.UnregisterName(_UniqueName);
                _targetDanmaku.Dispose();
            }
        }
    }
}
