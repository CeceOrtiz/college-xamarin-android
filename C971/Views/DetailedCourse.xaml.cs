using System;
using System.Linq;

using Xamarin.Essentials; // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using C971.Services;
using C971.Models;

namespace C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailedCourse : ContentPage
    {
        public int currentCourse;
        public bool newCourse;
        public DetailedCourse(int courseID, int termID)
        {
            InitializeComponent();
            newCourse = DatabaseServices.NewCourse(courseID);
            if (newCourse)
            {
                currentCourse = courseID;
                CourseID.Text = courseID.ToString();
                TermID.Text = termID.ToString();
            }
            else
            {
                currentCourse = courseID;
                Course course = DatabaseServices.PullCourse(courseID);
                CourseID.Text = courseID.ToString();
                TermID.Text = course.TermID.ToString();
                CourseName.Text = course.CourseName;
                CourseStartPicker.Date = course.CourseStart.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
                CourseEndPicker.Date = course.CourseEnd.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
                CourseNotifs.IsToggled = course.CourseNotifs;
                CourseStatus.SelectedItem = course.CourseStatus; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Picker?view=xamarin-forms
                CIName.Text = course.InstructorName;
                CIPhone.Text = course.InstructorPhone;
                CIEmail.Text = course.InstructorEmail;
                CourseNotes.Text = course.CourseNotes;

                var assessments = DatabaseServices.AssessmentsList(courseID);
                AssessmentsCV.ItemsSource = assessments;
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (CourseStatus.SelectedItem == null) // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Picker?view=xamarin-forms
            {
                Application.Current.MainPage.DisplayAlert("Status Required",
                            "A status is required. Please choose a status and try again.", "OK");
                return;
            }

            else
            {
                int termID = Convert.ToInt32(TermID.Text);
                string name = CourseName.Text;
                DateTime start = CourseStartPicker.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
                DateTime end = CourseEndPicker.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
                bool notifs = CourseNotifs.IsToggled;
                string status = CourseStatus.SelectedItem.ToString(); // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Picker?view=xamarin-forms
                string ciName = CIName.Text;
                string ciPhone = CIPhone.Text;
                string ciEmail = CIEmail.Text;
                string notes = CourseNotes.Text;

                bool continueSave = Course.CourseSaveValidation(name, start, end, ciName, ciPhone, ciEmail,
                    notes);

                if (continueSave)
                {
                    if (newCourse)
                    {
                        DatabaseServices.AddCourse(termID, name, start, end, notifs, status, ciName, ciPhone, ciEmail,
                            notes);
                        Application.Current.MainPage.DisplayAlert("Save successful!", "Course has been saved.", "OK");
                    }
                    else
                    {
                        DatabaseServices.UpdateCourse(currentCourse, name, start, end, notifs, status, ciName, ciPhone,
                            ciEmail, notes);
                        Application.Current.MainPage.DisplayAlert("Save successful!", "Course has been saved.", "OK");
                    }
                }
            }
        }

        private async void ShareButton_Clicked(object sender, EventArgs e)
        {
            string courseNotes = CourseNotes.Text;
            string courseName = CourseName.Text;

            await Share.RequestAsync(new ShareTextRequest // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            {
                Text = courseNotes,
                Title = $"{courseName} Notes"
            });
        }

        private async void AddAssessment_Clicked(object sender, EventArgs e)
        {
            // Determine if the course is saved before moving on
            bool InDatabase = DatabaseServices.CourseInDB(currentCourse);

            if (InDatabase)
            {
                bool continueAdd = Assessment.AssessCountValidation(currentCourse);

                if (continueAdd)
                {
                    // CourseID is assigned by finding the current max and adding 1
                    int maxAssessmentID = DatabaseServices.AssessmentsList().Max(t => t.AssessmentID);
                    int newAssessmentID = maxAssessmentID + 1;

                    // Open DetailedAssessment page with defaults
                    DetailedAssessment da = new DetailedAssessment(newAssessmentID, currentCourse);
                    await Navigation.PushModalAsync(da);
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Course Not Saved",
                        "Please save the course before continuing.", "OK");
            }
        }

        private async void AssessmentDetails_Clicked(object sender, EventArgs e)
        {
            // Determining the specific button clicked and the specific term associated with this part of the grid
            Button detailsButton = (Button)sender;
            Grid assessmentGrid = (Grid)detailsButton.Parent;

            // Pulling the assessment ID Label
            Label assessmentIDLabel = (Label)assessmentGrid.FindByName("AssessmentID");

            // Pulling the AssessmentID
            int assessmentID = Convert.ToInt32(assessmentIDLabel.Text);

            // Open the DetailedAssessment page
            DetailedAssessment da = new DetailedAssessment(assessmentID, currentCourse);
            await Navigation.PushModalAsync(da);
        }

        private async void DeleteAssessment_Clicked(object sender, EventArgs e)
        {
            bool confirmation = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this " +
                "assessment?", "Yes", "No");

            if (confirmation)
            {
                // Determining the specific button clicked and the specific term associated with this part of the grid
                Button deleteButton = (Button)sender;
                Grid assessmentGrid = (Grid)deleteButton.Parent;

                // Pulling the assessment ID Label
                Label assessmentIDLabel = (Label)assessmentGrid.FindByName("AssessmentID");

                // Pulling the AssessmentID
                int assessmentID = Convert.ToInt32(assessmentIDLabel.Text);

                // Deleting the assessment and refreshing the assessments list
                DatabaseServices.DeleteAssessment(assessmentID);
                AssessmentsCV.ItemsSource = null;
                var assessments = DatabaseServices.AssessmentsList(currentCourse);
                AssessmentsCV.ItemsSource = assessments;
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            AssessmentsCV.ItemsSource = null;
            var assessments = DatabaseServices.AssessmentsList(currentCourse);
            AssessmentsCV.ItemsSource = assessments;
        }

        private async void ReturnButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}