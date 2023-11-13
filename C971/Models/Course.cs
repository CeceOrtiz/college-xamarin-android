using System;
using SQLite;
using C971.Services;
using Xamarin.Forms;

namespace C971.Models
{
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int CourseID { get; set; }
        public int TermID { get; set; }     // Foreign key from term
        public string CourseName { get; set; }
        public DateTime CourseStart { get; set; }
        public DateTime CourseEnd { get; set; }
        public bool CourseNotifs { get; set; }
        public string CourseStatus { get; set; }
        public string InstructorName { get; set; }
        public string InstructorPhone { get; set; }
        public string InstructorEmail { get; set; }
        public string CourseNotes { get; set; }

        public static bool CourseCountValidation(int termID)
        {
            int coursesThisTerm = DatabaseServices.CourseCount(termID);

            if (coursesThisTerm == 6)
            {
                Application.Current.MainPage.DisplayAlert("Hit Course Limit",
                    "Terms may only have 6 courses. Please remove a course before adding a new one.", "OK");
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool CourseSaveValidation(string name, DateTime start, DateTime end, 
            string ciName, string ciPhone, string ciEmail, string notes)
        {
            if (name == null)
            {
                Application.Current.MainPage.DisplayAlert("Name Required",
                    "A course name is required. Please enter a name and try again.", "OK");
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
                    if (ciName == null)
                    {
                        Application.Current.MainPage.DisplayAlert("Instructor Name Required",
                        "An instructor name is required. Please enter a name and try again.", "OK");
                        return false;
                    }
                    else
                    {
                        if (ciPhone == null)
                        {
                            Application.Current.MainPage.DisplayAlert("Instructor Phone Required",
                                "An instructor phone number is required. Please enter a number and try again.",
                                "OK");
                            return false;
                        }
                        else
                        {
                            if (ciEmail == null)
                            {
                                Application.Current.MainPage.DisplayAlert("Instructor Email Required",
                                "An instructor email is required. Please enter an email and try again.",
                                "OK");
                                return false;
                            }
                            else
                            {
                                if (notes == null)
                                {
                                    Application.Current.MainPage.DisplayAlert("Notes Required",
                                        "Notes are required. Please fill out the notes field and try again.", "OK");
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
    }
}
