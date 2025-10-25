using MaterialTracking.DB;
using MaterialTracking.Models;
using MaterialTracking.UsersControl;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AutoCuttingScanView_LYV : ContentPage
	{
		Model_AutocuttingView_LYV vm = new Model_AutocuttingView_LYV();

		List<Model_DataDetail_LYV> data;
		List<Model_Data_Ry> data_ry;
		List<Model_ManySite_LYV> fullSz;
		List<Model_GrpSize_LYV> GrpSz;

		List<UC_Input>lst_Tools= new List<UC_Input>();

		List<Button> lst_btn_SaveLocal = new List<Button>();

		System.Data.DataTable table;

		int currentPage=1;

		UC_Input ToolInput;
        public AutoCuttingScanView_LYV ()
		{
			InitializeComponent ();

			vm.GetData_MainTitle();
			
			this.BindingContext = vm;			

			data = vm.Data().ListData;

			table = vm.Data().DataTable;

			data_ry = vm.Data().DataRY;

			fullSz = vm.HowMany_Size();

			GrpSz = vm.Group_Size();

			if (fullSz.Count > 0 && GrpSz.Count > 0) Generate_Title(fullSz, GrpSz); else {Services.Alert.Msg("Thông báo","Mã QCode sai"); return; }

			Add_pages_button();

            ScrollCoDinh.Scrolled += (s, e) =>
            {
                Scrolldata.ScrollToAsync(0, e.ScrollY, false);
            };

            Scrolldata.Scrolled += (s, e) =>
            {
                ScrollCoDinh.ScrollToAsync(0, e.ScrollY, false);
            };

            var a = DB.DataLocal.Table.Delete_All_LYV;

			BarcodeHistory his = new BarcodeHistory
			{
				Barcode = DB.StoreLocal.Instant.Barcode,
				UDate = DateTime.Now.Ticks
			};

			DB.DataLocal.Table.Update_History(his);
        }
		 Model_AutocuttingView_LYV ThisContext
        {
            get =>  this.BindingContext as Model_AutocuttingView_LYV;
		}
		protected override bool OnBackButtonPressed()
		{

			Device.BeginInvokeOnMainThread(async () =>
			{
				base.OnBackButtonPressed();

				await Navigation.PopToRootAsync(); //(new Views.MachineNoViews());


			});

			return true;
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
					, Content = lblSz
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

						,TextColor = Color.White
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

						,TextColor = Color.White

					};


					frmUsega.Content = lbl_Usage;

					G_1.ColumnDefinitions.Add(column);
				
					G_1.Children.Add(frmUsega, i + 2, 0);

					Grid.SetRowSpan(frmUsega, 2);


				}

				#endregion

			}



			//Second Row
			if(grpSz.Count>0) MergeCell_SzGrp(ref G_1);

			GridTitle.Children.Add(G_1);
			
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
					,TextColor=Class.Style.ColdKidsSky.BlueSea
				};

				var frm = new Frame
				{
					BorderColor = Class.Style.LightSky
					,
					BackgroundColor = Color.Transparent,
					Padding = new Thickness(0),
					Margin = new Thickness(-4, -3, -4, 0)
					,Content =lbl_sz
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
		
		void Data_left(int page)
        {
			if(Grd_DataLeft.Children.Count > 0)
				foreach (View childView in Grd_DataLeft.Children.ToList())            
					Grd_DataLeft.Children.Remove(childView);
            
			if(GridData.Children.Count >0)
				foreach (View childView in GridData.Children.ToList())            
				    GridData.Children.Remove(childView);
            
  

			var listData_TSL = data.Where(item => item.Pages == page && item.Type =="TSL").ToList();
			var listData_SH = data.Where(item => item.Pages == page && item.Type =="SH").ToList();
			var listData_CT = data.Where(item => item.Pages == page && item.Type =="CT").ToList();


			var listRY = data_ry.Where(item => listData_TSL.Any(s => item.Seq.Contains(s.Batch))).ToList();

			var DistinctRY = listRY.GroupBy(x => x.Zlbh)
						   .Select(g => g.First()) 
						   .ToList();

			if (listData_TSL.Any())
			{
				var g_left = new Grid();

				var g_right = new Grid();

			
				

				g_left.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80, GridUnitType.Absolute) });//content

				g_left.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80, GridUnitType.Absolute) });//Button

				for (int column_Gright = 0; column_Gright < fullSz.Count; column_Gright++)
				{
					double d = 185 / fullSz[column_Gright].GCount;

					g_right.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(Math.Ceiling(d), GridUnitType.Absolute) });

					if (column_Gright == fullSz.Count - 1) //Total and Usage column
					{
						g_right.ColumnDefinitions.Add(new ColumnDefinition
						{
							Width = new GridLength(85, GridUnitType.Absolute)
						});
						g_right.ColumnDefinitions.Add(new ColumnDefinition
						{
							Width = new GridLength(85, GridUnitType.Absolute)
						});

					}
				}
				//var rowGLeft = 0;
				for (int i = 0; i < listData_TSL.Count; i++)
				{


					var RY_Tua = listRY.GroupBy(x => x.Zlbh)
															.Where(g => g.Any(x => x.Seq == listData_TSL[i].Batch))
															 .Select(g => g.First())
															  .ToList();

					g_left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) }); //TSL
					g_left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) }); //SH
					g_left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) }); //CT

					//g_left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(190 * DistinctRY.Count, GridUnitType.Absolute) });

					var lbl_Batch = new Label
					{
						FontSize = 20,
						FontAttributes = FontAttributes.Bold,
						Text = listData_TSL[i].Batch
						,
						TextColor = Class.Style.ColdKidsSky.Green
						,
						VerticalTextAlignment = TextAlignment.Center
						, HorizontalTextAlignment = TextAlignment.Center,
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center

						, Rotation = -90
					};
					var lbl_TSL = new Label
					{
						FontSize = 16,
						Text = listData_TSL[i].Type,
						TextColor = Class.Style.ColdKidsSky.Green
						,
						HorizontalTextAlignment = TextAlignment.Center,
						Margin = new Thickness(0, 5, 0, 0)
					};	
					var lbl_SH = new Label
					{
						FontSize = 16,
						Text = "SH",
						TextColor = Class.Style.ColdKidsSky.Green
						,
						HorizontalTextAlignment = TextAlignment.Center,
						Margin = new Thickness(0, 5, 0, 0)
					};	
					var lbl_CT = new Label
					{
						FontSize = 16,
						Text = "CT",
						TextColor = Class.Style.ColdKidsSky.Green
						,
						HorizontalTextAlignment = TextAlignment.Center,
						Margin = new Thickness(0, 5, 0, 0)
					};

				

					var frm_Batch = new Frame
					{
						BackgroundColor = Class.Style.ColdKidsSky.BlueSky,
						Content = lbl_Batch
						,Padding = new Thickness(0)
					};

					var frm_TSL = new Frame
					{
						BackgroundColor = Class.Style.ColdKidsSky.BlueSky,
						Content = lbl_TSL,
						Padding = new Thickness(0) , 
					};
					var frm_SH = new Frame
					{
						BackgroundColor = Class.Style.ColdKidsSky.BlueSky,
						Content = lbl_SH,
						Padding = new Thickness(0) , 
					};
					var frm_CT = new Frame
					{
						BackgroundColor = Class.Style.ColdKidsSky.BlueSky,
						Content = lbl_CT,
						Padding = new Thickness(0) , 
					};

					

					//Grid grid_bntSave = new Grid();

				//	grid_bntSave.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });

					//grid_bntSave.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });

					//grid_bntSave.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });
					
					

					var ButtonSave_local = new Button
					{
						Text = "Save",

						FontSize = 16

						, Padding = new Thickness(0)

						, BackgroundColor = Class.Style.ColdKidsSky.BlueSea

						, AutomationId = string.Format("{0}_{1}_{2}_{3}",

							DB.StoreLocal.Instant.Barcode, 

							(this.BindingContext as Model_AutocuttingView_LYV).BWBH,

							(this.BindingContext as Model_AutocuttingView_LYV).CLBH, 

							listData_TSL[i].Batch)
						, HorizontalOptions = LayoutOptions.Center
					};

					ButtonSave_local.Clicked += (sender, e) =>
					{

						var _btn = (Button)sender;

						var classid = _btn.AutomationId.Split('_');

						var isdone = false;

						var btnSave_bwbh = classid[1].ToString();
						var btnSave_clbh = classid[2].ToString();
						var btnSave_tua = classid[3].ToString();


						

                        foreach (var toolInput in lst_Tools)
                        {
							var a = (toolInput.BindingContext as InputViewModel2);
							var zl = a.zlbh;
							if(a.tua ==btnSave_tua && a.bwbh == btnSave_bwbh && a.clbh == btnSave_clbh && !a.IsFul && a.Qtyed > 0)
                            {

								isdone = a.SaveLocal();

								var ddd = DB.DataLocal.Table.All_Local_LYV();
							}
							
                        }
						if (isdone)
						{

							Services.DisplayToast.Show.Toast("Success", Class.Style.ColdKidsSky.Green);

							Alarm.Sound.Completed();
                        }
                        else
                        {
							Services.DisplayToast.Show.Toast("Fail", Class.Style.Error);

							Alarm.Sound.Error();

						}
					};


					var btn_Reset = new Button
					{
						Text = "Reset",
						FontSize = 16,
						Padding = new Thickness(0),

						BackgroundColor = Class.Style.ColdKidsSky.BlueSea
						,
						AutomationId = ButtonSave_local.AutomationId,
						HorizontalOptions = LayoutOptions.Center
					};

                    btn_Reset.Clicked += (s, e) =>
                    {
                        var _btn = (Button)s;

                        var classid = _btn.AutomationId.Split('_');

                        var btnSave_bwbh = classid[1].ToString();
                        var btnSave_clbh = classid[2].ToString();
                        var btnSave_tua = classid[3].ToString();

                        foreach (var toolInput in lst_Tools)
                        {
                            var a = (toolInput.BindingContext as InputViewModel2);
                            if (a.tua == btnSave_tua && a.bwbh == btnSave_bwbh && a.clbh == btnSave_clbh)
                            {
                                a.SelectedIndex = a.Temp = 0;
                            }

                        }
                    };




                    lst_btn_SaveLocal.Add(ButtonSave_local);

					lst_btn_SaveLocal.Add(btn_Reset);

					g_left.Children.Add(frm_TSL, 1, g_left.RowDefinitions.Count - 3);

					g_left.Children.Add(frm_SH, 1, g_left.RowDefinitions.Count - 2);

					g_left.Children.Add(frm_CT, 1, g_left.RowDefinitions.Count - 1);

					//grid_bntSave.Children.Add(frm_CT, 0, 1);



					g_left.Padding = new Thickness(0);

					g_left.Margin = new Thickness(0);

					//g_left.Children.Add(grid_bntSave);



					//Grid.SetRow(grid_bntSave,rowGLeft );

					//Grid.SetColumn(grid_bntSave, 1);

					var spanrow = RY_Tua.Count + 3;

					g_left.Children.Add(frm_Batch);

					Grid.SetRow(frm_Batch, g_left.RowDefinitions.Count - 3);

					Grid.SetRowSpan(frm_Batch, spanrow);

					//rowGLeft = g_left.RowDefinitions.Count -1;

					

				

					var List_SH = listData_SH.Where(item => item.Batch == listData_TSL[i].Batch).ToList();
					
					var List_CT = listData_CT.Where(item => item.Batch == listData_TSL[i].Batch).ToList();



                    var list_TSL = listData_TSL.Where(item => item.Batch == listData_TSL[i].Batch && item.Pages == page && item.Type == "TSL").ToList();

                    Insert_TSL_Value(ref g_right, list_TSL); //Them du lieu vaof TSL cua Tua nay

                    Insert_TSL_Value(ref g_right, List_SH);//Them du lieu vaof SH cua Tua nay

					Insert_TSL_Value(ref g_right, List_CT);//Them du lieu vaof CT cua Tua nay

					for (int lstry = 0; lstry < RY_Tua.Count; lstry++) //them ry vao moi tua
					{
						var dataright = data_ry.Where(r => r.Seq == listData_TSL[i].Batch && r.Zlbh == RY_Tua[lstry].Zlbh).ToList();

						if (dataright.Any()) //neu co RY nay trong tua nay
						{
							//tao ra them mot dong trong g_left them ry nay vao
							g_left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(150, GridUnitType.Absolute) });

							g_left.Children.Add(new Frame
							{
								BackgroundColor = Class.Style.ColdKidsSky.BlueSky,
								Padding = new Thickness(-5),
								Content = new Label
								{
									FontSize = 18,
									Text = dataright[0].Zlbh,
									TextColor = Class.Style.ColdKidsSky.Green,
									HorizontalTextAlignment = TextAlignment.Center,
									VerticalTextAlignment = TextAlignment.Center,
									HorizontalOptions = LayoutOptions.Center,
									VerticalOptions = LayoutOptions.Center,
									Rotation = -90,
									Margin = new Thickness(-5),
									Padding = new Thickness(-5)

								}
							}, 1, g_left.RowDefinitions.Count - 1);
						
							Data_right(ref g_right, dataright, list_TSL, lstry == 0,RY_Tua.Count+3); //chen data cua RY nay tai vi tri nay

							if (lstry >= RY_Tua.Count - 1) //Neu la dong cuoi cung cua Tua thi them nut bam save Local
							{
								g_right.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) });

								var btnInfo = new Button
								{
									Text = "Sum",
									Margin= new Thickness(-5)
									,BackgroundColor = Class.Style.Primary

								};
								btnInfo.Clicked += (s, e) =>
								{
									var t = list_TSL[0].Total;
									var u = list_TSL[0].Usage;

									var info_total = string.Format("Batch: {0} Total: {1}/{2} Usage: {3}", lbl_Batch.Text, ThisContext.SumDid(lbl_Batch.Text), t, u);

									Services.DisplayToast.Show.Toast(info_total, Class.Style.Notification);
								};

								g_right.Children.Add(btnInfo, 0, g_right.RowDefinitions.Count - 1);

								Grid.SetColumnSpan(btnInfo, g_right.ColumnDefinitions.Count);
							}

						}
						if(lstry >= RY_Tua.Count - 1)
                        {
							g_left.RowDefinitions.Add(new RowDefinition { Height = new GridLength(40, GridUnitType.Absolute) });

							g_left.Children.Add(btn_Reset, 0, g_right.RowDefinitions.Count - 1);

							g_left.Children.Add(ButtonSave_local, 1, g_right.RowDefinitions.Count - 1);
						}
					}										

				}
				
				Grd_DataLeft.Children.Add(g_left);					
				this.GridData.Children.Add(g_right);
			}
			Alarm.Sound.Completed();
		}

      

		string Get_Value(List<Model_DataDetail_LYV> dataModel, string size)
        {
			var TotalSz_grp = "";
			foreach (Model_DataDetail_LYV item in dataModel)
			{
				System.Reflection.PropertyInfo[] properties = item.GetType().GetProperties();

				foreach (System.Reflection.PropertyInfo property in properties)
				{
					string propertyName = property.Name;

					if (vm.CheckSize(propertyName) == size)
					{
						try
						{
							TotalSz_grp = property.GetValue(item).ToString().Split('.')[0];
						}
						catch (Exception)
						{
							TotalSz_grp = "0";
						}
						goto GoOn;

					}



				}

			}
			GoOn:return TotalSz_grp;
		}
		void Insert_TSL_Value(ref Grid Grid_temp , List<Model_DataDetail_LYV> dataModel)
        {
			int pos = 0;

			int old_pos = 0;

			int old_merge = 1;						

			Grid_temp.RowDefinitions.Add(new RowDefinition { Height = new GridLength(30, GridUnitType.Absolute) }); 

			for (int i = 0; i < GrpSz.Count; i++)
            {
                var columnSpan = GrpSz[i].Count;

                var TotalSz_grp = "";

                var columnName = table.Columns[i + 7].ColumnName;


                TotalSz_grp = Get_Value(dataModel, columnName);

                //if (TotalSz_grp == "0" || TotalSz_grp == "") continue;

                var lbl_sz = new Label
                {
                    Text = TotalSz_grp == "" ? "0" : TotalSz_grp,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 15
                   ,
                    TextColor = Class.Style.ColdKidsSky.Cream
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
						Grid_temp.Children.Add(frm, pos, Grid_temp.RowDefinitions.Count-1);

						Grid.SetColumnSpan(frm, columnSpan);

						old_pos = pos;

						old_merge = columnSpan;

						break;

					default:

						pos = old_pos + old_merge;

						Grid_temp.Children.Add(frm, pos, Grid_temp.RowDefinitions.Count - 1);

						Grid.SetColumnSpan(frm, columnSpan);

						old_pos = pos;

						old_merge = columnSpan;

						break;
				}


			}

			

		}
        
		void Data_right(ref Grid tempGrid, List<Model_Data_Ry> dataModel , List<Model_DataDetail_LYV> TotalUsage, bool addTotal=true, int MergeRow=1)
        {

            tempGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(155, GridUnitType.Absolute) });
		

            int pos = 0;
            int old_pos = 0;
            int old_merge = 1;

            var gSize = "";

            for (int i = 0; i < fullSz.Count; i++)
            {
                var columnSpan = fullSz[i].GCount;

                //double d = 185 / columnSpan;
                //var w = Math.Ceiling(d);

                //tempGrid.ColumnDefinitions.Add(new ColumnDefinition
                //{
                //	Width = new GridLength(Math.Ceiling(d), GridUnitType.Absolute)
                //});

                if (gSize != fullSz[i].GXXCC)
                {
                    gSize = fullSz[i].GXXCC;

                    var batch = dataModel[0].Seq;




					var target = dataModel.Any(r => r.Gxxcc == gSize) ? dataModel.Where(r => r.Gxxcc == gSize).ToList()[0].Qty : 0;//int.Parse(Get_Value(dataModel, fullSz[i].XXCC));

					var PrN = string.Format("{0}_{1}_{2}_{3}_{4}_{5}",
                        DB.StoreLocal.Instant.Barcode,
                        (this.BindingContext as Model_AutocuttingView_LYV).BWBH,
                        (this.BindingContext as Model_AutocuttingView_LYV).CLBH,
                        gSize,
                        batch,
						dataModel[0].Zlbh);

                    var cutting_barcodes_groups_edit = ThisContext.Data_into_inputTool(DB.StoreLocal.Instant.Barcode).Where(x => x.Prono == PrN).ToList();

					//	var datalocal = DB.DataLocal.Table.All_Local_LYV().Where(x=>x.Prono ==PN).ToList();
				

                    ToolInput = new UC_Input(new InputViewModel2
                    {
                        Qtysql = cutting_barcodes_groups_edit.Any() ? cutting_barcodes_groups_edit[0].LocalQty : 0,

                        //	TempLocal = datalocal.Any() ? datalocal[0].LocalQty : 0,

                        TotalQty = target,

                        SelectedIndex = target,

                        Prono = PrN,                        

						BColor = cutting_barcodes_groups_edit.Any() ? cutting_barcodes_groups_edit[0].LocalQty == target ? Class.Style.ColdKidsSky.Green : Class.Style.Organ : Class.Style.EnableOff,

                        IsFul = cutting_barcodes_groups_edit.Any() ? cutting_barcodes_groups_edit[0].Isfull ? true : false : false,

                    });

                    ToolInput.ButtonPlusClicked += (sender, e) =>
                      {
                          // var input = (UC_Input)sender;

                          // var vm = input.BindingContext as InputViewModel2;

                          //Services.DisplayToast.Show.Toast(vm.Qtyed.ToString() , Class.Style.ColdKidsSky.Green);
                      };

                    lst_Tools.Add(ToolInput);

                    ToolInput.HorizontalOptions = LayoutOptions.Fill;

                    ToolInput.VerticalOptions = LayoutOptions.Fill;

                    var frm2 = new Frame
                    {
                        BackgroundColor = Color.White,

                        HeightRequest = 155,

                        Padding = new Thickness(0),

                        Margin = new Thickness(0),

                        Content = ToolInput,


                    };



					//tempGrid.Children.Add(frm2);

					//Grid.SetColumn(frm2, i);

					//Grid.SetRow(frm2, tempGrid.RowDefinitions.Count - 1);

					

                    switch (i)
                    {
                        case 0:
                            tempGrid.Children.Add(frm2, pos, tempGrid.RowDefinitions.Count - 1);

                            Grid.SetColumnSpan(frm2, columnSpan);

                            old_pos = pos;

                            old_merge = columnSpan;

                            break;

                        default:

                            pos = old_pos + old_merge;

                            tempGrid.Children.Add(frm2, pos, tempGrid.RowDefinitions.Count - 1);

                            Grid.SetColumnSpan(frm2, columnSpan);

                            old_pos = pos;

                            old_merge = columnSpan;

                            break;
                    }

                }
				if(addTotal)
					if (i == fullSz.Count - 1)
					{
                   
						var lbl = new Label
						{
							Text = string.Format("{0}/{1}", ThisContext.SumDid(dataModel[0].Seq), TotalUsage[0].Total),

							HorizontalTextAlignment = TextAlignment.Center,
							VerticalTextAlignment = TextAlignment.Center,
							FontAttributes = FontAttributes.Bold,
							FontSize = 16,
							TextColor = Class.Style.ColdKidsSky.Green
						};
						var frm = new Frame
						{
							Padding = new Thickness(0),
							BackgroundColor = Class.Style.ColdKidsSky.BlueSky
							,
							Content = lbl
						};

						tempGrid.Children.Add(frm);

						Grid.SetColumn(frm, i + 1);

						Grid.SetRow(frm, tempGrid.RowDefinitions.Count - 4);

						Grid.SetRowSpan(frm, MergeRow);


						lbl = new Label
						{
							Text = TotalUsage[0].Usage,

							HorizontalTextAlignment = TextAlignment.Center,
							VerticalTextAlignment = TextAlignment.Center,
							FontAttributes = FontAttributes.Bold,
							FontSize = 16
							,
							TextColor = Class.Style.ColdKidsSky.Green
						};
						frm = new Frame
						{
							Padding = new Thickness(0),
							BackgroundColor = Class.Style.ColdKidsSky.BlueSky,
							Content = lbl
						};

						tempGrid.Children.Add(frm);

						Grid.SetColumn(frm, i + 2);

						Grid.SetRow(frm, tempGrid.RowDefinitions.Count - 4);

						Grid.SetRowSpan(frm, MergeRow);
					}



			}




        }

        void Add_pages_button()
        {
			var maxTua = data.Max(maxtua => maxtua.Pages);

			if (maxTua > 0)
			{
				Data_left(1);

				for (int i = 1; i <= maxTua; i++)
				{
					var tuaName = data.Where(r => r.Pages == i).ToList();


					var btn_page = new Button
					{
						Text = tuaName[0].Batch.ToString(),

						AutomationId = i.ToString(),

						CornerRadius = 20,

						FontSize =20,

						TextColor =i==1? Class.Style.ColdKidsSky.BlueSea : Color.Gray,

						BackgroundColor = Color.Transparent
					};
					btn_page.Clicked += (sender, e) =>
					{
						var btn = (Button)sender;
                        foreach (var child in stk_pages.Children)
                        {
							if(child is Button a)
                            {
								a.TextColor = Color.Gray;
                            }
                        }
						btn.TextColor = Class.Style.ColdKidsSky.BlueSea;
						Data_left(int.Parse(btn.AutomationId));
						currentPage = int.Parse(btn.AutomationId);
					};

					stk_pages.Children.Add(btn_page);
				}
			}
        }

        private void btn_Confirm_Clicked(object sender, EventArgs e)
        {
			Data_left(currentPage);
						
        }

        private void btn_SaveLocalAll_Clicked(object sender, EventArgs e)
        {
			int isdone = 0;

			if (lst_Tools.Any())
				foreach (var toolInput in lst_Tools)
				{
					var a = (toolInput.BindingContext as InputViewModel2);
					if (!a.IsFul && a.Qtyed > 0) 
					{

						isdone = a.SaveLocal() ? isdone + 1 : isdone + 0;

					}
				}

			if (isdone > 0 )
			{

				Services.DisplayToast.Show.Toast("Success", Class.Style.ColdKidsSky.Green);

				Alarm.Sound.Completed();
			}
			else
			{
				Services.DisplayToast.Show.Toast(string.Format("Fail({0}/{1})",isdone,lst_Tools.Count), Class.Style.SoftColor.MPink);

				Alarm.Sound.Error();
			}
		}
    }
    //--------------------------------------------------------------------------------
    #region Class model
    public class Model_AutocuttingView_LYV : BaseViewModel
	{
		public string BarCode { get; set; }
		public string Lean { get; set; }		
		public string YWPM { get; set; }
		public string NameMat { get; set; }
		public string Layer { get; set; }
		public string RY { get; set; }
		public string CutDie { get; set; }
		public string ModelName { get; set; }
		public string Article { get; set; }
		public string Art_name { get; set; }

		public string CLBH { get; set; }
		public string BWBH { get; set; }

		ICommand _cmd_Collapsed;
		ICommand _cmdConfirm;

		private bool _isGridHide = true;
		public string DMQty{ get; set; }
		public string SumAndTua { get; set; }
		private GridLength _rowHeight = new GridLength(200);
		public ICommand Cmd_Collapsed { get => _cmd_Collapsed; set { _cmd_Collapsed = value; OnPropertyChanged("Cmd_Collapsed"); } }
        public ICommand CmdConfirm { get => _cmdConfirm; set { _cmdConfirm = value; OnPropertyChanged("CmdConfirm"); } }
		public Model_AutocuttingView_LYV()
        {
			BarCode = DB.StoreLocal.Instant.Barcode;
			Cmd_Collapsed = new Command(CollapseRunTime);
			CmdConfirm = new Command(Confirm);
		}
	
        		        
        private void Confirm(object obj)
        {
			int kq = 0;

			var a = DB.DataLocal.Table.All_Local_LYV().Where(x=>x.Barcode==DB.StoreLocal.Instant.Barcode).ToList();

            if (a.Any())
            {			

				var lstSQL = Data_into_inputTool(DB.StoreLocal.Instant.Barcode);

				foreach (var itemLocal in a)
                {					
					if (lstSQL.Any()) //Find and Update 
                    {

						string update = @"UPDATE [dbo].[App_Cutting_Barcodes_Groups_Edit]
SET 
      [ActualQty] = @ActualQty
      ,[USERID] = @USERID
      ,[USERDATE] = @USERDATE    
	WHERE (prono=@prono)";

						var sub_lst_SQL = lstSQL.Where(x => x.Prono == itemLocal.Prono ).ToList();

						if (sub_lst_SQL.Any())
						{
							string[] arr =
								{
									itemLocal.LocalQty.ToString(),

									DB.StoreLocal.Instant.UserName,

									DateTime.Now.ToString("yyyy-MM/dd HH:mm:ss.fff"),

									itemLocal.Prono
								};

							var oldKq = kq;

							kq += sub_lst_SQL[0].LocalQty != itemLocal.LocalQty ?  DB.SQL.ConnectDB.Connection.Update_Parameter(update, arr) : 0;

							if (kq != oldKq)
							{
								kq = Insert_To_History_LYV(new Table_Barcode
								{

									Prono = itemLocal.Prono,
									XXCC = itemLocal.GXXCC,
									Qty = itemLocal.LocalQty,
									UserID = DB.StoreLocal.Instant.UserName,

								});
							}

							goto GoOn;
						}

						else
						{
							kq = InsertNewRow_SQL(itemLocal);


							if (kq != 0)
							{
								kq = Insert_To_History_LYV(new Table_Barcode
								{

									Prono = itemLocal.Prono,
									XXCC = itemLocal.GXXCC,
									Qty = itemLocal.LocalQty,
									UserID = DB.StoreLocal.Instant.UserName,

								});
							}
						}
					}
                    else //insert new row
                    {
						kq = InsertNewRow_SQL(itemLocal) ;

						if (kq != 0)
						{
							kq = Insert_To_History_LYV(new Table_Barcode
							{

								Prono = itemLocal.Prono,
								XXCC = itemLocal.GXXCC,
								Qty = itemLocal.LocalQty,
								UserID = DB.StoreLocal.Instant.UserName,

							});
						}
					}
					GoOn: continue;
				}
            }

			Services.DisplayToast.Show.Toast(kq > 0 ? "Thành công!" : "Thất bại", kq > 0 ? Class.Style.ColdKidsSky.Green : Class.Style.Red);

			var del = DB.DataLocal.Table.Delete_All_LYV;
        }
		int InsertNewRow_SQL(Barcode_Edit_LVY item)
        {
			string insert = @"INSERT INTO [dbo].[App_Cutting_Barcodes_Groups_Edit]
		   ([Barcode]
           ,[Tua]
           ,[ZLBH]
           ,[CLBH]
           ,[XXCCs]
           ,[GXXCC]
           ,[PlanQty]
           ,[ActualQty]
           ,[USERID]
           ,[USERDATE]
           ,[YN]
           ,[BWBH]
           ,[ProNo])
     VALUES
		   (@barc
		   , @tua
		   , @zlbh
		   , @clbh
		   , @xxccs
		   , @gxxcc
		   , @target
		   , @actual
		   , @user
		   , @date
		   , 1
		   , @bwbh
		   , @prono)";


			string[] arrInsert =
			{
							item.Barcode,
							item.Tua,
							item.ZLBHs,
							item.CLBH,
							"",
							item.GXXCC,
							item.Target.ToString(),
							item.LocalQty.ToString(),
							item.UserID,
							DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
							
							item.BWBH,
							item.Prono

						};
		return	 DB.SQL.ConnectDB.Connection.Update_Parameter(insert, arrInsert);

		}


		int Insert_To_History_LYV(Table_Barcode Row)
		{
			string sql = $"SELECT top(1)* FROM App_Cutting_History_Edit where ProNo='{Row.Prono}' order by times desc";

			var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);
			int times = 1;
			if (ta.Rows.Count > 0)
			{
				times = int.TryParse(ta.Rows[0]["times"].ToString(), out times) ? times += 1 : 1;
			}

			string sqlInsert = @"INSERT INTO [dbo].[App_Cutting_History_Edit]
                                ([ProNo]
                                ,[XXCC]
                                ,[ActualQty]
                                ,[USERID]
                                ,[USERDATE]
                                ,[YN]
                                ,[Times]
                                )
                           VALUES
                               (@prono
                               , @xxcc
                               , @actualQty
                               , @uSERID
                               , getdate()
                               , 1
                               , @times)";
			return DB.SQL.ConnectDB.Connection.Update_Parameter(sqlInsert, new string[] { Row.Prono, Row.XXCC, Row.Qty.ToString(), Row.UserID, times.ToString() });

		}

		public GridLength RowHeight
		{
			get { return _rowHeight; }
			set
			{
				_rowHeight = value;
				OnPropertyChanged(nameof(RowHeight));
			}
		}

        public bool IsGridHide { get => _isGridHide; set { _isGridHide = value; OnPropertyChanged("IsGridHide"); } }

		public string SumAllBarcode()
        {
			string sql = $"SELECT convert(int,sum(actualQty)) s FROM App_Cutting_Barcodes_Groups_Edit where Barcode = '{DB.StoreLocal.Instant.Barcode}' ";

			var a = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

			return a.Rows[0][0].ToString() == "" ? "0" : a.Rows[0][0].ToString();
		}
		public string SumDid(string tua)
        {
			string sqlsum = $"SELECT convert(int,sum(actualQty)) s FROM App_Cutting_Barcodes_Groups_Edit where Barcode = '{DB.StoreLocal.Instant.Barcode}' and Tua = '{tua}' ";

			var a = DB.SQL.ConnectDB.Connection.FillDataTable(sqlsum);
			return a.Rows[0][0].ToString()==""?"0":a.Rows[0][0].ToString();
        }
        private void CollapseRunTime(object obj)
        {
			if (RowHeight.Value < 200)
			{
				RowHeight = new GridLength(200);
				_isGridHide = true;
			}

			else 
			{
				RowHeight = new GridLength(0);
				_isGridHide = false;
			} 
		}

        //Layer, CutDie, Qty, DMQty
        public void GetData_MainTitle()
        {
			

			string sql = $"SELECT cb.Barcode, cb.WorkDate, cb.Layers, cb.CutDie, convert(int, Total.Qty)qty, Total.DMQty, Total.CLSL FROM Cutting_Barcode AS cb left join(select A.Barcode,SUM(A.Qty) Qty, Sum(A.DMQty) DMQty, ROUND(Sum(A.DMQty)/SUM(A.Qty),4) CLSL from (SELECT Barcode, Qty, SUM(DMQty) DMQty FROM Cutting_Barcodes GROUP BY Barcode,ZLBH,Qty) A  group by Barcode  )Total on Total.Barcode=CB.Barcode  WHERE cb.Barcode='{BarCode}'";

			var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

			if (ta == null) { Services.DisplayToast.Show.Toast("Lỗi Dữ Liệu, tính ra CLSL", Class.Style.SoftColor.MPink); return; }

			string Total="";
			string Tua="";

			if (ta.Rows.Count > 0)
            {
				Layer =string.Format("Layers :{0}", ta.Rows[0]["Layers"].ToString());

				CutDie = ta.Rows[0]["CutDie"].ToString();

				Total = ta.Rows[0]["qty"].ToString();

				DMQty=ta.Rows[0]["DMQty"].ToString();
			}
			//RY, Tua
			 sql = $@" select BS.ZLBH ,ddzl.ARTICLE,XieMing,bs.CLBH,bs.BWBH
from Cutting_Barcodes BS inner join ddzl 
on ddzl.DDBH = bs.ZLBH inner join xxzl
on xxzl.ARTICLE = ddzl.ARTICLE 
where BS.Barcode = '{DB.StoreLocal.Instant.Barcode}' group by BS.ZLBH ,ddzl.article ,XieMing,bs.CLBH,bs.BWBH";

			 ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

			if (ta.Rows.Count > 0)
			{
				Article = ta.Rows[0]["article"].ToString();

				Art_name = ta.Rows[0]["Xieming"].ToString();

				BWBH = ta.Rows[0]["Bwbh"].ToString();

				CLBH = ta.Rows[0]["Clbh"].ToString();

				if (ta.Rows.Count > 0)
				{									
					List<string> lstRY = new List<string>();

					for (int i = 0; i < ta.Rows.Count; i++)
					{
                        if (!lstRY.Contains(ta.Rows[i]["ZLBH"].ToString()))
                        {
							lstRY.Add(ta.Rows[i]["ZLBH"].ToString());
                        }
					}
					for (int i = 0; i < lstRY.Count; i++)
					{

						RY += lstRY[i] + Environment.NewLine;
					}
				}
				//xieming, modelname
				sql = $"select CLZL.ywpm, CLZL.dwbh,bwzl.ywsm   from Cutting_Barcodes3 A LEFT JOIN clzl on A.CLBH = clzl.cldh LEFT JOIN bwzl on A.BWBH = bwzl.bwdh WHERE A.Barcode = '{BarCode}' GROUP BY CLZL.ywpm, CLZL.dwbh,bwzl.ywsm ";

				 ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

				if (ta.Rows.Count > 0)
				{
					YWPM = ta.Rows[0]["ywpm"].ToString();

					ModelName = ta.Rows[0]["ywsm"].ToString();

				}

			}
			 sql = $"SELECT COUNT(A.Seq_Plan) Max_Seq FROM(SELECT SEQ_Plan FROM Cutting_Barcodes_SEQ WHERE Barcode = '{BarCode}' GROUP BY SEQ_Plan) A";

			 ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta.Rows.Count > 0)
            {
                Tua = ta.Rows[0][0].ToString();
            }
			SumAndTua = string.Format("{0}/{1}{2}({3})", SumAllBarcode(), Total, Environment.NewLine, Tua);


			sql = $"SELECT DepName FROM cutting_barcode LEFT JOIN BDepartment b ON  cutting_barcode.DepID=ID  WHERE Barcode='{BarCode}'";

			ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

			if (ta.Rows.Count > 0)
			{
				Lean = ta.Rows[0][0].ToString();
			}
		}
		public List<Model_ManySite_LYV> HowMany_Size()
        {
			//string sql = $"select distinct XXCC, Cutting_Barcodes_SEQ.GXXCC from Cutting_Barcodes_SEQ where barcode ='{DB.StoreLocal.Instant.Barcode}'";

			var sql = $@"select distinct a.XXCC, a.GXXCC
  from (select barcode,ZLBH,BWBH,CLBH,Qty,XXCC, SEQ,
 case when isnull(GXXCC,'') <> '' then GXXCC else XXCC end GXXCC from cutting_barcodes_seq) a
  where barcode = '{DB.StoreLocal.Instant.Barcode}'";



			sql = $@"  select DISTINCT   a.XXCC, a.GXXCC ,b.[count]
  from (select barcode,ZLBH,BWBH,CLBH,Qty,XXCC, SEQ,
 case when isnull(GXXCC,'') <> '' then GXXCC else XXCC end GXXCC from cutting_barcodes_seq) a
 LEFT JOIN (SELECT gxxcc, count(gxxcc) count FROM(select distinct XXCC, case when ISNULL(GXXCC,'')<> '' then Cutting_Barcodes_SEQ.GXXCC ELSE XXCC END GXXCC from Cutting_Barcodes_SEQ 
where barcode = '{DB.StoreLocal.Instant.Barcode}' ) s group by gxxcc) b ON b.GXXCC = a.GXXCC
  where barcode = '{DB.StoreLocal.Instant.Barcode}'";

			var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

			var list = new List<Model_ManySite_LYV>();

            foreach (System.Data.DataRow row in ta.Rows)
            {
				list.Add(new Model_ManySite_LYV
				{
					XXCC = row["xxcc"].ToString(),

					GXXCC = row["Gxxcc"].ToString(),

					GCount=int.Parse(row["count"].ToString())
				});
            }
			return list;
        }
		public List<Barcode_Edit_LVY> Data_into_inputTool(string barcode)
        {
			var s = new List<Barcode_Edit_LVY>();
			
			string sql = $"SELECT Tua,Bwbh,CLBH,Gxxcc,Prono,convert(int,ActualQty) [ActualQty],convert(int,planqty)[planQty] FROM App_Cutting_Barcodes_Groups_Edit where barcode = '{barcode}'";

			var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta.Rows.Count > 0)
            {
                foreach (System.Data.DataRow r in ta.Rows)
                {					
					s.Add(new Barcode_Edit_LVY
					{
						Barcode = barcode,
						BWBH = r["BWBH"].ToString(),
						CLBH = r["CLBH"].ToString(),
						GXXCC = r["Gxxcc"].ToString(),
						Prono = r["Prono"].ToString(),
						LocalQty = int.Parse(r["ActualQty"].ToString()),
						Target = int.Parse(r["planQty"].ToString()),
						Tua = r["Tua"].ToString(),
						
                        Isfull = int.Parse(r["ActualQty"].ToString()) == int.Parse(r["planQty"].ToString())
					});
                }
            }
			return s;
        }
		public List<Model_GrpSize_LYV> Group_Size()
        {
		

		var	sql = $"SELECT gxxcc, count(gxxcc) count FROM(select distinct XXCC, case when ISNULL(GXXCC,'')<> '' then Cutting_Barcodes_SEQ.GXXCC ELSE XXCC END GXXCC from Cutting_Barcodes_SEQ where barcode = '{DB.StoreLocal.Instant.Barcode}' ) s group by gxxcc";

			var l = new List<Model_GrpSize_LYV>();

			var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

            if (ta.Rows.Count>0)
            {
                foreach (System.Data.DataRow r in ta.Rows)
                {
					l.Add(new Model_GrpSize_LYV
					{
						GXXCC = r["Gxxcc"].ToString(),
						Count = int.Parse(r["count"].ToString())
					});
                }
            }
			return l;
        }
		public (List<Model_DataDetail_LYV> ListData,System.Data.DataTable DataTable, List<Model_Data_Ry> DataRY )Data()
        {
			var l = new List<Model_DataDetail_LYV>();
			var lst_Data_ry = new List<Model_Data_Ry>();
			#region old SQL get table detail
			string sql = "DECLARE @Barcode VARCHAR(15) ,@ZLBH VARCHAR(15)";
	sql += "	, @CLBH VARCHAR(15), @BWBH VARCHAR(10), @Layers varchar(10), @QtyRY money, @DMQty money, @SH INT, @Count INT";
			sql += $"	SET @Barcode = '{DB.StoreLocal.Instant.Barcode}'";
			sql += "	SET @Layers = '4'";
			sql += "	SET @QtyRY = (select SUM(A.Qty) Qty";

			sql += $"	from(SELECT Barcode, Qty FROM Cutting_Barcodes GROUP BY Barcode, ZLBH, Qty) A WHERE A.Barcode = @Barcode)";
			sql += " SET @DMQty = (SELECT SUM(CASE WHEN clzl.dwbh <> 'M' THEN DMQty * 0.9144 ELSE DMQty END) DMQty ";
			sql += "			FROM Cutting_Barcodes CBS ";

			sql += "			LEFT JOIN clzl on clzl.cldh = CBS.CLBH";

			sql += $"			WHERE Barcode = @Barcode)";
			sql += " SET @SH = (SELECT SH ";
			sql += " 	 FROM Cutting_Barcodes3";
			sql += $" 	 WHERE Barcode = @Barcode";

			sql += "  GROUP BY SH)";
			sql += $" SET @Count = (SELECT COUNT(Barcode) FROM Cutting_Barcodes3 WHERE Barcode = @Barcode)";
			sql += " if object_id('tempdb..#Cutting_SEQ') is not null begin drop table #Cutting_SEQ end";
			sql += " SELECT cbs.Barcode, cbs.ZLBH, cbs.XXCC, cbs.SEQ, SUM(cbs.Qty) / @Count Qty, cast(@Layers as INT) layers, @SH SH";
			sql += " , ((@DMQty) * (SUM(cbs.Qty) / @Count))/ (@QtyRY)AS CLSL,CONVERT(NUMERIC(18, 0), cl.DAILY_CT)DAILY_CT";
			sql += " ,CASE WHEN RIGHT(cbs.SEQ, 1) = 'a' THEN 1 WHEN RIGHT(cbs.SEQ,1)= 'b' THEN 2 WHEN ISNUMERIC(cbs.SEQ)= 1 THEN 4 ELSE 3 END TT";
			sql += " INTO #Cutting_SEQ ";
			sql += " FROM Cutting_Barcodes_SEQ AS cbs";
			sql += " LEFT JOIN CheckLLasts cl ON cl.ZLBH = cbs.ZLBH";
			sql += " WHERE cbs.Barcode = @Barcode";
			sql += " GROUP BY cbs.Barcode, cbs.ZLBH, cbs.XXCC, cbs.SEQ,cl.DAILY_CT";
			sql += " if object_id('tempdb..#Cutting_SEQ_G') is not null begin drop table #Cutting_SEQ_G end";
			sql += " SELECT cbs.Barcode, cbs.ZLBH, cbs.SEQ, SUM(cbs.Qty) / @Count Qty, cast(@Layers as INT) layers, @SH SH";
			sql += " , ((@DMQty) * (SUM(cbs.Qty) / @Count))/ (@QtyRY)CLSL,CONVERT(NUMERIC(18, 0), cl.DAILY_CT)DAILY_CT";

			sql += " ,CASE WHEN RIGHT(cbs.SEQ, 1) = 'a' THEN 1 WHEN RIGHT(cbs.SEQ,1)= 'b' THEN 2 WHEN ISNUMERIC(cbs.SEQ)= 1 THEN 4 ELSE 3 END TT, cbs.GXXCC,B.CC";
			sql += " INTO #Cutting_SEQ_G";
			sql += " FROM Cutting_Barcodes_SEQ AS cbs";
			sql += " LEFT JOIN CheckLLasts cl ON cl.ZLBH = cbs.ZLBH";
			sql += " LEFT JOIN(SELECT cbs2.Barcode, cbs2.GXXCC, MIN(cbs2.XXCC) CC FROM Cutting_Barcodes_SEQ AS cbs2";

			sql += " GROUP BY cbs2.Barcode, cbs2.GXXCC) B ON B.Barcode = cbs.Barcode AND B.GXXCC = cbs.GXXCC";
			sql += " WHERE cbs.Barcode = @Barcode";
			sql += " GROUP BY cbs.Barcode, cbs.ZLBH, cbs.SEQ,cl.DAILY_CT,cbs.GXXCC,B.CC";


			sql += " if object_id('tempdb..#ChemicalSize') is not null";
			sql += " begin drop table #ChemicalSize end";
			sql += " SELECT ROW_NUMBER() OVER(ORDER BY ZLBH, XXCC DESC)ID,ZLBH,XXCC INTO #ChemicalSize";
			sql += " FROM #Cutting_SEQ";
			sql += " LEFT JOIN Cutting_Barcode AS cb ON cb.Barcode =#Cutting_SEQ.Barcode";
			sql += " LEFT JOIN xxzl AS x ON x.Article = cb.Article";
			sql += " WHERE RIGHT(XXCC,1)= 'K' OR XXCC<= x.BZCC";
			sql += " GROUP BY ZLBH,XXCC";
			sql += " HAVING SUM(Qty) > 0";
			sql += " CREATE CLUSTERED INDEX cx_ChemicalSize ON #ChemicalSize(ID,ZLBH)";




			sql += " if object_id('tempdb..#Cutting_SEQ_S') is not null begin drop table #Cutting_SEQ_S end ";
			sql += " SELECT Barcode,SEQ,SUM(qty) Qty,SUM(CLSL) CLSL,Layers,SH,GXXCC,CC,TT";
			sql += " INTO #Cutting_SEQ_S";
			sql += " FROM #Cutting_SEQ_G";
			sql += " GROUP BY Barcode, SEQ, Layers, SH, GXXCC, CC, TT";



			sql += " DECLARE @sql VARCHAR(8000),@sql1 VARCHAR(8000),@sql2 VARCHAR(8000),@sql3 VARCHAR(8000)";
			sql += " SET @sql = 'SELECT pp.Barcode,pp.SEQ,pp.SEQ SEQShow,''1'' STT,''TSL'' Type,len(pp.SEQ) GrpMany,pp.TT'";
			sql += " SELECT @sql = @sql + ',SUM(CASE [CC] WHEN ''' +[CC] + ''' THEN pp.[Qty]  END) AS [' + CC + ']'";
			sql += " FROM #Cutting_SEQ_S a";
			sql += " GROUP BY[CC]";
			sql += " ORDER BY[CC] ASC";
			sql += " SET @sql = @sql + ' ,convert(int, SUM(pp.Qty)) AS Total";

			sql += "          ,cast(ROUND((SUM(pp.CLSL) / pp.Layers), 4) as numeric(18, 4)) AS Usage";
			sql += " FROM #Cutting_SEQ_S pp";
			sql += " GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT";
			sql += " UNION ALL";
			sql += " '	 ";
sql += " SET @sql1 = ' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''2'' STT ,''SL'' Type,len(pp.SEQ),pp.TT'";
			sql += " SELECT @sql1 = @sql1 + ',SUM(CASE [CC] WHEN ''' +[CC] + ''' THEN (CASE WHEN (pp.[Qty] - pp.[Qty]%(Layers/2)<>0) THEN (CASE WHEN SH=4 and Layers=4 THEN pp.[Qty] ELSE pp.[Qty] - pp.[Qty]%(Layers/2) END) ELSE pp.[Qty] END)  END) AS [' + CC + ']'";
			sql += " FROM #Cutting_SEQ_S a";
			sql += " GROUP BY[CC]";
			sql += " ORDER BY[CC] ASC";
			sql += " SET @sql1 = @sql1 + ' ,NULL AS Total";

			sql += "            ,NULL AS Usage";
			sql += "  	FROM #Cutting_SEQ_S pp";
			sql += "  	GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT";
			sql += "  	UNION ALL";
			sql += " '	 ";
			sql += "  	SET @sql2 = ' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''3'' STT ,''SH'' Type,len(pp.SEQ),pp.TT'";
	sql += "  	SELECT @sql2 = @sql2 + ',SUM(CASE [CC] WHEN ''' +[CC] + ''' THEN (CASE WHEN (pp.[Qty] - pp.[Qty]%(Layers/2)<>0) THEN (CASE WHEN SH=4 and Layers=4 THEN pp.[Qty]*SH ELSE (pp.[Qty] - pp.[Qty]%(Layers/2))*SH END) ELSE pp.[Qty]*SH END) END) AS [' + CC + ']'";
			sql += "  	FROM #Cutting_SEQ_S a";
			sql += "  	GROUP BY[CC]";
			sql += "  	ORDER BY[CC] ASC";
			sql += "  	SET @sql2 = @sql2 + ' ,NULL AS Total";
			sql += "      ,NULL AS Usage";
		sql += " FROM #Cutting_SEQ_S pp";
			sql += " GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT";
			sql += " UNION ALL";
			sql += "'			";
sql += " SET @sql3 = ' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''4'' STT,''CT'' Type,len(pp.SEQ),pp.TT'";
			sql += " SELECT @sql3 = @sql3 + ',SUM(CASE [CC] WHEN ''' +[CC] + ''' THEN (CASE WHEN (Layers%2)=0 THEN (CASE WHEN SH=4 and Layers=4 THEN 0 ELSE pp.[Qty]%(Layers/2) END) ELSE pp.[Qty] END)  END) AS [' + CC + ']'";
			sql += " FROM #Cutting_SEQ_S a";
			sql += " GROUP BY[CC]";
sql += " ORDER BY[CC] ASC";
			sql += " SET @sql3 = @sql3 + ' ,NULL AS Total";

			sql += "            ,NULL AS Usage";
			sql += " FROM #Cutting_SEQ_S pp";
			sql += " GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT";
			sql += " ORDER BY pp.TT,len(pp.SEQ),pp.SEQ,STT'			";
sql += " EXEC(@sql + @sql1 + @sql2 + @sql3)";
			#endregion

			#region New SQL




			sql = $"DECLARE @Barcode VARCHAR(15),@ZLBH VARCHAR(15),@CLBH VARCHAR(15),@BWBH VARCHAR(10),@Layers VARCHAR(10),@QtyRY MONEY,@DMQty MONEY,@SH INT,@Count INT;SET @Barcode = '{DB.StoreLocal.Instant.Barcode}';SET @Layers = '4';SET @QtyRY =(SELECT SUM(A.Qty) Qty FROM(SELECT Barcode, Qty FROM Cutting_Barcodes GROUP BY Barcode, ZLBH, Qty) A     WHERE A.Barcode = @Barcode);SET @DMQty =(SELECT SUM(   CASE WHEN clzl.dwbh <> 'M' THEN DMQty * 0.9144 ELSE DMQty END ) DMQty FROM Cutting_Barcodes CBS LEFT JOIN clzl ON clzl.cldh = CBS.CLBH WHERE Barcode = @Barcode ); SET @SH =( SELECT SH FROM Cutting_Barcodes3 WHERE Barcode = @Barcode GROUP BY SH );SET @Count =(SELECT COUNT(Barcode)FROM Cutting_Barcodes3 WHERE Barcode = @Barcode);IF OBJECT_ID('tempdb..#Cutting_SEQ') IS NOT NULL BEGIN DROP TABLE #Cutting_SEQ;END;SELECT cbs.Barcode,cbs.ZLBH,cbs.XXCC,cbs.SEQ,SUM(cbs.Qty) / @Count Qty,CAST(@Layers AS INT) layers,@SH SH,((@DMQty) * (SUM(cbs.Qty) / @Count)) / (@QtyRY) AS CLSL,CONVERT(NUMERIC(18, 0), cl.DAILY_CT) DAILY_CT,CASE WHEN RIGHT(cbs.SEQ, 1) = 'a' THEN 1 WHEN RIGHT(cbs.SEQ, 1) = 'b' THEN 2 WHEN ISNUMERIC(cbs.SEQ) = 1 THEN 4 ELSE 3 END TT INTO #Cutting_SEQ FROM Cutting_Barcodes_SEQ AS cbs LEFT JOIN CheckLLasts cl ON cl.ZLBH = cbs.ZLBH WHERE cbs.Barcode = @Barcode GROUP BY cbs.Barcode, cbs.ZLBH, cbs.XXCC, cbs.SEQ, cl.DAILY_CT; IF OBJECT_ID('tempdb..#Cutting_SEQ_G') IS NOT NULL BEGIN DROP TABLE #Cutting_SEQ_G;END; SELECT cbs.Barcode, cbs.ZLBH, cbs.SEQ, SUM(cbs.Qty) / @Count Qty, CAST(@Layers AS INT) layers, @SH SH, ((@DMQty) * (SUM(cbs.Qty) / @Count)) / (@QtyRY) CLSL, CONVERT(NUMERIC(18, 0), cl.DAILY_CT) DAILY_CT, CASE WHEN RIGHT(cbs.SEQ, 1) = 'a' THEN 1 WHEN RIGHT(cbs.SEQ, 1) = 'b' THEN 2 WHEN ISNUMERIC(cbs.SEQ) = 1 THEN 4 ELSE 3 END TT, cbs.GXXCC, B.CC INTO #Cutting_SEQ_G FROM Cutting_Barcodes_SEQ AS cbs LEFT JOIN CheckLLasts cl ON cl.ZLBH = cbs.ZLBH LEFT JOIN ( SELECT cbs2.Barcode, cbs2.GXXCC, MIN(cbs2.XXCC) CC FROM Cutting_Barcodes_SEQ AS cbs2 GROUP BY cbs2.Barcode, cbs2.GXXCC ) B ON B.Barcode = cbs.Barcode AND B.GXXCC = cbs.GXXCC WHERE cbs.Barcode = @Barcode GROUP BY cbs.Barcode, cbs.ZLBH, cbs.SEQ, cl.DAILY_CT, cbs.GXXCC, B.CC; IF OBJECT_ID('tempdb..#ChemicalSize') IS NOT NULL BEGIN DROP TABLE #ChemicalSize; END; SELECT ROW_NUMBER() OVER (ORDER BY ZLBH, XXCC DESC) ID, ZLBH, XXCC INTO #ChemicalSize FROM #Cutting_SEQ LEFT JOIN Cutting_Barcode AS cb ON cb.Barcode = #Cutting_SEQ.Barcode LEFT JOIN xxzl AS x ON x.ARTICLE = cb.Article WHERE RIGHT(XXCC, 1) = 'K'OR XXCC <= x.BZCC GROUP BY ZLBH, XXCC HAVING SUM(Qty) > 0; CREATE CLUSTERED INDEX cx_ChemicalSize ON #ChemicalSize (ID, ZLBH); IF OBJECT_ID('tempdb..#Cutting_SEQ_S') IS NOT NULL BEGIN DROP TABLE #Cutting_SEQ_S; END; SELECT Barcode, SEQ, SUM(Qty) Qty,SUM(CLSL) CLSL, layers, SH, GXXCC, CC, TT INTO #Cutting_SEQ_S FROM #Cutting_SEQ_G GROUP BY Barcode, SEQ, layers, SH, GXXCC, CC,TT; DECLARE @sql VARCHAR(8000), @sql1 VARCHAR(8000), @sql2 VARCHAR(8000), @sql3 VARCHAR(8000); SET @sql = 'SELECT pp.Barcode,pp.SEQ,pp.SEQ SEQShow,''1'' STT,''TSL'' Type,len(pp.SEQ) GrpMany,pp.TT'; SELECT @sql = @sql + ',Convert(int, isnull( SUM(CASE [CC] WHEN ''' + [CC] + ''' THEN pp.[Qty]  END),0)) AS [' + CC + ']' FROM #Cutting_SEQ_S a GROUP BY [CC] ORDER BY [CC] ASC; SET @sql = @sql      + ' ,convert(int, SUM(pp.Qty)) AS Total          ,cast(ROUND((SUM(pp.CLSL) / pp.Layers), 4) as numeric(18, 4)) AS Usage FROM #Cutting_SEQ_S pp GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT UNION ALL '; SET @sql1 = ' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''2'' STT ,''SL'' Type,len(pp.SEQ),pp.TT';SELECT @sql1= @sql1 + ',Convert(int, isnull( SUM(CASE [CC] WHEN ''' + [CC]+ ''' THEN (CASE WHEN (pp.[Qty] - pp.[Qty]%(Layers/2)<>0) THEN (CASE WHEN SH=4 and Layers=4 THEN pp.[Qty] ELSE pp.[Qty] - pp.[Qty]%(Layers/2) END) ELSE pp.[Qty] END)  END),0)) AS [' + CC + ']' FROM #Cutting_SEQ_S a GROUP BY [CC]ORDER BY [CC] ASC; SET @sql1 = @sql1 + ' ,0 AS Total            ,0 AS Usage  	FROM #Cutting_SEQ_S pp  	GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT  	UNION ALL '; SET @sql2 = ' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''3'' STT ,''SH'' Type,len(pp.SEQ),pp.TT'; SELECT @sql2 = @sql2 + ',Convert(int, isnull( SUM(CASE [CC] WHEN ''' + [CC] + ''' THEN (CASE WHEN (pp.[Qty] - pp.[Qty]%(Layers/2)<>0) THEN (CASE WHEN SH=4 and Layers=4 THEN pp.[Qty]*SH ELSE (pp.[Qty] - pp.[Qty]%(Layers/2))*SH END) ELSE pp.[Qty]*SH END) END),0)) AS [' + CC + ']' FROM #Cutting_SEQ_S a GROUP BY [CC] ORDER BY [CC] ASC; SET @sql2 = @sql2 + ' ,0 AS Total      ,0 AS Usage FROM #Cutting_SEQ_S pp GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT UNION ALL';SET @sql3 = ' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''4'' STT,''CT'' Type,len(pp.SEQ),pp.TT';SELECT @sql3= @sql3 + ',Convert(int, isnull( SUM(CASE [CC] WHEN ''' + [CC]+ ''' THEN (CASE WHEN (Layers%2)=0 THEN (CASE WHEN SH=4 and Layers=4 THEN 0 ELSE pp.[Qty]%(Layers/2) END) ELSE pp.[Qty] END)  END),0)) AS ['+ CC + ']'FROM #Cutting_SEQ_S a GROUP BY [CC] ORDER BY [CC] ASC; SET @sql3 = @sql3 + ' ,0 AS Total, 0 AS Usage FROM #Cutting_SEQ_S pp GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT ORDER BY pp.TT,len(pp.SEQ),pp.SEQ,STT'; EXEC (@sql + @sql1 + @sql2 + @sql3);  ";





			#endregion

			#region new new SQL
			sql = $@"DECLARE @Barcode VARCHAR(15),
        @ZLBH    VARCHAR(15),
        @CLBH    VARCHAR(15),
        @BWBH    VARCHAR(10),
        @Layers  VARCHAR(10),
        @QtyRY   MONEY,
        @DMQty   MONEY,
        @SH      INT,
        @Count   INT;

SET @Barcode = '{DB.StoreLocal.Instant.Barcode}';
SET @Layers = (SELECT Layers FROM cutting_barcode where Barcode = @Barcode);
SET @QtyRY =(SELECT Sum(A.qty) Qty
             FROM  (SELECT barcode,
                           qty
                    FROM   cutting_barcodes
                    GROUP  BY barcode,
                              zlbh,
                              qty) A
             WHERE  A.barcode = @Barcode);
SET @DMQty =(SELECT Sum(CASE
                          WHEN clzl.dwbh <> 'M' THEN dmqty * 0.9144
                          ELSE dmqty
                        END) DMQty
             FROM   cutting_barcodes CBS
                    LEFT JOIN clzl
                           ON clzl.cldh = CBS.clbh
             WHERE  barcode = @Barcode);
SET @SH =(SELECT sh
          FROM   cutting_barcodes3
          WHERE  barcode = @Barcode
          GROUP  BY sh);
SET @Count =(SELECT Count(barcode)
             FROM   cutting_barcodes3
             WHERE  barcode = @Barcode);

IF Object_id('tempdb..#Cutting_SEQ') IS NOT NULL
  BEGIN
      DROP TABLE #cutting_seq;
  END;

SELECT cbs.barcode,
       cbs.zlbh,
       cbs.xxcc,
       cbs.seq,
       Sum(cbs.qty) / @Count                                   Qty,
       Cast(@Layers AS INT)                                    layers,
       @SH                                                     SH,
       ( ( @DMQty ) * ( Sum(cbs.qty) / @Count ) ) / ( @QtyRY ) AS CLSL,
       CONVERT(NUMERIC(18, 0), cl.daily_ct)                    DAILY_CT,
       CASE
         WHEN RIGHT(cbs.seq, 1) = 'a' THEN 1
         WHEN RIGHT(cbs.seq, 1) = 'b' THEN 2
         WHEN Isnumeric(cbs.seq) = 1 THEN 4
         ELSE 3
       END                                                     TT
INTO   #cutting_seq
FROM   (select barcode ,ZLBH,CLBH,BWBH,XXCC,case when isnull(GXXCC,'')<>'' then GXXCC else XXCC end GXXCC ,SEQ,Qty,SEQ_Plan,ReSize from cutting_barcodes_seq) AS cbs
       LEFT JOIN checkllasts cl
              ON cl.zlbh = cbs.zlbh
WHERE  cbs.barcode = @Barcode
GROUP  BY cbs.barcode,
          cbs.zlbh,
          cbs.xxcc,
          cbs.seq,
          cl.daily_ct;

IF Object_id('tempdb..#Cutting_SEQ_G') IS NOT NULL
  BEGIN
      DROP TABLE #cutting_seq_g;
  END;

SELECT cbs.barcode,
       cbs.zlbh,
       cbs.seq,
       Sum(cbs.qty) / @Count                                   Qty,
       Cast(@Layers AS INT)                                    layers,
       @SH                                                     SH,
       ( ( @DMQty ) * ( Sum(cbs.qty) / @Count ) ) / ( @QtyRY ) CLSL,
       CONVERT(NUMERIC(18, 0), cl.daily_ct)                    DAILY_CT,
       CASE
         WHEN RIGHT(cbs.seq, 1) = 'a' THEN 1
         WHEN RIGHT(cbs.seq, 1) = 'b' THEN 2
         WHEN Isnumeric(cbs.seq) = 1 THEN 4
         ELSE 3
       END                                                     TT,
       cbs.gxxcc ,
       B.cc
INTO   #cutting_seq_g
FROM   ( select  barcode,ZLBH,BWBH,CLBH,Qty,XXCC, SEQ,
 case when isnull(GXXCC,'') <>'' then GXXCC else XXCC end GXXCC from cutting_barcodes_seq) AS cbs
       LEFT JOIN checkllasts cl
              ON cl.zlbh = cbs.zlbh
       LEFT JOIN (SELECT cbs2.barcode,
                         case when isnull(cbs2.gxxcc,'')<>'' then cbs2.gxxcc else cbs2.xxcc end gxxcc,
                         Min(cbs2.xxcc) CC
                  FROM   cutting_barcodes_seq AS cbs2				 
                  GROUP  BY cbs2.barcode,
                            case when isnull(cbs2.gxxcc,'')<>'' then cbs2.gxxcc else cbs2.xxcc end  ) B
              ON B.barcode = cbs.barcode
                 AND B.gxxcc = cbs.gxxcc
WHERE  cbs.barcode = @Barcode
GROUP  BY cbs.barcode,
          cbs.zlbh,
          cbs.seq,
          cl.daily_ct,
          cbs.GXXCC,
          B.cc;

		

IF Object_id('tempdb..#ChemicalSize') IS NOT NULL
  BEGIN
      DROP TABLE #chemicalsize;
  END;

SELECT Row_number()
         OVER (
           ORDER BY zlbh, xxcc DESC) ID,

       zlbh,
       xxcc
INTO   #chemicalsize
FROM   #cutting_seq
       LEFT JOIN cutting_barcode AS cb
              ON cb.barcode = #cutting_seq.barcode
       LEFT JOIN xxzl AS x
              ON x.article = cb.article
WHERE  RIGHT(xxcc, 1) = 'K'
        OR cast(xxcc as float) <=cast( x.bzcc as float)
GROUP  BY zlbh,
          xxcc
HAVING Sum(qty) > 0;

CREATE CLUSTERED INDEX cx_chemicalsize
  ON #chemicalsize (id, zlbh);

IF Object_id('tempdb..#Cutting_SEQ_S') IS NOT NULL
  BEGIN
      DROP TABLE #cutting_seq_s;
  END;

SELECT barcode,
       seq,
       Sum(qty)  Qty,
       Sum(clsl) CLSL,
       layers,
       sh,
       gxxcc,
       cc,
       tt
INTO   #cutting_seq_s
FROM   #cutting_seq_g
GROUP  BY barcode,
          seq,
          layers,
          sh,
          gxxcc,
          cc,
          tt;
 

DECLARE @sql  VARCHAR(8000),
        @sql1 VARCHAR(8000),
        @sql2 VARCHAR(8000),
        @sql3 VARCHAR(8000);

SET @sql =
'SELECT pp.Barcode,pp.SEQ,pp.SEQ SEQShow,''1'' STT,''TSL'' Type,len(pp.SEQ) GrpMany,pp.TT'
;

SELECT @sql = @sql
              + ',Convert(int, isnull( SUM(CASE [CC] WHEN '''
              + [cc] + ''' THEN pp.[Qty]  END),0)) AS [' + cc
              + ']'
FROM   #cutting_seq_s a
GROUP  BY [cc]
ORDER  BY [cc] ASC;

SET @sql = @sql
           +
' ,convert(int, SUM(pp.Qty)) AS Total          ,cast(ROUND((SUM(pp.CLSL) / pp.Layers), 4) as numeric(18, 4)) AS Usage FROM #Cutting_SEQ_S pp GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT UNION ALL '
;
SET @sql1 =
' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''2'' STT ,''SL'' Type,len(pp.SEQ),pp.TT'
;

SELECT @sql1 = @sql1
               + ',Convert(int, isnull( SUM(CASE [CC] WHEN '''
               + [cc]
               +
''' THEN (CASE WHEN (pp.[Qty] - pp.[Qty]%(Layers/2)<>0) THEN (CASE WHEN SH=4 and Layers=4 THEN pp.[Qty] ELSE pp.[Qty] - pp.[Qty]%(Layers/2) END) ELSE pp.[Qty] END)  END),0)) AS ['
        + cc + ']'
FROM   #cutting_seq_s a
GROUP  BY [cc]
ORDER  BY [cc] ASC;

SET @sql1 = @sql1
            +
' ,0 AS Total            ,0 AS Usage   FROM #Cutting_SEQ_S pp   GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT   UNION ALL '
;
SET @sql2 =
' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''3'' STT ,''SH'' Type,len(pp.SEQ),pp.TT'
;

SELECT @sql2 = @sql2
               + ',Convert(int, isnull( SUM(CASE [CC] WHEN '''
               + [cc]
               +
''' THEN (CASE WHEN (pp.[Qty] - pp.[Qty]%(Layers/2)<>0) THEN (CASE WHEN SH=4 and Layers=4 THEN pp.[Qty]*SH ELSE (pp.[Qty] - pp.[Qty]%(Layers/2))*SH END) ELSE pp.[Qty]*SH END) END),0)) AS ['
        + cc + ']'
FROM   #cutting_seq_s a
GROUP  BY [cc]
ORDER  BY [cc] ASC;

SET @sql2 = @sql2
            +
' ,0 AS Total      ,0 AS Usage FROM #Cutting_SEQ_S pp GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT UNION ALL'
;
SET @sql3 =
' SELECT pp.Barcode,pp.SEQ,Null SEQShow,''4'' STT,''CT'' Type,len(pp.SEQ),pp.TT'
;

SELECT @sql3 = @sql3
               + ',Convert(int, isnull( SUM(CASE [CC] WHEN '''
               + [cc]
               +
''' THEN (CASE WHEN (Layers%2)=0 THEN (CASE WHEN SH=4 and Layers=4 THEN 0 ELSE pp.[Qty]%(Layers/2) END) ELSE pp.[Qty] END)  END),0)) AS ['
        + cc + ']'
FROM   #cutting_seq_s a
GROUP  BY [cc]
ORDER  BY [cc] ASC;

SET @sql3 = @sql3
            +
' ,0 AS Total, 0 AS Usage FROM #Cutting_SEQ_S pp GROUP BY pp.Barcode,pp.SEQ,pp.Layers,pp.TT ORDER BY pp.TT,len(pp.SEQ),pp.SEQ,STT'
;

exec (@sql + @sql1 + @sql2 + @sql3);  
";
			#endregion

			string sql_RY = $@"DECLARE @Barcode VARCHAR(15),

		@ZLBH VARCHAR(15),
        @CLBH VARCHAR(15),
        @BWBH VARCHAR(10),
        @Layers VARCHAR(10),
        @QtyRY MONEY,
		@DMQty   MONEY,
        @SH INT,
		@Count   INT;

			SET @Barcode = '{DB.StoreLocal.Instant.Barcode}';
			SET @Layers = (SELECT Layers FROM cutting_barcode where Barcode = @Barcode)
SET @QtyRY = (SELECT Sum(A.qty) Qty

			 FROM(SELECT barcode,
						   qty

					FROM   cutting_barcodes

					GROUP  BY barcode,
							  zlbh,
							  qty) A
			 WHERE  A.barcode = @Barcode);
			SET @DMQty = (SELECT Sum(CASE
			 
									   WHEN clzl.dwbh <> 'M' THEN dmqty * 0.9144
			 
									   ELSE dmqty
			 
									 END) DMQty
						  FROM   cutting_barcodes CBS

					LEFT JOIN clzl
						   ON clzl.cldh = CBS.clbh

			 WHERE barcode = @Barcode);
			SET @SH = (SELECT sh
					   FROM   cutting_barcodes3
					   WHERE  barcode = @Barcode

		  GROUP BY sh);
			SET @Count = (SELECT Count(barcode)

			 FROM cutting_barcodes3

			 WHERE barcode = @Barcode);
			SELECT cbs.barcode,
       cbs.zlbh,
       cbs.seq,
        convert(int, Sum(cbs.qty) / @Count)                         Qty,
       Cast(@Layers AS INT)                                    layers,
       @SH SH,
       ( (@DMQty) * (Sum(cbs.qty) / @Count) ) / (@QtyRY)CLSL,
       CONVERT(NUMERIC(18, 0), cl.daily_ct)                    DAILY_CT,
       CASE
		 WHEN RIGHT(cbs.seq, 1) = 'a' THEN 1

		 WHEN RIGHT(cbs.seq, 1) = 'b' THEN 2

		 WHEN Isnumeric(cbs.seq) = 1 THEN 4

		 ELSE 3

	   END TT,
	   cbs.gxxcc ,
       B.cc

FROM(select  barcode, ZLBH, BWBH, CLBH, Qty, XXCC, SEQ,
 case when isnull(GXXCC, '') <> '' then GXXCC else XXCC end GXXCC from cutting_barcodes_seq) AS cbs

	   LEFT JOIN checkllasts cl

			  ON cl.zlbh = cbs.zlbh

	   LEFT JOIN(SELECT cbs2.barcode,
                         case when isnull(cbs2.gxxcc,'')<> '' then cbs2.gxxcc else cbs2.xxcc end gxxcc,
                         Min(cbs2.xxcc) CC
				  FROM   cutting_barcodes_seq AS cbs2
				  GROUP  BY cbs2.barcode,
                            case when isnull(cbs2.gxxcc,'')<> '' then cbs2.gxxcc else cbs2.xxcc end  ) B
			   ON B.barcode = cbs.barcode

				 AND B.gxxcc = cbs.gxxcc
WHERE cbs.barcode = @Barcode
GROUP BY cbs.barcode,
          cbs.zlbh,
          cbs.seq,
          cl.daily_ct,
          cbs.GXXCC,
          B.cc";

			var ta = DB.SQL.ConnectDB.Connection.FillDataTable(sql);

			var ListRY = DB.SQL.ConnectDB.Connection.FillDataTable(sql_RY);

			if (ta.Rows.Count > 0)
			{

				var tua = 0;
				string seq = "";
				int countTua = 0;

				for (int i = 0; i < ta.Rows.Count; i++)
                {					
					var model = new Model_DataDetail_LYV();
					
					for (int j = 0; j < ta.Columns.Count; j++)
					{
						var ColumnName = ta.Columns[j].ColumnName;

						switch (ColumnName)
						{						

							case "SEQ":

								model.Batch = ta.Rows[i][ColumnName].ToString();

								var seqTa = ta.Rows[i][ColumnName].ToString();
								var a = l;
								if (seq != seqTa)
								{
									//countTua++;

									//seq = seqTa;

									//if (countTua <= 1)
									//{

									//	model.Pages = tua;
									//                           }
									//                           else
									//                           {

									//	tua++;

									//	model.Pages = tua;

									//	countTua = 0;
									//}

									seq = seqTa;

									tua++;
									model.Pages = tua;
								}
								else model.Pages = tua;
                               

								break;

							case "Type":
								model.Type = ta.Rows[i][ColumnName].ToString();
								break;

							case "Total":
								model.Total = string.IsNullOrEmpty(ta.Rows[i][ColumnName].ToString()) ? "" : ta.Rows[i][ColumnName].ToString();
								break;

							case "Usage":
								model.Usage = string.IsNullOrEmpty(ta.Rows[i][ColumnName].ToString()) ? "" : ta.Rows[i][ColumnName].ToString();
								break;

							case "01.0":
								model.Size_01 = ta.Rows[i][ColumnName].ToString();
								break;

							case "01.0K":
								model.Size_01k = ta.Rows[i][ColumnName].ToString();
								break;

							case "01.5":
								model.Size_015 = ta.Rows[i][ColumnName].ToString();
								break;

							case "01.5K":
								model.Size_015k = ta.Rows[i][ColumnName].ToString();
								break;

							case "02.0":
								model.Size_02 = ta.Rows[i][ColumnName].ToString();
								break;

							case "02.0K":
								model.Size_02k = ta.Rows[i][ColumnName].ToString();
								break;	

							case "02.5":
								model.Size_025 = ta.Rows[i][ColumnName].ToString();
								break;

							case "02.5K":
								model.Size_025k = ta.Rows[i][ColumnName].ToString();
								break;

							case "03.0":
								model.Size_03 = ta.Rows[i][ColumnName].ToString();
								break;

							case "03.0K":
								model.Size_03k = ta.Rows[i][ColumnName].ToString();
								break;

							case "03.5":
								model.Size_035 = ta.Rows[i][ColumnName].ToString();
								break;

							case "03.5K":
								model.Size_035k = ta.Rows[i][ColumnName].ToString();
								break;

							case "04.0":
								model.Size_04 = ta.Rows[i][ColumnName].ToString();
								break;


							case "04.0K":
								model.Size_04k = ta.Rows[i][ColumnName].ToString();
								break;

							case "04.5":
								model.Size_045 = ta.Rows[i][ColumnName].ToString();
								break;

							case "04.5K":
								model.Size_045k = ta.Rows[i][ColumnName].ToString();
								break;


							case "05.0":
								model.Size_05 = ta.Rows[i][ColumnName].ToString();
								break;

							case "05.0K":
								model.Size_05k = ta.Rows[i][ColumnName].ToString();
								break;

							case "05.5":
								model.Size_055 = ta.Rows[i][ColumnName].ToString();
								break;

							case "05.5K":
								model.Size_055k = ta.Rows[i][ColumnName].ToString();
								break;

							case "06.0":
								model.Size_06 = ta.Rows[i][ColumnName].ToString();
								break;
							
							case "06.0K":
								model.Size_06k = ta.Rows[i][ColumnName].ToString();
								break;


							case "06.5":
								model.Size_065 = ta.Rows[i][ColumnName].ToString();
								break;


							case "06.5K":
								model.Size_065k = ta.Rows[i][ColumnName].ToString();
								break;

							case "07.0":
								model.Size_07 = ta.Rows[i][ColumnName].ToString();
								break;

							case "07.0K":
								model.Size_07k = ta.Rows[i][ColumnName].ToString();
								break;

							case "07.5":
								model.Size_075 = ta.Rows[i][ColumnName].ToString();
								break;

							case "07.5K":
								model.Size_075k = ta.Rows[i][ColumnName].ToString();
								break;

							case "08.0":
								model.Size_08 = ta.Rows[i][ColumnName].ToString();
								break;


							case "08.0K":
								model.Size_08k = ta.Rows[i][ColumnName].ToString();
								break;

							case "08.5":
								model.Size_085 = ta.Rows[i][ColumnName].ToString();
								break;

							case "08.5K":
								model.Size_085k = ta.Rows[i][ColumnName].ToString();
								break;


							case "09.0":
								model.Size_09 = ta.Rows[i][ColumnName].ToString();
								break;


							case "09.0K":
								model.Size_09k = ta.Rows[i][ColumnName].ToString();
								break;

							case "09.5":
								model.Size_095 = ta.Rows[i][ColumnName].ToString();
								break;

							case "09.5K":
								model.Size_095k = ta.Rows[i][ColumnName].ToString();
								break;

							case "10.0":
								model.Size_10 = ta.Rows[i][ColumnName].ToString();
								break;

							case "10.0K":
								model.Size_10k = ta.Rows[i][ColumnName].ToString();
								break;

							case "10.5":
								model.Size_105 = ta.Rows[i][ColumnName].ToString();
								break;

							case "10.5K":
								model.Size_105k = ta.Rows[i][ColumnName].ToString();
								break;

							case "11.0":
								model.Size_11 = ta.Rows[i][ColumnName].ToString();
								break;


							case "11.0K":
								model.Size_11k = ta.Rows[i][ColumnName].ToString();
								break;

							case "11.5":
								model.Size_115 = ta.Rows[i][ColumnName].ToString();
								break;

							case "11.5K":
								model.Size_115k = ta.Rows[i][ColumnName].ToString();
								break;

							case "12.0":
								model.Size_12 = ta.Rows[i][ColumnName].ToString();
								break;

							case "12.0K":
								model.Size_12k = ta.Rows[i][ColumnName].ToString();
								break;

							case "12.5":
								model.Size_125 = ta.Rows[i][ColumnName].ToString();
								break;

							case "12.5K":
								model.Size_125k = ta.Rows[i][ColumnName].ToString();
								break;

							case "13.0":
								model.Size_13 = ta.Rows[i][ColumnName].ToString();
								break;

							case "13.0K":
								model.Size_13k = ta.Rows[i][ColumnName].ToString();
								break;

							case "13.5":
								model.Size_135 = ta.Rows[i][ColumnName].ToString();
								break;

							case "14.0":
								model.Size_14 = ta.Rows[i][ColumnName].ToString();
								break;

							case "14.5":
								model.Size_145 = ta.Rows[i][ColumnName].ToString();
								break;


							case "15.0":
								model.Size_15 = ta.Rows[i][ColumnName].ToString();
								break;


							case "15.5":
								model.Size_155 = ta.Rows[i][ColumnName].ToString();
								break;

							case "16.0":
								model.Size_16 = ta.Rows[i][ColumnName].ToString();
								break;

							case "16.5":
								model.Size_165 = ta.Rows[i][ColumnName].ToString();
								break;

							case "17.0":
								model.Size_17 = ta.Rows[i][ColumnName].ToString();
								break;

							case "17.5":
								model.Size_175 = ta.Rows[i][ColumnName].ToString();
								break;

							case "18.0":
								model.Size_18 = ta.Rows[i][ColumnName].ToString();
								break;

							case "18.5":
								model.Size_185 = ta.Rows[i][ColumnName].ToString();
								break;

							case "19.0":
								model.Size_19 = ta.Rows[i][ColumnName].ToString();
								break;

							case "19.5":
								model.Size_195 = ta.Rows[i][ColumnName].ToString();
								break;

							case "20.0":
								model.Size_20 = ta.Rows[i][ColumnName].ToString();
								break;

							case "20.5":
								model.Size_205 = ta.Rows[i][ColumnName].ToString();
								break;

							default:
								break;
						}
					}
					l.Add(model);
                }
            }
            
			if (ListRY.Rows.Count > 0)
            {
				lst_Data_ry = new List<Model_Data_Ry>();

                foreach (System.Data.DataRow r in ListRY.Rows)
                {
					lst_Data_ry.Add(new Model_Data_Ry { Zlbh = r["zlbh"].ToString(), Seq = r["seq"].ToString(), Qty = int.Parse(r["Qty"].ToString()), Xxcc = r["cc"].ToString(), Gxxcc=r["Gxxcc"].ToString() });
                }
            }


			return (l, ta, lst_Data_ry);
        }

		public string CheckSize(string sizeData)
		{
			string size = "";

			if (sizeData == "Size_01k") return size = "01.0K";
			if (sizeData == "Size_015k") return size = "01.5K";
			if (sizeData == "Size_02k") return size = "02.0K";
			if (sizeData == "Size_025k ") return size = "02.5k";
			if (sizeData == "Size_03k") return size = "03.0K";
			if (sizeData == "Size_035k") return size = "03.5K";
			if (sizeData == "Size_04k") return size = "04.0K";
			if (sizeData == "Size_045k") return size = "04.5K";
			if (sizeData == "Size_05k") return size = "05.0K";
			if (sizeData == "Size_055k") return size = "05.5K";

			if (sizeData == "Size_06k") return size = "06.0K";
			if (sizeData == "Size_065k") return size = "06.5K";
			if (sizeData == "Size_07k") return size = "07.0K";
			if (sizeData == "Size_075k") return size = "07.5K";
			if (sizeData == "Size_08k") return size = "08.0K";
			if (sizeData == "Size_085k") return size = "08.5K";
			if (sizeData == "Size_09k") return size = "09.0K";
			if (sizeData == "Size_095k") return size = "09.5K";
			if (sizeData == "Size_10k") return size = "10.0K";
			if (sizeData == "Size_105k") return size = "10.5K";
			if (sizeData == "Size_11k") return size = "11.0K";
			if (sizeData == "Size_115k") return size = "11.5K";
			if (sizeData == "Size_12k") return size = "12.0K";
			if (sizeData == "Size_125k") return size = "12.5K";
			if (sizeData == "Size_13k") return size = "13.0K";
			if (sizeData == "Size_135k") return size = "13.5K";

			if (sizeData == "Size_01") return size = "01.0";
			if (sizeData == "Size_015") return size = "01.5";
			if (sizeData == "Size_02") return size = "02.0";
			if (sizeData == "Size_025") return size = "02.5";
			if (sizeData == "Size_03") return size = "03.0";
			if (sizeData == "Size_035") return size = "03.5";
			if (sizeData == "Size_04") return size = "04.0";
			if (sizeData == "Size_045") return size = "04.5";
			if (sizeData == "Size_05") return size = "05.0";
			if (sizeData == "Size_055") return size = "05.5";
			if (sizeData == "Size_06") return size = "06.0";
			if (sizeData == "Size_065") return size = "06.5";
			if (sizeData == "Size_07") return size = "07.0";
			if (sizeData == "Size_075") return size = "07.5";
			if (sizeData == "Size_08") return size = "08.0";
			if (sizeData == "Size_085") return size = "08.5";
			if (sizeData == "Size_09") return size = "09.0";
			if (sizeData == "Size_095") return size = "09.5";
			if (sizeData == "Size_10") return size = "10.0";
			if (sizeData == "Size_105") return size = "10.5";
			if (sizeData == "Size_11") return size = "11.0";
			if (sizeData == "Size_115") return size = "11.5";
			if (sizeData == "Size_12") return size = "12.0";
			if (sizeData == "Size_125") return size = "12.5";

			if (sizeData == "Size_13") return size = "13.0";
			if (sizeData == "Size_135") return size = "13.5";
			if (sizeData == "Size_14") return size = "14.0";
			if (sizeData == "Size_145") return size = "14.5";
			if (sizeData == "Size_15") return size = "15.0";
			if (sizeData == "ize_155") return size = "15.5";
			if (sizeData == "Size_16") return size = "16.0";
			if (sizeData == "Size_165") return size = "16.5";
			if (sizeData == "Size_17") return size = "17.0";
			if (sizeData == "Size_175") return size = "17.5";
			if (sizeData == "Size_18") return size = "18.0";
			if (sizeData == "Size_185") return size = "18.5";
			if (sizeData == "Size_19") return size = "19.0";
			if (sizeData == "Size_195") return size = "19.5";
			if (sizeData == "Size_20") return size = "20.0";
			if (sizeData == "Size_205") return size = "20.5";


			return size;

		}


	}


    #endregion
	public class Model_Data_Ry
    {
		public string Zlbh { get; set; }
		public string Seq { get; set; }
		public int Qty { get; set; }
		public string Gxxcc { get; set; }
		public string Xxcc{ get; set; }
    }
    public class Model_DataDetail_LYV
    {
		public string Batch { set; get; }
		public string Type { set; get; }		      
		public string Total { set; get; }
		public string Usage{ set; get; }
		
		public int Pages { set; get; }
		public string Size_01k { set; get; }
		public string Size_015k { set; get; }
		public string Size_02k { set; get; }
		public string Size_025k { set; get; }
		public string Size_03k { set; get; }
		public string Size_035k { set; get; }
		public string Size_04k { set; get; }
		public string Size_045k { set; get; }
		public string Size_05k { set; get; }
		public string Size_055k { set; get; }
		public string Size_06k { set; get; }
		public string Size_065k { set; get; }
		public string Size_07k { set; get; }
		public string Size_075k { set; get; }
		public string Size_08k { set; get; }
		public string Size_085k { set; get; }
		public string Size_09k { set; get; }
		public string Size_095k { set; get; }
		public string Size_10k { set; get; }
		public string Size_105k { set; get; }
		public string Size_11k { set; get; }
		public string Size_115k { set; get; }
		public string Size_12k { set; get; }
		public string Size_125k { set; get; }
		public string Size_13k { set; get; }
		public string Size_135k { set; get; }
		public string Size_01 { set; get; }
		public string Size_015 { set; get; }
		public string Size_02 { set; get; }
		public string Size_025 { set; get; }
		public string Size_03 { set; get; }
		public string Size_035 { set; get; }
		public string Size_04 { set; get; }
		public string Size_045 { set; get; }
		public string Size_05 { set; get; }
		public string Size_055 { set; get; }
		public string Size_06 { set; get; }
		public string Size_065 { set; get; }
		public string Size_07 { set; get; }
		public string Size_075 { set; get; }
		public string Size_08 { set; get; }
		public string Size_085 { set; get; }
		public string Size_09 { set; get; }
		public string Size_095 { set; get; }
		public string Size_10 { set; get; }
		public string Size_105 { set; get; }
		public string Size_11 { set; get; }
		public string Size_115 { set; get; }
		public string Size_12 { set; get; }
		public string Size_125 { set; get; }

		public string Size_13 { set; get; }
		public string Size_135 { set; get; }
		public string Size_14 { set; get; }
		public string Size_145 { set; get; }
		public string Size_15 { set; get; }
		public string Size_155 { set; get; }
		public string Size_16 { set; get; }
		public string Size_165 { set; get; }
		public string Size_17 { set; get; }
		public string Size_175 { set; get; }
		public string Size_18 { set; get; }
		public string Size_185 { set; get; }
		public string Size_19 { set; get; }
		public string Size_195 { set; get; }
		public string Size_20 { set; get; }
		public string Size_205 { set; get; }
	
	}
	public class Model_ManySite_LYV
	{
		public string XXCC { get; set; }
		public string GXXCC { get; set; }
		public int GCount{ get; set; }

    }
	public class Model_GrpSize_LYV
    {
		public string GXXCC { get; set; }
		public int Count { set; get; }

	}
}