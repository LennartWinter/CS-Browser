using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CefSharp;
using CefSharp.Wpf;

namespace Cs_Browser
{
    public partial class MainWindow : Window
    {
        private int tabCounter = 1;

        private void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.T && Keyboard.Modifiers == ModifierKeys.Control)
            {
                AddTabButton_Click(sender, e);
                e.Handled = true;
            }
            else if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control)
            {
                CloseCurrentTab();
                e.Handled = true;
            }
            else if (e.Key == Key.Tab && Keyboard.Modifiers == ModifierKeys.Control)
            {
                SwitchToNextTab();
                e.Handled = true;
            }
        }

        private void SwitchToNextTab()
        {
            int selectedIndex = tabControl.SelectedIndex;

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                int nextIndex = (selectedIndex + 1) % tabControl.Items.Count;
                tabControl.SelectedIndex = nextIndex;
            }
        }



        private void CloseCurrentTab()
        {
            var currentTab = tabControl.SelectedItem as TabItem;

            if (currentTab != null)
            {
                var grid = currentTab.Content as Grid;
                var closeButton = grid?.Children.OfType<Button>().FirstOrDefault();

                if (closeButton != null)
                {
                    int tabIndex = (int)closeButton.Tag;

                    tabControl.Items.RemoveAt(tabIndex);

                    var webBrowser = (currentTab.Content as Grid)?.Children.OfType<ChromiumWebBrowser>().FirstOrDefault();
                    webBrowser?.Dispose();
                    tabCounter--;

                    for (int i = tabIndex; i < tabControl.Items.Count; i++)
                    {
                        var remainingTab = tabControl.Items[i] as TabItem;

                        if (remainingTab != null)
                        {
                            var remainingGrid = remainingTab.Content as Grid;
                            var remainingButton = remainingGrid?.Children.OfType<Button>().FirstOrDefault();

                            if (remainingButton != null)
                            {
                                remainingButton.Tag = i;
                                remainingTab.Header = i == 0 ? "Adam" : $"Tab {i}";
                            }
                        }
                    }
                }
            }
        }

        
        public MainWindow()
        {
            InitializeComponent();
            webView1.FrameLoadStart += WebView_FrameLoadStart;
            webView1.FrameLoadEnd += WebView_FrameLoadEnd;
            webView1.Address = "https://start.duckduckgo.com/";
            PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void WebView_FrameLoadStart(object sender, FrameLoadStartEventArgs e)
        {
            Console.WriteLine($"Frame Load Start: {e.Url}");
        }

        private void WebView_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Console.WriteLine($"Frame Load End: {e.Url}");
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchQuery = searchTextBox.Text;
            if (Uri.TryCreate(searchQuery, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            { 
                GetCurrentWebView().Load(searchQuery);
            }
            else
            {
                string searchUrl = $"http://{searchQuery}";

                GetCurrentWebView().Load(searchUrl);
            }
        }

        private void AddTabButton_Click(object sender, RoutedEventArgs e)
        {
            var newTab = new TabItem();
            var newWebView = new ChromiumWebBrowser();

            newWebView.FrameLoadStart += WebView_FrameLoadStart;
            newWebView.FrameLoadEnd += WebView_FrameLoadEnd;
            newWebView.Address = "https://start.duckduckgo.com/";
            newTab.Content = new Grid();
            (newTab.Content as Grid).Children.Add(newWebView);
            newTab.Header = $"Tab {tabCounter}";
            var closeButton = new Button
            {
                Content = "x",
                Tag = tabCounter,
                Background = Brushes.Transparent,
                BorderBrush = Brushes.Transparent,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top
            };

            closeButton.Click += CloseTabButton_Click;
            (newTab.Content as Grid).Children.Add(closeButton);
            tabControl.Items.Add(newTab);
            tabControl.SelectedItem = newTab;
            tabCounter++;
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchButton_Click(sender, e);
            }
        }


        private void CloseTabButton_Click(object sender, RoutedEventArgs e)
        {
            Button closeButton = sender as Button;

            if (closeButton != null)
            {
                int tabIndex = (int)closeButton.Tag;

                if (tabIndex >= 0 && tabIndex < tabControl.Items.Count)
                {
                    var tabItem = tabControl.Items[tabIndex] as TabItem;

                    tabControl.Items.RemoveAt(tabIndex);

                    var webBrowser = (tabItem.Content as Grid)?.Children.OfType<ChromiumWebBrowser>().FirstOrDefault();
                    webBrowser?.Dispose();
                    tabCounter--;
                    for (int i = tabIndex; i < tabControl.Items.Count; i++)
                    {
                        var remainingTab = tabControl.Items[i] as TabItem;
                        if (remainingTab != null)
                        {
                            var grid = remainingTab.Content as Grid;
                            var remainingButton = grid.Children.OfType<Button>().FirstOrDefault();

                            if (remainingButton != null)
                            {
                                remainingButton.Tag = i;
                                remainingTab.Header = i == 0 ? "Adam" : $"Tab {i}";
                            }
                        }
                    }
                }
            }
        }



        private ChromiumWebBrowser GetCurrentWebView()
        {
            var currentTab = tabControl.SelectedItem as TabItem;
            var grid = currentTab?.Content as Grid;
            return grid?.Children.OfType<ChromiumWebBrowser>().FirstOrDefault();
        }
    }
}