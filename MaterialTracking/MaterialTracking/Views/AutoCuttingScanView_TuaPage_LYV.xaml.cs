using MaterialTracking.UsersControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AutoCuttingScanView_TuaPage_LYV : ContentPage
	{
		Model_AutocuttingView_LYV vm = new Model_AutocuttingView_LYV();

		List<Model_DataDetail_LYV> data;
		List<Model_Data_Ry> data_ry;
		List<Model_ManySite_LYV> fullSz;
		List<Model_GrpSize_LYV> GrpSz;

		List<UC_Input> lst_Tools = new List<UC_Input>();

		List<Button> lst_btn_SaveLocal = new List<Button>();

		System.Data.DataTable table;

		int currentPage = 1;

		UC_Input ToolInput;

		public AutoCuttingScanView_TuaPage_LYV()
		{
			InitializeComponent(); vm.GetData_MainTitle();

			this.BindingContext = vm;

			data = vm.Data().ListData;

			table = vm.Data().DataTable;

			data_ry = vm.Data().DataRY;

			fullSz = vm.HowMany_Size();

			GrpSz = vm.Group_Size();

			if (fullSz.Count > 0 && GrpSz.Count > 0) Generate_Title(fullSz, GrpSz); else { Services.Alert.Msg("Thông báo", "Mã QCode sai"); return; }

		}
		void Generate_Title(List<Model_ManySite_LYV> HowManyColomn, List<Model_GrpSize_LYV> grpSz)
		{
			Grid G_1 = new Grid();
			ColumnDefinition column;

			G_1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(27, GridUnitType.Absolute) });
			G_1.RowDefinitions.Add(new RowDefinition { Height = new GridLength(27, GridUnitType.Absolute) });



			for (int i = 0; i < HowManyColomn.Count; i++)
			{
				double d = (185 / HowManyColomn[i].GCount);
				var w = Math.Ceiling(d);
				column = new ColumnDefinition
				{
					Width = new GridLength(w, GridUnitType.Absolute)
				};

				var lblSz = new Label
				{
					Text = HowManyColomn[i].XXCC,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					FontAttributes = FontAttributes.Bold,
					TextColor = Color.White,
					FontSize = 15
				};

				var frm = new Frame
				{
					BorderColor = Class.Style.LightSky
					,
					BackgroundColor = Color.Transparent
					,
					Padding = new Thickness(0),

					Margin = new Thickness(-4, -0, -4, -4)
					,
					Content = lblSz
				};


				G_1.ColumnDefinitions.Add(column);




				G_1.Children.Add(frm, i, 0);

				//------------------------------------------------
				#region Them Hai cot cuoi cung Total, Usage

				if (i == HowManyColomn.Count - 1)
				{
					column = new ColumnDefinition
					{
						Width = new GridLength(85, GridUnitType.Absolute)
					};
					var frmTotal = new Frame
					{
						BorderColor = Class.Style.LightSky
					,
						BackgroundColor = Color.Transparent
					,
						Padding = new Thickness(0)
						,
						Margin = new Thickness(-4, -3, -4, 0)
					};

					var lbl_Total = new Label
					{
						Text = "Total",

						HorizontalTextAlignment = TextAlignment.Center,

						VerticalTextAlignment = TextAlignment.Center,

						FontAttributes = FontAttributes.Bold,

						HorizontalOptions = LayoutOptions.Center,

						VerticalOptions = LayoutOptions.Center,

						FontSize = 20

						,
						TextColor = Color.White
					};

					frmTotal.Content = lbl_Total;

					G_1.ColumnDefinitions.Add(column);

					G_1.Children.Add(frmTotal, i + 1, 0);

					Grid.SetRowSpan(frmTotal, 2);

					column = new ColumnDefinition
					{
						Width = new GridLength(85, GridUnitType.Absolute)
					};

					var frmUsega = new Frame
					{
						BorderColor = Class.Style.LightSky
					,
						BackgroundColor = Color.Transparent
					,
						Padding = new Thickness(0),

						Margin = new Thickness(-4, -3, -4, 0)

					};


					var lbl_Usage = new Label
					{
						Text = "Usage",

						HorizontalTextAlignment = TextAlignment.Center,

						VerticalTextAlignment = TextAlignment.Center,

						HorizontalOptions = LayoutOptions.Center,

						VerticalOptions = LayoutOptions.Center,

						FontAttributes = FontAttributes.Bold,

						FontSize = 20

						,
						TextColor = Color.White

					};


					frmUsega.Content = lbl_Usage;

					G_1.ColumnDefinitions.Add(column);

					G_1.Children.Add(frmUsega, i + 2, 0);

					Grid.SetRowSpan(frmUsega, 2);


				}

				#endregion
			}
				//Second Row
				if (grpSz.Count > 0) MergeCell_SzGrp(ref G_1);

				//GridTitle.Children.Add(G_1);
		}
		void MergeCell_SzGrp(ref Grid Grid_temp)
		{
			int pos = 0;

			int old_pos = 0;
			int old_merge = 1;

			for (int i = 0; i < GrpSz.Count; i++)
			{
				var columnSpan = GrpSz[i].Count == 0 ? 1 : GrpSz[i].Count;

				var lbl_sz = new Label
				{
					Text = GrpSz[i].GXXCC,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
					FontAttributes = FontAttributes.Bold,
					FontSize = 15
					,
					TextColor = Class.Style.ColdKidsSky.BlueSea
				};

				var frm = new Frame
				{
					BorderColor = Class.Style.LightSky
					,
					BackgroundColor = Color.Transparent,
					Padding = new Thickness(0),
					Margin = new Thickness(-4, -3, -4, 0)
					,
					Content = lbl_sz
				};


				switch (i)
				{
					case 0:
						Grid_temp.Children.Add(frm, pos, 1);

						Grid.SetColumnSpan(frm, columnSpan);

						old_pos = pos;

						old_merge = columnSpan;

						break;

					default:

						pos = old_pos + old_merge;

						Grid_temp.Children.Add(frm, pos, 1);

						Grid.SetColumnSpan(frm, columnSpan);

						old_pos = pos;

						old_merge = columnSpan;

						break;
				}
			}

		}
		
		
		
		private void btn_SaveLocalAll_Clicked(object sender, EventArgs e)
        {

        }

        private void btn_Confirm_Clicked(object sender, EventArgs e)
        {

        }
    }
}