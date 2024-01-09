using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubtitleTimeshift
{
    public class Shifter
    {
        async static public Task Shift(Stream input, Stream output, TimeSpan timeSpan, Encoding encoding, int bufferSize = 1024, bool leaveOpen = false)
        {
            using (StreamReader sr = new StreamReader(input, encoding, true, bufferSize, leaveOpen)) 
            {
                using (StreamWriter writer = new StreamWriter (output, encoding, bufferSize, leaveOpen))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync()) != null)
                    {
                        string adjustedLine = AdjustSubtitleLine(line, timeSpan);
                        await writer.WriteLineAsync(adjustedLine);
                    }
                }
            }
        }


        static string AdjustSubtitleLine(string line, TimeSpan timeSpan)
        {
            // Padrão para identificar linhas de tempo no formato SRT
            Regex timePattern = new Regex(@"(\d+:\d+:\d+,\d+) --> (\d+:\d+:\d+,\d+)");

            Match match = timePattern.Match(line);
            if (match.Success)
            {
                string startTimeStr = match.Groups[1].Value;
                string endTimeStr = match.Groups[2].Value;

                // Substitui vírgulas por pontos nas representações de tempo
                startTimeStr = startTimeStr.Replace(',', '.');
                endTimeStr = endTimeStr.Replace(',', '.');

                TimeSpan startTime = TimeSpan.Parse(startTimeStr);
                TimeSpan endTime = TimeSpan.Parse(endTimeStr);

                startTime = startTime.Add(timeSpan);
                endTime = endTime.Add(timeSpan);

                // Formata as novas representações de tempo com pontos
                string adjustedLine = $"{startTime.ToString(@"hh\:mm\:ss\.fff")} --> {endTime.ToString(@"hh\:mm\:ss\.fff")}";
                return adjustedLine;
            }

            // Se a linha não for uma linha de tempo, retorna a linha inalterada
            return line;
        }        
    }
}
