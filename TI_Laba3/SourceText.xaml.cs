using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace TI_Laba3
{
    /// <summary>
    /// Логика взаимодействия для SourceText.xaml
    /// </summary>
    public partial class SourceText
    {
        public bool First = true;

        public SourceText()
        {
            InitializeComponent();
            RbText.IsChecked = true;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        public void Copy(int status, string source, string key, string crypto, long?[] keys, bool end, bool fast)
        {
            Dispatcher.Invoke(new ThreadStart(delegate
            {
                if (!end||fast)
                    TbText.Text += source;
                if (end)
                    RbText.IsChecked = true;
                TbText.PageDown();
            }),DispatcherPriority.Render);

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
                    TbText.Text.Where(c => c == '1' || c == '0').Aggregate("", (current, c) => current + c), false);
        }

        private void RbBinary_OnChecked(object sender, RoutedEventArgs e)
        {
            TbText.Text = Processor.ToBinary(TbText.Text, false);
        }
    }
}
