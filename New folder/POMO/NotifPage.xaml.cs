namespace POMO
{
	public partial class NotifPage : ContentPage
	{

        public NotifPage()
		{
			InitializeComponent();
		}

        private async void OnHomeButtonTapped(object sender, EventArgs e)
        {
            // Navigate to MainPage
            await Shell.Current.GoToAsync("MainPage");
        }
    }
}