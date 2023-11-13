using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using C971.Services;
using C971.Models;

namespace C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DetailedAssessment : ContentPage
    {
        public int currentAssessment;
        public bool newAssessment;
        public DetailedAssessment(int assessmentID, int courseID)
        {
            InitializeComponent();
            newAssessment = DatabaseServices.NewAssessment(assessmentID);

            if (newAssessment)
            {
                currentAssessment = assessmentID;
                AssessmentID.Text = assessmentID.ToString();
                CourseID.Text = courseID.ToString();
            }
            else
            {
                currentAssessment = assessmentID;
                Assessment a = DatabaseServices.PullAssessment(assessmentID);
                AssessmentID.Text = assessmentID.ToString();
                CourseID.Text = a.CourseID.ToString();
                AssessmentName.Text = a.AssessmentName;
                AssessmentStartPicker.Date = a.AssessmentStart.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
                AssessmentEndPicker.Date = a.AssessmentEnd.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
                AssessmentNotifs.IsToggled = a.AssessmentNotifs;
                AssessmentType.SelectedItem = a.AssessmentType; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Picker?view=xamarin-forms
            }
        }

        private void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (AssessmentType.SelectedItem == null) // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Picker?view=xamarin-forms
            {
                Application.Current.MainPage.DisplayAlert("Assessment Status Required",
                            "An assessment status is required to continue. Please try again.", "OK");
                return;
            }
            else
            {
                int courseID = Convert.ToInt32(CourseID.Text);
                string name = AssessmentName.Text;
                DateTime start = AssessmentStartPicker.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
                DateTime end = AssessmentEndPicker.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
                bool notifs = AssessmentNotifs.IsToggled;
                string type = AssessmentType.SelectedItem.ToString(); // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.Picker?view=xamarin-forms

                bool continueSave = Assessment.AssessmentSaveValidation(courseID, name, start, end, type,
                    newAssessment);

                if (continueSave)
                {
                    if (newAssessment)
                    {
                        DatabaseServices.AddAssessment(courseID, name, start, end, notifs, type);
                        Application.Current.MainPage.DisplayAlert("Save successful!", "Assessment has been saved.",
                            "OK");
                    }
                    else
                    {
                        DatabaseServices.UpdateAssessment(currentAssessment, name, start, end, notifs, type);
                        Application.Current.MainPage.DisplayAlert("Save successful!", "Assessment has been saved.",
                            "OK");
                    }
                }
            }
        }

        private async void ReturnButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}