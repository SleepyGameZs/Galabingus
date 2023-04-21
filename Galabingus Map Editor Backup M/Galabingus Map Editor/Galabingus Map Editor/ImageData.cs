using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus_Map_Editor
{
    internal struct ImageData
    {
        string fileName;

        int imageNumber;

        Image image;

        public ImageData(string savedFileName,int savedImageNumber, Image SavedImage)
        {
            fileName = savedFileName;

            imageNumber = savedImageNumber;

            image = SavedImage;
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        public int ImageNumber
        {
            get
            {
                return imageNumber;
            }
        }

        public Image Image
        {
            get
            {
                return image;
            }
        }
    }
}
