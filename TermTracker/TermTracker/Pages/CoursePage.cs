using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using TermTracker.Pages;
using TermTracker.DB;
using SQLite;
using Plugin.LocalNotifications;
using Xamarin.Essentials;


namespace TermTracker
{
    public class CoursePage : ContentPage
    {
        public static int activeCourseID = 1;
        public CoursePage()
        {
            var db = new SQLiteConnection(Constants.DatabasePath);
            var terms = db.Table<Table.Term>();
            var courses = db.Table<Table.Course>();
            var instructors = db.Table<Table.Instructor>();
            var assessments = db.Table<Table.Assessment>();
            var notes = db.Table<Table.Note>();

            StackLayout navCategories = NavCat.CreateStackLayout();

            Style active = (Style)Application.Current.Resources["ActiveTab"];
            Style inactive = (Style)Application.Current.Resources["InactiveTab"];
            Style activeEditField = (Style)Application.Current.Resources["ActiveEditField"];
            Style inactiveEditField = (Style)Application.Current.Resources["InactiveEditField"];

            Button studentButton = new Button { Text = "student view", Style = inactive };
            studentButton.Clicked += OnStudentButtonClicked;
            navCategories.Children.Add(studentButton);
            Button termButton = new Button { Text = "term view", Style = inactive };
            termButton.Clicked += OnTermButtonClicked;
            navCategories.Children.Add(termButton);
            Button courseButton = new Button { Text = "course view", Style = active };
            courseButton.Clicked += OnCourseButtonClicked;
            navCategories.Children.Add(courseButton);

            NavigationPage.SetHasNavigationBar(this, false);

            Style dataButtonStyle = (Style)Application.Current.Resources["DataTab"];
            StackLayout dataButtons = Helper.GenerateDataButtons();
            Button clearAll = new Button { Text = "Clear All", Style =  dataButtonStyle};
            Button revertData = new Button { Text = "Revert Data", Style = dataButtonStyle };
            clearAll.Clicked += OnClearAllClicked;
            revertData.Clicked += OnRevertDataClicked;
            dataButtons.Children.Add(clearAll);
            dataButtons.Children.Add(revertData);
            var courseStack = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };
            
