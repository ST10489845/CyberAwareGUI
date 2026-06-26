using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberAware
{
    public class QuizEngine
    {
        private ChatbotEngine chatbot;
        private List<QuizQuestion> questions;
        private int currentQuestionIndex = -1;
        private int score = 0;
        private bool quizActive = false;

        public QuizEngine(ChatbotEngine chatbot)
        {
            this.chatbot = chatbot;
            InitializeQuestions();
        }

        private void InitializeQuestions()
        {
            questions = new List<QuizQuestion>
            {
                new QuizQuestion
                {
                    Question = "What should you do if you receive an email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Delete the email", "Report the email as phishing", "Ignore it" },
                    CorrectAnswer = 2,
                    Explanation = "Reporting phishing emails helps prevent scams and protects others from falling victim."
                },
                new QuizQuestion
                {
                    Question = "Which of the following is a strong password?",
                    Options = new List<string> { "123456", "password", "MyDogIsTheBest!2024", "qwerty" },
                    CorrectAnswer = 2,
                    Explanation = "A strong password uses a mix of uppercase, lowercase, numbers, and special characters."
                },
                new QuizQuestion
                {
                    Question = "What does 2FA stand for?",
                    Options = new List<string> { "Two Factor Authentication", "Second Factor Access", "Two Form Authorization", "Dual Factor Approval" },
                    CorrectAnswer = 0,
                    Explanation = "Two-Factor Authentication adds an extra layer of security to your accounts."
                },
                new QuizQuestion
                {
                    Question = "Is it safe to use public WiFi for online banking?",
                    Options = new List<string> { "Yes, always", "No, it's risky", "Only if you have a password", "Only for checking balances" },
                    CorrectAnswer = 1,
                    Explanation = "Public WiFi is insecure and can be intercepted. Use a VPN or mobile data instead."
                },
                new QuizQuestion
                {
                    Question = "What is phishing?",
                    Options = new List<string> { "A type of fish", "A cyber attack that tricks people into giving personal information", "A security software", "A programming language" },
                    CorrectAnswer = 1,
                    Explanation = "Phishing is a social engineering attack where scammers trick people into revealing sensitive information."
                },
                new QuizQuestion
                {
                    Question = "True or False: It's safe to reuse the same password across multiple accounts.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "If one account is compromised, all accounts with the same password are at risk."
                },
                new QuizQuestion
                {
                    Question = "What is a SIM swap scam?",
                    Options = new List<string> { "A phone upgrade scam", "A scam where attackers take over your phone number", "A mobile plan scam", "A data plan scam" },
                    CorrectAnswer = 1,
                    Explanation = "SIM swap scams occur when attackers trick your mobile provider into transferring your number to their device."
                },
                new QuizQuestion
                {
                    Question = "What should you do before clicking a link in an email?",
                    Options = new List<string> { "Click it immediately", "Hover to check the URL", "Forward it to friends", "Open it on your phone" },
                    CorrectAnswer = 1,
                    Explanation = "Hovering over a link shows the actual URL. Always verify before clicking."
                },
                new QuizQuestion
                {
                    Question = "Which of these is NOT a safe browsing practice?",
                    Options = new List<string> { "Using a VPN", "Looking for HTTPS", "Using the same password for all sites", "Regularly clearing cookies" },
                    CorrectAnswer = 2,
                    Explanation = "Using the same password for all sites is dangerous. Use unique passwords for each account."
                },
                new QuizQuestion
                {
                    Question = "What is vishing?",
                    Options = new List<string> { "Video phishing", "Voice phishing", "Visual phishing", "Virtual phishing" },
                    CorrectAnswer = 1,
                    Explanation = "Vishing is voice phishing where scammers call you pretending to be from legitimate organizations."
                },
                new QuizQuestion
                {
                    Question = "True or False: You should share your verification codes with others.",
                    Options = new List<string> { "True", "False" },
                    CorrectAnswer = 1,
                    Explanation = "Verification codes are for your use only. Never share them with anyone."
                },
                new QuizQuestion
                {
                    Question = "What is a common sign of a scam?",
                    Options = new List<string> { "Creating urgency", "Professional language", "Clear explanations", "Multiple contact options" },
                    CorrectAnswer = 0,
                    Explanation = "Scammers often create a sense of urgency to pressure you into acting without thinking."
                }
            };
        }

        public bool ProcessQuizCommand(string input, bool waitingForName)
        {
            if (waitingForName) return false;

            string lowerInput = input.ToLower().Trim();

            if (lowerInput.Contains("start quiz") || lowerInput.Contains("take quiz") || lowerInput.Contains("begin quiz"))
            {
                StartQuiz();
                return true;
            }

            if (quizActive)
            {
                // Check for answer input
                if (int.TryParse(input, out int answer) && answer >= 1 && answer <= 4)
                {
                    ProcessAnswer(answer - 1);
                    return true;
                }

                // Check for true/false
                if (lowerInput == "true" || lowerInput == "false")
                {
                    int answerIndex = lowerInput == "true" ? 0 : 1;
                    ProcessAnswer(answerIndex);
                    return true;
                }
            }

            if (lowerInput.Contains("end quiz") || lowerInput.Contains("quit quiz") || lowerInput.Contains("stop quiz"))
            {
                EndQuiz();
                return true;
            }

            return false;
        }

        private void StartQuiz()
        {
            quizActive = true;
            currentQuestionIndex = 0;
            score = 0;
            chatbot.LogActivity("Quiz started");
            ShowQuestion();
        }

        private void ShowQuestion()
        {
            if (currentQuestionIndex >= questions.Count)
            {
                EndQuiz();
                return;
            }

            var question = questions[currentQuestionIndex];
            string questionText = $"📝 Question {currentQuestionIndex + 1} of {questions.Count}:\n\n{question.Question}\n\n";

            for (int i = 0; i < question.Options.Count; i++)
            {
                questionText += $"{i + 1}. {question.Options[i]}\n";
            }

            if (question.Options.Count == 2 && question.Options.Contains("True") && question.Options.Contains("False"))
            {
                questionText += "\nType 'true' or 'false' for your answer.";
            }
            else
            {
                questionText += "\nType the number of your answer (1-4).";
            }

            chatbot.OnResponse?.Invoke(questionText);
        }

        private void ProcessAnswer(int answerIndex)
        {
            if (!quizActive || currentQuestionIndex < 0 || currentQuestionIndex >= questions.Count) return;

            var question = questions[currentQuestionIndex];
            bool isCorrect = answerIndex == question.CorrectAnswer;

            if (isCorrect)
            {
                score++;
                chatbot.OnResponse?.Invoke($"✅ Correct! {question.Explanation}");
                chatbot.LogActivity($"Quiz: Question {currentQuestionIndex + 1} answered correctly");
            }
            else
            {
                string correctAnswer = question.Options[question.CorrectAnswer];
                chatbot.OnResponse?.Invoke($"❌ Incorrect. The correct answer was: {correctAnswer}\n\n{question.Explanation}");
                chatbot.LogActivity($"Quiz: Question {currentQuestionIndex + 1} answered incorrectly");
            }

            currentQuestionIndex++;

            if (currentQuestionIndex < questions.Count)
            {
                ShowQuestion();
            }
            else
            {
                EndQuiz();
            }
        }

        private void EndQuiz()
        {
            if (!quizActive) return;

            quizActive = false;
            int totalQuestions = questions.Count;
            double percentage = (double)score / totalQuestions * 100;

            string feedback = "";
            if (percentage >= 90) feedback = "🌟 Excellent! You're a cybersecurity pro!";
            else if (percentage >= 70) feedback = "👍 Good job! You know your cybersecurity basics well.";
            else if (percentage >= 50) feedback = "📚 Not bad! Keep learning to stay safe online.";
            else feedback = "🔒 Keep learning! Cybersecurity is important for everyone.";

            chatbot.OnResponse?.Invoke($"🏆 Quiz Complete!\n\nScore: {score} out of {totalQuestions} ({percentage:F1}%)\n\n{feedback}");
            chatbot.LogActivity($"Quiz completed: Score {score}/{totalQuestions} ({percentage:F1}%)");

            currentQuestionIndex = -1;
        }

        public bool IsQuizActive() => quizActive;
        public int GetCurrentQuestionIndex() => currentQuestionIndex;
    }

    public class QuizQuestion
    {
        public string Question { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectAnswer { get; set; }
        public string Explanation { get; set; } = "";
    }
}
