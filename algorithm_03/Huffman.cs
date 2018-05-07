using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;


/*
 *霍夫曼压缩类
 *
 */

public class Huffman
{
    /**
	 * 通过文件获取各个字符的出现频率，生成哈夫曼树，根据树生成哈夫曼编码，根据编码对源文件进行压缩
	 */
    public Dictionary<string, string> huffCode = new Dictionary<string, string>();//key为字符，value为编码
    public static string content = "";
    private int supply = 0;//记录补充位

    private const int BUFFER_SIZE = 1024;//缓冲区长度
    private const string COMPRESS_POSTFIX = ".huffp";//压缩文件后缀名


    //Huffman树节点
    private  class Node
    {
        public char data;//字符
        public int rate;//出现频率
        public Node rch;
        public Node lch;
    }


    public void compressFile(string path)
    {//根据文件创建压缩文件

        /**
		 * 1、根据文件获取字符串--对文件进行压缩应对二进制每8位进行遍历
		 * 2、根据字符串获取每个字符的出现频率
		 * 3、根据出现频率构建哈夫曼树
		 * 4、根据哈夫曼树生成哈夫曼编码
		 * 5、根据哈夫曼编码将文件生成哈夫曼二进制码
		 * 6、规范将编码写入二进制中****不添加解压
		 */
        string fileName = path.Substring(0, path.LastIndexOf("."));
        content = Common.readTxt(path);

        Console.WriteLine(">>> 开始压缩文件："+ fileName);

        Node root = getHuffmanTreeRoot(createRateList(content))[0];
        createHuffmanCode(root, "");

        createCompressFile(fileName + COMPRESS_POSTFIX, getCompressResult());

        Console.WriteLine("<<< 文件压缩完毕:" + fileName + COMPRESS_POSTFIX);
    }

    /**
	 * 创建频率数组
	 * @param content
	 * @return
	 */
    private List<Node> createRateList(string content)
    {
        Console.WriteLine("开始频率统计...");
        List<Node> charRateList = new List<Node>();
        Dictionary<string, int> charRateMap = new Dictionary<string, int>();//频率统计
        char[] chars = new char[BUFFER_SIZE];
        //分段统计文本，获取频率值，通过map方式进行获取
        for (int i = 0; i < content.Length;)
        {
            if (i + BUFFER_SIZE < content.Length)
            {
                chars = content.ToCharArray(i, BUFFER_SIZE);
                i = i + BUFFER_SIZE;
                for (int j = 0; j < chars.Length; j++)
                {
                    if (!charRateMap.ContainsKey(chars[j].ToString()))
                    {//没有值则添加一个值
                        charRateMap.Add(chars[j].ToString(), 1);
                    }
                    else
                    {
                        charRateMap[chars[j].ToString()] = charRateMap[chars[j].ToString()] +1;//有值则进行加一计算
                    }
                }
            }
            else
            {
                chars = content.ToCharArray(i, content.Length);

                for (int j = 0; j < content.Length - i; j++)
                {
                    if (!charRateMap.ContainsKey(chars[j].ToString()))
                    {//没有值则添加一个值
                        charRateMap.Add(chars[j].ToString(), 1);
                    }
                    else
                    {
                        charRateMap[chars[j].ToString()] = charRateMap[chars[j].ToString()] + 1;//有值则进行加一计算
                    }
                }
                i = content.Length;
            }
        }

        /**
		 * 根据频率map生成频率数组
		 */
        foreach (var item in charRateMap)
        {
            Node node = new Node();
            node.data = Convert.ToChar( item.Key);
            node.rate = item.Value;
            charRateList.Add(node);
        }

        return charRateList;
    }

    /**
	 * 根据频率列表生成哈夫曼树,使用贪心算法
	 * @param list
	 * @return
	 */
    private List<Node> getHuffmanTreeRoot(List<Node> list)
    {
        if (list.Count < 2 || list.Count ==0)
        {
            return list;
        }
        //升序排序频率列表
        list.Sort((o1, o2)=> { return o1.rate - o2.rate; });
        //合并频率最小的两个节点，生成新节点
        Node node = new Node();
        node.lch = list[0];
        node.rch = list[1];
        node.rate = node.lch.rate + node.rch.rate;
        list.Remove(list[1]);
        list.Remove(list[0]);
        list.Add(node);
        getHuffmanTreeRoot(list);
        return list;
    }

    /**
	 * 根据哈夫曼树获取哈夫曼编码
	 * @return 哈夫曼编码
	 */
    private void createHuffmanCode(Node node, String code)
    {
        if (node == null)
        {
            return;
        }
        if (node.data == '\0')
        {//非叶子节点
            createHuffmanCode(node.lch, code + "0");
            createHuffmanCode(node.rch, code + "1");
        }
        else
        {//叶子节点
            huffCode.Add(node.data.ToString(), code);
        }
    }


    /**
	 * 获取压缩结果
	 * @return
	 */
    private byte[] getCompressResult()
    {
        string result = "";
        byte[] resultBytes = null;
        byte[] buffer = new byte[BUFFER_SIZE];
        for (int i = 0; i < content.Length; i++)
        {
            result += huffCode[content.Substring(i,1)];
        }

        supply = 8 - result.Length % 8;
        switch (8 - supply)
        {//补全编码
            case 7:
                result += "0";
                break;
            case 6:
                result += "00";
                break;
            case 5:
                result += "000";
                break;
            case 4:
                result += "0000";
                break;
            case 3:
                result += "00000";
                break;
            case 2:
                result += "000000";
                break;
            case 1:
                result += "0000000";
                break;
            default:
                break;
        }
        for (int i = 0; i < result.Length;)
        {
            if (i + 8 * BUFFER_SIZE > result.Length)
            {
                buffer = new byte[(result.Length - i) / 8];
                for (int j = 0; j < result.Length - i; j += 8)
                {
                    buffer[j / 8] = Common.binaryToDecimal(result.Substring(j,8));
                }
                resultBytes = Common.mergeBytes(resultBytes, buffer);
                i = result.Length;
            }
            else
            {
                buffer = new byte[BUFFER_SIZE];
                for (int j = 0; j < 8 * BUFFER_SIZE; j += 8)
                {
                    buffer[j / 8] = Common.binaryToDecimal(result.Substring(j, 8));
                }
                resultBytes = Common.mergeBytes(resultBytes, buffer);
                i += 8 * BUFFER_SIZE;
            }
        }
        return resultBytes;
    }

    /**
	 * 根据压缩结果创建压缩文件
	 * @param path	压缩文件保存路径
	 * @param compressBytes	压缩结果
	 * @return 创建结果
	 */
    private void createCompressFile(String path, byte[] compressBytes)
    {
        //StreamWriter fop = null;
        FileStream fop = new FileStream(path, FileMode.Create);
        //fop= File.CreateText(path);
        fop.Write(compressBytes,0,compressBytes.Length);
        //fop.Flush();
        fop.Close();

        Console.WriteLine("已生成哈夫曼压缩文件：" + path);
    }

 
    public void clear()
    {
        huffCode = new Dictionary<string, string>();
        content = "";
    }
}