            foreach(var course in courses)
            {
                if (course.ID == activeCourseID)
                {
                    var courseNameLabel = new Label { Text = course.Name, VerticalOptions = LayoutOptions.Center };
                    var courseNameEdit = new Button { Text = "Edit Name", HorizontalOptions = LayoutOptions.EndAndExpand };
                    courseNameEdit.Clicked += async (sender, args) =>
                    {
                        course.Name = await DisplayPromptAsync("Edit Data", "Course Name:", placeholder: course.Name); db.Update(course);
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };
                    var courseNameDelete = new Button { Text = "Delete", HorizontalOptions = LayoutOptions.End };
                    courseNameDelete.Clicked += async (s, e) => {
                        bool answer = await DisplayAlert("Warning", "Permanently delete " + course.Name + "?", "Yes", "No");
                        if (answer) db.Delete(course);
                        await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                    };
                    var courseName = new StackLayout { Style = activeEditField, Children = { courseNameLabel, courseNameEdit, courseNameDelete } };


                    var courseStartDateLabel = new Label { Text = "Start Date: ", VerticalOptions = LayoutOptions.Center };
                    var courseStartDateEdit = new DatePicker { Date = course.StartDate, HorizontalOptions = LayoutOptions.EndAndExpand };
                    var courseStartDate = new StackLayout { Style = inactiveEditField, Children = { courseStartDateLabel, courseStartDateEdit } };
                    courseStartDateEdit.DateSelected += async (sender, e) => {
                        if(courseStartDateEdit.Date <= course.EndDate)
                        {

                        course.StartDate = courseStartDateEdit.Date; db.Update(course);
                        }
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };
                    var courseStartDateNotifyLabel = new Label { Text = "Start Notifications: ", VerticalOptions = LayoutOptions.Center };
                    var courseStartDateNotifyEdit = new Button { Text = course.StartDateNotify.ToString(), HorizontalOptions = LayoutOptions.EndAndExpand };
                    var courseStartDateNotify = new StackLayout { Style = inactiveEditField, Children = { courseStartDateNotifyLabel, courseStartDateNotifyEdit } };
                    courseStartDateNotifyEdit.Clicked += async (sender, e) => {
                        course.StartDateNotify = !course.StartDateNotify; db.Update(course);
                        var id = course.ID * 125513;
                        if ((course.StartDateNotify))
                        {
                            CrossLocalNotifications.Current.Show(course.Name + " starting " + course.StartDate.ToShortDateString(), "Good Luck!", id, course.StartDate.AddDays(-3));
                        }
                        else
                        {
                            CrossLocalNotifications.Current.Cancel(id);
                        }
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };

                    var courseEndDateLabel = new Label { Text = "End Date: ", VerticalOptions = LayoutOptions.Center };
                    var courseEndDateEdit = new DatePicker { Date = course.EndDate, HorizontalOptions = LayoutOptions.EndAndExpand };
                    var courseEndDate = new StackLayout { Style = inactiveEditField, Children = { courseEndDateLabel, courseEndDateEdit } };
                    courseEndDateEdit.DateSelected += async (sender, e) => {
                        if(courseEndDateEdit.Date >= course.StartDate)
                        {

                        course.EndDate = courseEndDateEdit.Date; db.Update(course);
                        }
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };

                    var courseEndDateNotifyLabel = new Label { Text = "End Notifications: ", VerticalOptions = LayoutOptions.Center };
                    var courseEndDateNotifyEdit = new Button { Text = course.EndDateNotify.ToString(), HorizontalOptions = LayoutOptions.EndAndExpand };
                    var courseEndDateNotify = new StackLayout { Style = inactiveEditField, Children = { courseEndDateNotifyLabel, courseEndDateNotifyEdit } };
                    courseEndDateNotifyEdit.Clicked += async (sender, e) => {
                        course.EndDateNotify = !course.EndDateNotify; db.Update(course);
                        var id = course.ID * 125517;
                        if ((course.EndDateNotify))
                        {
                            CrossLocalNotifications.Current.Show(course.Name + " ending " + course.EndDate.ToShortDateString(), "Crunch Time!", id, course.EndDate.AddDays(-3));
                        }
                        else
                        {
                            CrossLocalNotifications.Current.Cancel(id);
                        }
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };

                    var courseStatusLabel = new Label { Text = "Course Status: ", VerticalOptions = LayoutOptions.Center };
                    var courseStatusButton = new Picker { Title = course.Status.ToString(), HorizontalOptions = LayoutOptions.EndAndExpand };
                    foreach (var item in Data.GetStatusNames(db))
                    {

                        courseStatusButton.Items.Add(item);
                    }
                    courseStatusButton.SelectedIndexChanged += async (s, e) => {
                        course.Status = (CourseStatus)courseStatusButton.SelectedIndex;
                        db.Update(course);
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };
                    var courseStatus = new StackLayout { Style = inactiveEditField, Children = { courseStatusLabel, courseStatusButton } };

                    
                    var instructor = db.Table<Table.Instructor>().FirstOrDefault();
                    foreach(var item in instructors)
                    {
                        if(course.InstructorID == item.ID)
                        {
                            instructor = item;
                        }
                    }
                    var courseInstructorLabel = new Label { Text = "Instructor: ", VerticalOptions = LayoutOptions.Center };
                    var courseInstructorEdit = new Picker { Title = instructor.Name, HorizontalOptions = LayoutOptions.EndAndExpand };
                    foreach (var item in instructors)
                    {

                        courseInstructorEdit.Items.Add(item.Name);
                    }
                    courseInstructorEdit.SelectedIndexChanged += async (s, e) => {
                        course.InstructorID = courseInstructorEdit.SelectedIndex;
                        db.Update(course);
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };
                    var courseInstructor = new StackLayout
                    {
                        Style = activeEditField,
                        Children = { courseInstructorLabel, courseInstructorEdit } 
                    };

                    var courseInstructorPhoneLabel = new Label { Text = "Phone: ", VerticalOptions = LayoutOptions.Center };
                    var courseInstructorPhoneEdit = new Button { Text = instructor.PhoneNumber, HorizontalOptions = LayoutOptions.EndAndExpand };
                    var courseInstructorPhone = new StackLayout { Style = inactiveEditField, Children = { courseInstructorPhoneLabel, courseInstructorPhoneEdit } };
                    courseInstructorPhoneEdit.Clicked += async (sender, args) =>
                    {
                        instructor.PhoneNumber = await DisplayPromptAsync("Edit Data", "Instructor Phone Number:", placeholder: instructor.PhoneNumber); db.Update(instructor);
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };

                    var courseInstructorEmailLabel = new Label { Text = "Email: ", VerticalOptions = LayoutOptions.Center };
                    var courseInstructorEmailEdit = new Button { Text = instructor.WGUEmail, HorizontalOptions = LayoutOptions.EndAndExpand };
                    var courseInstructorEmail = new StackLayout { Style = inactiveEditField, Children = { courseInstructorEmailLabel, courseInstructorEmailEdit } };
                    courseInstructorEmailEdit.Clicked += async (sender, args) =>
                    {
                        instructor.WGUEmail = await DisplayPromptAsync("Edit Data", "Instructor email:", placeholder: instructor.WGUEmail); db.Update(instructor);
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };


                    var courseInstructorInfo = new StackLayout
                    {
                        Padding = new Thickness(15, 0, 0, 0),
                        // Margin = new Thickness(5,5,5,5),
                        Children = { courseInstructorPhone, courseInstructorEmail }
                    };
                    var courseTermLabel = new Label { Text = "Term: ", VerticalOptions = LayoutOptions.Center };
                    var courseTermEdit = new Picker { Title = "Term " + course.TermID.ToString(), HorizontalOptions = LayoutOptions.EndAndExpand };
                    foreach(var term in terms)
                    {
                        courseTermEdit.Items.Add("Term " + term.TermOrder.ToString());
                    }
                    courseTermEdit.SelectedIndexChanged += async (s, e) => {
                        course.TermID = courseTermEdit.SelectedIndex + 1;
                        db.Update(course);
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };
                    var courseTerm = new StackLayout { Padding = new Thickness(15, 0, 0, 0), Style = inactiveEditField, Children = { courseTermLabel, courseTermEdit } };

                    var assessmentStack = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };
                    //add assessment button
                    var assessmentNewLabel = new Label { Text = "New Assessment", VerticalOptions = LayoutOptions.Center };
                    var assessmentNewEdit = new Button { Text = "Add", HorizontalOptions = LayoutOptions.EndAndExpand };
                    var assessmentNew = new StackLayout
                    {
                        Style = activeEditField,
                        Padding = new Thickness(15, 0, 0, 0),
                        Children = { assessmentNewLabel, assessmentNewEdit }
                    };
                    assessmentNewEdit.Clicked += async (sender, args) =>
                    {
                        var assessment = new Table.Assessment
                        {
                            Name = "Assessment Name",
                            DueDate = course.EndDate,
                            DueDateNotify = false,
                            CourseID = course.ID
                        };
                        db.Insert(assessment);
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };

                    var addAssessment = new StackLayout
                    {

                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(30, 0, 0, 0),
                        Children = { assessmentNew }
                    };
                    assessmentStack.Children.Add(assessmentNew);
                    foreach (var assessment in assessments)
                    {
                        if(assessment.CourseID != course.ID)
                        {
                            continue;
                        }
                        var courseAssessmentLabel = new Label { Text = "Assessment: " + assessment.Name, VerticalOptions = LayoutOptions.Center};
                        var courseAssessmentEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };
                        courseAssessmentEdit.Clicked += async (sender, args) =>
                        {
                            assessment.Name = await DisplayPromptAsync("Edit Data", "Assessment Name:", placeholder: assessment.Name); db.Update(assessment);
                            await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                        };
                        var courseAssessmentDelete = new Button { Text = "Delete", HorizontalOptions = LayoutOptions.End };
                        courseAssessmentDelete.Clicked += async (s, e) => {
                            bool answer = await DisplayAlert("Warning", "Permanently delete " + assessment.Name + "?", "Yes", "No");
                            if (answer) db.Delete(assessment);
                            await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                        };
                        var courseAssessmentDueDateLabel = new Label { Text = "Due Date", VerticalOptions = LayoutOptions.Center };
                        var courseAssessmentDueDateEdit = new DatePicker { Date = assessment.DueDate, HorizontalOptions = LayoutOptions.EndAndExpand };
                        courseAssessmentDueDateEdit.DateSelected += (sender, e) => {
                            assessment.DueDate = courseAssessmentDueDateEdit.Date; db.Update(assessment);
                        };
                        var courseAssessment = new StackLayout { Style = activeEditField, Children =
                            {
                                courseAssessmentLabel, courseAssessmentEdit, courseAssessmentDelete
                            }
                        };
                        var courseAssessmentDateStack = new StackLayout
                        {
                            Style = inactiveEditField,
                            Padding = new Thickness (15, 0, 0, 0),
                            Children =
                            {
                                courseAssessmentDueDateLabel, courseAssessmentDueDateEdit
                            }
                        };

                        var courseAssessmentNotifyLabel = new Label { Text = "Due Date Notifications: ", VerticalOptions = LayoutOptions.Center };
                        var courseAssessmentNotifyEdit = new Button { Text = assessment.DueDateNotify.ToString(), HorizontalOptions = LayoutOptions.EndAndExpand };
                        var courseAssessmentNotify = new StackLayout { Style = inactiveEditField, Children = { courseAssessmentNotifyLabel, courseAssessmentNotifyEdit } };
                        courseAssessmentNotifyEdit.Clicked += async (sender, e) => {
                            assessment.DueDateNotify = !assessment.DueDateNotify; db.Update(assessment);
                            var id = assessment.ID * 125519;
                            if ((assessment.DueDateNotify))
                            {
                                CrossLocalNotifications.Current.Show(assessment.Name + " is due " + assessment.DueDate.ToShortDateString(), "Crunch Time!", id, assessment.DueDate.AddDays(-3));
                            }
                            else
                            {
                                CrossLocalNotifications.Current.Cancel(id);
                            }
                            await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                        };


                        assessmentStack.Children.Add(courseAssessment);
                        assessmentStack.Children.Add(new StackLayout
                        {
                            Padding = new Thickness(15, 0, 0, 0),
                            Children =
                            {
                                courseAssessmentDateStack, courseAssessmentNotify
                            }
                        });
                    }

