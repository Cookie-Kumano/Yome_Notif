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
    /// AddWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class AddWindow : Window
    {
        private String[] _strParam = new string[2];
        private string imgfilePath = "";
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

        public AddWindow(string[] data)
        {
            InitializeComponent();
            strParam[0] = "";
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

                for (int i = 0; i < n; i ++)
                {
                    if (_data[i] == NameInput.Text)
                        NoOverwrapping = false;
                }
                if (!NoOverwrapping)
                {
                    MessageBox.Show("名前が重複しています", "追加エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    this.strParam[0] = NameInput.Text;
                    this.strParam[1] = imgfilePath;
                    this.Close();
                }
            }
        }
    }
}
