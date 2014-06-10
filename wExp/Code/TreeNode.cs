using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace wExp
{
    class TreeNode
    {
        List<TreeNode> childs = null;
        public string Name { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }

        public List<TreeNode> Childs
        {
            get
            {
                if (childs != null) return childs;

                List<TreeNode> list = new List<TreeNode>();

                DirectoryInfo directoryInfo = new DirectoryInfo(Path);
                
                try
                {
                    FileSystemInfo[] dirs = directoryInfo.GetDirectories();

                    foreach (var dir in dirs)
                        if (((dir.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden))
                        {
                            list.Add(new TreeNode
                            {
                                Path = dir.FullName,
                                Name = dir.Name,
                                Icon = wExp.Icon.GetIconFromExtension(".Folder")
                            });
                        }
                    return list;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                childs = value;
            }
        }
    }
}
