using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace TheoryOfInformation.lab3
{
    /// <summary>
    /// Логика взаимодействия для FileWindow.xaml
    /// </summary>
    public partial class FileWindow : UserControl
    {
        public bool Encrypt = true;
        public FileWindow()
        {
            InitializeComponent();
        }

        private void InputFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Filter = Encrypt ? "All files (*.*)|*.*" : "Encrypted files (*.data)|*.data|All files (*.*)|*.*",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    ((Button)sender).Tag = filename;
                }
            }
        }
    }
}
