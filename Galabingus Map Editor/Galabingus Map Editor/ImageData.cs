using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus_Map_Editor
{
    //Justin Tong
    //4/30/2023
    //the ImageData class that hold all the needed info for the images that will be used in the editor with each image getting an assigned index
    internal struct ImageData
    {
        string fileName;

        int imageNumber;

        Image image;

        /// <summary>
        /// The ImageData contructor that saves the name of the image, the assigned ID number and the image itself
        /// </summary>
        /// <param name="savedFileName">The name of the file a basic desription is enough</param>
        /// <param name="savedImageNumber">The ID number of for the image that is used in saving and loading</param>
        /// <param name="SavedImage">The image that is actually saved</param>
        public ImageData(string savedFileName,int savedImageNumber, Image SavedImage)
        {
            fileName = savedFileName;

            imageNumber = savedImageNumber;

            image = SavedImage;
        }

        /// <summary>
        ///Returns the name of the image
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }
        }

        /// <summary>
        /// Returns the ID number of the image
        /// </summary>
        public int ImageNumber
        {
            get
            {
                return imageNumber;
            }
        }

        /// <summary>
        /// Returns the saved image
        /// </summary>
        public Image Image
        {
            get
            {
                return image;
            }
        }
    }
}
