using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.Json;
using System.Collections.Generic;
using WorkoutTracker.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace WorkoutTracker.RealView.Views
{
    public class AdminDashboardView : BaseView
    {
        private FlowLayoutPanel _mainLayout = null!;

        public AdminDashboardView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            _mainLayout = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(40)
            };

            var title = new Label
            {
                Text = "Admin Control Panel",
                Font = UIStyle.Header,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 40)
            };

            _mainLayout.Controls.Add(title);

            AddNavButton("MANAGE EXERCISES", () => AppRuntime.Navigation.NavigateTo<ExerciseManagementView>());
            AddNavButton("MANAGE MUSCLE GROUPS", () => AppRuntime.Navigation.NavigateTo<MuscleGroupManagementView>());
            AddNavButton("MANAGE USERS", () => AppRuntime.Navigation.NavigateTo<UserManagementView>());
            AddNavButton("MANAGE WORKOUT TEMPLATES", () => AppRuntime.Navigation.NavigateTo<WorkoutManagementView>());
            
            var spacer = new Panel { Size = new Size(400, 40) };
            _mainLayout.Controls.Add(spacer);

            AddNavButton("IMPORT DATA (JSON)", async () => await ImportDataAsync(), UIStyle.Success);

            this.Controls.Add(_mainLayout);
        }

        private void AddNavButton(string text, Action onClick, Color? backColor = null)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(400, 60),
                BackColor = backColor ?? UIStyle.SidebarHover,
                ForeColor = UIStyle.TextOnSidebar,
                FlatStyle = FlatStyle.Flat,
                Font = UIStyle.SubHeader,
                Margin = new Padding(0, 0, 0, 20),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            _mainLayout.Controls.Add(btn);
        }

        private async Task ImportDataAsync()
        {
            using (var ofd = new OpenFileDialog { Filter = "JSON files (*.json)|*.json" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var json = File.ReadAllText(ofd.FileName);
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var data = JsonSerializer.Deserialize<ImportModel>(json, options);

                        if (data == null) return;

                        int mgCount = 0;
                        int exCount = 0;

                        // 1. Import Muscle Groups
                        if(data.MuscleGroups.Count != 0)
                        {
                            var existingGroups = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();

                            HashSet<string> groups = new HashSet<string>(data.MuscleGroups);
                            groups.ExceptWith(existingGroups.Select(eg => eg.Name));

                            await AppRuntime.MuscleGroup.BulkCreateAsync(groups);
                            mgCount += groups.Count;
                        }


                        // Refresh groups for exercise mapping

                        if(data.Exercises.Count != 0)
                        {
                            var existingGroups = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();

                            // 2. Import Exercises
                            var existingExercises = await AppRuntime.Exercise.GetAllExercisesAsync();
                            HashSet<Exercise> importExercises = new HashSet<Exercise>(
                                data.Exercises.Select(ee => new Exercise
                                {
                                    Name = ee.Name,
                                    Description = ee.Description,
                                    Instructions = ee.Instructions,
                                    MuscleGroupId = ee.MuscleGroupId
                                }));


                            importExercises.ExceptWith(existingExercises.Select(ee => new Exercise
                            {
                                Name = ee.Name,
                                Description = ee.Description,
                                Instructions = ee.Instructions,
                                MuscleGroupId = ee.MuscleGroupId
                            }));

                            await AppRuntime.Exercise.BulkCreateExercisesAsync(importExercises);
                            exCount += importExercises.Count;
                        }
                        MessageBox.Show($"Import successful!\nCreated {mgCount} Muscle Groups and {exCount} Exercises.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Import failed: {ex.Message}, Fault: {ex.InnerException}");
                    }
                }
            }
        }

        private class ImportModel
        {
            public List<string>? MuscleGroups { get; set; }
            public List<ExerciseImportModel>? Exercises { get; set; }
        }

        private class ExerciseImportModel
        {
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public string? Instructions { get; set; }
            public int MuscleGroupId { get; set; }
        }
    }
}
