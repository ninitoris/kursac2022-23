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
	using System.Windows.Shapes;

	public class MyPlugin
	{
		[CommandMethod("runForm", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
		public static void myForm()
		{
			PluginForm frm = new PluginForm();
			frm.Show();
		}

		public static void ExecuteBuilder(
            float D_n,
            float d,
            float d1,
            float d2,
            float d3,
            float D_thread,
            float DD1,
            float DD3,
            float S,
            float L,
            float l1_nom,
            float l1_otkl,
            float l2,
            float l3,
            float LL,
            float h,
            float MASS,
			string gost)
		{
			McDocument.GetDocument(McDocumentsManager.GetActiveDoc().ID).Close();
			McDocument.CreateDocument();
            var doc = McDocumentsManager.GetActiveDoc();

            // var newSheet = McDocumentsManager.
            // var doc = McDocumentsManager.GetActiveDoc();

            // Create empty solid and add it to the document
            Mc3dSolid solid = new Mc3dSolid();
			McObjectManager.Add2Document(solid.DbEntity, doc);
			solid.DbEntity.AddToCurrentDocument();

			// Create base profile for the sketch
			PlanarSketch planarSketch1 = solid.AddPlanarSketch();

            DbPolyline polyline = new DbPolyline() {
				Polyline = CreatePolyline(D_n, d, d1, d2, d3, D_thread, DD1, DD3, S, L, l1_nom, l1_otkl, l2, l3, LL, h, MASS, gost)
            };

			polyline.DbEntity.AddToCurrentDocument();
			planarSketch1.AddObject(polyline.ID);
			SketchProfile sketchProfile1 = planarSketch1.CreateProfile();
			if (sketchProfile1 == null)
			{
				MessageBox.Show("sketchProfile1 == null");
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
					new Point3d(0, DD1 / 2, 0),
					new Point3d(DD1 * Math.Sqrt(3) / 4, DD1 / 4, 0),
					new Point3d(DD1 * Math.Sqrt(3) / 4, - DD1 / 4, 0),
					new Point3d(0, - DD1 / 2, 0),
					new Point3d(- DD1 * Math.Sqrt(3) / 4, - DD1 / 4, 0),
					new Point3d(- DD1 * Math.Sqrt(3) / 4, DD1 / 4, 0),
					new Point3d(0, DD1 / 2, 0),
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
			MessageBox.Show("Построение выполнено");
		}

		[CommandMethod("asdf_command", CommandFlags.NoCheck | CommandFlags.NoPrefix)]
		public static void AsdfCommandHadler()
		{
			McContext.ExecuteCommand("cmd_loft");
		}

		private static Polyline3d CreatePolyline(
            float D_n,
            float d,
            float d1, 
			float d2,
            float d3,
            float D_thread, 
			float DD1, 
			float DD3, 
			float S, 
			float L, 
			float l1_nom, 
			float l1_otkl, 
			float l2, 
			float l3, 
			float LL, 
			float h,
            float MASS,
			string gost)
        {
			if (gost == "44")
			{
                // построение по ГОСТ 16044
                Polyline3d polyline3d = new Polyline3d(new List<Point3d>() {
						new Point3d(1, D_thread / 2, 0),
						new Point3d(L - 2, D_thread / 2, 0),
						// фаска
						new Point3d(L - 1, d1 / 2, 0),
						new Point3d(L, d1 / 2, 0),

						// гайка
						new Point3d(L, DD1 * Math.Sqrt(3) / 4, 0),
						new Point3d(L + (Math.Tan(10 * Math.PI / 180) * (DD1 / 2 - DD1 * Math.Sqrt(3) / 4)), DD1 / 2, 0),
						new Point3d(L + h - (Math.Tan(10 * Math.PI / 180) * (DD1 / 2 - DD1 * Math.Sqrt(3) / 4)), DD1 / 2, 0),
						new Point3d(L + h, DD1 * Math.Sqrt(3) / 4, 0),


						new Point3d(L + h, DD3 / 2 + 1, 0),
						new Point3d(L + h, DD3 / 2, 0),		// тут сопряжение
						new Point3d(L + h + 1, DD3 / 2, 0),

						new Point3d(LL, DD3 / 2, 0),
						new Point3d(LL, d2 / 2 + 0.5, 0),
						new Point3d(LL - 0.5, d2 / 2, 0),
						new Point3d(LL - l1_nom + 1, d2 / 2, 0),
						new Point3d(LL - l1_nom, d2 / 2, 0), // тут еще сопряжение
						new Point3d(LL - l1_nom, d / 2, 0),

						new Point3d((d1 / 2 - d / 2 ) * 3 / Math.Sqrt(3), d / 2, 0),
						new Point3d(0, d1 / 2, 0),
						new Point3d(0, D_thread / 2 - 1, 0),
                        new Point3d(1, D_thread / 2, 0)
                 });

				polyline3d.Vertices.MakeFilletAtVertex(9, 1);
				polyline3d.Vertices.MakeFilletAtVertex(16, 1);

				return polyline3d;
            } else
			{
                // построение по ГОСТ 16045
                Polyline3d polyline3d = new Polyline3d(new List<Point3d>() {
                        new Point3d(1, D_thread / 2, 0),
                        new Point3d(L - 2, D_thread / 2, 0),
						// фаска
						new Point3d(L - 1, d1 / 2, 0),
                        new Point3d(L, d1 / 2, 0),

						// гайка
						new Point3d(L, DD1 * Math.Sqrt(3) / 4, 0),
                        new Point3d(L + (Math.Tan(10 * Math.PI / 180) * (DD1 / 2 - DD1 * Math.Sqrt(3) / 4)), DD1 / 2, 0),
                        new Point3d(L + h - (Math.Tan(10 * Math.PI / 180) * (DD1 / 2 - DD1 * Math.Sqrt(3) / 4)), DD1 / 2, 0),
                        new Point3d(L + h, DD1 * Math.Sqrt(3) / 4, 0),


                        new Point3d(L + h, DD3 / 2 + 1, 0),
                        new Point3d(L + h, DD3 / 2, 0),			// тут сопряжение
						new Point3d(L + h + 1, DD3 / 2, 0),

                        new Point3d(LL, DD3 / 2, 0),
                        new Point3d(LL, d / 2, 0),

                        new Point3d((d1 / 2 - d / 2 ) * 3 / Math.Sqrt(3), d / 2, 0),
                        new Point3d(0, d1 / 2, 0),
                        new Point3d(0, D_thread / 2 - 1, 0),
                        new Point3d(1, D_thread / 2, 0)
                 });

                polyline3d.Vertices.MakeFilletAtVertex(9, 1);

                return polyline3d;
            }

        }
    }
}
