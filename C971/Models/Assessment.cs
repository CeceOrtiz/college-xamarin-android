using System;
using SQLite;
using C971.Services;
using Xamarin.Forms;

namespace C971.Models
{
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int AssessmentID { get; set; }
        public int CourseID { get; set; }   // Foreign key from course
        public string AssessmentName { get; set; }
        public DateTime AssessmentStart { get; set; }
        public DateTime AssessmentEnd { get; set; }
        public bool AssessmentNotifs { get; set; }
        public string AssessmentType { get; set; }

        public static bool AssessCountValidation(int courseID)
        {
            int assessmentCount = DatabaseServices.AssessmentCount(courseID);
            if (assessmentCount == 2)
            {
                Application.Current.MainPage.DisplayAlert("Hit Assessment Limit",
                    "Courses can only have two assessments. Please delete an assessment before adding a new one.",
                    "OK");
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool AssessmentSaveValidation(int courseID, string name, DateTime start, DateTime end, 
            string type, bool newAssessment)
        {
            if (name == null)
            {
                Application.Current.MainPage.DisplayAlert("Name Required",
                    "An assessment name is required. Please enter a name and try again.", "OK");
                return false;
            }
            else
            {
                if (start > end)
                {
                    Application.Current.MainPage.DisplayAlert("Invalid Dates",
                        "Start date must be before end date. Please try again.", "OK");
                    return false;
                }
                else
                {
                    if (type == "Objective")
                    {
                        int oaCount = DatabaseServices.OACount(courseID);
                        if (oaCount == 1 && newAssessment)
                        {
                            Application.Current.MainPage.DisplayAlert("Assessment Exists",
                            "Only one Objective Assessment allowed per course. Please try again.", "OK");
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        int paCount = DatabaseServices.PACount(courseID);
                        if (paCount == 1 && newAssessment)
                        {
                            Application.Current.MainPage.DisplayAlert("Assessment Exists",
                            "Only one Performance Assessment allowed per course. Please try again.", "OK");
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
        }
    }
}
