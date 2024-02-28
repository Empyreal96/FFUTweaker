using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FFUTweak
{
    /// <summary>
    /// Interaction logic for img2ffu.xaml
    /// </summary>
    public partial class img2ffu : Window
    {
        static img2ffu img2Ffu;
        public static string FieldText;

        
        string dumpPath;
        string outputFFU;
        bool inProgress = false;
        public img2ffu()
        {
            img2Ffu = this;
            InitializeComponent();
        }

        private void LoadDump_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "eMMC Image Dump (*.img)|*.img";
            if (openFile.ShowDialog() == true)
            {
                // ImageOutputText.Text += $"{openFile.FileName}\n";

                StartProcess.IsEnabled = true;
                Log(openFile.FileName, LoggingLevel.Information, true);
                dumpPath = openFile.FileName;
                FFUGenInfo.Visibility = Visibility.Visible;
            }
        }

        private void StartProcess_Click(object sender, RoutedEventArgs e)
        {
            if (PlatIDText.Text == "Enter Platform ID" || PlatIDText.Text == "")
            {
                MessageBox.Show("You must enter the Platform ID, You can find this in WPInternals when you plug in a device");
                return;
            }
            else if (OSVersionText.Text == "Enter OS Version" || OSVersionText.Text == "")
            {
                MessageBox.Show("You must enter the OS version of the image");
                return;
            }
            else
            {
                inProgress = true;
                GenFFU(dumpPath, outputFFU, PlatIDText.Text, OSVersionText.Text);

            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (inProgress)
            {
                MessageBox.Show("Please wait until process is finished.");
                e.Cancel = true;
            }
        }

        private void SaveLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Full Flash Update (*.ffu)|*.ffu";
            if (saveFile.ShowDialog() == true)
            {

                outputFFU = saveFile.FileName;
            }
        }



        public static class TextBlockHelper
        {
            public static void SetText(TextBlock textBlock, string text)
            {
                if (textBlock.Dispatcher.CheckAccess())
                {
                    textBlock.Text = text;
                } else
                {
                    textBlock.Dispatcher.Invoke(() => textBlock.Text = text);
                }
            }
        }

        //static string log;
        public enum LoggingLevel
        {
            Information,
            Warning,
            Error
        }

        private static readonly object lockObj = new object();
        TextBlock textBlock1 = new TextBlock();
        public static void Log(string message, LoggingLevel severity = LoggingLevel.Information, bool returnline = true)
        {
            
            lock (lockObj)
            {
                if (message == "")
                {
                    Console.WriteLine();
                    return;
                }
                string msg = "";
                switch (severity)
                {
                    case LoggingLevel.Warning:
                        msg = "  Warning  ";
                        //Console.ForegroundColor = ConsoleColor.Yellow;
                        break;
                    case LoggingLevel.Error:
                        msg = "   Error   ";
                        //Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    case LoggingLevel.Information:
                        msg = "Information";
                        // Console.ForegroundColor = ConsoleColor.White;
                        break;
                }
                if (returnline)
                {

                    img2Ffu.ImageOutputText.Text += DateTime.Now.ToString("'['HH':'mm':'ss']'") + "[" + msg + "] " + message + "\n";
               
                }
                else
                {
                    img2Ffu.ImageOutputText.Text += "\r" + DateTime.Now.ToString("'['HH':'mm':'ss']'") + "[" + msg + "] " + message + "\n";
                }
                //Console.ForegroundColor = ConsoleColor.White;
            }
        }


        public static void GenFFU(string ImgFile, string FfuFile, string PlatId, string Osversion)
        {
            //Parser.Default.ParseArguments<Options>(args).WithParsed(delegate(Options o)
            //{
            Log("img2ffu - Converts raw image (img) files into full flash update (FFU) files");
            Log("Copyright (c) 2019, Gustave Monce - gus33000.me - @gus33000");
            Log("Copyright (c) 2018, Rene Lergner - wpinternals.net - @Heathcliff74xda");
            Log("Released under the MIT license at github.com/gus33000/img2ffu");
            Log("This is a modified version by WojTasXDA for Windows Phone image generation");
            Log("");
            try
            {
                GenerateFFU(ImgFile, FfuFile, PlatId, 131072u, "1.1", Osversion);
            }
            catch (Exception ex)
            {
                Log("Something happened.", LoggingLevel.Error);
                Log(ex.Message, LoggingLevel.Error);
                Log(ex.StackTrace, LoggingLevel.Error);
                Environment.Exit(1);
            }
            //});
        }

        private static void GenerateFFU(string ImageFile, string FFUFile, string PlatformId, uint chunkSize, string AntiTheftVersion, string Osversion)
        {
            Log("Input image: " + ImageFile);
            Log("Destination image: " + FFUFile);
            Log("Platform ID: " + PlatformId);
            Log("");
            Stream stream = ((!ImageFile.ToLower().Contains("\\\\.\\physicaldrive")) ? ((Stream)new FileStream(ImageFile, FileMode.Open)) : ((Stream)new DeviceStream(ImageFile, FileAccess.Read)));
            (FlashPart[], ulong, List<GPT.Partition> partitions) imageSlices = ImageSplitter.GetImageSlices(stream, chunkSize);
            FlashPart[] flashParts = imageSlices.Item1;
            ulong PlatEnd = imageSlices.Item2;
            List<GPT.Partition> partitions = imageSlices.partitions;
            IOrderedEnumerable<FlashingPayload> payloads = from x in FlashingPayloadGenerator.GetOptimizedPayloads(flashParts, chunkSize, PlatEnd)
                                                           orderby x.TargetLocations.First()
                                                           select x;
            Log("");
            Log("Building image headers...");
            string header1 = Path.GetTempFileName();
            FileStream Headerstream1 = new FileStream(header1, FileMode.OpenOrCreate);
            ImageHeader image = new ImageHeader();
            FullFlash fullFlash = new FullFlash();
            Store simage = new Store();
            fullFlash.OSVersion = Osversion;
            fullFlash.DevicePlatformId0 = PlatformId;
            fullFlash.AntiTheftVersion = AntiTheftVersion;
            simage.SectorSize = 512u;
            simage.MinSectorCount = (uint)(stream.Length / 512);
            Log("Generating image manifest...");
            string manifest = ManifestIni.BuildUpManifest(fullFlash, simage, partitions);
            byte[] TextBytes = Encoding.ASCII.GetBytes(manifest);
            image.ManifestLength = (uint)TextBytes.Length;
            byte[] ImageHeaderBuffer = new byte[24];
            ByteOperations.WriteUInt32(ImageHeaderBuffer, 0u, image.Size);
            ByteOperations.WriteAsciiString(ImageHeaderBuffer, 4u, image.Signature);
            ByteOperations.WriteUInt32(ImageHeaderBuffer, 16u, image.ManifestLength);
            ByteOperations.WriteUInt32(ImageHeaderBuffer, 20u, image.ChunkSize);
            Headerstream1.Write(ImageHeaderBuffer, 0, 24);
            Headerstream1.Write(TextBytes, 0, TextBytes.Length);
            RoundUpToChunks(Headerstream1, chunkSize);
            string header2 = Path.GetTempFileName();
            FileStream Headerstream2 = new FileStream(header2, FileMode.OpenOrCreate);
            StoreHeader store = new StoreHeader();
            store.WriteDescriptorCount = (uint)payloads.Count();
            store.FinalTableIndex = (uint)payloads.Count() - store.FinalTableCount;
            store.PlatformId = PlatformId;
            foreach (FlashingPayload payload5 in payloads)
            {
                store.WriteDescriptorLength += payload5.GetStoreHeaderSize();
            }
            using (IEnumerator<FlashingPayload> enumerator = payloads.GetEnumerator())
            {
                while (enumerator.MoveNext() && enumerator.Current.TargetLocations.First() <= PlatEnd)
                {
                    store.FlashOnlyTableIndex++;
                }
            }
            byte[] StoreHeaderBuffer = new byte[248];
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 0u, store.UpdateType);
            ByteOperations.WriteUInt16(StoreHeaderBuffer, 4u, store.MajorVersion);
            ByteOperations.WriteUInt16(StoreHeaderBuffer, 6u, store.MinorVersion);
            ByteOperations.WriteUInt16(StoreHeaderBuffer, 8u, store.FullFlashMajorVersion);
            ByteOperations.WriteUInt16(StoreHeaderBuffer, 10u, store.FullFlashMinorVersion);
            ByteOperations.WriteAsciiString(StoreHeaderBuffer, 12u, store.PlatformId);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 204u, store.BlockSizeInBytes);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 208u, store.WriteDescriptorCount);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 212u, store.WriteDescriptorLength);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 216u, store.ValidateDescriptorCount);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 220u, store.ValidateDescriptorLength);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 224u, store.InitialTableIndex);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 228u, store.InitialTableCount);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 232u, store.FlashOnlyTableIndex);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 236u, store.FlashOnlyTableCount);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 240u, store.FinalTableIndex);
            ByteOperations.WriteUInt32(StoreHeaderBuffer, 244u, store.FinalTableCount);
            Headerstream2.Write(StoreHeaderBuffer, 0, 248);
            byte[] descriptorsBuffer = new byte[store.WriteDescriptorLength];
            uint NewWriteDescriptorOffset = 0u;
            foreach (FlashingPayload payload4 in payloads)
            {
                ByteOperations.WriteUInt32(descriptorsBuffer, NewWriteDescriptorOffset, (uint)payload4.TargetLocations.Count());
                ByteOperations.WriteUInt32(descriptorsBuffer, NewWriteDescriptorOffset + 4, payload4.ChunkCount);
                NewWriteDescriptorOffset += 8;
                uint[] targetLocations = payload4.TargetLocations;
                foreach (uint location in targetLocations)
                {
                    ByteOperations.WriteUInt32(descriptorsBuffer, NewWriteDescriptorOffset, 0u);
                    ByteOperations.WriteUInt32(descriptorsBuffer, NewWriteDescriptorOffset + 4, location);
                    NewWriteDescriptorOffset += 8;
                }
            }
            Headerstream2.Write(descriptorsBuffer, 0, (int)store.WriteDescriptorLength);
            RoundUpToChunks(Headerstream2, chunkSize);
            SecurityHeader security = new SecurityHeader();
            Headerstream1.Seek(0L, SeekOrigin.Begin);
            Headerstream2.Seek(0L, SeekOrigin.Begin);
            security.HashTableSize = (uint)(32 * (int)((Headerstream1.Length + Headerstream2.Length) / (long)chunkSize));
            foreach (FlashingPayload payload3 in payloads)
            {
                security.HashTableSize += payload3.GetSecurityHeaderSize();
            }
            byte[] HashTable = new byte[security.HashTableSize];
            BinaryWriter bw = new BinaryWriter(new MemoryStream(HashTable));
            SHA256 crypto = SHA256.Create();
            for (int j = 0; j < Headerstream1.Length / (long)chunkSize; j++)
            {
                byte[] buffer = new byte[chunkSize];
                Headerstream1.Read(buffer, 0, (int)chunkSize);
                byte[] hash = crypto.ComputeHash(buffer);
                bw.Write(hash, 0, hash.Length);
            }
            for (int i = 0; i < Headerstream2.Length / (long)chunkSize; i++)
            {
                byte[] buffer2 = new byte[chunkSize];
                Headerstream2.Read(buffer2, 0, (int)chunkSize);
                byte[] hash2 = crypto.ComputeHash(buffer2);
                bw.Write(hash2, 0, hash2.Length);
            }
            foreach (FlashingPayload payload2 in payloads)
            {
                bw.Write(payload2.ChunkHashes[0], 0, payload2.ChunkHashes[0].Length);
            }
            bw.Close();
            Log("Generating image catalog...");
            byte[] catalog = GenerateCatalogFile(HashTable);
            security.CatalogSize = (uint)catalog.Length;
            byte[] SecurityHeaderBuffer = new byte[32];
            ByteOperations.WriteUInt32(SecurityHeaderBuffer, 0u, security.Size);
            ByteOperations.WriteAsciiString(SecurityHeaderBuffer, 4u, security.Signature);
            ByteOperations.WriteUInt32(SecurityHeaderBuffer, 16u, security.ChunkSizeInKb);
            ByteOperations.WriteUInt32(SecurityHeaderBuffer, 20u, security.HashAlgorithm);
            ByteOperations.WriteUInt32(SecurityHeaderBuffer, 24u, security.CatalogSize);
            ByteOperations.WriteUInt32(SecurityHeaderBuffer, 28u, security.HashTableSize);
            FileStream retstream = new FileStream(FFUFile, FileMode.CreateNew);
            retstream.Write(SecurityHeaderBuffer, 0, 32);
            retstream.Write(catalog, 0, (int)security.CatalogSize);
            retstream.Write(HashTable, 0, (int)security.HashTableSize);
            RoundUpToChunks(retstream, chunkSize);
            Headerstream1.Seek(0L, SeekOrigin.Begin);
            Headerstream2.Seek(0L, SeekOrigin.Begin);
            byte[] buff = new byte[Headerstream1.Length];
            Headerstream1.Read(buff, 0, (int)Headerstream1.Length);
            Headerstream1.Close();
            File.Delete(header1);
            retstream.Write(buff, 0, buff.Length);
            buff = new byte[Headerstream2.Length];
            Headerstream2.Read(buff, 0, (int)Headerstream2.Length);
            Headerstream2.Close();
            File.Delete(header2);
            retstream.Write(buff, 0, buff.Length);
            Log("Writing payloads...");
            ulong counter = 0uL;
            DateTime startTime = DateTime.Now;
            foreach (FlashingPayload payload in payloads)
            {
                uint StreamIndex = payload.StreamIndexes.First();
                Stream stream2 = flashParts[StreamIndex].Stream;
                stream2.Seek(payload.StreamLocations.First(), SeekOrigin.Begin);
                byte[] buffer3 = new byte[chunkSize];
                stream2.Read(buffer3, 0, (int)chunkSize);
                retstream.Write(buffer3, 0, (int)chunkSize);
                counter++;
                ShowProgress((ulong)(payloads.Count() * chunkSize), startTime, counter * chunkSize, counter * chunkSize, payload.TargetLocations.First() * chunkSize < PlatEnd);
            }
            retstream.Close();
            Log("");
        }

        private static void RoundUpToChunks(Stream stream, uint chunkSize)
        {
            long Size = stream.Length;
            if (Size % (long)chunkSize > 0)
            {
                long padding = (uint)((Size / (long)chunkSize + 1) * chunkSize) - Size;
                stream.Write(new byte[padding], 0, (int)padding);
            }
        }

        private static void ShowProgress(ulong totalBytes, DateTime startTime, ulong BytesRead, ulong SourcePosition, bool DisplayRed)
        {
            TimeSpan timeSoFar = DateTime.Now - startTime;
            TimeSpan remaining = TimeSpan.FromMilliseconds(timeSoFar.TotalMilliseconds / (double)BytesRead * (double)(totalBytes - BytesRead));
            double speed = Math.Round((double)(SourcePosition / 1024uL / 1024uL) / timeSoFar.TotalSeconds);
            Log(string.Format("{0} {1}MB/s {2:hh\\:mm\\:ss\\.f}", GetDismLikeProgBar(int.Parse((BytesRead * 100 / totalBytes).ToString())), speed.ToString(), remaining, remaining.TotalHours, remaining.Minutes, remaining.Seconds, remaining.Milliseconds), DisplayRed ? LoggingLevel.Warning : LoggingLevel.Information, returnline: false);
        }

        private static string GetDismLikeProgBar(int perc)
        {
            int eqsLength = (int)((double)perc / 100.0 * 55.0);
            string bases = new string('=', eqsLength) + new string(' ', 55 - eqsLength);
            bases = bases.Insert(28, perc + "%");
            if (perc == 100)
            {
                bases = bases.Substring(1);
            }
            else if (perc < 10)
            {
                bases = bases.Insert(28, " ");
            }
            return "[" + bases + "]";
        }

        private static byte[] GenerateCatalogFile(byte[] hashData)
        {
            string catalog = Path.GetTempFileName();
            string cdf = Path.GetTempFileName();
            string hashTableBlob = Path.GetTempFileName();
            File.WriteAllBytes(hashTableBlob, hashData);
            using (StreamWriter streamWriter = new StreamWriter(cdf))
            {
                streamWriter.WriteLine("[CatalogHeader]");
                streamWriter.WriteLine("Name={0}", catalog);
                streamWriter.WriteLine("[CatalogFiles]");
                streamWriter.WriteLine("{0}={1}", "HashTable.blob", hashTableBlob);
            }
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "MakeCat.exe";
                process.StartInfo.Arguments = $"\"{cdf}\"";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.Start();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    throw new Exception();
                }
            }
            byte[] result = File.ReadAllBytes(catalog);
            File.Delete(catalog);
            File.Delete(hashTableBlob);
            File.Delete(cdf);
            return result;
        }

        internal static class ByteOperations
        {
            private static uint[] CRC32Table = new uint[256]
            {
            0u, 1996959894u, 3993919788u, 2567524794u, 124634137u, 1886057615u, 3915621685u, 2657392035u, 249268274u, 2044508324u,
            3772115230u, 2547177864u, 162941995u, 2125561021u, 3887607047u, 2428444049u, 498536548u, 1789927666u, 4089016648u, 2227061214u,
            450548861u, 1843258603u, 4107580753u, 2211677639u, 325883990u, 1684777152u, 4251122042u, 2321926636u, 335633487u, 1661365465u,
            4195302755u, 2366115317u, 997073096u, 1281953886u, 3579855332u, 2724688242u, 1006888145u, 1258607687u, 3524101629u, 2768942443u,
            901097722u, 1119000684u, 3686517206u, 2898065728u, 853044451u, 1172266101u, 3705015759u, 2882616665u, 651767980u, 1373503546u,
            3369554304u, 3218104598u, 565507253u, 1454621731u, 3485111705u, 3099436303u, 671266974u, 1594198024u, 3322730930u, 2970347812u,
            795835527u, 1483230225u, 3244367275u, 3060149565u, 1994146192u, 31158534u, 2563907772u, 4023717930u, 1907459465u, 112637215u,
            2680153253u, 3904427059u, 2013776290u, 251722036u, 2517215374u, 3775830040u, 2137656763u, 141376813u, 2439277719u, 3865271297u,
            1802195444u, 476864866u, 2238001368u, 4066508878u, 1812370925u, 453092731u, 2181625025u, 4111451223u, 1706088902u, 314042704u,
            2344532202u, 4240017532u, 1658658271u, 366619977u, 2362670323u, 4224994405u, 1303535960u, 984961486u, 2747007092u, 3569037538u,
            1256170817u, 1037604311u, 2765210733u, 3554079995u, 1131014506u, 879679996u, 2909243462u, 3663771856u, 1141124467u, 855842277u,
            2852801631u, 3708648649u, 1342533948u, 654459306u, 3188396048u, 3373015174u, 1466479909u, 544179635u, 3110523913u, 3462522015u,
            1591671054u, 702138776u, 2966460450u, 3352799412u, 1504918807u, 783551873u, 3082640443u, 3233442989u, 3988292384u, 2596254646u,
            62317068u, 1957810842u, 3939845945u, 2647816111u, 81470997u, 1943803523u, 3814918930u, 2489596804u, 225274430u, 2053790376u,
            3826175755u, 2466906013u, 167816743u, 2097651377u, 4027552580u, 2265490386u, 503444072u, 1762050814u, 4150417245u, 2154129355u,
            426522225u, 1852507879u, 4275313526u, 2312317920u, 282753626u, 1742555852u, 4189708143u, 2394877945u, 397917763u, 1622183637u,
            3604390888u, 2714866558u, 953729732u, 1340076626u, 3518719985u, 2797360999u, 1068828381u, 1219638859u, 3624741850u, 2936675148u,
            906185462u, 1090812512u, 3747672003u, 2825379669u, 829329135u, 1181335161u, 3412177804u, 3160834842u, 628085408u, 1382605366u,
            3423369109u, 3138078467u, 570562233u, 1426400815u, 3317316542u, 2998733608u, 733239954u, 1555261956u, 3268935591u, 3050360625u,
            752459403u, 1541320221u, 2607071920u, 3965973030u, 1969922972u, 40735498u, 2617837225u, 3943577151u, 1913087877u, 83908371u,
            2512341634u, 3803740692u, 2075208622u, 213261112u, 2463272603u, 3855990285u, 2094854071u, 198958881u, 2262029012u, 4057260610u,
            1759359992u, 534414190u, 2176718541u, 4139329115u, 1873836001u, 414664567u, 2282248934u, 4279200368u, 1711684554u, 285281116u,
            2405801727u, 4167216745u, 1634467795u, 376229701u, 2685067896u, 3608007406u, 1308918612u, 956543938u, 2808555105u, 3495958263u,
            1231636301u, 1047427035u, 2932959818u, 3654703836u, 1088359270u, 936918000u, 2847714899u, 3736837829u, 1202900863u, 817233897u,
            3183342108u, 3401237130u, 1404277552u, 615818150u, 3134207493u, 3453421203u, 1423857449u, 601450431u, 3009837614u, 3294710456u,
            1567103746u, 711928724u, 3020668471u, 3272380065u, 1510334235u, 755167117u
            };

            internal static string ReadAsciiString(byte[] ByteArray, uint Offset, uint Length)
            {
                byte[] Bytes = new byte[Length];
                Buffer.BlockCopy(ByteArray, (int)Offset, Bytes, 0, (int)Length);
                return Encoding.ASCII.GetString(Bytes);
            }

            internal static string ReadUnicodeString(byte[] ByteArray, uint Offset, uint Length)
            {
                byte[] Bytes = new byte[Length];
                Buffer.BlockCopy(ByteArray, (int)Offset, Bytes, 0, (int)Length);
                return Encoding.Unicode.GetString(Bytes);
            }

            internal static void WriteAsciiString(byte[] ByteArray, uint Offset, string Text, uint? MaxBufferLength = null)
            {
                if (MaxBufferLength.HasValue)
                {
                    Array.Clear(ByteArray, (int)Offset, (int)MaxBufferLength.Value);
                }
                byte[] bytes = Encoding.ASCII.GetBytes(Text);
                int WriteLength = bytes.Length;
                if (WriteLength > MaxBufferLength)
                {
                    WriteLength = (int)MaxBufferLength.Value;
                }
                Buffer.BlockCopy(bytes, 0, ByteArray, (int)Offset, WriteLength);
            }

            internal static void WriteUnicodeString(byte[] ByteArray, uint Offset, string Text, uint? MaxBufferLength = null)
            {
                if (MaxBufferLength.HasValue)
                {
                    Array.Clear(ByteArray, (int)Offset, (int)MaxBufferLength.Value);
                }
                byte[] bytes = Encoding.Unicode.GetBytes(Text);
                int WriteLength = bytes.Length;
                if (WriteLength > MaxBufferLength)
                {
                    WriteLength = (int)MaxBufferLength.Value;
                }
                Buffer.BlockCopy(bytes, 0, ByteArray, (int)Offset, WriteLength);
            }

            internal static uint ReadUInt32(byte[] ByteArray, uint Offset)
            {
                return BitConverter.ToUInt32(ByteArray, (int)Offset);
            }

            internal static void WriteUInt32(byte[] ByteArray, uint Offset, uint Value)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(Value), 0, ByteArray, (int)Offset, 4);
            }

            internal static int ReadInt32(byte[] ByteArray, uint Offset)
            {
                return BitConverter.ToInt32(ByteArray, (int)Offset);
            }

            internal static void WriteInt32(byte[] ByteArray, uint Offset, int Value)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(Value), 0, ByteArray, (int)Offset, 4);
            }

            internal static ushort ReadUInt16(byte[] ByteArray, uint Offset)
            {
                return BitConverter.ToUInt16(ByteArray, (int)Offset);
            }

            internal static void WriteUInt16(byte[] ByteArray, uint Offset, ushort Value)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(Value), 0, ByteArray, (int)Offset, 2);
            }

            internal static short ReadInt16(byte[] ByteArray, uint Offset)
            {
                return BitConverter.ToInt16(ByteArray, (int)Offset);
            }

            internal static void WriteInt16(byte[] ByteArray, uint Offset, short Value)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(Value), 0, ByteArray, (int)Offset, 2);
            }

            internal static byte ReadUInt8(byte[] ByteArray, uint Offset)
            {
                return ByteArray[Offset];
            }

            internal static void WriteUInt8(byte[] ByteArray, uint Offset, byte Value)
            {
                ByteArray[Offset] = Value;
            }

            internal static uint ReadUInt24(byte[] ByteArray, uint Offset)
            {
                return (uint)(ByteArray[Offset] + (ByteArray[Offset + 1] << 8) + (ByteArray[Offset + 2] << 16));
            }

            internal static void WriteUInt24(byte[] ByteArray, uint Offset, uint Value)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(Value), 0, ByteArray, (int)Offset, 3);
            }

            internal static ulong ReadUInt64(byte[] ByteArray, uint Offset)
            {
                return BitConverter.ToUInt64(ByteArray, (int)Offset);
            }

            internal static void WriteUInt64(byte[] ByteArray, uint Offset, ulong Value)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(Value), 0, ByteArray, (int)Offset, 8);
            }

            internal static Guid ReadGuid(byte[] ByteArray, uint Offset)
            {
                byte[] GuidBuffer = new byte[16];
                Buffer.BlockCopy(ByteArray, (int)Offset, GuidBuffer, 0, 16);
                return new Guid(GuidBuffer);
            }

            internal static void WriteGuid(byte[] ByteArray, uint Offset, Guid Value)
            {
                Buffer.BlockCopy(Value.ToByteArray(), 0, ByteArray, (int)Offset, 16);
            }

            internal static uint Align(uint Base, uint Offset, uint Alignment)
            {
                if ((Offset - Base) % Alignment == 0)
                {
                    return Offset;
                }
                return ((Offset - Base) / Alignment + 1) * Alignment + Base;
            }

            internal static uint? FindPatternInFile(string FileName, byte[] Pattern, byte[] Mask, out byte[] OutPattern)
            {
                uint? Result = null;
                FileStream Stream = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                byte[] Buffer = new byte[65536 + Pattern.Length - 1];
                uint BufferReadPosition = 0u;
                uint BytesInBuffer = 0u;
                uint SearchPositionFile = 0u;
                uint SearchPositionBuffer = 0u;
                uint BufferFileOffset = 0u;
                bool Match = false;
                OutPattern = null;
                for (; SearchPositionFile <= Stream.Length - Pattern.Length; SearchPositionFile++)
                {
                    if (SearchPositionBuffer + Pattern.Length > BytesInBuffer)
                    {
                        if (BytesInBuffer - SearchPositionBuffer != 0)
                        {
                            System.Buffer.BlockCopy(Buffer, (int)SearchPositionBuffer, Buffer, 0, (int)(BytesInBuffer - SearchPositionBuffer));
                        }
                        BufferReadPosition = BytesInBuffer - SearchPositionBuffer;
                        BytesInBuffer -= SearchPositionBuffer;
                        BufferFileOffset += SearchPositionBuffer;
                        SearchPositionBuffer = 0u;
                        uint BytesRead = (uint)Stream.Read(Buffer, (int)BufferReadPosition, Buffer.Length - (int)BufferReadPosition);
                        BytesInBuffer += BytesRead;
                    }
                    Match = true;
                    for (int i = 0; i < Pattern.Length; i++)
                    {
                        if (Buffer[SearchPositionBuffer + i] != Pattern[i] && (Mask == null || Mask[i] == 0))
                        {
                            Match = false;
                            break;
                        }
                    }
                    if (Match)
                    {
                        Result = SearchPositionFile;
                        OutPattern = new byte[Pattern.Length];
                        System.Buffer.BlockCopy(Buffer, (int)SearchPositionBuffer, OutPattern, 0, Pattern.Length);
                        break;
                    }
                    SearchPositionBuffer++;
                }
                Stream.Close();
                return Result;
            }

            internal static uint? FindAscii(byte[] SourceBuffer, string Pattern)
            {
                return FindPattern(SourceBuffer, Encoding.ASCII.GetBytes(Pattern), null, null);
            }

            internal static uint? FindUnicode(byte[] SourceBuffer, string Pattern)
            {
                return FindPattern(SourceBuffer, Encoding.Unicode.GetBytes(Pattern), null, null);
            }

            internal static uint? FindUint(byte[] SourceBuffer, uint Pattern)
            {
                return FindPattern(SourceBuffer, BitConverter.GetBytes(Pattern), null, null);
            }

            internal static uint? FindPattern(byte[] SourceBuffer, byte[] Pattern, byte[] Mask, byte[] OutPattern)
            {
                return FindPattern(SourceBuffer, 0u, null, Pattern, Mask, OutPattern);
            }

            internal static bool Compare(byte[] Array1, byte[] Array2)
            {
                return StructuralComparisons.StructuralEqualityComparer.Equals(Array1, Array2);
            }

            internal static uint? FindPattern(byte[] SourceBuffer, uint SourceOffset, uint? SourceSize, byte[] Pattern, byte[] Mask, byte[] OutPattern)
            {
                uint? Result = null;
                uint SearchPosition = SourceOffset;
                bool Match = false;
                for (; SearchPosition <= SourceBuffer.Length - Pattern.Length && (!SourceSize.HasValue || SearchPosition <= SourceOffset + SourceSize - Pattern.Length); SearchPosition++)
                {
                    Match = true;
                    for (int i = 0; i < Pattern.Length; i++)
                    {
                        if (SourceBuffer[SearchPosition + i] != Pattern[i] && (Mask == null || Mask[i] == 0))
                        {
                            Match = false;
                            break;
                        }
                    }
                    if (Match)
                    {
                        Result = SearchPosition;
                        if (OutPattern != null)
                        {
                            Buffer.BlockCopy(SourceBuffer, (int)SearchPosition, OutPattern, 0, Pattern.Length);
                        }
                        break;
                    }
                }
                return Result;
            }

            internal static byte CalculateChecksum8(byte[] Buffer, uint Offset, uint Size)
            {
                byte Checksum = 0;
                for (uint i = Offset; i < Offset + Size; i++)
                {
                    Checksum = (byte)(Checksum + Buffer[i]);
                }
                return (byte)(256 - Checksum);
            }

            internal static ushort CalculateChecksum16(byte[] Buffer, uint Offset, uint Size)
            {
                ushort Checksum = 0;
                for (uint i = Offset; i < Offset + Size - 1; i += 2)
                {
                    Checksum = (ushort)(Checksum + BitConverter.ToUInt16(Buffer, (int)i));
                }
                return (ushort)(65536 - Checksum);
            }

            internal static uint CRC32(byte[] Input, uint Offset, uint Length)
            {
                if (Input == null || Offset + Length > Input.Length)
                {
                    throw new ArgumentException();
                }
                uint crc = uint.MaxValue;
                for (uint i = Offset; i < Offset + Length; i++)
                {
                    crc = (crc >> 8) ^ CRC32Table[(crc ^ Input[i]) & 0xFF];
                }
                crc = (uint)(crc ^ -1);
                if (crc < 0)
                {
                    crc = crc;
                }
                return crc;
            }
        }

        public class DeviceStream : Stream
        {
            private enum MEDIA_TYPE
            {
                Unknown,
                F5_1Pt2_512,
                F3_1Pt44_512,
                F3_2Pt88_512,
                F3_20Pt8_512,
                F3_720_512,
                F5_360_512,
                F5_320_512,
                F5_320_1024,
                F5_180_512,
                F5_160_512,
                RemovableMedia,
                FixedMedia,
                F3_120M_512,
                F3_640_512,
                F5_640_512,
                F5_720_512,
                F3_1Pt2_512,
                F3_1Pt23_1024,
                F5_1Pt23_1024,
                F3_128Mb_512,
                F3_230Mb_512,
                F8_256_128,
                F3_200Mb_512,
                F3_240M_512,
                F3_32M_512
            }

            private struct DISK_GEOMETRY
            {
                internal long Cylinders;

                internal MEDIA_TYPE MediaType;

                internal uint TracksPerCylinder;

                internal uint SectorsPerTrack;

                internal uint BytesPerSector;
            }

            private struct DISK_GEOMETRY_EX
            {
                internal DISK_GEOMETRY Geometry;

                internal long DiskSize;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
                internal byte[] Data;
            }

            private const uint GENERIC_READ = 2147483648u;

            private const uint GENERIC_WRITE = 1073741824u;

            private const uint OPEN_EXISTING = 3u;

            private const uint FILE_ATTRIBUTE_DEVICE = 64u;

            private const uint FILE_FLAG_NO_BUFFERING = 536870912u;

            private const uint FILE_FLAG_WRITE_THROUGH = 2147483648u;

            private const uint DISK_BASE = 7u;

            private const uint FILE_ANY_ACCESS = 0u;

            private const uint FILE_SHARE_READ = 1u;

            private const uint FILE_SHARE_WRITE = 2u;

            private const uint FILE_DEVICE_FILE_SYSTEM = 9u;

            private const uint METHOD_BUFFERED = 0u;

            private static readonly uint DISK_GET_DRIVE_GEOMETRY_EX = CTL_CODE(7u, 40u, 0u, 0u);

            private static readonly uint FSCTL_LOCK_VOLUME = CTL_CODE(9u, 6u, 0u, 0u);

            private static readonly uint FSCTL_UNLOCK_VOLUME = CTL_CODE(9u, 7u, 0u, 0u);

            private SafeFileHandle handleValue;

            private long _Position;

            private long _length;

            private uint _sectorsize;

            private bool _canWrite;

            private bool _canRead;

            private bool disposed;

            public override bool CanRead => _canRead;

            public override bool CanSeek => true;

            public override bool CanWrite => _canWrite;

            public override long Length => _length;

            public override long Position
            {
                get
                {
                    return _Position;
                }
                set
                {
                    Seek(value, SeekOrigin.Begin);
                }
            }

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToRead, ref int lpNumberOfBytesRead, IntPtr lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, ref int lpNumberOfBytesWritten, IntPtr lpOverlapped);

            [DllImport("kernel32.dll", SetLastError = true)]
            private static extern uint DeviceIoControl(SafeFileHandle hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, int nOutBufferSize, ref uint lpBytesReturned, IntPtr lpOverlapped);

            [DllImport("kernel32.dll")]
            private static extern bool SetFilePointerEx(SafeFileHandle hFile, long liDistanceToMove, out long lpNewFilePointer, uint dwMoveMethod);

            private static uint CTL_CODE(uint DeviceType, uint Function, uint Method, uint Access)
            {
                return (DeviceType << 16) | (Access << 14) | (Function << 2) | Method;
            }

            public DeviceStream(string device, FileAccess access)
            {
                if (string.IsNullOrEmpty(device))
                {
                    throw new ArgumentNullException("device");
                }
                uint fileAccess = 0u;
                switch (access)
                {
                    case FileAccess.Read:
                        fileAccess = 2147483648u;
                        _canRead = true;
                        break;
                    case FileAccess.ReadWrite:
                        fileAccess = 3221225472u;
                        _canRead = true;
                        _canWrite = true;
                        break;
                    case FileAccess.Write:
                        fileAccess = 1073741824u;
                        _canWrite = true;
                        break;
                }
                string text = "\\\\.\\PhysicalDrive" + device.ToLower().Replace("\\\\.\\physicaldrive", "");
                (long, uint) diskProperties = GetDiskProperties(text);
                _length = diskProperties.Item1;
                _sectorsize = diskProperties.Item2;
                IntPtr ptr = CreateFile(text, fileAccess, 0u, IntPtr.Zero, 3u, 2684354624u, IntPtr.Zero);
                handleValue = new SafeFileHandle(ptr, ownsHandle: true);
                if (handleValue.IsInvalid)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                uint lpBytesReturned = 0u;
                if (DeviceIoControl(handleValue, FSCTL_LOCK_VOLUME, IntPtr.Zero, 0u, IntPtr.Zero, 0, ref lpBytesReturned, IntPtr.Zero) == 0)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
            }

            public override void Flush()
            {
            }

            private int InternalRead(byte[] buffer, int offset, int count)
            {
                int BytesRead = 0;
                byte[] BufBytes = new byte[count];
                if (!ReadFile(handleValue.DangerousGetHandle(), BufBytes, count, ref BytesRead, IntPtr.Zero))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                for (int i = 0; i < BytesRead; i++)
                {
                    buffer[offset + i] = BufBytes[i];
                }
                _Position += count;
                return BytesRead;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                if ((long)count % (long)_sectorsize != 0L)
                {
                    long extrastart = Position % (long)_sectorsize;
                    if (extrastart != 0L)
                    {
                        Seek(-extrastart, SeekOrigin.Current);
                    }
                    long addedcount = _sectorsize - (long)count % (long)_sectorsize;
                    long ncount = count + addedcount;
                    byte[] tmpbuffer = new byte[extrastart + buffer.Length + addedcount];
                    buffer.CopyTo(tmpbuffer, extrastart);
                    InternalRead(tmpbuffer, offset + (int)extrastart, (int)ncount);
                    tmpbuffer.ToList().Skip((int)extrastart).Take(count + offset)
                        .ToArray()
                        .CopyTo(buffer, 0);
                    return count;
                }
                return InternalRead(buffer, offset, count);
            }

            public override int ReadByte()
            {
                int BytesRead = 0;
                byte[] lpBuffer = new byte[1];
                if (!ReadFile(handleValue.DangerousGetHandle(), lpBuffer, 1, ref BytesRead, IntPtr.Zero))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                _Position++;
                return lpBuffer[0];
            }

            public override void WriteByte(byte Byte)
            {
                int BytesWritten = 0;
                if (!WriteFile(lpBuffer: new byte[1] { Byte }, hFile: handleValue.DangerousGetHandle(), nNumberOfBytesToWrite: 1, lpNumberOfBytesWritten: ref BytesWritten, lpOverlapped: IntPtr.Zero))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                _Position++;
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                long off = offset;
                switch (origin)
                {
                    case SeekOrigin.Current:
                        off += _Position;
                        break;
                    case SeekOrigin.End:
                        off += _length;
                        break;
                }
                if (!SetFilePointerEx(handleValue, off, out var ret, 0u))
                {
                    return _Position;
                }
                _Position = ret;
                return ret;
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                int BytesWritten = 0;
                byte[] BufBytes = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    BufBytes[offset + i] = buffer[i];
                }
                if (!WriteFile(handleValue.DangerousGetHandle(), BufBytes, count, ref BytesWritten, IntPtr.Zero))
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                _Position += count;
            }

            public override void Close()
            {
                uint lpBytesReturned = 0u;
                if (DeviceIoControl(handleValue, FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0u, IntPtr.Zero, 0, ref lpBytesReturned, IntPtr.Zero) == 0)
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                handleValue.Close();
                handleValue.Dispose();
                handleValue = null;
                base.Close();
            }

            private new void Dispose()
            {
                Dispose(disposing: true);
                base.Dispose();
                GC.SuppressFinalize(this);
            }

            private new void Dispose(bool disposing)
            {
                if (disposed)
                {
                    return;
                }
                if (disposing && handleValue != null)
                {
                    uint lpBytesReturned = 0u;
                    if (DeviceIoControl(handleValue, FSCTL_UNLOCK_VOLUME, IntPtr.Zero, 0u, IntPtr.Zero, 0, ref lpBytesReturned, IntPtr.Zero) == 0)
                    {
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }
                    handleValue.Close();
                    handleValue.Dispose();
                    handleValue = null;
                }
                disposed = true;
            }

            private static (long, uint) GetDiskProperties(string deviceName)
            {
                DISK_GEOMETRY_EX x = default(DISK_GEOMETRY_EX);
                Execute(ref x, DISK_GET_DRIVE_GEOMETRY_EX, deviceName, 2147483648u, 3u, (IntPtr)0, 3u, 0u, (IntPtr)0);
                return (x.DiskSize, x.Geometry.BytesPerSector);
            }

            private static void Execute<T>(ref T x, uint dwIoControlCode, string lpFileName, uint dwDesiredAccess = 2147483648u, uint dwShareMode = 3u, IntPtr lpSecurityAttributes = default(IntPtr), uint dwCreationDisposition = 3u, uint dwFlagsAndAttributes = 0u, IntPtr hTemplateFile = default(IntPtr))
            {
                SafeFileHandle safeFileHandle = new SafeFileHandle(CreateFile(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile), ownsHandle: true);
                if (safeFileHandle.IsInvalid)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                int nOutBufferSize = Marshal.SizeOf(typeof(T));
                IntPtr lpOutBuffer = Marshal.AllocHGlobal(nOutBufferSize);
                uint lpBytesReturned = 0u;
                if (DeviceIoControl(safeFileHandle, dwIoControlCode, IntPtr.Zero, 0u, lpOutBuffer, nOutBufferSize, ref lpBytesReturned, IntPtr.Zero) == 0)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                x = (T)Marshal.PtrToStructure(lpOutBuffer, typeof(T));
                Marshal.FreeHGlobal(lpOutBuffer);
                safeFileHandle.Close();
                safeFileHandle.Dispose();
            }
        }

        internal class FlashPart
        {
            public Stream Stream;

            public ulong StartLocation;

            public FlashPart(Stream Stream, ulong StartLocation)
            {
                this.Stream = Stream;
                this.StartLocation = StartLocation;
            }
        }

        public class FlashingPayload
        {
            public uint ChunkCount;

            public byte[][] ChunkHashes;

            public uint[] TargetLocations;

            public uint[] StreamIndexes;

            public long[] StreamLocations;

            public FlashingPayload(uint ChunkCount, byte[][] ChunkHashes, uint[] TargetLocations, uint[] StreamIndexes, long[] StreamLocations)
            {
                this.ChunkCount = ChunkCount;
                this.ChunkHashes = ChunkHashes;
                this.TargetLocations = TargetLocations;
                this.StreamIndexes = StreamIndexes;
                this.StreamLocations = StreamLocations;
            }

            public uint GetSecurityHeaderSize()
            {
                return (uint)(32 * ChunkHashes.Count());
            }

            public uint GetStoreHeaderSize()
            {
                return (uint)(8 * (TargetLocations.Count() + 1));
            }
        }

        internal class FlashingPayloadGenerator
        {
            private static void ShowProgress(long CurrentProgress, long TotalProgress, DateTime startTime, bool DisplayRed)
            {
                TimeSpan remaining = TimeSpan.FromMilliseconds((DateTime.Now - startTime).TotalMilliseconds / (double)CurrentProgress * (double)(TotalProgress - CurrentProgress));
                Log(string.Format("{0} {1:hh\\:mm\\:ss\\.f}", GetDismLikeProgBar((int)(CurrentProgress * 100 / TotalProgress)), remaining, remaining.TotalHours, remaining.Minutes, remaining.Seconds, remaining.Milliseconds), DisplayRed ? LoggingLevel.Warning : LoggingLevel.Information, returnline: false);
            }

            private static string GetDismLikeProgBar(int perc)
            {
                int eqsLength = (int)((double)perc / 100.0 * 55.0);
                string bases = new string('=', eqsLength) + new string(' ', 55 - eqsLength);
                bases = bases.Insert(28, perc + "%");
                if (perc == 100)
                {
                    bases = bases.Substring(1);
                }
                else if (perc < 10)
                {
                    bases = bases.Insert(28, " ");
                }
                return "[" + bases + "]";
            }

            internal static FlashingPayload[] GetOptimizedPayloads(FlashPart[] flashParts, uint chunkSize, ulong PlatEnd)
            {
                List<FlashingPayload> flashingPayloads = new List<FlashingPayload>();
                if (flashParts == null)
                {
                    return flashingPayloads.ToArray();
                }
                long TotalProcess1 = 0L;
                for (int k = 0; k < flashParts.Count(); k++)
                {
                    FlashPart flashPart = flashParts[k];
                    TotalProcess1 += flashPart.Stream.Length / (long)chunkSize;
                }
                long CurrentProcess1 = 0L;
                DateTime startTime = DateTime.Now;
                Log("Hashing resources...");
                using (SHA256 crypto = SHA256.Create())
                {
                    for (uint j = 0u; j < flashParts.Count(); j++)
                    {
                        FlashPart flashPart2 = flashParts[j];
                        flashPart2.Stream.Seek(0L, SeekOrigin.Begin);
                        long totalChunkCount = flashPart2.Stream.Length / (long)chunkSize;
                        for (uint i = 0u; i < totalChunkCount; i++)
                        {
                            byte[] buffer = new byte[chunkSize];
                            long position = flashPart2.Stream.Position;
                            flashPart2.Stream.Read(buffer, 0, (int)chunkSize);
                            byte[] hash = crypto.ComputeHash(buffer);
                            byte[] emptyness = new byte[32]
                            {
                            250, 67, 35, 155, 206, 231, 185, 124, 166, 47,
                            0, 124, 198, 132, 135, 86, 10, 57, 225, 159,
                            116, 243, 221, 231, 72, 109, 179, 249, 141, 248,
                            228, 113
                            };
                            if ((uint)flashPart2.StartLocation / chunkSize + i < PlatEnd || !ByteOperations.Compare(emptyness, hash))
                            {
                                flashingPayloads.Add(new FlashingPayload(1u, new byte[1][] { hash }, new uint[1] { (uint)flashPart2.StartLocation / chunkSize + i }, new uint[1] { j }, new long[1] { position }));
                            }
                            CurrentProcess1++;
                            ShowProgress(CurrentProcess1, TotalProcess1, startTime, (uint)flashPart2.StartLocation / chunkSize + i < PlatEnd);
                        }
                    }
                }
                return flashingPayloads.ToArray();
            }
        }

        internal class FullFlash
        {
            public string AntiTheftVersion = "1.1";

            public string OSVersion;

            public string Description = "Update on: " + DateTime.Now.ToString("u") + "::\r\n";

            public string Version = "2.0";

            public string DevicePlatformId0;
        }

        public class GPT
        {
            public class Partition
            {
                private ulong _SizeInSectors;

                private ulong _FirstSector;

                private ulong _LastSector;

                public string Name;

                public Guid PartitionTypeGuid;

                public Guid PartitionGuid;

                internal ulong Attributes;

                internal ulong SizeInSectors
                {
                    get
                    {
                        if (_SizeInSectors != 0L)
                        {
                            return _SizeInSectors;
                        }
                        return LastSector - FirstSector + 1;
                    }
                    set
                    {
                        _SizeInSectors = value;
                        if (FirstSector != 0L)
                        {
                            LastSector = FirstSector + _SizeInSectors - 1;
                        }
                    }
                }

                internal ulong FirstSector
                {
                    get
                    {
                        return _FirstSector;
                    }
                    set
                    {
                        _FirstSector = value;
                        if (_SizeInSectors != 0L)
                        {
                            _LastSector = FirstSector + _SizeInSectors - 1;
                        }
                    }
                }

                internal ulong LastSector
                {
                    get
                    {
                        return _LastSector;
                    }
                    set
                    {
                        _LastSector = value;
                        _SizeInSectors = 0uL;
                    }
                }

                public string Volume => "\\\\?\\Volume" + PartitionGuid.ToString("b") + "\\";

                public string FirstSectorAsString
                {
                    get
                    {
                        return "0x" + FirstSector.ToString("X16");
                    }
                    set
                    {
                        FirstSector = Convert.ToUInt64(value, 16);
                    }
                }

                public string LastSectorAsString
                {
                    get
                    {
                        return "0x" + LastSector.ToString("X16");
                    }
                    set
                    {
                        LastSector = Convert.ToUInt64(value, 16);
                    }
                }

                public string AttributesAsString
                {
                    get
                    {
                        return "0x" + Attributes.ToString("X16");
                    }
                    set
                    {
                        Attributes = Convert.ToUInt64(value, 16);
                    }
                }
            }

            private byte[] GPTBuffer;

            private uint HeaderOffset;

            private uint HeaderSize;

            private uint TableOffset;

            private uint TableSize;

            private uint PartitionEntrySize;

            private uint MaxPartitions;

            internal ulong FirstUsableSector;

            internal ulong LastUsableSector;

            internal bool HasChanged;

            public List<Partition> Partitions = new List<Partition>();

            internal GPT(byte[] GPTBuffer)
            {
                this.GPTBuffer = GPTBuffer;
                uint? TempHeaderOffset = ByteOperations.FindAscii(GPTBuffer, "EFI PART");
                if (!TempHeaderOffset.HasValue)
                {
                    throw new Exception("Bad GPT");
                }
                HeaderOffset = TempHeaderOffset.Value;
                HeaderSize = ByteOperations.ReadUInt32(GPTBuffer, HeaderOffset + 12);
                TableOffset = HeaderOffset + 512;
                FirstUsableSector = ByteOperations.ReadUInt64(GPTBuffer, HeaderOffset + 40);
                LastUsableSector = ByteOperations.ReadUInt64(GPTBuffer, HeaderOffset + 48);
                MaxPartitions = ByteOperations.ReadUInt32(GPTBuffer, HeaderOffset + 80);
                PartitionEntrySize = ByteOperations.ReadUInt32(GPTBuffer, HeaderOffset + 84);
                TableSize = MaxPartitions * PartitionEntrySize;
                if (TableOffset + TableSize > GPTBuffer.Length)
                {
                    throw new Exception("Bad GPT");
                }
                for (uint PartitionOffset = TableOffset; PartitionOffset < TableOffset + TableSize; PartitionOffset += PartitionEntrySize)
                {
                    string Name = ByteOperations.ReadUnicodeString(GPTBuffer, PartitionOffset + 56, 72u).TrimEnd('\0', ' ');
                    if (Name.Length == 0)
                    {
                        break;
                    }
                    Partition CurrentPartition = new Partition
                    {
                        Name = Name,
                        FirstSector = ByteOperations.ReadUInt64(GPTBuffer, PartitionOffset + 32),
                        LastSector = ByteOperations.ReadUInt64(GPTBuffer, PartitionOffset + 40),
                        PartitionTypeGuid = ByteOperations.ReadGuid(GPTBuffer, PartitionOffset),
                        PartitionGuid = ByteOperations.ReadGuid(GPTBuffer, PartitionOffset + 16),
                        Attributes = ByteOperations.ReadUInt64(GPTBuffer, PartitionOffset + 48)
                    };
                    Partitions.Add(CurrentPartition);
                }
                HasChanged = false;
            }

            internal Partition GetPartition(string Name)
            {
                return Partitions.Where((Partition p) => string.Compare(p.Name, Name, ignoreCase: true) == 0).FirstOrDefault();
            }

            internal byte[] Rebuild()
            {
                if (GPTBuffer == null)
                {
                    TableSize = 16896u;
                    TableOffset = 0u;
                    GPTBuffer = new byte[TableSize];
                }
                else
                {
                    Array.Clear(GPTBuffer, (int)TableOffset, (int)TableSize);
                }
                uint PartitionOffset = TableOffset;
                foreach (Partition CurrentPartition in Partitions)
                {
                    ByteOperations.WriteGuid(GPTBuffer, PartitionOffset, CurrentPartition.PartitionTypeGuid);
                    ByteOperations.WriteGuid(GPTBuffer, PartitionOffset + 16, CurrentPartition.PartitionGuid);
                    ByteOperations.WriteUInt64(GPTBuffer, PartitionOffset + 32, CurrentPartition.FirstSector);
                    ByteOperations.WriteUInt64(GPTBuffer, PartitionOffset + 40, CurrentPartition.LastSector);
                    ByteOperations.WriteUInt64(GPTBuffer, PartitionOffset + 48, CurrentPartition.Attributes);
                    ByteOperations.WriteUnicodeString(GPTBuffer, PartitionOffset + 56, CurrentPartition.Name, 72u);
                    PartitionOffset += PartitionEntrySize;
                }
                ByteOperations.WriteUInt32(GPTBuffer, HeaderOffset + 88, ByteOperations.CRC32(GPTBuffer, TableOffset, TableSize));
                ByteOperations.WriteUInt32(GPTBuffer, HeaderOffset + 16, 0u);
                ByteOperations.WriteUInt32(GPTBuffer, HeaderOffset + 16, ByteOperations.CRC32(GPTBuffer, HeaderOffset, HeaderSize));
                return GPTBuffer;
            }
        }

        internal class ImageHeader
        {
            public uint Size = 24u;

            public string Signature = "ImageFlash  ";

            public uint ManifestLength;

            public uint ChunkSize = 128u;
        }

        internal class ImageSplitter
        {
            private static readonly string[] excluded = new string[35]
            {
            "DPP", "MODEM_FSG", "MODEM_FS1", "MODEM_FS2", "MODEM_FSC", "DDR", "SEC", "APDP", "MSADP", "DPO",
            "SSD", "DBI", "UEFI_BS_NV", "UEFI_NV", "UEFI_RT_NV", "UEFI_RT_NV_RPMB", "BOOTMODE", "LIMITS", "BACKUP_BS_NV", "BACKUP_SBL1",
            "BACKUP_SBL2", "BACKUP_SBL3", "BACKUP_PMIC", "BACKUP_DBI", "BACKUP_UEFI", "BACKUP_RPM", "BACKUP_QSEE", "BACKUP_QHEE", "BACKUP_TZ", "BACKUP_HYP",
            "BACKUP_WINSECAPP", "BACKUP_TZAPPS", "SVRawDump", "IS_UNLOCKED", "HACK"
            };

            internal static (FlashPart[], ulong, List<GPT.Partition> partitions) GetImageSlices(Stream stream, uint chunkSize)
            {
                byte[] GPTBuffer = new byte[chunkSize];
                stream.Read(GPTBuffer, 0, (int)chunkSize);
                uint sectorsInAChunk = chunkSize / 512u;
                GPT GPT = new GPT(GPTBuffer);
                List<GPT.Partition> Partitions = GPT.Partitions;
                bool isUnlocked = GPT.GetPartition("IS_UNLOCKED") != null;
                bool isUnlockedSpecA = GPT.GetPartition("HACK") != null && GPT.GetPartition("BACKUP_BS_NV") != null;
                if (isUnlocked)
                {
                    Log("The phone is an unlocked Spec B phone, UEFI_BS_NV will be kept in the FFU image for the unlock to work");
                }
                if (isUnlockedSpecA)
                {
                    Log("The phone is an UEFI unlocked Spec A phone, UEFI_BS_NV will be kept in the FFU image for the unlock to work");
                }
                List<FlashPart> flashParts = new List<FlashPart>();
                Log("Partitions with a * appended are ignored partitions");
                Log("");
                bool previouswasexcluded = true;
                FlashPart currentFlashPart = null;
                ulong PlatEnd = 0uL;
                foreach (GPT.Partition partition in Partitions.OrderBy((GPT.Partition x) => x.FirstSector))
                {
                    if (partition.Name == "PLAT")
                    {
                        PlatEnd = partition.LastSector / sectorsInAChunk;
                    }
                    bool isExcluded = false;
                    if (excluded.Any((string x) => x == partition.Name))
                    {
                        isExcluded = true;
                        if (isUnlocked && partition.Name == "UEFI_BS_NV")
                        {
                            isExcluded = false;
                        }
                        if (isUnlockedSpecA && partition.Name == "UEFI_BS_NV")
                        {
                            isExcluded = false;
                        }
                    }
                    Log(((isExcluded ? "*" : "") + partition.Name + new string(' ', 50)).Insert(25, " - " + partition.FirstSector).Insert(40, " - " + partition.LastSector), isExcluded ? LoggingLevel.Warning : LoggingLevel.Information);
                    if (isExcluded)
                    {
                        previouswasexcluded = true;
                        if (currentFlashPart != null)
                        {
                            if (currentFlashPart.StartLocation / 512uL % 256uL != 0L)
                            {
                                Log("- The stream doesn't start on a chunk boundary, a chunk is 256 sectors", LoggingLevel.Error);
                                throw new Exception();
                            }
                            if (currentFlashPart.Stream.Length / 512 % (long)sectorsInAChunk != 0L)
                            {
                                Log("- The stream doesn't finish on a chunk boundary, a chunk is 256 sectors", LoggingLevel.Error);
                                throw new Exception();
                            }
                            flashParts.Add(currentFlashPart);
                            currentFlashPart = null;
                        }
                    }
                    else
                    {
                        if (previouswasexcluded)
                        {
                            currentFlashPart = new FlashPart(stream, partition.FirstSector * 512);
                        }
                        previouswasexcluded = false;
                        currentFlashPart.Stream = new PartialStream(stream, (long)currentFlashPart.StartLocation, (long)((partition.LastSector + 1) * 512));
                    }
                }
                if (!previouswasexcluded && currentFlashPart != null)
                {
                    currentFlashPart.Stream = new PartialStream(stream, (long)currentFlashPart.StartLocation, stream.Length);
                    if (currentFlashPart.StartLocation / 512uL % 256uL != 0L)
                    {
                        Log("- The stream doesn't start on a chunk boundary, a chunk is 256 sectors", LoggingLevel.Error);
                        throw new Exception();
                    }
                    if (currentFlashPart.Stream.Length / 512 % (long)sectorsInAChunk != 0L)
                    {
                        Log("- The stream doesn't finish on a chunk boundary, a chunk is 256 sectors", LoggingLevel.Error);
                        throw new Exception();
                    }
                    flashParts.Add(currentFlashPart);
                }
                Log("");
                Log("Plat end: " + PlatEnd);
                Log("");
                Log("Inserting GPT back into the FFU image");
                flashParts.Insert(0, new FlashPart(new MemoryStream(GPTBuffer), 0uL));
                return (flashParts.OrderBy((FlashPart x) => x.StartLocation).ToArray(), PlatEnd, Partitions);
            }
        }

        internal class ManifestIni
        {
            internal static string BuildUpManifest(FullFlash fullFlash, Store store, List<Partition> partitions)
            {
                string Manifest = "[FullFlash]\r\n";
                if (!string.IsNullOrEmpty(fullFlash.AntiTheftVersion))
                {
                    Manifest = Manifest + "AntiTheftVersion  = " + fullFlash.AntiTheftVersion + "\r\n";
                }
                if (!string.IsNullOrEmpty(fullFlash.OSVersion))
                {
                    Manifest = Manifest + "OSVersion         = " + fullFlash.OSVersion + "\r\n";
                }
                if (!string.IsNullOrEmpty(fullFlash.Description))
                {
                    Manifest = Manifest + "Description       = " + fullFlash.Description + "\r\n";
                }
                if (!string.IsNullOrEmpty(fullFlash.Version))
                {
                    Manifest = Manifest + "Version           = " + fullFlash.Version + "\r\n";
                }
                if (!string.IsNullOrEmpty(fullFlash.DevicePlatformId0))
                {
                    Manifest = Manifest + "DevicePlatformId0 = " + fullFlash.DevicePlatformId0 + "\r\n";
                }
                Manifest += "\r\n[Store]\r\n";
                Manifest = Manifest + "SectorSize     = " + store.SectorSize + "\r\n";
                Manifest = Manifest + "MinSectorCount = " + store.MinSectorCount + "\r\n";
                Manifest += "\r\n";
                foreach (Partition partition in partitions)
                {
                    Manifest += "[Partition]\r\n";
                    if (partition.RequiredToFlash.HasValue)
                    {
                        Manifest = Manifest + "RequiredToFlash = " + (partition.RequiredToFlash.Value ? "True" : "False") + "\r\n";
                    }
                    if (!string.IsNullOrEmpty(partition.Name))
                    {
                        Manifest = Manifest + "Name            = " + partition.Name + "\r\n";
                    }
                    Manifest = Manifest + "UsedSectors     = " + partition.UsedSectors + "\r\n";
                    _ = partition.Type;
                    string text = Manifest;
                    Guid type = partition.Type;
                    Manifest = text + "Type            = " + type.ToString() + "\r\n";
                    Manifest = Manifest + "TotalSectors    = " + partition.TotalSectors + "\r\n";
                    if (!string.IsNullOrEmpty(partition.Primary))
                    {
                        Manifest = Manifest + "Primary         = " + partition.Primary + "\r\n";
                    }
                    if (!string.IsNullOrEmpty(partition.FileSystem))
                    {
                        Manifest = Manifest + "FileSystem      = " + partition.FileSystem + "\r\n";
                    }
                    if (partition.ByteAlignment.HasValue)
                    {
                        Manifest = Manifest + "ByteAlignment   = " + partition.ByteAlignment.Value + "\r\n";
                    }
                    if (partition.ClusterSize.HasValue)
                    {
                        Manifest = Manifest + "ClusterSize     = " + partition.ClusterSize.Value + "\r\n";
                    }
                    if (partition.UseAllSpace.HasValue)
                    {
                        Manifest = Manifest + "UseAllSpace     = " + (partition.UseAllSpace.Value ? "True" : "False") + "\r\n";
                    }
                    Manifest += "\r\n";
                }
                return Manifest;
            }

            internal static List<Partition> GPTPartitionsToPartitions(List<GPT.Partition> partitions)
            {
                List<Partition> Parts = new List<Partition>();
                foreach (GPT.Partition part in partitions)
                {
                    Parts.Add(new Partition
                    {
                        Name = part.Name,
                        TotalSectors = (uint)part.SizeInSectors,
                        UsedSectors = (uint)part.SizeInSectors,
                        RequiredToFlash = true
                    });
                }
                return Parts;
            }

            internal static string BuildUpManifest(FullFlash fullFlash, Store store, List<GPT.Partition> partitions)
            {
                return BuildUpManifest(fullFlash, store, GPTPartitionsToPartitions(partitions));
            }
        }

        internal class PartialStream : Stream
        {
            private Stream innerstream;

            private bool disposed;

            private long start;

            private long end;

            public override bool CanRead => innerstream.CanRead;

            public override bool CanSeek => innerstream.CanSeek;

            public override bool CanWrite => innerstream.CanWrite;

            public override long Length => end - start;

            public override long Position
            {
                get
                {
                    return innerstream.Position - start;
                }
                set
                {
                    innerstream.Position = value + start;
                }
            }

            public PartialStream(Stream stream, long StartOffset, long EndOffset)
            {
                stream.Seek(StartOffset, SeekOrigin.Begin);
                start = StartOffset;
                end = EndOffset;
                innerstream = stream;
            }

            public override void Flush()
            {
                innerstream.Flush();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                return innerstream.Read(buffer, offset, count);
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                return origin switch
                {
                    SeekOrigin.Begin => innerstream.Seek(offset + start, origin),
                    SeekOrigin.End => innerstream.Seek(end + offset, origin),
                    _ => innerstream.Seek(offset, origin),
                };
            }

            public override void SetLength(long value)
            {
                innerstream.SetLength(value);
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                innerstream.Write(buffer, offset, count);
            }

            public override void Close()
            {
                innerstream.Dispose();
                innerstream = null;
                base.Close();
            }

            private new void Dispose()
            {
                Dispose(disposing: true);
                base.Dispose();
                GC.SuppressFinalize(this);
            }

            private new void Dispose(bool disposing)
            {
                if (!disposed)
                {
                    if (disposing && innerstream != null)
                    {
                        innerstream.Dispose();
                        innerstream = null;
                    }
                    disposed = true;
                }
            }
        }

        internal class Partition
        {
            public bool? RequiredToFlash;

            public uint UsedSectors;

            public Guid Type;

            public uint TotalSectors;

            public string Primary;

            public string Name;

            public string FileSystem;

            public uint? ByteAlignment;

            public uint? ClusterSize;

            public bool? UseAllSpace;
        }

        public class SecurityHeader
        {
            public uint Size = 32u;

            public string Signature = "SignedImage ";

            public uint ChunkSizeInKb = 128u;

            public uint HashAlgorithm = 32780u;

            public uint CatalogSize;

            public uint HashTableSize;
        }

        internal class Store
        {
            public uint SectorSize;

            public uint MinSectorCount;
        }

        public class StoreHeader
        {
            public uint UpdateType;

            public ushort MajorVersion = 1;

            public ushort MinorVersion;

            public ushort FullFlashMajorVersion = 2;

            public ushort FullFlashMinorVersion;

            public string PlatformId;

            public uint BlockSizeInBytes = 131072u;

            public uint WriteDescriptorCount;

            public uint WriteDescriptorLength;

            public uint ValidateDescriptorCount;

            public uint ValidateDescriptorLength;

            public uint InitialTableIndex;

            public uint InitialTableCount;

            public uint FlashOnlyTableIndex;

            public uint FlashOnlyTableCount = 1u;

            public uint FinalTableIndex;

            public uint FinalTableCount;
        }
    }
}
