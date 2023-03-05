//----------------------------------------------
//
//         Copyright © 2014  Illogika
//----------------------------------------------

using System.Linq;
using System.Collections.Generic;

namespace BW.Inspector
{

	public class Spreadsheet : System.Object
	{

		protected List<List<string>> spreadsheet = new List<List<string>>();

		public List<string> GetRow(int row)
		{
			return this.spreadsheet[row];
		}

		public List<string> GetRow(string row)
		{
			return (from r in this.spreadsheet where r[0] == row select r).FirstOrDefault();
		}

		public string GetValue(int row, int column)
		{
			try
			{
				return this.spreadsheet[row][column];
			}
			catch
			{
				return "";
			}
		}

		public string GetValue(int row, string column)
		{
			return this.spreadsheet[row][this.spreadsheet[0].IndexOf(column)];
		}

		public string GetValue(string row, int column)
		{
			try
			{
				return (from r in this.spreadsheet where r[0] == row select r).FirstOrDefault()[column];
			}
			catch
			{
				return "";
			}
		}

		public string GetValue(string row, string column)
		{
			return (from r in this.spreadsheet where r[0] == row select r).FirstOrDefault()[this.spreadsheet[0].IndexOf(column)];
		}

		public string GetVariableName(int column)
		{
			string variableName = this.spreadsheet[0][column].Split('_')[0];
			return variableName;
		}

		public int GetIndexOf(string column)
		{
			int index = this.spreadsheet[0].IndexOf(column);
			if (index == -1) throw new System.Exception(string.Format("Colum {0} was not found", column));
			return index;
		}

		public int GetNbRows()
		{
			return this.spreadsheet.Count;
		}

		public int GetNbColumns()
		{
			return this.spreadsheet[0].Count;
		}

		public void Load(string content)
		{
			this.spreadsheet.Clear();
			string[] lines = content.Split('\n');
			foreach (string line in lines)
			{
				this.spreadsheet.Add(line.Split('	').ToList());
			}

			foreach (List<string> line in this.spreadsheet)
			{
				while (line.Count < this.spreadsheet[0].Count)
				{
					line.Add("");
				}
			}
		}
	}

}
