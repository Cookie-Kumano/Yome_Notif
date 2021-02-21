using System;
using System.Reflection;
using System.Windows;

namespace YomeNotif
{
    /// <summary>
    /// InfoWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class InfoWindow : Window
    {
        public InfoWindow()
        {
            InitializeComponent();

            Assembly assembly = Assembly.GetExecutingAssembly();
            AssemblyName asmName = assembly.GetName();
            Version version = asmName.Version;

            var asmTitle = (AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly,
                                                                typeof(AssemblyTitleAttribute));
            var asmCopyright = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(assembly,
                                                                typeof(AssemblyCopyrightAttribute));
            TitleView.Text = asmTitle.Title.ToString();
            Version.Text = "Version: "+ version.ToString();
            Developer.Text = asmCopyright.Copyright.ToString();

        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}