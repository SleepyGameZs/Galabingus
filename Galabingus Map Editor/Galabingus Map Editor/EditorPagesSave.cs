using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galabingus_Map_Editor
{
    internal struct EditorPagesSave
    {
        List<Image> editorImages;

        int pageNum;
        EditorPagesSave(int currentPageNum, bool pageState, List<Image> EditorPage)
        {
            editorImages = EditorPage;
        }
    }
}
