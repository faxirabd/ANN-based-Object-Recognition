/****************************************************************************
While the underlying libraries are covered by LGPL, this sample is released 
as public domain.  It is distributed in the hope that it will be useful, but 
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
or FITNESS FOR A PARTICULAR PURPOSE.  
*****************************************************************************/

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text;
using System.Net;
using VideoModul;
using System.Windows;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace VideoModul
{
    public class WebCamService
    {


        #region Member Variables
        private const int MAXOUTSTANDINGPACKETS = 3;

        /// <summary>
        /// The thread will run the job.
        /// The job is the Method Run() below
        /// </summary>
        protected Thread thread = null;
        private ManualResetEvent ConnectionReady;
        private volatile bool bShutDown;
        private volatile int iConnectionCount;


        PictureBox img = null;
        #endregion

        public WebCamService(PictureBox img)
        {
            this.img = img;
        }


        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        public void Start()
        {
            if (thread == null)
            {
                ThreadStart starter = new ThreadStart(Run);
                thread = new Thread(starter);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        /// <summary>
        /// Stop this service.
        /// The Run() Method tests for this thread state each second
        /// </summary>
        public void Stop()
        {
            // Set exit condition
            bShutDown = true;

            // Need to get out of wait
            ConnectionReady.Set();
            //thread.Abort();
        }
        public void Run()
        {
            const int VIDEODEVICE = 0; // zero based index of video capture device to use
            const int FRAMERATE = 30;  // Depends on video device caps.  Generally 4-30.
            const int VIDEOWIDTH = 160; // Depends on video device caps
            const int VIDEOHEIGHT = 120; // Depends on video device caps
            const long JPEGQUALITY = 100; // 1-100 or 0 for default


            Capture cam = null;
            //TcpServer serv = null;
            ImageCodecInfo myImageCodecInfo;
            EncoderParameters myEncoderParameters;

            try
            {
                // Set up member vars
                ConnectionReady = new ManualResetEvent(false);
                bShutDown = false;

                // Set up tcp server
                iConnectionCount = 0;
                /*serv = new TcpServer(port, IPAddress.Parse(actip));//TcpServer.GetAddresses()[0]);
                serv.Connected += new TcpConnected(Connected);
                serv.Disconnected += new TcpConnected(Disconnected);
                serv.DataReceived += new TcpReceive(Receive);
                serv.Send += new TcpSend(Send);
                */
                myEncoderParameters = null;
                myImageCodecInfo = GetEncoderInfo("image/jpeg");

                if (JPEGQUALITY != 0)
                {
                    // If not using the default jpeg quality setting
                    EncoderParameter myEncoderParameter;
                    myEncoderParameter = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, JPEGQUALITY);
                    myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = myEncoderParameter;
                }

                cam = new Capture(VIDEODEVICE, FRAMERATE, VIDEOWIDTH, VIDEOHEIGHT);
                // Initialization succeeded.  Now, start serving up frames
                DoIt(cam, myImageCodecInfo, myEncoderParameters); 
            }
            catch (Exception ex)
            {
                try
                {
                    Console.WriteLine(String.Format("{0}: Failed on startup {1}", DateTime.Now.ToString(), ex));
                }
                catch { }
            }
            finally
            {
                // Cleanup
                /*if (serv != null)
                {
                    serv.Dispose();
                }
                */
                if (cam != null)
                {
                    cam.Dispose();
                }
            }
        }

        // Start serving up frames
        private void DoIt(Capture cam, ImageCodecInfo myImageCodecInfo, EncoderParameters myEncoderParameters)
        {
           // MemoryStream m = new MemoryStream(20000);
            Bitmap image = null;
            IntPtr ip = IntPtr.Zero;
            do
            {
                    // Wait til a client connects before we start the graph
                    //ConnectionReady.WaitOne();
                cam.Start();

                // While not shutting down, and still at least one client
                while ((!bShutDown) )
                {
                    try
                    {

                        // capture image
                        ip = cam.GetBitMap();
                        image = new Bitmap(cam.Width, cam.Height, cam.Stride, PixelFormat.Format24bppRgb, ip);
                        image.RotateFlip(RotateFlipType.Rotate180FlipX);

                        // save it to jpeg using quality options
                        //m.Position = 10;
                        //image.Save(m, myImageCodecInfo, myEncoderParameters);

                        // Send the length as a fixed length string
                       // m.Position = 0;
                        //m.Write(Encoding.ASCII.GetBytes((m.Length - 10).ToString("d8") + "\r\n"), 0, 10);

                        // send the jpeg image
                        //serv.SendToAll(m);
                        //img.Dispatcher.Invoke(dl, m);
                        img.Image = MakeGrayscale(image);
                        //img.Image = MakeGrayscale(ResizeImage(image, 16, 12));// MakeGrayscale(image);
                      
                        // Empty the stream
                        //m.SetLength(0);

                        // remove the image from memory
                        //image.Dispose();
                        //image = null;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            Console.WriteLine(DateTime.Now.ToString());
                            Console.WriteLine(ex);
                        }
                        catch { }
                    }
                    finally
                    {
                        if (ip != IntPtr.Zero)
                        {
                            Marshal.FreeCoTaskMem(ip);
                            ip = IntPtr.Zero;
                        }
                    }
                }

                // Clients have all disconnected.  Pause, then sleep and wait for more
                cam.Pause();
                Console.WriteLine("Dropped frames: " + cam.m_Dropped.ToString());

            } while (!bShutDown);
        }

        private Bitmap ResizeImage(Bitmap imgToResize, int newWidth, int newHeight)
        {
            Bitmap b = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, newWidth, newHeight);
            g.Dispose();

            return b;
        }

        int brightnesscontrol = 0;

        public int BrightnessControl
        {
            get { return brightnesscontrol; }
            set { brightnesscontrol = value; }
        }

        public Bitmap MakeGrayscale(Bitmap original)
        {
            //make an empty bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);
           
            for (int i = 0; i <original.Width; i++)
            {
                for (int j = 0; j < original.Height; j++)
                {
                    //get the pixel from the original image
                    Color originalColor = original.GetPixel(i, j);

                    //create the grayscale version of the pixel
                    //int grayScale = (int)((originalColor.R * .3) + (originalColor.G * .59)
                    //    + (originalColor.B * .11));

                    int grayScale = ((int)((originalColor.R) + (originalColor.G)
                        + (originalColor.B))) / 3;
                    //here must be uncommented if you want
                    //only white and black colors instead of grayscale

                    if (grayScale > brightnesscontrol)
                        grayScale = 255;
                    else
                        grayScale = 0;
                        
                    //create the color object
                    Color newColor = Color.FromArgb(grayScale, grayScale, grayScale);

                    //set the new image's pixel to the grayscale version
                    newBitmap.SetPixel(i, j, newColor);
                }
            }

            return newBitmap;
        }

        class PacketCount : IDisposable
        {
            private int m_PacketCount;
            private int m_MaxPackets;

            public PacketCount(int i)
            {
                m_MaxPackets = i;
                m_PacketCount = 0;
            }

            public bool AddPacket()
            {
                bool b;

                lock (this)
                {
                    b = m_PacketCount < m_MaxPackets;
                    if (b)
                    {
                        m_PacketCount++;
                    }
                    else
                    {
                        Debug.WriteLine("Max outstanding Packets reached");
                    }
                }

                return b;
            }

            public void RemovePacket()
            {
                lock (this)
                {
                    if (m_PacketCount > 0)
                    {
                        m_PacketCount--;
                    }
                    else
                    {
                        Debug.WriteLine("Packet count is messed up");
                    }
                }
            }

            public int Count()
            {
                return m_PacketCount;
            }

            #region IDisposable Members

            public void Dispose()
            {
#if DEBUG
                if (m_PacketCount != 0)
                {
                    Debug.WriteLine("Packets left over: " + m_PacketCount.ToString());
                }
#endif
            }

            #endregion
        }

        // A client attached to the tcp port
        private void Connected(object sender, ref object t)
        {
            lock (this)
            {
                t = new PacketCount(MAXOUTSTANDINGPACKETS);
                iConnectionCount++;

                if (iConnectionCount == 1)
                {
                    ConnectionReady.Set();
                }
            }
        }

     
        private void Receive(Object sender, ref object o, ref byte[] b, int ByteCount)
        {
            PacketCount pc = (PacketCount)o;
            pc.RemovePacket();
        }

        private void Send(Object sender, ref object o, ref bool b)
        {
            PacketCount pc = (PacketCount)o;

            b = pc.AddPacket();
        }

        // Find the appropriate encoder
        private ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
