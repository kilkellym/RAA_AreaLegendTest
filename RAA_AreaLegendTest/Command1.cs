
namespace RAA_AreaLegendTest
{
	[Transaction(TransactionMode.Manual)]
	public class Command1 : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			// this is a variable for the Revit application
			UIApplication uiapp = commandData.Application;

			// this is a variable for the current Revit model
			Document doc = uiapp.ActiveUIDocument.Document;

			// Your code goes here
			View curView = doc.ActiveView;

			// get color fill scheme
			ColorFillScheme scheme = GetColorFillSchemeByName(doc, "Rentable Area");

			// add color legend to view
			AddColorLegend(curView, scheme);

			return Result.Succeeded;
		}

		internal static ColorFillScheme GetColorFillSchemeByName(Document doc, string name)
		{
			List<ColorFillScheme> collector = new FilteredElementCollector(doc)
				.OfClass(typeof(ColorFillScheme))
				.Cast<ColorFillScheme>().ToList();

			foreach(ColorFillScheme curScheme in collector)
			{
				if (curScheme.Name == name)
					return curScheme;
			}

			return null;
		}

		internal static void AddColorLegend(View view, ColorFillScheme scheme)
		{
			ElementId areaCatId = new ElementId(BuiltInCategory.OST_Areas);
			ElementId curLegendId = view.GetColorFillSchemeId(areaCatId);

			using(Transaction t = new Transaction(view.Document))
			{
				t.Start("Insert Area Legend");

				if(curLegendId == ElementId.InvalidElementId)
					view.SetColorFillSchemeId(areaCatId, scheme.Id);
				
				ColorFillLegend.Create(view.Document, view.Id, areaCatId, XYZ.Zero);
				
				t.Commit();
			}
		}
		
		internal static PushButtonData GetButtonData()
		{
			// use this method to define the properties for this command in the Revit ribbon
			string buttonInternalName = "btnCommand1";
			string buttonTitle = "Button 1";
			string? methodBase = MethodBase.GetCurrentMethod().DeclaringType?.FullName;

			if (methodBase == null)
			{
				throw new InvalidOperationException("MethodBase.GetCurrentMethod().DeclaringType?.FullName is null");
			}
			else
			{
				Common.ButtonDataClass myButtonData1 = new Common.ButtonDataClass(
					buttonInternalName,
					buttonTitle,
					methodBase,
					Properties.Resources.Blue_32,
					Properties.Resources.Blue_16,
					"This is a tooltip for Button 1");

				return myButtonData1.Data;
			}
		}
	}

}
