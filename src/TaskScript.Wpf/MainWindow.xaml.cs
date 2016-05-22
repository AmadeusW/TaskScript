using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TaskScript.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyUser
    {
        private ScriptRunner _runner;
        private string _params;

        public String Output
        {
            set
            {
                Dispatcher.Invoke(() =>
                {
                    if (!String.IsNullOrWhiteSpace(value)
                        && value.Trim() != ">") {
                        Result.Text = value;
                    }
                });
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            _runner = new ScriptRunner(this);
            Settings.Initialize();

            bool hasScripts = false;
            foreach (dynamic script in Settings.Current["scripts"])
            {
                var button = new ToggleButton()
                {
                    Content = script.label.ToString(),
                    Tag = script.path.ToString()
                };
                button.Click += (o, e) =>
                {
                    button.IsChecked = true;
                    button.Background = new SolidColorBrush(SystemColors.ControlColor);

                    string args = String.Empty;
                    if (Params.Visibility == Visibility.Visible
                    && !String.IsNullOrWhiteSpace(Params.Text))
                    {
                        args = Params.Text;
                    }
                    else
                    {
                        args = script.args.ToString();
                    }

                    _runner.RunScript(script.path.ToString(), args);

                    // display/save
                    if (String.IsNullOrWhiteSpace(Params.Text))
                    {
                        Params.Text = args;
                    }
                    script.args = args;
                };
                Scripts.Children.Add(button);
                hasScripts = true;
            }

            if (!hasScripts)
            {
                ExpandParamsButton.Visibility = Visibility.Collapsed;
            }
        }

        private void Params_Click(object sender, RoutedEventArgs e)
        {
            Params.Visibility = Visibility.Visible;
        }

        private void Pin_Click(object sender, RoutedEventArgs e)
        {
            this.Topmost = AlwaysOnTopButton.IsChecked == true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _runner.Stop();
        }

        private void Params_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _params = Params.Text;
                Params.Visibility = Visibility.Collapsed;
                Settings.Save();
            }
            if (e.Key == Key.Escape)
            {
                Params.Visibility = Visibility.Collapsed;
            }
        }

        public void NotifyOfError(string path)
        {
            Dispatcher.Invoke(() =>
            {
                var button = getButtonForScript(path);
                if (button != null)
                {
                    button.Background = new SolidColorBrush(Colors.Red);
                    button.IsChecked = false;
                }
            });
        }

        public void NotifyOfSuccess(string path)
        {
            Dispatcher.Invoke(() =>
            {
                var button = getButtonForScript(path);
                if (button != null)
                {
                    button.Background = new SolidColorBrush(SystemColors.ControlColor);
                    button.IsChecked = false;
                }
            });
        }

        private ToggleButton getButtonForScript(string path)
        {
            foreach (var childElement in Scripts.Children)
            {
                var button = childElement as ToggleButton;
                if (button == null)
                    continue;

                if (path.Equals(button.Tag))
                    return button;
            }
            return null;
        }
    }
}
