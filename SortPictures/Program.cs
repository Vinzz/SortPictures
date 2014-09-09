using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace SortPictures
{
    public static class Program
    {
        static IEnumerable<FileInfo> RecursiveGetFiles(this DirectoryInfo input)
        {
            return DirDeployExplore(input);
        }

        static public Stack<FileInfo> DirDeployExplore(DirectoryInfo dSrcDir)
        {
            try
            {
                Stack<DirectoryInfo> stackDirs = new Stack<DirectoryInfo>();
                Stack<FileInfo> stackPaths = new Stack<FileInfo>();

                stackDirs.Push(dSrcDir);
                while (stackDirs.Count > 0)
                {
                    DirectoryInfo currentDir = (DirectoryInfo)stackDirs.Pop();

                    //Process .\files
                    foreach (FileInfo fileInfo in currentDir.GetFiles())
                    {
                        stackPaths.Push(fileInfo);
                    }

                    //Process Subdirectories
                    foreach (DirectoryInfo diNext in currentDir.GetDirectories())
                        stackDirs.Push(diNext);
                }

                return stackPaths;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                return null;
            }
        }


        static void Main(string[] args)
        {
            try
            {
                string input = args[0];
                string outDir = args[1];
                DirectoryInfo inputDir = new DirectoryInfo(input);
                string extensions = SortPictures.Properties.SortPictures.Default.ExtensionsToSort;

                Console.WriteLine("inputDir: " + input);
                Console.WriteLine("OutDir: " + outDir);
                Console.WriteLine("Files considered: " + extensions);

                IEnumerable<FileInfo> srcPics = inputDir.RecursiveGetFiles();

                int count = 0;
                foreach (FileInfo fi in srcPics)
                {
                    if (extensions.Contains(fi.Extension.ToLower()))
                    {
                        if (++count % 10 == 0)
                            Console.WriteLine(string.Format("Files processed {0}/{1}", count, srcPics.Count()));

                        DateTime fileDate = fi.LastWriteTime;

                        string yearPath = Path.Combine(outDir, fileDate.Year.ToString());
                        string monthPath = Path.Combine(yearPath, fileDate.Month.ToString("00") + " - " + fileDate.ToString("MMMM"));
                        string destPath = Path.Combine(monthPath, fi.Name.TrimStart('_'));

                        if (!Directory.Exists(yearPath))
                            Directory.CreateDirectory(yearPath);
                        if (!Directory.Exists(monthPath))
                            Directory.CreateDirectory(monthPath);

                        if (!File.Exists(destPath))
                        {
                            fi.MoveTo(destPath);
                        }
                        else
                        {
                            // Duplicated file
                            fi.Delete();
                        }
                    }
                }
                Console.WriteLine(string.Format("{0} files moved", count));
                Console.WriteLine("That's all folks");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
