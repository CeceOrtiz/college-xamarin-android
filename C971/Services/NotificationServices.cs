using System;
using C971.Models;
using Plugin.LocalNotifications; // Source: https://github.com/edsnider/LocalNotificationsPlugin

namespace C971.Services
{
    public class NotificationServices
    {
        public static void CourseStartNotifs()
        {
            var courses = DatabaseServices.CourseNotifs();
            int i = 1;

            foreach (Course c in courses)
            {
                if (c.CourseStart == DateTime.Today)
                {
                    string courseName = c.CourseName;
                    CrossLocalNotifications.Current.Show("Course Start", $"Your course {courseName} begins today", 
                        i); // Source: https://github.com/edsnider/LocalNotificationsPlugin
                }
                else
                {
                    continue;
                }
                i++;
            }
        }

        public static void CourseEndNotifs()
        {
            var courses = DatabaseServices.CourseNotifs();
            int i = 100;

            foreach (Course c in courses)
            {
                if (c.CourseEnd == DateTime.Today)
                {
                    string courseName = c.CourseName;
                    CrossLocalNotifications.Current.Show("Course Start", $"Your course {courseName} ends today", 
                        i); // Source: https://github.com/edsnider/LocalNotificationsPlugin
                }
                else
                {
                    continue;
                }
                i++;
            }
        }

        public static void AssessmentStartNotifs()
        {
            var assessments = DatabaseServices.AssessmentNotifs();
            int i = 300;

            foreach (Assessment a in assessments)
            {
                if (a.AssessmentStart == DateTime.Today)
                {
                    string assessmentName = a.AssessmentName;
                    CrossLocalNotifications.Current.Show("Assessment Today", $"Your assessment {assessmentName}" +
                        $" is scheduled for today", i); // Source: https://github.com/edsnider/LocalNotificationsPlugin
                }
                else
                {
                    continue;
                }
                i++;
            }
        }

        public static void AssessmentEndNotifs()
        {
            var assessments = DatabaseServices.AssessmentNotifs();
            int i = 200;

            foreach (Assessment a in assessments)
            {
                if  (a.AssessmentEnd == DateTime.Today)
                {
                    string assessmentName = a.AssessmentName;
                    CrossLocalNotifications.Current.Show("Assessment Ending", $"Your assessment " +
                        $"{assessmentName} ends today", i); // Source: https://github.com/edsnider/LocalNotificationsPlugin
                }
                else
                {
                    continue;
                }
                i++;
            }
        }
    }
}
