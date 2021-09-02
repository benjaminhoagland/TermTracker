using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace TermTracker.DB
{
    public class Table
    {
        public class Data
        {
            [Column("initialized")]
            public bool Initialized { get; set; }
        }
        public class Student
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public string WGUID { get; set; }
            public string DegreeName { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string PersonalEmail { get; set; }
            public string WGUEmail { get; set; }
        }
        public class Term
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public DateTime StartDate { get; set; }
            public bool StartDateNotify { get; set; }
            public DateTime EndDate { get; set; }
            public bool EndDateNotify { get; set; }
            public int TermOrder { get; set; }
        }
        public class Course
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public string Name { get; set; }
            public CourseStatus Status { get; set; }
            public int InstructorID { get; set; }
            public int TermID { get; set; }
            public DateTime StartDate { get; set; }
            public bool StartDateNotify { get; set; }
            public DateTime EndDate { get; set; }
            public bool EndDateNotify { get; set; }

        }
        public class Assessment
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public string Name { get; set; }
            public DateTime DueDate { get; set; }
            public bool DueDateNotify { get; set; }
            public int CourseID { get; set; }
        }
        public class Instructor
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string WGUEmail { get; set; }
        }
        public class Note
        {
            [PrimaryKey, AutoIncrement]
            public int ID { get; set; }
            [Indexed]
            public string Name { get; set; }
            public string Content { get; set; }
            public int CourseID { get; set; }
        }
    }
    public enum CourseStatus
    {
        Plan_To_Take,
        Not_Started,
        In_Progress,
        Completed,
        Dropped
    }
}
