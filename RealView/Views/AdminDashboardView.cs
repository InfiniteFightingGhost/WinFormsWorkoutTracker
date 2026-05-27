using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text.Json;
using System.Collections.Generic;
using Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace RealView.Views
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
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
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

            AddNavButton("IMPORT DATA (JSON)", async () => await ImportDataAsync(), Color.FromArgb(40, 167, 69));

            this.Controls.Add(_mainLayout);
        }

        private void AddNavButton(string text, Action onClick, Color? backColor = null)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(400, 60),
                BackColor = backColor ?? Color.FromArgb(60, 64, 67),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
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
                        var json = System.IO.File.ReadAllText(ofd.FileName);
                        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                        var data = JsonSerializer.Deserialize<ImportModel>(json, options);

                        if (data == null) return;

                        int mgCount = 0;
                        int exCount = 0;

                        // 1. Import Muscle Groups
                        var existingGroups = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();
                        foreach (var mgName in data.MuscleGroups ?? new List<string>())
                        {
                            if (!existingGroups.Any(g => g.Name.Equals(mgName, StringComparison.OrdinalIgnoreCase)))
                            {
                                await AppRuntime.MuscleGroup.CreateMuscleGroupAsync(mgName);
                                mgCount++;
                            }
                        }

                        // Refresh groups for exercise mapping
                        existingGroups = await AppRuntime.MuscleGroup.GetMuscleGroupsAsync();

                        // 2. Import Exercises
                        var existingExercises = await AppRuntime.Exercise.GetAllExercisesAsync();
                        foreach (var exData in data.Exercises ?? new List<ExerciseImportModel>())
                        {
                            if (!existingExercises.Any(e => e.Name.Equals(exData.Name, StringComparison.OrdinalIgnoreCase)))
                            {
                                var mg = existingGroups.FirstOrDefault(g => g.Name.Equals(exData.MuscleGroup, StringComparison.OrdinalIgnoreCase));
                                if (mg != null)
                                {
                                    await AppRuntime.Exercise.CreateExerciseAsync(new Exercise
                                    {
                                        Name = exData.Name,
                                        Description = exData.Description ?? "",
                                        Instructions = exData.Instructions ?? "",
                                        MuscleGroupId = mg.Id
                                    });
                                    exCount++;
                                }
                            }
                        }

                        MessageBox.Show($"Import successful!\nCreated {mgCount} Muscle Groups and {exCount} Exercises.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Import failed: {ex.Message}");
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
            public string MuscleGroup { get; set; } = null!;
        }
    }
}
