using System.Drawing;
using System.IO;

namespace LaserGRBL.Project
{
    public class FileObject
    {
        public string FileName { get; set; }
        public byte[] ByteArray { get; set; }
        public FileObject(string fileName, byte[] byteArray = null)
        {
            // TODO: some validation
            this.FileName = fileName; 
            this.ByteArray = byteArray ?? File.ReadAllBytes(fileName);
        }

        public Bitmap ToBitmap()
        {
            Bitmap bmp;
            using (var ms = new MemoryStream(ByteArray))
            {
                bmp = new Bitmap(ms);
            }
            return bmp;
        }
        public void FromBitMap(Bitmap bmp)
        {
                using (var stream = new MemoryStream())
                {
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    ByteArray= stream.ToArray();
                }
        }
    }
}
