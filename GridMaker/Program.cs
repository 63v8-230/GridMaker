using System.Drawing;
using System.Drawing.Imaging;

namespace GridMaker
{
    internal class Program
    {
        //最大プロセス数
        static ulong maxProcess;

        //現在のプロセス数
        static ulong currentProcess = 0;

        //実行中フラグ
        static bool isDo = true;

        //Bitmapクラスの画像サイズ制限は1GB (GDI+依存)
        const ulong MEMORY_SIZE_LIMIT = (1) * 1024 * 1024 * 1024;

        //1pxあたりのサイズ
        const ushort BYTE_PER_PIXEL = 4;

        static void Main()
        {
            int pxSizeLimit = (int)MathF.Sqrt((float)(MEMORY_SIZE_LIMIT / BYTE_PER_PIXEL));

            Console.Write($"サイズ(px / 最大{pxSizeLimit:#,0}px): ");
            int size;
            if (!int.TryParse(Console.ReadLine(), out size))
                size = 1024;

            if (size > pxSizeLimit)
                size = pxSizeLimit;

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
            int[] lRGB = [30, 30, 30];
            var c = Console.ReadLine().Split(' ', '　');
            int indexMax = Math.Min(lRGB.Length, c.Length);

            for (int i = 0; i < indexMax; i++) 
            {
                int.TryParse(c[i], out lRGB[i]);
            }

            Console.Write("グリッドの色(255までのRGBを空白で区切る): ");
            int[] gRGB = [40, 180, 50];
            c = Console.ReadLine().Split(' ', '　');
            indexMax = Math.Min(gRGB.Length, c.Length);

            for (int i = 0; i < indexMax; i++) 
            {
                int.TryParse(c[i], out gRGB[i]);
            }

            //境界含めた1セル辺りのサイズ
            int unitPx = gridSize + linePx;

            maxProcess = (ulong)(size * size);
            currentProcess = 0;

            //進行度表示スレッド
            Task.Run(PrintProcess);

            for (int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                    //描画するピクセルが範囲外なら何もしない
                    //最初の境界線の分を引く
                    if (x - linePx >= unitPx * gridCount || y - linePx >= unitPx * gridCount)
                    {
                        currentProcess++;
                        continue;
                    }

                    //最初の境界線描画
                    if (x<linePx || y<linePx)
                    {
                        b.SetPixel(x, y, Color.FromArgb(lRGB[0], lRGB[1], lRGB[2]));
                        continue;
                    }

                    //現在の描画位置をセル内での相対位置に変換
                    //最初の境界線の分を引く
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

            //実行中フラグを消す
            isDo = false;
            b.Save("image.png");

            Console.WriteLine("image.pngに保存済み\n\n何かキーを押して終了");
            Console.ReadKey();
        }

        private static async Task PrintProcess()
        {

            await Task.Yield();

            //画像処理が実行中の間繰り返す
            while(isDo)
                Console.WriteLine($"{currentProcess}/{maxProcess} | {((double)currentProcess / maxProcess)*100:F2}%");

        }

    }
}
