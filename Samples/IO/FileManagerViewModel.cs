using System;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Humanizer;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Shiny;
using Shiny.IO;


namespace Samples.IO
{
    public class FileEntryViewModel : ReactiveObject
    {
        public FileInfo File { get; set; }
        public DirectoryInfo Directory { get; set; }


        public string Name => this.File?.Name ?? this.Directory?.Name;
        public string Size
        {
            get
            {
                if (!this.IsDirectory)
                {
                    var dc = this.Directory.GetDirectories().Length;
                    var fc = this.Directory.GetFiles().Length;
                    return $"{dc} Dir(s) - {fc} File(s)";
                }
                return this.File.Length.Bytes().Megabytes + " MB";
            }
        }

        public bool IsDirectory => this.File == null;
        public ICommand Actions { get; set; }
    }


    public class FileManagerViewModel : ViewModel
    {
        readonly IFileSystem fileSystem;
        readonly IUserDialogs dialogs;
        IDisposable dirSub;


        public FileManagerViewModel(IUserDialogs dialogs, IFileSystem fileSystem)
        {
            this.dialogs = dialogs;
            this.fileSystem = fileSystem;

            this.Select = ReactiveCommand.Create<FileEntryViewModel>(entry =>
            {
                var cfg = new ActionSheetConfig().SetCancel();

                if (entry.IsDirectory)
                {
                    cfg.Add("Enter", () => this.CurrentPath = entry.Directory.FullName);
                    cfg.Add("Delete", () => Confirm("Delete " + entry.Name, entry.Directory.Delete));
                }
                else
                {
                    cfg.Add("Delete", () => Confirm("Delete " + entry.Name, entry.File.Delete));
                    cfg.Add("Copy", () =>
                    {
                        var progress = dialogs.Progress(new ProgressDialogConfig
                        {
                            Title = "Copying File"
                        });
                        var target = new FileInfo(Path.GetFileNameWithoutExtension(entry.File.FullName) + "_1" + Path.GetExtension(entry.File.Name));
                        entry
                            .File
                            .CopyProgress(target, true)
                            .Subscribe(p =>
                            {
                                progress.Title = "Copying File - Seconds Left: " + p.TimeRemaining.TotalSeconds;
                                progress.PercentComplete = p.PercentComplete;
                            });
                    });
                }
                dialogs.ActionSheet(cfg);
            });

            this.showBack = this.WhenAnyValue(x => x.CurrentPath)
                .Select(x => x == this.CurrentPath)
                .ToProperty(this, x => x.ShowBack);

            this.WhenAnyValue(x => x.CurrentPath)
                .Skip(1)
                .Subscribe(x =>
                {
                    this.Title = x;
                    var dir = new DirectoryInfo(x);
                    this.LoadEntries(dir);

                    this.dirSub = dir
                        .WhenChanged()
                        .Subscribe(_ => this.LoadEntries(dir));
                })
                .DisposeWith(this.DestroyWith);
        }


        protected override void OnStart()
        {
            this.CurrentPath = this.fileSystem.AppData.FullName;
        }


        public override void OnDisappearing()
        {
            base.OnDisappearing();
            this.dirSub?.Dispose();
        }


        public ICommand Select { get; }
        public ICommand Back { get; }
        [Reactive] public string CurrentPath { get; private set; }
        public ObservableList<FileEntryViewModel> Entries { get; } = new ObservableList<FileEntryViewModel>();

        readonly ObservableAsPropertyHelper<bool> showBack;
        public bool ShowBack => this.showBack.Value;


        async void Confirm(string message, Action action)
        {
            var result = await this.dialogs.ConfirmAsync(message, null, "Yes", "No");
            if (result)
                action();
        }


        void LoadEntries(DirectoryInfo dir)
        {
            this.Entries.ReplaceAll(
                dir
                    .GetDirectories()
                    .Select(d => new FileEntryViewModel { Directory = d })
            );

            this.Entries.AddRange(
                dir
                    .GetFiles()
                    .Select(f => new FileEntryViewModel { File = f })
            );
        }
    }
}
