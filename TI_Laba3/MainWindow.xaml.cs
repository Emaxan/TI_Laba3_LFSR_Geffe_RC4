using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;

namespace TI_Laba3
{
    public struct Polynom
    {
        public int Count;
        
        private readonly int[] _degrees;
        public int this[int index]
        {
            get { return _degrees[index]; }
            set { _degrees[index] = value; }
        }

        public Polynom (int count)
        {
            _degrees = new int[count];
            Count = count;
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private SourceText _st = new SourceText();
        private Key _kw = new Key();
        private CryptoText _ct = new CryptoText();
        private readonly int[] _var1 = {23,24,25,26,27,28,29,30}, _var2 = {31,32,33,34,35,36,37,38}, _var3 = {39,40,23,24,25,26,27,28};
        private int _checked;
        private Processor _proc;

        public MainWindow()
        {
            InitializeComponent();
            while (!_contentLoaded){}
            // ReSharper disable once PossibleNullReferenceException
            (FirstLfsr.Children[0] as RadioButton).IsChecked = true;
            // ReSharper disable once PossibleNullReferenceException
            (Count.Children[0] as RadioButton).IsChecked = true;
            RbLfsr.IsChecked = true;
            Top = 500;
            Left = SystemParameters.PrimaryScreenWidth/2;
        } 

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var button = sender as RadioButton;
            if (button == null) return;
            _checked = _var1.ToList().IndexOf(int.Parse((string) button.Content));
            if (Key1Start.Text.Length > _var1[_checked])
                Key1Start.Text = Key1Start.Text.Substring(0, _var1[_checked]);
            if (Key2Start.Text.Length > _var2[_checked])
                Key2Start.Text = Key2Start.Text.Substring(0, _var2[_checked]);
            if (Key3Start.Text.Length > _var3[_checked])
                Key3Start.Text = Key3Start.Text.Substring(0, _var3[_checked]);
            if (Count1.IsChecked == true) return;
            var lfsr2 = new RadioButton {Content = _var2[_checked], IsChecked = true, Background = new SolidColorBrush(Colors.Transparent)};
            SecondLfsr.Children.Clear();
            SecondLfsr.Children.Add(lfsr2);
            lfsr2 = new RadioButton { Content = _var3[_checked], IsChecked = true, Background = new SolidColorBrush(Colors.Transparent) };
            ThirdLfsr.Children.Clear();
            ThirdLfsr.Children.Add(lfsr2);
        }

        private void Count1_Checked(object sender, RoutedEventArgs e)
        {
            SecondLfsr.Children.Clear();
            ThirdLfsr.Children.Clear();
            Second.Visibility = Visibility.Collapsed;
            Third.Visibility = Visibility.Collapsed;
            Key2Start.Visibility = Visibility.Collapsed;
            Key3Start.Visibility = Visibility.Collapsed;
        }

