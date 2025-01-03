﻿namespace POMO
{
    [QueryProperty(nameof(InitialTimeValue), "InitialTimeValue")]
    [QueryProperty(nameof(PomodoroCount), "PomodoroCount")]
    public partial class RunningTimePage : ContentPage
	{
        private double remainingTime;
        // Property to receive the time value
        public double InitialTimeValue { get; set; }
        public int PomodoroCount { get; set; } // Pomodoro count

        public RunningTimePage()
		{
			InitializeComponent();

            // Hook up the button click to the pause/resume function
            PauseResumeButton.Clicked += (sender, e) => PauseOrResumeTimer();

            // Listen for the SkipConfirmed event from the SkipPopUp
            SkipPopup.SkipConfirmed += OnSkipConfirmed;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Display the Pomodoro count
            PomodoroCountLabel.Text = $"POMO: (1/{PomodoroCount})";
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);

            // Ensure InitialTimeValue is applied
            remainingTime = Math.Floor(InitialTimeValue);
            RunningTimerLabel.Text = $"{(int)remainingTime}:00";

            StartTimerWithDelay();
        }

        private async void StartTimerWithDelay()
        {
            // Add a 2-second delay before the timer begins
            await Task.Delay(300);

            // Start the timer after the delay
            StartTimer();
        }

        // Variable to track if the timer is paused
        private bool isPaused = false;
        // Timer state control
        private void PauseOrResumeTimer()
        {
            // Toggle pause state
            isPaused = !isPaused;

            if (isPaused)
            {
                // Update the button icon to "Resume" (▶️) when paused
                PauseResumeButton.Text = "▶";
            }
            else
            {
                // Update the button icon to "Pause" (||) when resumed
                PauseResumeButton.Text = "||";
            }
        }

        private void StartTimer()
        {
            // Use the Dispatcher to start the timer
            Dispatcher.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (isPaused)
                {
                    // If paused, don't decrement time, just keep the timer running
                    return true;
                }

                if (remainingTime > 0)
                {
                    remainingTime -= 1.0 / 60; // Decrement by 1 second
                    UpdateTimerLabel();
                    return true; // Continue the timer
                }
                else
                {
                    // Timer has completed
                    Dispatcher.Dispatch(async () =>
                    {
                        await DisplayAlert("Time's Up!", "The session has ended.", "OK");
                        await Shell.Current.GoToAsync("TimerPage");
                    });
                    return false; // Stop the timer
                }
            });
        }

        private void UpdateTimerLabel()
        {
            int minutes = (int)Math.Floor(remainingTime);
            int seconds = (int)((remainingTime - minutes) * 60);
            
            // Ensure seconds always start at 0
            if (seconds < 0) seconds = 0;

            RunningTimerLabel.Text = $"{minutes:D2}:{seconds:D2}"; // Update the label
        }

        private async void OnHomeButtonTapped(object sender, EventArgs e)
        {
            // Navigate to MainPage
            await Shell.Current.GoToAsync("///MainPage");
        }

        private async void OnClockButtonTapped(object sender, EventArgs e)
        {
            // Navigate to TimerPage
            await Shell.Current.GoToAsync("TimerPage");
        }

        private void OnSkipButtonTapped(object sender, EventArgs e)
        {
            SkipPopup.IsVisible = true; // Show the pop-up
        }

        private void OnSkipConfirmed(object? sender, EventArgs e)
        {
            // Logic when the skip is confirmed
            DisplayAlert("Skipped", "You skipped the current focus session.", "OK");
        }
    }
}