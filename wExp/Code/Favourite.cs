using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace wExp
{
    static class Favourite
    {
        static string Path = Directory.GetCurrentDirectory() + @"\wExpData.xml";

        public static TreeNode GetFavourite()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);
            XmlNodeList folders = doc.GetElementsByTagName("Path");

            TreeNode favourite = new TreeNode();
            favourite.Name = "Favourites";
            favourite.Icon = wExp.Icon.GetIconFromExtension(".Favourite");
            favourite.Path = null;

            favourite.Childs = new List<TreeNode>();

            foreach (XmlNode folder in folders)
            {
                if (!Directory.Exists(folder.InnerText)) break;

                DirectoryInfo dir=new DirectoryInfo(folder.InnerText);
                favourite.Childs.Add(new TreeNode
                {
                    Path = dir.FullName,
                    Name = dir.Name,
                    Icon = wExp.Icon.GetIconFromExtension(".Folder"),
                    Childs = null
                });
            }

            return favourite;
        }

        public static void AddFavourite(string path)
        {
            //XmlTextWriter favouriteWriter = new XmlTextWriter(Path, null);
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);

            var pathList=doc.DocumentElement.GetElementsByTagName("Path");

            foreach (XmlNode name in pathList)
                if (name.InnerText == path)
                    return;

            var node=doc.CreateNode("element","Path","");
            node.InnerText=path;

            doc.DocumentElement.AppendChild(node);
            doc.Save(Path);
        }

        public static void DeleteFavourite(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);
            var pathList = doc.DocumentElement.GetElementsByTagName("Path");
            var root = doc.DocumentElement;
            
            int i=0;
            foreach (XmlNode name in pathList)
            {
                if (name.InnerText == path)
                    break;
                i++;
            }
            if (i < pathList.Count)
                root.RemoveChild(pathList[i]);

            doc.Save(Path);
        }

        public static bool IsFavouriteFolder(string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path);

            var pathList = doc.DocumentElement.GetElementsByTagName("Path");

            foreach (XmlNode name in pathList)
                if (name.InnerText == path)
                    return true;

            return false;
        }

    }
}
