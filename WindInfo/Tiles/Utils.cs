namespace WindInfo.Code
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;
    using System.Xml;
    using Tiles;
    using WindDataLib;
#if WINDOWS_PHONE
    using ToolStackPNGWriterLib;
#endif

    public static class Utils
    {
#if WINDOWS_PHONE
        public static _159_159 _159_159 = new _159_159();
        public static _336_336 _336_336 = new _336_336();
        public static void DeleteCreditState()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists("creditData"))
                    file.DeleteFile("creditData");

                if (file.FileExists("creditDataArray"))
                    file.DeleteFile("creditDataArray");
            }
        }

        public static string GetAppAttributeValue(string attributeName)
        {
            XmlReaderSettings settings2 = new XmlReaderSettings();
            settings2.XmlResolver = (new XmlXapResolver());
            XmlReaderSettings settings = settings2;
            using (XmlReader reader = XmlReader.Create("WMAppManifest.xml", settings))
            {
                reader.ReadToDescendant("App");
                return reader.GetAttribute(attributeName);
            }
        }

        public static CreditInfo GetSavedState()
        {
            string stringSavedState = GetStringSavedState();
            if (string.IsNullOrEmpty(stringSavedState))
            {
                return new CreditInfo();
            }
            return JsonConvert.DeserializeObject<CreditInfo>(stringSavedState);
        }



        public static string GetStringSavedState()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists("creditData"))
                {
                    using (IsolatedStorageFileStream stream = file.OpenFile("creditData", FileMode.Open))
                    {
                        if (stream.Length == 0L)
                        {
                            return null;
                        }
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        byte[] bytes = ProtectedData.Unprotect(buffer, null);
                        return Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                    }
                }
                return string.Empty;
            }
        }

        public static void RenderTile(int width, int height, string number, object context)
        {

            string path = string.Format("/Shared/ShellContent/{0}_{1}_{2}.jpg", width, height, number);

            WriteableBitmap bmp = null;
            UserControl control = null;

            ManualResetEventSlim evt = new ManualResetEventSlim(false, 50);

            var act = new Action(() =>
            {
                evt.Reset();
                control = (width == 0x9f) ? _159_159 as UserControl : _336_336 as UserControl;
                control.DataContext = context;
                control.Measure(new Size((double)width, (double)height));
                control.UpdateLayout();
                control.Arrange(new Rect(0.0, 0.0, (double)width, (double)height));
                control.UpdateLayout();

                bmp = new WriteableBitmap(width, height);
                bmp.Render(control, null);
                bmp.Invalidate();
                evt.Set();

            });

            if (!Deployment.Current.Dispatcher.CheckAccess())
                Deployment.Current.Dispatcher.BeginInvoke(act);
            else
                act();

            evt.Wait();

            if (bmp != null)
            {
                using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(path, FileMode.Create, FileAccess.Write, file))
                    {
                        PNGWriter.WritePNG(bmp, stream, -1);
                        //Extensions.SaveJpeg(bmp, stream, width, height, 0, 100);
                    }
                }
            }
            bmp = null;
        }

        public static void RenderTiles(string number, object context)
        {
            RenderTile(0x9f, 0x9f, number, context);
            RenderTile(0x150, 0x150, number, context);
        }

        public static void SaveCreditState(CreditInfo info)
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = file.OpenFile("creditData", FileMode.Create))
                {
                    string s = JsonConvert.SerializeObject(info);
                    byte[] buffer = ProtectedData.Protect(Encoding.UTF8.GetBytes(s), null);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public static void SaveCreditState(IList<CreditInfo> info)
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = file.OpenFile("creditDataArray", FileMode.Create))
                {
                    string s = JsonConvert.SerializeObject(info);
                    byte[] buffer = ProtectedData.Protect(Encoding.UTF8.GetBytes(s), null);
                    stream.Write(buffer, 0, buffer.Length);
                }
            }
        }

        public static IList<CreditInfo> GetArraySavedState()
        {
            using (IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists("creditDataArray"))
                {
                    using (IsolatedStorageFileStream stream = file.OpenFile("creditDataArray", FileMode.Open))
                    {
                        if (stream.Length == 0L)
                        {
                            return null;
                        }
                        byte[] buffer = new byte[stream.Length];
                        stream.Read(buffer, 0, buffer.Length);
                        byte[] bytes = ProtectedData.Unprotect(buffer, null);
                        var str = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        return JsonConvert.DeserializeObject<IList<CreditInfo>>(str);
                    }
                }
                return Enumerable.Empty<CreditInfo>().ToList();
            }
        }
#endif
        public static Color ToColor(this uint value)
        {
            return Color.FromArgb((byte)((value >> 0x18) & 0xff), (byte)((value >> 0x10) & 0xff), (byte)((value >> 8) & 0xff), (byte)(value & 0xff));
        }

        public static uint ToUint(this Color c)
        {
            return (uint)(((((c.A << 0x18) | (c.R << 0x10)) | (c.G << 8)) | c.B) & 0xffffffffL);
        }
    }
}

