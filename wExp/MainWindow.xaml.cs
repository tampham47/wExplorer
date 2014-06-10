using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace wExp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadTreeFolder();
            
        }

        public void LoadTreeFolder()
        {
            List<TreeNode> treeList = new List<TreeNode>();
            treeList.Add(Favourite.GetFavourite());

            TreeNode computer = new TreeNode();
            computer.Name = "MyComputer";
            computer.Icon = wExp.Icon.GetIconFromExtension(".Computer");
            computer.Path = null;
            computer.Childs = new List<TreeNode>();

            DriveInfo[] myDrivers = DriveInfo.GetDrives();
            foreach (var driver in myDrivers)
            {
                computer.Childs.Add(new TreeNode
                {
                    Path = driver.Name,
                    Name = driver.Name,
                    Icon = wExp.Icon.GetIconFromExtension(".volume")
                });
            }

            treeList.Add(computer);

            treeFolder.ItemsSource = treeList;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            ExtendGlassFrame();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MARGINS
        {
            public MARGINS(int Left, int Right, int Top, int Bottom)
            {
                cxLeftWidth = Left;
                cxRightWidth = Right;
                cyTopHeight = Top;
                cyBottomHeight = Bottom;

            }

            public int cxLeftWidth, cxRightWidth, cyTopHeight, cyBottomHeight;
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

        [DllImport("dwmapi.dll", PreserveSig = false)]
        static extern bool DwmIsCompositionEnabled();

        public bool ExtendGlassFrame()
        {
            if (Environment.OSVersion.Version.Major < 6 || !DwmIsCompositionEnabled())
                return false;

            IntPtr hWnd = new WindowInteropHelper(this).Handle;
            this.Background = Brushes.Transparent;
            HwndSource.FromHwnd(hWnd).CompositionTarget.BackgroundColor = Colors.Transparent;
            MARGINS margins = new MARGINS(-1, -1, -1, -1);
            DwmExtendFrameIntoClientArea(hWnd, ref margins);
            return true;
        }

        private void treeFolder_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                if (treeFolder.SelectedItem == null) return;
                var node = (TreeNode)treeFolder.SelectedItem;
                Display(node.Path, null);
                ManagerDirectories.AddNode(((TreeNode)treeFolder.SelectedItem).Path);
                RefreshDirStackPanel(node.Path);
                RefreshStackPanelInfo(node.Path);
            }
            catch
            {
                Display("D:\\", null);
                //RefreshDirStackPanel("D:\\");
            }
            
        }

        public void Display(string path, string search)
        {
            List<ElementDisplay> elementDisplays = new List<ElementDisplay>();
            if ((path == "") || (path == null))
            {
                List<TreeNode> treeElements = ((TreeNode)treeFolder.SelectedItem).Childs;
                foreach (TreeNode element in treeElements)
                {
                    elementDisplays.Add(new ElementDisplay
                    {
                        Name = element.Name,
                        Icon = wExp.Icon.GetIconFromExtension(System.IO.Path.GetExtension(element.Path)),
                        Path = element.Path
                    });
                }
            }
            else
            {
                FileSystemInfo[] files;
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                if (search == null)
                {
                    try
                    {
                        files = directoryInfo.GetFileSystemInfos();
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Không thể truy cập vào : " + path,
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }
                }
                else
                {
                    try
                    {
                        if (search == ".folder")
                            files = directoryInfo.GetDirectories();
                        else
                            files = directoryInfo.GetFileSystemInfos(search, SearchOption.TopDirectoryOnly);
                    }
                    catch
                    {
                        MessageBox.Show(
                            "Không thể truy cập vào : " + path,
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Warning);
                        return;
                    }
                }

                foreach (var file in files)
                    if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                    {
                        elementDisplays.Add(new ElementDisplay
                        {
                            Name = file.Name,
                            Icon = wExp.Icon.GetIconFromExtension(System.IO.Path.GetExtension(file.FullName)),
                            Path = file.FullName
                        });
                    }
            }
            listExp.ItemsSource = elementDisplays;
            RefreshDirStackPanel(path);
            Controller.CurrentDirectory = path;
        }

        #region TextSearch
        private void textSearch_MouseEnter(object sender, MouseEventArgs e)
        {
            if (textSearch.Text == "Search")
                textSearch.Text = null;
        }

        private void textSearch_MouseLeave(object sender, MouseEventArgs e)
        {
            if (textSearch.Text == "")
                textSearch.Text = "Search";
        }

        private void textSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                //var node = (TreeNode)treeFolder.SelectedItem;
                Display(Controller.CurrentDirectory, "*" + textSearch.Text + "*");
            }
            buttonSearchClose.Visibility = Visibility.Visible;
        }
        #endregion

        private void buttonBack_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MessageBox.Show("ok");
            }
        }

        private void buttonForward_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                MessageBox.Show("ok");
            }
        }

        private void ElementDisplay_MouseEnter(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(((StackPanel)sender).Background.ToString());
        }

        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            string path = ManagerDirectories.GetNode(true);
            if (path != null)
            {
                Display(path, null);
            }
        }

        private void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            string path = ManagerDirectories.GetNode(false);
            if (path != null)
            {
                Display(path, null);
            }
        }

        private void listExp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show("change");
            ElementDisplay file = (ElementDisplay)listExp.SelectedItem;
            if (file == null) return;
            else
            {
                RefreshStackPanelInfo(file.Path);
            }
        }

        private void RefreshStackPanelInfo(string filePath)
        {
            if (filePath == null) return;
            FileInfo fileInfo = new FileInfo(filePath);
            DisplayInfo displayInfo = new DisplayInfo();
            displayInfo.Icon = wExp.Icon.GetIconFromExtension((fileInfo.Extension != "") ? fileInfo.Extension : ".folder");
            displayInfo.Name = Truncate(fileInfo.Name, 28);
            displayInfo.Extension = (fileInfo.Extension != "") ? fileInfo.Extension : "folder";
            displayInfo.DateModified = "DateModifies " + fileInfo.CreationTime.ToShortDateString();
            displayInfo.Path = filePath;
            stackPanelInfo.DataContext = displayInfo;
            if (Favourite.IsFavouriteFolder(displayInfo.Path))
                favouriteText.Text = "Unfavourite this!";
            else
                favouriteText.Text = "Favourite this!";
            if (displayInfo.Extension != "folder")
                buttonFavourite.Visibility = Visibility.Hidden;
            else
                buttonFavourite.Visibility = Visibility.Visible;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {

                ElementDisplay element = (ElementDisplay)listExp.SelectedItem;
                if (!element.Name.Contains('.'))
                {
                    TreeNode node = new TreeNode
                    {
                        Path = element.Path,
                        Icon = element.Icon
                    };

                    RefreshDirStackPanel(element.Path);
                    Display(element.Path, null);
                }
                if (element.Path.Contains(".jpg"))
                {
                    PhotoViewer photoViewer = new PhotoViewer(element.Path);
                    photoViewer.Show();
                }
            }
        }


        //public delegate void DirBarEventHandler(object sender, DirBarEventArgs e);
        //public event DirBarEventHandler Click;
        private void RefreshDirStackPanel(string path)
        {
            if (path==null || !path.Contains("\\")) return;
            string[] paths=path.Split('\\');
            string temp = "";
            //Button newButton;
            DirButton newButton;

            stackDirectory.Children.Clear();
            for (int i = 0; i < paths.Length; i++)
            {
                newButton = new DirButton();
                newButton.Content = paths[i];
                temp += paths[i] + "\\";
                newButton.Path = temp;
                newButton.Click += new RoutedEventHandler(newButton_Click);
                

                stackDirectory.Children.Add(newButton);
            }
        }

        void newButton_Click(object sender, RoutedEventArgs e)
        {
            Display(((DirButton)sender).Path, null);
            //throw new NotImplementedException();
        }

        #region Menu
        private void menuCopy_Click(object sender, RoutedEventArgs e)
        {
            if ((ElementDisplay)listExp.SelectedItem != null)
            {
                Controller.PassDirectory = ((ElementDisplay)listExp.SelectedItem).Path;
                //MessageBox.Show(((ElementDisplay)listExp.SelectedItem).Path);
            }
        }

        private void menuPaste_Click(object sender, RoutedEventArgs e)
        {
            //File.Copy(
            var selectItem = (ElementDisplay)listExp.SelectedItem;
            if (selectItem != null && (selectItem.Path != Controller.PassDirectory))
            {
                //Directory.
                Directory.Move(
                    Controller.PassDirectory,
                    selectItem.Path + @"\" + System.IO.Path.GetFileName(Controller.PassDirectory));

                //MessageBox.Show("Ok " + selectItem.Path + @"\" + System.IO.Path.GetFileName(Controller.PassDirectory));
            }
            else
            {
                Directory.Move(
                    Controller.PassDirectory,
                    Controller.CurrentDirectory + System.IO.Path.GetFileName(Controller.PassDirectory));
                Display(Controller.CurrentDirectory, null);

                //MessageBox.Show(Controller.CurrentDirectory + System.IO.Path.GetFileName(Controller.PassDirectory));
            }
        }

        private void menuDelete_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = (ElementDisplay)listExp.SelectedItem;
            if (selectItem != null)
            {
                File.Delete(selectItem.Path);
                //MessageBox.Show("Ok");
                Display(System.IO.Path.GetDirectoryName(selectItem.Path), null);
            }
        }

        private void menuNewFolder_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = (ElementDisplay)listExp.SelectedItem;
            if (selectItem != null && !selectItem.Path.Contains('.'))
            {

            }
        }
        #endregion

        private void menuProperties_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = (ElementDisplay)listExp.SelectedItem;
            ShowFileProperties(selectItem.Path);
        }

        static string Truncate(string name, int k)
        {
            if (name.Length < k)
                return name;
            else
                return name.Substring(0, k);
        }

        #region Properties
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SHELLEXECUTEINFO
        {
            public int cbSize;
            public uint fMask;
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpVerb;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpFile;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpParameters;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpDirectory;
            public int nShow;
            public IntPtr hInstApp;
            public IntPtr lpIDList;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string lpClass;
            public IntPtr hkeyClass;
            public uint dwHotKey;
            public IntPtr hIcon;
            public IntPtr hProcess;
        }

        private const int SW_SHOW = 5;
        private const uint SEE_MASK_INVOKEIDLIST = 12;
        public static void ShowFileProperties(string Filename)
        {
            SHELLEXECUTEINFO info = new SHELLEXECUTEINFO();
            info.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(info);
            info.lpVerb = "properties";
            info.lpFile = Filename;
            info.nShow = SW_SHOW;
            info.fMask = SEE_MASK_INVOKEIDLIST;
            ShellExecuteEx(ref info);

        }
        #endregion

        private void buttonIconInfo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //var info = (DisplayInfo)stackPanelInfo.DataContext;
            //ShowFileProperties(info.Path);
            var displayInfo = (DisplayInfo)stackPanelInfo.DataContext;
            if (displayInfo == null) return;
            textSearch.Text = displayInfo.Extension;
            if (displayInfo.Extension != "folder")
                Display(System.IO.Path.GetDirectoryName(displayInfo.Path), "*" + displayInfo.Extension + "*");
            else
                Display(System.IO.Path.GetDirectoryName(displayInfo.Path), ".folder");

            buttonSearchClose.Visibility = Visibility.Visible;
        }

        //private void ShowProperties()

        private void buttonFavourite_Click(object sender, RoutedEventArgs e)
        {
            var displayInfo = (DisplayInfo)stackPanelInfo.DataContext;
            if (displayInfo == null) return;
            if (System.IO.Path.GetExtension(displayInfo.Path) == "")
            {
                if (Favourite.IsFavouriteFolder(displayInfo.Path))
                {
                    Favourite.DeleteFavourite(displayInfo.Path);
                    MessageBox.Show(
                    "Đã xóa " + displayInfo.Path + "khỏi favourite",
                    "Hoàn tất",
                    MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
                }
                else
                {
                    Favourite.AddFavourite(displayInfo.Path);
                    MessageBox.Show(
                        "Đã thêm " + displayInfo.Path + "vào favourite",
                        "Hoàn tất",
                        MessageBoxButton.OK,
                        MessageBoxImage.Asterisk);
                }
                LoadTreeFolder();
                Display(displayInfo.Path, null);
            }
        }

        private void buttonFilter_Click(object sender, RoutedEventArgs e)
        {
            var displayInfo = (DisplayInfo)stackPanelInfo.DataContext;
            if (displayInfo == null) return;
            textSearch.Text = displayInfo.Extension;
            if (displayInfo.Extension != "folder")
                Display(System.IO.Path.GetDirectoryName(displayInfo.Path), "*" + displayInfo.Extension + "*");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void treeFolderItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (treeFolder.SelectedItem == null) return;
            var node = (TreeNode)treeFolder.SelectedItem;
            Display(node.Path, null);
            ManagerDirectories.AddNode(((TreeNode)treeFolder.SelectedItem).Path);
            RefreshDirStackPanel(node.Path);
            RefreshStackPanelInfo(node.Path);
        }

        private void buttonSearchClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Display(Controller.CurrentDirectory, null);
            this.textSearch.Text = "Search";
            buttonSearchClose.Visibility = Visibility.Hidden;
        }
    }
}