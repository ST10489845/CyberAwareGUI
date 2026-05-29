using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.IO;

namespace CyberAware
{
    public class ChatbotEngine
    {
        public event Action<string>? OnResponse;
        public event Action<string, string>? OnUserInfoUpdated;
        public event Action<string>? OnSentimentDetected;

        public string UserName { get; private set; } = "";
        public string UserInterest { get; private set; } = "";

        private string? currentTopic;
        private Random random = new Random();
        private Dictionary<string, List<string>> responses = new Dictionary<string, List<string>>();

        public ChatbotEngine()
        {
            InitializeResponses();
        }

        private void InitializeResponses()
        {
            responses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["password"] = new List<string> {
                    "🔐 Use strong passwords with 12+ characters mixing uppercase, lowercase, numbers, and symbols.",
                    "🔑 Never reuse passwords across different accounts. Use a password manager!",
                    "🛡️ Enable Two-Factor Authentication whenever possible for extra security."
                },
                ["phishing"] = new List<string> {
                    "🎣 Never click suspicious links or download attachments from unknown senders.",
                    "📧 Check sender email addresses carefully for spelling errors in the domain name.",
                    "⚠️ Legitimate companies never ask for passwords or verification codes via email."
                },
                ["privacy"] = new List<string> {
                    "🕵️ Review your privacy settings on social media regularly.",
                    "🔒 Use a VPN on public WiFi to encrypt your internet traffic.",
                    "📱 Check app permissions on your phone - many request unnecessary access."
                },
                ["scam"] = new List<string> {
                    "🚨 Scammers create urgency to make you act quickly. Take time to verify independently.",
                    "📞 Never share OTPs, PINs, or passwords over the phone, even if caller ID looks legitimate.",
                    "💰 If something sounds too good to be true, it probably is a scam."
                },
                ["2fa"] = new List<string> {
                    "🔐 Two-Factor Authentication adds an extra layer of security to your accounts.",
                    "📱 Use authenticator apps like Google Authenticator or Microsoft Authenticator instead of SMS.",
                    "💾 Save backup codes in a safe place in case you lose access to your device."
                },
                ["sim swap"] = new List<string> {
                    "📱 Set up a PIN or password with your mobile provider to prevent unauthorized SIM swaps.",
                    "⚠️ Contact your bank immediately if you suddenly lose phone signal.",
                    "🔐 Never share your SIM card details or personal information with anyone over the phone."
                },
                ["safe browsing"] = new List<string> {
                    "🌐 Look for 'https://' and the padlock icon in your browser's address bar.",
                    "📡 Avoid using public WiFi for banking or shopping. Use mobile data or a VPN instead.",
                    "🔄 Keep your browser and extensions updated for important security patches."
                },
                ["vishing"] = new List<string> {
                    "📞 Vishing is voice phishing - scammers call pretending to be from banks or government.",
                    "⚠️ If you receive a suspicious call, hang up and call back using the official number.",
                    "🔴 Never share verification codes, PINs, or passwords over the phone, no matter who calls."
                }
            };
        }

        public void ProcessInput(string userInput, bool waitingForName)
        {
            string lowerInput = userInput.ToLower().Trim();

            if (waitingForName && string.IsNullOrEmpty(UserName))
            {
                if (IsValidName(userInput))
                {
                    UserName = CapitalizeName(userInput);
                    OnUserInfoUpdated?.Invoke(UserName, UserInterest);
                    OnResponse?.Invoke($"Hello {UserName}! How can I help you with cybersecurity today?");
                    return;
                }
                else
                {
                    OnResponse?.Invoke("Please tell me your name using letters only, 2 to 30 characters:");
                    return;
                }
            }

            // Sentiment detection
            string sentiment = DetectSentiment(lowerInput);
            if (sentiment != "neutral")
            {
                OnSentimentDetected?.Invoke(sentiment);
                string empathy = GetEmpathyResponse(sentiment);
                if (!string.IsNullOrEmpty(empathy))
                {
                    OnResponse?.Invoke(empathy);
                }
            }

            // Store user interest
            DetectAndStoreInterest(lowerInput);

            // Handle follow-up requests
            if (lowerInput.Contains("another") || lowerInput.Contains("more tips") || lowerInput.Contains("tell me more"))
            {
                HandleFollowUp();
                return;
            }

            // Handle greetings
            if (lowerInput.Contains("hello") || lowerInput.Contains("hi") || lowerInput.Contains("hey"))
            {
                OnResponse?.Invoke($"Hello {UserName}! What would you like to learn about cybersecurity?");
                return;
            }

            // Handle thank you
            if (lowerInput.Contains("thank") || lowerInput.Contains("thanks"))
            {
                OnResponse?.Invoke($"You are welcome {UserName}! Stay safe online! 🛡️");
                return;
            }

            // Handle help
            if (lowerInput.Contains("help") || lowerInput.Contains("what can you do"))
            {
                OnResponse?.Invoke("I can help you with:\n🔐 Password security\n🎣 Phishing scams\n🕵️ Online privacy\n🚨 Scam prevention\n📱 SIM swap protection\n🔒 Two-Factor Authentication\n🌐 Safe browsing\n\nWhat would you like to know?");
                return;
            }

            // Handle exit
            if (lowerInput.Contains("exit") || lowerInput.Contains("quit") || lowerInput.Contains("bye"))
            {
                OnResponse?.Invoke($"Goodbye {UserName}! Stay safe online! 🛡️");
                return;
            }

            // Keyword recognition
            string response = GetKeywordResponse(lowerInput);
            OnResponse?.Invoke(response);
        }

