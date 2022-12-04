namespace test1
{
	using Multicad.AplicationServices;
	using Multicad.DatabaseServices;
	using Multicad.DatabaseServices.StandardObjects;
	using Multicad.Geometry;
	using Multicad.Mc3D;
	using Multicad.Runtime;
	using System.Collections.Generic;
	using System.Windows.Forms;
	using System;

	public class MyPlugin
	{
		[CommandMethod("runForm", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
		public static void myForm()
		{
			PluginForm frm = new PluginForm();
			frm.Show();
		}

		public static void cmd_Loft()
		{
			//Document doc = Application.DocumentManager.MdiActiveDocument;
			//doc.SendStringToExecute("cmd_loft", true, true, true);
			//Editor ed = doc.Editor;
			//ed.Command("cmd_loft");
			McContext.ExecuteCommand("cmd_loft");
		}
		[CommandMethod("cmd_loft", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
		public static void Sample3dLoft()
		{
			var activeSheet = McDocumentsManager.GetActiveSheet();
			// var doc = McDocumentsManager.GetActiveDoc();

			// Create empty solid and add it to the document
			Mc3dSolid solid = new Mc3dSolid();
			McObjectManager.Add2Document(solid.DbEntity, activeSheet);
			solid.DbEntity.AddToCurrentDocument();

			// Create base profile for the sketch
			PlanarSketch planarSketch1 = solid.AddPlanarSketch();
			Polyline3d polyline3d = new Polyline3d(new List<Point3d>() {
				new Point3d(0, 50, 0),
				new Point3d(60, 50, 0),
				// фаска
				new Point3d(64, 45, 0),

				// шестерня
				new Point3d(70, 45, 0),
				new Point3d(70, 60, 0),
				new Point3d(72, 65, 0),
				new Point3d(83, 65, 0),
				new Point3d(85, 60, 0),

				
				new Point3d(85, 45, 0),
				new Point3d(85, 40, 0), // тут сопряжение
				new Point3d(90, 40, 0),

				new Point3d(120, 40, 0),
				new Point3d(120, 27, 0),
				new Point3d(30, 27, 0),
				new Point3d(0, 43, 0),
				new Point3d(0, 50, 0)
			});

			polyline3d.Vertices.MakeFilletAtVertex(9, 5);

			DbPolyline polyline = new DbPolyline() {
				Polyline = polyline3d
			};

			polyline.DbEntity.AddToCurrentDocument();
			planarSketch1.AddObject(polyline.ID);
			SketchProfile sketchProfile1 = planarSketch1.CreateProfile();
			if (sketchProfile1 == null)
			{
				MessageBox.Show("ПАешь гавна ");
				return;
			}
			sketchProfile1.AutoProcessExternalContours();
			DbLine axis = new DbLine() { 
				Line = new LineSeg3d(new Point3d(0, 0, 0), new Point3d(1, 0, 0)) 
			};
			axis.DbEntity.AddToCurrentDocument();
			McGeomParam axisGP = new McGeomParam() { ID = axis.ID };
			RevolveFeature revolveFeature = solid.AddRevolveFeature(sketchProfile1.ID, axisGP, Math.PI * 2);
			if(revolveFeature == null)
			{
				MessageBox.Show("revolveFeature is null");
			}

			McObjectManager.UpdateAll();

			// новый эскиз для обрезания гайки
			PlanarSketch planarSketch2 = solid.AddPlanarSketch();

			DbPolyline hexagon = new DbPolyline()
			{
				Polyline = new Polyline3d(new List<Point3d>() {
					new Point3d(100, 55, 0),
					new Point3d(-100, 55, 0),
					new Point3d(-100, -55, 0),
					new Point3d(100, -55, 0),
					new Point3d(100, 55, 0)
				})
			};

			hexagon.DbEntity.AddToCurrentDocument();
			planarSketch2.AddObject(hexagon.ID);

			// новая YZ плоскость для эскиза
			Plane3d plane3D = new Plane3d(new Point3d(0, 0, 0), new Vector3d(0, 1, 0), new Vector3d(0, 0, 1));
			// задаем плоскость для эскиза, теперь он в плоскости YZ
			planarSketch2.SetPlane(plane3D);
			
			SketchProfile sketchProfile2 = planarSketch2.CreateProfile();
			if(sketchProfile2 == null)
            {
				MessageBox.Show("sketchProfile2 == null");
				return;
            }
			sketchProfile2.AutoProcessExternalContours();

			ExtrudeFeature EF2 = solid.AddExtrudeFeature(
				sketchProfile2.ID,
				1000,
				0,
				FeatureExtentDirection.Positive);
			EF2.Operation = PartFeatureOperation.Intersect;

			// скрыть эскизы
			sketchProfile1.DbEntity.Visibility = 0;
			planarSketch1.DbEntity.Visibility = 0;
			planarSketch2.DbEntity.Visibility = 0;
			sketchProfile2.DbEntity.Visibility = 0;

			McObjectManager.UpdateAll();


			/*
			DbCircle circle = new DbCircle()
			{
				Center = Point3d.Origin,
				Radius = DataEvent.paramRadius
			};
			circle.DbEntity.AddToCurrentDocument();
			sketch.AddObject(circle.ID);
			SketchProfile profile = sketch.CreateProfile();
			if (profile == null)
			{
				MessageBox.Show("Gfitk yf[eq t,kfy t,fysq z ndj. vfnm t,fk ehjl ueq!!!");
				return;
			}
			
			profile.AutoProcessExternalContours();
			// Создаем дополнительное сечение.
			// Add additional section
			DbCircle section = new DbCircle()
			{
				Center = new Point3d(10, 20, 130),
				Radius = 300
			};
			section.DbEntity.AddToCurrentDocument();
			McGeomParam sectionGP = new McGeomParam() { ID = section.ID };
			// Создаем вытягивание по умолчанию.
			// Create default loft 
			LoftFeature loft = solid.AddLoftFeature(profile.ID, new McGeomParam[] { sectionGP });
			// Использовать центровые линии.
			// Use of center lines
			DbCircArc dbArc = new DbCircArc()
			{
				Arc = new CircArc3d(
					  new Point3d(30, 0, 0),
					  new Point3d(70, 0, 80),
					  new Point3d(20, 20, 130)
				)
			};
			dbArc.DbEntity.AddToCurrentDocument();
			McGeomParam gpArc = new McGeomParam() { ID = dbArc.ID };
			loft.LoftType = LoftType.WithCenterLine;
			loft.Path = gpArc;

			solid = null;
			// RevolveFeature revolveFeature = solid.AddRevolveFeature(profile.ID, )
			*/
			MessageBox.Show("На ешь блять ");
		}

		[CommandMethod("asdf_command", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
		public static void AsdfCommandHadler()
		{
			McContext.ExecuteCommand("cmd_loft");
		}
	}
}