                    var noteStack = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };
                    //add note button
                    var noteNewLabel = new Label { Text = "New note", VerticalOptions = LayoutOptions.Center };
                    var noteNewEdit = new Button { Text = "Add", HorizontalOptions = LayoutOptions.EndAndExpand };
                    var noteNew = new StackLayout
                    {
                        Style = activeEditField,
                        Padding = new Thickness(15, 0, 0, 0),
                        Children = { noteNewLabel, noteNewEdit }
                    };
                    noteNewEdit.Clicked += async (sender, args) =>
                    {
                        var note = new Table.Note
                        {
                            Name = "Note Name",
                            Content = "",
                            CourseID = course.ID
                        };
                        db.Insert(note);
                        await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                    };
                    var addNote = new StackLayout
                    {

                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(30, 0, 0, 0),
                        Children = { noteNew }
                    };
                    noteStack.Children.Add(noteNew);
                    foreach (var note in notes)
                    {
                        if (note.CourseID != course.ID)
                        {
                            continue;
                        }
                        var courseNoteLabel = new Label { Text = "Note: " + note.Name, VerticalOptions = LayoutOptions.Center };
                        var courseNoteEdit = new Button { Text = "Edit Name", HorizontalOptions = LayoutOptions.EndAndExpand };
                        courseNoteEdit.Clicked += async (sender, args) =>
                        {
                            note.Name = await DisplayPromptAsync("Edit Data", "Note Name:", placeholder: note.Name); db.Update(note);
                            await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                        };
                        var courseNoteNameDelete = new Button { Text = "Delete", HorizontalOptions = LayoutOptions.End };
                        courseNoteNameDelete.Clicked += async (s, e) => {
                            bool answer = await DisplayAlert("Warning", "Permanently delete " + note.Name + "?", "Yes", "No");
                            if (answer) db.Delete(note);
                            await Navigation.PushAsync(new CoursePage()); Navigation.RemovePage(this);
                        };
                        var courseNoteContentLabel = new Label { Text = "Note Contents: ", VerticalOptions = LayoutOptions.Center };
                        var courseNoteContentEdit = new Editor { Text = note.Content, HorizontalOptions = LayoutOptions.FillAndExpand };
                        courseNoteContentEdit.Completed += (sender, e) => {
                            note.Content = courseNoteContentEdit.Text; db.Update(note);
                        };
                        var courseNote = new StackLayout
                        {
                            Style = activeEditField,
                            Children =
                            {
                                courseNoteLabel, courseNoteEdit, courseNoteNameDelete
                            }
                        };
                        var courseNoteContentStack = new StackLayout
                        {
                            Style = inactiveEditField,
                            Padding = new Thickness(15, 15, 15, 15),
                            Orientation = StackOrientation.Vertical,
                            Children =
                            {
                                courseNoteContentLabel, courseNoteContentEdit
                            }
                        };
                        var shareLabel = new Label { Text = "", VerticalOptions = LayoutOptions.Center };
                        var shareEdit = new Button { Text = "Share", HorizontalOptions = LayoutOptions.EndAndExpand };
                        shareEdit.Clicked += async (sender, args) =>
                        {
                            if(note.Content.Length > 0)
                            {
                                await Share.RequestAsync(new ShareTextRequest
                                {
                                    Text = note.Content,
                                    Title = "Share Text"
                                });
                            }
                        };
                        var courseNoteShare = new StackLayout
                        {
                            Style = activeEditField,
                            Children =
                            {
                                shareLabel, shareEdit
                            }
                        };


                        noteStack.Children.Add(courseNote);
                        noteStack.Children.Add(new StackLayout
                        {
                            Padding = new Thickness(15, 0, 0, 0),
                            Children =
                            {
                                courseNoteContentStack,
                                courseNoteShare
                            }
                        });
                    }

