using System;
using System.Drawing;
using System.Drawing.Imaging;

using AForge.Imaging;

namespace PhotoBrushProject
{
    /// <summary>
    /// IDocumentsHost interface
    /// Provides connectione between documents and the main widnow
    /// </summary>
    public interface IDocumentsHost
    {
        bool CreateNewDocumentOnChange { get; }
        bool RememberOnChange { get; }

        bool NewDocument(Bitmap image);
        bool NewDocument(ComplexImage image);

        Bitmap GetImage(object sender, String text, Size size, PixelFormat format);
    }
}
