using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace wExp
{
    static class Icon
    {
        public static string GetIconFromExtension(string extension)
        {
            if (extension == "") return @"Image\folder.png";

            extension = extension.Remove(0, 1);

            var files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\Image");
            var names = FileName(files);

            for (int i = 0; i < names.Count; i++)
                if (names[i] == extension.ToLower())
                {
                    return @"Image\" + System.IO.Path.GetFileName(files[i]);
                }

            return @"Image\Unknow.png";
        }

        public static List<string> FileName(string[] files)
        {
            List<string> nameList=new List<string>();
            string name;
            foreach (var file in files)
            {
                name = System.IO.Path.GetFileName(file);
                nameList.Add(name.Substring(0, name.IndexOf(".")));
            }

            return nameList;
        }
    }
}
