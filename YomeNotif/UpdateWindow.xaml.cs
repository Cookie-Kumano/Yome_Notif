using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace YomeNotif
{
    /// <summary>
    /// UpdateWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class UpdateWindow : Window
    {
        private String[] _strParam = new string[2];
        private string imgfilePath = "";
        private string _defaultName;
        private string[] _data;

        public String[] strParam
        {
            get
            {
                return _strParam;
            }
            set
            {
                _strParam = value;
            }
        }

        public UpdateWindow(string defaultName, string defaultImage, string[] data)
        {
            InitializeComponent();
            _defaultName = defaultName;
            NameInput.Text = defaultName;
            PathView.Content = defaultImage;
            imgfilePath = defaultImage;
            _data = data;
        }

        private void ImgButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.bmp, *.jpg, *.jpeg, *.png, *.gif)|*.bmp;*.jpg;*.jpeg;*.png;*.gif";
            if (dialog.ShowDialog() == true)
            {
                imgfilePath = dialog.FileName;
                PathView.Content = dialog.FileName;
            }

        }

        private void SubmitButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameInput.Text == "")
                MessageBox.Show("名前を入力してください", "追加エラー", MessageBoxButton.OK, MessageBoxImage.Error);
            else
            {
                int n = _data.Length;
                bool NoOverwrapping = true;

                for (int i = 0; i < n; i++)
                {
                    if (_data[i] == NameInput.Text && _data[i] != _defaultName)
                        NoOverwrapping = false;
                }
                if (!NoOverwrapping)
                {
                    MessageBox.Show("名前が重複しています", "追加エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("以下の内容で変更します。よろしいですか？\n " + "名前：　" + _defaultName + " → " + NameInput.Text + "\n 画像：　" + imgfilePath, "確認", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.strParam[0] = NameInput.Text;
                        this.strParam[1] = imgfilePath;
                        this.Close();
                    }
                }
            }
        }
    }
}
