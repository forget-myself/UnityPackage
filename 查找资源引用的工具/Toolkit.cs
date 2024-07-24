//���ܼ���
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;
using System.Diagnostics;

public class Tooklit
{
    #region �޸��˺�
    [MenuItem("DGH/�޸��˺�")]
    public static void ChangeUserUID()
    {
        // ��ȡ��Ŀ�ľ���·��
        string projectPath = Application.dataPath;

        // ��ȡ��Ŀ�ĸ�Ŀ¼·��
        string projectParentPath = Path.GetDirectoryName(projectPath);
        string filePath = Path.Combine(projectParentPath, "tool", "loginconfig");
        if (File.Exists(filePath))
        {
            System.Diagnostics.Process.Start("notepad.exe", filePath);
        }

        UnityEngine.Debug.Log("Project parent path: " + projectParentPath);
    }
    #endregion

    #region �������־
    static string originalLogPath = "C:\\Users\\"+ System.Environment.UserName+ "\\AppData\\Local\\Unity\\Editor\\Editor.log";
    [MenuItem("DGH/����־(ȫ)")]
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
    [MenuItem("DGH/����־��10M��")]
    static void OpenLogSegment1()
    {
        var targetPath = "D:\\Bug\\" + System.DateTime.Now.ToString("D") + "\\";
        targetPath = targetPath.Replace(" ", "").Replace("\n", "").Replace("\\n", "");
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }
        string newFilePath = targetPath + System.DateTime.Now.ToString("HH_mm_ss_") + "Editor.log";
        // ��ȡԴ�ļ�
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
                // ��ȡ���1000���ַ�
                string lastNChars = sourceReader.ReadToEnd();
                // д��Ŀ���ļ�
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

    // Ҫ��ȡ������
    private const int maxLineCount = 20000;
    
    [MenuItem("DGH/����־��2���У�")]
    static void OpenLogSegment2()
    {
        // ���ļ���
        string filePath = originalLogPath;
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            // ����һ����������С�����Ը���ʵ��������е�����
            int bufferSize = 1024 * 1024; // 1MB

            // ����һ���ֽ����飬���ڴ洢����������
            byte[] buffer = new byte[bufferSize];

            // ���ļ���ָ���Ƶ��ļ�ĩβ
            fileStream.Seek(0, SeekOrigin.End);

            // ����һ�������������ڼ�¼�Ѿ���ȡ������
            int lineCount = 0;

            // ����һ���б����ڴ洢��� n �е��ı�
            var lastNLines = new List<string>();

            // ���ļ�ĩβ��ʼ��ǰ������ֱ���ҵ� n �е�����
            while (fileStream.Position > 0 && lineCount < maxLineCount)
            {
                // ����Ҫ��ȡ���ֽ���
                int bytesToRead = (int)Math.Min(bufferSize, fileStream.Position);

                // ���ļ���ָ���Ƶ���ǰλ�õ�ǰһ����������С��
                fileStream.Seek(-bytesToRead, SeekOrigin.Current);

                // ��ȡ����������
                fileStream.Read(buffer, 0, bytesToRead);

                // ������������ת��Ϊ�ַ���
                string bufferString = Encoding.UTF8.GetString(buffer);

                // ���ַ������зָ�
                string[] lines = bufferString.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                // �������һ��
                if (lastNLines.Count > 0)
                {
                    lines[0] = lastNLines[lastNLines.Count - 1] + lines[0];
                    lastNLines.RemoveAt(lastNLines.Count - 1);
                }

                // ����� n �е��ı���ӵ��б���
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
            // ������� n �е��ı�
            int aaa =lastNLines.Count;
            System.Diagnostics.Process.Start("notepad.exe", destPath);
        }
    }

    [MenuItem("DGH/����־Ŀ¼")]
    static void OpenLogD()
    {
        // ���ļ���
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