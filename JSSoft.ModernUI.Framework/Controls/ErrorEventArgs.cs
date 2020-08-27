using System.Windows;

namespace JSSoft.ModernUI.Framework.Controls
{
    public class ErrorEventArgs : RoutedEventArgs
    {
        public ErrorEventArgs(RoutedEvent routedEvent, object errorContent)
            : base(routedEvent)
        {
            this.ErrorContent = errorContent;
        }

        public object ErrorContent { get; }
    }
}
