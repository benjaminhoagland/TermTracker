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
    public partial class StudentPage : ContentPage
    {
        public StudentPage()
        {
            StackLayout navCategories = NavCat.CreateStackLayout();

            Style active = (Style)Application.Current.Resources["ActiveTab"];
            Style inactive = (Style)Application.Current.Resources["InactiveTab"];
            Style activeEditField = (Style)Application.Current.Resources["ActiveEditField"];
            Style inactiveEditField = (Style)Application.Current.Resources["InactiveEditField"];

            Button studentButton = new Button { Text = "student view", Style = active };
            studentButton.Clicked += OnstudentButtonClicked;
            navCategories.Children.Add(studentButton);
            Button termButton = new Button { Text = "term view", Style = inactive };
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
            var students = db.Table<Table.Student>();


            StackLayout studentStack = new StackLayout { HorizontalOptions = LayoutOptions.FillAndExpand };
            try
            {
                foreach (var student in students)
                {
                    Label nameLabel = new Label { Text = "Student Name:\n".Replace("\n", System.Environment.NewLine) + student.Name, VerticalOptions = LayoutOptions.Center };
                    Button nameEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };

                    Label phoneLabel = new Label { Text = "Student Phone Number:\n".Replace("\n", System.Environment.NewLine) + student.PhoneNumber, VerticalOptions = LayoutOptions.Center };
                    Button phoneEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };

                    Label personsalEmailLabel = new Label { Text = "Personal Email:\n".Replace("\n", System.Environment.NewLine) + student.PersonalEmail, VerticalOptions = LayoutOptions.Center };
                    Button personalEmailEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };

                    Label wguEmailLabel = new Label { Text = "WGU Email:\n".Replace("\n", System.Environment.NewLine) + student.WGUEmail, VerticalOptions = LayoutOptions.Center };
                    Button wguEmailEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };

                    Label degreeLabel = new Label { Text = "Degree Program:\n".Replace("\n", System.Environment.NewLine) + student.DegreeName, VerticalOptions = LayoutOptions.Center };
                    Button degreeEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };

                    Label wguIDLabel = new Label { Text = "Student ID:\n".Replace("\n", System.Environment.NewLine) + student.WGUID, VerticalOptions = LayoutOptions.Center };
                    Button wguIDEdit = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.EndAndExpand };

                    studentStack.Children.Add(new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(15, 0, 15, 0),
                        Children =
                        {
                            new StackLayout { Style = activeEditField, Children = { nameLabel, nameEdit } }
                        }

                    });
                    studentStack.Children.Add(new StackLayout
                    {
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Center,
                        Padding = new Thickness(30, 0, 15, 0),
                        Children =
                        {
                            new StackLayout { Style = inactiveEditField, Children = { degreeLabel, degreeEdit } },
                            new StackLayout { Style = inactiveEditField, Children = { wguIDLabel, wguIDEdit } },
                            new StackLayout { Style = inactiveEditField, Children = { wguEmailLabel, wguEmailEdit } },
                            new StackLayout { Style = inactiveEditField, Children = { personsalEmailLabel, personalEmailEdit } },
                            new StackLayout { Style = inactiveEditField, Children = { phoneLabel, phoneEdit } }
                        }

                    });

                    nameEdit.Clicked += async (sender, args) =>
                    {
                        student.Name = await DisplayPromptAsync("Edit Data", "Student Name:", placeholder: student.Name); db.Update(student);
                        await Navigation.PushAsync(new StudentPage()); Navigation.RemovePage(this);
                    };
                    phoneEdit.Clicked += async (sender, args) =>
                    {
                        student.PhoneNumber = await DisplayPromptAsync("Edit Data", "Student Phone:", placeholder: student.PhoneNumber); db.Update(student);
                        await Navigation.PushAsync(new StudentPage()); Navigation.RemovePage(this);
                    };
                    personalEmailEdit.Clicked += async (sender, args) =>
                    {
                        student.PersonalEmail = await DisplayPromptAsync("Edit Data", "Personal Email:", placeholder: student.PersonalEmail); db.Update(student);
                        await Navigation.PushAsync(new StudentPage()); Navigation.RemovePage(this);
                    };
                    wguEmailEdit.Clicked += async (sender, args) =>
                    {
                        student.WGUEmail = await DisplayPromptAsync("Edit Data", "Student WGU Email:", placeholder: student.WGUEmail); db.Update(student);
                        await Navigation.PushAsync(new StudentPage()); Navigation.RemovePage(this);
                    };
                    degreeEdit.Clicked += async (sender, args) =>
                    {
                        student.DegreeName = await DisplayPromptAsync("Edit Data", "Student Degree:", placeholder: student.DegreeName); db.Update(student);
                        await Navigation.PushAsync(new StudentPage()); Navigation.RemovePage(this);
                    };
                    wguIDEdit.Clicked += async (sender, args) =>
                    {
                        student.WGUID = await DisplayPromptAsync("Edit Data", "Student ID:", placeholder: student.WGUID); db.Update(student);
                        await Navigation.PushAsync(new StudentPage()); Navigation.RemovePage(this);
                    };
                };
            }
            catch {}


            Content = new StackLayout
            {
                Children = {
                    dataButtons,
                    new Label { Text = "WGU TermTracker - LAP1", HorizontalOptions = LayoutOptions.Center, Padding = new Thickness(0, 10, 0, 5) },
                    navCategories,
                    studentStack
                }
            };
        }
        async void OnClearAllClicked(object sender, EventArgs args)
        {
            var database = new SQLiteConnection(Constants.DatabasePath);
            Data.ClearAll(database);
            await Navigation.PushAsync(new StudentPage());
            Navigation.RemovePage(this);
        }
        async void OnRevertDataClicked(object sender, EventArgs args)
        {
            var database = new SQLiteConnection(Constants.DatabasePath);
            Data.RevertData(database);
            await Navigation.PushAsync(new StudentPage());
            Navigation.RemovePage(this);
        }
        async void OnstudentButtonClicked(object sender, EventArgs args)
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