        private bool IsValidName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;
            if (input.Length < 2 || input.Length > 30) return false;
            return input.All(c => char.IsLetter(c) || c == ' ');
        }

        private string CapitalizeName(string name)
        {
            var words = name.Split(' ');
            var capitalized = words.Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower());
            return string.Join(" ", capitalized);
        }

        private string DetectSentiment(string input)
        {
            if (input.Contains("worried") || input.Contains("scared") || input.Contains("anxious") || input.Contains("nervous"))
                return "Worried";
            if (input.Contains("confused") || input.Contains("don't understand") || input.Contains("not clear"))
                return "Confused";
            if (input.Contains("frustrated") || input.Contains("annoyed") || input.Contains("upset"))
                return "Frustrated";
            if (input.Contains("curious") || input.Contains("interested") || input.Contains("tell me"))
                return "Curious";
            if (input.Contains("thank") || input.Contains("grateful") || input.Contains("appreciate"))
                return "Grateful";
            return "neutral";
        }

        private string GetEmpathyResponse(string sentiment)
        {
            switch (sentiment)
            {
                case "Worried": return "😟 I understand your concern. Let me help you feel more secure. ";
                case "Confused": return "🤔 I understand this can be confusing. Let me explain more clearly. ";
                case "Frustrated": return "😤 I hear your frustration. Let us work through this together. ";
                case "Curious": return "😊 Great curiosity! That is the perfect attitude for learning about security. ";
                case "Grateful": return "🙏 You are very welcome! It is my pleasure to help. ";
                default: return "";
            }
        }

        private void DetectAndStoreInterest(string input)
        {
            if (input.Contains("password"))
            {
                UserInterest = "Password Security";
                OnUserInfoUpdated?.Invoke(UserName, UserInterest);
            }
            else if (input.Contains("phish"))
            {
                UserInterest = "Phishing Protection";
                OnUserInfoUpdated?.Invoke(UserName, UserInterest);
            }
            else if (input.Contains("privacy"))
            {
                UserInterest = "Online Privacy";
                OnUserInfoUpdated?.Invoke(UserName, UserInterest);
            }
            else if (input.Contains("scam"))
            {
                UserInterest = "Scam Prevention";
                OnUserInfoUpdated?.Invoke(UserName, UserInterest);
            }
            else if (input.Contains("2fa") || input.Contains("two factor"))
            {
                UserInterest = "Two-Factor Authentication";
                OnUserInfoUpdated?.Invoke(UserName, UserInterest);
            }
            else if (input.Contains("sim swap"))
            {
                UserInterest = "SIM Swap Protection";
                OnUserInfoUpdated?.Invoke(UserName, UserInterest);
            }
        }

        private void HandleFollowUp()
        {
            if (!string.IsNullOrEmpty(currentTopic) && responses.ContainsKey(currentTopic))
            {
                var responseList = responses[currentTopic];
                OnResponse?.Invoke(responseList[random.Next(responseList.Count)]);
            }
            else
            {
                OnResponse?.Invoke("What topic would you like more tips about? I can help with passwords, phishing, privacy, scams, and more!");
            }
        }

        private string GetKeywordResponse(string input)
        {
            foreach (var category in responses)
            {
                if (input.Contains(category.Key.ToLower()))
                {
                    currentTopic = category.Key;
                    var responseList = category.Value;
                    return responseList[random.Next(responseList.Count)];
                }
            }

            return "I can help with cybersecurity topics like:\n🔐 Passwords\n🎣 Phishing\n🕵️ Privacy\n🚨 Scams\n📱 SIM swap\n🔒 2FA\n🌐 Safe browsing\n\nWhat would you like to know?";
        }

        // NEW: Play WAV file instead of speech synthesis
        public void PlayVoiceGreeting()
        {
            try
            {
                // Look for the audio file in the Audio folder
                string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Audio", "greeting.wav");

                if (File.Exists(audioPath))
                {
                    using (var player = new SoundPlayer(audioPath))
                    {
                        player.PlaySync(); // Plays synchronously (waits for completion)
                    }
                }
                else
                {
                    // Try alternative paths if not found
                    string[] altPaths = {
                        Path.Combine(Directory.GetCurrentDirectory(), "Audio", "greeting.wav"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "greeting.wav"),
                        "greeting.wav"
                    };

                    bool played = false;
                    foreach (var path in altPaths)
                    {
                        if (File.Exists(path))
                        {
                            using (var player = new SoundPlayer(path))
                            {
                                player.PlaySync();
                                played = true;
                                break;
                            }
                        }
                    }

                    if (!played)
                    {
                        // Fallback to beep if file not found
                        System.Media.SystemSounds.Beep.Play();
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    System.Media.SystemSounds.Beep.Play();
                }
                catch { }
            }
        }

        public void Reset()
        {
            currentTopic = null;
        }
    }
}