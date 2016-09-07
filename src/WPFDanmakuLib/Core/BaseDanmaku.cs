using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WPFDanmakuLib
{
    /// <summary>
    /// BaseDanmaku Class, Contains some basic danmaku style properties.
    /// Including Color, FontSize, Outline, Shadow, FontFamily, Position.
    /// </summary>
    public class BaseDanmaku
    {
        private byte _ColorR;
        private byte _ColorG;
        private byte _ColorB;
        private int _FontSize;
        private bool _Outline;
        private bool _Shadow;
        private FontFamily _FontFamily;

        private double _PositionX;
        private double _PositionY;

        private int _Duration;


        public byte ColorR
        {
            set
            {
                if (value >= 0 && value <= 255)
                {
                    this._ColorR = value;
                }
                else
                {
                    throw new ArgumentException("Color Range should between 0 and 255.");
                }
            }
            get { return this._ColorR; }
        }
        public byte ColorG
        {
            set
            {
                if (value >= 0 && value <= 255)
                {
                    this._ColorG = value;
                }
                else
                {
                    throw new ArgumentException("Color Range should between 0 and 255.");
                }
            }
            get { return this._ColorG; }
        }
        public byte ColorB
        {
            set
            {
                if (value >= 0 && value <= 255)
                {
                    this._ColorB = value;
                }
                else
                {
                    throw new ArgumentException("Color Range should between 0 and 255.");
                }
            }
            get { return this._ColorB; }
        }
        public int FontSize
        {
            set
            {
                if (value > 0)
                {
                    this._FontSize = value;
                }
                else
                {
                    throw new ArgumentException("FontSize should larger than zero.");
                }
            }
            get { return this._FontSize; }
        }
        public bool Outline {
            set {
                this._Outline = value;
            }
            get {
                return this._Outline;
            }
        }
        public bool Shadow
        {
            set { this._Shadow = value; }
            get { return this._Shadow; }
        }
        public FontFamily FontFamily
        {
            set
            {
                if (value == null)
                {
                    this._FontFamily = (FontFamily)new FontFamilyConverter().ConvertFromString("Microsoft YaHei");
                }
                else
                {
                    this._FontFamily = value;
                }
            }
            get { return this._FontFamily; }
        }
        public double PositionX
        {
            set { this._PositionX = value; }
            get { return this._PositionX; }
        }
        public double PositionY
        {
            set { this._PositionY = value; }
            get { return this._PositionY; }
        }
        public int Duration
        {
            set
            {
                if (value > 0)
                {
                    this._Duration = value;
                }
                else
                {
                    throw new ArgumentException("Duration should more than zero");
                }
            }
            get { return this._Duration; }
        }

        /// <summary>
        /// Create a object represents danmaku style.
        /// </summary>
        /// <param name="Duration">Danmaku duration for disapper (ms)</param>
        /// <param name="Content">Danmaku content</param>
        /// <param name="ColorR">Danmaku Color - Red</param>
        /// <param name="ColorG">Danmaku Color - Green</param>
        /// <param name="ColorB">Danmaku Color - Blue</param>
        /// <param name="FontSize">Danmaku font size</param>
        /// <param name="Outline">Enable danmaku outline stroke</param>
        /// <param name="Shadow">Enable danmaku shadow (low performance)</param>
        /// <param name="Font">Danmaku font style (Default: Microsoft YaHei)</param>
        /// <param name="PositionX">Danmaku start position - X</param>
        /// <param name="PositionY">Danmaku start position - Y</param>
        public BaseDanmaku(int Duration = 9000, byte ColorR = 255, byte ColorG = 255, byte ColorB = 255, int FontSize = 30, bool Outline = true, bool Shadow = false, FontFamily Font = null, double PositionX = 0, double PositionY = 0)
        {
            this.ColorR = ColorR;
            this.ColorG = ColorG;
            this.ColorB = ColorB;
            this.FontSize = FontSize;
            this.Outline = Outline;
            this.Shadow = Shadow;
            this.FontFamily = Font;

            this.PositionX = PositionX;
            this.PositionY = PositionY;

            this.Duration = Duration;
        }
    }
}
