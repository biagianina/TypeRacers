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
        string textToType;
        InputCharacterValidation userInputValidator;
        public Dictionary<string, Tuple<bool, int>> Rank { get; set; }

        bool isValid;
        int spaceIndex;
        int correctChars;
        int incorrectChars;
        int currentWordIndex;
        private bool alert;
        readonly Model.Model model;
        private int numberOfCharactersTyped;

        public VersusViewModel()
        {
            model = new Model.Model();
            TextToType = model.GetGeneratedTextToTypeFromServer();
            userInputValidator = new InputCharacterValidation(TextToType);

            Rank = model.GetRanking();
            // first time getting opponents
            Opponents = model.GetOpponents();
            WaitingTime = model.GetWaitingTime();
            TimeToStart = DateTime.UtcNow.AddSeconds(WaitingTime /1000);
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

        public CommandHandler RemovePlayer { get; }
        public CommandHandler RestartSearchingOpponentsCommand { get; }
        public CommandHandler ExitProgramCommand { get; }

        public IEnumerable<Inline> TextToTypeStyles
        {
            get => new[] { new Run() { Text = TextToType.Substring(0, spaceIndex) , Foreground = Brushes.Gold},
                new Run() { Text = TextToType.Substring(spaceIndex, correctChars), Foreground = Brushes.Gold, TextDecorations = TextDecorations.Underline},
                new Run() { Text = TextToType.Substring(correctChars + spaceIndex, incorrectChars), TextDecorations = TextDecorations.Underline, Background = Brushes.IndianRed},
                new Run() {Text = TextToType.Substring(spaceIndex + correctChars + incorrectChars, CurrentWordLength - correctChars - incorrectChars), TextDecorations = TextDecorations.Underline},
                new Run() {Text = TextToType.Substring(spaceIndex + CurrentWordLength) }
                };
        }

        public IEnumerable<Tuple<string, Tuple<string, string, int>>> Opponents { get; private set; }

        public IEnumerable<Tuple<string, Tuple<string, string, bool, string>>> OpponentsAndRanking { get; private set; }
        public Visibility ShowFirstOpponent { get; set; }

        public Visibility ShowSecondOpponent { get; set; }

        public int WaitingTime { get; set; }
        public DateTime TimeToStart { get; private set; }
        public int OpponentsCount { get; set; }
        public int ElapsedTimeFromWaitingTimer { get; set; }
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
                    return "IndianRed";
                }

                return default;
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

        private void ReportProgress()
        {
            model.ReportProgress(WPMProgress, SliderProgress);
            Opponents = model.GetOpponents();
            TriggerPropertyChanged(nameof(Opponents));
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
                //recalculates progress 
                ReportProgress();
            }
            //checks if current word is the last one
            if (InputValidation && textToType.Length + spaceIndex == TextToType.Length)
            {
                AllTextTyped = true;
                TriggerPropertyChanged(nameof(AllTextTyped));
                TriggerPropertyChanged(nameof(SliderProgress));
                TriggerPropertyChanged(nameof(WPMProgress));//recalculates progress 
                ReportProgress();
            }
        }
        private void HighlightText()
        {
            if (!Keyboard.IsKeyDown(Key.Back))
            {
                if (isValid)
                {
                    TypingAlert = false;
                    correctChars = textToType.Length;
                    incorrectChars = 0;
                }

                if (!isValid)
                {
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
            TimeToStart = DateTime.UtcNow.AddSeconds(WaitingTime / 1000);

            
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
        private void UpdateOpponents(Tuple<List<Tuple<string, Tuple<string, string, int>>>, int, Dictionary<string, Tuple<bool, int>>> updatedOpponentsAndRankingAndElapsedTime)
        {
            Opponents = updatedOpponentsAndRankingAndElapsedTime.Item1;
            TriggerPropertyChanged(nameof(Opponents));

            ElapsedTimeFromWaitingTimer = updatedOpponentsAndRankingAndElapsedTime.Item2;
            TriggerPropertyChanged(nameof(ElapsedTimeFromWaitingTimer));

            Rank = updatedOpponentsAndRankingAndElapsedTime.Item3;
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
            if (!string.IsNullOrEmpty(model.GetStartingTime()))
            {
                EnableSearchingAnimation = false;
                TriggerPropertyChanged(nameof(EnableSearchingAnimation));
                StartTime = DateTime.Parse(model.GetStartingTime());
                TriggerPropertyChanged(nameof(StartTime));
                SecondsToGetReady = (StartTime - DateTime.UtcNow).Seconds.ToString();
                TriggerPropertyChanged(nameof(SecondsToGetReady));
                EnableGetReadyAlert = true;
                TriggerPropertyChanged(nameof(EnableGetReadyAlert));
            }
        }
        private void CheckIfWaitingTimeHasPassed()
        {
            if (TimeToStart.Subtract(DateTime.UtcNow) <= TimeSpan.Zero && string.IsNullOrEmpty(model.GetStartingTime()))
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
