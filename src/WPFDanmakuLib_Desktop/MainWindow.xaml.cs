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
        WPFDanmaku o;
        BaseDanmaku bd;

        Random ra;

        public MainWindow()
        {
            InitializeComponent();
            ra = new Random();
        }

        private void DanmakuRender_Loaded(object sender, RoutedEventArgs e)
        {
            //bind Canvas and WPFDanmakuLib.
            o = new WPFDanmaku(DanmakuRender);

            //get a Base Danmaku Class.
            bd = new BaseDanmaku("initContentOrSomethingElse");

            //You can set the Content of Base Danmaku Class like this.
            bd.Content = "Red Area is Canvas.";

            //Send the Danmaku to Screen like this.
            o.DrawDanmaku(bd);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Fire button part

            //Set Content to some random string.
            bd.Content = getRandomString(5);

            //set danmaku X position to a random value.
            bd.PositionX = ra.Next(0, 300);

            //Send Danmaku to screen.
            o.DrawDanmaku(bd);
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
