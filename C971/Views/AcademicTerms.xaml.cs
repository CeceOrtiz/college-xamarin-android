using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using C971.Services;
using C971.Models;

namespace C971.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AcademicTerms : ContentPage
    {
        private List<Term> addedTerms = new List<Term>();
        public AcademicTerms()
        {
            InitializeComponent();
            PopulateTerms();
        }
        private void PopulateTerms()
        {
            var terms = DatabaseServices.TermsList();
            TermsCV.ItemsSource = terms;
        }

        private void AddTerm_Clicked(object sender, EventArgs e)
        {
            // Create a new Term
            var newTerm = new Term();

            // Term ID is pulled from the TermsList plus any added terms to ensure there are no repeats
            int maxTermID = DatabaseServices.TermsList().Concat(addedTerms).Max(t => t.TermID);
            newTerm.TermID = maxTermID + 1;
            newTerm.TermName = "New Term";
            newTerm.TermStart = DateTime.Now;
            newTerm.TermEnd = DateTime.Now;

            // Add the term to a temporary list; will only be saved if the user clicks the Save Term button
            addedTerms.Add(newTerm);

            // Refreshing the Collection View
            TermsCV.ItemsSource = null;
            TermsCV.ItemsSource = DatabaseServices.TermsList().Concat(addedTerms);
        }

        private async void TermDetails_Clicked(object sender, EventArgs e)
        {
            // Determining the specific button clicked and the specific term associated with this part of the grid
            Button saveButton = (Button)sender;
            Grid termGrid = (Grid)saveButton.Parent;

            // Pulling the term's views
            Entry termIDEntry = (Entry)termGrid.FindByName("TermID");
            Entry termNameEntry = (Entry)termGrid.FindByName("TermName");
            DatePicker termStartDP = (DatePicker)termGrid.FindByName("TermStartPicker"); // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
            DatePicker termEndDP = (DatePicker)termGrid.FindByName("TermEndPicker"); // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms

            // Values from term's views
            string termID = termIDEntry.Text;
            string termName = termNameEntry.Text;
            string termStart = termStartDP.Date.ToString("d"); // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
            string termEnd = termEndDP.Date.ToString("d"); // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms

            // Determine if the term is saved before moving on
            bool InDatabase = DatabaseServices.TermInDB(Convert.ToInt32(termID));

            if (InDatabase)
            {
                DetailedTerm dt = new DetailedTerm(termID, termName, termStart, termEnd);
                await Navigation.PushModalAsync(dt);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Term Not Saved",
                        "Please save the term before continuing.", "OK");
            }
        }

        private void SaveTerm_Clicked(object sender, EventArgs e)
        {
            // Determining the specific button clicked and the specific term associated with this part of the grid
            Button saveButton = (Button)sender;
            Grid termGrid = (Grid)saveButton.Parent;

            // Pulling the specific views from the Parent
            Entry termIDEntry = (Entry)termGrid.FindByName("TermID");
            Entry termNameEntry = (Entry)termGrid.FindByName("TermName");
            DatePicker termStartPicker = (DatePicker)termGrid.FindByName("TermStartPicker"); // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
            DatePicker termEndPicker = (DatePicker)termGrid.FindByName("TermEndPicker"); // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms

            // Pulling data from the views
            int termID = Convert.ToInt32(termIDEntry.Text);
            string termName = termNameEntry.Text;
            DateTime termStart = termStartPicker.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms
            DateTime termEnd = termEndPicker.Date; // Source: https://learn.microsoft.com/en-us/dotnet/api/Xamarin.Forms.DatePicker?view=xamarin-forms

            // Confirm start is before end
            bool continueSave = Term.TermValidation(termName, termStart, termEnd);

            if (continueSave)
            {
                // Figure out whether it's a new term or an existing term
                bool InDatabase = DatabaseServices.TermInDB(termID);

                if (InDatabase) // Update existing term
                {
                    DatabaseServices.UpdateTerm(termID, termName, termStart, termEnd);
                    Application.Current.MainPage.DisplayAlert("Save successful!", "Term has been saved.", "OK");
                }
                else // Add new term
                {
                    DatabaseServices.AddTerm(termID, termName, termStart, termEnd);
                    addedTerms.RemoveAll(t => t.TermID == termID);

                    // Refreshing the Collection View (new Term should be part of the TermsList now)
                    TermsCV.ItemsSource = null;
                    TermsCV.ItemsSource = DatabaseServices.TermsList().Concat(addedTerms);
                    Application.Current.MainPage.DisplayAlert("Save successful!", "Term has been saved.", "OK");
                }
            }
        }

        private async void DeleteTerm_Clicked(object sender, EventArgs e)
        {
            bool confirmation = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this " +
                "term?", "Yes", "No");

            if (confirmation)
            {
                // Determining the specific button clicked and the specific term associated with this part of the grid
                Button saveButton = (Button)sender;
                Grid termGrid = (Grid)saveButton.Parent;

                // Pulling the Term ID view and then pulling its data
                Entry termIDEntry = (Entry)termGrid.FindByName("TermID");
                int termID = Convert.ToInt32(termIDEntry.Text);

                // Figure out whether it's a new term or an existing term
                bool InDatabase = DatabaseServices.TermInDB(termID);

                if (InDatabase)
                {
                    // Get the courses in this term
                    var courses = DatabaseServices.CoursesList(termID);

                    foreach (Course c in courses)
                    {
                        // Get the assessments in each course of this term
                        var assessments = DatabaseServices.AssessmentsList(c.CourseID);
                        foreach (Assessment a in assessments)
                        {
                            // Delete the associated assessments
                            DatabaseServices.DeleteAssessment(a.AssessmentID);
                        }
                        // Delete the associated courses
                        DatabaseServices.DeleteCourse(c.CourseID);
                    }
                    // Delete the term
                    DatabaseServices.DeleteTerm(termID);

                    // Refreshing the Collection View (deleted Term should be gone)
                    TermsCV.ItemsSource = null;
                    TermsCV.ItemsSource = DatabaseServices.TermsList().Concat(addedTerms);
                }
                else
                {
                    // Removing the temporary term from addedTerms
                    addedTerms.RemoveAll(term => term.TermID == termID);

                    // Refreshing the Collection View (deleted Term should be gone)
                    TermsCV.ItemsSource = null;
                    TermsCV.ItemsSource = DatabaseServices.TermsList().Concat(addedTerms);
                }
                
            }
        }
    }
}