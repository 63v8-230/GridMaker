using System.Drawing;
using System.Drawing.Imaging;

namespace GridMaker
{
    internal class Program
    {
        static long maxProcess;
        static long currentProcess = 0;

        static bool isDo = true;

        static void Main()
        {
            Console.Write("サイズ(px): ");
            int size;
            if (!int.TryParse(Console.ReadLine(), out size))
                size = 1024;
            //int size = int.Parse(Console.ReadLine());

            Bitmap b = new Bitmap(size,size);

            Console.Write("線の太さ(px): ");
            int linePx;
            if (!int.TryParse(Console.ReadLine(), out linePx))
                linePx = 5;

            Console.Write("1列あたりグリッドの数: ");
            int gridCount;
            if(!int.TryParse(Console.ReadLine(), out gridCount))
                gridCount = 8;
            int gridSize = ((size-linePx) / gridCount) - linePx;

            Console.Write("線の色(255までのRGBを空白で区切る): ");
            int[] lRGB = new int[3];
            var c = Console.ReadLine().Split(' ', '　');
            if (c.Length < 3)
                c = ["30","30","30"];

            for (int i = 0; i < lRGB.Length; i++) 
            {
                if (!int.TryParse(c[i], out lRGB[i]))
                    lRGB[i] = 30;

            }

            Console.Write("グリッドの色(255までのRGBを空白で区切る): ");
            int[] gRGB = new int[3];
            c = Console.ReadLine().Split(' ', '　');
            if (c.Length < 3)
                c = ["200", "200", "200"];

            for (int i = 0; i < gRGB.Length; i++) 
            {
                if (!int.TryParse(c[i], out gRGB[i]))
                    gRGB[i] = 200;

            }

            int unitPx = gridSize + linePx;

            maxProcess = size * size;
            currentProcess = 0;

            Task.Run(PrintProcess);

            for (int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                        

                    if (x - linePx >= unitPx * gridCount || y - linePx >= unitPx * gridCount)
                    {
                        currentProcess++;
                        continue;
                    }
                        

                        if (x<linePx || y<linePx)
                    {
                        b.SetPixel(x, y, Color.FromArgb(lRGB[0], lRGB[1], lRGB[2]));
                        continue;
                    }

                    var cx = (x - linePx) % unitPx;
                    var cy = (y - linePx) % unitPx;

                    if(cx<gridSize && cy < gridSize)
                    {
                        b.SetPixel(x, y, Color.FromArgb(gRGB[0], gRGB[1], gRGB[2]));
                    }
                    else
                    {
                        b.SetPixel(x, y, Color.FromArgb(lRGB[0], lRGB[1], lRGB[2]));
                    }

                    currentProcess++;
                }
            }


            isDo = false;
            b.Save("image.png");

            Console.WriteLine("image.pngに保存済み\n\n何かキーを押して終了");
            Console.ReadKey();
        }

        private static async Task PrintProcess()
        {

            await Task.Yield();


            while(isDo)
                Console.WriteLine($"{currentProcess}/{maxProcess} | {((double)currentProcess / maxProcess)*100:F2}%");

        }

    }
}
