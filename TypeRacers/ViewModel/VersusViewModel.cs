using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TypeRacers.ViewModel
{
    public class VersusViewModel : ITextToType, INotifyPropertyChanged
    {
        private string textToType;
        private InputCharacterValidation userInputValidator;
        private bool isValid;
        private int spaceIndex;
        private int correctChars;
        private int incorrectChars;
        private int currentWordIndex;
        private bool alert;
        private readonly Model.Model model;
        private int numberOfCharactersTyped;
        private int incorrectTyping;
        private int correctTyping;
        private bool startReporting;

        public VersusViewModel()
        {
            model = new Model.Model();
            TextToType = model.GetGeneratedTextToTypeFromServer();
            userInputValidator = new InputCharacterValidation(TextToType);

            Rank = model.GetRanking();
            // first time getting opponents
            Opponents = model.GetOpponents();
            WaitingTime = model.GetWaitingTime();
            TimeToStart = DateTime.UtcNow.AddSeconds((WaitingTime - DateTime.UtcNow).Seconds);
            EnableSearchingAnimation = true;

            //check how many players can we display on the screen
            UpdateShownPlayers();

            ExitProgramCommand = new CommandHandler(() => ExitProgram(), () => true);
            RemovePlayer = new CommandHandler(() => RemovePlayerFromPlayroom(), () => true);
            RestartSearchingOpponentsCommand = new CommandHandler(() => RestartSearchingOpponents(), () => true);
            //start searching for 30 seconds and subscribe to timer
            model.StartSearchingOpponents();
            model.SubscribeToSearchingOpponents(UpdateOpponents);
            CanUserType = false;
        }

        public Dictionary<string, Tuple<bool, int>> Rank { get; set; }
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

        public IEnumerable<Tuple<string, Tuple<string, string, int>>> Opponents { get; private set; }

        public IEnumerable<Tuple<string, Tuple<string, string, bool, string>>> OpponentsAndRanking { get; private set; }

        public Visibility ShowFirstOpponent { get; set; }

        public Visibility ShowSecondOpponent { get; set; }

        public DateTime WaitingTime { get; set; }
        public DateTime TimeToStart { get; private set; }
        public int OpponentsCount { get; set; }

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

        public bool CanUserType { get; set; }

        public int SliderProgress
        {
            get
            {
                if (AllTextTyped)
                {
                    return 100;
                }

                return spaceIndex * 100 / TextToType.Length;
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
        }

        public int CurrentWordLength
        {
            get => TextToType.Split()[currentWordIndex].Length;//length of current word
        }

        public bool GetReadyAlert { get; set; }
        public bool AllTextTyped { get; set; }

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

        public string TextToType { get; }

        public string CurrentInputText
        {
            get => textToType;
            set
            {
                // return because we dont need to execute logic if the input text has not changed
                if (textToType == value)
                    return;

                textToType = value;

                //validate current word
                InputValidation = userInputValidator.ValidateWord(CurrentInputText, CurrentInputText.Length);

                CheckUserInput(textToType);

                TriggerPropertyChanged(nameof(CurrentWordLength));//moves to next word

                //determine number of characters that are valid/invalid to form substrings
                HighlightText();

                TriggerPropertyChanged(nameof(CurrentInputText));
            }
        }

        public bool EnableGetReadyAlert { get; set; }
        public bool EnableRestartOrExitAlert { get; set; }
        public string SecondsToGetReady { get; set; }
        public bool EnableSearchingAnimation { get; private set; }
        public DateTime StartTime { get; set; }
        public bool ShowRanking { get; private set; }
        public string RankingPlace { get; private set; }
        public int Accuracy { get; private set; }
        public bool OpenFinishPopup { get; private set; }
        public DateTime EndTime { get; private set; }

        private void ReportProgress()
        {
            if (StartReportingProgress)
            {
                Opponents = model.GetOpponents();
                TriggerPropertyChanged(nameof(Opponents));
                model.ReportProgress(WPMProgress, SliderProgress);
            }
        }

        private void CheckUserInput(string value)
        {
            //checks if current word is typed, clears textbox, reintializes remaining text to the validation, sends progress
            if (isValid && value.EndsWith(" "))
            {
                spaceIndex += textToType.Length;

                if (currentWordIndex < TextToType.Split().Length - 1)
                {
                    currentWordIndex++;
                }

                userInputValidator = new InputCharacterValidation(TextToType.Substring(spaceIndex));
                numberOfCharactersTyped += CurrentInputText.Length;
                textToType = string.Empty;

                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(WPMProgress));
            }

            //checks if current word is the last one
            if (InputValidation && textToType.Length + spaceIndex == TextToType.Length)
            {
                AllTextTyped = true;
                TriggerPropertyChanged(nameof(AllTextTyped));

                EndTime = DateTime.UtcNow;
                TriggerPropertyChanged(nameof(EndTime));

                Accuracy = 100 - (incorrectTyping * 100 / correctTyping);
                TriggerPropertyChanged(nameof(Accuracy));

                OpenFinishPopup = true;
                TriggerPropertyChanged(nameof(OpenFinishPopup));

                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(WPMProgress));//recalculates progress
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
                    correctChars = textToType.Length;
                    incorrectChars = 0;
                }

                if (!isValid)
                {
                    incorrectTyping++;
                    incorrectChars++;
                    if (CurrentWordLength - correctChars - incorrectChars < 0)
                    {
                        TypingAlert = true;
                        textToType = textToType.Substring(0, correctChars);
                        incorrectChars = 0;
                    }
                }
            }
            else
            {
                if (!isValid && !string.IsNullOrEmpty(textToType))
                {
                    incorrectChars--;
                }
                else
                {
                    TypingAlert = false;
                    correctChars = textToType.Length;
                    incorrectChars = 0;
                }
            }

            TriggerPropertyChanged(nameof(TextToTypeStyles)); //new Inlines formed at each char in input
        }

        private void RestartSearchingOpponents()
        {
            EnableRestartOrExitAlert = false;
            TriggerPropertyChanged(nameof(EnableRestartOrExitAlert));

            model.RestartSearch();
            WaitingTime = model.GetWaitingTime();
            TimeToStart = DateTime.UtcNow.AddSeconds((WaitingTime - DateTime.UtcNow).Seconds);
            model.StartSearchingOpponents();

            EnableSearchingAnimation = true;
            TriggerPropertyChanged(nameof(EnableSearchingAnimation));
        }

        private void ExitProgram()
        {
            Application.Current.Shutdown();
        }

        private void RemovePlayerFromPlayroom()
        {
            model.RemovePlayer();
        }

        private void UpdateOpponents(Tuple<List<Tuple<string, Tuple<string, string, int>>>, Dictionary<string, Tuple<bool, int>>> updatedOpponentsAndRanking)
        {
            Opponents = updatedOpponentsAndRanking.Item1;
            TriggerPropertyChanged(nameof(Opponents));

            Rank = updatedOpponentsAndRanking.Item2;
            TriggerPropertyChanged(nameof(Rank));

            SetOpponentsAndRanking();

            OpponentsCount = Opponents.Count() + 1;
            TriggerPropertyChanged(nameof(OpponentsCount));

            UpdateShownPlayers();

            CheckIfStartTimeWasSet();

            CheckIfWaitingTimeHasPassed();
        }

        private void SetOpponentsAndRanking()
        {
            List<Tuple<string, Tuple<string, string, bool, string>>> opponentsAndRanking = new List<Tuple<string, Tuple<string, string, bool, string>>>();
            foreach (var rank in Rank)
            {
                var player = Opponents.FirstOrDefault(op => op.Item1.Equals(rank.Key));
                if (player != default)
                {
                    opponentsAndRanking.Add(new Tuple<string, Tuple<string, string, bool, string>>(rank.Key, new Tuple<string, string, bool, string>(player.Item2.Item1, player.Item2.Item2, rank.Value.Item1, Convert.ToString(rank.Value.Item2))));
                }
                else
                {
                    ShowRanking = rank.Value.Item1;
                    TriggerPropertyChanged(nameof(ShowRanking));
                    RankingPlace = rank.Value.Item2.ToString();
                    TriggerPropertyChanged(nameof(RankingPlace));
                }
            }

            OpponentsAndRanking = opponentsAndRanking.AsEnumerable();
            TriggerPropertyChanged(nameof(OpponentsAndRanking));
        }

        private void CheckIfStartTimeWasSet()
        {
            var m = model.GetStartingTime();
            var n = DateTime.MinValue;
            if (model.GetStartingTime() != DateTime.MinValue)
            {
                EnableSearchingAnimation = false;
                TriggerPropertyChanged(nameof(EnableSearchingAnimation));

                StartTime = model.GetStartingTime();
                TriggerPropertyChanged(nameof(StartTime));

                EndTime = model.GetEndingTime();
                TriggerPropertyChanged(nameof(EndTime));

                model.StartGameProgressReporting();

                SecondsToGetReady = (StartTime - DateTime.UtcNow).Seconds.ToString();
                TriggerPropertyChanged(nameof(SecondsToGetReady));

                EnableGetReadyAlert = true;
                TriggerPropertyChanged(nameof(EnableGetReadyAlert));
            }
        }

        private void CheckIfWaitingTimeHasPassed()
        {
            if (TimeToStart.Subtract(DateTime.UtcNow) <= TimeSpan.Zero && model.GetStartingTime() == DateTime.MinValue && !EnableGetReadyAlert)
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