using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;


/*
 * 工具类
 */
public static class Common
{

 /**
 * 将二进制字符转为十进制byte类型
 * @param bin 二进制字符
 * @return 十进制byte
 */
    public static byte binaryToDecimal(String bin)
    {
        return (byte)Convert.ToInt32(bin,2);
    }



/**
 * 合并两个数组
 * @param previous 	前一个数组
 * @param next		后一个数组
 * @return
 */
    public static byte[] mergeBytes(byte[] previous, byte[] next)
    {
        if (previous == null)
        {
            return next;
        }

        byte[] result = new byte[previous.Length + next.Length];
        Array.Copy(previous, 0, result, 0, previous.Length);
        Array.Copy(next, 0, result, previous.Length, next.Length);
        return result;
    }



    /**
	 * 读取一个txt文件
	 * @param path 文件的路径
	 * @return 返回读取结果，如果失败返回null
	 */
    public static String readTxt(String path)
    {
        String content = "";
        StreamReader sr = new StreamReader(path, Encoding.UTF8);
        String line;
        while ((line = sr.ReadLine())!= null)
        {
            content += "\n" + line;
        }
        sr.Close();
        if (content.StartsWith("\n"))
        {
            content = content.Substring(1);
        }


        return content;
    }
}

