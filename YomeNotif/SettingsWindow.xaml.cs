using System.Windows;

namespace YomeNotif
{
    /// <summary>
    /// SettingsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsWindow : Window
    {
        string[] _data = new string[3];

        public SettingsWindow()
        {
            InitializeComponent();

            // 既存の設定内容を当てる
            if (Properties.Settings.Default.Ziho)
                EnableZiho.IsChecked = true;
            if (Properties.Settings.Default.Voice)
                PlayVoice.IsChecked = true;
            
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
            // テスト通知を発火
            var window = new NotifWindow(new string[] { "TEST", "熊野", "通知テスト" });
            window.Show();
        }
    }
}
