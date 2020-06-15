using System.Windows;

namespace YomeNotif
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string[] _data = new string[3];

        public SettingsWindow(string[] data)
        {
            InitializeComponent();
            if (Properties.Settings.Default.Ziho)
                EnableZiho.IsChecked = true;
            if (Properties.Settings.Default.Voice)
                PlayVoice.IsChecked = true;

            _data[0] = data[0];
            _data[1] = data[1];
            _data[2] = data[2];
            if (data[0] == "" || data[1] == "" || data[2] == "")
                TestNotif.IsEnabled = false;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)EnableZiho.IsChecked)
                Properties.Settings.Default.Ziho = true;
            if ((bool)PlayVoice.IsChecked)
                Properties.Settings.Default.Voice = true;
            Properties.Settings.Default.Save();
            MessageBox.Show("設定を保存しました。", "設定保存", MessageBoxButton.OK, MessageBoxImage.Information) ;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult msgbox = MessageBox.Show("設定を破棄します。\nよろしいですか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
            if (msgbox == MessageBoxResult.Yes)
                Close();
        }

        private void TestNotif_Click(object sender, RoutedEventArgs e)
        {
            var window = new NotifWindow(_data);
            window.Show();
        }
    }
}
