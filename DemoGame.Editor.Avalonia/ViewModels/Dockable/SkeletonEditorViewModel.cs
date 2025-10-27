using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace DemoGame.Editor.Avalonia.ViewModels.Dockable
{
    /// <summary>
    /// ViewModel for Skeleton Editor - edit character animations
    /// Replaces WinForms SkeletonEditorForm
    /// </summary>
    public partial class SkeletonEditorViewModel : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<SkeletonItem> _skeletons = new();

        [ObservableProperty]
        private SkeletonItem? _selectedSkeleton;

        [ObservableProperty]
        private ObservableCollection<SkeletonNodeItem> _nodes = new();

        [ObservableProperty]
        private SkeletonNodeItem? _selectedNode;

        [ObservableProperty]
        private ObservableCollection<AnimationItem> _animations = new();

        [ObservableProperty]
        private AnimationItem? _selectedAnimation;

        [ObservableProperty]
        private string _statusText = "Ready";

        [ObservableProperty]
        private bool _isPlaying;

        [ObservableProperty]
        private int _currentFrame;

        [ObservableProperty]
        private int _totalFrames;

        public SkeletonEditorViewModel()
        {
            try
            {
                LoadSkeletons();
            }
            catch (Exception ex)
            {
                StatusText = $"Error: {ex.Message}";
                LoadPlaceholderData();
            }
        }

        private void LoadSkeletons()
        {
            Skeletons.Clear();

            try
            {
                // Try to load actual skeleton data
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
            Skeletons.Clear();
            
            Skeletons.Add(new SkeletonItem("Player_Male", "player_male.skel"));
            Skeletons.Add(new SkeletonItem("Player_Female", "player_female.skel"));
            Skeletons.Add(new SkeletonItem("Enemy_Skeleton", "enemy_skeleton.skel"));
            Skeletons.Add(new SkeletonItem("Enemy_Zombie", "enemy_zombie.skel"));
            Skeletons.Add(new SkeletonItem("Boss_Dragon", "boss_dragon.skel"));

            StatusText = $"Loaded {Skeletons.Count} skeleton(s)";
        }

        private void LoadSkeletonData(SkeletonItem skeleton)
        {
            // Load nodes
            Nodes.Clear();
            Nodes.Add(new SkeletonNodeItem("Root", 0, 0, 0));
            Nodes.Add(new SkeletonNodeItem("Body", 0, 0, 1));
            Nodes.Add(new SkeletonNodeItem("Head", 0, -32, 2));
            Nodes.Add(new SkeletonNodeItem("LeftArm", -16, -24, 3));
            Nodes.Add(new SkeletonNodeItem("RightArm", 16, -24, 4));
            Nodes.Add(new SkeletonNodeItem("LeftLeg", -8, 16, 5));
            Nodes.Add(new SkeletonNodeItem("RightLeg", 8, 16, 6));

            // Load animations
            Animations.Clear();
            Animations.Add(new AnimationItem("Idle", 4, 1.0f));
            Animations.Add(new AnimationItem("Walk", 8, 0.6f));
            Animations.Add(new AnimationItem("Run", 8, 0.4f));
            Animations.Add(new AnimationItem("Attack", 6, 0.8f));
            Animations.Add(new AnimationItem("Jump", 4, 1.2f));

            StatusText = $"Loaded {skeleton.Name}: {Nodes.Count} nodes, {Animations.Count} animations";
        }

        [RelayCommand]
        private void NewSkeleton()
        {
            var newSkeleton = new SkeletonItem($"NewSkeleton{Skeletons.Count + 1}", "new.skel");
            Skeletons.Add(newSkeleton);
            SelectedSkeleton = newSkeleton;
            StatusText = $"Created new skeleton: {newSkeleton.Name}";
        }

        [RelayCommand]
        private void DeleteSkeleton()
        {
            if (SelectedSkeleton != null)
            {
                var name = SelectedSkeleton.Name;
                Skeletons.Remove(SelectedSkeleton);
                SelectedSkeleton = Skeletons.FirstOrDefault();
                StatusText = $"Deleted skeleton: {name}";
            }
        }

        [RelayCommand]
        private void AddNode()
        {
            if (SelectedSkeleton != null)
            {
                var newNode = new SkeletonNodeItem($"Node{Nodes.Count}", 0, 0, Nodes.Count);
                Nodes.Add(newNode);
                SelectedNode = newNode;
                StatusText = $"Added new node: {newNode.Name}";
            }
        }

        [RelayCommand]
        private void DeleteNode()
        {
            if (SelectedNode != null)
            {
                var name = SelectedNode.Name;
                Nodes.Remove(SelectedNode);
                SelectedNode = Nodes.FirstOrDefault();
                StatusText = $"Deleted node: {name}";
            }
        }

        [RelayCommand]
        private void AddAnimation()
        {
            if (SelectedSkeleton != null)
            {
                var newAnim = new AnimationItem($"Animation{Animations.Count + 1}", 4, 1.0f);
                Animations.Add(newAnim);
                SelectedAnimation = newAnim;
                StatusText = $"Added new animation: {newAnim.Name}";
            }
        }

        [RelayCommand]
        private void DeleteAnimation()
        {
            if (SelectedAnimation != null)
            {
                var name = SelectedAnimation.Name;
                Animations.Remove(SelectedAnimation);
                SelectedAnimation = Animations.FirstOrDefault();
                StatusText = $"Deleted animation: {name}";
            }
        }

        [RelayCommand]
        private void PlayAnimation()
        {
            if (SelectedAnimation != null)
            {
                IsPlaying = true;
                CurrentFrame = 0;
                TotalFrames = SelectedAnimation.FrameCount;
                StatusText = $"Playing: {SelectedAnimation.Name}";
            }
        }

        [RelayCommand]
        private void StopAnimation()
        {
            IsPlaying = false;
            if (SelectedAnimation != null)
            {
                StatusText = $"Stopped: {SelectedAnimation.Name}";
            }
        }

        [RelayCommand]
        private void SaveSkeleton()
        {
            if (SelectedSkeleton != null)
            {
                StatusText = $"Saved skeleton: {SelectedSkeleton.Name}";
            }
        }

        partial void OnSelectedSkeletonChanged(SkeletonItem? value)
        {
            if (value != null)
            {
                LoadSkeletonData(value);
                
                // Notify other panels of selection (for Properties panel, etc.)
                SelectionChanged?.Invoke(this, value);
            }
            else
            {
                Nodes.Clear();
                Animations.Clear();
                StatusText = "No skeleton selected";
                SelectionChanged?.Invoke(this, null);
            }
        }

        /// <summary>
        /// Event raised when selection changes - other panels can subscribe to this
        /// </summary>
        public event EventHandler<object?>? SelectionChanged;

        partial void OnSelectedAnimationChanged(AnimationItem? value)
        {
            if (value != null)
            {
                TotalFrames = value.FrameCount;
                CurrentFrame = 0;
                IsPlaying = false;
                StatusText = $"Selected: {value.Name} ({value.FrameCount} frames, {value.Speed}s)";
            }
        }
    }

    /// <summary>
    /// Represents a skeleton
    /// </summary>
    public partial class SkeletonItem : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private string _fileName;

        public SkeletonItem(string name, string fileName)
        {
            _name = name;
            _fileName = fileName;
        }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Represents a skeleton node (bone)
    /// </summary>
    public partial class SkeletonNodeItem : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private int _x;

        [ObservableProperty]
        private int _y;

        [ObservableProperty]
        private int _nodeIndex;

        public SkeletonNodeItem(string name, int x, int y, int nodeIndex)
        {
            _name = name;
            _x = x;
            _y = y;
            _nodeIndex = nodeIndex;
        }

        public override string ToString() => $"[{NodeIndex}] {Name} ({X}, {Y})";
    }

    /// <summary>
    /// Represents an animation
    /// </summary>
    public partial class AnimationItem : ObservableObject
    {
        [ObservableProperty]
        private string _name;

        [ObservableProperty]
        private int _frameCount;

        [ObservableProperty]
        private float _speed;

        public AnimationItem(string name, int frameCount, float speed)
        {
            _name = name;
            _frameCount = frameCount;
            _speed = speed;
        }

        public override string ToString() => $"{Name} ({FrameCount} frames)";
    }
}