                    var courseDateStack = new StackLayout
                    { 
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(15, 0, 0, 0),
                        Children =
                        {
                            courseStartDate,
                            courseStartDateNotify,
                            courseEndDate,
                            courseEndDateNotify,
                            courseStatus,
                            courseInstructor,
                            courseInstructorInfo,
                            courseTerm,
                            assessmentStack,
                            noteStack
                        }

                    };

                    courseStack.Children.Add(new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(30, 0, 15, 15),
                        Children =
                        {
                            courseName,
                            courseDateStack
            
                        }

                    });
                }
            }

            var courseScroll = new ScrollView { Content = courseStack };
            Content = new StackLayout
            {
                Children = {
                    dataButtons,
                    new Label { Text = "WGU TermTracker - LAP1", HorizontalOptions = LayoutOptions.Center, Padding = new Thickness(0, 10, 0, 5) },
                    navCategories,
                    courseScroll
                }
            };
            


        }
        async void OnClearAllClicked(object sender, EventArgs args)
        {
            var database = new SQLiteConnection(Constants.DatabasePath);
            Data.ClearAll(database);
            await Navigation.PushAsync(new CoursePage());
            Navigation.RemovePage(this);
        }
        async void OnRevertDataClicked(object sender, EventArgs args)
        {
            var database = new SQLiteConnection(Constants.DatabasePath);
            Data.RevertData(database);
            await Navigation.PushAsync(new CoursePage());
            Navigation.RemovePage(this);
        }
        async void OnStudentButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new StudentPage());
            Navigation.RemovePage(this);
        }
        async void OnTermButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new TermPage());
            Navigation.RemovePage(this);
        }
        async void OnCourseButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new CoursePage());
            Navigation.RemovePage(this);
        }
    }

}