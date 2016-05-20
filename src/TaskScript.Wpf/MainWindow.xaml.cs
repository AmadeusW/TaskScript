using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    public partial class MainWindow : Window
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

            foreach (dynamic script in Settings.Current["scripts"])
            {
                var button = new Button()
                {
                    Content = script.label.ToString(),
                    Tag = script.label.ToString()
                };
                button.Click += (o, e) =>
                {
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
            }
        }

        private void Params_Click(object sender, RoutedEventArgs e)
        {
            Params.Visibility = Visibility.Visible;
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

    }
}
