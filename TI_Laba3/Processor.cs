using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace TI_Laba3
{
    public class Processor
    {
        public delegate void MethodConteiner(int status, string source, string key, string crypto, long?[] keys, bool end, bool fast);
        public event MethodConteiner WokrComplete;

        private const int Dif = 23; 
        public static string FilePath, OutPath, KeyPath;
        public static int LfsrCount;
        public static long[] Key = new long[3];
        public static int[] LfsrSize = new int[3];
        private static byte[] _generedKey;
        public char Source, Crypto;
        public string SoureceString, CryptoString, KeyString;
        public static bool Fast;
        public static bool Lfsr;
        private static Polynom[] _polynoms = new Polynom[18];
        private long _maxProgress;
        public TextBlock Progr;

        public static void GetPolynoms(ref Polynom[] poly)
        {
            Activate(out poly[0], 23, 5);
            Activate(out poly[1], 24, 4, 3, 1);
            Activate(out poly[2], 25, 3);
            Activate(out poly[3], 26, 8, 7, 1);
            Activate(out poly[4], 27, 8, 7, 1);
            Activate(out poly[5], 28, 3);
            Activate(out poly[6], 29, 2);
            Activate(out poly[7], 30, 16, 15, 1);
            Activate(out poly[8], 31, 3);
            Activate(out poly[9], 32, 28, 27, 1);
            Activate(out poly[10], 33, 13);
            Activate(out poly[11], 34, 15, 14, 1);
            Activate(out poly[12], 35, 2);
            Activate(out poly[13], 36, 11);
            Activate(out poly[14], 37, 12, 10, 2);
            Activate(out poly[15], 38, 6, 5, 1);
            Activate(out poly[16], 39, 4);
            Activate(out poly[17], 40, 21, 19, 2);
        }
 
        private static void Activate(out Polynom pol, params int[] param)
        {
            pol = new Polynom(param.Count());
            for (var i = 0; i < pol.Count; i++)
                pol[i] = param[i];
        }
        
        public static long PrepareKey(string keyString)
        {
            long key = 0;
            var number = 0;
            foreach (var i in keyString.Select(c => c - '0'))
            {
                if (i != 0)
                    key = key | ((long)i << keyString.Length - number - 1);
                number++;
            }
            return key;
        }
         
        private static void GetNextKeyState(ref long key, Polynom poly)
        {
            long res = 0;
            for (var i = 0; i < poly.Count; i++)
                if (i == 0)
                    res = (key & (1L << (poly[i] - 1))) >> (poly[i] - 1);
                else
                    res ^= (key & (1L << (poly[i] - 1))) >> (poly[i] - 1);
            key <<= 1;
            key |= res;
        }

        public void Work()
        {
            var worker = new BackgroundWorker{WorkerReportsProgress = true};
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progr.Text = "" + (int) (((double) e.ProgressPercentage/_maxProgress)*100) + "%";
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int status;
            var keys = new long?[3];
            keys[0] = 0;
            keys[1] = 0;
            keys[2] = 0;
            SoureceString = string.Empty;
            CryptoString = string.Empty;
            KeyString = string.Empty;
            var sw = new BinaryWriter(new FileStream(OutPath, FileMode.Create, FileAccess.Write, FileShare.None));
            var file = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.None);
            _maxProgress = file.Length;
            var sr = new BinaryReader(file);
            if (Lfsr)
            {
                GetPolynoms(ref _polynoms);
                _generedKey = new byte[file.Length];
                if (LfsrCount == 1)
                {
                    status = 1;
                    for (var i = 0; i < file.Length; i++)//generate Key; Length = SourceFile.Length
                    {
                        byte k = 0;
                        for (var j = 0; j < 8; j++)
                        {
                            k |= (byte)(((Key[0] & (1 << (LfsrSize[0] - 1))) >> (LfsrSize[0] - 1)) << (7 - j));
                            GetNextKeyState(ref Key[0], _polynoms[LfsrSize[0] - Dif]);
                        }
                        ((BackgroundWorker) sender).ReportProgress(i);
                        _generedKey[i] = k;
                        var c = sr.ReadByte();
                        if (Fast)
                        {
                            SoureceString += (char)c;
                            CryptoString += (char)(c ^ k);
                            KeyString += k + " ";
                        }
                        else
                        {
                            Source = (char) c;
                            Crypto = (char) (c ^ k);
                            if (WokrComplete != null)
                                WokrComplete(status, Source.ToString(), k + " ", Crypto.ToString(), keys, false, Fast);
                        }
                        sw.Write((byte)(c ^ k));
                    }
                }
                else
                {
                    var num = 56;
                    status = 2;
                    for (var i = 0; i < file.Length; i++)//generate Key; Length = SourceFile.Length
                    {
                        byte addk1 = 0, addk2 = 0, addk3 = 0, addres = 0;
                        for (var j = 0; j < 8; j++)
                        {
                            var k1 = (byte) ((Key[0] & (1L << (LfsrSize[0] - 1))) >> (LfsrSize[0] - 1));
                            var k2 = (byte) ((Key[1] & (1L << (LfsrSize[1] - 1))) >> (LfsrSize[1] - 1));
                            var k3 = (byte) ((Key[2] & (1L << (LfsrSize[2] - 1))) >> (LfsrSize[2] - 1));
                            GetNextKeyState(ref Key[0], _polynoms[LfsrSize[0] - Dif]);
                            GetNextKeyState(ref Key[1], _polynoms[LfsrSize[1] - Dif]);
                            GetNextKeyState(ref Key[2], _polynoms[LfsrSize[2] - Dif]);
                            var k4 = (byte)(k1==0?1:0);
                            var res = (byte)((k1 & k2) | (k4 & k3));
                            addk1 |= (byte) (k1 << 7 - j);
                            addk2 |= (byte) (k2 << 7 - j);
                            addk3 |= (byte) (k3 << 7 - j);
                            addres |= (byte) (res << 7 - j);
                        }
                        ((BackgroundWorker) sender).ReportProgress(i);
                        if (num >= 0)
                        {
                            keys[0] |= ((long)addk1 << num);
                            keys[1] |= ((long)addk2 << num);
                            keys[2] |= ((long)addk3 << num);
                            num -= 8;
                        }
                        _generedKey[i] = addres;
                        var c = sr.ReadByte();
                        if (Fast)
                        {
                            SoureceString += (char)c;
                            CryptoString += (char)(c ^ addres);
                            KeyString += addres + " ";
                        }
                        else
                        {
                            Source = (char) c;
                            Crypto = (char) (c ^ addres);
                            if (WokrComplete != null)
                                WokrComplete(status, Source.ToString(), addres + " ", Crypto.ToString(), keys, false, Fast);
                        }
                        sw.Write((byte)(c ^ addres));
                    }
                }
            }
            else
            {
                var s = new byte[256];
                for (var i = 0; i < 256; i++)
                    s[i] = (byte)i;
                byte[] key;
                if (ReadKey(out key) != 0)
                {
                    MessageBox.Show("Error in Key File!", "Error!", MessageBoxButton.OK, MessageBoxImage.Error,
                        MessageBoxResult.OK, MessageBoxOptions.None);
                    return;
                }
                var j = 0;
                for (var i = 0; i < 256; i++)
                {
                    j = (j + s[i] + key[i % key.Count()]) % 256;
                    Swap(ref s[i], ref s[j]);
                }
                status = 3;
                var k = 0;
                j = 0;
                for (var i = 0; i < file.Length; i++)
                {
                    var c = sr.ReadByte();
                    ((BackgroundWorker) sender).ReportProgress(i);
                    k = (k + 1)%256;
                    j = (j + s[k])%256;
                    Swap(ref s[k],ref s[j]);
                    var bit = s[(long)(s[k] + s[j])%256];
                    if (Fast)
                    {
                        SoureceString += (char)c;
                        CryptoString += (char)(c ^ bit);
                        KeyString += bit + " ";
                    }
                    else
                    {
                        Source = (char) c;
                        Crypto = (char) (c ^ bit);
                        if (WokrComplete != null)
                            WokrComplete(status, Source.ToString(), bit + " ", Crypto.ToString(), keys, false, Fast);
                    }
                    sw.Write((byte)(Fast?CryptoString[i]:Crypto));
                }
            }
            ((BackgroundWorker) sender).ReportProgress((int)file.Length);
            sr.Close();
            sw.Close();
            file.Close();
            if (Fast)
            {
                if (WokrComplete != null)
                    WokrComplete(status, SoureceString, KeyString, CryptoString, keys, true, Fast);
            }
            else
            {
                if (WokrComplete != null)
                    WokrComplete(status, Source.ToString(), string.Empty, Crypto.ToString(), keys, true, Fast);
            }
        }

        private static void Swap(ref byte s1, ref byte s2)
        {
            var s = s1;
            s1 = s2;
            s2 = s;
        }

        public static string ToText(string text, bool key)
        {
            var res = string.Empty;

            for (var i = 0; i < text.Length/8; i++)
            {
                var b = 0;
                for (var j = 0; j < 8; j++)
                    b |= ((text[i*8 + j] - '0') << (7 - j));
                res += (key ? b.ToString() + ' ' : ((char) b).ToString());
            }

            return res;
        }

        public static string ToBinary(string text, bool key)
        {
            if (!key)
            {
                return text.Aggregate(string.Empty,
                    (current, c) =>
                        current +
                        (((c & 128) >> 7).ToString() + ((c & 64) >> 6) + ((c & 32) >> 5) + ((c & 16) >> 4) +
                         ((c & 8) >> 3) +
                         ((c & 4) >> 2) + ((c & 2) >> 1) + (c & 1) + ' '));
            }
            if (text.Trim() != string.Empty && text.Where(char.IsLetterOrDigit).Aggregate("",(current, c)=>current + c)!=string.Empty)
                return
                    text.Trim().Split(' ')
                        .Select(int.Parse)
                        .Aggregate(string.Empty,
                            (current, c) =>
                                current +
                                (((c & 128) >> 7).ToString() + ((c & 64) >> 6) + ((c & 32) >> 5) + ((c & 16) >> 4) +
                                 ((c & 8) >> 3) + ((c & 4) >> 2) + ((c & 2) >> 1) + (c & 1) + ' '));
            return string.Empty;
        }

        private static int ReadKey(out byte[] key)
        {
            var length = 5;
            key = new byte[5];
            var br = new BinaryReader(new FileStream(KeyPath, FileMode.Open, FileAccess.Read, FileShare.None));
            var s = string.Empty;
            var i = 0;
            while (br.PeekChar()>0)
            {
                if (i == length)
                {
                    length *= 2;
                    Array.Resize(ref key,length);
                }
                var c = br.ReadChar();
                if (c != ' ') s += c;
                else
                {
                    if (!byte.TryParse(s, out key[i++])) return 1;
                    s = string.Empty;
                }
                if (br.PeekChar() < 0)
                {
                    if (!byte.TryParse(s, out key[i++])) return 1;
                    s = string.Empty;
                }

            }
            Array.Resize(ref key, i);  
            br.Close();
            return 0;
        }
    }
}