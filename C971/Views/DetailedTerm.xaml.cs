using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using C971.Services;
using C971.Models;

namespace C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailedTerm : ContentPage
    {
        int currentTerm;
        public DetailedTerm(string termID, string termName, string termStart, string termEnd)
        {
            InitializeComponent();
            TermID.Text = termID;
            TermName.Text = termName;
            TermStart.Text = termStart;
            TermEnd.Text = termEnd;
            currentTerm = Convert.ToInt32(termID);

            var courses = DatabaseServices.CoursesList(Convert.ToInt32(termID));
            CoursesCV.ItemsSource = courses;
        }

        private async void ReturnButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void AddCourse_Clicked(object sender, EventArgs e)
        {
            bool continueAdd = Course.CourseCountValidation(currentTerm);
            if (continueAdd)
            {
                // CourseID is assigned by finding the current max and adding 1
                int maxCourseID = DatabaseServices.CoursesList().Max(t => t.CourseID);
                int newCourseID = maxCourseID + 1;

                // Open DetailedCourse page with defaults
                DetailedCourse dc = new DetailedCourse(newCourseID, currentTerm);
                await Navigation.PushModalAsync(dc);
            }
        }

        private async void CourseDetails_Clicked(object sender, EventArgs e)
        {
            // Determining the specific button clicked and the specific term associated with this part of the grid
            Button detailsButton = (Button)sender;
            Grid courseGrid = (Grid)detailsButton.Parent;

            // Pulling the course ID Label
            Label courseIDLabel = (Label)courseGrid.FindByName("CourseID");

            // Pulling the CourseID
            int courseID = Convert.ToInt32(courseIDLabel.Text);

            // Open the DetailedCourse page
            DetailedCourse dc = new DetailedCourse(courseID, currentTerm);
            await Navigation.PushModalAsync(dc);
        }

        private async void DeleteCourse_Clicked(object sender, EventArgs e)
        {
            bool confirmation = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this " +
                "course?", "Yes", "No");

            if (confirmation)
            {
                // Determining the specific button clicked and the specific term associated with this part of the grid
                Button deleteButton = (Button)sender;
                Grid courseGrid = (Grid)deleteButton.Parent;

                // Pulling the Course ID view and then pulling its data
                Label courseIDEntry = (Label)courseGrid.FindByName("CourseID");
                int courseID = Convert.ToInt32(courseIDEntry.Text);

                // Get the assessments in this course
                var assessments = DatabaseServices.AssessmentsList(courseID);

                foreach (Assessment a in assessments)
                {
                    // Delete the associated assessments
                    DatabaseServices.DeleteAssessment(a.AssessmentID);
                }
                // Delete the course
                DatabaseServices.DeleteCourse(courseID);

                // Refreshing the Collection View (deleted Course should be gone)
                CoursesCV.ItemsSource = null;
                var courses = DatabaseServices.CoursesList(currentTerm);
                CoursesCV.ItemsSource = courses;
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            CoursesCV.ItemsSource = null;
            var courses = DatabaseServices.CoursesList(currentTerm);
            CoursesCV.ItemsSource = courses;
        }
    }
}