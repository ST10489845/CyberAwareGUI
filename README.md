# 🛡️ CYBERAWARE - Cybersecurity Awareness Chatbot

## 📋 Project Overview

CyberAware is an interactive desktop application designed to educate users about cybersecurity threats and best practices. It features a modern WPF GUI with an intelligent chatbot that responds to cybersecurity-related queries, detects user sentiment, remembers user information, and provides dynamic, engaging responses.

This application was developed as Part 2 of a cybersecurity awareness project, demonstrating proficiency in WPF GUI development, C# programming, and chatbot implementation.

---

## 🚀 Key Features

### 1. Modern GUI Interface
- Clean, dark-themed WPF application
- Collapsible sidebar with user information display
- Professional header with branding
- Chat bubble interface with user/bot differentiation
- Typing indicator animation
- Responsive design

### 2. Voice Greeting
- Custom voice greeting using your own `.wav` file
- One-click voice greeting button in sidebar
- Automatic file detection from Audio folder
- Fallback beep if audio file not found

### 3. Keyword Recognition (10+ Topics)

| Topic | Response Focus |
|-------|----------------|
| 🔐 Password Security | Strong passwords, password managers, 2FA |
| 🎣 Phishing Scams | Email fraud, suspicious links, social engineering |
| 🕵️ Online Privacy | Privacy settings, VPNs, app permissions |
| 🚨 Scam Prevention | Lottery scams, tech support scams, romance scams |
| 📱 SIM Swap Protection | Mobile security, carrier PINs, warning signs |
| 🔒 Two-Factor Authentication | 2FA setup, authenticator apps, backup codes |
| 🌐 Safe Browsing | HTTPS, public WiFi safety, browser updates |
| 📞 Vishing Calls | Voice phishing, phone scam awareness |

### 4. Random Responses
- Multiple response variations for each topic
- Randomized selection keeps conversations fresh
- Users get different tips each time they ask

### 5. Conversation Flow
- Handles follow-up questions ("another tip", "tell me more")
- Maintains context across the conversation
- Natural conversation progression without restarting

### 6. Memory and Recall
- Remembers user's name throughout the session
- Stores user's cybersecurity interest area
- Personalizes responses using stored information
- Memory persists until "Clear Chat" is clicked

### 7. Sentiment Detection

| Sentiment | Keywords | Response Style |
|-----------|----------|----------------|
| 😟 Worried | worried, scared, anxious, nervous | Reassurance + security tips |
| 🤔 Confused | confused, don't understand, not clear | Clearer explanations |
| 😤 Frustrated | frustrated, annoyed, upset | Empathy + working through |
| 😊 Curious | curious, interested, tell me | Encouragement to learn |
| 🙏 Grateful | thank, grateful, appreciate | Appreciation response |

### 8. Error Handling
- Graceful handling of invalid inputs
- Default responses for unrecognized queries
- No application crashes from unexpected input
- Help command available for guidance

### 9. Code Optimization
- Dictionary-based response management for O(1) lookups
- Clean separation of concerns (UI, Logic, Audio)
- Event-driven architecture
- Async operations for responsive UI
- Nullable reference types for safety

---

## 📁 Project Structure
CyberAware/
│
├── CyberAware.csproj # Project configuration file
├── App.xaml # Application definition
├── App.xaml.cs # Application code-behind
├── MainWindow.xaml # Main GUI layout
├── MainWindow.xaml.cs # UI event handlers
├── ChatbotEngine.cs # Chatbot logic engine
│
├── Audio/ # Audio files folder
│ └── greeting.wav # Your custom voice greeting


---

## 🛠️ Technologies Used

| Technology | Purpose |
|------------|---------|
| .NET 8.0 | Application framework |
| WPF | GUI development |
| C# | Programming language |
| XAML | UI markup language |
| System.Media.SoundPlayer | WAV audio playback |

---

## 📋 System Requirements

- **Operating System**: Windows 10 or Windows 11
- **Framework**: .NET 8.0 Runtime
- **Development**: Visual Studio 2022
- **Memory**: 2GB RAM minimum
- **Storage**: 50MB free space

