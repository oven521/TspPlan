#region netDxf, Copyright(C) 2014 Daniel Carvajal, Licensed under LGPL.

//                        netDxf library
// Copyright (C) 2014 Daniel Carvajal (haplokuon@gmail.com)
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
// FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
// IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using netDxf;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Header;
using netDxf.Objects;
using netDxf.Tables;
using Group = netDxf.Objects.Group;
using Point = netDxf.Entities.Point;
using Attribute = netDxf.Entities.Attribute;
using Image = netDxf.Entities.Image;


namespace TestDxfDocument
{
    /// <summary>
    /// This is just a simple test of work in progress for the netDxf Library.
    /// </summary>
    public class Program
    {

        private static void Main()
        {
            #region Samples for fixes, new and modified features 0.9.0

            //MakingGroups();
            //CombiningTwoDrawings();
            //BinaryChunkXData();
            //BinaryDxfFiles();
            //MeshEntity();

            #endregion

            #region Samples for new and modified features 0.8.0

            //MTextEntity();
            //TransparencySample();
            //DocumentUnits();
            //PaperSpace();
            //BlockWithAttributes();

            #endregion

            Test();

            //DimensionNestedBlock();
            //EncodingTest();
            //CheckReferences();
            //ComplexHatch();
            //RayAndXLine();
            //UserCoordinateSystems();
            //ExplodeInsert();
            //ImageUsesAndRemove();
            //LayerAndLineTypesUsesAndRemove();
            //TextAndDimensionStyleUsesAndRemove();
            //MLineStyleUsesAndRemove();
            //AppRegUsesAndRemove();
            //ExplodePolyfaceMesh(); 
            //ApplicationRegistries();
            //TestOCStoWCS();
            //WriteGradientPattern();
            //WriteGroup();
            //WriteMLine();
            //ObjectVisibility();
            //EntityTrueColor();
            //EntityLineWeight();
            //ReadWriteFromStream();
            //Text();
            //WriteNoAsciiText();
            //WriteSplineBoundaryHatch();
            //WriteNoInsertBlock();
            //WriteImage();
            //SplineDrawing();
            //AddAndRemove();
            //LoadAndSave();
            //CleanDrawing();
            //OrdinateDimensionDrawing();
            //Angular2LineDimensionDrawing();
            //Angular3PointDimensionDrawing();
            //DiametricDimensionDrawing();
            //RadialDimensionDrawing();
            //LinearDimensionDrawing();
            //AlignedDimensionDrawing();
            //WriteMText();
            //LineWidth();
            //HatchCircleBoundary();
            //ToPolyline();
            //FilesTest();
            //CustomHatchPattern();
            //LoadSaveHatchTest();
            //WriteDxfFile();
            //ReadDxfFile();
            //ExplodeTest();
            //HatchTestLinesBoundary();
            //HatchTest1();
            //HatchTest2();
            //HatchTest3();
            //HatchTest4();
            //WriteNestedInsert();
            //WritePolyfaceMesh();
            //Ellipse();
            //Solid();
            //Face3d();
            //LwPolyline();
            //Polyline();
            //Dxf2000();
            //SpeedTest();
            //WritePolyline3d();
            //WriteInsert();
        }

        #region Samples for new and modified features 0.9.0

