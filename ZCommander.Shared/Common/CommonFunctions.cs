using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace ZCommander.Shared.Common
{
    public static class CommonFunctions
    {
        public static T Clone<T>(T source)
        {
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, source);
                stream.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(stream);
            }
        }

        public static byte[] ObjectToBinary(Object obj, Boolean compress)
        {
            MemoryStream MemStream = null;
            DeflateStream DStream = null;
            MemoryStream OutStream = null;

            BinaryFormatter BinFormatter = default(BinaryFormatter);
            byte[] B = null;
            try
            {
                MemStream = new MemoryStream();
                BinFormatter = new BinaryFormatter();
                BinFormatter.Serialize(MemStream, obj);

                if (compress)
                {
                    B = MemStream.ToArray();

                    OutStream = new MemoryStream();
                    DStream = new DeflateStream(OutStream, CompressionMode.Compress, false);
                    DStream.Write(MemStream.ToArray(), 0, B.Length);
                    DStream.Flush();

                    B = OutStream.ToArray();
                }
                else
                {
                    B = MemStream.ToArray();
                }

                return B;
            }
            finally
            {
                if ((OutStream != null))
                    OutStream.Dispose();
                if ((DStream != null))
                    DStream.Dispose();
                if ((MemStream != null))
                    MemStream.Dispose();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public static bool BinaryToObject(byte[] Data, ref object obj, bool IsCompressed)
        {
            obj = null;
            MemoryStream MemStream = null;
            MemoryStream InputStream = null;
            DeflateStream DStream = null;
            BinaryFormatter BinFormatter = default(BinaryFormatter);
            byte[] B = null;

            try
            {
                MemStream = new MemoryStream(Data);
                BinFormatter = new BinaryFormatter();
                BinFormatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;

                if (IsCompressed)
                {
                    DStream = new DeflateStream(MemStream, CompressionMode.Decompress, false);
                    B = ReadFullStream(DStream);
                    DStream.Flush();

                    InputStream = new MemoryStream(B);
                    obj = BinFormatter.Deserialize(InputStream);

                }
                else
                {
                    obj = BinFormatter.Deserialize(MemStream);
                }
                return true;
            }
            finally
            {
                if ((DStream != null))
                    DStream.Dispose();
                if ((InputStream != null))
                    InputStream.Dispose();
                if ((MemStream != null))
                    MemStream.Dispose();
            }
        }
        
        public static byte[] ReadFullStream(Stream stream)
        {
            byte[] buffer = new byte[4096];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read(buffer, 0, buffer.Length);
                    if (read <= 0)
                    {
                        return ms.ToArray();
                    }
                    ms.Write(buffer, 0, read);
                }
            }
        }
        
        public static byte[] Getbytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        
        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static string ScrubMacroIdentifiers(string key)
        {
            var newKey = key;
            newKey = key.Replace("#!", "").Replace("!#", "");
            newKey = newKey.Replace("{!", "").Replace("!}", "");
            newKey = newKey.Replace("^!", "").Replace("!^", "");
            newKey = newKey.Replace("@!", "").Replace("!@", "");
            return newKey;
        }
    }
}
