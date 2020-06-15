using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Xml.Linq;

namespace YomeNotif
{
    public partial class NotifyIconWrapper : Component
    {

        private Timer timer;
        private int hour = 0;

        string _image;
        string _name;
        string _text;

        public NotifyIconWrapper()
        {
            // 初期化
            InitializeComponent();

            // コンテキストメニューのイベントを設定
            this.toolStripMenuItem_Open.Click += this.toolStripMenuItem_Open_Click;
            this.toolStripMenuItem_Exit.Click += this.toolStripMenuItem_Exit_Click;

            if (Properties.Settings.Default.Ziho)
            {
                /**
                var type = ToastTemplateType.ToastText01;
                var content = ToastNotificationManager.GetTemplateContent(type);
                var text = content.GetElementsByTagName("text").First();
                text.AppendChild(content.CreateTextNode("Toast"));
                var notifier = ToastNotificationManager.CreateToastNotifier("Microsoft.Windows.Computer");
                notifier.Show(new ToastNotification(content));
                **/
                notifyIcon1.BalloonTipTitle = "時報は有効です";
                notifyIcon1.BalloonTipText = "毎時間、艦娘が時間をお知らせします。";
                notifyIcon1.ShowBalloonTip(3000);
            }

            TimerWrapper();
        }

        // タイマーの設定
        private void TimerWrapper()
        {
            // 現在時刻を取得、次のn時0分0秒までの差分を出す
            DateTime dt = DateTime.Now;
            DateTime dt2 = dt;
            dt2 = dt2.AddHours(1);
            dt2 = dt2.AddMinutes(dt.Minute*(-1));
            dt2 = dt2.AddSeconds(dt.Second*(-1));

            // "次のn時"を取ることで、通知時に使用するデータを貰っておく
            hour = dt2.Hour;

            // 差分計算
            TimeSpan ts = dt2 - dt;

            // タイマ設定
            timer = new Timer();
            timer.AutoReset = false;
            timer.Interval = ts.TotalMilliseconds;
            // timer.Interval = 6000;
            timer.Elapsed += Timer_Elapsed;
            timer.Enabled = true;

        }

        // 時間が来たら実行する処理
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // まずタイマを止める
            timer.Close();
            var directorypath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            // 時報が有効で、xmlファイルが存在したら実行
            if (Properties.Settings.Default.Ziho && File.Exists(directorypath + @"\YomeNotif\YomeDB.xml")) {

                // xmlアクセス
                XDocument xdoc = XDocument.Load(directorypath + @"\YomeNotif\YomeDB.xml");
                XElement xele = xdoc.Element("YomeDB");
                IEnumerable<XElement> yome = xele.Elements("yome");

                // 艦娘が複数名登録されているときのためにランダムで時報を出す娘を決める
                Random rand = new Random();
                int randnum = rand.Next(0, yome.Count());

                // 時報を出す娘を確定
                string name = yome.Elements("Name").ToArray()[randnum].Value;
                XElement ship = (from item in yome
                                 where item.Element("Name").Value == name
                                 select item).Single();
                // 必要な情報を取る
                string Image = ship.Element("Image").Value;
                string Voice = ship.Element("Voices").Value;
                string Text = ship.Element("Text").Value;

                // 配列化
                string[] Voices = Voice.Split(',');
                string[] Texts = Text.Split(',');
                string TipText;

                // テキストが空だとエラー出るので
                if (Texts[hour] == "")
                    TipText = "テキストを設定して下さい";
                else
                    TipText = Texts[hour];

                // バルーン通知の設定
                // notifyIcon1.BalloonTipTitle = name;
                // notifyIcon1.BalloonTipText = TipText;
                // notifyIcon1.ShowBalloonTip(20);

                _image = Image;
                _name = name;
                _text = TipText;
                string[] data = { Image, name, TipText };

                Application.Current.Dispatcher.Invoke(
                    new Action<string[]>(NotifFire), (object)data
                );

                // 音声再生が有効で、且つ音声ファイルが存在する場合のみ実行
                if (Properties.Settings.Default.Voice && File.Exists(Voices[hour]))
                {
                    Microsoft.SmallBasic.Library.Sound.PlayChime();
                    Microsoft.SmallBasic.Library.Sound.PlayAndWait(Voices[hour]);
                }
            }
            TimerWrapper();
        }


        private void NotifFire(string[] data)
        {
            NotifWindow window = Application.Current.Windows.OfType<NotifWindow>().FirstOrDefault();
            window = new NotifWindow(data);
            window.Show();
        }

        public NotifyIconWrapper(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

        // "表示"選択時に呼び出し
        private void toolStripMenuItem_Open_Click(object sender, EventArgs e)
        {
            showWindow();
        }

        // "終了"選択時に呼び出し
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            // アプリ終了
            Application.Current.Shutdown();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            showWindow();
        }

        private void showWindow()
        {
            var window = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();
            if (window == null)
            {
                window = new MainWindow();
                window.Show();
            }
            else
                window.Activate();
        }
    }
}
