using System;
using System.IO;
using System.Timers;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace YomeNotif
{
    /// <summary>
    /// NotifWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class NotifWindow : Window
    {
        Timer timer = new Timer();

        public NotifWindow(string[] data)
        {
            InitializeComponent();

            SizeToContent = SizeToContent.Height;

            // 画面右下に表示させる
            var desktop = SystemParameters.WorkArea;
            this.Left = desktop.Right - (this.Width + 8);
            this.Top = desktop.Bottom - (this.Height + 8);

            setwindow();
            notifFire(data[0], data[1], data[2]);
        }

        public void setwindow()
        {
            var bc = new BrushConverter();

            // カラーテーマによって色を変える
            if (GetAppsUseLightTheme() == 1)
            {
                Background = (Brush)bc.ConvertFrom("#E0F5F5F5");
                BorderBrush = (Brush)bc.ConvertFrom("#E0E53935");
                Foreground = (Brush)bc.ConvertFrom("#FF212121");
                CloseButton.Foreground = (Brush)bc.ConvertFrom("#FF424242");
                TimeView.Foreground = (Brush)bc.ConvertFrom("#FF616161");
            }
        }

        // UIを組み立てる
        public void notifFire(string ImageFile, string Name, string Text)
        {
            // 画像ファイルの存在を確認したうえで表示
            if (File.Exists(ImageFile) && !ImageFile.Equals("TEST"))
                ImageView.ImageSource = new BitmapImage(new Uri(ImageFile));
            else if (ImageFile.Equals("TEST"))
                ImageView.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Resources/graph_kumano.jpg"));

            NameView.Text = Name;
            TextView.Text = Text;

            DateTime dt = DateTime.Now;
            TimeView.Text = dt.ToString("HH:mm");

            timer = new Timer();
            timer.AutoReset = false;
            timer.Interval = 6000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;
        }

        private int GetAppsUseLightTheme()
        {
            int getmode = -1;

            // 操作するレジストリ・キーの名前
            string rKeyName = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
            // 取得処理を行う対象となるレジストリの値の名前
            string rGetValueName = "AppsUseLightTheme";

            // レジストリの取得
            try
            {
                // レジストリ・キーのパスを指定してレジストリを開く
                Microsoft.Win32.RegistryKey rKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(rKeyName);

                // レジストリの値を取得(DWord)
                getmode = (int)rKey.GetValue(rGetValueName);

                // 開いたレジストリ・キーを閉じる
                rKey.Close();
            }
            catch (NullReferenceException)
            {
                // レジストリ・キーまたは値が存在しない
                Console.WriteLine("レジストリ［" + rKeyName
                  + "］の［" + rGetValueName + "］がありません！");
            }
            return getmode;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Close();
            GC.Collect();
            Application.Current.Dispatcher.Invoke(
                    new Action(this.Close)
                );
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            this.Close();
        }

        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 画面右下に表示させる
            var desktop = SystemParameters.WorkArea;
            this.Left = desktop.Right - (this.Width + 8);
            this.Top = desktop.Bottom - (ActualHeight + 8);
            Console.WriteLine("初期値: " + this.Height + "\n変更後: " + this.ActualHeight);
        }
    }
}
