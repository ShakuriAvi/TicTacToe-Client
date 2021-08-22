using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Tic_Tac_Toe_Backend
{
    class GFX
    {
        private Graphics gObject;
        public GFX(Graphics g)
        {
            gObject = g;
            setUpCanvas();
        }
        public void setUpCanvas()
        {
            Brush bg = new SolidBrush(Color.WhiteSmoke);
            Pen lines = new Pen(Color.Black);

            gObject.FillRectangle(bg, new Rectangle(0, 0, 500, 600));

            gObject.DrawLine(lines,new Point(100,0),new Point(100,600));
            gObject.DrawLine(lines, new Point(200, 0), new Point(200, 600));
            gObject.DrawLine(lines, new Point(300, 0), new Point(300, 600));
            gObject.DrawLine(lines, new Point(400, 0), new Point(400, 600));


            gObject.DrawLine(lines, new Point(0, 100), new Point(500, 100));
            gObject.DrawLine(lines, new Point(0, 200), new Point(500, 200));
            gObject.DrawLine(lines, new Point(0, 300), new Point(500, 300));
            gObject.DrawLine(lines, new Point(0, 400), new Point(500, 400));
        }
    }
}
