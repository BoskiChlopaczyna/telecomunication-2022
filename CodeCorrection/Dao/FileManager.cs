﻿using System.Collections;
using System.Text;

namespace Zadanie_1.Dao;

public static class FileManager
{
    public static readonly string BaseDataDirPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
            "telekomunikacja_2022"
        );

    public static void EncodeFile(string fileName)
    {
        var filePath = Path.Combine(BaseDataDirPath, fileName);

        List<int> bitsList = ReadFile(filePath);
        var buffer = new List<int>();
        var sb = new StringBuilder();
        
        for (var i = 0; i < bitsList.Count; i++)
        {
            buffer.Add(bitsList[i]);

            if (buffer.Count % 8 == 0)
            {
                var encodedBuffer = Code.Encode(buffer);
                foreach (int bit in encodedBuffer) sb.Append(bit);
                if (i != bitsList.Count - 1) sb.AppendLine();
                buffer.Clear();
            }
        }

        string encodedFilePath = Path.Combine(BaseDataDirPath, "encoded_" + fileName);
        File.WriteAllText(encodedFilePath, sb.ToString());
        Console.WriteLine($"File encoded as {encodedFilePath}");
    }

    public static void DecodeFile(string fileName)
    {
        var filePath = Path.Combine(BaseDataDirPath, fileName);
        if (!File.Exists(filePath)) throw new FileNotFoundException($"File {filePath} not found!");

        var bits = new List<bool>();
        foreach (var line in File.ReadLines(filePath))
        {
            var chunk = new List<int>();
            foreach (var bit in line)
            {
                var bitInt = (int)Char.GetNumericValue(bit);
                chunk.Add(bitInt);
            }
            var decodedChunk = Code.Decode(chunk);
            decodedChunk.ForEach(value => bits.Add(value == 1));
        }

        var bitArray = new BitArray(bits.ToArray());
        byte[] bytes = new byte[bitArray.Length / 8];
        bitArray.CopyTo(bytes, 0);

        string decodedFilePath = Path.Combine(BaseDataDirPath, "decoded_" + fileName);
        File.WriteAllBytes(decodedFilePath, bytes);
        Console.WriteLine($"File decoded as {decodedFilePath}");
    }

    private static List<int> ReadFile(string filePath)
    {
        if (!File.Exists(filePath)) throw new FileNotFoundException($"File {filePath} not found!");

        var bytes = File.ReadAllBytes(filePath);
        var bitsList = new List<int>();
        var bits = new BitArray(bytes);

        foreach (bool bit in bits)
        {
            var converted = bit ? 1 : 0;
            bitsList.Add(converted);
        }

        return bitsList;
    }
}