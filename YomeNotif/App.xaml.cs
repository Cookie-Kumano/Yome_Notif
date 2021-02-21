using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace YomeNotif
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary

    public partial class App : Application
    {
        static string databaseName = "Yome.db";
        static string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string DatabasePath = System.IO.Path.Combine(folderPath, databaseName);


        // タスクトレイに表示するアイコン
        private NotifyIconWrapper notifyIcon;

        [STAThread]
        public static void Main()
        {
            const string SemaphoreName = "YomeNotif";
            bool createdNew;

            // Semaphoreクラスのインスタンス生成・保持
            using (var semaphore
                = new System.Threading.Semaphore(1, 1, SemaphoreName, out createdNew))
            {
                // 既に起動していたら、死んでもらう。
                if (!createdNew)
                {
                    MessageBox.Show("嫁時報アプリは既に起動しています。タスクトレイを確認してみてください。", "嫁時報アプリ",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                App app = new App();
                app.InitializeComponent();
                app.Run();
            }
        }


        // System.Windows.Application.Startupイベントを発生
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            this.notifyIcon = new NotifyIconWrapper();

        }

        // System.Windows.Application.Startupイベントを発生
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            this.notifyIcon.Dispose();
        }
    }
}
