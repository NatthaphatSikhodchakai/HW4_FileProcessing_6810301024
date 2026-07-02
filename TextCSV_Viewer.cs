/*
MIT License

Copyright (c) 2026 Sarayut Chaisuriya

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

Note on dataset:
The included MalwareBazaar sample CSV has been modified:
- Limited to first 500 rows
- Header format adjusted for teaching purposes
See README.md for full details.
*/
using System;
using System.IO;
using System.Windows.Forms;

namespace FileProcessing
{
	public partial class frmTextView : Form
	{
		/// <summary>
		/// Initializes a new instance of the frmTextView class.
		/// </summary>
		public frmTextView()
		{
			InitializeComponent();
		}
		/// <summary>
		/// Handles the Click event of the Read button by loading the contents of the specified file into the display area.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The event data.</param>
		private void btRead_Click(object sender, EventArgs e)
		{			
            string content = File.ReadAllText(tbFileName.Text);
            rtbShow.Text = content;
		}
        /// <summary>
        /// Handles the Click event of the btReadCSV button, reading CSV data from the specified file and populating the
        /// DataGridView with its contents.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
		private void btReadCSV_Click(object sender, EventArgs e)
        {
            dgvData.Rows.Clear();
            dgvData.Columns.Clear();
            dgvData.AllowUserToAddRows = false;

            int startRow = 1;
            int endRow = int.MaxValue;

            if (!string.IsNullOrWhiteSpace(txtStartRow.Text))
            {
                if (!int.TryParse(txtStartRow.Text.Trim(), out startRow))
                {
                    MessageBox.Show("Start row must be a number.");
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtEndRow.Text))
            {
                if (!int.TryParse(txtEndRow.Text.Trim(), out endRow))
                {
                    MessageBox.Show("End row must be a number.");
                    return;
                }
            }

            if (startRow < 1)
            {
                MessageBox.Show("Start row must be greater than or equal to 1.");
                return;
            }

            if (endRow < startRow)
            {
                MessageBox.Show("End row must be greater than or equal to Start row.");
                return;
            }

            string fileTypeFilter = txtFileType.Text.Trim().Trim('"').ToLower();

            using (StreamReader srReader = new StreamReader(tbFileName.Text))
            {
                string strLine;
                string[] strHeaders_arr = null;
                bool bHeaderRead = false;
                int currentRecord = 0;

                while ((strLine = srReader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(strLine))
                    {
                        continue;
                    }

                    if (strLine.StartsWith("#"))
                    {
                        if (strLine.Length > 8 && strLine.Substring(0, 8).Equals("#HEADER"))
                        {
                            strHeaders_arr = strLine.Substring(8).Split(',');
                        }

                        continue;
                    }

                    string[] strValues_arr = strLine.Split(',');

                    currentRecord++;

                    if (currentRecord < startRow)
                    {
                        continue;
                    }

                    if (currentRecord > endRow)
                    {
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(fileTypeFilter))
                    {
                        bool matchedFileType = false;

                        foreach (string value in strValues_arr)
                        {
                            string cleanValue = value.Trim().Trim('"').ToLower();

                            if (cleanValue == fileTypeFilter)
                            {
                                matchedFileType = true;
                                break;
                            }
                        }

                        if (!matchedFileType)
                        {
                            continue;
                        }
                    }

                    if (!bHeaderRead)
                    {
                        int columnCount = strValues_arr.Length;

                        if (strHeaders_arr != null && strHeaders_arr.Length > columnCount)
                        {
                            columnCount = strHeaders_arr.Length;
                        }

                        for (int i = 0; i < columnCount; i++)
                        {
                            string columnName;

                            if (strHeaders_arr != null && i < strHeaders_arr.Length && !string.IsNullOrWhiteSpace(strHeaders_arr[i]))
                            {
                                columnName = strHeaders_arr[i].Trim().Trim('"');
                            }
                            else
                            {
                                columnName = "Column" + (i + 1);
                            }

                            if (dgvData.Columns.Contains(columnName))
                            {
                                columnName = columnName + "_" + i;
                            }

                            dgvData.Columns.Add(columnName, columnName);
                        }

                        bHeaderRead = true;
                    }

                    while (dgvData.Columns.Count < strValues_arr.Length)
                    {
                        string columnName = "Column" + (dgvData.Columns.Count + 1);
                        dgvData.Columns.Add(columnName, columnName);
                    }

                    dgvData.Rows.Add(strValues_arr);
                }
            }

            MessageBox.Show("Loaded " + dgvData.Rows.Count + " rows.");
        }
        /// <summary>
        /// Handles the Click event of the Browse button, allowing the user to select a file and displaying its path in the
        /// file name text box.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void btBrowse_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog ofd = new OpenFileDialog())
			{
				ofd.Filter = "Text Files (*.txt)|*.txt|CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
				if (ofd.ShowDialog() == DialogResult.OK)
				{
					tbFileName.Text = ofd.FileName;
				}
			}
		}

        private void rtbShow_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtStartRow_TextChanged(object sender, EventArgs e)
        {

        }
    }   // End of frmTextView class
}
