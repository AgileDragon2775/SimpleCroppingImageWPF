using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace CustomImageChange
{
    public class FileImageList
    {
        int index = 0;
        FileInfo[] imageList;
        BitmapImage nowImage;
        List<Point> trans;


        public void SelectImageList()
        {
            CommonOpenFileDialog folderDialog = new CommonOpenFileDialog()
            {
                InitialDirectory = "",
                IsFolderPicker = true
            };

            if (folderDialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                index = 0;
                string path = folderDialog.FileName;
                DirectoryInfo di = new DirectoryInfo(path);
                imageList = di.GetFiles();
            }
        }

        public BitmapImage GetSelectedImage()
        {
            return nowImage;
        }

        public string GetSelectedImageFullFIleName()
        {
            return imageList[index].FullName;        
        }

        public void SelectImage()
        {
            if (imageList == null)
            {
                nowImage = null;
                return;
            }

            index = ((index % imageList.Length) + imageList.Length) % imageList.Length;

            if (imageList.Length > index)
            {

                nowImage = new BitmapImage();
                var stream = File.OpenRead(imageList[index].FullName);
                nowImage.BeginInit();
                nowImage.CacheOption = BitmapCacheOption.OnLoad;
                nowImage.StreamSource = stream;
                nowImage.EndInit();
                stream.Close();
                stream.Dispose();           
            }
        }

        public void SelectNextImage()
        {
            index++;
            SelectImage();
        }

        public void SelectPreviousImage()
        {
            index--;
            SelectImage();
        }

        public string GetStatus()
        {
            return imageList != null ? index + "/" + imageList.Length : "";
        }

        public void SaveImage2FIle()
        {
            if (nowImage == null) return;
            Bitmap bmp = BitmapImage2Bitmap(nowImage);
            bmp.Save(GetSelectedImageFullFIleName(), System.Drawing.Imaging.ImageFormat.Png);
        }

        public void ApplyCropImage(CroppedBitmap cropped,int x,int y,int width,int height)
        {
            if (nowImage == null || cropped == null) return;

            Bitmap bmp = BitmapImage2Bitmap(nowImage);
            Bitmap bmp_cropped = BitmapImage2Bitmap(cropped);
            BackupTransparency(bmp, bmp_cropped, x, y);;            

            using (var g = Graphics.FromImage(bmp))
            {
                g.DrawImage(
                    bmp_cropped,
                    x,
                    y,
                    width,
                    height);
                g.Save();
                nowImage = BitmapToImageSource(bmp);
            }
        }

        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        private Bitmap BitmapImage2Bitmap(CroppedBitmap bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }

        private void BackupTransparency(Bitmap bitmap, Bitmap cropped, int x_cropped, int y_cropped)
        {
            trans = new List<Point>();

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    Color color = bitmap.GetPixel(x, y);
                    if (color.A == 0)
                    {
                        trans.Add(new Point(x, y));
                    }
                }
            }

            for (int x = 0; x < cropped.Width; x++)
            {
                for (int y = 0; y < cropped.Height; y++)
                {
                    Color color = cropped.GetPixel(x, y);
                    if (color.A == 0)
                    {
                        trans.Add(new Point(x + x_cropped, y + y_cropped));
                    }
                }
            }
        }

        public void RestoreTransparency()
        {
            Bitmap bmp = BitmapImage2Bitmap(nowImage);

            foreach (Point pos in trans)
            {
                bmp.SetPixel(pos.X, pos.Y, Color.Transparent);
            }
            nowImage = BitmapToImageSource(bmp);
        }
    }
}
