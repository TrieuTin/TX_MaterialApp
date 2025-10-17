using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MaterialTracking.UsersControl
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DataGridView : Grid
	{
		System.Data.DataTable ta;
		public DataGridView (System.Data.DataTable table)
		{
			InitializeComponent ();

			ta = table;
		}

		void SetUpTitle()
        {
            for (int i = 0; i < ta.Columns.Count; i++)
            {
				var col = new ColumnDefinition
				{
					Width = new GridLength(122, GridUnitType.Star)
				};

				GridTitle.ColumnDefinitions.Add(col);

				GridData.ColumnDefinitions.Add(col);

				Frame frame = new Frame
				{
					BackgroundColor = Color.FromHex("#234482"),
					Content = new Label
					{
						FontSize = 20 ,

						TextColor = Color.White,

						Text =ta.Columns[i].ColumnName

                    }
				};

				GridTitle.Children.Add(frame, i, 0);
			}
        }
		void SetData()
		{
			
			for (int i = 0; i < ta.Rows.Count; i++)
			{

				var row = new RowDefinition
				{
					Height = new GridLength(122, GridUnitType.Star)
				};
				
				GridData.RowDefinitions.Add(row);

                for (int j = 0; j < ta.Columns.Count; j++)
                {
					Frame fr = new Frame
					{
						Content = new Label
						{
							Text = ta.Rows[i][j].ToString()
						}
					};
					GridData.Children.Add(fr, j, i);
                }

			}
		}

	}

}