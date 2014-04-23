using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace shredder
{
    public class Shreder
    {
        [Test]
        public void ShredFile()
        {
            // Arrange
            const string filename = @"---- Put filename here ----";

            // Act
            Shred(filename);

            // Assert
        }

        private static void Shred(string path)
        {
            using (var file = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
            {
                const int bufferSize = 64*1024;
                var length = file.Length;
                var random = new Random();
                var buffer = new byte[bufferSize];

                for (var i = 0; i < 1000; i++)
                {
                    file.Seek(0, SeekOrigin.Begin);
                    for (var p = 0; p < length; p += bufferSize)
                    {
                        var chunk = (int)(length - p);
                        if (chunk > bufferSize) chunk = bufferSize;

                        random.NextBytes(buffer);

                        file.Write(buffer, 0, chunk);
                        file.Flush(true);
                    }
                }
            }
        }

        private void RandomLetters(byte[] buffer, Random random)
        {
            var source = Encoding.ASCII.GetBytes("ABCDEFGHIJKLMNOP");
            random.NextBytes(buffer);

            for (var i = 0; i < 16; i++)
            {
                var msb = buffer[i+16] % 16;
                var lsb = buffer[i+16] / 16;

                buffer[i * 2] = source[msb];
                buffer[(i * 2) + 1] = source[lsb];
            }

            buffer[30] = Encoding.ASCII.GetBytes("\r")[0];
            buffer[31] = Encoding.ASCII.GetBytes("\n")[0];
        }
    }
}