        private static void MakingGroups()
        {
            Line line1 = new Line(Vector2.Zero, Vector2.UnitX);
            Line line2 = new Line(Vector2.Zero, Vector2.UnitY);
            Group group = new Group();
            group.Entities.Add(line1);
            group.Entities.Add(line2);

            DxfDocument dxf = new DxfDocument();
            // when we add a group to the document all the entities contained in the group will be automatically added to the document
            dxf.Groups.Add(group);

            // adding the group entities to the document is not necessary, but doing so should not cause any harm
            // the AddEntity method will return false in those cases, since those entities are already in the document
            Console.WriteLine(dxf.AddEntity(line1));
            Console.WriteLine(dxf.AddEntity(line2));

            dxf.Save("group.dxf");

            DxfDocument load = DxfDocument.Load("group.dxf");

            Console.WriteLine("Press a key to finish...");
            Console.ReadKey();
        }
        private static void CombiningTwoDrawings()
        {
            // create first drawing
            Line line1 = new Line(Vector2.Zero, Vector2.UnitX);
            line1.Layer = new Layer("Layer01");
            line1.Layer.Color = AciColor.Blue;
            DxfDocument dxf1 = new DxfDocument();
            dxf1.AddEntity(line1);
            dxf1.Save("drawing01.dxf");

            // create second drawing
            Line line2 = new Line(Vector2.Zero, Vector2.UnitY);
            line2.Layer = new Layer("Layer02");
            line2.Layer.Color = AciColor.Red;
            DxfDocument dxf2 = new DxfDocument();
            dxf2.AddEntity(line2);
            dxf2.Save("drawing02.dxf");

            // load the drawings that will be combined
            DxfDocument source01 = DxfDocument.Load("drawing01.dxf");
            DxfDocument source02 = DxfDocument.Load("drawing02.dxf");

            // our destination drawing
            DxfDocument combined = new DxfDocument();
            foreach (Line l in source01.Lines)
            {
                // It is recommended to make a copy of the source line before we can added to the destination drawing
                // if we do not make a copy weird things might happen if we save the original drawing again
                Line copy = (Line)l.Clone();
                combined.AddEntity(copy);
            }

            // Another safe way is removing the entity from the original drawing before adding it to the destination drawing
            Line line = source02.Lines[0];
            source02.RemoveEntity(line);
            combined.AddEntity(line);

            combined.Save("CombinedDrawing.dxf");
        }
        private static void BinaryChunkXData()
        {
            Line line = new Line(Vector2.Zero, Vector2.UnitX);

            ApplicationRegistry appId = new ApplicationRegistry("TestBinaryChunk");

            // the extended data binary data (code 1004) is stored in a different way depending if the dxf file is text or binary.
            // in text based files as a string of hexadecimal digits, two per binary byte,
            // while in binary files the data is stored in chunks of 127 bytes, preceding a byte that defines the number of bytes in the chunk
            byte[] data = new byte[325];

            // fill up the array with some random data
            Random rnd = new Random();
            rnd.NextBytes(data);

            XData xdata = new XData(appId);

            // the XDataRecord will store the binary data as a byte array and not as a string as it use to be
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.BinaryData, data));
            line.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                             };

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);

            dxf.Save("BinaryChunkXData.dxf");
            dxf.Save("BinaryChunkXData binary.dxf", true);

            // some testing
            DxfDocument test = DxfDocument.Load("BinaryChunkXData binary.dxf");
            Line lineTest = test.Lines[0];
            XDataRecord recordTest = lineTest.XData[appId.Name].XDataRecord[0];
            Debug.Assert(recordTest.Code == XDataCode.BinaryData);
            byte[] dataText = (byte[]) recordTest.Value;

            byte[] compare = new byte[127];
            Array.Copy(data, compare, 127);

            for (int i = 0; i < 127; i++)
            {
                Console.WriteLine(dataText[i] == compare[i]);
            }

            // this is the string as it is saved in text based dxf files
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dataText.Length; i++)
            {
                sb.Append(String.Format("{0:X2}", data[i]));
            }
            Console.WriteLine(sb.ToString());

            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
        }
        private static void BinaryDxfFiles()
        {
            // Binary dxf files preserve the accuracy of the drawing while text dxf files are saved with 17 decimals.
            // Binary dxf files are 4-5 times faster to write, while reading is just a little faster.
            // Binary dxf files take about 20% less file space.
            DxfDocument dxf = new DxfDocument();
            // optionally you can give a name to de document
            dxf.Name = "Binary dxf";
            Line line = new Line(Vector3.Zero, new Vector3(10));
            dxf.AddEntity(line);


            // To save a document as a binary dxf just set the isBinary parameter to true, by default it will always be saved as a text based dxf 
            // you can use the document name as tha file name, or just give another one.
            string file = dxf.Name + ".dxf";

            // Handling with error checking of the saving process
            bool ok = dxf.Save(file, true);
            if(ok)
                Console.WriteLine("The file \"{0}\" has been correctly saved.", file);
            else
                Console.WriteLine("Fatal error while saving \"{0}\".", file);

            Console.WriteLine();

            // Handling with error checking of the loading process

            // check if the file exists
            if (!File.Exists(file))
            {
                Console.WriteLine("The file \"{0}\" does not exists.", file);
            }
            else
            {
                bool isBinary;
                DxfVersion version = DxfDocument.CheckDxfFileVersion(file, out isBinary);

                // netDxf only supports AutoCad2000 and above.
                if (version >= DxfVersion.AutoCad2000)
                {
                    // To load a binary dxf nothing needs to be done, the reader will detect the correct type.
                    DxfDocument load = DxfDocument.Load(file);
                    if (load == null)
                        Console.WriteLine("Fatal error while loading \"{0}\".", file);
                    else
                        //when a document is loaded the file name without extension is used as the document name
                        Console.WriteLine("The file \"{0}\" version {1} has been correctly loaded.\n\tBinary? {2}", file, version, isBinary);
                }
                else
                {
                    if(version == DxfVersion.Unknown)
                        Console.WriteLine("The file \"{0}\" is not a dxf.", file);
                    else
                        Console.WriteLine("Dxf file \"{0}\" version {1} is not supported, only AutoCad2000 and above.", file, version);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();

        }
        private static void MeshEntity()
        {
            DxfDocument dxf = new DxfDocument();

            // construct a simple cube (see the AutoCad documentation for more information about creating meshes)

            // the mesh data is always defined at level 0 (no subdivision)
            // 8 vertices
            List<Vector3> vertexes = new List<Vector3>
                                            {
                                                new Vector3(-5, -5, -5),
                                                new Vector3(5, -5, -5),
                                                new Vector3(5, 5, -5),
                                                new Vector3(-5, 5, -5),
                                                new Vector3(-5, -5, 5),
                                                new Vector3(5, -5, 5),
                                                new Vector3(5, 5, 5),
                                                new Vector3(-5, 5, 5)
                                            };

            //6 faces
            List<int[]> faces = new List<int[]>
                                               {
                                                   new[] {0, 3, 2, 1},
                                                   new[] {0, 1, 5, 4},
                                                   new[] {1, 2, 6, 5},
                                                   new[] {2, 3, 7, 6},
                                                   new[] {0, 4, 7, 3},
                                                   new[] {4, 5, 6, 7}
                                               };

            

            // the list of edges is optional and only really needed when applying creases values to them
            // crease negative values will sharpen the edge independently of the subdivision level. Any negative crease value will be reseted as -1.
            // by default edge creases are set to 0.0 (no edge sharpening)
            List<MeshEdge> edges = new List<MeshEdge>
            {
                new MeshEdge(0, 1),
                new MeshEdge(1, 2),
                new MeshEdge(2, 3),
                new MeshEdge(3, 0),
                new MeshEdge(4, 5, -1.0),
                new MeshEdge(5, 6, -1.0),
                new MeshEdge(6, 7, -1.0),
                new MeshEdge(7, 4, -1.0),
                new MeshEdge(0, 4),
                new MeshEdge(1, 5),
                new MeshEdge(2, 6),
                new MeshEdge(3, 7)

            };
            Mesh mesh = new Mesh(vertexes, faces, edges);
            mesh.SubdivisionLevel = 3;
            dxf.AddEntity(mesh);

            dxf.Save("mesh.dxf");
        }

        #endregion

        #region Samples for new and modified features 0.8.0

        private static void MTextEntity()
        {
            TextStyle style = new TextStyle("Arial");

            MText text1 = new MText(Vector2.Zero, 10, 0, style);
            // you can set manually the text value with all available formatting commands
            text1.Value = "{\\C71;\\c10938556;Text} with true color\\P{\\C140;Text} with indexed color";

            MText text2 = new MText(new Vector2(0, 30), 10, 0, style);
            // or use the Write() method
            MTextFormattingOptions op = new MTextFormattingOptions(text2.Style);
            op.Color = new AciColor(188, 232, 166); // using true color
            text2.Write("Text", op);
            op.Color = null; // set color to the default defined in text2.Style
            text2.Write(" with true color");
            text2.EndParagraph();
            op.Color = new AciColor(140); // using index color
            text2.Write("Text", op); // set color to the default defined in text2.Style
            op.Color = null;
            text2.Write(" with indexed color");

            // both text1 and text2 should yield to the same result
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);
            dxf.AddEntity(text1);
            dxf.AddEntity(text2);

            dxf.Save("MText format.dxf");

            // now you can retrieve the MText text value without any formatting codes, control characters like tab '\t' will be preserved in the result,
            // the new paragraph command "\P" will be converted to new line feed '\r\n'.
            Console.WriteLine(text1.PlainText());
            Console.WriteLine();
            Console.WriteLine(text2.PlainText());
            Console.WriteLine();
            Console.WriteLine("Press a key to finish...");
            Console.ReadKey();

        }
        private static void TransparencySample()
        {
            // transparencies can only be applied to entities and layer
            Layer layer = new Layer("Layer with transparency");
            layer.Color = new AciColor(Color.MediumVioletRed);
            // the transparency is expresed in percentage. Initially all Transparency values are initialized as ByLayer.
            layer.Transparency.Value = 50;
            // You cannot use the reserved values 0 and 100 that represents ByLayer and ByBlock. Use Transparency.ByLayer and Transparency.ByBlock
            // this behaviour is similar to the index in AciColor or the weight in Lineweight
            // this is wrong and will rise and exception
            //layer.Transparency.Value = 0;
            // this is ok
            //layer.Transparency = Transparency.ByLayer;

            // this line will use the transparency defined in the layer to which it belongs
            Line line1 = new Line(new Vector2(-5, -5), new Vector2(5, 5));
            line1.Layer = layer;

            // this line will use its own transparency
            Line line2 = new Line(new Vector2(-5, 5), new Vector2(5, -5));
            line2.Transparency.Value = 80;

            // transparency as the true color is not supported by AutoCad2000 database version
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2004);
            dxf.AddEntity(line1);
            dxf.AddEntity(line2);

            dxf.Save("TransparencySample.dxf");

            dxf = DxfDocument.Load("TransparencySample.dxf");

        }
        private static void DocumentUnits()
        {
            DxfDocument dxf = new DxfDocument();

            // setting the LUnit variable to engineering or architectural will also set the InsUnits variable to Inches,
            // this need to be this way since AutoCad will show those units in feet and inches and will always consider the drawing base units as inches.
            // You can change again the InsUnits it at your own risk.
            // its main purpose is at the user interface level
            //dxf.DrawingVariables.LUnits = LinearUnitType.Engineering;

            // this is the recommended document unit type
            dxf.DrawingVariables.LUnits = LinearUnitType.Decimal;

            // this is the real important unit,
            // it is used when inserting blocks or images into the drawing as this and the block units will give the scale of the resulting Insert
            dxf.DrawingVariables.InsUnits = DrawingUnits.Millimeters;

            // the angle unit type is purely cosmetic as it has no influence on how the angles are stored in the dxf 
            // its purpose is only at the user interface level
            dxf.DrawingVariables.AUnits = AngleUnitType.Radians;

            // even though we have set the drawing angles in radians the dxf always stores angle data in degrees,
            // this arc goes from 45 to 270 degrees and not radians or whatever the AUnits header variable says.
            Arc arc = new Arc(Vector2.Zero, 5, 45, 270);
            // Remember, at the moment, once the entity has been added to the document is not safe to modify it, changes in some of their properties might generate problems
            dxf.AddEntity(arc);

            // the units of this line will correspond to the ones set in InsUnits
            Line lineM = new Line(new Vector2(-5, -5), new Vector2(5, 5));
            dxf.AddEntity(lineM);

            // All entities added to a block are expressed in the coordinates defined by the block
            // You can set a default unit so all new blocks will use them, the default value is Unitless
            // You might want to use the same units as the drawing, this is just a convenient way to make sure all blocks share the same units 
            BlockRecord.DefaultUnits = dxf.DrawingVariables.InsUnits;

            // In this case the line will be 10 cm long
            Line lineCm = new Line(new Vector2(-5, 0), new Vector2(5, 0));
            Block blockCm = new Block("CmBlock");
            // You can override the default units changing the block.Record.Units value
            blockCm.Record.Units = DrawingUnits.Centimeters;
            blockCm.Entities.Add(lineCm);
            Insert insCm = new Insert(blockCm);

            // In this case the line will be 10 dm long
            Line lineDm = new Line(new Vector2(0, 5), new Vector2(0, -5));
            Block blockDm = new Block("DmBlock");
            blockDm.Record.Units = DrawingUnits.Decimeters;
            // AllowExploding and ScaleUniformy properties will only be recognized by dxf version AutoCad2007 and upwards
            blockDm.Record.AllowExploding = false;
            blockDm.Record.ScaleUniformly = true;
            blockDm.Entities.Add(lineDm);
            blockDm.Entities.Add(insCm);
            Insert insDm = new Insert(blockDm);

            dxf.AddEntity(insDm);

            // the image units are stored in the raster variables units, it is recommended to use the same units as the document to avoid confusions
            dxf.RasterVariables.Units = ImageUnits.Millimeters;
            // Sometimes AutoCad does not like image file relative paths, in any case reloading the references will fix the problem
            ImageDef imgDef = new ImageDef("image.jpg");
            // the resolution units is only used to calculate the image resolution that will return pixels per inch or per centimeter (the use of NoUnits is not recommended).
            imgDef.ResolutionUnits = ImageResolutionUnits.Inches;
            // this image will be 10x10 mm in size
            Image img = new Image(imgDef, Vector3.Zero, 10, 10);
            dxf.AddEntity(img);

            dxf.Save("Document Units.dxf");

            DxfDocument dxfLoad = DxfDocument.Load("Document Units.dxf");

        }
        private static void PaperSpace()
        {
            // Sample on how to work with Layouts
            DxfDocument dxf = new DxfDocument();
            // A new DxfDocument will create the default "Model" layout that is associated with the ModelSpace block. This layout cannot be erased or renamed.
            Line line = new Line(new Vector2(0), new Vector2(100));
            // The line will be added to the "Model" layout since this is the active one by default.
            dxf.AddEntity(line);

            // Create a new Layout, all new layouts will be associated with different PaperSpace blocks,
            // while there can be only one ModelSpace multiple PaperSpace blocks might exist in the document
            Layout layout1 = new Layout("Layout1");

            // When the layout is added to the list, a new PaperSpace block will be created automatically
            dxf.Layouts.Add(layout1);
            // Set this new Layout as the active one. All entities will now be added to this layout.
            dxf.ActiveLayout = layout1.Name;

            // Create a viewport, this is the window to the ModelSpace
            Viewport viewport1 = new Viewport
                {
                    Width = 100,
                    Height = 100,
                    Center = new Vector3(50, 50, 0),
                };

            // Add it to the "Layout1" since this is the active one
            dxf.AddEntity(viewport1);
            // Also add a circle
            Circle circle = new Circle(new Vector2(150), 25);
            dxf.AddEntity(circle);

            // Create a second Layout, add it to the list, and set it as the active one.
            Layout layout2 = new Layout("Layout2");
            dxf.Layouts.Add(layout2);
            dxf.ActiveLayout = layout2.Name;

            // Viewports might have a non rectangular boundary, in this case we will use an ellipse.
            Ellipse ellipse = new Ellipse(new Vector2(100), 200, 150);
            Viewport viewport2 = new Viewport
            {
                ClippingBoundary = ellipse,
            };

            // Add the viewport to the document. This will also add the ellipse to the document.
            dxf.AddEntity(viewport2);
     
            // Save the document as always.
            dxf.Save("PaperSpace.dxf");

#region CAUTION - This is subject to change in the future, use it with care

            // You cannot directly remove the ellipse from the document since it has been attached to a viewport
            bool ok = dxf.RemoveEntity(ellipse); // ok = false

            // If an entity has been attached to another, its reactor will point to its owner
            // This information is subject to change in the future to become a list, an entity can be attached to multiple objects;
            // but at the moment only the viewport clipping boundary make use of this.
            // This is the way AutoCad also handles hatch and dimension associativity, that I might implement in the future
            DxfObject reactor = ellipse.Reactor; // in this case reactor points to viewport2

            // You need to delete the viewport instead. This deletes the viewport and the ellipse
            //dxf.RemoveEntity(viewport2);

            // another way of deleting the ellipse, is first to assign another clipping boundary to the viewport or just set it to null
            viewport2.ClippingBoundary = null;
            // now it will be possible to delete the ellipse. This will not delete the viewport.
            ok = dxf.RemoveEntity(ellipse); // ok = true

            // Save the document if you want to test the changes
            dxf.Save("PaperSpace.dxf");

#endregion

            DxfDocument dxfLoad = DxfDocument.Load("PaperSpace.dxf");

            // For every entity you can check its layout
            // The entity Owner will return the block to which it belongs, it can be a *Model_Space, *Paper_Space, ... or a common block if the entity is part of its geometry.
            // The block record stores information about the block and one of them is the layout, this mimics the way the dxf stores this information.
            // Remember only the internal blocks *Model_Space, *Paper_Space, *Paper_Space0, *Paper_Space1, ... have an associated layout,
            // all other blocks will return null is asked for block.Record.Layout
            Layout associatedLayout = dxfLoad.Lines[0].Owner.Record.Layout;

            // or you can get the complete list of entities of a layout
            foreach (Layout layout in dxfLoad.Layouts)
            {
                List<DxfObject> entities = dxfLoad.Layouts.GetReferences(layout.Name); 
            }

            // You can also remove any layout from the list, except the "Model".
            // Remember all entities that has been added to this layout will also be removed.
            // This mimics the behaviour in AutoCad, when a layout is deleted all entities in it will also be deleted.
            dxf.Layouts.Remove(layout2);

            dxf.Save("PaperSpace removed.dxf");

        }
        private static void BlockWithAttributes()
        {
            DxfDocument dxf = new DxfDocument();
            Block block = new Block("BlockWithAttributes");
            block.Layer = new Layer("BlockSample");
            // It is possible to change the block position, even though it is recommended to keep it at Vector3.Zero,
            // since the block geometry is expressed in local coordinates of the block.
            // The block position defines the base point when inserting an Insert entity.
            block.Position = new Vector3(10, 5, 0);

            // create an attribute definition, the attdef tag must be unique as it is the way to identify the attribute.
            // even thought AutoCad allows multiple attribute definition in block definitions, it is not recommended
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            // this is the text prompt shown to introduce the attribute value when a new Insert entity is inserted into the drawing
            attdef.Text = "InfoText";
            // optionally we can set a default value for new Insert entities
            attdef.Value = 0;
            // the attribute definition position is in local coordinates to the Insert entity to which it belongs
            attdef.Position = new Vector3(1, 1, 0);

            // modifying directly the text style might not get the desired results. Create one or get one from the text style table, modify it and assign it to the attribute text style.
            // one thing to note, if there is already a text style with the assigned name, the existing one in the text style table will override the new one.
            //attdef.Style.IsVertical = true;

            TextStyle txt = new TextStyle("MyStyle", "Arial.ttf");
            txt.IsVertical = true;
            attdef.Style = txt;
            attdef.WidthFactor = 2;
            // not all alignment options are avaliable for ttf fonts 
            attdef.Alignment = TextAlignment.MiddleCenter;
            attdef.Rotation = 90;

            // remember, netDxf does not allow adding attribute definitions with the same tag, even thought AutoCad allows this behaviour, it is not recommended in anyway.
            // internally attributes and their associated attribute definitions are handled through dictionaries,
            // and the tags work as ids to easily identify the information stored in the attributte value.
            // When reading a file the attributes or attribute definitions with duplicate tags will be automatically removed.
            // This is subject to change on public demand, it is possible to reimplement this behaviour with simple collections to allow for duplicate tags.
            block.AttributeDefinitions.Add(attdef);

            // The entities list defines the actual geometry of the block, they are expressed in th block local coordinates
            Line line1 = new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0));
            Line line2 = new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0));
            block.Entities.Add(line1);
            block.Entities.Add(line2);

            // You can check the entity ownership with:
            Block line1Owner = line1.Owner;
            Block line2Owner = line2.Owner;
            // in this example line1Oner = line2Owner = block
            // As explained in the PaperSpace() sample, the layout associated with a common block will always be null
            Layout associatedLayout = line1.Owner.Record.Layout;
            // associatedLayout = null

            // create an Insert entity with the block definition, during the initialization the Insert attributes list will be created with the default attdef properties
            Insert insert1 = new Insert(block)
            {
                Position = new Vector3(5, 5, 5),
                Normal = new Vector3(1, 1, 1),
                Rotation = 45
            };

            // When the insert position, rotation, normal and/or scale are modified we need to transform the attributes.
            // It is not recommended to manually change the attribute position and orientation and let the Insert entity handle the transformations to mantain them in the same local position.
            // The attribute position and orientation are stored in WCS (world coordinate system) even if the documentation says they are in OCS (object coordinate system). The documentation is WRONG!.
            // In this particular case we have changed the position, normal and rotation.
            insert1.TransformAttributes();
            
            // Once the insert has been created we can modify the attributes properties, the list cannot be modified only the items stored in it
            insert1.Attributes[attdef.Tag].Value = 24;

            // Modifying directly the layer might not get the desired results. Create one or get one from the layers table, modify it and assign it to the insert
            // One thing to note, if there is already a layer with the same name, the existing one in the layers table will override the new one, when the entity is added to the document.
            Layer layer = new Layer("MyInsertLayer");
            layer.Color.Index = 4;

            // optionally we can add the new layer to the document, if not the new layer will be added to the Layers collection when the insert entity is added to the document
            // in case a new layer is found in the list the add method will return the layer already stored in the list
            // this behaviour is similar for all TableObject elements, all table object names must be unique (case insensitive)
            layer = dxf.Layers.Add(layer);

            // assign the new layer to the insert
            insert1.Layer = layer;

            // add the entity to the document
            dxf.AddEntity(insert1);

            // create a second insert entity
            // the constructor will automatically reposition the insert2 attributes to the insert local position
            Insert insert2 = new Insert(block, new Vector3(10, 5, 0));

            // as before now we can change the insert2 attribute value
            insert2.Attributes[attdef.Tag].Value = 34;

            // additionally we can insert extended data information
            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata1.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata1.XDataRecord.Add(XDataRecord.CloseControlString);

            insert2.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata1.ApplicationRegistry.Name, xdata1},
                             };
            dxf.AddEntity(insert2);

            // all entities support this feature
            XData xdata2 = new XData(new ApplicationRegistry("MyApplication1"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata2.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata2.XDataRecord.Add(XDataRecord.CloseControlString);

            // multiple extended data entries might be added
            XData xdata3 = new XData(new ApplicationRegistry("MyApplication2"));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata3.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata3.XDataRecord.Add(XDataRecord.CloseControlString);

            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("MyCircleLayer");
            // AutoCad 2000 does not support true colors, in that case an approximated color index will be used instead
            circle.Layer.Color = new AciColor(Color.MediumSlateBlue);
            circle.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata2.ApplicationRegistry.Name, xdata2},
                                 {xdata3.ApplicationRegistry.Name, xdata3},
                             };
            dxf.AddEntity(circle);

            dxf.Save("BlockWithAttributes.dxf");
            DxfDocument dxfLoad = DxfDocument.Load("BlockWithAttributes.dxf");
        }

        #endregion

        private static void Test()
        {

            // sample.dxf contains all supported entities by netDxf
            string file = "BG2-001-27 copy.dxf";
            bool isBinary;
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(file, out isBinary);
            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                Console.WriteLine("THE FILE {0} IS NOT A VALID DXF", file);
                Console.WriteLine();

                Console.WriteLine("FILE VERSION: {0}", dxfVersion);
                Console.WriteLine();

                Console.WriteLine("Press a key to continue...");
                Console.ReadLine();

                return;
            }

            DxfDocument dxf = DxfDocument.Load(file);
            Console.WriteLine("FILE NAME: {0}", file);
            Console.WriteLine("\tbinary dxf: {0}", isBinary);
            Console.WriteLine();            
            Console.WriteLine("FILE VERSION: {0}", dxf.DrawingVariables.AcadVer);
            Console.WriteLine();
            Console.WriteLine("FILE COMMENTS: {0}", dxf.Comments.Count);
            foreach (var o in dxf.Comments)
            {
                Console.WriteLine("\t{0}", o);
            }
            Console.WriteLine();
            Console.WriteLine("FILE TIME:");
            Console.WriteLine("\tdrawing created (UTC): {0}.{1}", dxf.DrawingVariables.TduCreate, dxf.DrawingVariables.TduCreate.Millisecond.ToString("000"));
            Console.WriteLine("\tdrawing last update (UTC): {0}.{1}", dxf.DrawingVariables.TduUpdate, dxf.DrawingVariables.TduUpdate.Millisecond.ToString("000"));
            Console.WriteLine("\tdrawing edition time: {0}", dxf.DrawingVariables.TdinDwg);
            Console.WriteLine();    
            Console.WriteLine("APPLICATION REGISTRIES: {0}", dxf.ApplicationRegistries.Count);
            foreach (var o in dxf.ApplicationRegistries)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.ApplicationRegistries.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("LAYERS: {0}", dxf.Layers.Count);
            foreach (var o in dxf.Layers)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Layers.GetReferences(o).Count);
            }
            Console.WriteLine();

            Console.WriteLine("LINE TYPES: {0}", dxf.LineTypes.Count);
            foreach (var o in dxf.LineTypes)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.LineTypes.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("TEXT STYLES: {0}", dxf.TextStyles.Count);
            foreach (var o in dxf.TextStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.TextStyles.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("DIMENSION STYLES: {0}", dxf.DimensionStyles.Count);
            foreach (var o in dxf.DimensionStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.DimensionStyles.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("MLINE STYLES: {0}", dxf.MlineStyles.Count);
            foreach (var o in dxf.MlineStyles)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.MlineStyles.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("UCSs: {0}", dxf.UCSs.Count);
            foreach (var o in dxf.UCSs)
            {
                Console.WriteLine("\t{0}", o.Name);
            }
            Console.WriteLine();

            Console.WriteLine("BLOCKS: {0}", dxf.Blocks.Count);
            foreach (var o in dxf.Blocks)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Blocks.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("LAYOUTS: {0}", dxf.Layouts.Count);
            foreach (var o in dxf.Layouts)
            {
                Console.WriteLine("\t{0}; References count: {1}", o.Name, dxf.Layouts.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("IMAGE DEFINITIONS: {0}", dxf.ImageDefinitions.Count);
            foreach (var o in dxf.ImageDefinitions)
            {
                Console.WriteLine("\t{0}; File name: {1}; References count: {2}", o.Name, o.FileName, dxf.ImageDefinitions.GetReferences(o.Name).Count);
            }
            Console.WriteLine();

            Console.WriteLine("GROUPS: {0}", dxf.Groups.Count);
            foreach (var o in dxf.Groups)
            {
                Console.WriteLine("\t{0}; Entities count: {1}", o.Name, o.Entities.Count);
            }
            Console.WriteLine();

            // the entities lists contain the geometry that has a graphical representation in the drawing across all layouts,
            // to get the entities that belongs to an specific layout you can get the references through the Layouts.GetReferences(name)
            // or check the Entity.Owner.Record.Layout property
            Console.WriteLine("ENTITIES:");
            Console.WriteLine("\t{0}; count: {1}", EntityType.Arc, dxf.Arcs.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Circle, dxf.Circles.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Dimension, dxf.Dimensions.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Ellipse, dxf.Ellipses.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Face3D, dxf.Faces3d.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Hatch, dxf.Hatches.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Image, dxf.Images.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Insert, dxf.Inserts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.LightWeightPolyline, dxf.LwPolylines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Line, dxf.Lines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Mesh, dxf.Meshes.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.MLine, dxf.MLines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.MText, dxf.MTexts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Point, dxf.Points.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.PolyfaceMesh, dxf.PolyfaceMeshes.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Polyline, dxf.Polylines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Solid, dxf.Solids.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Spline, dxf.Splines.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Text, dxf.Texts.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Ray, dxf.Rays.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.Viewport, dxf.Viewports.Count);
            Console.WriteLine("\t{0}; count: {1}", EntityType.XLine, dxf.XLines.Count);
            Console.WriteLine();

            // the dxf version is controlled by the DrawingVariables property of the dxf document,
            // also a HeaderVariables instance or a DxfVersion can be passed to the constructor to initialize a new DxfDocument.
            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2013;
            //dxf.Save("sample 2013.dxf");
            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            //dxf.Save("sample 2010.dxf");
            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            //dxf.Save("sample 2007.dxf");
            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            //dxf.Save("sample 2004.dxf");
            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            //dxf.Save("sample 2000.dxf");

            //// saving to binary dxf
            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2013;
            //dxf.Save("binary test.dxf", true);
            //DxfDocument test = DxfDocument.Load("binary test.dxf");

            Console.WriteLine("Press a key to continue...");
            Console.ReadLine();
        }

        //private static void ExplodeInsert()
        //{
        //    DxfDocument dxf = DxfDocument.Load("explode\\ExplodeInsertUniformScale.dxf");

        //    List<DxfObject> refs = dxf.Blocks.References["ExplodeBlock"];
        //    Insert insert = (Insert)refs[0];
        //    dxf.RemoveEntity(insert);
        //    insert.Layer = new Layer("Original block");
        //    insert.Layer.Color = AciColor.DarkGrey;
        //    dxf.AddEntity(insert);
        //    List<EntityObject> explodedEntities = insert.Explode();
        //    dxf.AddEntity(explodedEntities);

        //    dxf.Save("ExplodeInsert.dxf");
        //}

        private static void EncodingTest()
        {
            DxfDocument dxf;
            dxf = DxfDocument.Load("tests//EncodeDecodeProcess (cad 2010).dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("EncodeDecodeProcess (netDxf 2000).dxf");

            dxf = DxfDocument.Load("tests//EncodeDecodeProcess (cad 2000).dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("EncodeDecodeProcess (netDxf 2010).dxf");
        }
        private static void CheckReferences()
        {
            DxfDocument dxf = new DxfDocument();
            
            Layer layer1 = new Layer("Layer1");
            layer1.Color = AciColor.Blue;
            layer1.LineType = LineType.Center;

            Layer layer2 = new Layer("Layer2");
            layer2.Color = AciColor.Red;

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(0, 0));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(20, 0));
            poly.Vertexes.Add(new LwPolylineVertex(30, 10));
            poly.Layer = layer1;
            dxf.AddEntity(poly);

            Ellipse ellipse = new Ellipse(new Vector3(2, 2, 0), 5, 3);
            ellipse.Rotation = 30;
            ellipse.Layer = layer1;
            dxf.AddEntity(ellipse);

            Line line = new Line(new Vector2(10, 5), new Vector2(-10, -5));
            line.Layer = layer2;
            line.LineType = LineType.DashDot;
            dxf.AddEntity(line);

            dxf.Save("test.dxf");

            foreach (ApplicationRegistry registry in dxf.ApplicationRegistries)
            {
                foreach (DxfObject o in dxf.ApplicationRegistries.GetReferences(registry))
                {
                    if (o is EntityObject)
                    {
                        foreach (KeyValuePair<string, XData> data in ((EntityObject)o).XData)
                        {
                            if (data.Key == registry.Name)
                                if (!ReferenceEquals(registry, data.Value.ApplicationRegistry))
                                    Console.WriteLine("Application registry {0} not equal entity to {1}", registry.Name, o.CodeName);
                        }
                    }
                }
            }

            foreach (Block block in dxf.Blocks)
            {
                foreach (DxfObject o in dxf.Blocks.GetReferences(block))
                {
                    if (o is Insert)
                        if (!ReferenceEquals(block, ((Insert)o).Block))
                            Console.WriteLine("Block {0} not equal entity to {1}", block.Name, o.CodeName);
                }
            }

            foreach (ImageDef def in dxf.ImageDefinitions)
            {
                foreach (DxfObject o in dxf.ImageDefinitions.GetReferences(def))
                {
                    if (o is Image)
                        if (!ReferenceEquals(def, ((Image)o).Definition))
                            Console.WriteLine("Image definition {0} not equal entity to {1}", def.Name, o.CodeName);
                }
            }

            foreach (DimensionStyle dimStyle in dxf.DimensionStyles)
            {
                foreach (DxfObject o in dxf.DimensionStyles.GetReferences(dimStyle))
                {
                    if (o is Dimension)
                        if (!ReferenceEquals(dimStyle, ((Dimension)o).Style))
                            Console.WriteLine("Dimension style {0} not equal entity to {1}", dimStyle.Name, o.CodeName);
                }

            }

            foreach (Group g in dxf.Groups)
            {
                foreach (DxfObject o in dxf.Groups.GetReferences(g))
                {
                    // no references
                }
            }

            foreach (UCS u in dxf.UCSs)
            {
                foreach (DxfObject o in dxf.UCSs.GetReferences(u))
                {
                    
                }
            }

            foreach (TextStyle style in dxf.TextStyles)
            {
                foreach (DxfObject o in dxf.TextStyles.GetReferences(style))
                {
                    if (o is Text)
                        if (!ReferenceEquals(style, ((Text)o).Style))
                            Console.WriteLine("Text style {0} not equal entity to {1}", style.Name, o.CodeName);

                    if (o is MText)
                        if (!ReferenceEquals(style, ((MText)o).Style))
                            Console.WriteLine("Text style {0} not equal entity to {1}", style.Name, o.CodeName);

                    if (o is DimensionStyle)
                        if (!ReferenceEquals(style, ((DimensionStyle)o).TextStyle))
                            Console.WriteLine("Text style {0} not equal entity to {1}", style.Name, o.CodeName);
                }
            }

            foreach (Layer layer in dxf.Layers)
            {
                foreach (DxfObject o in dxf.Layers.GetReferences(layer))
                {
                    if (o is Block)
                        if (!ReferenceEquals(layer, ((Block)o).Layer))
                            Console.WriteLine("Layer {0} not equal entity to {1}", layer.Name, o.CodeName);
                    if (o is EntityObject)
                        if (!ReferenceEquals(layer, ((EntityObject)o).Layer))
                            Console.WriteLine("Layer {0} not equal entity to {1}", layer.Name, o.CodeName);
                }
            }

            foreach (LineType lType in dxf.LineTypes)
            {
                foreach (DxfObject o in dxf.LineTypes.GetReferences(lType))
                {
                    if (o is Layer)
                        if (!ReferenceEquals(lType, ((Layer)o).LineType))
                            Console.WriteLine("Line type {0} not equal to {1}", lType.Name, o.CodeName);
                    if (o is MLineStyle)
                    {
                        foreach (MLineStyleElement e in ((MLineStyle)o).Elements)
                        {
                            if (!ReferenceEquals(lType, e.LineType))
                                Console.WriteLine("Line type {0} not equal to {1}", lType.Name, o.CodeName);
                        }
                    }
                    if (o is EntityObject)
                        if (!ReferenceEquals(lType, ((EntityObject)o).LineType))
                            Console.WriteLine("Line type {0} not equal entity to {1}", lType.Name, o.CodeName);

                }
            }

            Console.WriteLine("Press a key to continue...");
            Console.ReadKey();
        }
        private static void DimensionNestedBlock()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(5, 5, 0);
            Line line = new Line(p1, p2);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            LinearDimension dim = new LinearDimension(line, 7, 0.0, myStyle);

            Block nestedBlock = new Block("NestedBlock");
            nestedBlock.Entities.Add(line);
            Insert nestedIns = new Insert(nestedBlock);

            Block block = new Block("MyBlock");
            block.Entities.Add(dim);
            block.Entities.Add(nestedIns);

            Insert ins = new Insert(block);
            ins.Position = new Vector3(10, 10, 0);
            dxf.AddEntity(ins);

            Circle circle = new Circle(p2, 5);
            Block block2 = new Block("MyBlock2");
            block2.Entities.Add(circle);

            Insert ins2 = new Insert(block2);
            ins2.Position = new Vector3(-10, -10, 0);
            dxf.AddEntity(ins2);

            Block block3 = new Block("MyBlock3");
            block3.Entities.Add((EntityObject)ins.Clone());
            block3.Entities.Add((EntityObject)ins2.Clone());

            Insert ins3 = new Insert(block3);
            ins3.Position = new Vector3(-10, 10, 0);
            dxf.AddEntity(ins3);

            dxf.Save("nested blocks.dxf");

            dxf = DxfDocument.Load("nested blocks.dxf");

            dxf.Save("nested blocks.dxf");
        }
        private static void ComplexHatch()
        {
            HatchPattern pattern = HatchPattern.FromFile("hatch\\acad.pat", "ESCHER");
            pattern.Scale = 1.5;
            pattern.Angle = 30;

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.IsClosed = true;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>
                {
                    new HatchBoundaryPath(new List<EntityObject> {poly})
                };
            Hatch hatch = new Hatch(pattern, boundary);
            
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(poly);
            dxf.AddEntity(hatch);
            dxf.Save("complexhatch.dxf");

            DxfDocument dxf2 = DxfDocument.Load("complexhatch.dxf");
            dxf2.Save("complexhatch2.dxf");

        }
        private static void RayAndXLine()
        {
            Ray ray = new Ray(new Vector3(1, 1, 1), new Vector3(1, 1, 1));
            XLine xline = new XLine(Vector2.Zero, new Vector2(1,1));

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(ray);
            dxf.AddEntity(xline);
            dxf.Save("RayAndXLine.dxf");


            dxf = DxfDocument.Load("RayAndXLine.dxf");

        }
        private static void UserCoordinateSystems()
        {
            DxfDocument dxf = new DxfDocument();
            UCS ucs1 = new UCS("user1", Vector3.Zero, Vector3.UnitX, Vector3.UnitZ);
            UCS ucs2 = UCS.FromXAxisAndPointOnXYplane("user2", Vector3.Zero, new Vector3(1,1,0), new Vector3(1,1,1));
            UCS ucs3 = UCS.FromNormal("user3", Vector3.Zero, new Vector3(1, 1, 1), 0);
            dxf.UCSs.Add(ucs1);
            dxf.UCSs.Add(ucs2);
            dxf.UCSs.Add(ucs3);

            dxf.Save("ucs.dxf");

            dxf = DxfDocument.Load("ucs.dxf");

        }
        private static void ImageUsesAndRemove()
        {
            ImageDef imageDef1 = new ImageDef("img\\image01.jpg");
            Image image1 = new Image(imageDef1, Vector3.Zero, 10, 10);

            ImageDef imageDef2 = new ImageDef("img\\image02.jpg");
            Image image2 = new Image(imageDef2, new Vector3(0, 220, 0), 10, 10);
            Image image3 = new Image(imageDef2, image2.Position + new Vector3(280, 0, 0), 10, 10);

            Block block =new Block("MyImageBlock");
            block.Entities.Add(image1);

            Insert insert = new Insert(block);

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(insert);
            dxf.AddEntity(image2);
            dxf.AddEntity(image3);
            dxf.Save("test netDxf.dxf");

           
            dxf.RemoveEntity(insert);
            dxf.Blocks.Remove(insert.Block.Name);
            // imageDef1 has no references in the document
            List<DxfObject> uses = dxf.ImageDefinitions.GetReferences(imageDef1.Name);
            dxf.Save("test netDxf with unreferenced imageDef.dxf");
            dxf = DxfDocument.Load("test netDxf with unreferenced imageDef.dxf");

            // once we have removed the insert and then the block that contained image1 we don't have more references to imageDef1
            dxf.ImageDefinitions.Remove(imageDef1.Name);
            dxf.Save("test netDxf with deleted imageDef.dxf");

        }
        private static void LayerAndLineTypesUsesAndRemove()
        {
            DxfDocument dxf = new DxfDocument();

            Layer layer1 = new Layer("Layer1");
            layer1.Color = AciColor.Blue;
            layer1.LineType = LineType.Center;

            Layer layer2 = new Layer("Layer2");
            layer2.Color = AciColor.Red;

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(0, 0));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(20, 0));
            poly.Vertexes.Add(new LwPolylineVertex(30, 10));
            poly.Layer = layer1;
            dxf.AddEntity(poly);

            Ellipse ellipse = new Ellipse(new Vector3(2, 2, 0), 5, 3);
            ellipse.Rotation = 30;
            ellipse.Layer = layer1;
            dxf.AddEntity(ellipse);

            Line line = new Line(new Vector2(10, 5), new Vector2(-10, -5));
            line.Layer = layer2;
            line.LineType = LineType.DashDot;
            dxf.AddEntity(line);


            bool ok;

            // this will return false since layer1 is not empty
            ok = dxf.Layers.Remove(layer1.Name);

            List<DxfObject> entities = dxf.Layers.GetReferences(layer1.Name);
            foreach (DxfObject o in entities)
            {
                dxf.RemoveEntity(o as EntityObject);
            }

            // now this should return true since layer1 is empty
            ok = dxf.Layers.Remove(layer1.Name);

            // blocks needs an special attention
            Layer layer3 = new Layer("Layer3");
            layer3.Color = AciColor.Yellow;

            Circle circle = new Circle(Vector3.Zero, 15);
            // it is always recommended that all block entities will be located in layer 0, but this is up to the user.
            circle.Layer = new Layer("circle");
            circle.Layer.Color = AciColor.Green;

            Block block = new Block("MyBlock");
            block.Entities.Add(circle);
            block.Layer = new Layer("blockLayer");
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            attdef.Layer = new Layer("attDefLayer");
            attdef.LineType = LineType.Center;
            block.AttributeDefinitions.Add(attdef);

            Insert insert = new Insert(block, new Vector2(5, 5));
            insert.Layer = layer3;
            insert.Attributes[attdef.Tag].Layer = new Layer("attLayer");
            insert.Attributes[attdef.Tag].LineType = LineType.Dashed;
            dxf.AddEntity(insert);

            dxf.Save("test.dxf");

            DxfDocument dxf2 = DxfDocument.Load("test.dxf");

            // this list will contain the circle entity
            List<DxfObject> dxfObjects;
            dxfObjects = dxf.Layers.GetReferences("circle");

            // but we cannot removed since it is part of a block
            ok = dxf.RemoveEntity(circle);
            // we need to remove first the block, but to do this we need to make sure there are no references of that block in the document
            dxfObjects = dxf.Blocks.GetReferences(block.Name);
            foreach (DxfObject o in dxfObjects)
            {
                dxf.RemoveEntity(o as EntityObject);
            }


            // now it is safe to remove the block since we do not have more references in the document
            ok = dxf.Blocks.Remove(block.Name);
            // now it is safe to remove the layer "circle", the circle entity was removed with the block since it was part of it
            ok = dxf.Layers.Remove("circle");

            // purge all document layers, only empty layers will be removed
            dxf.Layers.Clear();

            // purge all document line types, only line types without references will be removed
            dxf.LineTypes.Clear();

            dxf.Save("test2.dxf");
        }
        private static void TextAndDimensionStyleUsesAndRemove()
        {
            DxfDocument dxf = new DxfDocument();

            Layer layer1 = new Layer("Layer1");
            layer1.Color = AciColor.Blue;
            layer1.LineType = LineType.Center;

            Layer layer2 = new Layer("Layer2");
            layer2.Color = AciColor.Red;

            // blocks needs an special attention
            Layer layer3 = new Layer("Layer3");
            layer3.Color = AciColor.Yellow;

            Circle circle = new Circle(Vector3.Zero, 15);
            // it is always recommended that all block entities will be located in layer 0, but this is up to the user.
            circle.Layer = new Layer("circle");
            circle.Layer.Color = AciColor.Green;

            Block block = new Block("MyBlock");
            block.Entities.Add(circle);
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");

            block.AttributeDefinitions.Add(attdef);

            Insert insert = new Insert(block, new Vector2(5, 5));
            insert.Attributes[attdef.Tag].Style = new TextStyle("Arial.ttf");

            dxf.AddEntity(insert);

            dxf.Save("style.dxf");
            DxfDocument dxf2;
            dxf2 = DxfDocument.Load("style.dxf");

            dxf.RemoveEntity(circle);

            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(5, 5, 0);
            Line line = new Line(p1, p2);

            dxf.AddEntity(line);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.TextStyle = new TextStyle("Tahoma.ttf");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            double offset = 7;
            LinearDimension dimX = new LinearDimension(line, offset, 0.0, myStyle);
            dimX.Rotation += 30.0;
            LinearDimension dimY = new LinearDimension(line, offset, 90.0, myStyle);
            dimY.Rotation += 30.0;

            dxf.AddEntity(dimX);
            dxf.AddEntity(dimY);

            dxf.Save("style2.dxf");
            dxf2 = DxfDocument.Load("style2.dxf");


            dxf.RemoveEntity(dimX);
            dxf.RemoveEntity(dimY);

            bool ok;

            // we can remove myStyle it was only referenced by dimX and dimY
            ok = dxf.DimensionStyles.Remove(myStyle.Name);

            // we cannot remove myStyle.TextStyle since it is in use by the internal blocks created by the dimension entities
            ok = dxf.Blocks.Remove(dimX.Block.Name);
            ok = dxf.Blocks.Remove(dimY.Block.Name);

            // no we can remove the unreferenced textStyle
            ok = dxf.TextStyles.Remove(myStyle.TextStyle.Name);

            dxf.Save("style3.dxf");
            dxf2 = DxfDocument.Load("style3.dxf");
        }
        private static void MLineStyleUsesAndRemove()
        {
            DxfDocument dxf = new DxfDocument();
            //MLineStyle style = MLineStyle.Default;
            //dxf.AddMLineStyle(style);

            List<Vector2> vertexes = new List<Vector2>
                                         {
                                             new Vector2(0, 0),
                                             new Vector2(0, 150),
                                             new Vector2(150, 150),
                                             new Vector2(150, 0)
                                         };

            MLine mline = new MLine(vertexes);
            mline.Scale = 20;
            mline.Justification = MLineJustification.Zero;
            //mline.IsClosed = true;

            MLineStyle style = new MLineStyle("MyStyle", "Personalized style.");
            style.Elements.Add(new MLineStyleElement(0.25));
            style.Elements.Add(new MLineStyleElement(-0.25));
            // if we add new elements directly to the list we need to sort the list,
            style.Elements.Sort();
            style.Flags = MLineStyleFlags.EndInnerArcsCap | MLineStyleFlags.EndRoundCap | MLineStyleFlags.StartInnerArcsCap | MLineStyleFlags.StartRoundCap;
            //style.StartAngle = 25.0;
            //style.EndAngle = 160.0;
            // AutoCad2000 dxf version does not support true colors for MLineStyle elements
            style.Elements[0].Color = new AciColor(180, 230, 147);
            mline.Style = style;
            // we have modified the mline after setting its vertexes so we need to manually call this method.
            // also when manually editting the vertex distances
            mline.CalculateVertexesInfo();

            // we can manually create cuts or gaps in the individual elements that made the multiline.
            // the cuts are defined as distances from the start point of the element along its direction.
            mline.Vertexes[0].Distances[0].Add(50);
            mline.Vertexes[0].Distances[0].Add(100);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count - 1].Add(50);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count - 1].Add(100);
            dxf.AddEntity(mline);

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("MLine.dxf");

            DxfDocument dxf2 = DxfDocument.Load("MLine.dxf");

            // "MyStyle" is used only once
            List<DxfObject> uses;
            uses = dxf.MlineStyles.GetReferences(mline.Style.Name);

            // if we try to get the LineTypeUses, we will find out that "MyStyle" appears several times,
            // this is due to that each MLineStyleElement of a MLineStyle has an associated LineType
            uses = dxf.LineTypes.GetReferences(LineType.ByLayer.Name);

            bool ok;
            ok = dxf.RemoveEntity(mline);

            // "MyStyle" is not used its reference has been deleted
            uses = dxf.MlineStyles.GetReferences(mline.Style.Name);
            // we can safely remove it
            dxf.MlineStyles.Remove(mline.Style.Name);

            dxf.Save("MLine2.dxf");

            dxf.Layers.Clear();

            dxf.Save("MLine2.dxf");
        }
        private static void AppRegUsesAndRemove()
        {
            DxfDocument dxf = new DxfDocument();

            List<PolylineVertex> vertexes = new List<PolylineVertex>{
                                                                        new PolylineVertex(0, 0, 0), 
                                                                        new PolylineVertex(10, 0, 10), 
                                                                        new PolylineVertex(10, 10, 20), 
                                                                        new PolylineVertex(0, 10, 30)
                                                                        };

            Polyline poly = new Polyline(vertexes, true);

            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));

            poly.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata1.ApplicationRegistry.Name, xdata1},
                             };
            dxf.AddEntity(poly);

            Line line = new Line(new Vector2(10, 5), new Vector2(-10, -5));

            ApplicationRegistry myAppReg = new ApplicationRegistry("MyAppReg");
            XData xdata2 = new XData(myAppReg);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Distance, Vector3.Distance(line.StartPoint, line.EndPoint)));
            line.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {myAppReg.Name, xdata2},
                             };
            dxf.AddEntity(line);

            Circle circle = new Circle(Vector3.Zero, 15);
            XData xdata3 = new XData(myAppReg);
            xdata3.XDataRecord.Add(new XDataRecord(XDataCode.Real, circle.Radius));
            circle.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {myAppReg.Name, xdata3},
                             };
            dxf.AddEntity(circle);

            dxf.Save("appreg.dxf");

            DxfDocument dxf2 = DxfDocument.Load("appreg.dxf");

            // will return false the "MyAppReg" is in use
            bool ok;
            ok = dxf.ApplicationRegistries.Remove(myAppReg.Name);
            dxf.RemoveEntity(line);
            dxf.RemoveEntity(circle);
            // "MyAppReg" is not used anymore
            IList<DxfObject> uses = dxf.ApplicationRegistries.GetReferences(myAppReg.Name);
            // it is safe to delete it
            ok = dxf.ApplicationRegistries.Remove(myAppReg.Name);
            
            // we can even make a full cleanup
            dxf.ApplicationRegistries.Clear();

            dxf.Save("appreg2.dxf");


        }
        private static void ExplodePolyfaceMesh()
        {
            DxfDocument dxf = DxfDocument.Load("polyface mesh.dxf");
            DxfDocument dxfOut = new DxfDocument(dxf.DrawingVariables);
            foreach (PolyfaceMesh polyfaceMesh in dxf.PolyfaceMeshes)
            {
                List<EntityObject> entities = polyfaceMesh.Explode();
                dxfOut.AddEntity(entities);
            }

            dxfOut.Save("polyface mesh exploded.dxf");
        }
        private static void ApplicationRegistries()
        {
            DxfDocument dxf = new DxfDocument();
            // add a new application registry to the document (optional), if not present it will be added when the entity is passed to the document
            ApplicationRegistry newAppReg = dxf.ApplicationRegistries.Add(new ApplicationRegistry("NewAppReg"));

            Line line = new Line(Vector2.Zero, 100 * Vector2.UnitX);
            XData xdata = new XData(newAppReg);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "string of the new application registry"));
            line.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                             };

            dxf.AddEntity(line);
            dxf.Save("ApplicationRegistryTest.dxf");

            // gets the complete application registries present in the document
            ICollection<ApplicationRegistry> appRegs = dxf.ApplicationRegistries.Items;

            // get an application registry by name
            //ApplicationRegistry netDxfAppReg = dxf.ApplicationRegistries[appRegs[dxf.ApplicationRegistries.Count - 1].Name];
        }
        private static void TestOCStoWCS()
        {
            // vertexes of the light weight polyline, they are defined in OCS (Object Coordinate System)
            LwPolylineVertex v1 = new LwPolylineVertex(1, -5);
            LwPolylineVertex v2 = new LwPolylineVertex(-3, 2);
            LwPolylineVertex v3 = new LwPolylineVertex(8, 15);

            LwPolyline lwp = new LwPolyline(new List<LwPolylineVertex> {v1, v2, v3});
            // the normal will define the plane where the lwpolyline is defined
            lwp.Normal = new Vector3(1, 1, 0);
            // the entity elevation defines the z vector of the vertexes along the entity normal
            lwp.Elevation = 2.5;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(lwp);
            dxf.Save("OCStoWCS.dxf");

            // if you want to convert the vertexes of the polyline to WCS (World Coordinate System), you can
            Vector3 v1OCS = new Vector3(v1.Location.X, v1.Location.Y, lwp.Elevation);
            Vector3 v2OCS = new Vector3(v2.Location.X, v2.Location.Y, lwp.Elevation);
            Vector3 v3OCS = new Vector3(v3.Location.X, v3.Location.Y, lwp.Elevation);
            List<Vector3> vertexesWCS = MathHelper.Transform(new List<Vector3> { v1OCS, v2OCS, v3OCS }, lwp.Normal, MathHelper.CoordinateSystem.Object, MathHelper.CoordinateSystem.World);


        }
        private static void WriteGradientPattern()
        {
            List<LwPolylineVertex> vertexes = new List<LwPolylineVertex>
                                         {
                                             new LwPolylineVertex(new Vector2(0, 0)),
                                             new LwPolylineVertex(new Vector2(0, 150)),
                                             new LwPolylineVertex(new Vector2(150, 150)),
                                             new LwPolylineVertex(new Vector2(150, 0))
                                         };
            LwPolyline pol = new LwPolyline(vertexes, true);


            Line line1 = new Line(new Vector2(0, 0), new Vector2(0, 150));
            Line line2 = new Line(new Vector2(0, 150), new Vector2(150, 150));
            Line line3 = new Line(new Vector2(150, 150), new Vector2(150, 0));
            Line line4 = new Line(new Vector2(150, 0), new Vector2(0, 0));


            HatchGradientPattern gradient = new HatchGradientPattern(AciColor.Red, AciColor.Blue, HatchGradientPatternType.Linear);
            //HatchGradientPattern gradient = new HatchGradientPattern(AciColor.Red, 0.75, HatchGradientPatternType.Linear);
            gradient.Angle = 30;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>
                                                   {
                                                       new HatchBoundaryPath(new List<EntityObject> {pol})
                                                   };
            Hatch hatch = new Hatch(gradient, boundary);
            
            // gradients are only supported for AutoCad2004 and later
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2004);
            dxf.AddEntity(hatch);
            dxf.Save("gradient test.dxf");

            //DxfDocument dxf2 = DxfDocument.Load("gradient test.dxf");

            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;

            //AciColor color = new AciColor(63, 79, 127);

            //dxf.Save("gradient test 2000.dxf");

        }
        private static void WriteGroup()
        {
            Line line1 = new Line(new Vector2(0, 0), new Vector2(100, 100));
            Line line2 = new Line(new Vector2(100, 0), new Vector2(200, 100));
            Line line3 = new Line(new Vector2(200, 0), new Vector2(300, 100));

            // named group
            Group group1 = new Group("MyGroup")
                {
                    Entities = new EntityCollection {line1, line2}
                };

            //unnamed group
            Group group2 = new Group
                {
                    Entities = new EntityCollection { line1, line3 }
                };

            DxfDocument dxf = new DxfDocument();
            // the AddGroup method will also add the entities contained in a group to the document.
            dxf.Groups.Add(group1);
            dxf.Groups.Add(group2);

            List<DxfObject> list = dxf.Groups.GetReferences(group1);
            dxf.Save("group.dxf");

            dxf = DxfDocument.Load("group.dxf");

            group1 = dxf.Groups[group1.Name];
            group2 = dxf.Groups[group2.Name];
            dxf.Groups.Remove(group1);
            dxf.Groups.Ungroup(group2);
            dxf.Save("group copy.dxf");


        }
        private static void WriteMLine()
        {
            DxfDocument dxf = new DxfDocument();
            //MLineStyle style = MLineStyle.Default;
            //dxf.AddMLineStyle(style);

            List<Vector2> vertexes = new List<Vector2>
                                         {
                                             new Vector2(0, 0),
                                             new Vector2(0, 150),
                                             new Vector2(150, 150),
                                             new Vector2(150, 0)
                                         };

            MLine mline = new MLine(vertexes);
            mline.Scale = 20;
            mline.Justification = MLineJustification.Zero;
            //mline.IsClosed = true;

            MLineStyle style = new MLineStyle("MyStyle", "Personalized style.");
            style.Elements.Add(new MLineStyleElement(0.25));
            style.Elements.Add(new MLineStyleElement(-0.25));
            // if we add new elements directly to the list we need to sort the list,
            style.Elements.Sort();
            style.Flags = MLineStyleFlags.EndInnerArcsCap | MLineStyleFlags.EndRoundCap | MLineStyleFlags.StartInnerArcsCap | MLineStyleFlags.StartRoundCap;
            //style.StartAngle = 25.0;
            //style.EndAngle = 160.0;
            // AutoCad2000 dxf version does not support true colors for MLineStyle elements
            style.Elements[0].Color = new AciColor(180, 230, 147);
            mline.Style = style;
            // we have modified the mline after setting its vertexes so we need to manually call this method.
            // also when manually editting the vertex distances
            mline.CalculateVertexesInfo();

            // we can manually create cuts or gaps in the individual elements that made the multiline.
            // the cuts are defined as distances from the start point of the element along its direction.
            mline.Vertexes[0].Distances[0].Add(50);
            mline.Vertexes[0].Distances[0].Add(100);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count-1].Add(50);
            mline.Vertexes[0].Distances[mline.Style.Elements.Count-1].Add(100);
            dxf.AddEntity(mline);

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("MLine.dxf");

            //dxf = DxfDocument.Load("Drawing1.dxf");
            //dxf.Save("Drawing1 copy.dxf");

            //dxf = DxfDocument.Load("Drawing3.dxf");
            //dxf.Save("Drawing3 copy.dxf");

            //dxf = DxfDocument.Load("Drawing2.dxf");
            //dxf.Save("Drawing2 copy.dxf");

            // empty mline
            //List<Vector2> vertexes2 = new List<Vector2>
            //                             {
            //                                 new Vector2(0, 0),
            //                                 new Vector2(100, 100),
            //                                 new Vector2(100, 100),
            //                                 new Vector2(200, 0)
            //                             };

            //MLine mline2 = new MLine(vertexes2){Scale = 20};
            //mline2.CalculateVertexesInfo();

            //DxfDocument dxf2 = new DxfDocument();
            //dxf2.AddEntity(mline2);
            ////dxf2.Save("void mline.dxf");

            //MLine mline3 = new MLine();
            //dxf2.AddEntity(mline3);
            ////dxf2.Save("void mline.dxf");

            //Polyline pol = new Polyline();
            //LwPolyline lwPol = new LwPolyline();
            //dxf2.AddEntity(pol);
            //dxf2.AddEntity(lwPol);
            //dxf2.Save("void mline.dxf");
            //dxf2 = DxfDocument.Load("void mline.dxf");
            
        }
        private static void ObjectVisibility()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0))
                            {
                                Color = AciColor.Yellow
                            };

            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0))
                             {
                                 IsVisible = false
                             };

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.Save("object visibility.dxf");
            dxf = DxfDocument.Load("object visibility.dxf");
            dxf.Save("object visibility 2.dxf");

        }
        private static void WriteInsert()
        {
            // nested blocks
            DxfDocument dxf = new DxfDocument();

            Block nestedBlock = new Block("Nested block");
            nestedBlock.Entities.Add(new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0)));
            nestedBlock.Entities.Add(new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0)));

            Insert nestedInsert = new Insert(nestedBlock, new Vector3(0, 0, 0)); // the position will be relative to the position of the insert that nest it

            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("circle");
            circle.Layer.Color.Index = 2;
            Block block = new Block("MyBlock");
            block.Entities.Add(circle);
            block.Entities.Add(nestedInsert);

            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");

            dxf.AddEntity(insert);

            dxf.Save("insert.dxf");
            dxf = DxfDocument.Load("insert.dxf");

        }
        private static void EntityTrueColor()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            line.Color = new AciColor(152, 103, 136);
            // by default a color initialized with rgb components will be exported as true color
            // you can override this behaviour with
            // line.Color.UseTrueColor = false;

            Layer layer = new Layer("MyLayer");
            layer.Color = new AciColor(157, 238, 17);
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            line2.Layer = layer;
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.Save("line true color.dxf");
            dxf = DxfDocument.Load("line true color.dxf");
        }
        private static void EntityLineWeight()
        {
            // the lineweight is always defined as 1/100 mm, this property is the equivalent of stroke width, outline width in other programs. Do not confuse with line.Thickness
            // it follow the AutoCAD naming style, check the documentation in case of doubt
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            line.Lineweight.Value = 100; // 1.0 mm
            Text text = new Text("Text with lineweight", Vector3.Zero, 10);
            text.Lineweight.Value = 50; // 0.5 mm

            Layer layer = new Layer("MyLayer");
            layer.Lineweight.Value = 200; // 2 mm all entities in the layer with Color.ByLayer will inherit this value
            layer.Color = AciColor.Green;
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            line2.Layer = layer;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.AddEntity(line2);
            dxf.AddEntity(text);
            dxf.Save("line weight.dxf");
            dxf = DxfDocument.Load("line weight.dxf");
        }
        private static void ReadWriteFromStream()
        {
            // Load and Save methods are now able to work directly with a stream.
            // They will return true or false if the operation has been carried out successfully or not.
            // The Save(string file, DxfVersion dxfVersion) and Load(string file) methods will still raise an exception if they are unable to create the FileStream.
            // On Debug mode they will raise any exception that migh occurr during the whole process.
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(line);
            dxf.Save("test.dxf");
            
            // saving to memory stream always use the default constructor, a fixed size stream will not work.
            MemoryStream memoryStream = new MemoryStream();
            if(!dxf.Save(memoryStream))
                throw new Exception("Error saving to memory stream.");
            
            // loading from memory stream
            DxfDocument dxf2 = DxfDocument.Load(memoryStream);
            memoryStream.Close(); // once the stream is not need anymore we need to close the stream

            // saving to file stream
            FileStream fileStream = new FileStream("test fileStream.dxf", FileMode.Create);
            if (!dxf2.Save(fileStream, true))
                throw new Exception("Error saving to file stream.");

            fileStream.Close(); // you will need to close the stream manually to avoid file already open conflicts

            FileStream fileStreamLoad = new FileStream("test fileStream.dxf", FileMode.Open, FileAccess.Read);
            DxfDocument dxf3 = DxfDocument.Load(fileStreamLoad);
            fileStreamLoad.Close();

            DxfDocument dxf4 = DxfDocument.Load("test fileStream.dxf");

        }
        private static void Text()
        {
            // use a font that has support for chinesse characters
            TextStyle textStyle = new TextStyle("Chinese text", "simsun.ttf");

            // for dxf database version 2007 and later you can use directly the characters,
            DxfDocument dxf1 = new DxfDocument(DxfVersion.AutoCad2010);
            Text text1 = new Text("这是中国文字", Vector2.Zero, 10, textStyle);
            MText mtext1 = new MText("这是中国文字", new Vector2(0, 30), 10, 0, textStyle);
            dxf1.AddEntity(text1);
            dxf1.AddEntity(mtext1);
            dxf1.Save("textCad2010.dxf");

            foreach (Text text in dxf1.Texts)
            {
                Console.WriteLine(text.Value);
            }
            foreach (MText text in dxf1.MTexts)
            {
                Console.WriteLine(text.Value);
            }

            Console.WriteLine("Press a key to continue...");
            Console.ReadLine();

            DxfDocument loadDxf = DxfDocument.Load("textCad2010.dxf");

            // for previous version (this method will also work for later ones) you will need to supply the unicode value (U+value),
            // you can get this value with the Windows Character Map application
            DxfDocument dxf2 = new DxfDocument(DxfVersion.AutoCad2010);
            Text text2 = new Text("\\U+8FD9\\U+662F\\U+4E2D\\U+56FD\\U+6587\\U+5B57", Vector2.Zero, 10, textStyle);
            MText mtext2 = new MText("\\U+8FD9\\U+662F\\U+4E2D\\U+56FD\\U+6587\\U+5B57", new Vector2(0, 30), 10, 0, textStyle);
            dxf2.AddEntity(text2);
            dxf2.AddEntity(mtext2);
            dxf2.Save("textCad2000.dxf");
        }
        private static void WriteNoAsciiText()
        {
            TextStyle textStyle = new TextStyle("Arial.ttf");
            DxfDocument dxf = new DxfDocument();
            dxf.DrawingVariables.LastSavedBy = "ЉЊЋЌЍжзицрлЯ";
            //Text text = new Text("ÁÉÍÓÚ áéíóú Ññ àèìòù âêîôû", Vector2.Zero,10);
            Text text = new Text("ЉЊЋЌЍжзицрлЯ", Vector2.Zero, 10, textStyle);
            MText mtext = new MText("ЉЊЋЌЍжзицрлЯ", new Vector2(0, 50), 10, 0, textStyle);

            dxf.AddEntity(text);
            dxf.AddEntity(mtext);
            foreach (Text t in dxf.Texts)
            {
                Console.WriteLine(t.Value);
            }
            foreach (MText t in dxf.MTexts)
            {
                Console.WriteLine(t.Value);
            }
            Console.WriteLine("Press a key to continue...");
            Console.ReadLine();
            dxf.Save("text1.dxf");

            dxf = DxfDocument.Load("text1.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("text2.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            dxf.Save("text3.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("text4.dxf");

        }
        private static void WriteSplineBoundaryHatch()
        {

            List<SplineVertex> ctrlPoints = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0)),
                                                    new SplineVertex(new Vector3(25, 50, 0)),
                                                    new SplineVertex(new Vector3(50, 0, 0)),
                                                    new SplineVertex(new Vector3(75, 50, 0)),
                                                    new SplineVertex(new Vector3(100, 0, 0))
                                                };

            // hatch with single closed spline boundary path
            Spline spline = new Spline(ctrlPoints, 3, true); // closed periodic

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>();

            HatchBoundaryPath path = new HatchBoundaryPath(new List<EntityObject> {spline});
            boundary.Add(path);
            Hatch hatch = new Hatch(HatchPattern.Line, boundary);
            hatch.Pattern.Angle = 45;
            hatch.Pattern.Scale = 10;

            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(hatch);
            dxf.AddEntity(spline);
            dxf.Save("hatch closed spline.dxf");
            dxf = DxfDocument.Load("hatch closed spline.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("hatch closed spline 2010.dxf");

            // hatch boundary path with spline and line
            Spline openSpline = new Spline(ctrlPoints, 3);
            Line line = new Line(ctrlPoints[0].Location, ctrlPoints[ctrlPoints.Count - 1].Location);

            List<HatchBoundaryPath> boundary2 = new List<HatchBoundaryPath>();
            HatchBoundaryPath path2 = new HatchBoundaryPath(new List<EntityObject> { openSpline, line });
            boundary2.Add(path2);
            Hatch hatch2 = new Hatch(HatchPattern.Line, boundary2);
            hatch2.Pattern.Angle = 45;
            hatch2.Pattern.Scale = 10;

            DxfDocument dxf2 = new DxfDocument();
            dxf2.AddEntity(hatch2);
            dxf2.AddEntity(openSpline);
            dxf2.AddEntity(line);
            dxf2.Save("hatch open spline.dxf");
            dxf2 = DxfDocument.Load("hatch open spline.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf2.Save("hatch open spline 2010.dxf");

        }
        private static void WriteNoInsertBlock()
        {
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(100, 100, 0));
            Line line2 = new Line(new Vector3(0, 100, 0), new Vector3(100, 0, 0));
            // create the block definition
            Block block = new Block("MyBlock");
            // add the entities that you want in the block
            block.Entities.Add(line);
            block.Entities.Add(line2);
            
            
            // create the document
            DxfDocument dxf = new DxfDocument();
            // add the block definition to the block table list (this is the function that was private in earlier versions, check the changelog.txt)
            dxf.Blocks.Add(block);

            // and save file, no visible entities will appear if you try to open the drawing but the block will be there
            dxf.Save("Block definiton.dxf");

        }
        private static void WriteImage()
        {
            ImageDef imageDef = new ImageDef("img\\image01.jpg");
            Image image = new Image(imageDef, Vector3.Zero, 10, 10);

            XData xdata1 = new XData(new ApplicationRegistry("netDxf"));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.String, "xData image position"));
            xdata1.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, image.Position.X));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, image.Position.Y));
            xdata1.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, image.Position.Z));
            xdata1.XDataRecord.Add(XDataRecord.CloseControlString);
            image.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata1.ApplicationRegistry.Name, xdata1},
                             };

            //image.Normal = new Vector3(1, 1, 1);
            //image.Rotation = 30;

            // you can pass a name that must be unique for the image definiton, by default it will use the file name without the extension
            ImageDef imageDef2 = new ImageDef("img\\image02.jpg", "MyImage");
            Image image2 = new Image(imageDef2, new Vector3(0, 150, 0), 10, 10);
            Image image3 = new Image(imageDef2, new Vector3(150, 150, 0), 10, 10);

            // clipping boundary definition in local coordinates
            ImageClippingBoundary clip = new ImageClippingBoundary(100, 100, 500, 300);
            image.ClippingBoundary = clip;
            // set to null to restore the default clipping boundary (full image)
            image.ClippingBoundary = null;

            // images can be part of a block definition
            Block block = new Block("ImageBlock");
            block.Entities.Add(image2);
            block.Entities.Add(image3);
            Insert insert = new Insert(block, new Vector3(0, 100, 0));

            DxfDocument dxf = new DxfDocument();

            dxf.AddEntity(image);
            //dxf.AddEntity(image2);
            //dxf.AddEntity(image3);
            dxf.AddEntity(insert);

            dxf.Save("image.dxf");
            dxf = DxfDocument.Load("image.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("image.dxf");

            //dxf.RemoveEntity(image2);
            //dxf.Save("image2.dxf");
            //dxf.RemoveEntity(image3);
            //dxf.RemoveEntity(image);
            //dxf.Save("image3.dxf");

        }
        private static void SplineDrawing()
        {
            List<SplineVertex> ctrlPoints = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0), 1),
                                                    new SplineVertex(new Vector3(25, 50, 0), 2),
                                                    new SplineVertex(new Vector3(50, 0, 0), 3),
                                                    new SplineVertex(new Vector3(75, 50, 0), 4),
                                                    new SplineVertex(new Vector3(100, 0, 0), 5)
                                                };

            // the constructor will generate a uniform knot vector 
            Spline openSpline = new Spline(ctrlPoints, 3);

            List<SplineVertex> ctrlPointsClosed = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(0, 0, 0), 1),
                                                    new SplineVertex(new Vector3(25, 50, 0), 2),
                                                    new SplineVertex(new Vector3(50, 0, 0), 3),
                                                    new SplineVertex(new Vector3(75, 50, 0), 4),
                                                    new SplineVertex(new Vector3(100, 0, 0), 5),
                                                    new SplineVertex(new Vector3(0, 0, 0), 1) // closed spline non periodic we repeat the last control point
                                                };
            Spline closedNonPeriodicSpline = new Spline(ctrlPointsClosed, 3);

            // the periodic spline will generate a periodic (unclamped) closed curve,
            // as far as my tests have gone not all programs handle them correctly, most of them only handle clamped splines
            // seems that AutoCAD imports periodic closed splines correclty if all control point weights are equal to one.
            // seems that internally AutoCAD converts periodic closed splines to nonperiodic (clamped) closed splines.
            // my knowledge on nurbs is limited
            Spline closedPeriodicSpline = new Spline(ctrlPoints, 3, true);
            closedPeriodicSpline.SetUniformWeights(1.0);

            // manually defining the control points and the knot vector (example a circle created with nurbs)
            List<SplineVertex> circle = new List<SplineVertex>
                                                {
                                                    new SplineVertex(new Vector3(50, 0, 0), 1.0),
                                                    new SplineVertex(new Vector3(100, 0, 0), 1.0/2.0),
                                                    new SplineVertex(new Vector3(100, 100, 0), 1.0/2.0),
                                                    new SplineVertex(new Vector3(50, 100, 0), 1.0),
                                                    new SplineVertex(new Vector3(0, 100, 0), 1.0/2.0),
                                                    new SplineVertex(new Vector3(0, 0, 0), 1.0/2.0),
                                                    new SplineVertex(new Vector3(50, 0, 0), 1.0) // repeat the first point to close the circle
                                                };

            // the number of knots must be control points number + degree + 1
            // Conics are 2nd degree curves
            double[] knots = new[] {0, 0, 0, 1.0/4.0, 1.0/2.0, 1.0/2.0, 3.0/4.0, 1.0, 1.0, 1.0};
            Spline splineCircle = new Spline(circle, knots, 2);

            DxfDocument dxf = new DxfDocument();
            //dxf.AddEntity(openSpline);
            //dxf.AddEntity(closedNonPeriodicSpline);
            dxf.AddEntity(closedPeriodicSpline);
            //dxf.AddEntity(splineCircle);
            dxf.Save("spline.dxf");

        }
        private static void AddAndRemove()
        {
            Layer layer1 = new Layer("layer1") { Color = AciColor.Blue };
            Layer layer2 = new Layer("layer2") { Color = AciColor.Green };

            Line line = new Line(new Vector2(0, 0), new Vector2(10, 10));
            line.Layer = layer1;
            Circle circle = new Circle(new Vector2(0, 0), 10);
            circle.Layer = layer2;

            double offset = -0.9;
            Vector3 p1 = new Vector3(1, 2, 0);
            Vector3 p2 = new Vector3(2, 6, 0);
            Line line1 = new Line(p1, p2);
            Vector3 l1;
            Vector3 l2;
            MathHelper.OffsetLine(line1.StartPoint, line1.EndPoint, line1.Normal, offset, out l1, out l2);

            DimensionStyle myStyle = new DimensionStyle("MyDimStyle");
            myStyle.DIMPOST = "<>mm";
            AlignedDimension dim1 = new AlignedDimension(p1, p2, offset, myStyle);

            //text
            TextStyle style = new TextStyle("MyTextStyle", "Arial.ttf");
            Text text = new Text("Hello world!", Vector3.Zero, 10.0f, style)
                            {
                                Layer = new Layer("text")
                                            {
                                                Color = {Index = 8}
                                            }
                            };
            text.Alignment = TextAlignment.TopRight;

            HeaderVariables variables = new HeaderVariables
                                            {
                                                AcadVer = DxfVersion.AutoCad2004
                                            };
            DxfDocument dxf = new DxfDocument();
            dxf.AddEntity(new EntityObject[] {line, circle, dim1, text});
            dxf.Save("before remove.dxf");

            dxf.RemoveEntity(circle);
            dxf.Save("after remove.dxf");

            dxf.AddEntity(circle);
            dxf.Save("after remove and add.dxf");

            dxf.RemoveEntity(dim1);
            dxf.Save("remove dim.dxf");

            dxf.AddEntity(dim1);
            dxf.Save("add dim.dxf");

            DxfDocument dxf2 = DxfDocument.Load("dim block names.dxf");
            dxf2.AddEntity(dim1);
            dxf2.Save("dim block names2.dxf");
        }
        private static void LoadAndSave()
        {
            DxfDocument dxf = DxfDocument.Load("block sample.dxf");
            dxf.Save("block sample1.dxf");

            DxfDocument dxf2 = new DxfDocument();
            dxf2.AddEntity(dxf.Inserts[0]);
            dxf2.Save("block sample2.dxf");

            dxf.Save("clean2.dxf");
            dxf = DxfDocument.Load("clean.dxf");
            dxf.Save("clean1.dxf");

            // open a dxf saved with autocad
            dxf = DxfDocument.Load("sample.dxf");
            dxf.Save("sample4.dxf");

            Line cadLine = dxf.Lines[0];
            Layer layer = new Layer("netLayer");
            layer.Color = AciColor.Yellow;

            Line line = new Line(new Vector2(20, 40), new Vector2(100, 200));
            line.Layer = layer;
            // add a new entity to the document
            dxf.AddEntity(line);

            dxf.Save("sample2.dxf");

            DxfDocument dxf3 = new DxfDocument();
            dxf3.AddEntity(cadLine);
            dxf3.AddEntity(line);
            dxf3.Save("sample3.dxf");
        }
        private static void CleanDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            dxf.Save("clean drawing.dxf");
        }
        private static void OrdinateDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 origin = new Vector3(2, 1, 0);
            Vector2 refX = new Vector2(1, 0);
            Vector2 refY = new Vector2(0, 2);
            double length = 3;
            double angle = 30;
            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;

            OrdinateDimension dimX1 = new OrdinateDimension(origin, refX, length, OrdinateDimensionAxis.X, 0, myStyle);
            OrdinateDimension dimX2 = new OrdinateDimension(origin, refX, length, OrdinateDimensionAxis.X, angle, myStyle);
            OrdinateDimension dimY1 = new OrdinateDimension(origin, refY, length, OrdinateDimensionAxis.Y, 0, myStyle);
            OrdinateDimension dimY2 = new OrdinateDimension(origin, refY, length, OrdinateDimensionAxis.Y, angle, myStyle);

            dxf.AddEntity(dimX1);
            dxf.AddEntity(dimY1);
            dxf.AddEntity(dimX2);
            dxf.AddEntity(dimY2);

            Line lineX = new Line(origin, origin+5 * Vector3.UnitX);
            Line lineY = new Line(origin, origin+5 * Vector3.UnitY);

            Vector2 point = Vector2.Polar(new Vector2(origin.X, origin.Y), 5, angle * MathHelper.DegToRad);
            Line lineXRotate = new Line(origin, new Vector3(point.X, point.Y, 0));

            point = Vector2.Polar(new Vector2(origin.X, origin.Y), 5, angle * MathHelper.DegToRad + MathHelper.HalfPI);
            Line lineYRotate = new Line(origin, new Vector3(point.X, point.Y, 0));

            dxf.AddEntity(lineX);
            dxf.AddEntity(lineY);
            dxf.AddEntity(lineXRotate);
            dxf.AddEntity(lineYRotate);

            dxf.Save("ordinate dimension.dxf");

            dxf = DxfDocument.Load("ordinate dimension.dxf");
        }
        private static void Angular2LineDimensionDrawing()
        {
            double offset = 7.5;
            
            Line line1 = new Line(new Vector2(1, 2), new Vector2(6, 0));
            Line line2 = new Line(new Vector2(2, 1), new Vector2(4,5));

            Angular2LineDimension dim = new Angular2LineDimension(line1, line2, offset);
            
            DxfDocument dxf = new DxfDocument();
            //dxf.AddEntity(line1);
            //dxf.AddEntity(line2);
            //dxf.AddEntity(dim);

            Block block = new Block("DimensionBlock");
            block.Entities.Add(line1);
            block.Entities.Add(line2);
            block.Entities.Add(dim);
            Insert insert = new Insert(block);

            dxf.AddEntity(insert);

            dxf.Save("angular 2 line dimension.dxf");
            dxf = DxfDocument.Load("angular 2 line dimension.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("angular 2 line dimension.dxf");
        }
        private static void Angular3PointDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 2.42548;
            Arc arc = new Arc(center, radius, -30, 60);
            //arc.Normal = new Vector3(1, 1, 1);
            DimensionStyle myStyle = new DimensionStyle("MyStyle");

            Angular3PointDimension dim = new Angular3PointDimension(arc, 5, myStyle);
            dxf.AddEntity(arc);
            dxf.AddEntity(dim);
            dxf.Save("angular 3 point dimension.dxf");

            dxf = DxfDocument.Load("angular 3 point dimension.dxf");
        }
        private static void DiametricDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 2.42548;
            Circle circle = new Circle(center, radius);
            //circle.Normal = new Vector3(1, 1, 1);
            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            myStyle.DIMDSEP = ',';

            DiametricDimension dim = new DiametricDimension(circle, 30, myStyle);
            dxf.AddEntity(circle);
            dxf.AddEntity(dim);
            dxf.Save("diametric dimension.dxf");

            dxf.RemoveEntity(dim);
            dxf.Save("diametric dimension removed.dxf");

            dxf = DxfDocument.Load("diametric dimension.dxf");
            // remove entitiy with a handle
            Dimension dimLoaded = (Dimension) dxf.GetObjectByHandle(dim.Handle);
            dxf.RemoveEntity(dimLoaded);
            dxf.Save("diametric dimension removed 2.dxf");

        }
        private static void RadialDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 2, 0);
            double radius = 2.42548;
            Circle circle = new Circle(center, radius);
            circle.Normal = new Vector3(1, 1, 1);
            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            myStyle.DIMDSEP = ',';
            
            RadialDimension dim = new RadialDimension(circle, 30, myStyle);
            dxf.AddEntity(circle);
            dxf.AddEntity(dim);
            dxf.Save("radial dimension.dxf");

            dxf = DxfDocument.Load("radial dimension.dxf");
        }
        private static void LinearDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            
            Vector3 p1 = new Vector3(0, 0, 0);
            Vector3 p2 = new Vector3(5, 5, 0);
            Line line = new Line(p1, p2);

            dxf.AddEntity(line);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            myStyle.DIMDEC = 2;
            double offset = 7;
            LinearDimension dimX = new LinearDimension(line, offset,0.0, myStyle);
            dimX.Rotation += 30.0;
            LinearDimension dimY = new LinearDimension(line, offset, 90.0, myStyle);
            dimY.Rotation += 30.0;

            XData xdata = new XData(new ApplicationRegistry("other application"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "Linear Dimension"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);
            dimX.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata}
                             };
            dimY.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata}
                             };
            dxf.AddEntity(dimX);
            dxf.AddEntity(dimY);
            dxf.Save("linear dimension.dxf");
           // dxf = DxfDocument.Load("linear dimension.dxf");
        }
        private static void AlignedDimensionDrawing()
        {
            DxfDocument dxf = new DxfDocument();
            double offset = -0.9;
            Vector3 p1 = new Vector3(1, 2, 0);
            Vector3 p2 = new Vector3(2, 6, 0);
            Line line1 = new Line(p1, p2);
            Vector3 l1;
            Vector3 l2;
            MathHelper.OffsetLine(line1.StartPoint, line1.EndPoint, line1.Normal, offset, out l1, out l2);

            DimensionStyle myStyle = new DimensionStyle("MyStyle");
            myStyle.DIMPOST = "<>mm";
            AlignedDimension dim1 = new AlignedDimension(p1, p2, offset, myStyle);

            Vector3 p3 = p1 + new Vector3(4, 0, 0);
            Vector3 p4 = p2 + new Vector3(4, 0, 0);
            Line line2 = new Line(p3, p4);
            AlignedDimension dim2 = new AlignedDimension(p3, p4, -offset, myStyle);


            Vector2 perp = Vector2.Perpendicular(new Vector2(p2.X, p2.Y) - new Vector2(p1.X, p1.Y));
            //dim.Normal = -new Vector3(perp.X, perp.Y, 0.0) ;

            XData xdata = new XData(new ApplicationRegistry("other application"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "Aligned Dimension"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);
            dim1.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata}
                             };

            //dxf.AddEntity(line1);
            //dxf.AddEntity(line2);
            //dxf.AddEntity(dim1);
            //dxf.AddEntity(dim2);



            Block block = new Block("DimensionBlock");
            block.Entities.Add(line1);
            block.Entities.Add(line2);
            block.Entities.Add(dim1);
            block.Entities.Add(dim2);
            Insert insert = new Insert(block);
            dxf.AddEntity(insert);

            dxf.Save("aligned dimension.dxf");

            dxf = DxfDocument.Load("aligned dimension.dxf");

        }
        private static void WriteMText()
        {
            DxfDocument dxf = new DxfDocument();

            //xData sample
            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            //text
            TextStyle style = new TextStyle("Times.ttf");
            //TextStyle style = TextStyle.Default;
            MText mText = new MText(new Vector3(3,2,0), 1.0f, 100.0f, style);
            mText.Layer = new Layer("Multiline Text");
            //mText.Layer.Color.Index = 8;
            mText.Rotation = 0;
            mText.LineSpacingFactor = 1.0;
            mText.ParagraphHeightFactor = 1.0;

            //mText.AttachmentPoint = MTextAttachmentPoint.TopCenter;
            //mText.Write("Hello World!");
            //mText.Write(" we keep writting on the same line.");
            //mText.WriteLine("This text is in a new line");

            //mText.Write("Hello World! ");
            //for (int i = 0; i < 50; i++)
            //{
            //    mText.Write("1234567890");
            //}
            //mText.Write(" This text is over the limit of the 250 character chunk");
            //mText.NewParagraph();
            //mText.Write("This is a text in a new paragraph");
            //mText.Write(" and we continue writing in the previous paragraph");
            //mText.NewParagraph();
            MTextFormattingOptions options = new MTextFormattingOptions(mText.Style);
            options.Bold = true;
            mText.Write("Bold text in mText.Style", options);
            mText.EndParagraph();
            options.Italic = true;
            mText.Write("Bold and italic text in mText.Style", options);
            mText.EndParagraph();
            options.Bold = false;
            options.FontName = "Arial";
            options.Color = AciColor.Blue;
            mText.ParagraphHeightFactor = 2;
            mText.Write("Italic text in Arial", options);
            mText.EndParagraph();
            options.Italic = false;
            options.Color = null; // back to the default text color
            mText.Write("Normal text in Arial with the default paragraph height factor", options);
            mText.EndParagraph();
            mText.ParagraphHeightFactor = 1;
            mText.Write("No formatted text uses mText.Style");
            mText.Write(" and the text continues in the same paragraph.");
            mText.EndParagraph();

            //options.HeightPercentage = 2.5;
            //options.Color = AciColor.Red;
            //options.Overstrike = true;
            //options.Underline = true;
            //options.FontFile = "times.ttf";
            //options.ObliqueAngle = 15;
            //options.CharacterSpacePercentage = 2.35;
            //options.WidthFactor = 1.8;
            
            //for unknown reasons the aligment doesn't seem to change anything
            //mText.Write("Formatted text", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Center;
            //mText.Write("Center", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Top;
            //mText.Write("Top", options);
            //options.Aligment = MTextFormattingOptions.TextAligment.Bottom;
            //mText.Write("Bottom", options);

            mText.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata}
                             };
            
            dxf.AddEntity(mText);

            dxf.Save("MText sample.dxf");

        }
        private static void HatchCircleBoundary()
        {
            DxfDocument dxf = new DxfDocument();

            // create a circle that will be our hatch boundary in this case it is a circle with center (5.5, -5.5, 0.0) and a radius 10.0
            Circle circle = new Circle(new Vector3(5.5, -5.5, 0), 10);

            // create the hatch boundary path with only the circle (a circle is already a closed loop it is all we need to define a valid boundary path)
            // a hatch can have many boundaries (closed loops) and every boundary path can be made of several entities (lines, polylines, arcs, circles and ellipses)
            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>
                                                        {
                                                            new HatchBoundaryPath(new List<EntityObject>{circle})
                                                        };  

            // create the hatch in this case we will use the predefined Solid hatch pattern and our circle as the boundary path
            Hatch hatch = new Hatch(HatchPattern.Solid, boundary);

            // to give a color to the hatch, we have to options:

            // create a new layer with a color for the hatch (in this case by default the hatch will have a ByLayer color)
            //Layer hatchLayer = new Layer("HathLayer") {Color = AciColor.Green};
            //hatch.Layer = hatchLayer;

            // or give the hatch a color just for it
            // old AutoCAD versions only had 255 colors (indexed color), now in AutoCAD you can use true colors (8 bits per channel) but at the moment this is not supported.
            // if you try to give r, g, b values to define a color it will be converted to an indexed color
            // (I haven't tested this code a lot, so errors might appear and the result might not be what you expected).
            hatch.Color = AciColor.Red;

            // the hatch by itself will not show the boundary, but we can use the same entity to show the limits of the hatch, adding it to the document 
            dxf.AddEntity(circle);

            // add the hatch to the document
            dxf.AddEntity(hatch);

            dxf.Save("circle solid fill.dxf");
        }
        private static void LineWidth()
        {
            // the line thickness works as expected, according to the AutoCAD way of doing things
            Line thickLine = new Line(new Vector3(0,10,0),  new Vector3(10,20,0));

            // when you assign a thickness to a line, the result is like a wall, it is like a 3d face whose vertexes are defined by the
            // start and end points of the line and the thickness along the normal of the line.
            thickLine.Thickness = 5;

            // maybe what you are trying to do is create a line with a width (something that we can read it as a line with thickness), the only way to do this is to create a polyline
            // the kind of result you will get if you give a width to a 2d polyline 
            // you can only give a width to a vertex of a Polyline or a LightweigthPolyline
            // I am planning to drop support to AutoCAD 12 dxf files, so to define a bidimensional polyline the only way will be to use lightweight polyline
            // (the Polyline class and the LightWeightPolyline are basically the same).
            LwPolyline widthLine = new LwPolyline();
            LwPolylineVertex startVertex = new LwPolylineVertex(new Vector2(0, 0));
            LwPolylineVertex endVertex = new LwPolylineVertex(new Vector2(10, 10));
            widthLine.Vertexes = new List<LwPolylineVertex> { startVertex, endVertex };

            // the easy way to give a constant width to a polyline, but you can also give a polyline width by vertex
            // there is a mistake on my part, following the AutoCAD documentation I should have called the PolylineVertex.StartThickness and PolylineVertex.EndThickness as
            // PolylineVertex.StartWidth and PolylineVertex.EndWidth
            // SetConstantWidth is a sort cut that will asign the given value to every start width and end width of every vertex of the polyline
            widthLine.SetConstantWidth(0.5);

            DxfDocument dxf = new DxfDocument();

            // add the entities to the document (both of them to see the difference)
            dxf.AddEntity(thickLine);
            dxf.AddEntity(widthLine);

            dxf.Save("line width.dxf");

        }
        private static void ToPolyline()
        {
            DxfDocument dxf = new DxfDocument();

            Vector3 center = new Vector3(1, 8, -7);
            Vector3 normal = new Vector3(1, 1, 1);

            Circle circle = new Circle(center, 7.5);
            circle.Normal = normal;

            Arc arc = new Arc(center, 5, -45, 45);
            arc.Normal = normal;

            Ellipse ellipse = new Ellipse(center, 15, 7.5);
            ellipse.Rotation = 35;
            ellipse.Normal = normal;

            Ellipse ellipseArc = new Ellipse(center, 10, 5);
            ellipseArc.StartAngle = 315;
            ellipseArc.EndAngle = 45;
            ellipseArc.Rotation = 35;
            ellipseArc.Normal = normal;

            dxf.AddEntity(circle);
            dxf.AddEntity(circle.ToPolyline(10));

            dxf.AddEntity(arc);
            dxf.AddEntity(arc.ToPolyline(10));

            dxf.AddEntity(ellipse);
            dxf.AddEntity(ellipse.ToPolyline(10));

            dxf.AddEntity(ellipseArc);
            dxf.AddEntity(ellipseArc.ToPolyline(10));

            dxf.Save("to polyline.dxf");

            dxf = DxfDocument.Load("to polyline.dxf");

            dxf.Save("to polyline2.dxf");
        }
        private static void CustomHatchPattern()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, 5));
            poly2.Vertexes.Add(new LwPolylineVertex(-5, 5));
            poly2.Vertexes[1].Bulge = -0.25;
            poly2.IsClosed = true;

            LwPolyline poly3 = new LwPolyline();
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -6));
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -6));
            poly3.IsClosed = true;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly3}),
                                                                          };

            HatchPattern pattern = new HatchPattern("MyPattern", "A custom hatch pattern");

            HatchPatternLineDefinition line1 = new HatchPatternLineDefinition();
            line1.Angle = 45;
            line1.Origin = Vector2.Zero;
            line1.Delta=new Vector2(4,4);
            line1.DashPattern.Add(12);
            line1.DashPattern.Add(-4);
            pattern.LineDefinitions.Add(line1);

            HatchPatternLineDefinition line2 = new HatchPatternLineDefinition();
            line2.Angle = 135;
            line2.Origin = new Vector2(2.828427125, 2.828427125);
            line2.Delta = new Vector2(4,-4);
            line2.DashPattern.Add(12);
            line2.DashPattern.Add(-4);
            pattern.LineDefinitions.Add(line2);

            Hatch hatch = new Hatch(pattern, boundary);
            hatch.Layer = new Layer("hatch")
            {
                Color = AciColor.Red,
                LineType = LineType.Continuous
            };
            hatch.Pattern.Angle = 0;
            hatch.Pattern.Scale = 1;
            dxf.AddEntity(poly);
            dxf.AddEntity(poly2);
            dxf.AddEntity(poly3);
            dxf.AddEntity(hatch);

            dxf.Save("hatchTest.dxf");
        }
        private static void FilesTest()
        {
            LineType lineType = LineType.FromFile("acad.lin", "ACAD_ISO15W100");
            HatchPattern hatch = HatchPattern.FromFile("acad.pat", "zigzag");

        }
        private static void LoadSaveHatchTest()
        {
            DxfDocument dxf = DxfDocument.Load("Hatch2.dxf");
            dxf.Save("HatchTest.dxf");
        }
        private static void ExplodeTest()
        {
            DxfDocument dxf = new DxfDocument();
            //polyline
            LwPolylineVertex polyVertex;
            List<LwPolylineVertex> polyVertexes = new List<LwPolylineVertex>();
            polyVertex = new LwPolylineVertex(new Vector2(-50, -23.5));
            polyVertex.Bulge = 1.33;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(34.8, -42.7));
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(65.3, 54.7));
            polyVertex.Bulge = -0.47;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(-48.2, 42.5));
            polyVertexes.Add(polyVertex);
            LwPolyline polyline2d = new LwPolyline(polyVertexes);
            polyline2d.Layer = new Layer("polyline2d");
            polyline2d.Layer.Color.Index = 5;
            polyline2d.Normal = new Vector3(1, 1, 1);
            polyline2d.Elevation = 100.0f;

            dxf.AddEntity(polyline2d);
            dxf.AddEntity(polyline2d.Explode());

            dxf.Save("explode.dxf");
        }
        private static void HatchTestLinesBoundary()
        {
            DxfDocument dxf = new DxfDocument();

            Line line1 = new Line(new Vector3(-10,-10,0),new Vector3(10,-10,0));
            Line line2 = new Line(new Vector3(10, -10, 0), new Vector3(10, 10, 0));
            Line line3 = new Line(new Vector3(10, 10, 0), new Vector3(-10, 10, 0));
            Line line4 = new Line(new Vector3(-10, 10, 0), new Vector3(-10, -10, 0));


            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{line1, line2, line3, line4})
                                                                          };
            Hatch hatch = new Hatch(HatchPattern.Line, boundary);
            hatch.Layer = new Layer("hatch")
            {
                Color = AciColor.Red,
                LineType = LineType.Dashed
            };
            hatch.Elevation = 52;
            hatch.Pattern.Angle = 45;
            hatch.Pattern.Scale = 10;
            hatch.Normal = new Vector3(1, 1, 1);

            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "netDxf hatch"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Distance, hatch.Pattern.Scale));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Real, hatch.Pattern.Angle));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            hatch.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                             };

            //dxf.AddEntity(line1);
            //dxf.AddEntity(line2);
            //dxf.AddEntity(line3);
            //dxf.AddEntity(line4);
            dxf.AddEntity(hatch);
            dxf.AddEntity(hatch.CreateWCSBoundary());

            dxf.Save("hatchTest.dxf");
        }
        private static void HatchTest1()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, -5));
            poly2.Vertexes.Add(new LwPolylineVertex(5, 5));
            poly2.Vertexes.Add(new LwPolylineVertex(-5, 5));
            poly2.Vertexes[1].Bulge = -0.25;
            poly2.IsClosed = true;

            LwPolyline poly3 = new LwPolyline();
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -8));
            poly3.Vertexes.Add(new LwPolylineVertex(-6, -6));
            poly3.Vertexes.Add(new LwPolylineVertex(-8, -6));
            poly3.IsClosed = true;

            Line line = new Line(new Vector2(-5, -5), new Vector2(5, -5));
            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{line, poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly3}),
                                                                          };
            Hatch hatch = new Hatch(HatchPattern.Net, boundary);
            hatch.Layer = new Layer("hatch")
                              {
                                  Color = AciColor.Red,
                                  LineType = LineType.Continuous
                              };
            hatch.Pattern.Angle = 30;
            hatch.Elevation = 52;
            hatch.Normal = new Vector3(1,1,0);
            hatch.Pattern.Scale = 1 / hatch.Pattern.LineDefinitions[0].Delta.Y;
            //dxf.AddEntity(poly);
            //dxf.AddEntity(poly2);
            //dxf.AddEntity(poly3);
            dxf.AddEntity(hatch);
            dxf.AddEntity(hatch.CreateWCSBoundary());

            dxf.Save("hatchTest1.dxf");
            dxf = DxfDocument.Load("hatchTest1.dxf");
        }
        private static void HatchTest2()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            Circle circle = new Circle(Vector3.Zero, 5);

            Ellipse ellipse = new Ellipse(Vector3.Zero,16,10);
            ellipse.Rotation = 30;
            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{circle}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{ellipse})
                                                                          };

            Hatch hatch = new Hatch(HatchPattern.Line, boundary);
            hatch.Pattern.Angle = 150;
            hatch.Pattern.Scale = 5;
            hatch.Normal = new Vector3(1,1,1);
            hatch.Elevation = 23;
            //dxf.AddEntity(poly);
            //dxf.AddEntity(circle);
            //dxf.AddEntity(ellipse);
            dxf.AddEntity(hatch);
            dxf.AddEntity(hatch.CreateWCSBoundary());
            dxf.Save("hatchTest2.dxf");
            dxf = DxfDocument.Load("hatchTest2.dxf");
            dxf.Save("hatchTest2 copy.dxf");
        }
        private static void HatchTest3()
        {
            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.Vertexes[2].Bulge = 1;
            poly.IsClosed = true;

            Ellipse ellipse = new Ellipse(Vector3.Zero, 16, 10);
            ellipse.Rotation = 0;
            ellipse.StartAngle = 0;
            ellipse.EndAngle = 180;

            LwPolyline poly2 = new LwPolyline();
            poly2.Vertexes.Add(new LwPolylineVertex(-8, 0));
            poly2.Vertexes.Add(new LwPolylineVertex(0, -4));
            poly2.Vertexes.Add(new LwPolylineVertex(8, 0));

            Arc arc = new Arc(Vector3.Zero,8,180,0);
            Line line =new Line(new Vector3(8,0,0), new Vector3(-8,0,0));

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>{
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly}),
                                                                            new HatchBoundaryPath(new List<EntityObject>{poly2, ellipse})
                                                                          };

            Hatch hatch = new Hatch(HatchPattern.Line, boundary);
            hatch.Pattern.Angle = 45;
            dxf.AddEntity(poly);
            dxf.AddEntity(ellipse);
            //dxf.AddEntity(arc);
            //dxf.AddEntity(line);
            dxf.AddEntity(poly2);
            dxf.AddEntity(hatch);

            dxf.Save("hatchTest3.dxf");
        }
        private static void HatchTest4()
        {
            DxfDocument dxf = new DxfDocument(DxfVersion.AutoCad2010);

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(-10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, -10));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(-10, 10));
            poly.IsClosed = true;

            List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath>() { new HatchBoundaryPath(new List<EntityObject>()) }; ;
            //List<HatchBoundaryPath> boundary = new List<HatchBoundaryPath> {new HatchBoundaryPath(new List<EntityObject> {poly})};
            HatchGradientPattern pattern = new HatchGradientPattern(AciColor.Yellow, AciColor.Blue, HatchGradientPatternType.Linear);
            pattern.Origin = new Vector2(120, -365);
            Hatch hatch = new Hatch(pattern, boundary);
            dxf.AddEntity(hatch);
            dxf.AddEntity(hatch.CreateWCSBoundary());
            
            dxf.Save("HatchTest4.dxf");
            dxf = DxfDocument.Load("HatchTest4.dxf");
            dxf.Save("HatchTest4 copy.dxf");

        }
        private static void Dxf2000()
        {
            DxfDocument dxf = new DxfDocument();
           //line
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(5, 5, 5));
            line.Layer = new Layer("line");
            line.Layer.Color.Index = 6;

            dxf.AddEntity(line);

            dxf.Save("test2000.dxf");
        }
        private static void LwPolyline()
        {

            DxfDocument dxf = new DxfDocument();

            LwPolyline poly = new LwPolyline();
            poly.Vertexes.Add(new LwPolylineVertex(0, 0));
            poly.Vertexes.Add(new LwPolylineVertex(10, 10));
            poly.Vertexes.Add(new LwPolylineVertex(20, 0));
            poly.Vertexes.Add(new LwPolylineVertex(30, 10));
            poly.SetConstantWidth(2);
            //poly.IsClosed = true;
            dxf.AddEntity(poly);

            dxf.Save("lwpolyline.dxf");

            dxf = DxfDocument.Load("lwpolyline.dxf");
        }
        private static void Polyline()
        {
            DxfDocument dxf = new DxfDocument();
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            Polyline poly = new Polyline();
            poly.Vertexes.Add(new PolylineVertex(0, 0, 0));
            poly.Vertexes.Add(new PolylineVertex(10, 10, 0));
            poly.Vertexes.Add(new PolylineVertex(20, 0, 0));
            poly.Vertexes.Add(new PolylineVertex(30, 10, 0));
            dxf.AddEntity(poly);

            dxf.Save("polyline.dxf");

        }
        private static void Solid()
        {

            DxfDocument dxf = new DxfDocument();

            Solid solid = new Solid();
            solid.FirstVertex=new Vector3(0,0,0);
            solid.SecondVertex  = new Vector3(1, 0, 0);
            solid.ThirdVertex  = new Vector3(0, 1, 0);
            solid.FourthVertex  = new Vector3(1, 1, 0);
            dxf.AddEntity(solid);

            dxf.Save("solid.dxf");
            //dxf = DxfDocument.Load("solid.dxf");
            //dxf.Save("solid.dxf");

        }
        private static void Face3d()
        {

            DxfDocument dxf = new DxfDocument();

            Face3d face3d = new Face3d();
            face3d.FirstVertex = new Vector3(0, 0, 0);
            face3d.SecondVertex = new Vector3(1, 0, 0);
            face3d.ThirdVertex = new Vector3(1, 1, 0);
            face3d.FourthVertex = new Vector3(0, 1, 0);
            dxf.AddEntity(face3d);

            dxf.Save("face.dxf");
            dxf = DxfDocument.Load("face.dxf");
            dxf.Save("face return.dxf");

        }
        private static void Ellipse()
        {
           
            DxfDocument dxf = new DxfDocument();

            //Line line = new Line(new Vector3(0, 0, 0), new Vector3(2 * Math.Cos(Math.PI / 4),2 * Math.Cos(Math.PI / 4), 0));

            //dxf.AddEntity(line);

            //Line line2 = new Line(new Vector3(0, 0, 0), new Vector3(0, -2, 0));
            //dxf.AddEntity(line2);

            //Arc arc=new Arc(Vector3.Zero,2,45,270);
            //dxf.AddEntity(arc);

            Ellipse ellipse = new Ellipse(new Vector3(2,2,0), 5,3);
            ellipse.Rotation = 30;
            ellipse.Normal=new Vector3(1,1,1);
            ellipse.Thickness = 2;
            dxf.AddEntity(ellipse);

            Ellipse ellipseArc = new Ellipse(new Vector3(2, 10, 0), 5, 3);
            ellipseArc.StartAngle = -45;
            ellipseArc.EndAngle = 45;
            dxf.AddEntity(ellipseArc);

            dxf.Save("ellipse.dxf");
            dxf = new DxfDocument();
            dxf = DxfDocument.Load("ellipse.dxf");

            DxfDocument load = DxfDocument.Load("test ellipse.dxf");
            load.Save("saved test ellipse.dxf");

        }
        private static void SpeedTest()
        {
            Stopwatch crono = new Stopwatch();
            const int numLines = (int)1e6; // create # lines
            string layerName = "MyLayer";
            float totalTime=0;
            
            List<EntityObject> lines = new List<EntityObject>(numLines);
            DxfDocument dxf = new DxfDocument();

            crono.Start();
            for (int i = 0; i < numLines; i++)
            {
                 //line
                Line line = new Line(new Vector3(0, i, 0), new Vector3(5, i, 0));
                line.Layer = new Layer(layerName);
                line.Layer.Color.Index = 6;
                lines.Add(line);
            }
            Console.WriteLine("Time creating entities : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            crono.Start();
            dxf.AddEntity(lines);
            Console.WriteLine("Time adding entities to document : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            //crono.Start();
            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            //dxf.Save("speedtest (netDxf 2000).dxf");
            //Console.WriteLine("Time saving file 2000 : " + crono.ElapsedMilliseconds / 1000.0f);
            //totalTime += crono.ElapsedMilliseconds;
            //crono.Reset();

            crono.Start();
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("speedtest (binary netDxf 2000).dxf", true);
            Console.WriteLine("Time saving binary file 2000 : " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Reset();

            //crono.Start();
            //dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            //dxf.Save("speedtest (netDxf 2010).dxf");
            //Console.WriteLine("Time saving file 2010 : " + crono.ElapsedMilliseconds / 1000.0f);
            //totalTime += crono.ElapsedMilliseconds;
            //crono.Reset();


            //crono.Start();
            //dxf = DxfDocument.Load("speedtest (netDxf 2000).dxf");
            //Console.WriteLine("Time loading file 2000: " + crono.ElapsedMilliseconds / 1000.0f);
            //totalTime += crono.ElapsedMilliseconds;
            //crono.Stop();
            //crono.Reset();

            crono.Start();
            dxf = DxfDocument.Load("speedtest (binary netDxf 2000).dxf");
            Console.WriteLine("Time loading binary file 2000: " + crono.ElapsedMilliseconds / 1000.0f);
            totalTime += crono.ElapsedMilliseconds;
            crono.Stop();
            crono.Reset();

            //crono.Start();
            //dxf = DxfDocument.Load("speedtest (netDxf 2010).dxf");
            //Console.WriteLine("Time loading file 2010: " + crono.ElapsedMilliseconds / 1000.0f);
            //totalTime += crono.ElapsedMilliseconds;
            //crono.Stop();
            //crono.Reset();

            Console.WriteLine("Total time : " + totalTime / 1000.0f);
            Console.ReadLine();

        }
        private static void WriteNestedInsert()
        {
            // nested blocks
            DxfDocument dxf = new DxfDocument();
            
            Block nestedBlock = new Block("Nested block");
            Circle circle = new Circle(Vector3.Zero, 5);
            circle.Layer = new Layer("circle");
            circle.Layer.Color.Index = 2;
            nestedBlock.Entities.Add(circle);
            
            AttributeDefinition attdef = new AttributeDefinition("NewAttribute");
            attdef.Text = "InfoText";
            attdef.Alignment = TextAlignment.MiddleCenter;
            nestedBlock.AttributeDefinitions.Add(attdef);

            Insert nestedInsert = new Insert(nestedBlock, new Vector3(0, 0, 0)); // the position will be relative to the position of the insert that nest it
            nestedInsert.Attributes[attdef.Tag].Value = 24;

            Insert nestedInsert2 = new Insert(nestedBlock, new Vector3(-20, 0, 0)); // the position will be relative to the position of the insert that nest it
            nestedInsert2.Attributes[attdef.Tag].Value = -20;

            Block block = new Block("MyBlock");
            block.Entities.Add(new Line(new Vector3(-5, -5, 0), new Vector3(5, 5, 0)));
            block.Entities.Add(new Line(new Vector3(5, -5, 0), new Vector3(-5, 5, 0)));
            block.Entities.Add(nestedInsert);
            block.Entities.Add(nestedInsert2);

            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");

            dxf.AddEntity(insert);
            //dxf.AddEntity(circle); // this is not allowed the circle is already part of a block

            dxf.Save("nested insert.dxf");
            dxf = DxfDocument.Load("nested insert.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("nested insert copy.dxf");

        }
        private static void WritePolyfaceMesh()
        {
            DxfDocument dxf = new DxfDocument();


            List<PolyfaceMeshVertex> vertexes = new List<PolyfaceMeshVertex>
                                                    {
                                                        new PolyfaceMeshVertex(0, 0, 0),
                                                        new PolyfaceMeshVertex(10, 0, 0),
                                                        new PolyfaceMeshVertex(10, 10, 0),
                                                        new PolyfaceMeshVertex(5, 15, 0),
                                                        new PolyfaceMeshVertex(0, 10, 0)
                                                    };
            List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>
                                               {
                                                   new PolyfaceMeshFace(new short[] {1, 2, -3}),
                                                   new PolyfaceMeshFace(new short[] {-1, 3, -4}),
                                                   new PolyfaceMeshFace(new short[] {-1, 4, 5})
                                               };

            PolyfaceMesh mesh = new PolyfaceMesh(vertexes, faces);
            dxf.AddEntity(mesh);

            dxf.Save("mesh.dxf");
        }
        private static void WriteDxfFile()
        {
            DxfDocument dxf = new DxfDocument();

            //arc
            Arc arc = new Arc(new Vector3(10, 10, 0), 10, 45, 135);
            arc.Layer = new Layer("arc");
            arc.Layer.Color.Index = 1;
            dxf.AddEntity(arc);

            //xData sample
            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionX, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionY, 0));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.WorldSpacePositionZ, 0));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            XData xdata2 = new XData(new ApplicationRegistry("other application"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata2.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.String, "string record"));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Real, 15.5));
            xdata2.XDataRecord.Add(new XDataRecord(XDataCode.Int32, 350));
            xdata2.XDataRecord.Add(XDataRecord.CloseControlString);

            //circle
            Vector3 extrusion = new Vector3(1, 1, 1);
            Vector3 centerWCS = new Vector3(1, 1, 1);
            Vector3 centerOCS = MathHelper.Transform(centerWCS,
                                                      extrusion,
                                                      MathHelper.CoordinateSystem.World,
                                                      MathHelper.CoordinateSystem.Object);

            Circle circle = new Circle(centerOCS, 5);
            circle.Layer = new Layer("circle with spaces");
            circle.Layer.Color=AciColor.Yellow;
            circle.LineType = LineType.Dashed;
            circle.Normal = extrusion;
            circle.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                                 {xdata2.ApplicationRegistry.Name, xdata2}
                             };

            dxf.AddEntity(circle);

            //points
            Point point1 = new Point(new Vector3(-3, -3, 0));
            point1.Layer = new Layer("point");
            point1.Color = new AciColor(30);
            Point point2 = new Point(new Vector3(1, 1, 1));
            point2.Layer = point1.Layer;
            point2.Layer.Color.Index = 9;
            point2.Normal = new Vector3(1, 1, 1);
            dxf.AddEntity(point1);
            dxf.AddEntity(point2);

            //3dface
            Face3d face3D = new Face3d(new Vector3(-5, -5, 5),
                                       new Vector3(5, -5, 5),
                                       new Vector3(5, 5, 5),
                                       new Vector3(-5, 5, 5));
            face3D.Layer = new Layer("3dface");
            face3D.Layer.Color.Index = 3;
            dxf.AddEntity(face3D);
            
            //polyline
            LwPolylineVertex polyVertex;
            List<LwPolylineVertex> polyVertexes = new List<LwPolylineVertex>();
            polyVertex = new LwPolylineVertex(new Vector2(-50, -50));
            polyVertex.BeginWidth = 2;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(50, -50));
            polyVertex.BeginWidth = 1;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(50, 50));
            polyVertex.Bulge = 1;
            polyVertexes.Add(polyVertex);
            polyVertex = new LwPolylineVertex(new Vector2(-50, 50));
            polyVertexes.Add(polyVertex);
            LwPolyline polyline2d = new LwPolyline(polyVertexes, true);
            polyline2d.Layer = new Layer("polyline2d");
            polyline2d.Layer.Color.Index = 5;
            polyline2d.Normal = new Vector3(1, 1, 1);
            polyline2d.Elevation = 100.0f;
            dxf.AddEntity(polyline2d);

            //lightweight polyline
            LwPolylineVertex lwVertex;
            List<LwPolylineVertex> lwVertexes = new List<LwPolylineVertex>();
            lwVertex = new LwPolylineVertex(new Vector2(-25, -25));
            lwVertex.BeginWidth = 2;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(25, -25));
            lwVertex.BeginWidth = 1;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(25, 25));
            lwVertex.Bulge = 1;
            lwVertexes.Add(lwVertex);
            lwVertex = new LwPolylineVertex(new Vector2(-25, 25));
            lwVertexes.Add(lwVertex);
            LwPolyline lwPolyline = new LwPolyline(lwVertexes, true);
            lwPolyline.Layer = new Layer("lwpolyline");
            lwPolyline.Layer.Color.Index = 5;
            lwPolyline.Normal = new Vector3(1, 1, 1);
            lwPolyline.Elevation = 100.0f;
            dxf.AddEntity(lwPolyline);

            // polyfaceMesh
            List<PolyfaceMeshVertex> meshVertexes = new List<PolyfaceMeshVertex>
                                                    {
                                                        new PolyfaceMeshVertex(0, 0, 0),
                                                        new PolyfaceMeshVertex(10, 0, 0),
                                                        new PolyfaceMeshVertex(10, 10, 0),
                                                        new PolyfaceMeshVertex(5, 15, 0),
                                                        new PolyfaceMeshVertex(0, 10, 0)
                                                    };
            List<PolyfaceMeshFace> faces = new List<PolyfaceMeshFace>
                                               {
                                                   new PolyfaceMeshFace(new short[] {1, 2, -3}),
                                                   new PolyfaceMeshFace(new short[] {-1, 3, -4}),
                                                   new PolyfaceMeshFace(new short[] {-1, 4, 5})
                                               };

            PolyfaceMesh mesh = new PolyfaceMesh(meshVertexes, faces);
            mesh.Layer = new Layer("polyfacemesh");
            mesh.Layer.Color.Index = 104;
            dxf.AddEntity(mesh);

            //line
            Line line = new Line(new Vector3(0, 0, 0), new Vector3(10, 10, 10));
            line.Layer = new Layer("line");
            line.Layer.Color.Index = 6;
            dxf.AddEntity(line);

            //3d polyline
            PolylineVertex vertex;
            List<PolylineVertex> vertexes = new List<PolylineVertex>();
            vertex = new PolylineVertex(new Vector3(-50, -50, 0));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(50, -50, 10));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(50, 50, 25));
            vertexes.Add(vertex);
            vertex = new PolylineVertex(new Vector3(-50, 50, 50));
            vertexes.Add(vertex);
            Polyline polyline = new Polyline(vertexes, true);
            polyline.Layer = new Layer("polyline3d");
            polyline.Layer.Color.Index = 24;
            dxf.AddEntity(polyline);

            //block definition
            Block block = new Block("TestBlock");
            block.Entities.Add(new Line(new Vector3(-5, -5, 5), new Vector3(5, 5, 5)));
            block.Entities.Add(new Line(new Vector3(5, -5, 5), new Vector3(-5, 5, 5)));
           
            //insert
            Insert insert = new Insert(block, new Vector3(5, 5, 5));
            insert.Layer = new Layer("insert");
            insert.Layer.Color.Index = 4;
            dxf.AddEntity(insert);

            //text
            TextStyle style=new TextStyle("True type font","Arial.ttf");
            Text text = new Text("Hello world!", Vector3.Zero, 10.0f,style);
            text.Layer = new Layer("text");
            text.Layer.Color.Index = 8;
            text.Alignment = TextAlignment.TopRight;
            dxf.AddEntity(text);

            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2010;
            dxf.Save("AutoCad2010.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2007;
            dxf.Save("AutoCad2007.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2004;
            dxf.Save("AutoCad2004.dxf");
            dxf.DrawingVariables.AcadVer = DxfVersion.AutoCad2000;
            dxf.Save("AutoCad2000.dxf");
            dxf = DxfDocument.Load("AutoCad2000.dxf");
            dxf.Save("AutoCad2000 result.dxf");
        }
        private static void WritePolyline3d()
        {
            DxfDocument dxf = new DxfDocument();

            List<PolylineVertex> vertexes = new List<PolylineVertex>{
                                                                        new PolylineVertex(0, 0, 0), 
                                                                        new PolylineVertex(10, 0, 10), 
                                                                        new PolylineVertex(10, 10, 20), 
                                                                        new PolylineVertex(0, 10, 30)
                                                                        };

            Polyline poly = new Polyline(vertexes, true);

            XData xdata = new XData(new ApplicationRegistry("netDxf"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "extended data with netDxf"));
            xdata.XDataRecord.Add(XDataRecord.OpenControlString);
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.String, "netDxf polyline3d"));
            xdata.XDataRecord.Add(new XDataRecord(XDataCode.Int16, poly.Vertexes.Count));
            xdata.XDataRecord.Add(XDataRecord.CloseControlString);

            poly.XData = new Dictionary<string, XData>(StringComparer.OrdinalIgnoreCase)
                             {
                                 {xdata.ApplicationRegistry.Name, xdata},
                             }; 
            dxf.AddEntity(poly);

            dxf.Save("polyline.dxf");

            
        }
    }
}