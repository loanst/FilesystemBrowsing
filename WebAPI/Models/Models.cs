using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebAPI.Models
{
    public class FolderInfo
    {
        public string currentPath { get; set; }
        public int numberOfFilesWithSizeLessThan10mb { get; set; }
        public int numberOfFilesWithSizeFrom10To50mb { get; set; }
        public int numberOfFilesWithSizeMoreThan100mb { get; set; }

        public string parentPath { get; set; }
        public List<string> childFoldersNames { get; set; }
        public List<string> fileNames { get; set; }


    }
    public class DirPath
    {
        public string directoryPath { get; set; }
        public string parentDirectoryPath { get; set; }

    }
}