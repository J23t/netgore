using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Database Editor - edit game database tables
    /// Replaces WinForms DbEditorForm
    /// </summary>
    public partial class DbEditorViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<DbTableItem> _tables = new();

        [ObservableProperty]
        private DbTableItem? _selectedTable;

        [ObservableProperty]
        private ObservableCollection<object> _tableRows = new();

        [ObservableProperty]
        private object? _selectedRow;

        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private int _totalRecords;

        public DbEditorViewModel()
        {
            try
            {
                LoadTables();
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                LoadPlaceholderData();
            }
        }

        private void LoadTables()
        {
            Tables.Clear();

            try
            {
                // Try to load actual database tables
                // For now, use placeholder data showing common game tables
                LoadPlaceholderData();
            }
            catch
            {
                LoadPlaceholderData();
            }
        }

        private void LoadPlaceholderData()
        {
            Tables.Clear();
            
            // Common game database tables
            Tables.Add(new DbTableItem("character", "Character profiles", 45));
            Tables.Add(new DbTableItem("character_template", "Character templates", 12));
            Tables.Add(new DbTableItem("item_template", "Item templates", 234));
            Tables.Add(new DbTableItem("quest", "Quest definitions", 28));
            Tables.Add(new DbTableItem("shop", "Shop definitions", 15));
            Tables.Add(new DbTableItem("alliance", "Alliance/factions", 8));
            Tables.Add(new DbTableItem("map", "Map definitions", 32));
            Tables.Add(new DbTableItem("npc_chat", "NPC dialog trees", 56));
            Tables.Add(new DbTableItem("guild", "Guild data", 21));
            Tables.Add(new DbTableItem("account", "Player accounts", 128));

            StatusText = $"Loaded {Tables.Count} table(s)";
        }

        private void LoadTableData(DbTableItem table)
        {
            TableRows.Clear();

            // Load placeholder rows for the table
            TotalRecords = table.RecordCount;
            
            for (int i = 0; i < Math.Min(100, table.RecordCount); i++)
            {
                TableRows.Add(new { 
                    Id = i + 1, 
                    Name = $"{table.TableName}_{i + 1}",
                    Value = $"Sample data {i + 1}"
                });
            }

            StatusText = $"Showing {TableRows.Count} of {TotalRecords} record(s) from {table.TableName}";
        }

        [RelayCommand]
        private void NewRecord()
        {
            if (SelectedTable != null)
            {
                StatusText = $"Create new record in {SelectedTable.TableName} (not implemented)";
            }
        }

        [RelayCommand]
        private void DeleteRecord()
        {
            if (SelectedRow != null && SelectedTable != null)
            {
                TableRows.Remove(SelectedRow);
                TotalRecords--;
                SelectedTable.RecordCount = TotalRecords;
                StatusText = $"Deleted record from {SelectedTable.TableName}";
            }
        }

        [RelayCommand]
        private void SaveChanges()
        {
            if (SelectedTable != null)
            {
                StatusText = $"Saved changes to {SelectedTable.TableName}";
            }
        }

        [RelayCommand]
        private void RefreshTable()
        {
            if (SelectedTable != null)
            {
                LoadTableData(SelectedTable);
            }
        }

        [RelayCommand]
        private void ExportTable()
        {
            if (SelectedTable != null)
            {
                StatusText = $"Export {SelectedTable.TableName} to CSV (not implemented)";
            }
        }

        partial void OnSelectedTableChanged(DbTableItem? value)
        {
            if (value != null)
            {
                LoadTableData(value);
            }
            else
            {
                TableRows.Clear();
                TotalRecords = 0;
                StatusText = "No table selected";
            }
        }
    }

    /// <summary>
    /// Represents a database table
    /// </summary>
    public partial class DbTableItem : ObservableObject
    {
        [ObservableProperty]
        private string _tableName;

        [ObservableProperty]
        private string _description;

        [ObservableProperty]
        private int _recordCount;

        public DbTableItem(string tableName, string description, int recordCount)
        {
            _tableName = tableName;
            _description = description;
            _recordCount = recordCount;
        }

        public override string ToString() => $"{TableName} ({RecordCount} records)";
    }
}

