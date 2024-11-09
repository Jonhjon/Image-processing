using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.IO;
using System.Drawing;
using Color = System.Drawing.Color;
using System.Drawing.Imaging;
using System.Windows.Media.Media3D;




namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)//二值化
        {
            int slifer_change = (int)e.NewValue;
            Textbox1.Text = "閥值 : " + slifer_change.ToString();
            if (Origin_Image.Source is BitmapImage bitmapImage)//檢查Image.source是否為bitmapImage的類型
            {
                Bitmap colorBitmap = Image_To_Bitmap((BitmapImage)Origin_Image.Source);//把Origin裡的圖像轉換成Bitmap
                Bitmap grayBitmap = new Bitmap(colorBitmap.Width, colorBitmap.Height);//創一個

                for (int j = 0; j < colorBitmap.Height; j++)//歷便每一個像素
                {
                    for (int i = 0; i < colorBitmap.Width; i++)
                    {
                        Color originalColor = colorBitmap.GetPixel(i, j);
                        int value = originalColor.R;//取灰階圖的其中一個像素
                        if (value >= slifer_change)
                        {//判斷項素有沒有大於閥值
                            Color color = Color.FromArgb(255, 255, 255);//大於就把該像素點設為最大值
                            grayBitmap.SetPixel(i, j, color);
                        }
                        else
                        {
                            Color color = Color.FromArgb(0, 0, 0);//反之就設為最小值
                            grayBitmap.SetPixel(i, j, color);
                        }
                    }
                }
                after_Image.Source = Bitmap_To_ImageSource(grayBitmap);//把計算好的圖片
            }
        }
        private void Grayscale_conversion(object sender, RoutedEventArgs e)//灰階處理
        {
            if (Origin_Image == null)
            {
                MessageBox.Show("請先選擇一張圖片！");
                return;
            }
            if (Origin_Image.Source is BitmapImage bitmapImage)//檢查Image.source是否為bitmapImage的類型
            {
                Bitmap colorBitmap = Image_To_Bitmap(bitmapImage);
                // 建立灰階 Bitmap
                Bitmap grayBitmap = new Bitmap(colorBitmap.Width, colorBitmap.Height);

                for (int y = 0; y < colorBitmap.Height; y++)
                {
                    for (int x = 0; x < colorBitmap.Width; x++)
                    {
                        Color originalColor = colorBitmap.GetPixel(x, y);
                        int grayValue = (int)(originalColor.R * 0.3 + originalColor.G * 0.59 + originalColor.B * 0.11);//使用加權法去做影像的調整
                        Color grayColor = Color.FromArgb(grayValue, grayValue, grayValue);
                        grayBitmap.SetPixel(x, y, grayColor);
                    }
                }
                after_Image.Source = Bitmap_To_ImageSource(grayBitmap);//把計算好的圖片

            }
        }

        private void Channel_onversion(object sender, RoutedEventArgs e)//通道轉換
        {
            if (Origin_Image.Source is BitmapImage bitmapImage)
            {
                // 將 BitmapImage 轉換為 Bitmap
                Bitmap colorBitmap = Image_To_Bitmap(bitmapImage);
                // 建立分別顯示 R、G、B 通道的 Bitmap
                Bitmap R_Bitmap = new Bitmap(colorBitmap.Width, colorBitmap.Height);
                Bitmap G_Bitmap = new Bitmap(colorBitmap.Width, colorBitmap.Height);
                Bitmap B_Bitmap = new Bitmap(colorBitmap.Width, colorBitmap.Height);

                for (int i = 0; i < colorBitmap.Width; i++)
                {
                    for (int j = 0; j < colorBitmap.Height; j++)
                    {
                        Color origin_color = colorBitmap.GetPixel(i, j);//取得該像素的RGB三種顏色

                        Color R_gray_value = Color.FromArgb(origin_color.R, origin_color.R, origin_color.R);//
                        Color G_gray_value = Color.FromArgb(origin_color.G, origin_color.G, origin_color.G);
                        Color B_gray_value = Color.FromArgb(origin_color.B, origin_color.B, origin_color.B);

                        R_Bitmap.SetPixel(i, j, R_gray_value);
                        G_Bitmap.SetPixel(i, j, G_gray_value);
                        B_Bitmap.SetPixel(i, j, B_gray_value);
                    }
                }

                Image_R.Source = Bitmap_To_ImageSource(R_Bitmap);
                Image_G.Source = Bitmap_To_ImageSource(G_Bitmap);
                Image_B.Source = Bitmap_To_ImageSource(B_Bitmap);
            }
        }

        private void UploadImageButton_Click(object sender, RoutedEventArgs e)//上傳影像到origin_Image
        {
            // 建立一個 OpenFileDialog 實例
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg";

            // 檢查使用者是否選擇了圖片
            if (openFileDialog.ShowDialog() == true)
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

        private void Erosion_Expansion(object sender, RoutedEventArgs e)//侵蝕擴張
        {
            if (Origin_Image.Source is BitmapImage bitmapImage)
            {
                // 將 BitmapImage 轉換為 Bitmap
                Bitmap colorBitmap = Image_To_Bitmap(bitmapImage);

                Bitmap Erosion = new Bitmap(colorBitmap.Width, colorBitmap.Height);
                Bitmap Expansion = new Bitmap(colorBitmap.Width, colorBitmap.Height);

                Erosion = Erosion_func(colorBitmap);
                Erosion = Erosion_func(Erosion);

                Expansion = Expansion_func(colorBitmap);
                Expansion = Expansion_func(Expansion);

                after_Image.Source = Bitmap_To_ImageSource(Erosion);
                Expansion_Image.Source = Bitmap_To_ImageSource(Expansion);
            }
        }


        private Bitmap Erosion_func(Bitmap bitmap)//侵蝕
        {
            Bitmap Erosion = new Bitmap(bitmap.Width, bitmap.Height);

            for (int x = 0; x < bitmap.Width ; x++)
            {
                for (int y = 0; y < bitmap.Height ; y++)
                {
                    bool ersion = false;
                    for (int kx = -2; kx <= 2; kx++)//判斷該像素周圍5 * 5的是像素否有白色出現
                    {
                        for (int ky = -2; ky <= 2; ky++)
                        {

                            int nx = x + kx;
                            int ny = y + ky;//算出周圍的點的位置

                            // 檢查是否在圖像範圍內
                            if (nx >= 0 && nx < bitmap.Width && ny >= 0 && ny < bitmap.Height && bitmap.GetPixel(x + kx, y + ky).R != 255)//判斷像素是否為白色
                            {
                                ersion = true;
                                break;
                            }
                        }
                        if (ersion)
                            break;
                    }
                    if (ersion)//如果有就設置為白色反之設置黑色
                        Erosion.SetPixel(x, y, Color.Black);
                    else
                        Erosion.SetPixel(x, y, Color.White);
                }
            }
            return Erosion;
        }

        private Bitmap Expansion_func(Bitmap bitmap)//擴張
        {
            Bitmap Expansion = new Bitmap(bitmap.Width, bitmap.Height);

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bool expansion = false;
                    for (int kx = -2; kx <= 2; kx++)//該像素5*5的範圍
                    {
                        for (int ky = -2; ky <= 2; ky++)
                        {
                            int nx = x + kx;
                            int ny = y + ky;//算出周圍的點的位置

                            // 檢查是否在圖像範圍內
                            if (nx >= 0 && nx < bitmap.Width && ny >= 0 && ny < bitmap.Height && bitmap.GetPixel(x + kx, y + ky).R == 255)//判斷像素是否為白色
                            {
                                expansion = true;
                                break;
                            }
                        }
                        if (expansion)
                            break;
                    }
                    if (expansion)
                        Expansion.SetPixel(x, y, Color.White);
                    else
                        Expansion.SetPixel(x, y, Color.Black);
                }
            }
            return Expansion;
        }

        private void Denoise(object sender, RoutedEventArgs e)//去雜訊，使用中值法
        {
            Bitmap bitmap = Image_To_Bitmap((BitmapImage)Origin_Image.Source);
            Bitmap after_bitmap = new Bitmap(bitmap.Width, bitmap.Height);

            after_bitmap = Denoise_func(bitmap);
            after_Image.Source = Bitmap_To_ImageSource(after_bitmap);

        }

        private Bitmap Denoise_func(Bitmap bitmap)
        {
            Bitmap after_bitmap = new Bitmap(bitmap.Width, bitmap.Height);
            for (int x = 0; x < bitmap.Width ; x++)
            {
                for (int y = 0; y < bitmap.Height ; y++)    
                {
                    //int value = 0;
                    int[] R = new int[9] ;
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
                                Color pixel = bitmap.GetPixel(x + kx, y + ky);//取得像素的顏色
                                R[i] = pixel.R;//分別放入三個不同顏色的陣列
                                G[i] = pixel.G;
                                B[i] = pixel.B;
                              
                            }
                            else//如果不在範圍內則設為0
                            {
                                R[i] = 0;//
                                G[i] = 0;
                                B[i] = 0;
                            }
                            i++;
                        }
                    }

                    Array.Sort(R);//排序
                    Array.Sort(G);
                    Array.Sort(B);

                    rsum = R[4];//取得中間值
                    gsum = R[4];
                    bsum = R[4];

                    after_bitmap.SetPixel(x, y, Color.FromArgb(rsum, gsum, bsum));
                }
            }

            return after_bitmap;
        }

        private void Button_Click(object sender, RoutedEventArgs e)//銳化
        {
            int[,] kernal = {
                { 0,-1,0},
                { -1,4,-1}, 
                { 0,-1,0}, 
            };//3*3的拉普拉斯矩陣

            Bitmap bitmap = Image_To_Bitmap((BitmapImage)Origin_Image.Source);//抓取原始的圖檔
            Bitmap result_bitmap = new Bitmap(bitmap.Width, bitmap.Height);//創建一個新的bitmap物件用來放算好的結果

            for(int x = 0; x < bitmap.Width; x++)
            {
                for(int y = 0; y < bitmap.Height; y++)//歷遍每一個像素
                {
                    Color color = bitmap.GetPixel(x, y);//抓取當個像素的顏色

                    int red =color.R,blue = color.B,green =color.G;//R、G、B三個顏色的值
                   
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        for (int ky = -1; ky <= 1; ky++)//當前像素的八鄰域
                        {

                            int nx = x + kx;
                            int ny = y + ky;//算出周圍的點的位置

                            // 檢查是否在圖像範圍內
                            if (nx >= 0 && nx < bitmap.Width && ny >= 0 && ny < bitmap.Height)
                            {
                                color = bitmap.GetPixel(nx, ny);//抓取周圍當下的像素的顏色
                                red += color.R * kernal[kx + 1, ky + 1];//算出顏色的值
                                green += color.G * kernal[kx + 1, ky + 1];
                                blue += color.B * kernal[kx + 1, ky + 1];
                            }

                        }
                    }
                    if (red > 255) red = 255;//判斷顏色的值是否有超出範圍
                    if(red <0) red = 0;

                    if(green >255) green= 255;
                    if(green<0) green= 0;

                    if(blue >255) blue= 255;
                    if(blue<0) blue= 0;

                    result_bitmap.SetPixel(x, y, Color.FromArgb(red, green, blue));//把結果賦值給儲存結果的物件
                }
            }
            after_Image.Source= Bitmap_To_ImageSource(result_bitmap);//放進Image控制項
        }

        private void edge(object sender, RoutedEventArgs e)//找邊界
        {
            Bitmap bitmap= Image_To_Bitmap((BitmapImage)Origin_Image.Source);
            Bitmap result_bitmap = new Bitmap(bitmap.Width, bitmap.Height);

            int[,] sobelX =
            {
                { -1,0,1},
                { -2,0,2},
                {-1,0,1}
            };
            int[,] sobelY =
            {
                { -1,-2,-1},
                { 0,0,0},
                { 1,2,1},
            };
            int threshold = 90;
            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)//歷遍每一個像素
                {
                    int gx = 0, gy = 0;

                    for (int kx = -1; kx <= 1; kx++)
                    {
                        for (int ky = -1; ky <= 1; ky++)//當前像素的八鄰域
                        {

                            int nx = x + kx;
                            int ny = y + ky;//算出周圍的點的位置

                            // 檢查是否在圖像範圍內
                            if (nx >= 0 && nx < bitmap.Width && ny >= 0 && ny < bitmap.Height)
                            {
                                Color pixel = bitmap.GetPixel(x + kx, y + ky);

                                int xKernel = sobelX[kx + 1, ky + 1];
                                int yKernel = sobelY[kx + 1, ky + 1];

                                gx += pixel.R * xKernel;
                                gy += pixel.R * yKernel;

                            }
                        }
                    }

                    int result_gray = (int)Math.Sqrt(gx * gx + gy * gy);//算出梯度

                    if(result_gray>=threshold) 
                        result_gray= 0;//黑色
                    else 
                        result_gray= 255;//白色

                    result_bitmap.SetPixel(x, y, Color.FromArgb(result_gray, result_gray, result_gray));

                }
            }
            after_Image.Source = Bitmap_To_ImageSource(result_bitmap);
        }

        private void Histogram_equalization(object sender, RoutedEventArgs e)//值方圖等化
        {
            Bitmap bitmap = Image_To_Bitmap((BitmapImage)Origin_Image.Source);
            Bitmap result_Image = new Bitmap(bitmap.Width,bitmap.Height);
            
            int width = bitmap.Width;
            int height = bitmap.Height;

            // 分別對 R、G、B 通道進行等化
            int[] histogramR = new int[256];
            int[] histogramG = new int[256];
            int[] histogramB = new int[256];

            //計算每個通道的直方圖
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    histogramR[pixel.R]++;
                    histogramG[pixel.G]++;
                    histogramB[pixel.B]++;
                }
            }

            // 計算每個通道的累積分佈函數 (CDF)
            int[] cdfR = new int[256];
            int[] cdfG = new int[256];
            int[] cdfB = new int[256];

            cdfR[0] = histogramR[0];//用於累積分布函數的陣列
            cdfG[0] = histogramG[0];
            cdfB[0] = histogramB[0];

            for (int i = 1; i < 256; i++)
            {
                cdfR[i] = cdfR[i - 1] + histogramR[i];
                cdfG[i] = cdfG[i - 1] + histogramG[i];
                cdfB[i] = cdfB[i - 1] + histogramB[i];
            }

            //標準化 CDF，並映射到 [0, 255]
            int totalPixels = width * height;

            int[] equalizedR = new int[256];
            int[] equalizedG = new int[256];
            int[] equalizedB = new int[256];

            for (int i = 0; i < 256; i++)
            {
                equalizedR[i] = (int)((cdfR[i]) / (double)(totalPixels) * 255);
                equalizedG[i] = (int)((cdfG[i]) / (double)(totalPixels) * 255);
                equalizedB[i] = (int)((cdfB[i]) / (double)(totalPixels) * 255);

                equalizedR[i] = Math.Max(0, Math.Min(255, equalizedR[i]));//確保範圍是在0~255，超過255或小於0則設為255跟0。
                equalizedG[i] = Math.Max(0, Math.Min(255, equalizedG[i]));
                equalizedB[i] = Math.Max(0, Math.Min(255, equalizedB[i]));
            }

            // 使用等化後的映射表生成新圖像
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);

                    int newR = equalizedR[pixel.R];
                    int newG = equalizedG[pixel.G];
                    int newB = equalizedB[pixel.B];

                    result_Image.SetPixel(x, y, Color.FromArgb(newR, newG, newB));
                }
            }

            after_Image.Source = Bitmap_To_ImageSource(result_Image);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Page1 page1= new Page1();
            
            NavigationWindow navigationWindow = new NavigationWindow();
            navigationWindow.Source = new Uri("Page1.xaml", UriKind.Relative);
            navigationWindow.Show();
        }
    }
}
