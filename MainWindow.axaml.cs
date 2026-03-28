using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PaintPower.Dialogs;
using PaintPower.Editors;
using PaintPower.FileExplorer;
using PaintPower.Logging;
using PaintPower.Networking;
using PaintPower.ProjectSystem;
using PaintPower.SpriteEditor;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
namespace PaintPower;

public partial class MainWindow : Window
{
    private readonly Editor _editorManager;
    private readonly PaintProject _project;
    private EditorBase _editor;
    public Server server;

    private SpriteEditorView _spriteEditorView;

    public bool saveNeeded = false;

    public static PaintPower_Engine App = new PaintPower_Engine();
    public static MainWindow window;

    public MainWindow()
    {
        InitializeComponent();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        server = new Server();
        SaveButton.Click += (_, __) => App.Save();
        SaveAsButton.Click += (_, __) => App.SaveAs();
        ProjectStatus.PointerPressed += App.StatusClicked;
        UploadProjectButton.Click += (_, __) => App.SaveToServer();
        NewButton.Click += (_, __) => App.newProject();

        // Display PaintPower version:
        VersionInfoTextBlock.Text = PaintPower_Engine.version;

        // After, make a static reference.
        window = this;
    }

    protected override async void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        App.attachWindow(this);
        App.Start();
    }
}