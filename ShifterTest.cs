using System;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace SubtitleTimeshift.Tests
{
    public class ShifterTest
    {
        [Fact]
        async public Task TestShiftSubtitle()
        {
            var inputPath = "C:\\Users\\pimpi\\Downloads\\devassessment-ANDRITZ\\devassessment-master\\SubtitleTimeshift.Tests\\The.Matrix.1999.BluRay.720p.Malay.srt";
            var outputPath = "C:\\Users\\pimpi\\Downloads\\devassessment-ANDRITZ\\devassessment-master\\SubtitleTimeshift.Tests\\The.Matrix.1999.BluRay.720p.Malay - Copy.srt";
            var assertPath = "C:\\Users\\pimpi\\Downloads\\devassessment-ANDRITZ\\devassessment-master\\SubtitleTimeshift.Tests\\The.Matrix.1999.BluRay.720p.Malay - Assert.srt";


            // Limpa o arquivo de saída antes do teste
            if (File.Exists(outputPath))
            {
                File.Delete(outputPath);
            }


            var timeSpan = TimeSpan.FromMilliseconds(123);
            var encoding = System.Text.Encoding.UTF8;

            using (var inputStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read))
            using (var outputStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
            {
                await Shifter.Shift(inputStream, outputStream, timeSpan, encoding);
            }

            using (var outputStream = new FileStream(outputPath, FileMode.Open, FileAccess.Read))
            using (var outputReader = new StreamReader(outputPath, encoding, true))
            using (var assertStream = new FileStream(assertPath, FileMode.Open, FileAccess.Read))
            using (var assertReader = new StreamReader(assertStream, encoding, true))
            {
                var assertLine = default(string);
                var outputLine = default(string);

                while (null != (assertLine = await assertReader.ReadLineAsync()))
                {
                    Assert.False(outputReader.EndOfStream);
                    outputLine = await outputReader.ReadLineAsync();
                    Assert.Equal(assertLine, outputLine);
                }
            }
        }
    }
}
