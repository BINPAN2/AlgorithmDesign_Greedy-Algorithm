using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 * 测试类
 */

public class Test
{
    static void Main(string[] args)
    {
        Huffman huff = new Huffman();
        for (int i = 1; i < 6; i++)
        {
            huff.compressFile("data/input_assign03_0" + i + ".txt");
            huff.clear();
        }
        Console.ReadKey();
    }
}