using System.Windows;
using System.Windows.Controls;

namespace JSSoft.ModernUI.Shell.Contents.Views
{
    /// <summary>
    /// TerminalView.xaml�� ���� ��ȣ �ۿ� ��
    /// </summary>
    public partial class TerminalView : UserControl
    {
        public TerminalView()
        {
            InitializeComponent();
        }

        private void Editor_Executed(object sender, RoutedEventArgs e)
        {

        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeAsync(() =>
            {
                //if (this.editor.ApplyTemplate() == true)
                {
                    this.Editor.Focus();
                    this.Editor.AppendLine("�ȳ��ϼ���.");
                    this.Editor.Prompt = "c:> ";
                }
            });
        }
    }
}
