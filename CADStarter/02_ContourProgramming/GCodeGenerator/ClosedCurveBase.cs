using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CADEngine.DrawingObject;

namespace ContourProgramming {
    public class ClosedCurveBase {
        List<CDrawingObjectBase> m_List = new List<CDrawingObjectBase>();

        public ClosedCurveBase() {
        }
        public void AddDrawingObject(CDrawingObjectBase obj) {
            m_List.Add(obj);
        }
        public void GenerateGCode() {
            for (int i = 0; i < m_List.Count; ++i) {


            }
        }

    }

}
