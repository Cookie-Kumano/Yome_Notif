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
