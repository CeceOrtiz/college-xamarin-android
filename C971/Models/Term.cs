using System;
using SQLite;
using Xamarin.Forms;

namespace C971.Models
{
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int TermID { get; set; }
        public string TermName { get; set; }
        public DateTime TermStart { get; set; }
        public DateTime TermEnd { get; set; }

        public static bool TermValidation(string name, DateTime start, DateTime end)
        {
            if (name == null)
            {
                Application.Current.MainPage.DisplayAlert("Term Name Required",
                        "A term name is required. Please enter a name and try again.", "OK");
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
                    return true;
                }
            }
        }
    }
}
