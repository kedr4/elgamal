using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TheoryOfInformation.lab3.Encryption;
using TheoryOfInformation.lab3.Service;

namespace TheoryOfInformation.lab3
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ElGamal _key;

        private bool _encode = false;
        private bool Encode { get => _encode; set { if (fileUnit_in != null) fileUnit_in.Encrypt = value; _encode = value; } }
        public bool Visualisation { get; set; } = true;
        
        public MainWindow()
        {
            InitializeComponent();
            encCheck.IsChecked = true;
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e) => Encode = encCheck.IsChecked.Value;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            MainBTN.IsEnabled = false;
            await Task.Delay(100);
            await EncodeFunction();
            MainBTN.IsEnabled = true;
        }

        private async Task EncodeFunction()
        {
            string path = fileUnit_in.OutputFile.Text;
            byte[] bytesRaw;

            using (FileStream sourceStream = new FileStream(path, FileMode.Open))
            {
                bytesRaw = new byte[sourceStream.Length];
                _ = await sourceStream.ReadAsync(bytesRaw, 0, (int)sourceStream.Length);
            }

            string reportStr;
            if (_encode)
            {
                var resultUint = Resizer.FromFile(bytesRaw, 1);
                var resultTmp = _key.Encrypt(resultUint);
                var result = Resizer.ToFile(resultTmp, _key.Resize);

                string source = string.Join(" ", resultUint.Take(300).Select(x => $"{x:d9} "));
                string resStr = "";
                for (int i = 0; i < resultTmp.Take(600).Count(); i += 2)
                    resStr += $"{resultTmp[i]:d4},{resultTmp[i + 1]:d4}  ";
                reportStr = string.Join("\n", _key.Y, source, resStr);

                File.WriteAllBytes(path + ".data", result);
            }
            else
            {
                var resultUINT = Resizer.FromFile(bytesRaw, _key.Resize);
                var resultTMP = _key.Decrypt(resultUINT);
                var result = Resizer.ToFile(resultTMP, 1);

                string resStr = string.Join(" ", resultTMP.Take(300).Select(x => $"{x:d9} "));
                string source = "";
                for (int i=0; i< resultUINT.Take(600).Count(); i += 2)
                    source += $"{resultUINT[i]:d4},{resultUINT[i + 1]:d4}  ";
                reportStr = string.Join("\n", _key.Y, source, resStr);

                string filename = path.Replace(".data", "");
                filename = filename.Insert(filename.LastIndexOf('\\') + 1, "dec_");
                File.WriteAllBytes(filename, result);
            }

            if (Visualisation)
            {
                ReportWindow report = new ReportWindow();
                report.outputText.Text = reportStr;
                report.Show();
            }
        }

        private void keyBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(IsGood);
        }

        private void OnPasting(object sender, DataObjectPastingEventArgs e)
        {
            var stringData = (string)e.DataObject.GetData(typeof(string));
            if (stringData == null || !stringData.All(IsGood))
                e.CancelCommand();
        }

        bool IsGood(char c) => c > 47 && c < 59;

        private void BuildBTN_Click(object sender, RoutedEventArgs e)
        {
            BuildBTN.IsEnabled = false;
            try
            {
                _key = new ElGamal(uint.Parse(pBox.Text), uint.Parse(xBox.Text), uint.Parse(kBox.Text));
                gBox.ItemsSource = _key.Roots;
                CounterLBL.Content = $"Count of roots: {_key.Roots.Count}";
                gBox.SelectedIndex = 0;
                MainBTN.IsEnabled = true;
            } catch (Exception exp) {
                BuildBTN.IsEnabled = true;
                MessageBox.Show(exp.Message);
            }
        }

        private void ResetAll()
        {
            MainBTN.IsEnabled = false;
            BuildBTN.IsEnabled = true;
            CounterLBL.Content = "Count of roots: 0";
            gBox.ItemsSource = null;
            _key = null;
        }

        private void gBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedIndex<0) return; 
            int s = ((ComboBox)sender).SelectedIndex;
            _key.ChangeG(s);
        }

        private void pBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ResetAll();
        }
    }
}
