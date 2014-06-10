using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace wExp
{
    static class ManagerDirectories
    {
        public static List<string> TracePath = new List<string>();
        static int Index = -1;

        public static void AddNode(string path)
        {
            TracePath.Add(path);
            Index = TracePath.Count - 1;
        }

        public static string GetNode(bool isBack)
        {
            if (isBack)
            {
                if (Index - 1 < 0)
                    return null;
                else
                {
                    Index--;
                    return TracePath[Index];
                }
            }
            else
            {
                if (Index + 1 >= TracePath.Count)
                    return null;
                else
                {
                    Index++;
                    return TracePath[Index];
                }
            }
        }
    }

    static class Controller
    {
        public static string PassDirectory { get; set; }
        public static string CurrentDirectory { get; set; }
    }
}
