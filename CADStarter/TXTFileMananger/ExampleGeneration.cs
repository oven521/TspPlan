using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADEngine;
using CADEngine.DrawingObject;
using MainHMI;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Tsp;
namespace TXTMananger
{
    public class ExampleGeneration
    {

        public ExampleGeneration(int count = 100)//点的总个数
        {
            using (StreamWriter stream = new StreamWriter("随机例子.txt"))
            {
                for (int i = 0; i < count; i++)
                {
                    stream.WriteLine(Math.Abs(GetRandomSeed() % 5000) + " " + Math.Abs(GetRandomSeed() % 5000));
                }
            }
        }
        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
