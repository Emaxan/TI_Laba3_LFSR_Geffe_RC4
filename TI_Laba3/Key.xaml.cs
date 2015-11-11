using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TI_Laba3
{
    /// <summary>
    /// Логика взаимодействия для Key.xaml
    /// </summary>
    public partial class Key
    {
        public bool First = true;

        public Key()
        {
            InitializeComponent();
            RbText.IsChecked = true;
        }

        private void RbText_OnChecked(object sender, RoutedEventArgs e)
        {
            if (First)
            {
                First = false;
                return;
            }
            TbText.Text =
                Processor.ToText(
                    TbText.Text.Where(c => c == '1' || c == '0').Aggregate("", (current, c) => current + c), true);
        }

        private void RbBinary_OnChecked(object sender, RoutedEventArgs e)
        {
            TbText.Text = Processor.ToBinary(TbText.Text, true);
        }
         
        public void Copy(int status, string source, string key, string crypto, long?[] keys, bool end, bool fast)
        {
            Dispatcher.Invoke(new ThreadStart(delegate
            {
                TbText.Text += key;
                if(end)
                    RbText.IsChecked = true;
                TbText.PageDown();
                if (end)
                if (status == 2)
                {
                    string key1 = "", key2 = "", key3 = "";
                    for (var i = 0; i < 64; i++)
                    {
                        var l = (((ulong)keys[0] & (1Lu << (63 - i))) >> (63 - i)) + '0';
                        var l1 = (((ulong)keys[1] & (1Lu << (63 - i))) >> (63 - i)) + '0';
                        var l2 = (((ulong)keys[2] & (1Lu << (63 - i))) >> (63 - i)) + '0';
                        if (l != null)
                            key1 += (char) l;
                        if (l1 != null)
                            key2 += (char) l1;
                        if (l2 != null)
                            key3 += (char) l2;
                        if ((i+1)%8 == 0 && i!=0)
                        {
                            key1 += ' ';
                            key2 += ' ';
                            key3 += ' ';
                        }
                    }
                    MessageBox.Show("LFSR1: " + key1 + ";\n" + "LFSR2: " + key2 + ";\n" + "LFSR3: " + key3 + ";",
                        "Generated sequences.", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK,
                        MessageBoxOptions.None);
                }
            }), DispatcherPriority.Render);
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }
    }
}
