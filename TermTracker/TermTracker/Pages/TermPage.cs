using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;
using TermTracker.Pages;
using TermTracker.DB;
using SQLite;

namespace TermTracker
{
    public class TermPage : ContentPage
    {
        public TermPage()
        {
            StackLayout navCategories = NavCat.CreateStackLayout();
            Style active = (Style)Application.Current.Resources["ActiveTab"];
            Style inactive = (Style)Application.Current.Resources["InactiveTab"];
            Style activeEditField = (Style)Application.Current.Resources["ActiveEditField"];
            Style inactiveEditField = (Style)Application.Current.Resources["InactiveEditField"];

            Button studentButton = new Button { Text = "student view", Style = inactive };
            studentButton.Clicked += OnStudentButtonClicked;
            navCategories.Children.Add(studentButton);
            Button termButton = new Button { Text = "term view", Style = active };
            termButton.Clicked += OnTermButtonClicked;
            navCategories.Children.Add(termButton);
            Button courseButton = new Button { Text = "course view", Style = inactive };
            courseButton.Clicked += OnCourseButtonClicked;
            navCategories.Children.Add(courseButton);

            NavigationPage.SetHasNavigationBar(this, false);

            Style dataButtonStyle = (Style)Application.Current.Resources["DataTab"];
            StackLayout dataButtons = Helper.GenerateDataButtons();
            Button clearAll = new Button { Text = "Clear All", Style = dataButtonStyle };
            Button revertData = new Button { Text = "Revert Data", Style = dataButtonStyle };
            clearAll.Clicked += OnClearAllClicked;
            revertData.Clicked += OnRevertDataClicked;
            dataButtons.Children.Add(clearAll);
            dataButtons.Children.Add(revertData);

            var db = new SQLiteConnection(Constants.DatabasePath);
            var terms = db.Table<Table.Term>();
            var courses = db.Table<Table.Course>();

        

            var termStack = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };
            try
            {
                
                var termNameLabelNew = new Label { Text = "New Term", VerticalOptions = LayoutOptions.Center };
                var termNameEditNew = new Button { Text = "Add", HorizontalOptions = LayoutOptions.EndAndExpand };
                var termNameNew = new StackLayout { Style = activeEditField,
                    Padding = new Thickness(15, 0, 0, 0),
                    Children = { termNameLabelNew, termNameEditNew } };
                termNameEditNew.Clicked += async (sender, args) =>
                {
                    var lastTerm = terms.Last();
                    var term = new Table.Term
                    {
                        StartDate = lastTerm.EndDate.AddDays(1),
                        StartDateNotify = true,
                        EndDate = lastTerm.EndDate.AddMonths(6),
                        EndDateNotify = true,
                        TermOrder = terms.ToList().Count + 1
                    };
                    db.Insert(term);
                    await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                };
                var addTerm = new StackLayout {
              
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(15, 0, 15, 0),
                    Children = { termNameNew }
                };
                termStack.Children.Add(addTerm);
                

                foreach (var term in terms)
                {
                    var thisTerm = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };

                    var termNameLabel = new Label { Text = "Term: " + term.TermOrder, VerticalOptions = LayoutOptions.Center };
                    var termNameEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };
                    var termNameDelete = new Button { Text = "Delete", HorizontalOptions = LayoutOptions.End };
                    termNameDelete.Clicked += async (s, e) => {
                        bool answer = await DisplayAlert("Warning", "Permanently delete Term " + term.TermOrder + "?", "Yes", "No");
                        if (answer) db.Delete(term);
                        await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                    };
                    var termName = new StackLayout { Style = activeEditField, Children = { termNameLabel, termNameEdit, termNameDelete } };
                    termNameEdit.Clicked += async (sender, args) =>
                    {
                        string result = await DisplayPromptAsync("Edit Data", "Term Number:", placeholder: term.TermOrder.ToString());
                        int parsed; int.TryParse(result, out parsed);
                        if(parsed != 0)
                        { 
                        term.TermOrder = parsed; db.Update(term);
                        termNameLabel.Text = "Term: " + parsed;
                        }
                        else await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                    };
                    
                    var startDateLabel = new Label { Text = "Start Date: ", VerticalOptions = LayoutOptions.Center };
                    var startDateEdit = new DatePicker { Date = term.StartDate, HorizontalOptions = LayoutOptions.EndAndExpand };
                    var startDate = new StackLayout { Style = inactiveEditField, Children = { startDateLabel, startDateEdit } };
                    startDateEdit.DateSelected += async (sender, e) => {
                        if(startDateEdit.Date <= term.EndDate)
                        {

                        term.StartDate = startDateEdit.Date; db.Update(term);
                        }
                        await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                    };

                    var endDateLabel = new Label { Text = "End Date: ", VerticalOptions = LayoutOptions.Center };
                    var endDateEdit = new DatePicker { Date = term.EndDate, HorizontalOptions = LayoutOptions.EndAndExpand };
                    var endDate = new StackLayout { Style = inactiveEditField, Children = { endDateLabel, endDateEdit } };
                    endDateEdit.DateSelected += async (sender, e) => {
                        if(endDateEdit.Date >= term.StartDate)
                        {

                        term.EndDate = endDateEdit.Date; db.Update(term);
                        }
                        await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                    };


