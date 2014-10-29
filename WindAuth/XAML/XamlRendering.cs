using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Markup;
using System.Xml;
using Tiles;
using WindDataLib;
using System.Web.Hosting;
public static class XamlRendering
{

    public static void ConvertToJpg(NumberInfo context)
    {
        foreach (var oCanvas in new Control[] { new _159_159(), new _336_336() })
        {
            oCanvas.DataContext = context;

            int x = oCanvas.GetType().FullName.Contains("159") ? 159 : oCanvas.GetType().FullName.Contains("691") ? 691 : 336;
            oCanvas.DataContext = context;
            oCanvas.Measure(new Size((double)x, (double)(x == 691 ? 336 : x)));
            oCanvas.UpdateLayout();
            oCanvas.Arrange(new Rect(0.0, 0.0, x, x == 691 ? 336 : x));
            oCanvas.UpdateLayout();

            RenderTargetBitmap oRenderTargetBitmap = new RenderTargetBitmap(x, x == 691 ? 336 : x, 96, 96, PixelFormats.Default);
            oRenderTargetBitmap.Render(oCanvas);
            PngBitmapEncoder oPngBitmapEncoder = new PngBitmapEncoder();
            oPngBitmapEncoder.Frames.Add(BitmapFrame.Create(oRenderTargetBitmap));
            using (FileStream oFileStream = new FileStream(Path.Combine(HostingEnvironment.MapPath("~/Tiles/"), context.Number + "_" + x + ".jpg"), FileMode.Create))
            {
                oPngBitmapEncoder.Save(oFileStream);
            }

        }

    }

}