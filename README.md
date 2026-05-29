# CyberAwareGUI
# 🛡️ CYBERAWARE - Cybersecurity Awareness Chatbot

## 📋 Overview

CyberAware is an interactive desktop application designed to educate users about cybersecurity threats and best practices. It features a modern WPF GUI with a chatbot that responds to cybersecurity-related queries, detects user sentiment, remembers user information, and provides dynamic responses.

## 🚀 Features

### 1. Modern GUI Interface
- Clean, dark-themed WPF application
- Collapsible sidebar with user information
- Professional ASCII art header
- Chat bubble interface with typing indicators
- Responsive design that works on different screen sizes

### 2. Voice Greeting
- Custom voice greeting with your own `.wav` file
- One-click voice greeting button
- Fallback beep if audio file is not found

### 3. Keyword Recognition
The chatbot recognizes the following cybersecurity topics:
- 🔐 **Password Security** - Strong passwords, password managers, 2FA
- 🎣 **Phishing Scams** - Email fraud, suspicious links, social engineering
- 🕵️ **Online Privacy** - Privacy settings, VPNs, app permissions
- 🚨 **Scam Prevention** - Lottery scams, tech support scams, romance scams
- 📱 **SIM Swap Protection** - Mobile security, carrier PINs
- 🔒 **Two-Factor Authentication** - 2FA setup, authenticator apps
- 🌐 **Safe Browsing** - HTTPS, public WiFi safety, browser updates
- 📞 **Vishing Calls** - Voice phishing, phone scam awareness

### 4. Random Responses
- Multiple response variations for each topic
- Randomized selection keeps conversations fresh
- Different tips each time you ask

### 5. Conversation Flow
- Handles follow-up questions ("another tip", "tell me more")
- Maintains context across the conversation
- Natural conversation progression

### 6. Memory and Recall
- Remembers user's name throughout the session
- Stores user's cybersecurity interest area
- Personalizes responses using stored information
- Resets only when user clicks "Clear Chat"

### 7. Sentiment Detection
Detects user emotions and responds appropriately:
- 😟 **Worried** - Provides reassurance and security tips
- 🤔 **Confused** - Offers clearer explanations
- 😤 **Frustrated** - Shows empathy and works through issues
- 😊 **Curious** - Encourages learning more
- 🙏 **Grateful** - Responds with appreciation

### 8. Error Handling
- Graceful handling of invalid inputs
- Default responses for unrecognized queries
- No application crashes from unexpected input

### 9. Code Optimization
- Dictionary-based response management for O(1) lookups
- Clean separation of concerns (UI, Logic, Audio)
- Event-driven architecture
- Async operations for responsive UI

## 📁 Project Structure


