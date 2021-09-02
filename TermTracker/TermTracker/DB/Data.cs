using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TermTracker.DB
{
    public class Data
    {
        private static int notificationID = 0;
        public static int GetNotifyID()
        {
            notificationID += 1;
            return notificationID;
        }
        public static List<String> GetInstructorNames(SQLiteConnection database)
        {
            var returnList = new List<String>(); 
            var instructors = database.Table<Table.Instructor>();
            foreach(var instructor in instructors)
            {
                returnList.Add(instructor.Name);
            }
            return returnList;
        }
        public static List<String> GetStatusNames(SQLiteConnection database)
        {
            return new List<string>(Enum.GetNames(typeof(CourseStatus)));
        }

        
        public static void Populate(SQLiteConnection database)
        {

            var student = new Table.Student
            {
                WGUID = "00123456789",
                Name = "Benjamin Hoagland",
                DegreeName = "Bachelor of Science Software Development",
                PhoneNumber = "716-255-9569",
                PersonalEmail = "bhoagl1@wgu.edu",
                WGUEmail = "bhoagl1@wgu.edu"
            };
            database.Insert(student);

            var term = new Table.Term
            {
                StartDate = new DateTime(2021, 01, 01),
                StartDateNotify = false,
                EndDate = new DateTime(2021, 06, 30),
                EndDateNotify = false,
                TermOrder = 1
            };
            database.Insert(term);

            var instructor = new Table.Instructor
            {
                Name = "Benjamin Hoagland",
                PhoneNumber = "716-255-9569",
                WGUEmail = "bhoagl1@wgu.edu"
            };
            database.Insert(instructor);

            var course = new Table.Course
            {
                Name = "CRS# - Course 1",
                Status = CourseStatus.Completed,
                InstructorID = 1,
                TermID = 1,
                StartDate = term.StartDate,
                StartDateNotify = false,
                EndDate = term.EndDate,
                EndDateNotify = false
            };
            database.Insert(course);

            var assessment = new Table.Assessment
            {
                Name = "OAX# - First Test",
                DueDate = course.EndDate,
                DueDateNotify = false,
                CourseID = 1

            };
            database.Insert(assessment);
            assessment = new Table.Assessment
            {
                Name = "PAX# - Second Project",
                DueDate = course.EndDate,
                DueDateNotify = false,
                CourseID = 1

            };
            database.Insert(assessment);

            course = new Table.Course
            {
                Name = "CRS# - Course 2",
                Status = CourseStatus.Completed,
                InstructorID = 1,
                TermID = 1,
                StartDate = term.StartDate,
                StartDateNotify = false,
                EndDate = term.EndDate,
                EndDateNotify = false
            };
            database.Insert(course);
            course = new Table.Course
            {
                Name = "CRS# - Course 3",
                Status = CourseStatus.Completed,
                InstructorID = 1,
                TermID = 1,
                StartDate = term.StartDate,
                StartDateNotify = false,
                EndDate = term.EndDate,
                EndDateNotify = false
            };
            database.Insert(course);
            course = new Table.Course
            {
                Name = "CRS# - Course 4",
                Status = CourseStatus.Completed,
                InstructorID = 1,
                TermID = 1,
                StartDate = term.StartDate,
                StartDateNotify = false,
                EndDate = term.EndDate,
                EndDateNotify = false
            };
            database.Insert(course);
            course = new Table.Course
            {
                Name = "CRS# - Course 5",
                Status = CourseStatus.Completed,
                InstructorID = 1,
                TermID = 1,
                StartDate = term.StartDate, // could also be new Date(2020, 12, 01);
                StartDateNotify = false,
                EndDate = term.EndDate,
                EndDateNotify = false
            };
            database.Insert(course);
            course = new Table.Course
            {
                Name = "CRS# - Course 6",
                Status = CourseStatus.Completed,
                InstructorID = 1,
                TermID = 1,
                StartDate = term.StartDate,
                StartDateNotify = false,
                EndDate = term.EndDate,
                EndDateNotify = false
            };
            database.Insert(course);



            var data = new Table.Data
            {
                Initialized = true
            };
            database.Insert(data);

        }
        
        public static void CreateTables(SQLiteConnection database)
        {
            database.CreateTable<Table.Data>();
            database.CreateTable<Table.Student>();
            database.CreateTable<Table.Term>();
            database.CreateTable<Table.Course>();
            database.CreateTable<Table.Assessment>();
            database.CreateTable<Table.Instructor>();
            database.CreateTable<Table.Note>();
        }
        public static void ClearAll(SQLiteConnection database)
        {
            try
            {
                database.DeleteAll<Table.Data>();
                database.DeleteAll<Table.Student>();
                database.DeleteAll<Table.Course>();
                database.DeleteAll<Table.Term>();
                database.DeleteAll<Table.Assessment>();
                database.DeleteAll<Table.Instructor>();
                database.DeleteAll<Table.Note>();

                database.DropTable<Table.Data>();
                database.DropTable<Table.Student>();
                database.DropTable<Table.Term>();
                database.DropTable<Table.Course>();
                database.DropTable<Table.Assessment>();
                database.DropTable<Table.Instructor>();
                database.DropTable<Table.Note>();
            }
            catch {}
        }
        public static void RevertData(SQLiteConnection database)
        {
            ClearAll(database);
            CreateTables(database);
            Populate(database);
        }
        public static bool NotInitialized(SQLiteConnection database)
        {
            try
            {
                var data = database.Table<Table.Data>();

                if (data.Count() == 0) return true;

                foreach (var item in data)
                {
                    if (item.Initialized == true) return false;
                }
            }
            catch
            {
                return true;
            }
            return true;
        }
    }
}