        private void Count3_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var rb in FirstLfsr.Children.Cast<RadioButton>().Where(rb => rb.IsChecked==true))
            {
                rb.IsChecked = false;
                rb.IsChecked = true;
            }
            Second.Visibility = Visibility.Visible;
            Third.Visibility = Visibility.Visible;
            Key2Start.Visibility = Visibility.Visible;
            Key3Start.Visibility = Visibility.Visible;
        }

        private void LFSR_OnChecked(object sender, RoutedEventArgs e)
        {
            Lfsr.Visibility = Visibility.Visible;
            Rc4.Visibility = Visibility.Collapsed;
        }

        private void RC4_OnChecked(object sender, RoutedEventArgs e)
        {
            Rc4.Visibility = Visibility.Visible;
            Lfsr.Visibility = Visibility.Collapsed;
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var block = sender as TextBlock;
            if (block==null) return;
            var ofd = new OpenFileDialog
            {
                InitialDirectory = @"E:\Programs\VS\_TI\TI_Laba3",
                Title = "Open source file"
            };
            if (ofd.ShowDialog() == true)
                block.Text = ofd.FileNames[0];            
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            var box = sender as TextBox;
            if (box == null) return;
            if (box.Text.Length <
                (Equals(box, Key1Start) ? _var1[_checked] : Equals(box, Key2Start) ? _var2[_checked] : _var3[_checked]))
                if (e.Key == System.Windows.Input.Key.NumPad0 || e.Key == System.Windows.Input.Key.NumPad1 ||
                    e.Key == System.Windows.Input.Key.D0 || e.Key == System.Windows.Input.Key.D0)
                    box.Text += e.Key.ToString().Where(char.IsDigit).Aggregate("", (current, c) => current + c);
            if (e.Key == System.Windows.Input.Key.Subtract && box.Text.Length > 0)
                box.Text = box.Text.Remove(box.Text.Length - 1);
        }

        private void MyWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Left -= (e.NewSize.Width - e.PreviousSize.Width)/2;
            Top -= (e.NewSize.Height - e.PreviousSize.Height)/2;
        }

        private void TextBlock_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if(RbFast.IsChecked == false && RbBeauty.IsChecked == false) return;
            Processor.Lfsr = RbLfsr.IsChecked == true;
            if (Processor.Lfsr)
            {
                Processor.LfsrCount = Count1.IsChecked == true ? 1 : 3;
                Processor.LfsrSize[0] = _var1[_checked];
                if (Key1Start.Text.Length < _var1[_checked])
                {
                    MessageBox.Show("Key 1 very small", "Error!", MessageBoxButton.OK, MessageBoxImage.Error,
                        MessageBoxResult.OK, MessageBoxOptions.None);
                    return;
                }
                Processor.Key[0] = Processor.PrepareKey(Key1Start.Text);
                if (Processor.LfsrCount == 3)
                {
                    Processor.LfsrSize[1] = _var2[_checked];
                    if (Key2Start.Text.Length < _var2[_checked])
                    {
                        MessageBox.Show("Key 2 very small", "Error!", MessageBoxButton.OK, MessageBoxImage.Error,
                            MessageBoxResult.OK, MessageBoxOptions.None);
                        return;
                    }
                    Processor.Key[1] = Processor.PrepareKey(Key2Start.Text);
                    Processor.LfsrSize[2] = _var3[_checked];
                    if (Key3Start.Text.Length < _var3[_checked])
                    {
                        MessageBox.Show("Key 3 very small", "Error!", MessageBoxButton.OK, MessageBoxImage.Error,
                            MessageBoxResult.OK, MessageBoxOptions.None);
                        return;
                    }
                    Processor.Key[2] = Processor.PrepareKey(Key3Start.Text);
                }
            }
            else
            {
                Processor.KeyPath = Rc4KeyPath.Text;
                if (!new FileInfo(Processor.KeyPath).Exists)
                {
                    MessageBox.Show("Key file not found!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error,
                        MessageBoxResult.OK, MessageBoxOptions.None);
                    return;
                }
            }
            Processor.FilePath = FilePath.Text;
            if(! new FileInfo(Processor.FilePath).Exists)
            {
                MessageBox.Show("Source file not found!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error,
                    MessageBoxResult.OK, MessageBoxOptions.None);
                return;
            }
            Processor.OutPath = FilePath.Text.Remove(FilePath.Text.Length - 4) + '_' +
                                FilePath.Text.Substring(FilePath.Text.Length - 4, 4);
            if (new FileInfo(Processor.OutPath).Exists)
            {
                var mes =
                    MessageBox.Show(
                        string.Format("Out file \"{0}\" exist! \nAre you want to rewrite it?", Processor.OutPath),
                        "Warning!", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.Yes,
                        MessageBoxOptions.None);
                if (mes == MessageBoxResult.No)
                    return;
            }
            {
                if(!_st.IsLoaded)
                    _st = new SourceText();
                _st.Owner = this;
                _st.Top = Top + _st.Height < SystemParameters.FullPrimaryScreenHeight
                    ? Top
                    : Top - (Top + _st.Height - SystemParameters.FullPrimaryScreenHeight);
                _st.ShowInTaskbar = false;
                _st.Left = Left - _st.Width;
                _st.TbText.Text = string.Empty;
                _st.RbText.IsChecked = true;
                _st.Show();
            }
            {
                if(!_kw.IsLoaded)
                    _kw = new Key();
                _kw.Owner = this;
                _kw.Top = Top - _kw.Height;
                _kw.ShowInTaskbar = false;
                _kw.Left = (Left + Width/2) - _kw.Width/2;
                _kw.TbText.Text = string.Empty;
                _kw.RbText.IsChecked = true;
                _kw.Show();
            }
            {
                if (!_ct.IsLoaded)
                    _ct = new CryptoText();
                _ct.Owner = this;
                _ct.Top = Top + _ct.Height < SystemParameters.FullPrimaryScreenHeight
                    ? Top
                    : Top - (Top + _ct.Height - SystemParameters.FullPrimaryScreenHeight);
                _ct.ShowInTaskbar = false;
                _ct.Left = Left + Width;
                _ct.TbText.Text = string.Empty;
                _ct.RbText.IsChecked = true;
                _ct.Show();
            }
            if (_proc == null)
            {
                _proc = new Processor();
                _proc.WokrComplete += _st.Copy;
                _proc.WokrComplete += _kw.Copy;
                _proc.WokrComplete += _ct.Copy;
                _proc.Progr = TbProgress;
            }
            _proc.Work();
        }

        private void TbTitle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Fast_OnChecked(object sender, RoutedEventArgs e)
        {
            Processor.Fast = Equals(e.OriginalSource, RbFast);
        }
    }
}
