using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TypeRacers.ViewModel
{
    public class VersusViewModel : ITextToType, INotifyPropertyChanged
    {
        private string currentInputText;
        private InputCharacterValidation userInputValidator;
        private bool isValid;
        private int spaceIndex;
        private int correctChars;
        private int incorrectChars;
        private int currentWordIndex;
        private bool alert;
        private int numberOfCharactersTyped;
        private int incorrectTyping;
        private int correctTyping;
        private bool startReporting;
        private Player player;
        private IPlayroom gameInfo;

        public VersusViewModel()
        {
            UpdateShownPlayers();
            EnableSearchingAnimation = true;
            ExitProgramCommand = new CommandHandler(ExitProgram, () => true);
            RemovePlayer = new CommandHandler(RemovePlayerFromPlayroom, () => true);
            RestartSearchingOpponentsCommand = new CommandHandler(RestartSearchingOpponents, () => true);
        }

        public Player Player
        {
            get => player;
            set
            {
                player = value;
                TriggerPropertyChanged(nameof(Player));
            }
        }

        public IPlayroom GameInfo
        {
            get => gameInfo;
            set
            {
                gameInfo = value;
                TriggerPropertyChanged(nameof(GameInfo));
                TriggerPropertyChanged(nameof(TextToType));
                TriggerPropertyChanged(nameof(TextToTypeStyles));
                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(UserInputValidator));
                GameInfo.SubscribeToSearchingOpponents(UpdateOpponents);
                TriggerPropertyChanged(nameof(Opponents));
                TriggerPropertyChanged(nameof(WaitingTime));
                TriggerPropertyChanged(nameof(StartTime));
                TriggerPropertyChanged(nameof(EndTime));
                TriggerPropertyChanged(nameof(ShowRanking));
                TriggerPropertyChanged(nameof(RankingPlace));
                UpdateShownPlayers();
            }
        }

        public CommandHandler RemovePlayer { get; }
        public CommandHandler RestartSearchingOpponentsCommand { get; }
        public CommandHandler ExitProgramCommand { get; }

        public IEnumerable<Inline> TextToTypeStyles
        {
            get => new[] { new Run() { Text = TextToType.Substring(0, spaceIndex) , Foreground = Brushes.Salmon},
                new Run() { Text = TextToType.Substring(spaceIndex, correctChars), Foreground = Brushes.Salmon, TextDecorations = TextDecorations.Underline},
                new Run() { Text = TextToType.Substring(correctChars + spaceIndex, incorrectChars), TextDecorations = TextDecorations.Underline, Background = Brushes.Salmon},
                new Run() {Text = TextToType.Substring(spaceIndex + correctChars + incorrectChars, CurrentWordLength - correctChars - incorrectChars), TextDecorations = TextDecorations.Underline},
                new Run() {Text = TextToType.Substring(spaceIndex + CurrentWordLength) }
                };
        }

        public IEnumerable<Player> Opponents => GameInfo?.Players ?? new List<Player>();

        public Visibility ShowFirstOpponent { get; set; }

        public Visibility ShowSecondOpponent { get; set; }

        public DateTime WaitingTime => GameInfo.TimeToWaitForOpponents;
        public DateTime TimeToStart => DateTime.UtcNow.AddSeconds((WaitingTime.Subtract(DateTime.UtcNow).Seconds));
        public int OpponentsCount { get; set; }

        private InputCharacterValidation UserInputValidator { get => userInputValidator ?? new InputCharacterValidation(TextToType); set => userInputValidator = value; }

        public bool InputValidation
        {
            get => isValid;

            set
            {
                if (isValid == value)
                    return;

                isValid = value;
                TriggerPropertyChanged(nameof(InputValidation));
                TriggerPropertyChanged(nameof(InputBackgroundColor));
            }
        }

        public int SliderProgress
        {
            get
            {
                if (AllTextTyped || TextToType.Length == 0)
                {
                    return 100;
                }

                return spaceIndex * 100 / TextToType.Length;
            }
            set
            {
                SliderProgress = value;

                TriggerPropertyChanged(nameof(SliderProgress));
            }
        }

        public int WPMProgress
        {
            get
            {
                if (currentWordIndex == 0)
                {
                    return 0;
                }

                var wordperminut = (numberOfCharactersTyped / 5) * 60;
                var secondsInGame = (int)(DateTime.UtcNow - StartTime).TotalSeconds;
                return wordperminut / secondsInGame;
            }

            set
            {
                WPMProgress = value;

                TriggerPropertyChanged(nameof(WPMProgress));
            }
        }

        public int CurrentWordLength
        {
            get => TextToType.Split()[currentWordIndex].Length;//length of current word
        }

        public bool AllTextTyped
        {
            get
            {
                return AllTextTyped;
            }
            set
            {
                AllTextTyped = value;
                TriggerPropertyChanged(nameof(AllTextTyped));

            }
        }

        //determines if a popup alert should apear, binded in open property of popup xaml
        public bool TypingAlert
        {
            get => alert;

            set
            {
                if (alert == value)
                {
                    return;
                }

                alert = value;
                TriggerPropertyChanged(nameof(TypingAlert));
            }
        }

        public string InputBackgroundColor
        {
            get
            {
                if (string.IsNullOrEmpty(CurrentInputText))
                {
                    return default;
                }
                if (!isValid)
                {
                    return "Salmon";
                }

                return default;
            }
        }

        public bool StartReportingProgress
        {
            get => startReporting;

            set
            {
                startReporting = value;
                TriggerPropertyChanged(nameof(StartReportingProgress));
                ReportProgress();
            }
        }

        public string TextToType => GameInfo?.CompetitionText ?? string.Empty;

        public bool EnableGetReadyAlert { get; set; }
        public bool EnableRestartOrExitAlert { get; set; }
        public string SecondsToGetReady { get; set; }
        public bool EnableSearchingAnimation { get; private set; }
        public DateTime StartTime { get; set; }
        public bool ShowRanking => Player?.Finnished ?? false;
        public string RankingPlace => Player?.Place.ToString() ?? string.Empty;
        public int Accuracy
        {
            get { return Accuracy; }
            private set
            {
                Accuracy = value;
                TriggerPropertyChanged(nameof(Accuracy));
            }
        }
        public bool OpenFinishPopup
        {
            get { return OpenFinishPopup; }
            private set
            {
                OpenFinishPopup = value;
                TriggerPropertyChanged(nameof(OpenFinishPopup));
            }
        }
        public DateTime EndTime
        {
            get { return EndTime; }
            private set
            {

                EndTime = value;
                TriggerPropertyChanged(nameof(EndTime));
            }
        }

        private void ReportProgress()
        {
            if (StartReportingProgress)
            {
                TriggerPropertyChanged(nameof(Opponents));
                Player.UpdateInfo(WPMProgress, SliderProgress);
            }
        }
        public string CurrentInputText
        {
            get => currentInputText;
            set
            {
                // return because we dont need to execute logic if the input text has not changed
                if (currentInputText == value)
                    return;

                currentInputText = value;

                //validate current word
                InputValidation = UserInputValidator.ValidateWord(CurrentInputText, CurrentInputText.Length, ref spaceIndex, ref currentWordIndex, ref numberOfCharactersTyped);

                //CheckUserInput(currentInputText);
                if (UserInputValidator.IsLastWord())
                {
                    AllTextTyped = true;
                    EndTime = DateTime.Now;
                    Accuracy = 100 - (incorrectTyping * 100 / correctTyping);
                    OpenFinishPopup = true;

                }

                TriggerPropertyChanged(nameof(CurrentWordLength));//moves to next word
                //determine number of characters that are valid/invalid to form substrings
                HighlightText();
                TriggerPropertyChanged(nameof(CurrentInputText));
            }
        }

        private void HighlightText()
        {
            if (!Keyboard.IsKeyDown(Key.Back))
            {
                if (isValid)
                {
                    correctTyping++;
                    TypingAlert = false;
                    correctChars = currentInputText.Length;
                    incorrectChars = 0;
                }

                if (!isValid)
                {
                    incorrectTyping++;
                    incorrectChars++;
                    if (CurrentWordLength - correctChars - incorrectChars < 0)
                    {
                        TypingAlert = true;
                        currentInputText = currentInputText.Substring(0, correctChars);
                        incorrectChars = 0;
                    }
                }
            }
            else
            {
                if (!isValid && !string.IsNullOrEmpty(currentInputText))
                {
                    incorrectChars--;
                }
                else
                {
                    TypingAlert = false;
                    correctChars = currentInputText.Length;
                    incorrectChars = 0;
                }
            }

            TriggerPropertyChanged(nameof(TextToTypeStyles)); //new Inlines formed at each char in input
        }

        private void RestartSearchingOpponents()
        {
            EnableRestartOrExitAlert = false;
            TriggerPropertyChanged(nameof(EnableRestartOrExitAlert));

            Player.Restarting = true;
            Thread.Sleep(1000);
            //getting the waiting time again
            TriggerPropertyChanged(nameof(WaitingTime));
            TriggerPropertyChanged(nameof(TimeToStart));
            EnableSearchingAnimation = true;
            TriggerPropertyChanged(nameof(EnableSearchingAnimation));
        }

        private void ExitProgram()
        {
            Player.Removed = true;
            Application.Current.Shutdown();
        }

        private void RemovePlayerFromPlayroom()
        {
            Player.Removed = true;
        }

        private void UpdateOpponents(List<Player> uppdateOpponents)
        {
            //    Opponents = uppdateOpponents;
            TriggerPropertyChanged(nameof(Opponents));

            OpponentsCount = Opponents.Count();
            TriggerPropertyChanged(nameof(OpponentsCount));

            TriggerPropertyChanged(nameof(RankingPlace));
            TriggerPropertyChanged(nameof(ShowRanking));

            UpdateShownPlayers();

            CheckIfStartTimeWasSet();

            CheckIfWaitingTimeHasPassed();
        }

        private void CheckIfStartTimeWasSet()
        {
            if (GameInfo.GameStartingTime != DateTime.MinValue)
            {
                EnableSearchingAnimation = false;
                TriggerPropertyChanged(nameof(EnableSearchingAnimation));

                StartTime = GameInfo.GameStartingTime;
                TriggerPropertyChanged(nameof(StartTime));

                EndTime = GameInfo.GameEndingTime;
                TriggerPropertyChanged(nameof(EndTime));

                SecondsToGetReady = (StartTime - DateTime.UtcNow).Seconds.ToString();
                TriggerPropertyChanged(nameof(SecondsToGetReady));

                EnableGetReadyAlert = true;
                TriggerPropertyChanged(nameof(EnableGetReadyAlert));
            }
        }

        private void CheckIfWaitingTimeHasPassed()
        {
            if (TimeToStart.Subtract(DateTime.UtcNow) <= TimeSpan.Zero && GameInfo.GameStartingTime == DateTime.MinValue && !EnableGetReadyAlert)
            {
                if (Opponents.Count() == 1)
                {
                    CheckIfStartTimeWasSet();
                }
                else
                {
                    EnableSearchingAnimation = false;
                    TriggerPropertyChanged(nameof(EnableSearchingAnimation));
                    EnableRestartOrExitAlert = true;
                    TriggerPropertyChanged(nameof(EnableRestartOrExitAlert));
                }
            }
        }

        private void UpdateShownPlayers()
        {
            if (Opponents.Count() == 0)
            {
                ShowFirstOpponent = Visibility.Hidden;
                ShowSecondOpponent = Visibility.Hidden;
                return;
            }
            if (Opponents.Count() == 1)
            {
                ShowFirstOpponent = Visibility.Visible;
                ShowSecondOpponent = Visibility.Hidden;
            }

            if (Opponents.Count() == 2)
            {
                ShowFirstOpponent = Visibility.Visible;
                ShowSecondOpponent = Visibility.Visible;
            }

            TriggerPropertyChanged(nameof(ShowFirstOpponent));
            TriggerPropertyChanged(nameof(ShowSecondOpponent));
        }

        //INotifyPropertyChanged code - basic
        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}