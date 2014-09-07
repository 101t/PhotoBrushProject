using System;
using System.Collections.Generic;
using System.Text;

namespace Painting.Shapes
{
    public class GDIGenerationHelper
    {
        Dictionary<short, ColorVarPlaceHolder> penColorVarName = new Dictionary<short, ColorVarPlaceHolder>(8);
        Dictionary<short, ColorVarPlaceHolder> brushColorVarNames = new Dictionary<short, ColorVarPlaceHolder>(8);
        
        private bool blnHasGradientBrush;
        public bool HasGradientBrush
        {
            get {return blnHasGradientBrush;}
        }

        public ColorVarPlaceHolder GetPenColorVarName(short shape_zOrder)
        {
            if (penColorVarName.ContainsKey(shape_zOrder))
                return penColorVarName[shape_zOrder];
            else
                throw new Exception("Could not find the specified pen reference!");
        }

        public void AddPenColorVarReference(short shape_zOrder, string[] arrPenColorName)
        {
            penColorVarName.Add(shape_zOrder, new ColorVarPlaceHolder(arrPenColorName));
        }


        public ColorVarPlaceHolder GetBrushColorVarNames(short shape_zOrder)
        {
            if (brushColorVarNames.ContainsKey(shape_zOrder))
                return brushColorVarNames[shape_zOrder];
            else
                throw new Exception("Could not find the specified brush reference!");
        }

        public void AddBrushColorVarReference(short shape_zOrder, string[] arrBrushColorNames)
        {
            brushColorVarNames.Add(shape_zOrder, new ColorVarPlaceHolder(arrBrushColorNames));
            if (arrBrushColorNames.Length > 1) blnHasGradientBrush = true;
        }

        //public string GeneratePaintToolInit(Shape shape)
        //{
        //    string str = "";
        //    ColorVarPlaceHolder holder;
        //    if (shape.painter.PaintBorder)
        //        str += "Pen pen" + shape.Name +
        //            " = new Pen(" + GetPenColorVarName(shape.zOrder).
        //            GetColorVarName(0) + ");\r\n";
        //    if (shape.painter.PaintFill)
        //    {
        //        holder = GetBrushColorVarNames(shape.zOrder);
        //        if (shape.painter.ColorCount == 1)
        //        {  //paint with a solid brush
        //            str += "SolidBrush brush" + shape.Name +
        //                " = new SolidBrush(" + holder.GetColorVarName(0) + ");\r\n";
        //        }
        //        else
        //        { //Paint with a linear gradient brush
        //            str += "LinearGradientBrush brush" + shape.Name +
        //                " = new LinearGradientBrush(new Rectangle(" +
        //                shape.Location.X + "," + shape.Location.Y + "," +
        //                shape.Size.Width + "," + shape.Size.Height + ")," +
        //                holder.GetColorVarName(0) + "," + holder.GetColorVarName(1) + ")," +
        //                shape.painter.LinearModeString + ");\r\n" +
        //             "brush" + shape.Name + ".Blend = GetBlend(" + shape.painter.PercentCoverage +
        //             "," + shape.painter.BlendSmoothness + ");\r\n";
        //        }
        //    }
        //    return str;
        //}

        //public string GeneratePaintToolCleanup(Shape shape)
        //{
        //    string str = "//Cleanup paint tools\r\n";
        //    if (shape.painter.PaintFill)
        //        str += "brush" + shape.Name + ".Dispose();\r\n";
        //    if (shape.painter.PaintBorder)
        //        str += "pen" + shape.Name + ".Dispose();\r\n";

        //    return str;
        //}

        //public string GenerateBlendMethod()
        //{
        //    string str = "//Gets the blend for the LinearGradient brush\r\n" +
        //                 "//Byte values from 0 to 100 are passed in,\r\n" +
        //                 "//representing the BlendSmoothing and gradient\r\n" +
        //                 "//CoverageArea.\r\n" +

        //        "protected Blend GetBlend(byte smoothing, byte coverage)\r\n" +
        //        "{\r\nBlend blend = new Blend(9);\r\nshort i = 0; //loop var\r\n" +
        //        "blend.Positions[0] = 0.0f;\r\nfor (i = 1; i < 9; i++)\r\n" +
        //        "blend.Positions[i] = (float)(blend.Positions[i-1] + 0.125f);\r\n\r\n" +
        //        "byte intHalf = Convert.ToByte(((float)(coverage+1)/100)*8);\r\n" +
        //        "float increment = (((float)(smoothness)/100) / (8 - intHalf));\r\n" +
        //        "blend.Factors[intHalf] = ((float)(100 - smoothness) / 100);\r\n\r\n" +
        //        "if (smoothness > 0 && intHalf != 0)\r\n{\r\nshort startingFactor;\r\n" +
        //        "if (smoothness == 100)\r\nstartingFactor = 0;\r\nelse\r\n" +
        //        "startingFactor = Convert.ToInt16(((1-smoothness/100.0f))*intHalf);\r\n" +
        //        "float smoothIncrement = (blend.Factors[intHalf] / (intHalf - startingFactor+1));\r\n\r\n" +
        //        "blend.Factors[startingFactor] = smoothIncrement;\r\n" +
        //        "for (i = (short)(startingFactor+1); i < intHalf; i++)\r\n" +
        //        "blend.Factors[i] = (float)(blend.Factors[i - 1] + smoothIncrement);\r\n" +
        //        "\r\nfor (i = (short)(intHalf + 1); i < 9; i++)\r\n" +
        //        "blend.Factors[i] = (float)(blend.Factors[i - 1] + increment);\r\n}\r\n";
        //    return str;
        //}
    }

    public class ColorVarPlaceHolder
    {
        string[] arrColorVars;
        public ColorVarPlaceHolder(short numOfColorVars)
        {
            arrColorVars = new string[numOfColorVars];
        }
        public ColorVarPlaceHolder(string[] colorVars)
        {
            arrColorVars = colorVars;
        }


        public short ColorCount
        {
            get {return (short)arrColorVars.Length;}
        }

        public string GetColorVarName(int index)
        {
            return arrColorVars[index];
        }

    }
}
