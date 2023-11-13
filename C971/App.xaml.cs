using Xamarin.Forms;
using C971.Views;
using C971.Services;

namespace C971
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            DatabaseServices.DBInit();
            MainPage = new AcademicTerms();
        }

        protected override void OnStart()
        {
            NotificationServices.CourseStartNotifs();
            NotificationServices.CourseEndNotifs();
            NotificationServices.AssessmentStartNotifs();
            NotificationServices.AssessmentEndNotifs();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
