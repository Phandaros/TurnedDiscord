using Discord;
using MahApps.Metro.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using UnturnedBot.Discord.Discord;
using UnturnedBot.Discord.Discord.Contexts;
using UnturnedBot.Discord.Utils;

namespace UnturnedBot.Discord
{
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow instance = null;
        public static Timer shutdownTimer = null;
        public MainWindow()
        {
            instance = this;
            InitializeComponent();

            SetDefaults();
            Program.Start();
        }

        private void SetDefaults()
        {
            UpdateServerPathTextBlock(Settings.GetValue("pathToServer") ?? "");
            UpdateDllPathTextBlock(Settings.GetValue("pathToDll") ?? "");
            HourToShutdownTextBox.Text = (Settings.GetValue("hourToShutdown") ?? "") + ':' + (Settings.GetValue("minuteToShutdown") ?? "");
            ServerNameTextBox.Text = Settings.GetValue("serverName") ?? "";
        }
        public static void UpdateBotStatus()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var loginState = DiscordBot.client.LoginState;
                bool isReady = loginState == LoginState.LoggedIn || loginState == LoginState.LoggingIn;
                instance.DiscordStatusLabel.Content = loginState.ToString();
                instance.DiscordStatusLabel.Foreground = isReady ? Brushes.Green : Brushes.Red;
            }));
        }
        public static void UpdateServerPathTextBlock(string value)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var textBlock = instance.ServerPathTextBlock;
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(new Run("Server Path: ") { Foreground = Brushes.Red });
                textBlock.Inlines.Add(new Run(value));
            }));
        }
        public static void UpdateDllPathTextBlock(string value)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var textBlock = instance.DllPathLabel;
                textBlock.Inlines.Clear();
                textBlock.Inlines.Add(new Run("Dll Path: ") { Foreground = Brushes.Red });
                textBlock.Inlines.Add(new Run(value));
            }));
        }

        public static void AppendText(string text, SolidColorBrush bracketsColor = null)
        {
            bracketsColor = bracketsColor ?? Brushes.Red;
            var textBox = instance.ConsoleTextBox;

            var date = DateTime.Now.ToString("HH:mm:ss");
            AppendColouredText("[" + date + "] ", Brushes.Red);
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '[')
                {
                    int endIndex = text.IndexOf(']', i);
                    if (endIndex != -1)
                    {
                        var subString = text.Substring(i, endIndex - i + 1);
                        AppendColouredText(subString, bracketsColor);

                        i += subString.Length - 1;
                        continue;
                    }
                }

                AppendColouredText(c, Brushes.Black);
            }
            textBox.AppendText("\r");
            textBox.ScrollToEnd();
        }
        public static void AppendColouredText(char c, SolidColorBrush color)
        {
            var textBox = instance.ConsoleTextBox;
            TextRange rangeOfText = new TextRange(textBox.Document.ContentEnd, textBox.Document.ContentEnd)
            {
                Text = c.ToString()
            };
            rangeOfText.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }
        public static void AppendColouredText(string text, SolidColorBrush color)
        {
            var textBox = instance.ConsoleTextBox;
            TextRange rangeOfText = new TextRange(textBox.Document.ContentEnd, textBox.Document.ContentEnd)
            {
                Text = text
            };
            rangeOfText.ApplyPropertyValue(TextElement.ForegroundProperty, color);
        }

        private async void ConsoleInTextbox_KeyDownAsync(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(ConsoleInTextBox.Text))
            {
                var command = ConsoleInTextBox.Text;
                ConsoleInTextBox.Text = string.Empty;
                if (command.Equals("clear", StringComparison.CurrentCultureIgnoreCase))
                {
                    ConsoleTextBox.Document.Blocks.Clear();
                }
                else
                {
                    AppendText("In: " + command);
                    await DiscordBot.handler.HandleCommandAsync(new CustomCommandContext(), command);
                }
            }
        }

        private void StartServerButtonClick(object sender, RoutedEventArgs e)
        {
            var path = Settings.GetValue("pathToServer");

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                var result = MessageBox.Show("O caminho para o server não existe, quer selecionar um novo?", "Caminho não existe", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (ShowSelectFolderDialogAndUpdateConfig("pathToServer") == CommonFileDialogResult.Ok)
                        StartServerButtonClick(sender, e);
                }
                return;
            }
            var pathToUnturnedExe = path + @"\Unturned.exe";
            if (!File.Exists(pathToUnturnedExe))
            {
                var result = MessageBox.Show("O Unturned não foi encontrado no caminho especificado, quer selecionar um novo?", "Caminho não encontrado", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (ShowSelectFolderDialogAndUpdateConfig("pathToServer") == CommonFileDialogResult.Ok)
                        StartServerButtonClick(sender, e);
                }
                return;
            }

            var serverName = ServerNameTextBox.Text;
            Logger.Log("Starting Unturned server " + serverName);
            var startInfo = new ProcessStartInfo(pathToUnturnedExe, " -nographics -batchmode -silent-crashes +secureserver/" + serverName)
            {
                WorkingDirectory = path
            };
            var proc = new Process() { StartInfo = startInfo };
            proc.EnableRaisingEvents = true;

            proc.Start();
            //proc.Exited += delegate (object o, EventArgs evArgs)
            //{
            //    Logger.Log("UnturnedServer has been closed!!");
            //};
        }

        private void SelectServerFolderButtonClick(object sender, RoutedEventArgs e) => ShowSelectFolderDialogAndUpdateConfig("pathToServer");

        private void SelectDllFolderButtonClick(object sender, RoutedEventArgs e) => ShowSelectFolderDialogAndUpdateConfig("pathToDll");

        private void CopyDllToServerButtonClick(object sender, RoutedEventArgs e)
        {
            var path = Settings.GetValue("pathToDll");

            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                var result = MessageBox.Show("O caminho para a dll não existe, quer selecionar um novo?", "Caminho não existe", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (ShowSelectFolderDialogAndUpdateConfig("pathToDll") == CommonFileDialogResult.Ok)
                        CopyDllToServerButtonClick(sender, e);
                }
                return;
            }

            var pathToDll = path + @"\UnturnedBot.Unturned.dll";
            if (!File.Exists(pathToDll))
            {
                var result = MessageBox.Show("UnturnedBot.Unturned.dll não foi encontrado no caminho especificado, quer selecionar um novo?", "Caminho não encontrado", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (ShowSelectFolderDialogAndUpdateConfig("pathToDll") == CommonFileDialogResult.Ok)
                        CopyDllToServerButtonClick(sender, e);
                }
                return;
            }

            var serverFolder = Settings.GetValue("pathToServer");
            if (string.IsNullOrEmpty(serverFolder))
            {
                var result = MessageBox.Show("O caminho para o server não foi especificado, quer selecionar um?", "Caminho não especificado", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    if (ShowSelectFolderDialogAndUpdateConfig("pathToServer") == CommonFileDialogResult.Ok)
                        CopyDllToServerButtonClick(sender, e);
                }
                return;
            }

            var pluginsFolder = System.IO.Path.Combine(serverFolder, "Servers", ServerNameTextBox.Text, "Rocket\\Plugins\\UnturnedBot.Unturned.dll");

            if (File.Exists(pluginsFolder))
                File.Delete(pluginsFolder);

            File.Move(pathToDll, pluginsFolder);
        }

        private CommonFileDialogResult ShowSelectFolderDialogAndUpdateConfig(string configName)
        {
            var dialog = new CommonOpenFileDialog()
            {
                IsFolderPicker = true,
                EnsurePathExists = true,
                Title = "Selecione a pasta"
            };
            var dialogResult = dialog.ShowDialog();
            if (dialogResult == CommonFileDialogResult.Ok)
            {
                Settings.AddOrUpdate(configName, dialog.FileName);
                if (configName == "pathToServer")
                    UpdateServerPathTextBlock(dialog.FileName);
                else if (configName == "pathToDll")
                    UpdateDllPathTextBlock(dialog.FileName);

            }

            return dialogResult;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (HourToShutdownTextBox == null) return;
            HourToShutdownTextBox.IsEnabled = true;
        }
        private void CheckBox_UnChecked(object sender, RoutedEventArgs e)
        {
            if (HourToShutdownTextBox == null) return;
            HourToShutdownTextBox.IsEnabled = false;
        }

        private void HourToShutdownTextBox_ParseText(object sender, TextCompositionEventArgs e)
        {
            var text = HourToShutdownTextBox.Text;
            if ((!int.TryParse(e.Text, out int newNumber) && e.Text[0] != ':') || text.Replace(":", string.Empty).Length + e.Text.Replace(":", string.Empty).Length > 4)
                e.Handled = true;
        }

        private void HourToShutdownTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!HourToShutdownTextBox.IsEnabled) return;

            var split = HourToShutdownTextBox.Text.Split(':');
            if (HourToShutdownTextBox.Text.Length != 5 || split.Length != 2 || !int.TryParse(split[0], out int hours) || !int.TryParse(split[1], out int minutes))
                return;


            Settings.AddOrUpdate("hourToShutdown", split[0]);
            Settings.AddOrUpdate("minuteToShutdown", split[1]);

            SetUpTimer(new TimeSpan(hours, minutes, 00));
        }
        private void SetUpTimer(TimeSpan alertTime)
        {
            if (shutdownTimer != null)
            {
                shutdownTimer.Dispose();
                shutdownTimer = null;
            }

            DateTime current = DateTime.Now;
            TimeSpan timeToGo = alertTime - current.TimeOfDay;
            Logger.Log("Timer set up at " + alertTime.ToString());
            if (timeToGo < TimeSpan.Zero)
            {
                Logger.Log("Time already passed, setting to tomorrow...");
                timeToGo = alertTime.Add(TimeSpan.FromDays(1.0) - current.TimeOfDay);
                //time already passed
            }
            shutdownTimer = new Timer(x =>
            {
                Logger.Log("Shutting down...");
                var psi = new ProcessStartInfo("shutdown", "/s /t 0")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                Process.Start(psi);
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        private void ServerNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textbox = sender as TextBox;

            Settings.AddOrUpdate("serverName", textbox.Text);
        }

        private void HourToShutdownTextBox_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var value = e.NewValue as bool?;
            if (!value.Value)
            {
                if (shutdownTimer != null)
                {
                    Logger.Log("Timer disabled");
                    shutdownTimer.Dispose();
                    shutdownTimer = null;
                }
            }
            else
            {
                HourToShutdownTextBox_TextChanged(sender, null);
            }
        }
    }
}
