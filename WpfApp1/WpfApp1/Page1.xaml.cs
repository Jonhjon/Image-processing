using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Reflection.Emit;

namespace WpfApp1
{
    /// <summary>
    /// Page1.xaml 的互動邏輯
    /// </summary>
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)//上傳影像到origin_Image
        {
            // 建立一個 OpenFileDialog 實例
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";
            bool? result = openFileDialog.ShowDialog();
            // 檢查使用者是否選擇了圖片
            if (result == true)
            {
                // 創建 BitmapImage 並加載選擇的圖片路徑
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(openFileDialog.FileName);
                bitmap.EndInit();

                // 將 BitmapImage 賦值給 Image 控制項的 Source
                Origin_Image.Source = bitmap;
            }
        }
        private BitmapImage Bitmap_To_ImageSource(Bitmap bitmap)//把BitmapImage轉換成Image控制項可以讀取的圖片
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
        private Bitmap Image_To_Bitmap(BitmapImage bitmapImage)//把Image控制項的的圖片轉換成bitmapImage
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(outStream);
                return new Bitmap(outStream);
            }
        }

       // Bitmap bitmap = Image_To_Bitmap((BitmapImage)Origin_Image.Source);


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //InitializeComponent();
            double value = e.NewValue;
            Lable1.Content = "角度 : " + (int)value;

            value = value * Math.PI / 180; 
            Bitmap bitmap = Image_To_Bitmap((BitmapImage)Origin_Image.Source);
            Bitmap result_Image = new Bitmap(bitmap.Width, bitmap.Height);

            int x0 = bitmap.Width / 2,
                y0 = bitmap.Height/2;

            int newx = 0,newy = 0;

            double sin = Math.Sin(value);
            double cos = Math.Cos(value);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for(int y = 0; y < bitmap.Height; y++)
                {
                    Color color =bitmap.GetPixel(x, y); 
                    int dx = (int)(x - x0);
                    int dy = (int)(y - y0);


                    newx = (int)(cos * dx - sin * dy) + x0;
                    newy = (int)(sin * dx + cos * dy) + y0;

                    if(newx >= 0 && newx < bitmap.Width && newy >= 0 && newy < bitmap.Height)
                    {
                        //color = bitmap.GetPixel(newx, newy);
                        result_Image.SetPixel(newx, newy, color);
                    }
                }
            }

            result_Image= Denoise_func(result_Image);
            Result_Image.Source= Bitmap_To_ImageSource(result_Image);
        }

        private Bitmap Denoise_func(Bitmap bitmap)
        {
            Bitmap after_bitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    //int value = 0;
                    int[] R = new int[9];
                    int[] G = new int[9];
                    int[] B = new int[9];

                    int rsum = 0, gsum = 0, bsum = 0;
                    int i = 0;
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        for (int ky = -1; ky <= 1; ky++)
                        {
                            int nx = x + kx;
                            int ny = y + ky;//算出周圍的點的位置

                            // 檢查是否在圖像範圍內
                            if (nx >= 0 && nx < bitmap.Width && ny >= 0 && ny < bitmap.Height)
                            {
                                Color pixel = bitmap.GetPixel(x + kx, y + ky);
                                R[i] = pixel.R;
                                G[i] = pixel.G;
                                B[i] = pixel.B;
                            }
                            else
                            {
                                R[i] = 0;
                                G[i] = 0;
                                B[i] = 0;
                            }
                            i++;
                        }
                    }

                    Array.Sort(R);
                    Array.Sort(G);
                    Array.Sort(B);

                    rsum = R[4];
                    gsum = G[4];
                    bsum = B[4];

                    after_bitmap.SetPixel(x, y, Color.FromArgb(rsum, gsum, bsum));
                }
            }

            return after_bitmap;
        }
    }

    
}
