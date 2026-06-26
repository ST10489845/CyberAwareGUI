using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace CyberAware
{
    public partial class MainWindow : Window
    {
        private ChatbotEngine? chatbot;
        private DispatcherTimer? typingTimer;
        private bool waitingForName = true;

        public MainWindow()
        {
            InitializeComponent();
            InitializeChatbot();
            InitializeTypingTimer();
            Loaded += (s, e) => UserInputBox.Focus();
        }

        private void InitializeChatbot()
        {
            chatbot = new ChatbotEngine();
            if (chatbot != null)
            {
                chatbot.OnResponse += Chatbot_OnResponse;
                chatbot.OnUserInfoUpdated += Chatbot_OnUserInfoUpdated;
                chatbot.OnSentimentDetected += Chatbot_OnSentimentDetected;
                chatbot.OnActivityLogged += Chatbot_OnActivityLogged;
            }
            AddBotMessage("Welcome to CyberAware! I am your cybersecurity assistant.\n\nWhat is your name?");
        }

        private void InitializeTypingTimer()
        {
            typingTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1500) };
            if (typingTimer != null)
            {
                typingTimer.Tick += (s, e) =>
                {
                    typingTimer.Stop();
                    TypingIndicator.Visibility = Visibility.Collapsed;
                };
            }
        }

        private void Chatbot_OnResponse(string response)
        {
            Dispatcher.Invoke(() =>
            {
                typingTimer?.Stop();
                TypingIndicator.Visibility = Visibility.Collapsed;
                AddBotMessage(response);
                ScrollToBottom();
            });
        }

        private void Chatbot_OnUserInfoUpdated(string userName, string interest)
        {
            Dispatcher.Invoke(() =>
            {
                UserNameDisplay.Text = string.IsNullOrEmpty(userName) ? "Not set" : userName;
                UserInterestDisplay.Text = string.IsNullOrEmpty(interest) ? "None" : interest;

                if (!string.IsNullOrEmpty(userName) && waitingForName)
                {
                    waitingForName = false;
                    AddBotMessage($"Nice to meet you, {userName}!\n\nWhat would you like to learn about cybersecurity?");
                }
            });
        }

        private void Chatbot_OnSentimentDetected(string sentiment)
        {
            Dispatcher.Invoke(() =>
            {
                SentimentDisplay.Text = sentiment;
            });
        }

        private void Chatbot_OnActivityLogged(string activity)
        {
            // Optionally handle activity logging in UI
            Dispatcher.Invoke(() =>
            {
                // Could update a log panel if added
            });
        }

        private void AddUserMessage(string message)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(15),
                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(15, 52, 96)),
                Margin = new Thickness(50, 0, 0, 10),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Right,
                MaxWidth = 500
            };

            var stack = new StackPanel { Margin = new Thickness(12, 8, 12, 8) };
            stack.Children.Add(new TextBlock 
            { 
                Text = "YOU", 
                Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 180, 216)), 
                FontWeight = FontWeights.Bold, 
                FontSize = 11 
            });
            stack.Children.Add(new TextBlock 
            { 
                Text = message, 
                Foreground = System.Windows.Media.Brushes.White, 
                FontSize = 13, 
                TextWrapping = TextWrapping.Wrap 
            });

            border.Child = stack;
            ChatMessagesPanel.Children.Add(border);
        }

        private void AddBotMessage(string message)
        {
            var border = new Border
            {
                CornerRadius = new CornerRadius(15),
                Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(27, 27, 58)),
                Margin = new Thickness(0, 0, 50, 10),
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                MaxWidth = 500
            };

            var stack = new StackPanel { Margin = new Thickness(12, 8, 12, 8) };
            stack.Children.Add(new TextBlock 
            { 
                Text = "CYBERAWARE", 
                Foreground = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 245, 160)), 
                FontWeight = FontWeights.Bold, 
                FontSize = 11 
            });
            stack.Children.Add(new TextBlock 
            { 
                Text = message, 
                Foreground = System.Windows.Media.Brushes.White, 
                FontSize = 13, 
                TextWrapping = TextWrapping.Wrap 
            });

            border.Child = stack;
            ChatMessagesPanel.Children.Add(border);
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            await ProcessUserInput();
        }

        private async void UserInputBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrWhiteSpace(UserInputBox.Text))
            {
                e.Handled = true;
                await ProcessUserInput();
            }
        }

        private void QuickTopic_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.Tag != null)
            {
                UserInputBox.Text = button.Tag.ToString();
                UserInputBox.Focus();
            }
        }

        private async void VoiceGreeting_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button)
            {
                button.IsEnabled = false;
                button.Content = "🔊 Playing...";
                if (chatbot != null) await Task.Run(() => chatbot.PlayVoiceGreeting());
                button.Content = "🔊 Voice Greeting";
                button.IsEnabled = true;
            }
        }

        private void ActivityLog_Click(object sender, RoutedEventArgs e)
        {
            if (chatbot != null)
            {
                UserInputBox.Text = "Show activity log";
                UserInputBox.Focus();
                // Auto-process the command
                _ = ProcessUserInput();
            }
        }

        private void ClearChat_Click(object sender, RoutedEventArgs e)
        {
            // Keep only the first message (welcome)
            while (ChatMessagesPanel.Children.Count > 1) 
                ChatMessagesPanel.Children.RemoveAt(1);
            
            waitingForName = true;
            chatbot?.Reset();
            AddBotMessage("Chat cleared! What is your name?");
            UserNameDisplay.Text = "Not set";
            UserInterestDisplay.Text = "None";
            SentimentDisplay.Text = "Neutral";
            UserInputBox.Focus();
        }

        private async Task ProcessUserInput()
        {
            string input = UserInputBox.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AddUserMessage(input);
            UserInputBox.Clear();
            ScrollToBottom();

            TypingIndicator.Visibility = Visibility.Visible;
            typingTimer?.Start();

            if (chatbot != null) 
                await Task.Run(() => chatbot.ProcessInput(input, waitingForName));
        }

        private void ScrollToBottom()
        {
            ChatScrollViewer.ScrollToBottom();
        }
    }
}
