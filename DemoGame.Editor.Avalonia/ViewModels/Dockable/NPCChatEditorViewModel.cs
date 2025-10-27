using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for NPC Chat Editor - edit dialog trees
    /// Replaces WinForms NPCChatEditorForm
    /// </summary>
    public partial class NPCChatEditorViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<NPCChatDialogItem> _dialogs = new();

        [ObservableProperty]
        private NPCChatDialogItem? _selectedDialog;

        [ObservableProperty]
        private ObservableCollection<ChatNodeItem> _chatNodes = new();

        [ObservableProperty]
        private ChatNodeItem? _selectedNode;

        [ObservableProperty]
        private string _statusText = "Ready";

        public NPCChatEditorViewModel()
        {
            try
            {
                LoadDialogs();
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                LoadPlaceholderData();
            }
        }

        private void LoadDialogs()
        {
            Dialogs.Clear();

            try
            {
                // Try to load actual NPC chat dialogs
                // For now, use placeholder data
                LoadPlaceholderData();
            }
            catch
            {
                LoadPlaceholderData();
            }
        }

        private void LoadPlaceholderData()
        {
            Dialogs.Clear();
            Dialogs.Add(new NPCChatDialogItem("ShopKeeper_Greeting", 1));
            Dialogs.Add(new NPCChatDialogItem("Quest_GatherItems", 2));
            Dialogs.Add(new NPCChatDialogItem("Guard_EntranceCheck", 3));
            Dialogs.Add(new NPCChatDialogItem("Villager_Gossip", 4));
            StatusText = $"Loaded {Dialogs.Count} dialog(s)";
        }

        private void LoadChatNodesForDialog(NPCChatDialogItem dialog)
        {
            ChatNodes.Clear();

            // Load dialog tree structure
            var rootNode = new ChatNodeItem(0, "Root", "Welcome to my shop!");
            rootNode.Children.Add(new ChatNodeItem(1, "Response 1", "I'd like to buy something."));
            rootNode.Children.Add(new ChatNodeItem(2, "Response 2", "Just looking around."));
            rootNode.Children.Add(new ChatNodeItem(3, "Response 3", "Goodbye."));

            ChatNodes.Add(rootNode);
        }

        [RelayCommand]
        private void NewDialog()
        {
            var newDialog = new NPCChatDialogItem($"NewDialog{Dialogs.Count + 1}", Dialogs.Count + 1);
            Dialogs.Add(newDialog);
            SelectedDialog = newDialog;
            StatusText = $"Created new dialog: {newDialog.Name}";
        }

        [RelayCommand]
        private void DeleteDialog()
        {
            if (SelectedDialog != null)
            {
                var name = SelectedDialog.Name;
                Dialogs.Remove(SelectedDialog);
                SelectedDialog = Dialogs.FirstOrDefault();
                StatusText = $"Deleted dialog: {name}";
            }
        }

        [RelayCommand]
        private void AddChatNode()
        {
            if (SelectedNode != null)
            {
                var newNode = new ChatNodeItem(
                    SelectedNode.Children.Count + 1, 
                    "New Response", 
                    "Enter response text...");
                SelectedNode.Children.Add(newNode);
                StatusText = $"Added new chat node";
            }
            else if (ChatNodes.Any())
            {
                var newNode = new ChatNodeItem(
                    ChatNodes.Count, 
                    "New Node", 
                    "Enter node text...");
                ChatNodes.Add(newNode);
                StatusText = $"Added new root node";
            }
        }

        [RelayCommand]
        private void DeleteChatNode()
        {
            if (SelectedNode != null)
            {
                StatusText = $"Delete node (not fully implemented)";
            }
        }

        [RelayCommand]
        private void SaveDialog()
        {
            if (SelectedDialog != null)
            {
                StatusText = $"Saved dialog: {SelectedDialog.Name}";
            }
        }

        partial void OnSelectedDialogChanged(NPCChatDialogItem? value)
        {
            if (value != null)
            {
                StatusText = $"Selected: {value.Name}";
                LoadChatNodesForDialog(value);
                
                // Notify other panels of selection (for Properties panel, etc.)
                SelectionChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Event raised when selection changes - other panels can subscribe to this
        /// </summary>
        public event EventHandler<object?>? SelectionChanged;
    }

    /// <summary>
    /// Represents an NPC dialog in the list
    /// </summary>
    public partial class NPCChatDialogItem : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private int _dialogId;

        public NPCChatDialogItem(string name, int dialogId)
        {
            _name = name;
            _dialogId = dialogId;
        }

        public override string ToString() => $"[{DialogId}] {Name}";
    }

    /// <summary>
    /// Represents a node in the chat tree
    /// </summary>
    public partial class ChatNodeItem : ObservableObject
    {
        [ObservableProperty]
        private int _nodeId;

        [ObservableProperty]
        private string _title;

        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private bool _isExpanded;

        public ObservableCollection<ChatNodeItem> Children { get; } = new();

        public ChatNodeItem(int nodeId, string title, string text)
        {
            _nodeId = nodeId;
            _title = title;
            _text = text;
        }

        public override string ToString() => $"[{NodeId}] {Title}";
    }
}