                    var termDateStack = new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(15, 0, 0, 0),
                        Children =
                            {
                                startDate,
                                endDate
                            }
                    };
                    


                    var courseStack = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };

                    // add course button
                    var courseNameLabelNew = new Label { Text = "New Course", VerticalOptions = LayoutOptions.Center };
                    var courseNameEditNew = new Button { Text = "Add", HorizontalOptions = LayoutOptions.EndAndExpand };
                    var courseNameNew = new StackLayout
                    {
                        Style = activeEditField,
                        Padding = new Thickness(15, 0, 0, 0),
                        Children = { courseNameLabelNew, courseNameEditNew }
                    };
                    courseNameEditNew.Clicked += async (sender, args) =>
                    {
                        var course = new Table.Course
                        {
                            Name = "Course ",
                            Status = CourseStatus.Plan_To_Take,
                            InstructorID = 1,
                            TermID = term.ID,
                            StartDate = term.StartDate,
                            StartDateNotify = true,
                            EndDate = term.EndDate,
                            EndDateNotify = true
                        };
                        db.Insert(course);
                        course.Name = course.Name + course.ID;
                        db.Update(course);
                        await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                    };

                    var addCourse = new StackLayout
                    {

                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(30, 0, 0, 0),
                        Children = { courseNameNew }
                    };
                    courseStack.Children.Add(addCourse);

                    // populate courseStack
                    foreach (var course in courses)
                    {
                        if(course.TermID == term.ID)
                        {
                            var courseNameLabel = new Label { Text = course.Name, VerticalOptions = LayoutOptions.Center };
                            var courseNameEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };
                            courseNameEdit.Clicked += async (s, e) => {
                                CoursePage.activeCourseID = course.ID;
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
                                if (courseStartDateEdit.Date <= course.EndDate)
                                {

                                    course.StartDate = courseStartDateEdit.Date; db.Update(course);
                                }
                                await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                            };

                            var courseEndDateLabel = new Label { Text = "End Date: ", VerticalOptions = LayoutOptions.Center };
                            var courseEndDateEdit = new DatePicker { Date = course.EndDate, HorizontalOptions = LayoutOptions.EndAndExpand };
                            var courseEndDate = new StackLayout { Style = inactiveEditField, Children = { courseEndDateLabel, courseEndDateEdit } };
                            courseEndDateEdit.DateSelected += async (sender, e) => {
                                if(courseEndDateEdit.Date >= course.StartDate)
                                {

                                course.EndDate = courseEndDateEdit.Date; db.Update(course);
                                }
                                await Navigation.PushAsync(new TermPage()); Navigation.RemovePage(this);
                            };

                            var courseDateStack = new StackLayout
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.Center,
                                Padding = new Thickness(15, 0, 0, 0),
                                Children =
                                {
                                    courseStartDate,
                                    courseEndDate
                                }

                            };

                            courseStack.Children.Add(new StackLayout
                            {
                                HorizontalOptions = LayoutOptions.FillAndExpand,
                                VerticalOptions = LayoutOptions.Center,
                                Padding = new Thickness(30, 0, 0, 0),
                                Children =
                                {
                                    courseName,
                                    courseDateStack
                                }

                            });

                            var courseName_tap = new TapGestureRecognizer();
                            courseName_tap.Tapped += (s, e) =>
                            {
                                courseDateStack.IsVisible = !courseDateStack.IsVisible;
                            };
                            courseName.GestureRecognizers.Add(courseName_tap);
                        }

                    }

                    thisTerm.Children.Add(new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(15, 0, 15, 0),
                        Children =
                        {
                            termName,
                            termDateStack,
                            courseStack
                        }

                    });

                    var termName_tap = new TapGestureRecognizer();
                    termName_tap.Tapped += (s, e) =>
                    {
                        termDateStack.IsVisible = !termDateStack.IsVisible;
                        courseStack.IsVisible = !courseStack.IsVisible;
                    };
                    termName.GestureRecognizers.Add(termName_tap);

                    termStack.Children.Add(thisTerm);
                }


            }
            catch
            {
                Label nameLabel = new Label { Text = "Term data not found ", VerticalOptions = LayoutOptions.Center };
                Button nameEdit = new Button { Text = "Add Term", HorizontalOptions = LayoutOptions.EndAndExpand };
            }

            var termScroll = new ScrollView
            {
                Content = termStack
            };

            Content = new StackLayout
            {
                Children =
                {
                    dataButtons,
                    new Label { Text = "WGU TermTracker - LAP1", HorizontalOptions = LayoutOptions.Center, Padding = new Thickness(0, 10, 0, 5) },
                    navCategories,
                    termScroll
                }
            };
        }
        async void OnClearAllClicked(object sender, EventArgs args)
        {
            var database = new SQLiteConnection(Constants.DatabasePath);
            Data.ClearAll(database);
            await Navigation.PushAsync(new TermPage());
            Navigation.RemovePage(this);
        }
        async void OnRevertDataClicked(object sender, EventArgs args)
        {
            var database = new SQLiteConnection(Constants.DatabasePath);
            Data.RevertData(database);
            await Navigation.PushAsync(new TermPage());
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