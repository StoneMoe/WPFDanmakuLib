using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace WPFDanmakuLib
{
    /// <summary>
    /// BaseDanmaku Class, Contains some basic danmaku styles.
    /// Including Color, FontSize, Shadow, FontFamily, Position and Content.
    /// </summary>
    public class BaseDanmaku
    {
        private byte _ColorR;
        private byte _ColorG;
        private byte _ColorB;
        private int _FontSize;
        private bool _Shadow;
        private FontFamily _FontFamily;

        private double _PositionX;
        private double _PositionY;

        private string _Content;

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
        public string Content
        {
            set
            {
                if (value != string.Empty && value != null)
                {
                    this._Content = value;
                }
                else
                {
                    throw new ArgumentException("Content should not be empty/null.");
                }
            }
            get { return this._Content; }
        }

        public BaseDanmaku(string Content, byte ColorR = 255, byte ColorG = 255, byte ColorB = 255, int FontSize = 30, bool Shadow = true, FontFamily Font = null, double PositionX = 0, double PositionY = 0)
        {
            this.ColorR = ColorR;
            this.ColorG = ColorG;
            this.ColorB = ColorB;
            this.FontSize = FontSize;
            this.Shadow = Shadow;
            this.FontFamily = Font;

            this.PositionX = PositionX;
            this.PositionY = PositionY;


            this.Content = Content;
        }
    }
}
