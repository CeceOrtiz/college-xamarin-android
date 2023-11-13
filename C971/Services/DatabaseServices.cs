using System;
using System.Collections.Generic;
using System.IO;
using SQLite;
using Xamarin.Essentials; // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
using C971.Models;

namespace C971.Services
{
    public static class DatabaseServices
    {
        public static SQLiteConnection dbConn;

        #region Database Initialization & Samples
        public static void DBInit()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C971.db");

            bool isNewDatabase = !File.Exists(dbPath);
            dbConn = new SQLiteConnection(dbPath);

            if (isNewDatabase)
            {
                dbConn.CreateTable<Term>(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
                dbConn.CreateTable<Course>(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
                dbConn.CreateTable<Assessment>(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
                LoadSampleData();
            }
        }
        public static void LoadSampleData()
        {
            Term term1 = new Term
            {
                TermID = 1,
                TermName = "Summer 2023",
                TermStart = Convert.ToDateTime("05/17/2023"),
                TermEnd = Convert.ToDateTime("09/30/2023")
            };
            dbConn.Insert(term1); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/

            Course course1 = new Course
            {
                CourseID = 1,
                TermID = 1,
                CourseName = "C971",
                CourseStart = Convert.ToDateTime("05/17/2023"),
                CourseEnd = Convert.ToDateTime("09/30/2023"),
                CourseNotifs = true,
                CourseStatus = "Plan to take",
                InstructorName = "Lacey Ortiz",
                InstructorPhone = "123-456-7890",
                InstructorEmail = "l.ortiz@pretendemail.com",
                CourseNotes = "First course of the term"
            };
            dbConn.Insert(course1); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/

            Assessment oa = new Assessment
            {
                AssessmentID = 1,
                CourseID = 1,
                AssessmentName = "Understanding Xamarin",
                AssessmentStart = Convert.ToDateTime("05/17/2023"),
                AssessmentEnd = Convert.ToDateTime("05/17/2023"),
                AssessmentNotifs = true,
                AssessmentType = "Objective"
            };
            dbConn.Insert(oa); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/

            Assessment pa = new Assessment
            {
                AssessmentID = 2,
                CourseID = 1,
                AssessmentName = "Build a Mobile App",
                AssessmentStart = Convert.ToDateTime("05/17/2023"),
                AssessmentEnd = Convert.ToDateTime("09/30/2023"),
                AssessmentNotifs = true,
                AssessmentType = "Performance"
            };
            dbConn.Insert(pa); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
        }
        public static void ClearSampleData()
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "C971_V2.db");
            dbConn = new SQLiteConnection(dbPath);

            dbConn.DropTable<Term>(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            dbConn.DropTable<Course>(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            dbConn.DropTable<Assessment>(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            dbConn = null;
        }

        #endregion

        #region Term Methods
        public static void AddTerm(int termID, string termName, DateTime termStart, DateTime termEnd)
        {
            Term term = new Term()
            {
                TermID = termID,
                TermName = termName,
                TermStart = termStart,
                TermEnd = termEnd
            };

            dbConn.Insert(term); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
        }
        public static IEnumerable<Term> TermsList()
        {
            var terms = dbConn.Table<Term>().ToList(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return terms;
        }
        public static void UpdateTerm(int termID, string termName, DateTime termStart, DateTime termEnd)
        {
            var termQuery = dbConn.Table<Term>().Where(i => i.TermID == termID).FirstOrDefault(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/

            if (termQuery != null)
            {
                termQuery.TermName = termName;
                termQuery.TermStart = termStart;
                termQuery.TermEnd = termEnd;

                dbConn.Update(termQuery); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            }
        }
        public static void DeleteTerm(int termID)
        {
            dbConn.Delete<Term>(termID); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
        }

        #endregion

        #region Course Methods
        public static void AddCourse(int termID, string courseName, DateTime courseStart, DateTime courseEnd,
            bool courseNotifs, string courseStatus, string instructorName, string instructorPhone,
            string instructorEmail, string courseNotes)
        {
            Course course = new Course()
            {
                TermID = termID,
                CourseName = courseName,
                CourseStart = courseStart,
                CourseEnd = courseEnd,
                CourseNotifs = courseNotifs,
                CourseStatus = courseStatus,
                InstructorName = instructorName,
                InstructorPhone = instructorPhone,
                InstructorEmail = instructorEmail,
                CourseNotes = courseNotes
            };

            dbConn.Insert(course); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
        }
        public static IEnumerable<Course> CoursesList(int termID)
        {
            var courses = dbConn.Table<Course>().Where(i => i.TermID == termID).ToList(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return courses;
        }
        public static IEnumerable<Course> CoursesList()
        {
            var courses = dbConn.Table<Course>().ToList(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return courses;
        }
        public static bool NewCourse(int courseID)
        {
            var course = dbConn.Table<Course>().Where(i => i.CourseID == courseID).FirstOrDefault(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            if (course == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Course PullCourse(int courseID)
        {
            Course course = dbConn.Table<Course>().Where(i => i.CourseID == courseID).FirstOrDefault(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return course;
        }
        public static void UpdateCourse(int courseID, string courseName, DateTime courseStart,
            DateTime courseEnd, bool courseNotifs, string courseStatus, string instructorName,
            string instructorPhone, string instructorEmail, string courseNotes)
        {
            var courseQuery = dbConn.Table<Course>().Where(i => i.CourseID == courseID).FirstOrDefault(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/

            if (courseQuery != null)
            {
                courseQuery.CourseName = courseName;
                courseQuery.CourseStart = courseStart;
                courseQuery.CourseEnd = courseEnd;
                courseQuery.CourseNotifs = courseNotifs;
                courseQuery.CourseStatus = courseStatus;
                courseQuery.InstructorName = instructorName;
                courseQuery.InstructorPhone = instructorPhone;
                courseQuery.InstructorEmail = instructorEmail;
                courseQuery.CourseNotes = courseNotes;

                dbConn.Update(courseQuery); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            }
        }
        public static void DeleteCourse(int courseID)
        {
            dbConn.Delete<Course>(courseID); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
        }

        #endregion

        #region Assessment Methods
        public static void AddAssessment(int courseID, string assessmentName, DateTime assessmentStart,
            DateTime assessmentEnd, bool assessmentNotifs, string assessmentType)
        {
            Assessment assessment = new Assessment()
            {
                CourseID = courseID,
                AssessmentName = assessmentName,
                AssessmentStart = assessmentStart,
                AssessmentEnd = assessmentEnd,
                AssessmentNotifs = assessmentNotifs,
                AssessmentType = assessmentType
            };

            dbConn.Insert(assessment); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
        }
        public static IEnumerable<Assessment> AssessmentsList(int courseID)
        {
            var assessments = dbConn.Table<Assessment>().Where(i => i.CourseID == courseID).ToList(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return assessments;
        }
        public static IEnumerable<Assessment> AssessmentsList()
        {
            var assessments = dbConn.Table<Assessment>().ToList(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return assessments;
        }
        public static bool NewAssessment(int assessmentID)
        {
            var assessment = dbConn.Table<Assessment>().Where(i => i.AssessmentID == assessmentID)
                .FirstOrDefault(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            if (assessment == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static Assessment PullAssessment(int assessmentID)
        {
            Assessment assessment = dbConn.Table<Assessment>()
                .Where(i => i.AssessmentID == assessmentID).FirstOrDefault(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return assessment;
        }
        public static void UpdateAssessment(int assessmentID, string assessmentName,
            DateTime assessmentStart, DateTime assessmentEnd, bool assessmentNotifs, string assessmentType)
        {
            var assessmentQuery = dbConn.Table<Assessment>().Where(i => i.AssessmentID == assessmentID)
                .FirstOrDefault(); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/

            if (assessmentQuery != null)
            {
                assessmentQuery.AssessmentName = assessmentName;
                assessmentQuery.AssessmentStart = assessmentStart;
                assessmentQuery.AssessmentEnd = assessmentEnd;
                assessmentQuery.AssessmentNotifs = assessmentNotifs;
                assessmentQuery.AssessmentType = assessmentType;

                dbConn.Update(assessmentQuery); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            }
        }
        public static void DeleteAssessment(int assessmentID)
        {
            dbConn.Delete<Assessment>(assessmentID); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
        }

        #endregion

        #region Count Methods
        public static int TermCount()
        {
            int termCount = dbConn.ExecuteScalar<int>("SELECT * FROM Term"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return termCount;
        }
        public static bool TermInDB(int termID)
        {
            int termCount = dbConn.ExecuteScalar<int>($"SELECT * FROM Term WHERE TermID = {termID}"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return termCount > 0;
        }
        public static int CourseCount(int selectedTermID)
        {
            int courseCount = dbConn.ExecuteScalar<int>($"SELECT * FROM Course " +
                $"WHERE TermID = {selectedTermID}"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return courseCount;
        }
        public static bool CourseInDB(int courseID)
        {
            int courseCount = dbConn.ExecuteScalar<int>($"SELECT * FROM Course " +
                $"WHERE CourseID = {courseID}"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return courseCount > 0;
        }
        public static int AssessmentCount(int selectedCourseID)
        {
            int assessmentCount = dbConn.ExecuteScalar<int>($"SELECT * FROM Assessment " +
                $"WHERE CourseID = {selectedCourseID}"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return assessmentCount;
        }
        public static int OACount(int selectedCourseID)
        {
            int oaCount = dbConn.ExecuteScalar<int>($"SELECT * FROM Assessment " +
                $"WHERE CourseID = {selectedCourseID} AND AssessmentType = 'Objective'"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return oaCount;
        }
        public static int PACount(int selectedCourseID)
        {
            int paCount = dbConn.ExecuteScalar<int>($"SELECT * FROM Assessment " +
                $"WHERE CourseID = {selectedCourseID} AND AssessmentType = 'Performance'"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return paCount;
        }

        #endregion

        #region Notifications Settings Methods
        public static IEnumerable<Course> CourseNotifs()
        {
            var coursesWithNotifs = dbConn.Query<Course>("SELECT * FROM Course WHERE CourseNotifs = 1"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return coursesWithNotifs;
        }
        public static IEnumerable<Assessment> AssessmentNotifs()
        {
            var assessmentsWithNotifs = dbConn.Query<Assessment>("SELECT * FROM Assessment " +
                "WHERE AssessmentNotifs = 1"); // Source: https://learn.microsoft.com/en-us/xamarin/essentials/
            return assessmentsWithNotifs;
        }

        #endregion
    }
}
