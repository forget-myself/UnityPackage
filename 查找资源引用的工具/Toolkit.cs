//功能集合
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Diagnostics;

public class Tooklit
{
    #region 修改账号
    [MenuItem("DGH/修改账号")]
    public static void ChangeUserUID()
    {
        // 获取项目的绝对路径
        string projectPath = Application.dataPath;

        // 获取项目的父目录路径
        string projectParentPath = Path.GetDirectoryName(projectPath);
        string filePath = Path.Combine(projectParentPath, "tool", "loginconfig");
        if (File.Exists(filePath))
        {
            System.Diagnostics.Process.Start("notepad.exe", filePath);
        }

        UnityEngine.Debug.Log("Project parent path: " + projectParentPath);
    }
    #endregion

    #region 打开最近日志
    static string originalLogPath = "C:\\Users\\"+ System.Environment.UserName+ "\\AppData\\Local\\Unity\\Editor\\Editor.log";
    [MenuItem("DGH/打开日志(全)")]
    static void OpenWholeLog()
    {
        var targetPath = "D:\\Bug\\" + System.DateTime.Now.ToString("D") + "\\";
        targetPath = targetPath.Replace(" ", "").Replace("\n", "").Replace("\\n", "");
        if (!Directory.Exists(targetPath))
        {
            //  UnityEngine.Debug.Log("targetPath  " + targetPath);
            Directory.CreateDirectory(targetPath);
        }
        string newFilePath = targetPath + System.DateTime.Now.ToString("HH_mm_ss_") + "Editor_Whole.log";
        File.Copy(originalLogPath, newFilePath);
        System.Diagnostics.Process.Start(newFilePath);
    }

    private static int segmentSize = 1024 * 1024 * 10;
    [MenuItem("DGH/打开日志（10M）")]
    static void OpenLogSegment1()
    {
        var targetPath = "D:\\Bug\\" + System.DateTime.Now.ToString("D") + "\\";
        targetPath = targetPath.Replace(" ", "").Replace("\n", "").Replace("\\n", "");
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        string newFilePath = targetPath + System.DateTime.Now.ToString("HH_mm_ss_") + "Editor.log";
        // 读取源文件
        string sourcePath = originalLogPath;
        //using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
        using (var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            //new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
        {
            using (var sourceReader = new StreamReader(sourceStream))
            {
                long fileLength = sourceStream.Length;
                long readStart = (long)Mathf.Max(0, fileLength - segmentSize);
                sourceStream.Seek(readStart, SeekOrigin.Begin);
                // 获取最后1000个字符
                string lastNChars = sourceReader.ReadToEnd();
                // 写入目标文件
                string destPath = newFilePath;
                using (var destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
                {
                    using (var destWriter = new StreamWriter(destStream))
                    {
                        destWriter.Write(lastNChars);
                    }
                }
            }
            System.Diagnostics.Process.Start("D:\\Software\\Dev\\SublimeTextv4.0.4113\\sublime_text.exe", newFilePath);
        }
    }

    // 要读取的行数
    private const int maxLineCount = 20000;
    
    [MenuItem("DGH/打开日志（2万行）")]
    static void OpenLogSegment2()
    {
        // 打开文件流
        string filePath = originalLogPath;
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            // 定义一个缓冲区大小（可以根据实际情况进行调整）
            int bufferSize = 1024 * 1024; // 1MB

            // 定义一个字节数组，用于存储缓冲区数据
            byte[] buffer = new byte[bufferSize];

            // 将文件流指针移到文件末尾
            fileStream.Seek(0, SeekOrigin.End);

            // 定义一个计数器，用于记录已经读取的行数
            int lineCount = 0;

            // 定义一个列表，用于存储最后 n 行的文本
            var lastNLines = new List<string>();

            // 从文件末尾开始向前搜索，直到找到 n 行的行首
            while (fileStream.Position > 0 && lineCount < maxLineCount)
            {
                // 计算要读取的字节数
                int bytesToRead = (int)Math.Min(bufferSize, fileStream.Position);

                // 将文件流指针移到当前位置的前一个缓冲区大小处
                fileStream.Seek(-bytesToRead, SeekOrigin.Current);

                // 读取缓冲区数据
                fileStream.Read(buffer, 0, bytesToRead);

                // 将缓冲区数据转换为字符串
                string bufferString = Encoding.UTF8.GetString(buffer);

                // 将字符串按行分割
                string[] lines = bufferString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                // 处理最后一行
                if (lastNLines.Count > 0)
                {
                    lines[0] = lastNLines[lastNLines.Count - 1] + lines[0];
                    lastNLines.RemoveAt(lastNLines.Count - 1);
                }

                // 将最后 n 行的文本添加到列表中
                for (int i = lines.Length - 1; i >= 0; i--)
                {
                    if (lineCount >= maxLineCount)
                    {
                        break;
                    }

                    lastNLines.Insert(0, lines[i]);
                    lineCount++;
                }
            }

            var targetPath = "D:\\Bug\\" + System.DateTime.Now.ToString("D") + "\\";
            targetPath = targetPath.Replace(" ", "").Replace("\n", "").Replace("\\n", "");
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
            string destPath = targetPath + System.DateTime.Now.ToString("HH_mm_ss_") + "Editor.log";
            using (var destStream = new FileStream(destPath, FileMode.Create, FileAccess.Write))
            {
                using (var destWriter = new StreamWriter(destStream))
                {
                    foreach(var line in lastNLines)
                    {
                        destWriter.WriteLine(line.TrimEnd('\0'));
                    }
                }
            }
            // 返回最后 n 行的文本
            int aaa =lastNLines.Count;
            System.Diagnostics.Process.Start("notepad.exe", destPath);
        }
    }

    [MenuItem("DGH/打开日志目录")]
    static void OpenLogD()
    {
        // 打开文件流
        var targetPath = "D:\\Bug\\" + System.DateTime.Now.ToString("D") + "\\";
        targetPath = targetPath.Replace(" ", "").Replace("\n", "").Replace("\\n", "");
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        System.Diagnostics.Process.Start("explorer.exe", targetPath);
    }
    #endregion
}