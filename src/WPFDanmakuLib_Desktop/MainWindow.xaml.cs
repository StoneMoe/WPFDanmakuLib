using System;
using System.Windows;
using WPFDanmakuLib;

namespace WPFDanmakuLib_Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WPFDanmakuEngine Engine;
        BaseDanmaku bd;

        Random ra;

        public MainWindow()
        {
            InitializeComponent();
            ra = new Random();
            bd = new BaseDanmaku();
        }

        private void DanmakuRender_Loaded(object sender, RoutedEventArgs e)
        {
            // Bind Canvas to WPFDanmakuLib, and set default danmaku style
            Engine = new WPFDanmakuEngine(DanmakuRender, new BaseDanmaku());
            
            // Draw a R2L danmaku with default style
            Engine.DrawDanmaku_R2L("Red Area is Canvas.");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Fire button Click

            // Set a random X position
            bd.PositionX = ra.Next(0, 300);

            // override default danmaku style
            Engine.DrawDanmaku_R2L(getRandomString(200), bd);
        }

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
    }
}
