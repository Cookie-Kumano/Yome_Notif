using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Linq;

namespace YomeNotif
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<Dto> _dtos { get; set; } = new ObservableCollection<Dto>();
        String[] _zikan = new string[24];
        DatabaseWrapper objDB = new DatabaseWrapper();

        string[,] Zihodata = new string[24, 2];
        Dto DtoData = null;
        int selectedTime;


        public MainWindow()
        {
            InitializeComponent();
            ReadDatabase();
            Yome_List.SelectionMode = SelectionMode.Single;

            for (int i=0; i<24; i++)
            {
                _zikan[i] = i.ToString() + "時";
            }
            Ziho_List.ItemsSource = _zikan;
        }

        // 嫁リスト追加ボタンクリック時
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // 名前重複防止のために、現在のリストを渡してウィンドウ起動
            var f = new AddWindow(objDB.CheckYomeList());
            f.ShowDialog();
            // 名前がちゃんと登録されている場合のみ保存
            if (f.strParam[0] != "" && f.strParam[0] != null)
            {
                _dtos.Add(new Dto(f.strParam[1], f.strParam[0]));
                objDB.WriteYomeList(f.strParam[0], f.strParam[1]);
            }
        }

        // 嫁リスト削除ボタンクリック時
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            // 選択中の娘を取得
            var data = (Dto)Yome_List.SelectedItem;

            // 削除対象が選択されているか確認
            if (Yome_List.SelectedItem == null)
            {
                MessageBox.Show("削除する対象を選択してください", "削除エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                // 本当に消していいか確認
                MessageBoxResult result = MessageBox.Show("本当に " + data.Name + " を削除しますか？\nすべての設定内容が失われます。", "確認", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    // 消す
                    _dtos.Remove(data);
                    objDB.DeleteYomeList(data.Name);
                }
            }
        }

        // 嫁リスト変更ボタンクリック時
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // 変更対象が選択されているか確認
            if (Yome_List.SelectedItem == null)
            {
                MessageBox.Show("変更する対象を選択してください", "削除エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                // データ照会ができるように、変更前の時点での名前を取得、必要なデータを渡してウィンドウ起動
                var defaultData = (Dto)Yome_List.SelectedItem;
                var f = new UpdateWindow(defaultData.Name, defaultData.FileName, objDB.CheckYomeList());
                f.ShowDialog();
                // 削除・追加の処理を行うので、変更前の位置を取っておく
                int id = objDB.GetYomeID(defaultData.Name)-1;
                if (f.strParam[0] != "" && f.strParam[0] != null)
                {
                    // 削除・割り込み追加処理とデータベース編集処理。
                    _dtos.Remove(defaultData);
                    _dtos.Insert(id, new Dto(f.strParam[1], f.strParam[0]));
                    objDB.UpdateYomeList(defaultData.Name, f.strParam[0], f.strParam[1]);
                }
            }
        }

        // 音声ファイル選択ボタンクリック時
        private void VoiceEditButton_Click(object sender, RoutedEventArgs e)
        {
            string voicefilePath = "";
            var dialog = new OpenFileDialog();
            dialog.Filter = "Audio Files (*.mp3, *.wav, *.wma)|*.mp3;*.wav;*.wma";
            if(dialog.ShowDialog() == true)
            {
                voicefilePath = dialog.FileName;
                VoiceLink.Text = voicefilePath;
                if (!SubmitNotifList("Voices", voicefilePath))
                    VoiceLink.Text = Zihodata[selectedTime, 0];
            }
            else
                ZihoSetSucceed.Visibility = Visibility.Hidden;
        }

        // テキスト変更ボタンクリック時
        private void TextEditButton_Click(object sender, RoutedEventArgs e)
        {
            string setText = TextEditBox.Text;
            if (!SubmitNotifList("Text", setText))
                TextEditBox.Text = Zihodata[selectedTime, 1];
        }

        // 音声・テキストの保存処理
        private bool SubmitNotifList(string dataType, string data)
        {
            if (DtoData != null && Ziho_List.SelectedIndex > -1)
            {
                string msgstring = "";
                if (dataType == "Voices")
                    msgstring = "音声ファイル";
                else
                    msgstring = "テキストデータ";
                MessageBoxResult result = MessageBox.Show("以下の内容が変更されます\n 「 " + DtoData.Name + " 」の "
                                                                + Ziho_List.SelectedIndex + " 時の" + msgstring
                                                                + "\n内容: " + data, "確認", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    if (dataType == "Voices")
                        Zihodata[Ziho_List.SelectedIndex, 0] = data;
                    else
                        Zihodata[Ziho_List.SelectedIndex, 1] = data;
                    if (objDB.SetYomeList(DtoData.Name, Zihodata))
                    {
                        ZihoSetSucceed.Text = msgstring + "の変更を保存しました";
                        ZihoSetSucceed.Visibility = Visibility.Visible;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("登録に失敗しました。\nもう一度試して下さい。", "失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                        return false;
                    }
                }
                else
                {
                    ZihoSetSucceed.Visibility = Visibility.Hidden;
                    return false;
                }
            }
            else
                return false;
        }

        // 嫁リストの選択対象が変わったときの処理
        private void Yome_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DtoData = (Dto)Yome_List.SelectedItem;
            if ((DtoData != null))
            {
                SettingsTitle.Text = "「" + DtoData.Name + "」の時報設定";
                Ziho_List.IsEnabled = true;
                Zihodata = new string[24, 2];
                Array.Copy(objDB.GetNotifList(DtoData.Name), Zihodata, Zihodata.Length);

                if (Ziho_List.SelectedIndex > -1)
                {
                    ZihoSetTitle.Text = Ziho_List.SelectedIndex + "時の時報設定";
                    VoiceLink.Text = Zihodata[Ziho_List.SelectedIndex, 0];
                    VoiceLink.ToolTip = Zihodata[selectedTime, 0];
                    VoiceEditButton.IsEnabled = true;
                    TextEditBox.Text = Zihodata[Ziho_List.SelectedIndex, 1];
                    TextEditBox.IsEnabled = true;
                    TextEditButton.IsEnabled = true;
                }
                else
                    ZihoSetTitle.Text = "時報設定する時刻を選択してください";
            }
            else
            {
                SettingsTitle.Text = "時報設定するキャラクターを選択してください";
                Ziho_List.IsEnabled = false;
                VoiceEditButton.IsEnabled = false;
                TextEditBox.IsEnabled = false;
                TextEditButton.IsEnabled = false;
            }
            ZihoSetSucceed.Visibility = Visibility.Hidden;
        }

        // 時報設定の選択時間が変わったときの処理。
        private void Ziho_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedTime = Ziho_List.SelectedIndex;
            if (selectedTime > -1)
            {
                ZihoSetTitle.Text = selectedTime+"時の時報設定";
                VoiceLink.Text = Zihodata[selectedTime, 0];
                VoiceLink.ToolTip = Zihodata[selectedTime, 0];
                VoiceEditButton.IsEnabled = true;
                TextEditBox.Text = Zihodata[selectedTime, 1];
                TextEditBox.IsEnabled = true;
                TextEditButton.IsEnabled = true;
            }
            else
            {
                SettingsTitle.Text = "時報設定する艦娘を選択してください";
                Ziho_List.IsEnabled = false;
                VoiceEditButton.IsEnabled = false;
                TextEditBox.IsEnabled = false;
                TextEditButton.IsEnabled = false;
            }
            ZihoSetSucceed.Visibility = Visibility.Hidden;
        }

        // データベース照会処理。
        private void ReadDatabase()
        {
            if (!objDB.isExistsYomeList())
            {
                // 初回起動と思われるのでチュートリアルを表示
            } else
            {
                string[,] data = objDB.GetYomeList();

                for (int i = 0; i < data.GetLength(0); i++)
                {
                    _dtos.Add(new Dto(data[i, 1], data[i, 0]));
                }
                Yome_List.ItemsSource = _dtos;
            }
        }

        // 設定ボタンクリック時
        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var f = new SettingsWindow();
            f.Show();
        }

        // アプリ情報ボタンクリック時
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var f = new InfoWindow();
            f.Show();
        }

        // 嫁リスト用の
        public sealed class Dto
        {
            public Dto(string fileName, string name)
            {
                FileName = fileName;
                Name = name;
            }

            public string FileName { get; set; }
            public string Name { get; set; }
        }
    }
}
