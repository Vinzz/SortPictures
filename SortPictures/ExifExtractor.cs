// -----------------------------------------------------------------------
// <copyright file="ExifExtractor.cs" company="Maison">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace SortPictures
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>
    /// Extract shooting date for a jpg file
    /// </summary>
    public static class ExifExtractor
    {
        public static DateTime GetImageShootingDate(string stImagePath)
        {
            using (Image imgPhoto = Image.FromFile(stImagePath))
            {

                PropertyItem propItem = null;
                string myDate = "0000:00:00 00:00:00";
                DateTime ans = DateTime.MinValue;

                try
                {
                    propItem = imgPhoto.GetPropertyItem((int)0x9003 /*DateTimeOriginal*/);

                    System.Text.ASCIIEncoding Value = new System.Text.ASCIIEncoding();
                    myDate = Value.GetString(propItem.Value);

                    if (!myDate.StartsWith("0000"))
                    {
                        char[] cSeps = { ':', ' ' };
                        string[] stSplittedDate = myDate.Split(cSeps);

                        ans = new DateTime(int.Parse(stSplittedDate[0]), int.Parse(stSplittedDate[1]), int.Parse(stSplittedDate[2]), int.Parse(stSplittedDate[3]), int.Parse(stSplittedDate[4]), int.Parse(stSplittedDate[5]));
                    }
                }
                finally
                {
                    if (ans == DateTime.MinValue)
                        ans = File.GetCreationTime(stImagePath);
                }

                return ans;
            }
        }
    }
}
