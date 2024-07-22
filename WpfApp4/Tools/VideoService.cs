using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfApp4.Tools
{
    public class VideoService
    {
        private string _outputFolder;

        public VideoService(string windowName)
        {
            _outputFolder = Path.Combine(@"C:\output", windowName);
            Directory.CreateDirectory(_outputFolder);
        }

        public void SaveFrames(List<BitmapSource> frames)
        {
            for (int i = 0; i < frames.Count; i++)
            {
                string filePath = Path.Combine(_outputFolder, $"frame_{i:D4}.png");
                SaveBitmapSource(frames[i], filePath);
            }
        }

        public void CreateVideo(string windowName)
        {
            string ffmpegPath = "ffmpeg";
            string outputVideo = Path.Combine(_outputFolder, $"{windowName}.mp4");
            string inputPattern = Path.Combine(_outputFolder, "frame_%04d.png");
            string arguments = $"-r 30 -f image2 -i {inputPattern} -vcodec libx264 -crf 18 -pix_fmt yuv420p {outputVideo}";

            var startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);
            process.WaitForExit();

            // Delete the frames after the video is created
            DeleteFrames();
        }

        private void SaveBitmapSource(BitmapSource bitmapSource, string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(fileStream);
            }
        }

        private void DeleteFrames()
        {
            var files = Directory.GetFiles(_outputFolder, "frame_*.png");
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }
}
