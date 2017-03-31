using System;
using System.Windows.Media;

namespace UnturnedBot.Discord.Utils
{
    static class Logger
    {
        public static void Log(string message, SolidColorBrush bracketsColor = null)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.AppendText(message, bracketsColor);
            }));
        }
        public static void Log(string message, params object[] args)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MainWindow.AppendText(string.Format(message, args), null);
            }));
        }
    }
}
