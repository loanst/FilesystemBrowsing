using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public void Get(string directoryPath)
        {
        }

        [HttpPost]
        public FolderInfo Post(DirPath path)
        {
            try
            {
                //http://localhost:49677/api/Values/Get?directoryPath=d:\Projects\
                FolderInfo di = new FolderInfo();

                if (path.directoryPath.Contains("All local HDDs"))
                {
                    di.currentPath = "All local HDDs";
                    di.parentPath = "All local HDDs";
                    di.childFoldersNames = GetAllHDD();

                    return di;
                }
                else if(Directory.Exists(path.directoryPath))
                {
                    di.currentPath = path.directoryPath;

                    if(GetAllHDD().Any(d => d == path.directoryPath))
                    {
                        di.parentPath = "All local HDDs";
                    }
                    else
                    {
                        di.parentPath = GetParentDirPath(path.directoryPath);
                    }

                    di.fileNames = Directory.GetFiles(path.directoryPath).Select(i => i = i.Replace(path.directoryPath, "").Replace(@"\", "")).ToList();
                    di.childFoldersNames = Directory.GetDirectories(path.directoryPath).Select(i => i = i.Replace(path.directoryPath, "").Replace(@"\", "")).ToList();

                    foreach (string fileName in di.fileNames)
                    {
                        string currentFilePath = di.currentPath + fileName;
                        FileInfo fi = new FileInfo(currentFilePath);
                        float fileLengthInMegabytes = ConvertBytesToMegabytes(fi.Length);

                        if (fileLengthInMegabytes <= 10)
                        {
                            di.numberOfFilesWithSizeLessThan10mb++;
                        }
                        else if (fileLengthInMegabytes > 10 && fileLengthInMegabytes <= 50)
                        {
                            di.numberOfFilesWithSizeFrom10To50mb++;
                        }
                        else if (fileLengthInMegabytes >= 100)
                        {
                            di.numberOfFilesWithSizeMoreThan100mb++;
                        }

                    }

                    return di;
                }
                else
                {
                    return new FolderInfo() { currentPath = "Wrong path", parentPath = "All local HDDs" };
                }

            }
            catch (UnauthorizedAccessException ex)
            {
                FolderInfo fi = new FolderInfo();

                fi.currentPath = "Unauthorized access error occurred";

                if (path.parentDirectoryPath.EndsWith("\\"))
                {
                    fi.parentPath = path.parentDirectoryPath.Remove(path.parentDirectoryPath.Length - 1, 1);
                }
                else
                {
                    fi.parentPath = path.parentDirectoryPath;
                }

                return fi;
            }
            catch (Exception ex)
            {
                return new FolderInfo() { currentPath = "Some error occurred", parentPath = "All local HDDs" };
            }
        }

        private float ConvertBytesToMegabytes(long bytes)
        {
            return (bytes / 1024f) / 1024f;
        }
        private List<string> GetAllHDD()
        {
            List<string> colOfHDDNames = DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed).Select(d => d.Name.ToLower()).ToList();

            return colOfHDDNames;
        }
        private string GetParentDirPath(string childPath)
        {
            // для правильної роботи Directory.GetParent, знак "\" в кінці шляху є неприйнятним, тому він видаляється.
            string dirPath = childPath.Remove(childPath.Length - 1, 1);
            string parentPath = Directory.GetParent(dirPath).FullName;
            // GetParent повертає каталоги "батьківського" рівня без знака "\". але для найвищого рівня - дисків - результат буде з знаком "\" в кінці. якщо це так, то він видаляється.
            if (parentPath.EndsWith("\\"))
            {
                parentPath = parentPath.Remove(parentPath.Length - 1, 1);
            }

            return parentPath;
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
